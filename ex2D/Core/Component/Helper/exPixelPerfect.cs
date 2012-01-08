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
    [System.NonSerialized] public Vector3 toCamera;

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
        if ( sprite.renderCamera.orthographic == false ) {
            toCamera = transform.position - sprite.renderCamera.transform.position;
        }

        //
        ppfCamera = sprite.renderCamera.GetComponent<exPixelPerfectCamera>();
        if ( ppfCamera == null ) {
            ppfCamera = sprite.renderCamera.gameObject.AddComponent<exPixelPerfectCamera>();
        }
        ppfCamera.CalculatePixelPerfectScale ( sprite, toCamera.magnitude );
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
    // ------------------------------------------------------------------ 

    void OnDestroy () {
        sprite.ppfScale = Vector2.one; 
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // NOTE: if in LateUpdate, it may not go into sprite.Commit while changes
    // ------------------------------------------------------------------ 

    void Update () {
        if ( sprite.renderCamera.orthographic == false ) {
            Vector3 newToCamera = transform.position - sprite.renderCamera.transform.position;

            if ( newToCamera.sqrMagnitude != toCamera.sqrMagnitude ) {
                toCamera = newToCamera;

                //
                if ( ppfCamera == null || ppfCamera.camera != sprite.renderCamera ) {
                    ppfCamera = sprite.renderCamera.GetComponent<exPixelPerfectCamera>();
                    if ( ppfCamera == null ) {
                        ppfCamera = sprite.renderCamera.gameObject.AddComponent<exPixelPerfectCamera>();
                    }
                }
                ppfCamera.CalculatePixelPerfectScale ( sprite, toCamera.magnitude );
            }
        }
    }
}
