// ======================================================================================
// File         : exClippingAnimHelper.cs
// Author       : Wu Jie 
// Last Change  : 06/09/2012 | 11:17:32 AM | Saturday,June
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
[RequireComponent (typeof(exClipping))]
public class exClippingAnimHelper : exAnimationHelper {

    exClipping clipping;
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

        clipping = GetComponent<exClipping>();
        lastWidth = clipping.width;
        lastHeight = clipping.height;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected new void Update () {
        base.Update();
        if ( clipping == null ) {
            clipping = GetComponent<exClipping>();
            if ( clipping == null ) {
                Debug.LogError("Can't find exClipping Component in GameObject " + gameObject.name);
                return;
            }
        }

        if ( lastWidth != clipping.width ) {
            lastWidth = clipping.width;
            clipping.updateFlags |= exPlane.UpdateFlags.Vertex;
        }
        if ( lastHeight != clipping.height ) {
            lastHeight = clipping.height;
            clipping.updateFlags |= exPlane.UpdateFlags.Vertex;
        }
    }

}

