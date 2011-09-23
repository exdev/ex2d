// ======================================================================================
// File         : exSpriteBorder.cs
// Author       : Wu Jie 
// Last Change  : 09/20/2011 | 14:49:18 PM | Tuesday,September
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
/// A component to render Scale9Grid sprite for beautiful GUI
///
///////////////////////////////////////////////////////////////////////////////

[ExecuteInEditMode] // NOTE: without ExecuteInEditMode, we can't not drag and create mesh in the scene 
[RequireComponent (typeof(MeshRenderer))]
[RequireComponent (typeof(MeshFilter))]
[AddComponentMenu("ex2D Sprite/Sprite Border")]
public class exSpriteBorder : exSpriteBase {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    [SerializeField] protected float width_ = 100.0f;
    /// the width of the sprite
    /// 
    /// \note if you want to custom the width of it, you need to set exSprite.customSize to true
    // ------------------------------------------------------------------ 

    public float width {
        get { return width_; }
        set {
            float newWidth = value; 
            if ( guiBorder_ )
                newWidth = Mathf.Max( guiBorder_.border.horizontal, newWidth );

            if ( width_ != newWidth ) {
                width_ = newWidth;
                updateFlags |= UpdateFlags.Vertex;
            }
        }
    }

    // ------------------------------------------------------------------ 
    [SerializeField] protected float height_ = 100.0f;
    /// the height of the sprite
    /// 
    /// \note if you want to custom the height of it, you need to set exSprite.customSize to true
    // ------------------------------------------------------------------ 

    public float height {
        get { return height_; }
        set {
            float newHeight = value;
            if ( guiBorder_ )
                newHeight = Mathf.Max( guiBorder_.border.vertical, newHeight );

            if ( height_ != newHeight ) {
                height_ = newHeight;
                updateFlags |= UpdateFlags.Vertex;
            }
        }
    }

    // ------------------------------------------------------------------ 
    [SerializeField] protected exGUIBorder guiBorder_ = null;
    /// the gui-border referenced in this sprite. (readonly)
    ///
    /// \sa exSpriteBorder.SetBorder
    // ------------------------------------------------------------------ 

    public exGUIBorder guiBorder { 
        get { return guiBorder_; } 
    }

    // ------------------------------------------------------------------ 
    [SerializeField] protected exAtlas atlas_ = null;
    /// the atlas referenced in this sprite. (readonly)
    /// 
    /// \sa exSprite.SetBorder
    // ------------------------------------------------------------------ 

    public exAtlas atlas { get { return atlas_; } }

    // ------------------------------------------------------------------ 
    [SerializeField] protected int index_ = -1;
    /// the index of the element in atlas used in this sprite. (readonly)
    /// 
    /// \sa exSprite.SetBorder
    // ------------------------------------------------------------------ 

    public int index { get { return index_; } }

    // ------------------------------------------------------------------ 
    /// if the sprite use atlas
    // ------------------------------------------------------------------ 

    public bool useAtlas { 
        get { 
            return ( atlas_ != null 
                     && atlas_.elements != null
                     && index_ >= 0
                     && index_ < atlas_.elements.Length ); 
        } 
    }

    // ------------------------------------------------------------------ 
    [SerializeField] protected Color color_ = Color.white;
    /// the vertex color of the sprite
    // ------------------------------------------------------------------ 

