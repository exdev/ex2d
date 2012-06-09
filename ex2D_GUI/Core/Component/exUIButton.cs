// ======================================================================================
// File         : exUIButton.cs
// Author       : Wu Jie 
// Last Change  : 10/30/2011 | 11:27:13 AM | Sunday,October
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

[AddComponentMenu("ex2D GUI/Button")]
public class exUIButton : exUIElement {

    // delegates
	public delegate void EventHandler ();

    // events
	public event EventHandler OnHoverIn;
	public event EventHandler OnHoverOut;
	public event EventHandler OnButtonPress;
	public event EventHandler OnButtonRelease;


    // ------------------------------------------------------------------ 
    [SerializeField] protected string text_ = "";
    /// the text of the button
    // ------------------------------------------------------------------ 

    public string text {
        get { return text_; }
        set {
            if ( text_ != value ) {
                text_ = value;
                font.text = text_;
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    bool isPressing = false;

    public exSpriteBorder border = null;
    public exSpriteFont font = null;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public override void Sync () {
        base.Sync ();

        if ( border ) {
            border.anchor = anchor;
            border.width = width;
            border.height = height;
            border.transform.localPosition = new Vector3 ( 0.0f, 0.0f, border.transform.localPosition.z );
        }

        if ( font ) {
            BoxCollider boxCollider = GetComponent<BoxCollider>();
            font.transform.localPosition 
                = new Vector3( boxCollider.center.x, boxCollider.center.y, font.transform.localPosition.z );

            // DELME { 
            // switch ( plane ) {
            // case exSprite.Plane.XY:
            //     font.transform.localPosition 
            //         = new Vector3( boxCollider.center.x, boxCollider.center.y, font.transform.localPosition.z );
            //     break;

            // case exSprite.Plane.XZ:
            //     font.transform.localPosition 
            //         = new Vector3( boxCollider.center.x, font.transform.localPosition.y, boxCollider.center.z );
            //     break;

            // case exSprite.Plane.ZY:
            //     font.transform.localPosition 
            //         = new Vector3( font.transform.localPosition.x, boxCollider.center.y, boxCollider.center.z );
            //     break;
            // }
            // } DELME end 
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public override bool OnEvent ( exUIEvent _e ) {
        switch ( _e.type ) {
        case exUIEvent.Type.HoverIn: 
            if ( OnHoverIn != null )
                OnHoverIn ();
            return true;

        case exUIEvent.Type.HoverOut: 
            if ( exUIMng.instance.activeElement == this ) {
                isPressing = false;
                exUIMng.instance.activeElement = null;
                if ( OnHoverOut != null )
                    OnHoverOut ();
            }
            return true;

        case exUIEvent.Type.PointerPress: 
            if ( _e.buttons == exUIEvent.PointerButtonFlags.Left ||
                 _e.buttons == exUIEvent.PointerButtonFlags.Touch ) {
                isPressing = true;
                exUIMng.instance.activeElement = this;
                if ( OnButtonPress != null )
                    OnButtonPress ();
            }
            return true;

        case exUIEvent.Type.PointerRelease: 
            if ( _e.buttons == exUIEvent.PointerButtonFlags.Left ||
                 _e.buttons == exUIEvent.PointerButtonFlags.Touch ) {
                exUIMng.instance.activeElement = null;
                if ( isPressing ) {
                    StartCoroutine ( DelayButtonRelease(0.0f) );
                }
            }
            return true;
        }

        return false;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    IEnumerator DelayButtonRelease ( float _delay ) {
        float delay = _delay;
        while ( delay > 0.0f ) {
            delay -= Time.deltaTime;
            yield return false;
        }
        if ( OnButtonRelease != null )
            OnButtonRelease ();
        isPressing = false;
    }
}
