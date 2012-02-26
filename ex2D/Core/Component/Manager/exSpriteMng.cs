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
    List<exSoftClip> softClips = new List<exSoftClip>();
    
    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnPreRender () {

        // ======================================================== 
        // pre-softclip 
        // ======================================================== 

        foreach ( exPlane sp in sprites ) {
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
        // process softclip items after sprites' boundingRect changes 
        // ======================================================== 

        foreach ( exSoftClip sp in softClips ) {
            sp.UpdateClipInfo();
        }
        softClips.Clear();

        // ======================================================== 
        // post-softclip 
        // ======================================================== 

        foreach ( exPlane sp in sprites ) {
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

    public void AddToSoftClipList ( exSoftClip _softClip ) {
        softClips.Add (_softClip);
    }
}