    public Color color { 
        get { return color_; } 
        set {
            if ( color_ != value ) {
                color_ = value;
                updateFlags |= UpdateFlags.Color;
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////////
    // mesh building functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    /// \param _mesh the mesh to update
    /// 
    /// Update the _mesh depends on the exPlane.updateFlags
    // ------------------------------------------------------------------ 

    public void UpdateMesh ( Mesh _mesh ) {

        exAtlas.Element el = null;
        if ( useAtlas )
            el = atlas_.elements[index_];

        // ======================================================== 
        // get clip info first
        // ======================================================== 

        float clipLeft   = 0.0f; 
        float clipRight  = 0.0f; 
        float clipTop    = 0.0f; 
        float clipBottom = 0.0f;

        if ( clipInfo_.clipped ) {
            if ( scale_.x >= 0.0f ) {
                clipLeft = clipInfo_.left;
                clipRight = clipInfo_.right;
            }
            else {
                clipLeft = clipInfo_.right;
                clipRight = clipInfo_.left;
            }

            if ( scale_.y >= 0.0f ) {
                clipTop = clipInfo_.top;
                clipBottom = clipInfo_.bottom;
            }
            else{
                clipTop = clipInfo_.bottom;
                clipBottom = clipInfo_.top;
            }
        }

        // ======================================================== 
        // Update Vertex
        // ======================================================== 

        if ( (updateFlags & UpdateFlags.Vertex) != 0 ) {
            // init
            float widthScaled = width_ * scale_.x;
            float heightScaled = height_ * scale_.y;
            float halfWidthScaled = widthScaled * 0.5f;
            float halfHeightScaled = heightScaled * 0.5f;
            float offsetX = 0.0f;
            float offsetY = 0.0f;

            Vector3[] vertices = new Vector3[16];

            switch ( anchor_ ) {
            case Anchor.TopLeft     : offsetX = -halfWidthScaled;   offsetY = -halfHeightScaled;  break;
            case Anchor.TopCenter   : offsetX = 0.0f;               offsetY = -halfHeightScaled;  break;
            case Anchor.TopRight    : offsetX = halfWidthScaled;    offsetY = -halfHeightScaled;  break;

            case Anchor.MidLeft     : offsetX = -halfWidthScaled;   offsetY = 0.0f;               break;
            case Anchor.MidCenter   : offsetX = 0.0f;               offsetY = 0.0f;               break;
            case Anchor.MidRight    : offsetX = halfWidthScaled;    offsetY = 0.0f;               break;

            case Anchor.BotLeft     : offsetX = -halfWidthScaled;   offsetY = halfHeightScaled;   break;
            case Anchor.BotCenter   : offsetX = 0.0f;               offsetY = halfHeightScaled;   break;
            case Anchor.BotRight    : offsetX = halfWidthScaled;    offsetY = halfHeightScaled;   break;

            default                 : offsetX = 0.0f;               offsetY = 0.0f;               break;
            }
            offsetX -= offset_.x;
            offsetY += offset_.y;

            //
            float xMinClip = scale_.x * width_  * ( -0.5f + clipLeft   );
            float xMaxClip = scale_.x * width_  * (  0.5f - clipRight  );
            float yMinClip = scale_.y * height_ * ( -0.5f + clipTop    );
            float yMaxClip = scale_.y * height_ * (  0.5f - clipBottom );

            float centerWidth = width_ - guiBorder_.border.horizontal;
            float centerHeight = height_ - guiBorder_.border.vertical;

            // calculate the base pos
            float x = -halfWidthScaled;
            float y = halfHeightScaled;

            float x0 = x; 
            float x1 = x0 + guiBorder_.border.left * scale_.x; 
            float x2 = x1 + centerWidth * scale_.x; 
            float x3 = x2 + guiBorder_.border.right * scale_.x; 

            float y0 = y; 
            float y1 = y0 - guiBorder_.border.top * scale_.y; 
            float y2 = y1 - centerHeight * scale_.y; 
            float y3 = y2 - guiBorder_.border.bottom * scale_.y; 

            // do clip
            if ( clipInfo_.clipped ) {
                // xMinClip
                if ( x2 <= xMinClip ) {
                    clipLeft = (xMinClip - x3) / (guiBorder_.border.right * scale_.x); 
                    x0 = xMinClip;
                    x1 = xMinClip;
                    x2 = xMinClip;
                }
                else if ( x1 <= xMinClip ) {
                    clipLeft = 0.0f; // FIXME
                    x0 = xMinClip;
                    x1 = xMinClip;
                }
                else if ( x0 <= xMinClip ) {
                    clipLeft = (xMinClip - x0) / (float)(guiBorder_.border.left * scale_.x); 
                    x0 = xMinClip;
                }

                // xMaxClip
                if ( x1 >= xMaxClip ) {
                    clipRight = (x0 - xMaxClip) / (guiBorder_.border.left * scale_.x); 
                    x1 = xMaxClip;
                    x2 = xMaxClip;
                    x3 = xMaxClip;
                }
                else if ( x2 >= xMaxClip ) {
                    clipRight = 0.0f; // FIXME
                    x2 = xMaxClip;
                    x3 = xMaxClip;
                }
                else if ( x3 >= xMaxClip ) {
                    clipRight = (x3 - xMaxClip) / (guiBorder_.border.right * scale_.x); 
                    x3 = xMaxClip;
                }

                // yMinClip
                if ( y1 <= yMinClip ) {
                    clipTop = (yMinClip - y0) / (guiBorder_.border.bottom * scale_.y); 
                    y1 = yMinClip;
                    y2 = yMinClip;
                    y3 = yMinClip;
                }
                else if ( y2 <= yMinClip ) {
                    clipTop = 0.0f; // FIXME
                    y2 = yMinClip;
                    y3 = yMinClip;
                }
                else if ( y3 <= yMinClip ) {
                    clipTop = (yMinClip - y3) / (guiBorder_.border.top * scale_.y); 
                    y3 = yMinClip;
                }

                // yMaxClip
                if ( y2 >= yMaxClip ) {
                    clipBottom = (y3 - yMaxClip) / (guiBorder_.border.top * scale_.y); 
                    y0 = yMaxClip;
                    y1 = yMaxClip;
                    y2 = yMaxClip;
                }
                else if ( y1 >= yMaxClip ) {
                    clipBottom = 0.0f; // FIXME
                    y0 = yMaxClip;
                    y1 = yMaxClip;
                }
                else if ( y0 >= yMaxClip ) {
                    clipBottom = (y0 - yMaxClip) / (guiBorder_.border.bottom * scale_.y); 
                    y0 = yMaxClip;
                }
            }

            // calculate the pos affect by anchor
            x0 -= offsetX; x1 -= offsetX; x2 -= offsetX; x3 -= offsetX;
            y0 += offsetY; y1 += offsetY; y2 += offsetY; y3 += offsetY;

            // build vertices
            switch ( plane ) {
            case Plane.XY:
                vertices[0]  = new Vector3( x0 + y0 * shear_.x, y0 + x0 * shear_.y, 0.0f );
                vertices[1]  = new Vector3( x1 + y0 * shear_.x, y0 + x1 * shear_.y, 0.0f );
                vertices[2]  = new Vector3( x2 + y0 * shear_.x, y0 + x2 * shear_.y, 0.0f );
                vertices[3]  = new Vector3( x3 + y0 * shear_.x, y0 + x3 * shear_.y, 0.0f );

                vertices[4]  = new Vector3( x0 + y1 * shear_.x, y1 + x0 * shear_.y, 0.0f );
                vertices[5]  = new Vector3( x1 + y1 * shear_.x, y1 + x1 * shear_.y, 0.0f );
                vertices[6]  = new Vector3( x2 + y1 * shear_.x, y1 + x2 * shear_.y, 0.0f );
                vertices[7]  = new Vector3( x3 + y1 * shear_.x, y1 + x3 * shear_.y, 0.0f );

                vertices[8]  = new Vector3( x0 + y2 * shear_.x, y2 + x0 * shear_.y, 0.0f );
                vertices[9]  = new Vector3( x1 + y2 * shear_.x, y2 + x1 * shear_.y, 0.0f );
                vertices[10] = new Vector3( x2 + y2 * shear_.x, y2 + x2 * shear_.y, 0.0f );
                vertices[11] = new Vector3( x3 + y2 * shear_.x, y2 + x3 * shear_.y, 0.0f );

                vertices[12] = new Vector3( x0 + y3 * shear_.x, y3 + x0 * shear_.y, 0.0f );
                vertices[13] = new Vector3( x1 + y3 * shear_.x, y3 + x1 * shear_.y, 0.0f );
                vertices[14] = new Vector3( x2 + y3 * shear_.x, y3 + x2 * shear_.y, 0.0f );
                vertices[15] = new Vector3( x3 + y3 * shear_.x, y3 + x3 * shear_.y, 0.0f );
                break;

            case Plane.XZ:
                vertices[0]  = new Vector3( x0 + y0 * shear_.x, 0.0f, y0 + x0 * shear_.y );
                vertices[1]  = new Vector3( x1 + y0 * shear_.x, 0.0f, y0 + x1 * shear_.y );
                vertices[2]  = new Vector3( x2 + y0 * shear_.x, 0.0f, y0 + x2 * shear_.y );
                vertices[3]  = new Vector3( x3 + y0 * shear_.x, 0.0f, y0 + x3 * shear_.y );

                vertices[4]  = new Vector3( x0 + y1 * shear_.x, 0.0f, y1 + x0 * shear_.y );
                vertices[5]  = new Vector3( x1 + y1 * shear_.x, 0.0f, y1 + x1 * shear_.y );
                vertices[6]  = new Vector3( x2 + y1 * shear_.x, 0.0f, y1 + x2 * shear_.y );
                vertices[7]  = new Vector3( x3 + y1 * shear_.x, 0.0f, y1 + x3 * shear_.y );

                vertices[8]  = new Vector3( x0 + y2 * shear_.x, 0.0f, y2 + x0 * shear_.y );
                vertices[9]  = new Vector3( x1 + y2 * shear_.x, 0.0f, y2 + x1 * shear_.y );
                vertices[10] = new Vector3( x2 + y2 * shear_.x, 0.0f, y2 + x2 * shear_.y );
                vertices[11] = new Vector3( x3 + y2 * shear_.x, 0.0f, y2 + x3 * shear_.y );

                vertices[12] = new Vector3( x0 + y3 * shear_.x, 0.0f, y3 + x0 * shear_.y );
                vertices[13] = new Vector3( x1 + y3 * shear_.x, 0.0f, y3 + x1 * shear_.y );
                vertices[14] = new Vector3( x2 + y3 * shear_.x, 0.0f, y3 + x2 * shear_.y );
                vertices[15] = new Vector3( x3 + y3 * shear_.x, 0.0f, y3 + x3 * shear_.y );
                break;

            case Plane.ZY:
                vertices[0]  = new Vector3( 0.0f, y0 + x0 * shear_.y, x0 + y0 * shear_.x );
                vertices[1]  = new Vector3( 0.0f, y0 + x1 * shear_.y, x1 + y0 * shear_.x );
                vertices[2]  = new Vector3( 0.0f, y0 + x2 * shear_.y, x2 + y0 * shear_.x );
                vertices[3]  = new Vector3( 0.0f, y0 + x3 * shear_.y, x3 + y0 * shear_.x );
                                                                                        
                vertices[4]  = new Vector3( 0.0f, y1 + x0 * shear_.y, x0 + y1 * shear_.x );
                vertices[5]  = new Vector3( 0.0f, y1 + x1 * shear_.y, x1 + y1 * shear_.x );
                vertices[6]  = new Vector3( 0.0f, y1 + x2 * shear_.y, x2 + y1 * shear_.x );
                vertices[7]  = new Vector3( 0.0f, y1 + x3 * shear_.y, x3 + y1 * shear_.x );
                                                                                        
                vertices[8]  = new Vector3( 0.0f, y2 + x0 * shear_.y, x0 + y2 * shear_.x );
                vertices[9]  = new Vector3( 0.0f, y2 + x1 * shear_.y, x1 + y2 * shear_.x );
                vertices[10] = new Vector3( 0.0f, y2 + x2 * shear_.y, x2 + y2 * shear_.x );
                vertices[11] = new Vector3( 0.0f, y2 + x3 * shear_.y, x3 + y2 * shear_.x );
                                                                                        
                vertices[12] = new Vector3( 0.0f, y3 + x0 * shear_.y, x0 + y3 * shear_.x );
                vertices[13] = new Vector3( 0.0f, y3 + x1 * shear_.y, x1 + y3 * shear_.x );
                vertices[14] = new Vector3( 0.0f, y3 + x2 * shear_.y, x2 + y3 * shear_.x );
                vertices[15] = new Vector3( 0.0f, y3 + x3 * shear_.y, x3 + y3 * shear_.x );
                break;
            }
            _mesh.vertices = vertices;
            _mesh.bounds = GetMeshBounds ( offsetX, offsetY, halfWidthScaled * 2.0f, halfHeightScaled * 2.0f );

            // update collider if we have
            UpdateBoundRect ( offsetX, offsetY, halfWidthScaled * 2.0f, halfHeightScaled * 2.0f );
            if ( collisionHelper ) 
                collisionHelper.UpdateCollider();
        }

        // ======================================================== 
        // Update UV
        // ======================================================== 

        if ( (updateFlags & UpdateFlags.UV) != 0 ) {
            Vector2[] uvs = new Vector2[16];

            // if the sprite is in an atlas
            if ( el != null ) {
                float xStart  = el.coords.x;
                float yStart  = el.coords.y;
                float xEnd    = el.coords.xMax;
                float yEnd    = el.coords.yMax;

                float leftRatio     = (float)guiBorder_.border.left   / (float)atlas_.texture.width; 
                float rightRatio    = (float)guiBorder_.border.right  / (float)atlas_.texture.width; 
                float topRatio      = (float)guiBorder_.border.top    / (float)atlas_.texture.height; 
                float bottomRatio   = (float)guiBorder_.border.bottom / (float)atlas_.texture.height; 

                float umin = xStart;
                float umax = xEnd;
                float vmin = yStart;
                float vmax = yEnd;

                float u0 = xStart; 
                float u1 = xStart + leftRatio; 
                float u2 = xEnd - rightRatio; 
                float u3 = xEnd;

                float v0 = yEnd; 
                float v1 = yEnd - topRatio; 
                float v2 = yStart + bottomRatio; 
                float v3 = yStart;

                // do uv clip
                if ( clipInfo_.clipped ) {
                    umin = clipLeft   >= 0.0f ? xStart + clipLeft * leftRatio     : xEnd   + clipLeft * rightRatio;
                    umax = clipRight  >= 0.0f ? xEnd   - clipRight * rightRatio   : xStart - clipRight * leftRatio;
                    vmin = clipTop    >= 0.0f ? yStart + clipTop * topRatio       : yEnd   + clipTop * bottomRatio;
                    vmax = clipBottom >= 0.0f ? yEnd   - clipBottom * bottomRatio : yStart - clipBottom * topRatio;

                    u0 = Mathf.Clamp ( u0, umin, umax ); 
                    u1 = Mathf.Clamp ( u1, umin, umax ); 
                    u2 = Mathf.Clamp ( u2, umin, umax ); 
                    u3 = Mathf.Clamp ( u3, umin, umax ); 

                    v0 = Mathf.Clamp ( v0, vmin, vmax ); 
                    v1 = Mathf.Clamp ( v1, vmin, vmax ); 
                    v2 = Mathf.Clamp ( v2, vmin, vmax ); 
                    v3 = Mathf.Clamp ( v3, vmin, vmax ); 
                }

                uvs[0]  = new Vector2 ( u0, v0 );
                uvs[1]  = new Vector2 ( u1, v0 );
                uvs[2]  = new Vector2 ( u2, v0 );
                uvs[3]  = new Vector2 ( u3, v0 );

                uvs[4]  = new Vector2 ( u0, v1 );
                uvs[5]  = new Vector2 ( u1, v1 );
                uvs[6]  = new Vector2 ( u2, v1 );
                uvs[7]  = new Vector2 ( u3, v1 );

                uvs[8]  = new Vector2 ( u0, v2 );
                uvs[9]  = new Vector2 ( u1, v2 );
                uvs[10] = new Vector2 ( u2, v2 );
                uvs[11] = new Vector2 ( u3, v2 );

                uvs[12] = new Vector2 ( u0, v3 );
                uvs[13] = new Vector2 ( u1, v3 );
                uvs[14] = new Vector2 ( u2, v3 );
                uvs[15] = new Vector2 ( u3, v3 );
            }
            else {
                float xStart  = 0.0f;
                float yStart  = 0.0f;
                float xEnd    = 1.0f;
                float yEnd    = 1.0f;

                Texture texture = renderer.sharedMaterial.mainTexture;
                float leftRatio     = (float)guiBorder_.border.left   / (float)texture.width; 
                float rightRatio    = (float)guiBorder_.border.right  / (float)texture.width; 
                float topRatio      = (float)guiBorder_.border.top    / (float)texture.height; 
                float bottomRatio   = (float)guiBorder_.border.bottom / (float)texture.height; 

                float umin = xStart;
                float umax = xEnd;
                float vmin = yStart;
                float vmax = yEnd;

                float u0 = xStart; 
                float u1 = xStart + leftRatio; 
                float u2 = xEnd - rightRatio; 
                float u3 = xEnd;

                float v0 = yEnd; 
                float v1 = yEnd - topRatio; 
                float v2 = yStart + bottomRatio; 
                float v3 = yStart;

                // do uv clip
                if ( clipInfo_.clipped ) {
                    umin = clipLeft   >= 0.0f ? xStart + clipLeft * leftRatio     : xEnd   + clipLeft * rightRatio;
                    umax = clipRight  >= 0.0f ? xEnd   - clipRight * rightRatio   : xStart - clipRight * leftRatio;
                    vmin = clipTop    >= 0.0f ? yStart + clipTop * topRatio       : yEnd   + clipTop * bottomRatio;
                    vmax = clipBottom >= 0.0f ? yEnd   - clipBottom * bottomRatio : yStart - clipBottom * topRatio;

                    u0 = Mathf.Clamp ( u0, umin, umax ); 
                    u1 = Mathf.Clamp ( u1, umin, umax ); 
                    u2 = Mathf.Clamp ( u2, umin, umax ); 
                    u3 = Mathf.Clamp ( u3, umin, umax ); 

                    v0 = Mathf.Clamp ( v0, vmin, vmax ); 
                    v1 = Mathf.Clamp ( v1, vmin, vmax ); 
                    v2 = Mathf.Clamp ( v2, vmin, vmax ); 
                    v3 = Mathf.Clamp ( v3, vmin, vmax ); 
                }

                uvs[0]  = new Vector2 ( u0, v0 );
                uvs[1]  = new Vector2 ( u1, v0 );
                uvs[2]  = new Vector2 ( u2, v0 );
                uvs[3]  = new Vector2 ( u3, v0 );

                uvs[4]  = new Vector2 ( u0, v1 );
                uvs[5]  = new Vector2 ( u1, v1 );
                uvs[6]  = new Vector2 ( u2, v1 );
                uvs[7]  = new Vector2 ( u3, v1 );

                uvs[8]  = new Vector2 ( u0, v2 );
                uvs[9]  = new Vector2 ( u1, v2 );
                uvs[10] = new Vector2 ( u2, v2 );
                uvs[11] = new Vector2 ( u3, v2 );

                uvs[12] = new Vector2 ( u0, v3 );
                uvs[13] = new Vector2 ( u1, v3 );
                uvs[14] = new Vector2 ( u2, v3 );
                uvs[15] = new Vector2 ( u3, v3 );
            }
            _mesh.uv = uvs;
        }

        // ======================================================== 
        // Update Color
        // ======================================================== 

        if ( (updateFlags & UpdateFlags.Color) != 0 ) {
            Color[] colors = new Color[16];
            for ( int i = 0; i < 16; ++i ) {
                colors[i] = color_;
            }
            _mesh.colors = colors;
        }

        // ======================================================== 
        // Update Index 
        // ======================================================== 

        if (  (updateFlags & UpdateFlags.Index) != 0 ) {
            int[] indices = new int[6*9];
            for ( int i = 0; i < 3; ++i ) {
                int vid = 4 * i;
                for ( int j = 0; j < 3; ++j ) {
                    int vert_id = vid + j;
                    int idx_id = 6 * (i*3 + j);

                    indices[idx_id + 0] = vert_id + 0;
                    indices[idx_id + 1] = vert_id + 1;
                    indices[idx_id + 2] = vert_id + 4;
                    indices[idx_id + 3] = vert_id + 4;
                    indices[idx_id + 4] = vert_id + 1;
                    indices[idx_id + 5] = vert_id + 5;
                }
            }
            _mesh.triangles = indices; 
        }

        // NOTE: though we set updateFlags to None at exPlane::LateUpdate, 
        //       the Editor still need this or it will caused editor keep dirty
        updateFlags = UpdateFlags.None;
    }

    // ------------------------------------------------------------------ 
    /// \param _mesh the mesh to update
    /// 
    /// Force to update the _mesh use the Vertex, UV, Color and Index flags in exPlane.UpdateFlags
    // ------------------------------------------------------------------ 

    public void ForceUpdateMesh ( Mesh _mesh ) {
        // pre check mesh
        if ( _mesh == null )
            return;

        _mesh.Clear();
        updateFlags = UpdateFlags.Vertex | UpdateFlags.UV | UpdateFlags.Color | UpdateFlags.Index;
        UpdateMesh( _mesh );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    override protected void InternalUpdate () {
        if ( meshFilter ) {
            if ( meshFilter_.sharedMesh != null ) {
                UpdateMesh (meshFilter_.sharedMesh);
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    override protected void Awake () {
        base.Awake();

        if ( guiBorder_ != null 
             && ( atlas_ != null 
                  || ( renderer.sharedMaterial != null && renderer.sharedMaterial.mainTexture != null ) ) ) 
        {
            if ( meshFilter ) {
                meshFilter_.mesh = new Mesh();
                ForceUpdateMesh( meshFilter_.sharedMesh );
            }
        }
    }

    // ------------------------------------------------------------------ 
    /// Clear the altas, material and mesh of the sprite, make it empty
    // ------------------------------------------------------------------ 

    public void Clear () {
        atlas_ = null;
        index_ = -1;
        guiBorder_ = null;
        if ( renderer != null )
            renderer.sharedMaterial = null;

        if ( meshFilter ) {
            DestroyImmediate( meshFilter_.sharedMesh, true );
            meshFilter_.sharedMesh = null;
        }
    }

    // ------------------------------------------------------------------ 
    /// \return the current used atlas element 
    ///
    /// Get current element used in exSprite.atlas
    // ------------------------------------------------------------------ 

    public exAtlas.Element GetCurrentElement () {
        if ( useAtlas )
            return atlas_.elements[index_];
        return null;
    }

    // ------------------------------------------------------------------ 
    /// \param _border the new gui-border
    /// \param _atlas the new atlas
    /// \param _index the index of the element in the new atlas
    /// Set a new picture in an atlas to this sprite 
    // ------------------------------------------------------------------ 

    public void SetBorder ( exGUIBorder _border, exAtlas _atlas, int _index ) {
        // pre-check
        if ( _border == null 
             || ( _atlas != null 
                  && ( _atlas.elements == null 
                       || _index < 0 
                       || _index >= _atlas.elements.Length ) ) ) 
        {
            Debug.LogWarning ( "Invalid input in SetBorder." );
            return;
        }

        //
        if ( guiBorder_ != _border ) {
            if ( guiBorder_ == null || guiBorder_.border != _border.border )
                updateFlags |= UpdateFlags.Vertex;
            guiBorder_ = _border;
        }

        //
        if ( atlas_ != _atlas ) {
            atlas_ = _atlas;
            if ( atlas_ )
                renderer.sharedMaterial = _atlas.material;
            updateFlags |= UpdateFlags.UV;
        }

        //
        if ( index_ != _index ) {
            index_ = _index;
            updateFlags |= UpdateFlags.UV;
        }
    }
}

