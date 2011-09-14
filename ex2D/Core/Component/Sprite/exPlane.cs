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

    // ------------------------------------------------------------------ 
    /// The 2D plane the exPlane used in 3D space
    // ------------------------------------------------------------------ 

    public enum Plane {
        XY, ///< the plane in XY space
        XZ, ///< the plane in XZ space
        ZY  ///< the plane in ZY space
    }

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
        get { return camera_; }
        set {
            Camera newCamera = value;
            if ( newCamera == null )
                newCamera = Camera.main;
            if ( newCamera != camera_ ) {
                camera_ = newCamera;
                if ( layer2d )
                    layer2d_.UpdateDepth();
            }
        }
    }

    // ------------------------------------------------------------------ 
    [SerializeField] protected Plane plane_ = Plane.XY;
    /// the 2D coordination (XY, XZ or ZY) used in this plane 
    // ------------------------------------------------------------------ 

    public Plane plane {
        get { return plane_; }
        set {
            if ( plane_ != value ) {
                plane_ = value;

                int layer = 0;
                float bias = 0.0f;

                if ( layer2d ) {
                    layer = layer2d_.layer; 
                    bias = layer2d_.bias; 
                    Object.DestroyImmediate(layer2d_,true);

                    switch ( plane_ ) {
                    case exPlane.Plane.XY: layer2d_ = gameObject.AddComponent<exLayerXY>(); break;
                    case exPlane.Plane.XZ: layer2d_ = gameObject.AddComponent<exLayerXZ>(); break;
                    case exPlane.Plane.ZY: layer2d_ = gameObject.AddComponent<exLayerZY>(); break;
                    }
                    layer2d_.SetLayer( layer, bias );
                }

                //
                updateFlags |= UpdateFlags.Vertex;
            }
        }
    }

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

    ///////////////////////////////////////////////////////////////////////////////
    // Non Serialized
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    /// The cached exLayer2D component
    // ------------------------------------------------------------------ 

    protected exLayer2D layer2d_ = null;
    public exLayer2D layer2d {
        get {
            if ( layer2d_ == null )
                layer2d_ = GetComponent<exLayer2D>();
            return layer2d_;
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

	[System.NonSerialized] public UpdateFlags updateFlags = UpdateFlags.None;

    // ------------------------------------------------------------------ 
    /// The bounding rect of the plane
    // ------------------------------------------------------------------ 

    public Rect boundingRect { get; protected set; }

    protected ClipInfo clipInfo_ = new ClipInfo();
    public ClipInfo clipInfo { 
        get { return clipInfo_; }
        set {
            if ( clipInfo_ != value ) {
                clipInfo_ = value;

                if ( clipInfo_.clipped ) {
                    if ( clipInfo_.left >= 1.0f ||
                         clipInfo_.right >= 1.0f ||
                         clipInfo_.top >= 1.0f ||
                         clipInfo_.bottom >= 1.0f )
                    {
                        enabled = false; // just hide it
                    }
                    else {
                        enabled = true;
                        updateFlags |= (UpdateFlags.Vertex|UpdateFlags.UV|UpdateFlags.Text);
                    }
                }
                else {
                    enabled = true;
                    updateFlags |= (UpdateFlags.Vertex|UpdateFlags.UV|UpdateFlags.Text);
                }
            }
        } 
    }

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Reset() {
        if ( layer2d == null ) {
            switch ( plane ) {
            case exPlane.Plane.XY: layer2d_ = gameObject.AddComponent<exLayerXY>(); break;
            case exPlane.Plane.XZ: layer2d_ = gameObject.AddComponent<exLayerXZ>(); break;
            case exPlane.Plane.ZY: layer2d_ = gameObject.AddComponent<exLayerZY>(); break;
            }
            layer2d_.plane = this;
            layer2d_.UpdateDepth();
        }
    }

    // ------------------------------------------------------------------ 
    /// Awake functoin inherit from MonoBehaviour.
    /// 
    /// \note if you inherit from exPlane, and implement your own Awake function, 
    /// you need to override this and call base.Awake() in your Awake block.
    // ------------------------------------------------------------------ 

    virtual protected void Awake () {
        if ( camera_ == null )
            camera_ = Camera.main;
        meshFilter_ = GetComponent<MeshFilter>();
        layer2d_ = GetComponent<exLayer2D>();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnDestroy () {
        // NOTE: though we have ExecuteInEditMode, user can Add/Remove layer2d in Editor
        if ( meshFilter ) {
            DestroyImmediate( meshFilter.sharedMesh, true );
        }
    }

    // ------------------------------------------------------------------ 
    /// OnEnable functoin inherit from MonoBehaviour,
    /// When exPlane.enabled set to true, this function will be invoked,
    /// exPlane will enable the renderer and layer2d if they exist. 
    /// 
    /// \note if you inherit from exPlane, and implement your own Awake function, 
    /// you need to override this and call base.OnEnable() in your OnEnable block.
    // ------------------------------------------------------------------ 

    virtual protected void OnEnable () {
        if ( renderer != null )
            renderer.enabled = true;

        // NOTE: though we have ExecuteInEditMode, user can Add/Remove layer2d in Editor
        if ( layer2d ) {
            layer2d_.enabled = true;
        }
    }

    // ------------------------------------------------------------------ 
    /// OnDisable functoin inherit from MonoBehaviour,
    /// When exPlane.enabled set to false, this function will be invoked,
    /// exPlane will disable the renderer and layer2d if they exist. 
    /// 
    /// \note if you inherit from exPlane, and implement your own Awake function, 
    /// you need to override this and call base.OnDisable() in your OnDisable block.
    // ------------------------------------------------------------------ 

    virtual protected void OnDisable () {
        if ( renderer != null )
            renderer.enabled = false;

        // NOTE: though we have ExecuteInEditMode, user can Add/Remove layer2d in Editor
        if ( layer2d ) {
            layer2d_.enabled = false;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void LateUpdate () {
        if ( updateFlags != UpdateFlags.None ) {
            InternalUpdate();
            updateFlags = UpdateFlags.None;
        }
    }

    // ------------------------------------------------------------------ 
    /// A virtual for user to override.
    /// It will be invoked when updateFlags is not UpdateFlags.None
    // ------------------------------------------------------------------ 

    virtual protected void InternalUpdate () {
        // Debug.LogWarning ("You should not directly call this function. please override it!");
    }

    // ------------------------------------------------------------------ 
    /// \param _offsetX the offset x pos of the plane (normally it is equals to offset.x + anchor_pos.x )  
    /// \param _offsetY the offset y pos of the plane (normally it is equals to offset.y + anchor_pos.y )  
    /// \param _width the width of the plane
    /// \param _height the height of the plane
    /// 
    /// Update the boundingRect of the plane.
    // ------------------------------------------------------------------ 

    protected void UpdateBoundRect ( float _offsetX, float _offsetY, float _width, float _height ) {
        float sign_w = Mathf.Sign(_width);
        float sign_h = Mathf.Sign(_height);
        boundingRect = new Rect( -_offsetX - sign_w * _width * 0.5f, 
                                  _offsetY - sign_h * _height * 0.5f, 
                                  sign_w * _width, 
                                  sign_h * _height );
    }

    // ------------------------------------------------------------------ 
    /// \param _offsetX the offset x pos of the plane (normally it is equals to offset.x + anchor_pos.x )  
    /// \param _offsetY the offset y pos of the plane (normally it is equals to offset.y + anchor_pos.y )  
    /// \param _width the width of the plane
    /// \param _height the height of the plane
    /// \return the bounds result
    /// 
    /// Get the Bounds of the plane, used for BoxCollider 
    // ------------------------------------------------------------------ 

    protected Bounds GetBounds ( float _offsetX, float _offsetY, float _width, float _height ) {
        switch ( plane ) {
        case exSprite.Plane.XY:
            return new Bounds (  new Vector3( -_offsetX, _offsetY, 0.0f ), 
                                 new Vector3( _width, _height, 0.2f ) );
        case exSprite.Plane.XZ:
            return new Bounds (  new Vector3( -_offsetX, 0.0f, _offsetY ), 
                                 new Vector3( _width, 0.2f, _height ) );
        case exSprite.Plane.ZY:
            return new Bounds (  new Vector3( 0.0f, _offsetY, -_offsetX ), 
                                 new Vector3( 0.2f, _height, _width ) );
        default:
            return new Bounds (  new Vector3( -_offsetX, _offsetY, 0.0f ), 
                                 new Vector3( _width, _height, 0.2f ) );
        }
    } 
}
