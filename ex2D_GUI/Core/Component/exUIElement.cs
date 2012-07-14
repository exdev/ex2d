// ======================================================================================
// File         : exUIElement.cs
// Author       : Wu Jie 
// Last Change  : 07/20/2011 | 00:07:45 AM | Wednesday,July
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
// Interactions Data
///////////////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

public class exUIElement : exPlane {

    [System.Serializable]
    public class MessageInfo {
        public GameObject receiver = null;
        public string method = "";
    }

    ///////////////////////////////////////////////////////////////////////////////
    // non-Serializable
    ///////////////////////////////////////////////////////////////////////////////

    [System.NonSerialized] public exUIElement parent = null;
    [System.NonSerialized] public List<exUIElement> children = new List<exUIElement>();

    // ------------------------------------------------------------------ 
    [SerializeField] protected float width_ = 100.0f;
    /// the width of the soft-clip
    // ------------------------------------------------------------------ 

    public float width {
        get { return width_; }
        set {
            if ( width_ != value ) {
                OnSizeChanged ( Mathf.Max(value, 0.0f), height_ );
            }
        }
    }

    // ------------------------------------------------------------------ 
    [SerializeField] protected float height_ = 100.0f;
    /// the height of the soft-clip
    // ------------------------------------------------------------------ 

    public float height {
        get { return height_; }
        set {
            if ( height_ != value ) {
                OnSizeChanged ( width_, Mathf.Max(value, 0.0f) );
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // NOTE: used in Raycast test
    // ------------------------------------------------------------------ 

    public bool isActive {
        get {
            if ( enabled == false )
                return false;
            exUIElement p = parent;
            while ( p != null ) {
                if ( p.enabled == false )
                    return false;
                p = p.parent;
            }
            return true;
        }
    }

    ///////////////////////////////////////////////////////////////////////////////
    // static functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    /// Awake functoin inherit from exPlane.
    // ------------------------------------------------------------------ 

    protected new void Awake () {
        base.Awake();
        updateFlags |= UpdateFlags.Vertex;
        Commit();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public override void Commit () {

        if ( (updateFlags & UpdateFlags.Vertex) != 0 ) {
            //
            float halfWidth = width_ * 0.5f;
            float halfHeight = height_ * 0.5f;
            float offsetX = 0.0f;
            float offsetY = 0.0f;

            //
            switch ( anchor ) {
            case Anchor.TopLeft     : offsetX = -halfWidth;   offsetY = -halfHeight;  break;
            case Anchor.TopCenter   : offsetX = 0.0f;         offsetY = -halfHeight;  break;
            case Anchor.TopRight    : offsetX = halfWidth;    offsetY = -halfHeight;  break;

            case Anchor.MidLeft     : offsetX = -halfWidth;   offsetY = 0.0f;         break;
            case Anchor.MidCenter   : offsetX = 0.0f;         offsetY = 0.0f;         break;
            case Anchor.MidRight    : offsetX = halfWidth;    offsetY = 0.0f;         break;

            case Anchor.BotLeft     : offsetX = -halfWidth;   offsetY = halfHeight;   break;
            case Anchor.BotCenter   : offsetX = 0.0f;         offsetY = halfHeight;   break;
            case Anchor.BotRight    : offsetX = halfWidth;    offsetY = halfHeight;   break;

            default                 : offsetX = 0.0f;         offsetY = 0.0f;         break;
            }

            //
            boundingRect = new Rect( -offsetX - halfWidth, 
                                      offsetY - halfHeight,
                                      width_, 
                                      height_ );

            if ( collisionHelper ) 
                collisionHelper.UpdateCollider();
        }

        //
        updateFlags = UpdateFlags.None;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static void FindAndAddChild ( exUIElement _el ) {
        _el.children.Clear();
        FindAndAddChildRecursively (_el, _el.transform );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static void FindAndAddChildRecursively ( exUIElement _el, Transform _trans ) {
        foreach ( Transform child in _trans ) {
            exUIElement child_el = child.GetComponent<exUIElement>();
            if ( child_el ) {
                _el.AddChild (child_el);
                exUIElement.FindAndAddChild (child_el);
            }
            else {
                FindAndAddChildRecursively( _el, child );
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected new void OnDestroy () {
        base.OnDestroy();

        if ( parent != null ) {
            parent.RemoveChild(this);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public virtual bool OnEvent ( exUIEvent _e ) {
        return false;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void AddChild ( exUIElement _element ) {
        _element.parent = this;
        children.Add(_element);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void RemoveChild ( exUIElement _element ) {
        _element.parent = null;
        children.Remove(_element);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public exUIElement FindParent () {
        Transform tranParent = transform.parent;
        while ( tranParent != null ) {
            exUIElement el = tranParent.GetComponent<exUIElement>();
            if ( el != null )
                return el;
            tranParent = tranParent.parent;
        }
        return null;
    } 

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected void ProcessMessageInfoList ( List<MessageInfo> _messageInfos ) {
        for ( int i = 0; i < _messageInfos.Count; ++i ) {
            MessageInfo msgInfo = _messageInfos[i];
            if ( msgInfo.receiver != null ) {
                msgInfo.receiver.SendMessage ( msgInfo.method, SendMessageOptions.DontRequireReceiver );
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected virtual void OnSizeChanged ( float _newWidth, float _newHeight ) {
        width_ = _newWidth;
        height_ = _newHeight;
        updateFlags |= UpdateFlags.Vertex;
    }
}
