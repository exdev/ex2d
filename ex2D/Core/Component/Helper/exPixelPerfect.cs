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
    [System.NonSerialized] public Vector3 cameraToSprite;

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
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Start () {
        ppfCamera = sprite.renderCamera.GetComponent<exPixelPerfectCamera>();
        if ( ppfCamera == null ) {
            ppfCamera = sprite.renderCamera.gameObject.AddComponent<exPixelPerfectCamera>();
        }

        UpdatePixelPerfectCamera (ppfCamera);
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
        if ( sprite != null )
            sprite.ppfScale = Vector2.one; 
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void UpdatePixelPerfectCamera ( exPixelPerfectCamera _ppfCamera ) {
        ppfCamera = _ppfCamera;
        if ( sprite.renderCamera.orthographic == false ) {
            cameraToSprite = transform.position - sprite.renderCamera.transform.position;
        }

        //
        ppfCamera.CalculatePixelPerfectScale ( sprite, cameraToSprite );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // NOTE: if in LateUpdate, it may not go into sprite.Commit while changes
    // ------------------------------------------------------------------ 

    void Update () {
        if ( sprite != null && sprite.renderCamera.orthographic == false ) {
            Vector3 newCameraToSprite = transform.position - sprite.renderCamera.transform.position;

            if ( newCameraToSprite.sqrMagnitude != cameraToSprite.sqrMagnitude ) {
                cameraToSprite = newCameraToSprite;

                //
                if ( ppfCamera == null || ppfCamera.camera != sprite.renderCamera ) {
                    ppfCamera = sprite.renderCamera.GetComponent<exPixelPerfectCamera>();
                    if ( ppfCamera == null ) {
                        ppfCamera = sprite.renderCamera.gameObject.AddComponent<exPixelPerfectCamera>();
                    }
                }
                ppfCamera.CalculatePixelPerfectScale ( sprite, cameraToSprite );
            }
        }
    }
}
