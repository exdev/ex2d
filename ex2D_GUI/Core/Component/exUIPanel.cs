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

    ///////////////////////////////////////////////////////////////////////////////
    // serialized 
    ///////////////////////////////////////////////////////////////////////////////

    public exSpriteBase background = null;

    // message infos
    public List<MessageInfo> hoverInSlots   = new List<MessageInfo>();
    public List<MessageInfo> hoverOutSlots  = new List<MessageInfo>();
    public List<MessageInfo> pressSlots     = new List<MessageInfo>();
    public List<MessageInfo> releaseSlots   = new List<MessageInfo>();
    public List<MessageInfo> moveSlots   = new List<MessageInfo>();

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public override bool OnEvent ( exUIEvent _e ) {
        exUIMng uimng = exUIMng.instance;

        if ( _e.category == exUIEvent.Category.Mouse ) {
            if ( _e.type == exUIEvent.Type.MouseEnter ) {
                OnHoverIn (_e);
                return true;
            }
            else if ( _e.type == exUIEvent.Type.MouseExit ) {
                if ( uimng.GetMouseFocus() == this ) {
                    uimng.SetMouseFocus(null);
                }
                OnHoverOut(_e);
                return true;
            }
            else if ( _e.type == exUIEvent.Type.MouseDown ) {
                uimng.SetMouseFocus( this );
                OnPress(_e);
                return true;
            }
            else if ( _e.type == exUIEvent.Type.MouseUp ) {
                if ( uimng.GetMouseFocus() == this ) {
                    uimng.SetMouseFocus( null );
                }
                OnRelease(_e);
                return true;
            }
            else if ( _e.type == exUIEvent.Type.MouseMove ) {
                OnPointerMove(_e);
                return true;
            }
        }
        else if ( _e.category == exUIEvent.Category.Touch ) {
            if ( _e.type == exUIEvent.Type.TouchEnter ) {
                OnHoverIn (_e);
                return true;
            }
            else if ( _e.type == exUIEvent.Type.TouchExit ) {
                if ( uimng.GetTouchFocus(_e.touchID) == this ) {
                    uimng.SetTouchFocus( _e.touchID, null );
                }
                OnHoverOut(_e);
                return true;
            }
            else if ( _e.type == exUIEvent.Type.TouchDown ) {
                uimng.SetTouchFocus( _e.touchID, this );
                OnPress(_e);
                return true;
            }
            else if ( _e.type == exUIEvent.Type.TouchUp ) {
                if ( uimng.GetTouchFocus(_e.touchID) == this ) {
                    uimng.SetTouchFocus( _e.touchID, null );
                }
                OnRelease(_e);
                OnHoverOut(_e);
                return true;
            }
            else if ( _e.type == exUIEvent.Type.TouchMove ) {
                OnPointerMove(_e);
                return true;
            }
        }

        //
        return false;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected override void OnSizeChanged ( float _newWidth, float _newHeight ) {
        base.OnSizeChanged( _newWidth, _newHeight );

        if ( background ) {
            exSprite spriteBG = background as exSprite;
            if ( spriteBG ) {
                spriteBG.width = _newWidth;
                spriteBG.height = _newHeight;
            }
            else {
                exSpriteBorder borderBG = background as exSpriteBorder;
                if ( borderBG ) {
                    borderBG.width = _newWidth;
                    borderBG.height = _newHeight;
                }
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	public virtual void OnHoverIn ( exUIEvent _e ) {
        ProcessMessageInfoList ( hoverInSlots );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	public virtual void OnHoverOut ( exUIEvent _e ) {
        ProcessMessageInfoList ( hoverOutSlots );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	public virtual void OnPress ( exUIEvent _e ) {
        ProcessMessageInfoList ( pressSlots );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	public virtual void OnRelease ( exUIEvent _e ) {
        ProcessMessageInfoList ( releaseSlots );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	public virtual void OnPointerMove ( exUIEvent _e ) {
        ProcessMessageInfoList ( moveSlots );
    }
}
