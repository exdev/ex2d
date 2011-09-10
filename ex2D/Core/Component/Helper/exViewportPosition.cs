// ======================================================================================
// File         : exViewportPosition.cs
// Author       : Wu Jie 
// Last Change  : 08/06/2011 | 21:36:15 PM | Saturday,August
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
/// A component to position a plane in viewport space
/// 
///////////////////////////////////////////////////////////////////////////////

[ExecuteInEditMode]
[AddComponentMenu("ex2D Helper/Viewport Position")]
public class exViewportPosition : MonoBehaviour {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    [SerializeField] protected float x_;
    /// the screen position x
    // ------------------------------------------------------------------ 

    public float x {
        get { return x_; }
        set {
            if ( value != x_ )
                x_ = value;
        }
    }

    // ------------------------------------------------------------------ 
    [SerializeField] protected float y_;
    /// the screen position y
    // ------------------------------------------------------------------ 

    public float y {
        get { return y_; }
        set {
            if ( value != y_ )
                y_ = value;
        }
    }

    ///////////////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    /// The cached plane component
    // ------------------------------------------------------------------ 

    [System.NonSerialized] public exPlane plane;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // DISABLE { 
    // // ------------------------------------------------------------------ 
    // // Desc: 
    // //  example: CalculateWorldPosition(Camera.main,Screen.width, Screen.height) 
    // // ------------------------------------------------------------------ 

    // Vector3 CalculateWorldPosition ( Camera _camera, float _screenWidth, float _screenHeight ) {
    //     float s = 1.0f;
    //     if ( _camera.orthographic ) {
    //         s =  2.0f * _camera.orthographicSize / _screenHeight;
    //     }
    //     else {
    //         float ratio = 2.0f * Mathf.Tan(Mathf.Deg2Rad * _camera.fov * 0.5f) / _screenHeight;
    //         s = ratio * ( transform.position.z - _camera.transform.position.z );
    //     }

    //     return new Vector3( (x_ - 0.5f) * _screenWidth * s + _camera.transform.position.x,
    //                         (y_ - 0.5f) * _screenHeight * s + _camera.transform.position.y,
    //                         transform.position.z );
    // }
    // } DISABLE end 

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Awake () {
        plane = GetComponent<exPlane>();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnEnable () {
        if ( plane == null ) {
            plane = GetComponent<exPlane>();
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void LateUpdate () {
        //
        Vector3 newPos = Vector3.zero;

        //
        if ( plane ) {
            newPos = plane.ViewportToWorldPoint ( plane.renderCamera, x_, y_ );
        }
        else {
            newPos = Camera.main.ViewportToWorldPoint( new Vector3(x_, y_, transform.position.z) );
            newPos.z = transform.position.z;
        }

        //
        if ( newPos != transform.position ) {
            transform.position = newPos;
        }
    }
}
