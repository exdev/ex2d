// ======================================================================================
// File         : exSoftClip.cs
// Author       : Wu Jie 
// Last Change  : 08/25/2011 | 01:48:47 AM | Thursday,August
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
[AddComponentMenu("ex2D Sprite/Soft Clip")]
public class exSoftClip : exPlane {

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
    /// the list of the planes to clip
    // ------------------------------------------------------------------ 

    public List<exPlane> planes = new List<exPlane>();

    // ------------------------------------------------------------------ 
    /// the clipped rect, if the soft-clip plane is a child of another soft-clip plane
    // ------------------------------------------------------------------ 

    public Rect clippedRect { get; protected set; }

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////
    
    // ------------------------------------------------------------------ 
    /// update the list of planes to clip
    // ------------------------------------------------------------------ 

    public void UpdateClipList () {
        planes.Clear();
        if ( transform.childCount > 0 )
            RecursivelyAddToClip (transform);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void RecursivelyAddToClip ( Transform _t ) {
        foreach ( Transform child in _t ) {
            exPlane plane = child.GetComponent<exPlane>();
            if ( plane != null ) {
                planes.Add(plane);
                exSoftClip clipPlane = plane as exSoftClip;
                // if this is a clip plane, add child to it 
                if ( clipPlane != null ) {
                    clipPlane.UpdateClipList ();
                    continue;
                }
            }
            RecursivelyAddToClip (child);
        }
    }

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    /// Awake functoin inherit from exPlane.
    // ------------------------------------------------------------------ 

    override protected void Awake () {
        base.Awake();
        updateFlags |= UpdateFlags.Vertex;
        Commit();
    }

    // ------------------------------------------------------------------ 
    /// OnEnable functoin inherit from exPlane.
    /// When enabled set to true, it will enable all the item in the planes
    // ------------------------------------------------------------------ 

    override protected void OnEnable () {
        base.OnEnable();

        for ( int i = 0; i < planes.Count; ++i ) {
            exPlane p = planes[i];
            if ( p == null ) {
                planes.RemoveAt(i);
                --i;
                continue;
            }
            p.enabled = true;
        }
    }

    // ------------------------------------------------------------------ 
    /// OnDisable functoin inherit from exPlane.
    /// When enabled set to false, it will disable all the item in the planes
    // ------------------------------------------------------------------ 

    override protected void OnDisable () {
        base.OnDisable();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    override public void Commit () {

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
            boundingRect = new Rect( -offsetX - halfWidth, 
                                      offsetY - halfHeight,
                                      width_, 
                                      height_ );

            // do clip
            if ( clipInfo_.clipped ) {
                clippedRect = new Rect( boundingRect.x + clipInfo_.left * boundingRect.width, 
                                        boundingRect.y + clipInfo_.top * boundingRect.height, 
                                        (1.0f - clipInfo_.left - clipInfo_.right) * boundingRect.width,
                                        (1.0f - clipInfo_.top - clipInfo_.bottom) * boundingRect.height
                                      ); 
            }
            else {
                clippedRect = boundingRect;
            }

            if ( collisionHelper ) 
                collisionHelper.UpdateCollider();
        }

        //
        updateFlags = UpdateFlags.None;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Update () {
        spriteMng.AddToSoftClipList(this);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void UpdateClipInfo () {
        //
        Rect a = clippedRect;
        a.x += transform.position.x;
        a.y += transform.position.y;

        // DELME { 
        // switch ( plane ) {
        // case exSprite.Plane.XY:
        //     a.x += transform.position.x;
        //     a.y += transform.position.y;
        //     break;
        // case exSprite.Plane.XZ:
        //     a.x += transform.position.x;
        //     a.y += transform.position.z;
        //     break;
        // case exSprite.Plane.ZY:
        //     a.x += transform.position.z;
        //     a.y += transform.position.y;
        //     break;
        // }
        // } DELME end 

        //
        for ( int i = 0; i < planes.Count; ++i ) {
            exPlane p = planes[i];
            if ( p == null ) {
                planes.RemoveAt(i);
                --i;
                continue;
            }

            exPlane.ClipInfo newClipInfo = new exPlane.ClipInfo(); 

            //
            Rect b = p.boundingRect;
            b.x += p.transform.position.x;
            b.y += p.transform.position.y;

            // DELME { 
            // switch ( plane ) {
            // case exSprite.Plane.XY:
            //     b.x += p.transform.position.x;
            //     b.y += p.transform.position.y;
            //     break;
            // case exSprite.Plane.XZ:
            //     b.x += p.transform.position.x;
            //     b.y += p.transform.position.z;
            //     break;
            // case exSprite.Plane.ZY:
            //     b.x += p.transform.position.z;
            //     b.y += p.transform.position.y;
            //     break;
            // }
            // } DELME end 

            //
            if ( a.xMin > b.xMin ) {
                newClipInfo.left = (a.xMin - b.xMin) / b.width;
                newClipInfo.clipped = true;
            }
            if ( b.xMax > a.xMax ) {
                newClipInfo.right = (b.xMax - a.xMax) / b.width;
                newClipInfo.clipped = true;
            }

            if ( a.yMin > b.yMin ) {
                newClipInfo.top = (a.yMin - b.yMin) / b.height;
                newClipInfo.clipped = true;
            }
            if ( b.yMax > a.yMax ) {
                newClipInfo.bottom = (b.yMax - a.yMax) / b.height;
                newClipInfo.clipped = true;
            }
            p.clipInfo = newClipInfo;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnDrawGizmos () {
        //
        Vector3 center_v3 = transform.position;
        Vector3 size_v3 = Vector3.zero;
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
        float x = offset_.x - offsetX;
        float y = offset_.y + offsetY;

        center_v3 += new Vector3( x, y, 0.0f );
        size_v3 = new Vector3 ( width_, height_, 0.0f );

        // DELME { 
        // switch ( plane ) {
        // case exPlane.Plane.XY:
        //     center_v3 += new Vector3( x, y, 0.0f );
        //     size_v3 = new Vector3 ( width_, height_, 0.0f );
        //     break;
        // case exPlane.Plane.XZ:
        //     center_v3 += new Vector3( x, 0.0f, y );
        //     size_v3 = new Vector3 ( width_, 0.0f, height_ );
        //     break;
        // case exPlane.Plane.ZY:
        //     center_v3 += new Vector3( 0.0f, y, x );
        //     size_v3 = new Vector3 ( 0.0f, height_, width_ );
        //     break;
        // }
        // } DELME end 

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube ( center_v3, size_v3 );
        // Gizmos.color = new Color ( 1.0f, 1.0f, 0.0f, 0.0001f ); // this is very hack
        // Gizmos.DrawCube ( center_v3, new Vector3 ( size.x, size.y, 0.0f ) );
    }
}

