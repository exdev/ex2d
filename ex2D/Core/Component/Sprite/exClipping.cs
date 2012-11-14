// ======================================================================================
// File         : exClipping.cs
// Author       : Wu Jie 
// Last Change  : 03/05/2012 | 19:19:47 PM | Monday,March
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////
///
/// A component handles the a list of exPlane GameObjects, clip them
/// to the boundingRect.
///
///////////////////////////////////////////////////////////////////////////////

[ExecuteInEditMode]
[AddComponentMenu("ex2D Sprite/Clipping")]
public class exClipping : exPlane {

    [System.Serializable]
    public class PlaneInfo {
        public exPlane plane = null;
        public Material material = null;
    }  

    ///////////////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    [SerializeField] protected float width_ = 100.0f;
    /// the width of the soft-clip
    // ------------------------------------------------------------------ 

    public float width {
        get { return width_; }
        set {
            if ( width_ != value ) {
                width_ = Mathf.Max(value, 0.0f);
                updateFlags |= UpdateFlags.Vertex;
            }
        }
    }

    // ------------------------------------------------------------------ 
    [SerializeField] protected float height_ = 100.0f;
    /// the height of the soft-clip
    // ------------------------------------------------------------------ 

    public float height {
        get { return height_; }
        set {
            if ( height_ != value ) {
                height_ = Mathf.Max(value, 0.0f);
                updateFlags |= UpdateFlags.Vertex;
            }
        }
    }

    // ------------------------------------------------------------------ 
    /// is dynamic clipping plane, the dynamic plane will update clip material list at runtime if you remove plane from clipping list 
    // ------------------------------------------------------------------ 

    public bool isDyanmic = false;

    // ------------------------------------------------------------------ 
    /// the list of the planeInfoList to clip
    // ------------------------------------------------------------------ 

    public List<PlaneInfo> planeInfoList = new List<PlaneInfo>();
    public List<Material> clipMaterialList = new List<Material>();
    [System.NonSerialized] public Dictionary<Texture2D,Material> textureToClipMaterialTable = new Dictionary<Texture2D,Material>(); 

    // ------------------------------------------------------------------ 
    /// the clipped rect, if the clipping plane is a child of another soft-clip plane
    // ------------------------------------------------------------------ 

    public Rect clippedRect { get; protected set; }


    ///////////////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////////////

    protected bool initialized = false;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected void Init () {
        if ( textureToClipMaterialTable.Count == 0 )
            initialized = false;

        if ( initialized == false ) {
            initialized = true;

            spriteMng.AddToClippingList(this);
            for ( int i = 0; i < clipMaterialList.Count; ++i ) {
                Material mat = clipMaterialList[i];
                textureToClipMaterialTable.Add( mat.mainTexture as Texture2D, mat );
            }

            updateFlags |= UpdateFlags.Vertex;
            Commit();
            CommitMaterialProperties();
        }
    }

    // ------------------------------------------------------------------ 
    /// Awake functoin inherit from exPlane.
    // ------------------------------------------------------------------ 

