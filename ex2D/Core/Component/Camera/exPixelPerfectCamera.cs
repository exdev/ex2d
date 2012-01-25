// ======================================================================================
// File         : exPixelPerfectCamera.cs
// Author       : Wu Jie 
// Last Change  : 10/05/2011 | 15:02:39 PM | Wednesday,October
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
/// A component to handle pixel perfect in camera
/// 
///////////////////////////////////////////////////////////////////////////////

[ExecuteInEditMode]
public class exPixelPerfectCamera : MonoBehaviour {

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public bool customResolution = false;
    public int width = 480;
    public int height = 320;

    [System.NonSerialized] public float scale = -1.0f;
    [System.NonSerialized] public float ratio = -1.0f;

    ///////////////////////////////////////////////////////////////////////////////
    // private
    ///////////////////////////////////////////////////////////////////////////////

    protected int lastScreenWidth = 0;
    protected int lastScreenHeight = 0;
    protected float lastOrthographicSize = 0.0f;
    protected float lastFieldOfView = 0.0f;
    protected bool lastOrthographic = false;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Awake () {
        lastScreenWidth         = Screen.width;
        lastScreenHeight        = Screen.height;
        lastOrthographicSize    = camera.orthographicSize;
        lastFieldOfView         = camera.fieldOfView;
        lastOrthographic        = camera.orthographic;

        CalculateScaleAndRatio ();
        exPixelPerfect[] ppfs = GameObject.FindObjectsOfType ( typeof(exPixelPerfect) ) as exPixelPerfect[];
        foreach ( exPixelPerfect ppf in ppfs ) {
            exSpriteBase sprite = ppf.GetComponent<exSpriteBase>();
            if ( sprite == null || sprite.renderCamera != this )
                continue;

            Vector3 toCamera = sprite.transform.position - transform.position;
            CalculatePixelPerfectScale ( sprite, toCamera.magnitude );
            sprite.Commit();
        }
    }
    
    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnPreRender () {
        bool changed = false;

        // check if we need to update pixel perfect scale
        if ( customResolution ) {
            if ( lastScreenWidth != width || lastScreenHeight != height ) {
                lastScreenWidth = width;
                lastScreenHeight = height;
                changed = true;
            }
        }
        else {
            if ( lastScreenWidth != Screen.width || lastScreenHeight != Screen.height ) {
                lastScreenWidth = Screen.width;
                lastScreenHeight = Screen.height;
                changed = true;
            }
        }

        //
        if ( camera.orthographic ) {
            if ( lastOrthographicSize != camera.orthographicSize ) {
                lastOrthographicSize = camera.orthographicSize;
                changed = true;
            }
        }
        else {
            if ( lastFieldOfView != camera.fieldOfView ) {
                lastFieldOfView = camera.fieldOfView;
                changed = true;
            }
        }

        //
        if ( lastOrthographic != camera.orthographic ) {
            lastOrthographic = camera.orthographic;
            changed = true;
        }

        //
        if ( changed ) {
            CalculateScaleAndRatio ();
            exPixelPerfect[] ppfs = GameObject.FindObjectsOfType ( typeof(exPixelPerfect) ) as exPixelPerfect[];
            foreach ( exPixelPerfect ppf in ppfs ) {
                exSpriteBase sprite = ppf.GetComponent<exSpriteBase>();
                if ( sprite == null || sprite.renderCamera != this )
                    continue;

                Vector3 toCamera = sprite.transform.position - transform.position;
                CalculatePixelPerfectScale ( sprite, toCamera.magnitude );
                sprite.Commit();
            }
        }
    }

    // ------------------------------------------------------------------ 
    /// calculate the pixel perfect for this sprite
    // ------------------------------------------------------------------ 

    public void CalculateScaleAndRatio () {
        if ( camera.orthographic )
            scale = 2.0f * lastOrthographicSize / lastScreenHeight;
        else
            ratio = 2.0f * Mathf.Tan(Mathf.Deg2Rad * lastFieldOfView * 0.5f) / lastScreenHeight;
    }

    // ------------------------------------------------------------------ 
    /// \param _sprite the in sprite
    /// \param _depthToCamera the depth to camera
    // ------------------------------------------------------------------ 

    public void CalculatePixelPerfectScale ( exSpriteBase _sprite, float _depthToCamera ) {
        if ( (camera.orthographic && scale <= 0.0f) || ratio <= 0.0f )
            CalculateScaleAndRatio ();

        float s = scale;
        if ( camera.orthographic == false )
            s = ratio * _depthToCamera;

        _sprite.ppfScale = new Vector2( s, s );
    }
}
