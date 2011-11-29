// ======================================================================================
// File         : exSpriteFontAnimHelper.cs
// Author       : Wu Jie 
// Last Change  : 08/27/2011 | 17:38:41 PM | Saturday,August
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

[ExecuteInEditMode]
[RequireComponent (typeof(exSpriteFont))]
public class exSpriteFontAnimHelper : exSpriteBaseAnimHelper {

    exSpriteFont spriteFont;
    Color lastTopColor = Color.white;
    Color lastBotColor = Color.white;
    float lastOutlineWidth = 1.0f;
    Color lastOutlineColor = Color.white; 
    Vector2 lastShadowBias = new Vector2 ( 1.0f, -1.0f );
    Color lastShadowColor = Color.black;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    override protected void Awake () {
        base.Awake();

        spriteFont = GetComponent<exSpriteFont>();
        lastTopColor = spriteFont.topColor; 
        lastBotColor = spriteFont.botColor;
        lastOutlineWidth = spriteFont.outlineWidth;
        lastOutlineColor = spriteFont.outlineColor;
        lastShadowBias = spriteFont.shadowBias;
        lastShadowColor = spriteFont.shadowColor;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    override protected void Update () {
        base.Update();
        if ( spriteFont == null ) {
            spriteFont = GetComponent<exSpriteFont>();
            if ( spriteFont == null ) {
                Debug.LogError("Can't find exSpriteBorder Component in GameObject " + gameObject.name);
                return;
            }
        }

        //
        if ( lastTopColor != spriteFont.topColor ) {
            lastTopColor = spriteFont.topColor;
            spriteFont.updateFlags |= exPlane.UpdateFlags.Color;
        }
        if ( lastBotColor != spriteFont.botColor ) {
            lastBotColor = spriteFont.botColor;
            spriteFont.updateFlags |= exPlane.UpdateFlags.Color;
        }

        //
        if ( spriteFont.useOutline ) {
            if ( lastOutlineWidth != spriteFont.outlineWidth ) {
                lastOutlineWidth = spriteFont.outlineWidth;
                spriteFont.updateFlags |= exPlane.UpdateFlags.Vertex;
            }
            if ( lastOutlineColor != spriteFont.outlineColor ) {
                lastOutlineColor = spriteFont.outlineColor;
                spriteFont.updateFlags |= exPlane.UpdateFlags.Color;
            }
        }

        //
        if ( spriteFont.useShadow ) {
            if ( lastShadowBias != spriteFont.shadowBias ) {
                lastShadowBias = spriteFont.shadowBias;
                spriteFont.updateFlags |= exPlane.UpdateFlags.Vertex;
            }
            if ( lastShadowColor != spriteFont.shadowColor ) {
                lastShadowColor = spriteFont.shadowColor;
                spriteFont.updateFlags |= exPlane.UpdateFlags.Color;
            }
        }
    }
}
