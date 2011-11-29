// ======================================================================================
// File         : exPlane.cs
// Author       : Wu Jie 
// Last Change  : 09/03/2011 | 11:47:44 AM | Saturday,September
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
///
/// The base class of sprites in ex2D
///
///////////////////////////////////////////////////////////////////////////////

[ExecuteInEditMode]
public class exPlane : MonoBehaviour {

    // ------------------------------------------------------------------ 
    /// The type of update
    // ------------------------------------------------------------------ 

	[System.FlagsAttribute]
	public enum UpdateFlags {
		None		= 0,  ///< none
		Vertex		= 1,  ///< update the vertices
		UV	        = 2,  ///< update the uv coordination
		Color	    = 4,  ///< update the vertex color
		Text	    = 8,  ///< update the text
		Index	    = 16, ///< update the indices
	};

    // DELME { 
    // // ------------------------------------------------------------------ 
    // /// The 2D plane the exPlane used in 3D space
    // // ------------------------------------------------------------------ 

    // public enum Plane {
    //     XY, ///< the plane in XY space
    //     XZ, ///< the plane in XZ space
    //     ZY  ///< the plane in ZY space
    // }
    // } DELME end 

    // ------------------------------------------------------------------ 
    /// The anchor position of the exPlane in 2D space
    // ------------------------------------------------------------------ 

    public enum Anchor {
		TopLeft = 0, ///< the top-left of the plane  
		TopCenter,   ///< the top-center of the plane
		TopRight,    ///< the top-right of the plane
		MidLeft,     ///< the middle-left of the plane
		MidCenter,   ///< the middle-center of the plane
		MidRight,    ///< the middle-right of the plane
		BotLeft,     ///< the bottom-left of the plane
		BotCenter,   ///< the bottom-center of the plane
		BotRight,    ///< the bottom-right of the plane
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [System.Serializable]
    public class ClipInfo {

        public static bool operator == ( ClipInfo _a, ClipInfo _b ) { return _a.Equals(_b); }
        public static bool operator != ( ClipInfo _a, ClipInfo _b ) { return !_a.Equals(_b); }

        public bool clipped = false; 
        public float top    = 0.0f; // percentage of clipped top
        public float bottom = 0.0f; // percentage of clipped bottom
        public float left   = 0.0f; // percentage of clipped left
        public float right  = 0.0f; // percentage of clipped right

        public override int GetHashCode() { 
            return Mathf.FloorToInt(top * 10.0f) 
                ^ Mathf.FloorToInt(bottom * 10.0f) 
                ^ Mathf.FloorToInt(left * 10.0f) 
                ^ Mathf.FloorToInt(right * 10.0f) 
                ;
        }
        public override bool Equals ( object _obj ) {
            if ( !(_obj is ClipInfo) )
                return false;

            return Equals((ClipInfo)_obj);
        }
        public bool Equals ( ClipInfo _other ) {
            if ( clipped != _other.clipped ||
                 top != _other.top ||
                 bottom != _other.bottom ||
                 left != _other.left ||
                 right != _other.right )
            {
                return false;
            }
            return true;
        }
    }

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    [SerializeField] protected Camera camera_;
    /// The camera you used to calculate the pixel perfect scale
    // ------------------------------------------------------------------ 

    public Camera renderCamera {
        get { 
            // NOTE: this is because prefab may missing link of main camera ( but will not missing second one )
            if ( camera_ != null )
                return camera_; 
            return Camera.main; 
        }
        set {
            Camera newCamera = value;
            if ( newCamera != camera_ ) {
                camera_ = newCamera;

                // update sprite manager
                spriteMng_ = camera_.GetComponent<exSpriteMng>();
                if ( spriteMng_ == null )
                    spriteMng_ = camera_.gameObject.AddComponent<exSpriteMng>();
            }
        }
    }

    // DELME { 
    // // ------------------------------------------------------------------ 
    // [SerializeField] protected Plane plane_ = Plane.XY;
    // /// the 2D coordination (XY, XZ or ZY) used in this plane 
    // // ------------------------------------------------------------------ 

    // public Plane plane {
    //     get { return plane_; }
    //     set {
    //         if ( plane_ != value ) {
    //             plane_ = value;

    //             //
    //             updateFlags |= UpdateFlags.Vertex;
    //         }
    //     }
    // }
    // } DELME end 

    // ------------------------------------------------------------------ 
    [SerializeField] protected Anchor anchor_ = Anchor.MidCenter;
    /// the anchor position used in this plane
    // ------------------------------------------------------------------ 

    public Anchor anchor {
        get { return anchor_; }
        set {
            if ( anchor_ != value ) {
                anchor_ = value;
                updateFlags |= UpdateFlags.Vertex;
            }
        }
    }

    // ------------------------------------------------------------------ 
    [SerializeField] protected Vector2 offset_ = Vector2.zero;
    /// the offset based on the anchor, the final position of the plane equals to offset + anchor
    // ------------------------------------------------------------------ 

    public Vector2 offset {
        get { return offset_; }
        set { 
            if ( offset_ != value ) {
                offset_ = value;
                updateFlags |= UpdateFlags.Vertex;
            }
        }
    }

    // ------------------------------------------------------------------ 
    /// The cached exCollisionHelper component
    // ------------------------------------------------------------------ 

