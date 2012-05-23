// ======================================================================================
// File         : exSpriteAnimation.cs
// Author       : Wu Jie 
// Last Change  : 08/06/2011 | 21:32:18 PM | Saturday,August
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
///
/// The state of the exSpriteAnimClip, a state can be treat as an instance. In 
/// ex2D, when you play the sprite animation, it will load and reference the data
/// in exSpriteAnimClip. But you can't just save your state in the clip, because 
/// it is possible the other gameObject play the same animation in the same time. 
/// 
/// The exSpriteAnimState is designed to solve the problems, it initialized self by  
/// copy the settings in exSpriteAnimClip, and provide identity state for user in 
/// exSpriteAnimation component.
///
///////////////////////////////////////////////////////////////////////////////

[System.Serializable]
public class exSpriteAnimState {

    [System.NonSerialized] public exSpriteAnimClip clip; ///< the referenced sprite animation clip
    [System.NonSerialized] public string name; ///< the name of the sprite animation state
    [System.NonSerialized] public WrapMode wrapMode; ///< the wrap mode
    [System.NonSerialized] public exSpriteAnimClip.StopAction stopAction; ///< the stop action
    [System.NonSerialized] public float length; ///< the length of the sprite animation in seconds with speed = 1.0f

    [System.NonSerialized] public float speed = 1.0f; ///< the speed to play the sprite animation clip
    [System.NonSerialized] public float time = 0.0f; ///< the current time in seoncds
    // [System.NonSerialized] public float normalizedTime = 0.0f;
    [System.NonSerialized] public List<float> frameTimes; ///< the list of the start time in seconds of each frame in the exSpriteAnimClip

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Init ( string _name, exSpriteAnimClip _animClip ) {
        clip = _animClip;
        name = _name;
        wrapMode = _animClip.wrapMode;
        stopAction = _animClip.stopAction;
        length = _animClip.length;
        speed = _animClip.speed;

        frameTimes = new List<float>(_animClip.frameInfos.Count);
        float tmp = 0.0f;
        for ( int i = 0; i < _animClip.frameInfos.Count; ++i ) {
            tmp += _animClip.frameInfos[i].length;
            frameTimes.Add(tmp);
        }
    }

    // ------------------------------------------------------------------ 
    /// \param _animClip the referenced animation clip
    /// Constructor of exSpriteAnimState, it will copy the settings from _animClip. 
    // ------------------------------------------------------------------ 

    public exSpriteAnimState ( exSpriteAnimClip _animClip ) {
        Init ( _animClip.name, _animClip );
    }

    // ------------------------------------------------------------------ 
    /// \param _name the name of the animation state
    /// \param _animClip the referenced animation clip
    /// Constructor of exSpriteAnimState, it will copy the settings from _animClip. 
    // ------------------------------------------------------------------ 

    public exSpriteAnimState ( string _name, exSpriteAnimClip _animClip ) {
        Init ( _name, _animClip );
    }
}

///////////////////////////////////////////////////////////////////////////////
/// 
/// A component to help user manipulate exSpriteAnimClip in a sprite 
/// 
///////////////////////////////////////////////////////////////////////////////

[RequireComponent (typeof(exSprite))]
[RequireComponent (typeof(MeshRenderer))]
[RequireComponent (typeof(MeshFilter))]
[AddComponentMenu("ex2D Sprite/Sprite Animation")]
public class exSpriteAnimation : MonoBehaviour {

    // ------------------------------------------------------------------ 
    /// the list of sprite animation clips used in the component
    // ------------------------------------------------------------------ 

    public List<exSpriteAnimClip> animations = new List<exSpriteAnimClip>();

    // ------------------------------------------------------------------ 
    /// the default sprite animation clip
    // ------------------------------------------------------------------ 

    public exSpriteAnimClip defaultAnimation;

    // ------------------------------------------------------------------ 
    /// When playAutomatically set to true, it will play the 
    /// exSpriteAnimation.defaultAnimation at the start
    // ------------------------------------------------------------------ 

    public bool playAutomatically = false;

    ///////////////////////////////////////////////////////////////////////////////
    // private
    ///////////////////////////////////////////////////////////////////////////////

