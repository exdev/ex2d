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
    // non-serialized
    ///////////////////////////////////////////////////////////////////////////////

    bool isPressing = false;

    ///////////////////////////////////////////////////////////////////////////////
    // serialized 
    ///////////////////////////////////////////////////////////////////////////////

    public exSpriteFont font = null;
    public exSpriteBase background = null;

    // message infos
    public List<MessageInfo> hoverInSlots   = new List<MessageInfo>();
    public List<MessageInfo> hoverOutSlots  = new List<MessageInfo>();
    public List<MessageInfo> pressSlots     = new List<MessageInfo>();
    public List<MessageInfo> releaseSlots   = new List<MessageInfo>();
    public List<MessageInfo> clickSlots     = new List<MessageInfo>();

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
                    isPressing = false;
                    uimng.SetMouseFocus(null);
                }
                OnHoverOut(_e);
                return true;
            }
            else if ( _e.type == exUIEvent.Type.MouseDown &&
                      _e.buttons == exUIEvent.MouseButtonFlags.Left ) 
            {
                uimng.SetMouseFocus( this );
                isPressing = true;
                OnPress(_e);
                return true;
            }
            else if ( _e.type == exUIEvent.Type.MouseUp &&
                      _e.buttons == exUIEvent.MouseButtonFlags.Left )
            {
                if ( isPressing ) {
                    if ( uimng.GetMouseFocus() == this ) {
                        uimng.SetMouseFocus( null );
                    }
                    isPressing = false;
                    OnClick(_e);
                }
                OnRelease(_e);
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
                    isPressing = false;
                    uimng.SetTouchFocus( _e.touchID, null );
                }
                OnHoverOut(_e);
                return true;
            }
            else if ( _e.type == exUIEvent.Type.TouchDown ) {
                uimng.SetTouchFocus( _e.touchID, this );
                isPressing = true;
                OnPress(_e);
                return true;
            }
            else if ( _e.type == exUIEvent.Type.TouchUp ) {
                if ( isPressing ) {
                    if ( uimng.GetTouchFocus(_e.touchID) == this ) {
                        uimng.SetTouchFocus( _e.touchID, null );
                    }
                    isPressing = false;
                    OnClick(_e);
                }
                OnRelease(_e);
                OnHoverOut(_e);
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

	public virtual void OnClick ( exUIEvent _e ) {
        ProcessMessageInfoList ( clickSlots );
    }

}