    protected exCollisionHelper collisionHelper_ = null;
    public exCollisionHelper collisionHelper {
        get {
            if ( collisionHelper_ == null )
                collisionHelper_ = GetComponent<exCollisionHelper>();
            return collisionHelper_;
        }
    }

    // ------------------------------------------------------------------ 
    /// The cached MeshFilter component
    // ------------------------------------------------------------------ 

    protected MeshFilter meshFilter_ = null;
    public MeshFilter meshFilter {
        get {
            if ( meshFilter_ == null )
                meshFilter_ = GetComponent<MeshFilter>();
            return meshFilter_;
        }
    }

    // ------------------------------------------------------------------ 
    /// The current updateFlags, this value will reset after every LateUpdate()
    /// 
    /// \note The only reason I public this is because exAnimationHelper need to asscess it, 
    /// user should only change this in class derived from exPlane.
    // ------------------------------------------------------------------ 

    protected UpdateFlags updateFlags_ = UpdateFlags.None;
	public UpdateFlags updateFlags {
        set {
            updateFlags_ = value;
            if ( updateFlags_ != UpdateFlags.None ) {
                if ( spriteMng != null ) {
                    spriteMng_.AddToCommitList(this);
                }
            }
        }
        get { return updateFlags_; }
    }

    // ------------------------------------------------------------------ 
    /// The bounding rect of the plane
    // ------------------------------------------------------------------ 

    public Rect boundingRect { get; protected set; }

    protected ClipInfo clipInfo_ = new ClipInfo();
    public ClipInfo clipInfo { 
        get { return clipInfo_; }
        set {
            if ( value.clipped ) {
                if ( value.left >= 1.0f ||
                     value.right >= 1.0f ||
                     value.top >= 1.0f ||
                     value.bottom >= 1.0f )
                {
                    if ( enabled == true )
                        enabled = false; // just hide it
                }
                else {
                    if ( clipInfo_ != value || enabled == false ) {
                        enabled = true;
                        updateFlags |= (UpdateFlags.Vertex|UpdateFlags.UV|UpdateFlags.Text);
                    }
                }
            }
            else {
                if ( clipInfo_ != value || enabled == false ) {
                    enabled = true;
                    updateFlags |= (UpdateFlags.Vertex|UpdateFlags.UV|UpdateFlags.Text);
                }
            }

            clipInfo_ = value;
        } 
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected exSpriteMng spriteMng_ = null;
    protected exSpriteMng spriteMng {
        get {
            if ( camera_ == null )
                return null;

            if ( spriteMng_ == null ) {
                spriteMng_ = camera_.GetComponent<exSpriteMng>();
                if ( spriteMng_ == null )
                    spriteMng_ = camera_.gameObject.AddComponent<exSpriteMng>();
            }
            return spriteMng_;
        }
    }

    [System.NonSerialized] public bool inCommitList = false;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    /// Awake functoin inherit from MonoBehaviour.
    /// 
    /// \note if you inherit from exPlane, and implement your own Awake function, 
    /// you need to override this and call base.Awake() in your Awake block.
    // ------------------------------------------------------------------ 

    virtual protected void Awake () {
        //
        if ( camera_ == null )
            camera_ = Camera.main;

        //
        if ( camera_ != null ) {
            spriteMng_ = camera_.GetComponent<exSpriteMng>();
            if ( spriteMng_ == null )
                spriteMng_ = camera_.gameObject.AddComponent<exSpriteMng>();
        }

        //
        meshFilter_ = GetComponent<MeshFilter>();

        //
        collisionHelper_ = GetComponent<exCollisionHelper>();
        if ( collisionHelper_ )
            collisionHelper_.plane = this;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnDestroy () {
        if ( meshFilter ) {
            DestroyImmediate( meshFilter.sharedMesh, true );
        }
    }

    // ------------------------------------------------------------------ 
    /// OnEnable functoin inherit from MonoBehaviour,
    /// When exPlane.enabled set to true, this function will be invoked,
    /// exPlane will enable the renderer if they exist. 
    /// 
    /// \note if you inherit from exPlane, and implement your own Awake function, 
    /// you need to override this and call base.OnEnable() in your OnEnable block.
    // ------------------------------------------------------------------ 

    virtual protected void OnEnable () {
        if ( renderer != null )
            renderer.enabled = true;
    }

    // ------------------------------------------------------------------ 
    /// OnDisable functoin inherit from MonoBehaviour,
    /// When exPlane.enabled set to false, this function will be invoked,
    /// exPlane will disable the renderer if they exist. 
    /// 
    /// \note if you inherit from exPlane, and implement your own Awake function, 
    /// you need to override this and call base.OnDisable() in your OnDisable block.
    // ------------------------------------------------------------------ 

    virtual protected void OnDisable () {
        if ( renderer != null )
            renderer.enabled = false;
    }

    // DISABLE { 
    // // ------------------------------------------------------------------ 
    // // Desc: 
    // // ------------------------------------------------------------------ 

    // void LateUpdate () {
    //     if ( updateFlags != UpdateFlags.None ) {
    //         Commit();
    //         updateFlags = UpdateFlags.None;
    //     }
    // }
    // } DISABLE end 

    // ------------------------------------------------------------------ 
    /// A virtual for user to override.
    /// It will be invoked when updateFlags is not UpdateFlags.None
    // ------------------------------------------------------------------ 

    virtual public void Commit () {
        // Debug.LogWarning ("You should not directly call this function. please override it!");
    }

}
