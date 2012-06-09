// ======================================================================================
// File         : exUIPanel.cs
// Author       : Wu Jie 
// Last Change  : 10/30/2011 | 15:43:39 PM | Sunday,October
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

[AddComponentMenu("ex2D GUI/Panel")]
public class exUIPanel : exUIElement {

    // delegates
	public delegate void EventHandler ();

    // events
	public event EventHandler OnHoverIn;
	public event EventHandler OnHoverOut;
	public event EventHandler OnButtonPress;
	public event EventHandler OnButtonRelease;
	public event EventHandler OnPointerMove;

    public exSpriteBorder background = null;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public override void Sync () {
        base.Sync ();

        if ( background ) {
            background.anchor = anchor;
            background.width = width;
            background.height = height;
            background.transform.localPosition = new Vector3 ( 0.0f, 0.0f, background.transform.localPosition.z );
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public override bool OnEvent ( exUIEvent _e ) {
        switch ( _e.type ) {
        case exUIEvent.Type.HoverIn: 
            if ( OnHoverIn != null ) {
                OnHoverIn ();
                return true;
            }
            return false;

        case exUIEvent.Type.HoverOut: 
            if ( OnHoverOut != null ) {
                OnHoverOut ();
                return true;
            }
            return false;

        case exUIEvent.Type.PointerPress: 
            exUIMng.instance.activeElement = this;
            if ( OnButtonPress != null ) {
                OnButtonPress ();
                return true;
            }
            return false;

        case exUIEvent.Type.PointerRelease: 
            exUIMng.instance.activeElement = null;
            if ( OnButtonRelease != null ) {
                OnButtonRelease ();
                return true;
            }
            return false;

        case exUIEvent.Type.PointerMove: 
            if ( OnPointerMove != null ) {
                OnPointerMove ();
                return false;
            }
            return true;
        }

        return false;
    }
}
