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
                if ( hoverInSlots.Count > 0 ) {
                    OnHoverIn (_e);
                    return true;
                }
                return false;
            }
            else if ( _e.type == exUIEvent.Type.MouseExit ) {
                if ( uimng.GetMouseFocus() == this ) {
                    uimng.SetMouseFocus(null);
                }

                if ( hoverOutSlots.Count > 0 ) {
                    OnHoverOut(_e);
                    return true;
                }
                return false;
            }
            else if ( _e.type == exUIEvent.Type.MouseDown ) {
                if ( uimng.GetMouseFocus() == null )
                    uimng.SetMouseFocus( this );

                if ( pressSlots.Count > 0 ) {
                    OnPress(_e);
                    return true;
                }
                return false;
            }
            else if ( _e.type == exUIEvent.Type.MouseUp ) {
                if ( uimng.GetMouseFocus() == this ) {
                    uimng.SetMouseFocus( null );
                }

                if ( releaseSlots.Count > 0 ) {
                    OnRelease(_e);
                    return true;
                }
                return false;
            }
            else if ( _e.type == exUIEvent.Type.MouseMove ) {
                if ( moveSlots.Count > 0 ) {
                    OnPointerMove(_e);
                    return true;
                }
                return false;
            }
        }
        else if ( _e.category == exUIEvent.Category.Touch ) {
            if ( _e.type == exUIEvent.Type.TouchEnter ) {
                if ( hoverInSlots.Count > 0 ) {
                    OnHoverIn (_e);
                    return true;
                }
                return false;
            }
            else if ( _e.type == exUIEvent.Type.TouchExit ) {
                if ( uimng.GetTouchFocus(_e.touchID) == this ) {
                    uimng.SetTouchFocus( _e.touchID, null );
                }

                if ( hoverOutSlots.Count > 0 ) {
                    OnHoverOut(_e);
                    return true;
                }
                return false;
            }
            else if ( _e.type == exUIEvent.Type.TouchDown ) {
                if ( uimng.GetTouchFocus(_e.touchID) == null ) {
                    uimng.SetTouchFocus( _e.touchID, this );
                }

                if ( pressSlots.Count > 0 ) {
                    OnPress(_e);
                    return true;
                }
                return false;
            }
            else if ( _e.type == exUIEvent.Type.TouchUp ) {
                if ( uimng.GetTouchFocus(_e.touchID) == this ) {
                    uimng.SetTouchFocus( _e.touchID, null );
                }

                bool used = false;
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
            else if ( _e.type == exUIEvent.Type.TouchMove ) {
                if ( moveSlots.Count > 0 ) {
                    OnPointerMove(_e);
                    return true;
                }
                return false;
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

	public virtual void OnPointerMove ( exUIEvent _e ) {
        ProcessMessageInfoList ( moveSlots );
    }
}
