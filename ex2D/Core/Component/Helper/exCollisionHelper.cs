// ======================================================================================
// File         : exCollisionHelper.cs
// Author       : Wu Jie 
// Last Change  : 09/19/2011 | 17:32:59 PM | Monday,September
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
///
/// collision helper
///
///////////////////////////////////////////////////////////////////////////////

[ExecuteInEditMode]
public class exCollisionHelper : MonoBehaviour {

    // ------------------------------------------------------------------ 
    /// collision type
    // ------------------------------------------------------------------ 

	[System.FlagsAttribute]
    public enum CollisionType {
        None   = 0,    ///< no collision
        Boxed  = 1,    ///< collision box
        Mesh   = 2,    ///< collision mesh
    }

    ///////////////////////////////////////////////////////////////////////////////
    // Serialized
    ///////////////////////////////////////////////////////////////////////////////

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
                UpdateCollider ();
            }
        }
    }

    // ------------------------------------------------------------------ 
    [SerializeField] protected bool autoLength_ = true;
    /// if the value is true and we use BoxCollider in the sprite, the 
    /// length of the BoxCollider will locked to camera.farClipPlane - Camera.nearClipPlane
    // ------------------------------------------------------------------ 

    public bool autoLength {
        get { return autoLength_; }
        set {
            if ( autoLength_ != value ) {
                autoLength_ = value;
                UpdateCollider ();
            }
        }
    }


    // ------------------------------------------------------------------ 
    [SerializeField] protected float length_ = 0.2f;
    /// the length of the collision box
    // ------------------------------------------------------------------ 

    public float length {
        get { return length_; }
        set {
            if ( length_ != value ) {
                length_ = value;
                UpdateCollider ();
            }
        }
    }


    ///////////////////////////////////////////////////////////////////////////////
    // Non Serialized
    ///////////////////////////////////////////////////////////////////////////////

    [System.NonSerialized] public exPlane plane;

    // ------------------------------------------------------------------ 
    // 
    // ------------------------------------------------------------------ 

    void Awake () {
        plane = GetComponent<exPlane>();
    }

    // ------------------------------------------------------------------ 
    /// add a MeshCollider component on the sprite if no collider exists 
    // ------------------------------------------------------------------ 

    public void AddMeshCollider () {
        if ( collider == null ) {
            gameObject.AddComponent<MeshCollider>();
            UpdateCollider ();
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
            UpdateCollider ();
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void UpdateCollider () {
        UpdateSize ();
        UpdateCenter ();
    }

    // ------------------------------------------------------------------ 
    /// Update the length of the collider if we use auto length
    // ------------------------------------------------------------------ 

    public void UpdateCenter () {
        if ( autoLength_ == false ||
             collider == null )
            return;

        if ( plane == null )
            plane = GetComponent<exPlane>();

        // update box collider
        if ( collider is BoxCollider ) {
            BoxCollider boxCollider = collider as BoxCollider;

            Camera camera = plane.renderCamera;

            float myLength = camera.farClipPlane - camera.nearClipPlane;
            float offset = 0.0f;
            float thick = camera.farClipPlane - camera.nearClipPlane;

            switch ( plane.plane ) {
            case exSprite.Plane.XY:
                offset = myLength * 0.5f + (camera.transform.position.z + camera.nearClipPlane) - transform.position.z;
                boxCollider.center = new Vector3( boxCollider.center.x, boxCollider.center.y, offset );
                boxCollider.size = new Vector3( boxCollider.size.x, boxCollider.size.y, thick );
                break;

            case exSprite.Plane.XZ:
                offset = myLength * 0.5f + transform.position.y - (camera.transform.position.y + camera.nearClipPlane);
                boxCollider.center = new Vector3( boxCollider.center.x, -offset, boxCollider.center.z );
                boxCollider.size = new Vector3( boxCollider.size.x, thick, boxCollider.size.z );
                break;

            case exSprite.Plane.ZY:
                offset = myLength * 0.5f + transform.position.x - (camera.transform.position.x + camera.nearClipPlane);
                boxCollider.center = new Vector3( -offset, boxCollider.center.x, boxCollider.center.z );
                boxCollider.size = new Vector3( thick, boxCollider.size.y, boxCollider.size.z );
                break;
            }

            return;
        }
    }

    // ------------------------------------------------------------------ 
    /// Update the size BoxCollider to fit the size of sprite, only affect 
    /// when autoResizeCollision is true
    // ------------------------------------------------------------------ 

    public void UpdateSize () {

        if ( plane == null )
            plane = GetComponent<exPlane>();

        if ( plane.meshFilter == null||
             plane.meshFilter.sharedMesh == null || 
             collider == null || 
             autoResizeCollision == false )
            return;

        plane.UpdateColliderSize(length_);
    }

}
