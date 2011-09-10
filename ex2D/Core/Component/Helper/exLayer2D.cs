// ======================================================================================
// File         : exLayer2D.cs
// Author       : Wu Jie 
// Last Change  : 09/01/2011 | 14:35:44 PM | Thursday,September
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
// DISABLE { 
// #if UNITY_EDITOR
// using UnityEditor;
// #endif
// } DISABLE end 

///////////////////////////////////////////////////////////////////////////////
///
/// The base layer class
///
///////////////////////////////////////////////////////////////////////////////

// DISABLE { 
// [ExecuteInEditMode]
// } DISABLE end 
public partial class exLayer2D : MonoBehaviour {

    // ------------------------------------------------------------------ 
    /// \memberof MAX_LAYER
    // ------------------------------------------------------------------ 

    public static int MAX_LAYER = 32;

    // ------------------------------------------------------------------ 
    [SerializeField] protected int layer_ = 0;
    /// layer is a value from 0 to exLayer2D.MAX_LAYER
    // ------------------------------------------------------------------ 

    public int layer { get { return layer_; } }

    // ------------------------------------------------------------------ 
    [SerializeField] protected float bias_ = 0.0f;
    /// bias is a value from 0.0f to 1.0f
    // ------------------------------------------------------------------ 

    public float bias { get { return bias_; } }

    ///////////////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////////////

    protected float depth_ = 0.0f;
    protected float depth { 
        get { return depth_; } 
        set {
            if ( Mathf.Approximately(depth_,value) == false ) {
                depth_ = value;
                RecursivelyUpdateTransformDepth ();
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////////
    // none-serialize
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    /// the cached plane
    // ------------------------------------------------------------------ 

    [System.NonSerialized] public exPlane plane;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    virtual protected float CalculateDepth ( Camera _cam ) { return 0.0f; }
    virtual protected void UpdateTransformDepth () {}

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
        UpdateDepth ();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

// DISABLE: just use Edit/ex2D/Update Scene Layers { 
//     void LateUpdate () {
// #if UNITY_EDITOR
//         if ( EditorApplication.isPlaying == false && 
//              AnimationUtility.InAnimationMode() == false ) 
//         {
//             UpdateDepth();
//             UpdateTransformDepth ();
//         }
//         else {
// #endif
//         UpdateTransformDepth ();

// #if UNITY_EDITOR
//         }
// #endif
//     }
// } DISABLE end 

    // ------------------------------------------------------------------ 
    /// \param _layer the expect layer
    /// \param _bias the expect bias
    /// set the layer and bias of the sprite
    // ------------------------------------------------------------------ 

    public void SetLayer ( int _layer, float _bias ) {
        int newLayer = Mathf.Clamp( _layer, 0, MAX_LAYER-1 );

        if ( layer_ != newLayer || Mathf.Approximately(bias_, _bias) == false ) {
            layer_ = newLayer;
            bias_ = _bias;

            UpdateDepth ();
        }
    }

    // ------------------------------------------------------------------ 
    /// Calculate and update the depth manually, useful in editor
    // ------------------------------------------------------------------ 

    public void UpdateDepth () {
        if ( plane != null )
            depth = CalculateDepth( plane.renderCamera );
        else 
            depth = CalculateDepth( Camera.main );
    }


    // ------------------------------------------------------------------ 
    /// recursively update the transform depth in child layers 
    // ------------------------------------------------------------------ 

    public void RecursivelyUpdateTransformDepth () {
        UpdateTransformDepth ();

        foreach ( Transform child in transform ) {
            exLayer2D layer = child.GetComponent<exLayer2D>();
            if ( layer ) {
                layer.RecursivelyUpdateTransformDepth ();
            }
        }
    }

    // ------------------------------------------------------------------ 
    /// recursively update child layers, this will recalculate depth, and update the transform
    // ------------------------------------------------------------------ 

    public void RecursivelyUpdateLayer () {
        UpdateDepth();
        UpdateTransformDepth();

        foreach ( Transform child in transform ) {
            exLayer2D layer = child.GetComponent<exLayer2D>();
            if ( layer ) {
                layer.RecursivelyUpdateLayer();
            }
        }
    }
}
