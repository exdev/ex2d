// ======================================================================================
// File         : exSpriteAnimClip.cs
// Author       : Wu Jie 
// Last Change  : 09/03/2011 | 18:29:15 PM | Saturday,September
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
/// The sprite animation clip asset used in exSpriteAnimation component. 
///
///////////////////////////////////////////////////////////////////////////////

public class exSpriteAnimClip : ScriptableObject {

    public class EventInfoComparer: IComparer<EventInfo> {
        public int Compare( EventInfo _x, EventInfo _y ) {
            if ( _x.time > _y.time )
                return 1;
            else if ( _x.time == _y.time  )
                return 0;
            else
                return -1;
        }
    }

    // ------------------------------------------------------------------ 
    /// The action type used when animation stpped
    // ------------------------------------------------------------------ 

    public enum StopAction {
        DoNothing,      ///< do nothing
        DefaultSprite,  ///< set to default sprite when the sprite animation stopped
        Hide,           ///< hide the sprite when the sprite animation stopped
        Destroy         ///< destroy the GameObject the sprite belongs to when the sprite animation stopped
    }

    // ------------------------------------------------------------------ 
    /// The structure to descrip a frame in the sprite animation clip
    // ------------------------------------------------------------------ 

    [System.Serializable]
    public class FrameInfo {
        public string textureGUID = ""; ///< the guid of the referenced texture
        public float length = 0.0f;     ///< the length of the frame in seconds
        public exAtlas atlas;           ///< the atlas used in this frame
        public int index;               ///< the index of the atlas used in this frame
    }

    // ------------------------------------------------------------------ 
    /// The structure to descrip an event in the sprite animation clip
    // ------------------------------------------------------------------ 

    [System.Serializable]
    public class EventInfo {
        // ------------------------------------------------------------------ 
        /// the type of the parameter
        // ------------------------------------------------------------------ 

        public enum ParamType {
            NONE,   ///< none
            STRING, ///< string
            FLOAT,  ///< float
            INT,    ///< int
            BOOL,   ///< bool
            OBJECT  ///< object
        }

        public float time = 0.0f; ///< the time in seconds to trigger the event
        public string methodName = ""; ///< the name of method to invoke 
        public ParamType paramType = ParamType.NONE; ///< the first parameter type 
        public string stringParam = ""; ///< the value of the string parameter
        public float floatParam = 0.0f; ///< the value of the float parameter
        public int intParam = -1; ///< the value of the int parameter
        public bool boolParam = false; ///< the value of the boolean parameter
        public Object objectParam = null; ///< the value of the object parameter
        public SendMessageOptions msgOptions = SendMessageOptions.RequireReceiver; ///< the SendMessage option
    }

    public WrapMode wrapMode = WrapMode.Once; ///< default wrap mode
    public StopAction stopAction = StopAction.DoNothing; ///< the default type of action used when the animation stopped 
    public float length = 1.0f; ///< the length of the animation clip in seconds

    // ------------------------------------------------------------------ 
    [SerializeField] protected float sampleRate_ = 60.0f;
    /// the sample rate used in this animation clip
    // ------------------------------------------------------------------ 

    public float sampleRate {
        get { return sampleRate_; }
        set {
            if ( value != sampleRate_ ) {
                sampleRate_ = Mathf.RoundToInt(Mathf.Max(value,1.0f)); 
            }
        }
    }

    // DELME { 
    // // ------------------------------------------------------------------ 
    // [SerializeField] protected float length_ = 1.0f;
    // /// the length of the animation clip in seconds
    // // ------------------------------------------------------------------ 

    // public float length {
    //     get { return length_; }
    //     set {  
    //         if ( value != 0.0f && Mathf.Approximately (value, length_) == false ) {
    //             float totalLength = 0.0f;
    //             float delta = value - length_;
    //             foreach ( exSpriteAnimClip.FrameInfo fi in frameInfos) {
    //                 float ratio = fi.length/length_;
    //                 fi.length = Mathf.Max(1.0f/60.0f, fi.length + delta * ratio);
    //                 totalLength += fi.length;
    //             }
    //             length_ = totalLength;
    //             foreach ( exSpriteAnimClip.EventInfo ei in eventInfos) {
    //                 ei.time = ei.time/length_ * length_;
    //             }
    //         }
    //     }
    // }
    // } DELME end 

