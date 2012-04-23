// ======================================================================================
// File         : exSoftClipAnimHelper.cs
// Author       : Wu Jie 
// Last Change  : 08/27/2011 | 20:54:58 PM | Saturday,August
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
[RequireComponent (typeof(exSoftClip))]
public class exSoftClipAnimHelper : exSpriteBaseAnimHelper {

    exSoftClip softClip;
    // Vector2 lastCenter = Vector2.zero;
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

        softClip = GetComponent<exSoftClip>();
        // lastCenter = softClip.center;
        lastWidth = softClip.width;
        lastHeight = softClip.height;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected new void Update () {
        base.Update();
        if ( softClip == null ) {
            softClip = GetComponent<exSoftClip>();
            if ( softClip == null ) {
                Debug.LogError("Can't find exSoftClip Component in GameObject " + gameObject.name);
                return;
            }
        }

        // if ( lastCenter != softClip.center ) {
        //     lastCenter = softClip.center;
        // }
        if ( lastWidth != softClip.width ) {
            lastWidth = softClip.width;
            softClip.updateFlags |= exPlane.UpdateFlags.Vertex;
        }
        if ( lastHeight != softClip.height ) {
            lastHeight = softClip.height;
            softClip.updateFlags |= exPlane.UpdateFlags.Vertex;
        }
    }
}

