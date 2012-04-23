// ======================================================================================
// File         : exSpriteAnimHelper.cs
// Author       : Wu Jie 
// Last Change  : 08/27/2011 | 17:30:00 PM | Saturday,August
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
[RequireComponent (typeof(exSprite))]
public class exSpriteAnimHelper : exSpriteBaseAnimHelper {

    exSprite sprite;
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

        sprite = GetComponent<exSprite>();
        lastColor = sprite.color;
        lastWidth = sprite.width;
        lastHeight = sprite.height;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected new void Update () {
        base.Update();
        if ( sprite == null ) {
            sprite = GetComponent<exSprite>();
            if ( sprite == null ) {
                Debug.LogError("Can't find exSprite Component in GameObject " + gameObject.name);
                return;
            }
        }

        if ( lastColor != sprite.color ) {
            lastColor = sprite.color;
            sprite.updateFlags |= exPlane.UpdateFlags.Color;
        }
        if ( lastWidth != sprite.width ) {
            lastWidth = sprite.width;
            sprite.updateFlags |= exPlane.UpdateFlags.Vertex;
        }
        if ( lastHeight != sprite.height ) {
            lastHeight = sprite.height;
            sprite.updateFlags |= exPlane.UpdateFlags.Vertex;
        }
    }
}
