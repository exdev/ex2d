// ======================================================================================
// File         : exPixelPerfect.cs
// Author       : Wu Jie 
// Last Change  : 07/24/2011 | 20:19:52 PM | Sunday,July
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
/// A component to handle pixel perfect process
/// 
///////////////////////////////////////////////////////////////////////////////

[ExecuteInEditMode]
public class exPixelPerfect : MonoBehaviour {

    ///////////////////////////////////////////////////////////////////////////////
    // private data
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    /// The cached sprite base component
    // ------------------------------------------------------------------ 

    [System.NonSerialized] public exSpriteBase sprite;
    [System.NonSerialized] public float depthToCamera = 0.0f;

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    exPixelPerfectCamera ppfCamera;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Awake () {
        sprite = GetComponent<exSpriteBase>();
        depthToCamera = transform.position.z - sprite.renderCamera.transform.position.z;

        // DELME { 
        // //
        // if ( sprite.renderCamera.orthographic == false ) {
        //     switch ( sprite.plane ) {
        //     case exPlane.Plane.XY: depthToCamera = transform.position.z - sprite.renderCamera.transform.position.z; break;
        //     case exPlane.Plane.XZ: depthToCamera = transform.position.y - sprite.renderCamera.transform.position.y; break;
        //     case exPlane.Plane.ZY: depthToCamera = transform.position.x - sprite.renderCamera.transform.position.x; break;
        //     }
        // }
        // } DELME end 

        //
        ppfCamera = sprite.renderCamera.GetComponent<exPixelPerfectCamera>();
        if ( ppfCamera == null ) {
            ppfCamera = sprite.renderCamera.gameObject.AddComponent<exPixelPerfectCamera>();
        }
        ppfCamera.MakePixelPerfect(sprite,depthToCamera);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnEnable () {
        if ( sprite == null ) {
            sprite = GetComponent<exSpriteBase>();
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // NOTE: if in LateUpdate, it may not go into sprite.Commit while changes
    // ------------------------------------------------------------------ 

    void Update () {
        if ( sprite.renderCamera.orthographic == false ) {
            float depth = transform.position.z - sprite.renderCamera.transform.position.z;

            // DELME { 
            // switch ( sprite.plane ) {
            // case exPlane.Plane.XY: depth = transform.position.z - sprite.renderCamera.transform.position.z; break;
            // case exPlane.Plane.XZ: depth = transform.position.y - sprite.renderCamera.transform.position.y; break;
            // case exPlane.Plane.ZY: depth = transform.position.x - sprite.renderCamera.transform.position.x; break;
            // }
            // } DELME end 

            if ( depth != depthToCamera ) {
                depthToCamera = depth;

                //
                if ( ppfCamera == null || ppfCamera.camera != sprite.renderCamera ) {
                    ppfCamera = sprite.renderCamera.GetComponent<exPixelPerfectCamera>();
                    if ( ppfCamera == null ) {
                        ppfCamera = sprite.renderCamera.gameObject.AddComponent<exPixelPerfectCamera>();
                    }
                }
                ppfCamera.MakePixelPerfect(sprite,depthToCamera);
            }
        }
    }
}