    protected new void Awake () {
        base.Awake();
        Init ();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected new void OnDestroy () {
        base.OnDestroy();

        if ( spriteMng != null ) {
            spriteMng_.RemoveFromClippingList(this);
        }

        // 
        Clear();
    }

    // ------------------------------------------------------------------ 
    /// OnEnable functoin inherit from exPlane.
    /// When enabled set to true, it will enable all the item in the planeInfoList
    // ------------------------------------------------------------------ 

    protected new void OnEnable () {
        base.OnEnable();
        Init ();

        // 
        for ( int i = 0; i < planeInfoList.Count; ++i ) {
            PlaneInfo pi = planeInfoList[i];
            exPlane plane = pi.plane;
            if ( plane && plane.renderer && plane.renderer.sharedMaterial ) {
                Texture2D texture = plane.renderer.sharedMaterial.mainTexture as Texture2D;
                plane.renderer.material = textureToClipMaterialTable[texture];
            }
        }
    }

    // ------------------------------------------------------------------ 
    /// OnDisable functoin inherit from exPlane.
    /// When enabled set to false, it will disable all the item in the planeInfoList
    // ------------------------------------------------------------------ 

    protected new void OnDisable () {
        base.OnDisable();

        // 
        for ( int i = 0; i < planeInfoList.Count; ++i ) {
            PlaneInfo pi = planeInfoList[i];
            if ( pi.plane && pi.plane.renderer )
                pi.plane.renderer.sharedMaterial = pi.material;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public bool HasPlaneInfo ( exPlane _plane ) {
        for ( int i = 0; i < planeInfoList.Count; ++i ) {
            if ( planeInfoList[i].plane == _plane )
                return true;
        }
        return false;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected void AddPlaneInfo ( exPlane _plane ) {
        PlaneInfo planeInfo = new PlaneInfo();
        planeInfo.plane = _plane;
        if ( _plane.renderer != null )
            planeInfo.material = _plane.renderer.sharedMaterial;
        planeInfoList.Add(planeInfo);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected void InsertPlaneInfo ( int _idx, exPlane _plane ) {
        PlaneInfo planeInfo = new PlaneInfo();
        planeInfo.plane = _plane;
        if ( _plane.renderer != null )
            planeInfo.material = _plane.renderer.sharedMaterial;
        planeInfoList.Insert(_idx,planeInfo);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected bool RemovePlaneInfo ( exPlane _plane ) {
        for ( int i = 0; i < planeInfoList.Count; ++i ) {
            PlaneInfo pi = planeInfoList[i];
            if ( _plane ==  pi.plane ) {
                if ( _plane.renderer != null )
                    _plane.renderer.sharedMaterial = pi.material;
                planeInfoList.RemoveAt(i);
                return true;
            }
        }
        return false;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void UpdatePlane ( exPlane _plane ) {
        PlaneInfo planeInfo = null;
        for ( int i = 0; i < planeInfoList.Count; ++i ) {
            if ( planeInfoList[i].plane == _plane ) {
                planeInfo = planeInfoList[i];
                break;
            }
        }
        if ( planeInfo == null ) {
            Debug.LogWarning( "Can't find plane info of " + _plane.name );
            return;
        }

        // update plane info material
        if ( _plane.renderer != null )
            planeInfo.material = _plane.renderer.sharedMaterial;

        // if we are in player or if we are running in editor
        if ( Application.isPlaying ) {
            exClipping clipPlane = _plane as exClipping;
            // if this is not a clip plane
            if ( clipPlane == null ) {
                ApplyClipMaterial (_plane);
            }
        }
    }
    
    // ------------------------------------------------------------------ 
    /// add plane to clipping list
    // ------------------------------------------------------------------ 

    public void AddPlane ( exPlane _plane ) {
        // we already have this in clipping list
        if ( HasPlaneInfo(_plane) )
            return;

        if ( _plane.clippingPlane != null ) {
            _plane.clippingPlane.RemovePlane (_plane);
        }
        AddPlaneInfo(_plane);
        _plane.clippingPlane = this;

        // if we are in player or if we are running in editor
        if ( Application.isPlaying ) {
            exClipping clipPlane = _plane as exClipping;
            // if this is not a clip plane
            if ( clipPlane == null ) {
                ApplyClipMaterial (_plane);
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void InsertPlane ( int _idx, exPlane _plane ) {
        // we already have this in clipping list
        if ( HasPlaneInfo(_plane) )
            return;

        if ( _plane.clippingPlane != null ) {
            _plane.clippingPlane.RemovePlane (_plane);
        }
        InsertPlaneInfo(_idx,_plane);
        _plane.clippingPlane = this;

        // if we are in player or if we are running in editor
        if ( Application.isPlaying ) {
            exClipping clipPlane = _plane as exClipping;
            // if this is not a clip plane
            if ( clipPlane == null ) {
                ApplyClipMaterial (_plane);
            }
        }
    }

    // ------------------------------------------------------------------ 
    /// remove plane from clipping list
    // ------------------------------------------------------------------ 

    public bool RemovePlane ( exPlane _plane ) {
        bool result = RemovePlaneInfo(_plane);
        if ( result ) {
            _plane.clippingPlane = null;

            if ( isDyanmic || (Application.isPlaying == false ) ) {
                if ( _plane.renderer && _plane.renderer.sharedMaterial ) {
                    CheckAndRemoveClipMaterial(_plane.renderer.sharedMaterial.mainTexture as Texture2D);
                }
            }
        }
        return result;
    }

    // ------------------------------------------------------------------ 
    /// remove all planes from clipping list
    // ------------------------------------------------------------------ 

    public void Clear () {
        for ( int i = 0; i < planeInfoList.Count; ++i ) {
            PlaneInfo pi = planeInfoList[i];
            if ( pi.plane && pi.plane.renderer && pi.plane.renderer.sharedMaterial  )
                pi.plane.renderer.sharedMaterial = pi.material;
        }
        planeInfoList.Clear();
        textureToClipMaterialTable.Clear();
        clipMaterialList.Clear();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected void ApplyClipMaterial ( exPlane _plane ) {
        Renderer r = _plane.renderer;
        if ( r != null && r.sharedMaterial != null ) {
            Texture2D texture = r.sharedMaterial.mainTexture as Texture2D;
            if ( texture != null ) {
                if ( textureToClipMaterialTable.ContainsKey(texture) == false ) {
                    r.material = new Material( Shader.Find("ex2D/Alpha Blended (Clipping)") );
                    r.material.mainTexture = texture;
                    AddClipMaterial ( texture, r.material );
                }
                else {
                    r.material = textureToClipMaterialTable[texture];
                }
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void CheckAndRemoveClipMaterial ( Texture2D _texture ) {
        bool hasPlaneUseIt = false;
        for ( int i = 0; i < planeInfoList.Count; ++i ) {
            PlaneInfo pi = planeInfoList[i];
            if ( pi.material != null && pi.material.mainTexture == _texture ) {
                hasPlaneUseIt = true;
                break;
            }
        }
        if ( hasPlaneUseIt == false ) {
            Material clipMaterial = textureToClipMaterialTable[_texture];
            textureToClipMaterialTable.Remove(_texture);
            clipMaterialList.Remove(clipMaterial);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public override void Commit () {
        if ( (updateFlags & UpdateFlags.Vertex) != 0 ) {
            //
            float halfWidth = width_ * 0.5f;
            float halfHeight = height_ * 0.5f;
            float offsetX = 0.0f;
            float offsetY = 0.0f;

            //
            switch ( anchor ) {
            case Anchor.TopLeft     : offsetX = -halfWidth;   offsetY = -halfHeight;  break;
            case Anchor.TopCenter   : offsetX = 0.0f;         offsetY = -halfHeight;  break;
            case Anchor.TopRight    : offsetX = halfWidth;    offsetY = -halfHeight;  break;

            case Anchor.MidLeft     : offsetX = -halfWidth;   offsetY = 0.0f;         break;
            case Anchor.MidCenter   : offsetX = 0.0f;         offsetY = 0.0f;         break;
            case Anchor.MidRight    : offsetX = halfWidth;    offsetY = 0.0f;         break;

            case Anchor.BotLeft     : offsetX = -halfWidth;   offsetY = halfHeight;   break;
            case Anchor.BotCenter   : offsetX = 0.0f;         offsetY = halfHeight;   break;
            case Anchor.BotRight    : offsetX = halfWidth;    offsetY = halfHeight;   break;

            default                 : offsetX = 0.0f;         offsetY = 0.0f;         break;
            }

            //
            boundingRect = new Rect( -offsetX - halfWidth - offset.x, 
                                      offsetY - halfHeight + offset.y,
                                      width_, 
                                      height_ );

            // TODO: child clip { 
            // // do clip
            // if ( clipInfo_.clipped ) {
            //     clippedRect = new Rect( boundingRect.x + clipInfo_.left * boundingRect.width, 
            //                             boundingRect.y + clipInfo_.top * boundingRect.height, 
            //                             (1.0f - clipInfo_.left - clipInfo_.right) * boundingRect.width,
            //                             (1.0f - clipInfo_.top - clipInfo_.bottom) * boundingRect.height
            //                           ); 
            // }
            // else {
            //     clippedRect = boundingRect;
            // }
            // } TODO end 
            clippedRect = boundingRect;

            if ( collisionHelper ) 
                collisionHelper.UpdateCollider();
        }

        //
        updateFlags = UpdateFlags.None;
    }

    // TODO: for sub clip { 
    // // ------------------------------------------------------------------ 
    // // Desc: 
    // // ------------------------------------------------------------------ 

    // public void UpdateClipInfo () {
    //     //
    //     Rect a = clippedRect;
    //     a.x += transform.position.x;
    //     a.y += transform.position.y;

    //     // DELME { 
    //     // switch ( plane ) {
    //     // case exSprite.Plane.XY:
    //     //     a.x += transform.position.x;
    //     //     a.y += transform.position.y;
    //     //     break;
    //     // case exSprite.Plane.XZ:
    //     //     a.x += transform.position.x;
    //     //     a.y += transform.position.z;
    //     //     break;
    //     // case exSprite.Plane.ZY:
    //     //     a.x += transform.position.z;
    //     //     a.y += transform.position.y;
    //     //     break;
    //     // }
    //     // } DELME end 

    //     //
    //     for ( int i = 0; i < planes.Count; ++i ) {
    //         exPlane p = planes[i];
    //         if ( p == null ) {
    //             planes.RemoveAt(i);
    //             --i;
    //             continue;
    //         }

    //         exPlane.ClipInfo newClipInfo = new exPlane.ClipInfo(); 

    //         //
    //         Rect b = p.boundingRect;
    //         b.x += p.transform.position.x;
    //         b.y += p.transform.position.y;

    //         // DELME { 
    //         // switch ( plane ) {
    //         // case exSprite.Plane.XY:
    //         //     b.x += p.transform.position.x;
    //         //     b.y += p.transform.position.y;
    //         //     break;
    //         // case exSprite.Plane.XZ:
    //         //     b.x += p.transform.position.x;
    //         //     b.y += p.transform.position.z;
    //         //     break;
    //         // case exSprite.Plane.ZY:
    //         //     b.x += p.transform.position.z;
    //         //     b.y += p.transform.position.y;
    //         //     break;
    //         // }
    //         // } DELME end 

    //         //
    //         if ( a.xMin > b.xMin ) {
    //             newClipInfo.left = (a.xMin - b.xMin) / b.width;
    //             newClipInfo.clipped = true;
    //         }
    //         if ( b.xMax > a.xMax ) {
    //             newClipInfo.right = (b.xMax - a.xMax) / b.width;
    //             newClipInfo.clipped = true;
    //         }

    //         if ( a.yMin > b.yMin ) {
    //             newClipInfo.top = (a.yMin - b.yMin) / b.height;
    //             newClipInfo.clipped = true;
    //         }
    //         if ( b.yMax > a.yMax ) {
    //             newClipInfo.bottom = (b.yMax - a.yMax) / b.height;
    //             newClipInfo.clipped = true;
    //         }
    //         p.clipInfo = newClipInfo;
    //     }
    // }
    // } TODO end 

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void AddClipMaterial ( Texture2D _texture, Material _mat ) {
        if ( textureToClipMaterialTable.ContainsKey(_texture) == false ) {
            textureToClipMaterialTable.Add( _texture, _mat );
            if ( clipMaterialList.IndexOf(_mat) == -1 )
                clipMaterialList.Add(_mat);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void CommitMaterialProperties () {
        Vector4 rect = new Vector4 ( clippedRect.center.x,
                                     clippedRect.center.y,
                                     clippedRect.width, 
                                     clippedRect.height ); 

        for ( int i = 0; i < clipMaterialList.Count; ++i ) {
            Material mat = clipMaterialList[i];
            mat.SetVector ( "_ClipRect", rect );
            mat.SetMatrix ( "_ClipMatrix", transform.worldToLocalMatrix );
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnDrawGizmos () {
        //
        Vector3[] vertices = new Vector3[4]; 
        Vector3[] corners = new Vector3[4];

        float halfWidthScaled = width * 0.5f;
        float halfHeightScaled = height * 0.5f;
        float offsetX = 0.0f;
        float offsetY = 0.0f;

        //
        switch ( anchor ) {
        case exPlane.Anchor.TopLeft     : offsetX = -halfWidthScaled;   offsetY = -halfHeightScaled;  break;
        case exPlane.Anchor.TopCenter   : offsetX = 0.0f;               offsetY = -halfHeightScaled;  break;
        case exPlane.Anchor.TopRight    : offsetX = halfWidthScaled;    offsetY = -halfHeightScaled;  break;

        case exPlane.Anchor.MidLeft     : offsetX = -halfWidthScaled;   offsetY = 0.0f;               break;
        case exPlane.Anchor.MidCenter   : offsetX = 0.0f;               offsetY = 0.0f;               break;
        case exPlane.Anchor.MidRight    : offsetX = halfWidthScaled;    offsetY = 0.0f;               break;

        case exPlane.Anchor.BotLeft     : offsetX = -halfWidthScaled;   offsetY = halfHeightScaled;   break;
        case exPlane.Anchor.BotCenter   : offsetX = 0.0f;               offsetY = halfHeightScaled;   break;
        case exPlane.Anchor.BotRight    : offsetX = halfWidthScaled;    offsetY = halfHeightScaled;   break;

        default                         : offsetX = 0.0f;               offsetY = 0.0f;               break;
        }
        offsetX += offset.x;
        offsetY += offset.y;

        //
        vertices[0] = new Vector3 (-halfWidthScaled-offsetX,  halfHeightScaled+offsetY, 0.0f );
        vertices[1] = new Vector3 ( halfWidthScaled-offsetX,  halfHeightScaled+offsetY, 0.0f );
        vertices[2] = new Vector3 ( halfWidthScaled-offsetX, -halfHeightScaled+offsetY, 0.0f );
        vertices[3] = new Vector3 (-halfWidthScaled-offsetX, -halfHeightScaled+offsetY, 0.0f );

        // 0 -- 1
        // |    |
        // 3 -- 2

        corners[0] = transform.localToWorldMatrix * new Vector4 ( vertices[0].x, vertices[0].y, vertices[0].z, 1.0f );
        corners[1] = transform.localToWorldMatrix * new Vector4 ( vertices[1].x, vertices[1].y, vertices[1].z, 1.0f );
        corners[2] = transform.localToWorldMatrix * new Vector4 ( vertices[2].x, vertices[2].y, vertices[2].z, 1.0f );
        corners[3] = transform.localToWorldMatrix * new Vector4 ( vertices[3].x, vertices[3].y, vertices[3].z, 1.0f );

        Gizmos.color = Color.red;
        Gizmos.DrawLine ( corners[0], corners[1] );
        Gizmos.DrawLine ( corners[1], corners[2] );
        Gizmos.DrawLine ( corners[2], corners[3] );
        Gizmos.DrawLine ( corners[3], corners[0] );
    }
}

