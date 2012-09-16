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

        // ======================================================== 
        if ( _e.category == exUIEvent.Category.Mouse ) {
        // ======================================================== 

            if ( _e.type == exUIEvent.Type.MouseEnter ) {
                if ( hoverInSlots.Count > 0 ) {
                    OnHoverIn (_e);
                    return true;
                }
                return false;
            }
            else if ( _e.type == exUIEvent.Type.MouseExit ) {
                if ( uimng.GetMouseFocus() == this ) {
                    isPressing = false;
                    uimng.SetMouseFocus(null);
                }

                if ( hoverOutSlots.Count > 0 ) {
                    OnHoverOut(_e);
                    return true;
                }
                return false;
            }
            else if ( _e.type == exUIEvent.Type.MouseDown &&
                      _e.buttons == exUIEvent.MouseButtonFlags.Left ) 
            {
                if ( uimng.GetMouseFocus() == null ) {
                    uimng.SetMouseFocus( this );
                    isPressing = true;

                    if ( pressSlots.Count > 0 ) {
                        OnPress(_e);
                        return true;
                    }
                }
                return false;
            }
            else if ( _e.type == exUIEvent.Type.MouseUp &&
                      _e.buttons == exUIEvent.MouseButtonFlags.Left )
            {
                bool used = false;
                if ( isPressing ) {
                    if ( uimng.GetMouseFocus() == this ) {
                        uimng.SetMouseFocus( null );

                        if ( clickSlots.Count > 0 ) {
                            OnClick(_e);
                            used = true;
                        }
                    }
                    isPressing = false;
                }

                if ( releaseSlots.Count > 0 ) {
                    OnRelease(_e);
                    used = true;
                }
                return used;
            }
        }

        // ======================================================== 
        else if ( _e.category == exUIEvent.Category.Touch ) {
        // ======================================================== 

            if ( _e.type == exUIEvent.Type.TouchEnter ) {
                if ( hoverInSlots.Count > 0 ) {
                    OnHoverIn (_e);
                    return true;
                }
                return false;
            }
            else if ( _e.type == exUIEvent.Type.TouchExit ) {
                if ( uimng.GetTouchFocus(_e.touchID) == this ) {
                    isPressing = false;
                    uimng.SetTouchFocus( _e.touchID, null );
                }

                if ( hoverOutSlots.Count > 0 ) {
                    OnHoverOut(_e);
                    return true;
                }
                return false;
            }
            else if ( _e.type == exUIEvent.Type.TouchDown ) {
                if ( uimng.GetTouchFocus( _e.touchID ) == null ) {
                    uimng.SetTouchFocus( _e.touchID, this );
                    isPressing = true;

                    if ( pressSlots.Count > 0 ) {
                        OnPress(_e);
                        return true;
                    }
                }
                return false;
            }
            else if ( _e.type == exUIEvent.Type.TouchUp ) {
                bool used = false;
                if ( isPressing ) {
                    if ( uimng.GetTouchFocus(_e.touchID) == this ) {
                        uimng.SetTouchFocus( _e.touchID, null );

                        if ( clickSlots.Count > 0 ) {
                            OnClick(_e);
                            used = true;
                        }
                    }
                    isPressing = false;
                }

                if ( releaseSlots.Count > 0 ) {
                    OnRelease(_e);
                    used = true;
                }

                if ( hoverOutSlots.Count > 0 ) {
                    OnHoverOut(_e);
                    used = true;
                }

                return used;
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
            exUIMng.SetSize ( background, _newWidth, _newHeight );
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
