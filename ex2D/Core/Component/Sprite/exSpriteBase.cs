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
    /// Get the Bounds of the plane, used for Mesh Renderer and BoxCollider 
    // ------------------------------------------------------------------ 

    protected Bounds GetMeshBounds ( float _offsetX, float _offsetY, float _width, float _height ) {
        return new Bounds (  new Vector3( -_offsetX, _offsetY, 0.0f ), 
                             new Vector3( _width, _height, 0.2f ) );
        // DELME { 
        // switch ( plane ) {
        // case exSprite.Plane.XY:
        //     return new Bounds (  new Vector3( -_offsetX, _offsetY, 0.0f ), 
        //                          new Vector3( _width, _height, 0.2f ) );
        // case exSprite.Plane.XZ:
        //     return new Bounds (  new Vector3( -_offsetX, 0.0f, _offsetY ), 
        //                          new Vector3( _width, 0.2f, _height ) );
        // case exSprite.Plane.ZY:
        //     return new Bounds (  new Vector3( 0.0f, _offsetY, -_offsetX ), 
        //                          new Vector3( 0.2f, _height, _width ) );
        // default:
        //     return new Bounds (  new Vector3( -_offsetX, _offsetY, 0.0f ), 
        //                          new Vector3( _width, _height, 0.2f ) );
        // }
        // } DELME end 
    } 
}

