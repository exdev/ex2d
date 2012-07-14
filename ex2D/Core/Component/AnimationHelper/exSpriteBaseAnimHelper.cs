// ======================================================================================
// File         : exSpriteBaseAnimHelper.cs
// Author       : Wu Jie 
// Last Change  : 08/27/2011 | 17:16:27 PM | Saturday,August
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
[RequireComponent (typeof(exSpriteBase))]
public class exSpriteBaseAnimHelper : exAnimationHelper {

    exSpriteBase spriteBase;
    Vector2 lastScale = Vector2.one;
    Vector2 lastShear = Vector2.zero;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected new void Awake () {
        spriteBase = GetComponent<exSpriteBase>();
        lastScale = spriteBase.scale;
        lastShear = spriteBase.shear;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected new void LateUpdate () {
        if ( spriteBase == null ) {
            spriteBase = GetComponent<exSpriteBase>();
            if ( spriteBase == null ) {
                Debug.LogError("Can't find exSpriteBase Component in GameObject " + gameObject.name);
                return;
            }
        }

        if ( lastScale != spriteBase.scale ) {
            lastScale = spriteBase.scale;
            spriteBase.updateFlags |= exPlane.UpdateFlags.Vertex;
        }
        if ( lastShear != spriteBase.shear ) {
            lastShear = spriteBase.shear;
            spriteBase.updateFlags |= exPlane.UpdateFlags.Vertex;
        }
    }
}
