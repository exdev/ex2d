// ======================================================================================
// File         : exUILabel.cs
// Author       : Wu Jie 
// Last Change  : 08/07/2012 | 00:58:06 AM | Tuesday,August
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

[AddComponentMenu("ex2D GUI/Label")]
public class exUILabel : exUIElement {

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
                if ( autoSize_ ) {
                    font.Commit();
                    OnSizeChanged( font.boundingRect.width, font.boundingRect.height );
                }
            }
        }
    }

    // ------------------------------------------------------------------ 
    [SerializeField] protected Anchor alignment_ = Anchor.TopLeft;
    /// the text of the button
    // ------------------------------------------------------------------ 

    public Anchor alignment {
        get { return alignment_; }
        set {
            if ( alignment_ != value ) {
                alignment_ = value;
                AdjustTextLocalPosition ();
            }
        }
    }

    // ------------------------------------------------------------------ 
    [SerializeField] protected bool autoSize_ = true;
    /// the text of the button
    // ------------------------------------------------------------------ 

    public bool autoSize {
        get { return autoSize_; }
        set {
            if ( autoSize_ != value ) {
                autoSize_ = value;
                OnSizeChanged ( font.boundingRect.width, font.boundingRect.height );
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////////
    // serialized 
    ///////////////////////////////////////////////////////////////////////////////

    public exSpriteFont font = null;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected override void OnSizeChanged ( float _newWidth, float _newHeight ) {
        if ( autoSize_ ) {
            _newWidth = font.boundingRect.width;
            _newHeight = font.boundingRect.height;
        }
        base.OnSizeChanged( _newWidth, _newHeight );
        Commit();
        AdjustTextLocalPosition ();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected void AdjustTextLocalPosition () {
        float offsetX = 0.0f;
        float offsetY = 0.0f;

        font.anchor = alignment_;
        switch ( alignment_ ) {
        case Anchor.TopLeft     : offsetX = boundingRect.xMin + style.padding.left;         offsetY = boundingRect.yMax + style.padding.top;  font.textAlign = exSpriteFont.TextAlign.Left; break;
        case Anchor.TopCenter   : offsetX = boundingRect.xMin + boundingRect.width * 0.5f;  offsetY = boundingRect.yMax + style.padding.top;  font.textAlign = exSpriteFont.TextAlign.Center; break;
        case Anchor.TopRight    : offsetX = boundingRect.xMax - style.padding.right;        offsetY = boundingRect.yMax + style.padding.top;  font.textAlign = exSpriteFont.TextAlign.Right; break;

        case Anchor.MidLeft     : offsetX = boundingRect.xMin + style.padding.left;         offsetY = boundingRect.yMin + boundingRect.height * 0.5f;  font.textAlign = exSpriteFont.TextAlign.Left; break;
        case Anchor.MidCenter   : offsetX = boundingRect.xMin + boundingRect.width * 0.5f;  offsetY = boundingRect.yMin + boundingRect.height * 0.5f;  font.textAlign = exSpriteFont.TextAlign.Center; break;
        case Anchor.MidRight    : offsetX = boundingRect.xMax - style.padding.right;        offsetY = boundingRect.yMin + boundingRect.height * 0.5f;  font.textAlign = exSpriteFont.TextAlign.Right; break;

        case Anchor.BotLeft     : offsetX = boundingRect.xMin + style.padding.left;         offsetY = boundingRect.yMin - style.padding.bottom;   font.textAlign = exSpriteFont.TextAlign.Left; break;
        case Anchor.BotCenter   : offsetX = boundingRect.xMin + boundingRect.width * 0.5f;  offsetY = boundingRect.yMin - style.padding.bottom;   font.textAlign = exSpriteFont.TextAlign.Center; break;
        case Anchor.BotRight    : offsetX = boundingRect.xMax - style.padding.right;        offsetY = boundingRect.yMin - style.padding.bottom;   font.textAlign = exSpriteFont.TextAlign.Right; break;

        default                 : offsetX = 0.0f;         offsetY = 0.0f;         break;
        }
        font.transform.localPosition = new Vector3 ( offsetX, offsetY, font.transform.localPosition.z );
    }
}

