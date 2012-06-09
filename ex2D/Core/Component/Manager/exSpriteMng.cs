// ======================================================================================
// File         : exSpriteMng.cs
// Author       : Wu Jie 
// Last Change  : 10/26/2011 | 09:29:45 AM | Wednesday,October
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
/// 
/// A component to manage all sprite update before rendering
/// 
///////////////////////////////////////////////////////////////////////////////

[ExecuteInEditMode]
public class exSpriteMng : MonoBehaviour {

    List<exPlane> sprites = new List<exPlane>();
    List<exClipping> clippingList = new List<exClipping>();
    
    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnPreRender () {

        // ======================================================== 
        // update sprites
        // ======================================================== 

        for ( int i = 0; i < sprites.Count; ++i ) {
            exPlane sp = sprites[i];
            // NOTE: it is possible the sprite has been destroyed first
            if ( sp != null ) { 
                if ( sp.updateFlags != exPlane.UpdateFlags.None ) {
                    sp.Commit();
                    sp.updateFlags = exPlane.UpdateFlags.None;
                }
                sp.inCommitList = false;
            }
        }
        sprites.Clear();

        // ======================================================== 
        // update clip items
        // ======================================================== 

        for ( int i = 0; i < clippingList.Count; ++i ) {
            exClipping clipping = clippingList[i];
            if ( clipping.enabled )
                clipping.CommitMaterialProperties();
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void AddToCommitList ( exPlane _plane ) { 
        if ( _plane.inCommitList == false ) {
            sprites.Add (_plane); 
            _plane.inCommitList = true;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void AddToClippingList ( exClipping _clipping ) {
        if ( clippingList.IndexOf(_clipping) == -1 )
            clippingList.Add (_clipping);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void RemoveFromClippingList ( exClipping _clipping ) {
        clippingList.Remove (_clipping);
    }
}
