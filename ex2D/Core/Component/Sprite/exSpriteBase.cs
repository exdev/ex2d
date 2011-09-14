// ======================================================================================
// File         : exSpriteBase.cs
// Author       : Wu Jie 
// Last Change  : 08/06/2011 | 21:18:47 PM | Saturday,August
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

///////////////////////////////////////////////////////////////////////////////
///
/// The base class for rendering sprite by different assets
///
///////////////////////////////////////////////////////////////////////////////

[ExecuteInEditMode]
public class exSpriteBase : exPlane {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    [SerializeField] protected Vector2 scale_ = Vector2.one;
    /// the scale of the sprite
    // ------------------------------------------------------------------ 

    public Vector2 scale {
        get { return scale_; }
        set { 
            if ( scale_ != value ) {
                scale_ = value;
                updateFlags |= UpdateFlags.Vertex;
            }
        }
    }

    // ------------------------------------------------------------------ 
    [SerializeField] protected Vector2 shear_ = Vector2.zero;
    /// the shear of the sprite
    // ------------------------------------------------------------------ 

    public Vector2 shear {
        get { return shear_; }
        set { 
            if ( shear_ != value ) {
                shear_ = value;
                updateFlags |= UpdateFlags.Vertex;
            }
        }
    }

    // ------------------------------------------------------------------ 
    [SerializeField] protected bool autoResizeCollision_ = true;
    /// if the value is true and we use BoxCollider in the sprite, the 
    /// width and height of the BoxCollider will be the same as the boundingRect 
    /// of the sprite, and the thick of it will fix to 0.2f.
    // ------------------------------------------------------------------ 

    public bool autoResizeCollision {
        get { return autoResizeCollision_; }
        set {
            if ( autoResizeCollision_ != value ) {
                autoResizeCollision_ = value;
                if ( meshFilter )
                    UpdateCollider ( meshFilter_.sharedMesh );
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    /// OnEnable functoin inherit from exPlane
    // ------------------------------------------------------------------ 

    override protected void OnEnable () {
        base.OnEnable();
        exPixelPerfect ppf = GetComponent<exPixelPerfect>();
        if ( ppf ) {
            ppf.enabled = true;
        }
    }

    // ------------------------------------------------------------------ 
    /// OnDisable functoin inherit from exPlane
    // ------------------------------------------------------------------ 

    override protected void OnDisable () {
        base.OnDisable();
        exPixelPerfect ppf = GetComponent<exPixelPerfect>();
        if ( ppf ) {
            ppf.enabled = false;
        }
    }

    // ------------------------------------------------------------------ 
    /// add a MeshCollider component on the sprite if no collider exists 
    // ------------------------------------------------------------------ 

    public void AddMeshCollider () {
        if ( collider == null ) {
            gameObject.AddComponent<MeshCollider>();
            if ( meshFilter )
                UpdateCollider ( meshFilter_.sharedMesh );
        }
    }

    // ------------------------------------------------------------------ 
    /// add a BoxCollider component on the sprite if no collider exists 
    /// if the autoResizeCollision is true, it will also update the size 
    /// BoxCollider to fit the size of sprite
    // ------------------------------------------------------------------ 

    public void AddBoxCollider () {
        if ( collider == null ) {
            gameObject.AddComponent<BoxCollider>();
            if ( meshFilter )
                UpdateCollider ( meshFilter_.sharedMesh );
        }
    }

    // ------------------------------------------------------------------ 
    /// \param _mesh the mesh of the sprite  
    ///
    /// Update the size BoxCollider to fit the size of sprite, only affect 
    /// when autoResizeCollision is true
    // ------------------------------------------------------------------ 

    public void UpdateCollider ( Mesh _mesh ) {
        if ( _mesh == null || 
             collider == null || 
             autoResizeCollision == false )
            return;

        if ( collider is BoxCollider ) {
            BoxCollider boxCollider = collider as BoxCollider;
            boxCollider.center = _mesh.bounds.center;
            boxCollider.size = _mesh.bounds.size;
            return;
        }

        if ( collider is MeshCollider ) {
            MeshCollider meshCollider = collider as MeshCollider;
            // NOTE: only in this way, mesh collider changes
            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = _mesh;
            return;
        }
    }
}

