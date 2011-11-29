// ======================================================================================
// File         : exLayerZY.cs
// Author       : Wu Jie 
// Last Change  : 09/01/2011 | 15:53:56 PM | Thursday,September
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
//
// The layer calculated in zy 2D space
//
///////////////////////////////////////////////////////////////////////////////

public class exLayerZY : exLayer2D {

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    override protected float CalculateDepth ( Camera _cam ) {
        if ( _cam == null )
            return 0.0f;
        float dist = _cam.farClipPlane - _cam.nearClipPlane;
        float unitLayer = dist/MAX_LAYER;
        return -(((float)layer_ + bias_) * unitLayer - _cam.transform.position.x + _cam.nearClipPlane);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    override protected void UpdateTransformDepth () { 
        if ( Mathf.Approximately(depth_, transform.position.x) == false ) {
            transform.position = new Vector3( depth_,
                                              transform.position.y,
                                              transform.position.z );
        }
    }

}
