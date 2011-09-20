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
    /// \param _animClip the referenced animation clip
    /// Constructor of exSpriteAnimState, it will copy the settings from _animClip. 
    // ------------------------------------------------------------------ 

    public exSpriteAnimState ( exSpriteAnimClip _animClip ) {
        clip = _animClip;
        name = _animClip.name;
        wrapMode = _animClip.wrapMode;
        stopAction = _animClip.stopAction;
        length = _animClip.length;
        speed = _animClip.speed;

        frameTimes = new List<float>(_animClip.frameInfos.Count);
        float tmp = 0.0f;
        foreach ( exSpriteAnimClip.FrameInfo fi in _animClip.frameInfos ) {
            tmp += fi.length;
            frameTimes.Add(tmp);
        }
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
    private bool paused = false;
    private exAtlas defaultAtlas;
    private int defaultIndex;

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
            foreach ( exSpriteAnimClip clip in animations ) {
                exSpriteAnimState state = new exSpriteAnimState(clip);
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
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Start () {
        if ( playAutomatically && defaultAnimation != null ) {
            Play (defaultAnimation.name);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Update () {
        if ( !paused && playing && (curAnimation != null) ) {
            // advance the time and check if we trigger any animation events
            float delta = Time.deltaTime * curAnimation.speed;
            float curTime = curAnimation.time;

            // advance the time
            curAnimation.time += delta;
            curAnimation.clip.TriggerEvents( gameObject, 
                                             curTime,
                                             delta,
                                             curAnimation.wrapMode );

            // set sprite to current time
            exSpriteAnimClip.FrameInfo fi = GetCurFrameInfo();
            if ( fi != null )
                sprite.SetSprite ( fi.atlas, fi.index );

            // check if stop
            if ( ( curAnimation.wrapMode == WrapMode.Once ||
                   curAnimation.wrapMode == WrapMode.Default ) && 
                 curAnimation.time >= curAnimation.length )
            {
                Stop();
            }
        }
    }

    // NOTE: the reason I design to Play instead of using default parameter is because in 
    // Unity Animation Editor, it can send message to function that only have one parameter.

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
                curAnimation.time = curAnimation.frameTimes[_index];
            playing = true;
            paused = false;
        }
    }

    // ------------------------------------------------------------------ 
    /// \param _name the name of the animation
    /// \param _index the frame index
    /// Get the texture used in the _index of frames in the animation by _name,   
    /// and set it as the default picture of the sprite
    // ------------------------------------------------------------------ 

    public void SetFrame ( string _name, int _index ) {
        curAnimation = GetAnimation(_name);
        if ( curAnimation != null &&
             _index >= 0 &&
             _index < curAnimation.clip.frameInfos.Count ) 
        {
            exSpriteAnimClip.FrameInfo fi = curAnimation.clip.frameInfos[_index]; 
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
            paused = false;

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
    }

    // ------------------------------------------------------------------ 
    /// Pause the playing animation
    // ------------------------------------------------------------------ 

    public void Pause () {
        paused = true;
    }

    // ------------------------------------------------------------------ 
    /// Resume the paused animation
    // ------------------------------------------------------------------ 

    public void Resume () {
        paused = false;
    }

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
            return ( playing && curAnimation.name == _name );
    }

    // ------------------------------------------------------------------ 
    /// \param _name the name of the animation
    /// \return the boolean result
    /// Check if the _name of the animation is the current paused animation.
    /// If the _name is empty, it will check if there is animation paused now.
    // ------------------------------------------------------------------ 

    public bool IsPaused ( string _name = "" ) {
        if ( string.IsNullOrEmpty(_name) )
            return paused;
        else
            return (paused && curAnimation.name == _name);
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
        return nameToState[_name];
    }

    // ------------------------------------------------------------------ 
    /// \return the frame info
    /// Get the information of current frame in the playing animation.
    // ------------------------------------------------------------------ 

    public exSpriteAnimClip.FrameInfo GetCurFrameInfo () {
        if ( curAnimation != null ) {
            float wrappedTime = curAnimation.clip.WrapSeconds(curAnimation.time, curAnimation.wrapMode);
            int index = curAnimation.frameTimes.BinarySearch(wrappedTime);
            if ( index < 0 ) {
                index = ~index;
            }
            if ( index < curAnimation.clip.frameInfos.Count )
                return curAnimation.clip.frameInfos[index];
        }
        return null;
    }

    // ------------------------------------------------------------------ 
    /// \param _animClip the sprite animation clip wants to add
    /// \return the instantiate animation state of the added _animClip 
    /// Add a sprite animation clip, create a new animation state and saves 
    /// it to the lookup table by the name of the clip
    /// 
    /// \note if the animation already in the exSpriteAnimation.animations, 
    /// it will do nothing
    // ------------------------------------------------------------------ 

    public exSpriteAnimState AddAnimation ( exSpriteAnimClip _animClip ) {
        // if we already have the animation, just return the animation state
        if ( animations.IndexOf(_animClip) != -1 ) {
            return nameToState[_animClip.name];
        }

        //
        animations.Add (_animClip);
        exSpriteAnimState state = new exSpriteAnimState(_animClip);
        nameToState[state.name] = state;
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