    private bool initialized = false;
    private Dictionary<string,exSpriteAnimState> nameToState;
    private exSpriteAnimState curAnimation;
    private exSprite sprite;
    private bool playing = false;
    private exAtlas defaultAtlas;
    private int defaultIndex;
    private int lastEventInfoIndex = -1;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Init () {
        if ( initialized == false ) {
            initialized = true;

            sprite = GetComponent<exSprite>();
            defaultAtlas = sprite.atlas;
            defaultIndex = sprite.index;

            nameToState = new Dictionary<string,exSpriteAnimState> ();
            for ( int i = 0; i < animations.Count; ++i ) {
                exSpriteAnimState state = new exSpriteAnimState(animations[i]);
                nameToState[state.name] = state;
            }

            if ( defaultAnimation != null )
                curAnimation = nameToState[defaultAnimation.name];
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Awake () {
        // DEBUG { 
        // Debug.Log("exSpriteAnimation:Awake()");
        // } DEBUG end 

        Init ();

        // NOTE: start will only trigger when the Component first run after enabled
        if ( playAutomatically && defaultAnimation != null ) {
            Play (defaultAnimation.name);
        }
        else {
            enabled = false;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Update () {
        if ( playing && (curAnimation != null) ) {
            // advance the time and check if we trigger any animation events
            float delta = Time.deltaTime * curAnimation.speed;
            float curTime = curAnimation.time;

            // advance the time
            curAnimation.time += delta;

            // save the last state
            float lastAnimTime = curAnimation.time;
            exSpriteAnimState lastAnimation = curAnimation;

            //
            int newIdx = curAnimation.clip.TriggerEvents( this, 
                                                          lastAnimation,
                                                          lastEventInfoIndex,
                                                          curTime,
                                                          delta,
                                                          curAnimation.wrapMode );

            // NOTE: it is possible in the events, user destroy this component. In this case, 
            //       the curAnimation will be null.
            if ( curAnimation == null || 
                 curAnimation != lastAnimation || 
                 lastAnimTime != curAnimation.time /*NOTE: it is possible in the event we reply the same animation*/ ) {
                return;
            }
            lastEventInfoIndex = newIdx;

            // set sprite to current time
            exSpriteAnimClip.FrameInfo fi = GetCurFrameInfo();
            if ( fi != null )
                sprite.SetSprite ( fi.atlas, fi.index );

            // check if stop
            if ( curAnimation.wrapMode == WrapMode.Once ||
                 curAnimation.wrapMode == WrapMode.Default )
            {
                if ( (curAnimation.speed > 0.0f && curAnimation.time >= curAnimation.length) ||
                     (curAnimation.speed < 0.0f && curAnimation.time <= 0.0f) )
                {
                    Stop();
                }
            }
        }
    }

    // ------------------------------------------------------------------ 
    /// reset to default sprite
    // ------------------------------------------------------------------ 

    public void SetDefaultSprite () {
        sprite.SetSprite( defaultAtlas, defaultIndex );
    }

    // ------------------------------------------------------------------ 
    /// update the default sprite if we dynamically change it in the game
    // ------------------------------------------------------------------ 

    public void UpdateDefaultSprite ( exAtlas _atlas, int _index ) {
        defaultAtlas = _atlas;
        defaultIndex = _index;
    }

    // NOTE: the reason I design to Play instead of using default parameter is because in 
    // Unity Animation Editor, it can send message to function that only have one parameter.

    // ------------------------------------------------------------------ 
    /// Play the default animation by _name 
    // ------------------------------------------------------------------ 

    public void PlayDefault () {
        if ( defaultAnimation )
            Play( defaultAnimation.name, 0 );
    }

    // ------------------------------------------------------------------ 
    /// \param _name the name of the animation to play
    /// Play the animation by _name 
    // ------------------------------------------------------------------ 

    public void Play ( string _name ) {
        Play( _name, 0 );
    }

    // ------------------------------------------------------------------ 
    /// \param _name the name of the animation to play
    /// \param _index the frame index
    /// Play the animation by _name, start from the _index of frame  
    // ------------------------------------------------------------------ 

    public void Play ( string _name, int _index ) {
        curAnimation = GetAnimation(_name);
        if ( curAnimation != null ) {
            if ( _index == 0 )
                curAnimation.time = 0.0f;
            else if ( _index > 0 && _index < curAnimation.frameTimes.Count )
                curAnimation.time = curAnimation.frameTimes[_index-1];
            playing = true;
            if ( curAnimation.speed >= 0.0f )
                lastEventInfoIndex = -1;
            else
                lastEventInfoIndex = curAnimation.clip.eventInfos.Count;

            exSpriteAnimClip.FrameInfo fi = curAnimation.clip.frameInfos[_index]; 
            if ( fi != null )
                sprite.SetSprite ( fi.atlas, fi.index );
            enabled = true;
        }
    }

    // ------------------------------------------------------------------ 
    /// \param _name the name of the animation
    /// \param _index the frame index
    /// Get the texture used in the _index of frames in the animation by _name,   
    /// and set it as the default picture of the sprite
    // ------------------------------------------------------------------ 

    public void SetFrame ( string _name, int _index ) {
        exSpriteAnimState animState = GetAnimation(_name);
        if ( animState != null &&
             _index >= 0 &&
             _index < animState.clip.frameInfos.Count ) 
        {
            exSpriteAnimClip.FrameInfo fi = animState.clip.frameInfos[_index]; 
            sprite.SetSprite ( fi.atlas, fi.index );
        }
    }

    // ------------------------------------------------------------------ 
    /// Stop the playing animation, take the action that setup in the 
    /// exSpriteAnimState.stopAction 
    // ------------------------------------------------------------------ 

    public void Stop () {
        if ( curAnimation != null ) {
            //
            exSpriteAnimClip.StopAction stopAction = curAnimation.stopAction; 

            //
            curAnimation.time = 0.0f;
            curAnimation = null;
            playing = false;

            //
            switch ( stopAction ) {
            case exSpriteAnimClip.StopAction.DoNothing:
                // Nothing todo;
                break;

            case exSpriteAnimClip.StopAction.DefaultSprite:
                sprite.SetSprite( defaultAtlas, defaultIndex );
                break;

            case exSpriteAnimClip.StopAction.Hide:
                sprite.enabled = false;
                break;

            case exSpriteAnimClip.StopAction.Destroy:
                GameObject.Destroy(gameObject);
                break;
            }
        }
        enabled = false;
    }

    // DELME { 
    // ------------------------------------------------------------------ 
    /// Pause the playing animation
    // ------------------------------------------------------------------ 

    public void Pause () {
        enabled = false;
    }

    // ------------------------------------------------------------------ 
    /// Resume the paused animation
    // ------------------------------------------------------------------ 

    public void Resume () {
        enabled = true;
    }
    // } DELME end 

    // ------------------------------------------------------------------ 
    /// \param _name the name of the animation
    /// \return the boolean result
    /// Check if the _name of the animation is the current playing animation.
    /// If the _name is empty, it will check if there is animation playing now.
    // ------------------------------------------------------------------ 

    public bool IsPlaying ( string _name = "" ) {
        if ( string.IsNullOrEmpty(_name) )
            return playing;
        else
            return ( playing && curAnimation != null && curAnimation.name == _name );
    }

    // ------------------------------------------------------------------ 
    /// \param _name the name of the animation
    /// \return the boolean result
    /// Check if the _name of the animation is the current paused animation.
    /// If the _name is empty, it will check if there is animation paused now.
    // ------------------------------------------------------------------ 

    public bool IsPaused ( string _name = "" ) {
        if ( string.IsNullOrEmpty(_name) )
            return (enabled == false);
        else
            return (enabled == false && curAnimation != null && curAnimation.name == _name);
    }

    // ------------------------------------------------------------------ 
    /// \param _name the name of the animation
    /// \return the animation state
    /// Get the animation state by _name
    // ------------------------------------------------------------------ 

    public exSpriteAnimState GetAnimation ( string _name ) {
        Init ();
        // DISABLE { 
        // if ( nameToState == null ) {
        //     Debug.LogError ("The exSpriteAnimation is not Awake yet. Please put it before Default Time in Menu/Edit/Project Settings/Script Execution Order");
        //     return null;
        // }
        // } DISABLE end 
        if ( nameToState.ContainsKey(_name) )
            return nameToState[_name];
        return null;
    }

    // ------------------------------------------------------------------ 
    /// \return the current animation state
    /// Get the current playing animation state
    // ------------------------------------------------------------------ 

    public exSpriteAnimState GetCurrentAnimation () { return curAnimation; }

    // DISABLE { 
    // // ------------------------------------------------------------------ 
    // // \return the last event info index
    // // Get the last event info index
    // // ------------------------------------------------------------------ 

    // public int GetLastEventInfoIndex () { return lastEventInfoIndex; }
    // } DISABLE end 

    // ------------------------------------------------------------------ 
    /// \return the frame info
    /// Get the information of current frame in the playing animation.
    // ------------------------------------------------------------------ 

    public exSpriteAnimClip.FrameInfo GetCurFrameInfo () {
        if ( curAnimation != null ) {
            float wrappedTime = curAnimation.clip.WrapSeconds(curAnimation.time, curAnimation.wrapMode);
#if UNITY_FLASH
            int index = curAnimation.frameTimes.Count - 1;
            for ( int i = 0; i < curAnimation.frameTimes.Count; ++i ) {
                if ( curAnimation.frameTimes[i] > wrappedTime ) {
                    index = i;
                    break;
                }
            }
#else
            int index = curAnimation.frameTimes.BinarySearch(wrappedTime);
#endif
            if ( index < 0 ) {
                index = ~index;
            }
            if ( index < curAnimation.clip.frameInfos.Count )
                return curAnimation.clip.frameInfos[index];
        }
        return null;
    }

    // ------------------------------------------------------------------ 
    /// \return the frame index
    /// Get the index of current frame in the playing animation.
    // ------------------------------------------------------------------ 

    public int GetCurFrameIndex () {
        int index = -1;
        if ( curAnimation != null ) {
            float wrappedTime = curAnimation.clip.WrapSeconds(curAnimation.time, curAnimation.wrapMode);
#if UNITY_FLASH
            index = curAnimation.frameTimes.Count - 1;
            for ( int i = 0; i < curAnimation.frameTimes.Count; ++i ) {
                if ( curAnimation.frameTimes[i] > wrappedTime ) {
                    index = i;
                    break;
                }
            }
#else
            index = curAnimation.frameTimes.BinarySearch(wrappedTime);
            if ( index < 0 ) {
                index = ~index;
            }
#endif
        }
        return index;
    }

    // ------------------------------------------------------------------ 
    /// \param _animClip the sprite animation clip wants to add
    /// \return the instantiate animation state of the added _animClip 
    /// Add a sprite animation clip, create a new animation state and saves 
    /// it to the lookup table by the name of the clip
    /// 
    /// \note if the animation already in the exSpriteAnimation.animations, 
    /// it will override the old clip and return a new animation state.
    // ------------------------------------------------------------------ 

    public exSpriteAnimState AddAnimation ( exSpriteAnimClip _animClip ) {
        return AddAnimation ( _animClip.name, _animClip );
    }

    // ------------------------------------------------------------------ 
    /// \param _name the name of animation state you want to add
    /// \param _animClip the sprite animation clip wants to add
    /// \return the instantiate animation state of the added _animClip 
    /// Add a sprite animation clip, create a new animation state and saves 
    /// it to the lookup table by the name of the clip
    /// 
    /// \note if the animation already in the exSpriteAnimation.animations, 
    /// it will override the old clip and return a new animation state.
    // ------------------------------------------------------------------ 

    public exSpriteAnimState AddAnimation ( string _name, exSpriteAnimClip _animClip ) {
        Init ();
        exSpriteAnimState state = null;

        // if we already have the animation, just return the animation state
        if ( animations.IndexOf(_animClip) != -1 ) {
            state = nameToState[_name];
            if ( state.clip != _animClip ) {
                state = new exSpriteAnimState( _name, _animClip );
                nameToState[_name] = state;
            }
            return state;
        }

        //
        animations.Add (_animClip);
        state = new exSpriteAnimState( _name, _animClip );
        nameToState[_name] = state;
        return state;
    }

    // ------------------------------------------------------------------ 
    /// \param _animClip the sprite animation clip wants to remove
    /// Remove a sprite animation clip from exSpriteAnimation.animations, 
    // ------------------------------------------------------------------ 

    public void RemoveAnimation ( exSpriteAnimClip _animClip ) {
        // if we already have the animation, just return the animation state
        if ( animations.IndexOf(_animClip) == -1 ) {
            return; 
        }

        //
        animations.Remove (_animClip);
        nameToState.Remove (_animClip.name);
    }
}

