// ======================================================================================
// File         : exSpriteBorderAnimHelper.cs
// Author       : Wu Jie 
// Last Change  : 09/23/2011 | 12:15:21 PM | Friday,September
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
[RequireComponent (typeof(exSpriteBorder))]
public class exSpriteBorderAnimHelper : exSpriteBaseAnimHelper {

    exSpriteBorder spriteBorder;
    Color lastColor = Color.white;
    float lastWidth;
    float lastHeight;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected new void Awake () {
        base.Awake();

        spriteBorder = GetComponent<exSpriteBorder>();
        lastColor = spriteBorder.color;
        lastWidth = spriteBorder.width;
        lastHeight = spriteBorder.height;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected new void Update () {
        base.Update();
        if ( spriteBorder == null ) {
            spriteBorder = GetComponent<exSpriteBorder>();
            if ( spriteBorder == null ) {
                Debug.LogError("Can't find exSpriteBorder Component in GameObject " + gameObject.name);
                return;
            }
        }

        if ( lastColor != spriteBorder.color ) {
            lastColor = spriteBorder.color;
            spriteBorder.updateFlags |= exPlane.UpdateFlags.Color;
        }
        if ( lastWidth != spriteBorder.width ) {
            lastWidth = spriteBorder.width;
            spriteBorder.updateFlags |= exPlane.UpdateFlags.Vertex;
        }
        if ( lastHeight != spriteBorder.height ) {
            lastHeight = spriteBorder.height;
            spriteBorder.updateFlags |= exPlane.UpdateFlags.Vertex;
        }
    }
}