    public List<FrameInfo> frameInfos = new List<FrameInfo>(); ///< the list of frame info 
    public List<EventInfo> eventInfos = new List<EventInfo>(); ///< the list of event info
    public float speed = 1.0f; ///< the default speed of the animation clip

    // editor only
    public float editorScale = 1.0f; ///< the scale used in editor timeline view
    public float editorOffset = 0.0f; ///< the offset used in editor timeline view
    public float editorSpeed = 1.0f; ///< the preview speed in sprite animation editor 
    public bool editorNeedRebuild = false; ///< check if the sprite animation clip need rebuild

    private EventInfoComparer eventInfoComparer = new EventInfoComparer();
    private EventInfo tmpEventInfo = new EventInfo();

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    /// \param _e the event information wants to add
    /// add an event information
    // ------------------------------------------------------------------ 

    public void AddEvent ( EventInfo _e ) {
        //
        int index = eventInfos.BinarySearch( _e, eventInfoComparer );
        if ( index < 0 ) {
            index = ~index;
        }

        eventInfos.Insert( index, _e );
    }

    // ------------------------------------------------------------------ 
    /// \param _e the event information wants to remove
    /// remove an event information
    // ------------------------------------------------------------------ 

    public void RemoveEvent ( EventInfo _e ) {
        eventInfos.Remove( _e );
    }

    // ------------------------------------------------------------------ 
    /// \param _seconds the in seconds used for wrapping
    /// \param _wrapMode the wrap mode used for wrapping
    /// wrap the seconds of the anim clip by the wrap mode
    // ------------------------------------------------------------------ 

    public float WrapSeconds ( float _seconds, WrapMode _wrapMode ) {
        float t = Mathf.Abs(_seconds);
        if ( _wrapMode == WrapMode.Loop ) {
            t %= length;
        }
        else if ( _wrapMode == WrapMode.PingPong ) {
            int cnt = (int)(t/length);
            t %= length;
            if ( cnt % 2 == 1 ) {
                t = length - t;
            }
        }
        else {
            t = Mathf.Clamp( t, 0.0f, length );
        }
        return t;
    }

    // ------------------------------------------------------------------ 
    /// \param _gameObject send message to target _gameObject
    /// \param _start the start time in seconds 
    /// \param _delta the delta time in seconds
    /// \param _wrapMode  the wrap mode
    /// Trigger events locate between the start and start+_delta time span
    // ------------------------------------------------------------------ 

    public void TriggerEvents ( GameObject _gameObject, 
                                float _start, 
                                float _delta, 
                                WrapMode _wrapMode ) 
    {
        if ( eventInfos.Count == 0 )
            return;
        if ( _delta == 0.0f )
            return;

        int index = 0;

        // WrapSeconds
        float t = WrapSeconds(_start,_wrapMode); 

        // start index
        tmpEventInfo.time = t;
        index = eventInfos.BinarySearch( tmpEventInfo, eventInfoComparer );
        if ( index < 0 )
            index = ~index;

        // forward
        if ( _delta > 0.0f ) {
            if ( t + _delta > length ) {
                float rest = t + _delta - length;
                if ( _wrapMode == WrapMode.Loop ) {
                    ForwardTriggerEvents ( _gameObject, index, t, length, false );
                        tmpEventInfo.time = 0.0f;
                        index = eventInfos.BinarySearch( tmpEventInfo, eventInfoComparer );
                        if ( index < 0 )
                            index = ~index;
                    ForwardTriggerEvents ( _gameObject, index, 0.0f, rest, true );
                }
                else if ( _wrapMode == WrapMode.PingPong ) {
                    // FIXME: it looks like the backward trigger didn't work at all { 
                    int cnt = (int)(_start/length);
                    if ( cnt % 2 == 1 ) {
                        BackwardTriggerEvents ( _gameObject, index, t, 0.0f, false );
                            tmpEventInfo.time = 0.0f;
                            index = eventInfos.BinarySearch( tmpEventInfo, eventInfoComparer );
                            if ( index < 0 )
                                index = ~index;
                        ForwardTriggerEvents ( _gameObject, index, 0.0f, rest, false );
                    }
                    else {
                        ForwardTriggerEvents ( _gameObject, index, t, length, false );
                            tmpEventInfo.time = length;
                            index = eventInfos.BinarySearch( tmpEventInfo, eventInfoComparer );
                            if ( index < 0 )
                                index = ~index;
                        BackwardTriggerEvents ( _gameObject, index, length, length - rest, false );
                    }
                    // } FIXME end 
                }
                else {
                    ForwardTriggerEvents ( _gameObject, index, t, t + _delta, false );
                }
            }
            else {
                ForwardTriggerEvents ( _gameObject, index, t, t + _delta, true );
            }
        }
        // backward
        else {
            if ( t + _delta < 0.0f ) {
                float rest = 0.0f - (t + _delta);
                if ( _wrapMode == WrapMode.Loop ) {
                    BackwardTriggerEvents ( _gameObject, index, t, 0.0f, false );
                        tmpEventInfo.time = length;
                        index = eventInfos.BinarySearch( tmpEventInfo, eventInfoComparer );
                        if ( index < 0 )
                            index = ~index;
                    BackwardTriggerEvents ( _gameObject, index, length, length - rest, true );
                }
                else if ( _wrapMode == WrapMode.PingPong ) {
                    // FIXME { 
                    BackwardTriggerEvents ( _gameObject, index, t, 0.0f, false );
                        tmpEventInfo.time = 0.0f;
                        index = eventInfos.BinarySearch( tmpEventInfo, eventInfoComparer );
                        if ( index < 0 )
                            index = ~index;
                    ForwardTriggerEvents ( _gameObject, index, 0.0f, rest, false );
                    // } FIXME end 
                }
                else {
                    BackwardTriggerEvents ( _gameObject, index, t, t + _delta, false );
                }
            }
            else {
                BackwardTriggerEvents ( _gameObject, index, t, t + _delta, true );
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void ForwardTriggerEvents ( GameObject _gameObject, 
                                int _index, 
                                float _start, 
                                float _end,
                                bool _includeStart ) 
    {
        for ( int i = _index; i < eventInfos.Count; ++i ) {
            EventInfo ei = eventInfos[i];
            if ( ei.time == _start && _includeStart == false )
                continue;

            if ( ei.time <= _end ) {
                Trigger ( _gameObject, ei );
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void BackwardTriggerEvents ( GameObject _gameObject, 
                                 int _index, 
                                 float _start, 
                                 float _end,
                                 bool _includeStart )
    {
        for ( int i = _index-1; i >= 0; --i ) {
            EventInfo ei = eventInfos[i];
            if ( ei.time == _start && _includeStart == false )
                continue;

            if ( ei.time >= _end ) {
                Trigger ( _gameObject, ei );
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Trigger ( GameObject _gameObject, 
                   EventInfo _eventInfo ) {

        if ( _eventInfo.methodName == "" )
            return;

        switch ( _eventInfo.paramType ) {
        case EventInfo.ParamType.NONE:
            _gameObject.SendMessage ( _eventInfo.methodName, _eventInfo.msgOptions );
            break;

        case EventInfo.ParamType.STRING:
            _gameObject.SendMessage ( _eventInfo.methodName, _eventInfo.stringParam, _eventInfo.msgOptions );
            break;

        case EventInfo.ParamType.FLOAT:
            _gameObject.SendMessage ( _eventInfo.methodName, _eventInfo.floatParam, _eventInfo.msgOptions );
            break;

        case EventInfo.ParamType.INT:
            _gameObject.SendMessage ( _eventInfo.methodName, _eventInfo.intParam, _eventInfo.msgOptions );
            break;

        case EventInfo.ParamType.BOOL:
            _gameObject.SendMessage ( _eventInfo.methodName, _eventInfo.boolParam, _eventInfo.msgOptions );
            break;

        case EventInfo.ParamType.OBJECT:
            _gameObject.SendMessage ( _eventInfo.methodName, _eventInfo.objectParam, _eventInfo.msgOptions );
            break;
        }
    }
}

