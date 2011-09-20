// ======================================================================================
// File         : exSpriteFont.cs
// Author       : Wu Jie 
// Last Change  : 07/16/2011 | 10:37:59 AM | Saturday,July
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
/// \class exSpriteFont
/// 
/// A component to render exBitmapFont in the game 
/// 
///////////////////////////////////////////////////////////////////////////////

[ExecuteInEditMode] // NOTE: without ExecuteInEditMode, we can't not drag and create mesh in the scene 
[RequireComponent (typeof(MeshRenderer))]
[RequireComponent (typeof(MeshFilter))]
[AddComponentMenu("ex2D Sprite/Sprite Font")]
public class exSpriteFont : exSpriteBase {

    // ------------------------------------------------------------------ 
    /// The alignment method for text
    // ------------------------------------------------------------------ 

    public enum TextAlign {
        Left = 0, ///< align to left
        Center,   ///< align to center
        Right     ///< align to right
    }

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    [SerializeField] protected exBitmapFont fontInfo_;
    /// The referenced bitmap font asset
    // ------------------------------------------------------------------ 

    public exBitmapFont fontInfo {
        get { return fontInfo_; }
        set {
            if ( fontInfo_ != value ) {
                fontInfo_ = value;
                if ( fontInfo_ != null && fontInfo_.pageInfos.Count == 1 )
                    renderer.sharedMaterial = fontInfo_.pageInfos[0].material;

                updateFlags |= UpdateFlags.Text;
            }
        }
    }

    // ------------------------------------------------------------------ 
    [SerializeField] protected string text_ = "Hello World!"; 
    /// The text to rendered
    // ------------------------------------------------------------------ 

    public string text {
        get { return text_; }
        set {
            if ( text_ != value ) {
                text_ = value;
                updateFlags |= UpdateFlags.Text;
            }
        }
    }

    // ------------------------------------------------------------------ 
    [SerializeField] protected bool useMultiline_ = false;
    /// If useMultiline is true, the exSpriteFont.text accept multiline string. 
    // ------------------------------------------------------------------ 

    public bool useMultiline {
        get { return useMultiline_; }
        set {
            if ( useMultiline_ != value ) {
                useMultiline_ = value;
                updateFlags |= UpdateFlags.Vertex;
            }
        }
    }

    // ------------------------------------------------------------------ 
    [SerializeField] protected TextAlign textAlign_ = TextAlign.Left;
    /// The alignment method used in the text
    // ------------------------------------------------------------------ 

    public TextAlign textAlign {
        get { return textAlign_; }
        set {
            if ( textAlign_ != value ) {
                textAlign_ = value;
                updateFlags |= UpdateFlags.Vertex;
            }
        }
    }

    // ------------------------------------------------------------------ 
    [SerializeField] protected bool useKerning_ = false;
    /// If useKerning is true, the SpriteFont will use the exBitmapFont.KerningInfo in 
    /// the exSpriteFont.fontInfo to layout the text
    // ------------------------------------------------------------------ 

    public bool useKerning {
        get { return useKerning_; }
        set {
            if ( useKerning_ != value ) {
                useKerning_ = value;
                updateFlags |= UpdateFlags.Vertex;
            }
        }
    }

    // ------------------------------------------------------------------ 
    [SerializeField] protected float tracking_ = 0.0f;
    /// A fixed width applied between two characters in the text. 
    // ------------------------------------------------------------------ 

    public float tracking {
        get { return tracking_; }
        set {
            if ( tracking_ != value ) {
                tracking_ = value;
                updateFlags |= UpdateFlags.Vertex;
            }
        }
    }

    // ------------------------------------------------------------------ 
    [SerializeField] protected float lineSpacing_ = 0.0f;
    /// A fixed line space applied between two lines.
    // ------------------------------------------------------------------ 

    public float lineSpacing {
        get { return lineSpacing_; }
        set {
            if ( lineSpacing_ != value ) {
                lineSpacing_ = value;
                updateFlags |= UpdateFlags.Vertex;
            }
        }
    }

    // color option

    // ------------------------------------------------------------------ 
    [SerializeField] protected Color topColor_ = Color.white;
    /// the color of the vertices at top 
    // ------------------------------------------------------------------ 

    public Color topColor {
        get { return topColor_; }
        set {
            if ( topColor_ != value ) {
                topColor_ = value;
                updateFlags |= UpdateFlags.Color;
            }
        }
    }

    // ------------------------------------------------------------------ 
    [SerializeField] protected Color botColor_ = Color.white;
    /// the color of the vertices at bottom 
    // ------------------------------------------------------------------ 

    public Color botColor {
        get { return botColor_; }
        set {
            if ( botColor_ != value ) {
                botColor_ = value;
                updateFlags |= UpdateFlags.Color;
            }
        }
    }

    // outline option

    // ------------------------------------------------------------------ 
    [SerializeField] protected bool useOutline_ = false;
    /// If useOutline is true, the component will render the text with outline
    // ------------------------------------------------------------------ 

    public bool useOutline {
        get { return useOutline_; }
        set {
            if ( useOutline_ != value ) {
                useOutline_ = value;
                updateFlags |= UpdateFlags.Text; 
            }
        }
    }

    // ------------------------------------------------------------------ 
    [SerializeField] protected float outlineWidth_ = 1.0f;
    /// The width of the outline text
    // ------------------------------------------------------------------ 

    public float outlineWidth {
        get { return outlineWidth_; }
        set {
            if ( outlineWidth_ != value ) {
                outlineWidth_ = value;
                if ( useOutline_ )
                    updateFlags |= UpdateFlags.Vertex;
            }
        }
    }

    // ------------------------------------------------------------------ 
    [SerializeField] protected Color outlineColor_ = Color.black;
    /// The color of the outline text
    // ------------------------------------------------------------------ 

    public Color outlineColor {
        get { return outlineColor_; }
        set {
            if ( outlineColor_ != value ) {
                outlineColor_ = value;
                if ( useOutline_ )
                    updateFlags |= UpdateFlags.Color;
            }
        }
    }

    // shadow option

    // ------------------------------------------------------------------ 
    [SerializeField] protected bool useShadow_ = false;
    /// If useShadow is true, the component will render the text with shadow
    // ------------------------------------------------------------------ 

    public bool useShadow {
        get { return useShadow_; }
        set {
            if ( useShadow_ != value ) {
                useShadow_ = value;
                updateFlags |= UpdateFlags.Text; 
            }
        }
    }

    // ------------------------------------------------------------------ 
    [SerializeField] protected Vector2 shadowBias_ = new Vector2 ( 1.0f, -1.0f );
    /// The bias of the shadow text 
    // ------------------------------------------------------------------ 

    public Vector2 shadowBias {
        get { return shadowBias_; }
        set {
            if ( shadowBias_ != value ) {
                shadowBias_ = value;
                if ( useShadow_ )
                    updateFlags |= UpdateFlags.Vertex;
            }
        }
    }

    // ------------------------------------------------------------------ 
    [SerializeField] protected Color shadowColor_ = Color.black;
    /// The color of the shadow text 
    // ------------------------------------------------------------------ 

    public Color shadowColor {
        get { return shadowColor_; }
        set {
            if ( shadowColor_ != value ) {
                shadowColor_ = value;
                if ( useShadow_ )
                    updateFlags |= UpdateFlags.Color;
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////////
    // mesh building functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void CalculateSize ( out float[] _lineWidths,
                         out float[] _kernings, 
                         out float _halfWidth,
                         out float _halfHeight,
                         out float _offsetX,
                         out float _offsetY )
    {
        if ( useMultiline_ ) {
            long lines = exStringHelper.CountLinesInString(text_);
            _lineWidths = new float[lines];
        }
        else {
            _lineWidths = new float[0];
        }
        _kernings = new float[Mathf.Max(text_.Length-1,0)];
        float maxWidth = 0.0f;
        float curWidth = 0.0f;
        float height = fontInfo_.lineHeight;

        int curLine = 0;
        for ( int i = 0; i < text_.Length; ++i ) {
            char c = text_[i];
            if ( c == '\n' ) {
                if ( useMultiline_ ) {
                    if ( curWidth > maxWidth ) {
                        maxWidth = curWidth;
                    }
                    _lineWidths[curLine] = curWidth;
                    curWidth = 0.0f;
                    height = height + fontInfo_.lineHeight * scale_.y + lineSpacing_;
                    ++curLine;
                }
                continue;
            }

            // if we don't have the character, it will become space.
            exBitmapFont.CharInfo charInfo = fontInfo_.GetCharInfo(c);
            if ( charInfo != null ) {
                curWidth = curWidth + charInfo.xadvance + tracking_;
                if ( useKerning_ ) {
                    if ( i < text_.Length - 1 ) {
                        foreach ( exBitmapFont.KerningInfo k in fontInfo_.kernings ) {
                            if ( k.first == c && k.second == text_[i+1] ) {
                                curWidth += k.amount;
                                _kernings[i] = k.amount;
                                break;
                            }
                        }
                    }
                }
            }
        }
        if ( curWidth > maxWidth ) {
            maxWidth = curWidth;
        }
        if ( useMultiline_ ) {
            _lineWidths[curLine] = curWidth;
        }

        _halfWidth = maxWidth * scale_.x * 0.5f;
        _halfHeight = height * scale_.y * 0.5f;
        _offsetX = 0.0f;
        _offsetY = 0.0f;

        // calculate anchor offset
        switch ( anchor_ ) {
        case Anchor.TopLeft     : _offsetX = -_halfWidth;  _offsetY = -_halfHeight;  break;
        case Anchor.TopCenter   : _offsetX = 0.0f;         _offsetY = -_halfHeight;  break;
        case Anchor.TopRight    : _offsetX = _halfWidth;   _offsetY = -_halfHeight;  break;

        case Anchor.MidLeft     : _offsetX = -_halfWidth;  _offsetY = 0.0f;          break;
        case Anchor.MidCenter   : _offsetX = 0.0f;         _offsetY = 0.0f;          break;
        case Anchor.MidRight    : _offsetX = _halfWidth;   _offsetY = 0.0f;          break;

        case Anchor.BotLeft     : _offsetX = -_halfWidth;  _offsetY = _halfHeight;   break;
        case Anchor.BotCenter   : _offsetX = 0.0f;         _offsetY = _halfHeight;   break;
        case Anchor.BotRight    : _offsetX = _halfWidth;   _offsetY = _halfHeight;   break;

        default                 : _offsetX = 0.0f;         _offsetY = 0.0f;          break;
        }
        _offsetX -= offset_.x;
        _offsetY += offset_.y;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void UpdateOutline ( int _vertexStartAt, 
                         int _indexStartAt, 
                         int _srcVertexStartAt,
                         Vector3[] _vertices, 
                         Vector2[] _uvs, 
                         int[] _indices ) {

        int numVerts = text_.Length * 4;
        int numIndices = text_.Length * 6;
        float length = Mathf.Sqrt(outlineWidth_*outlineWidth_*0.5f);

        for ( int i = 0; i < text_.Length; ++i ) {
            int vert_id = 4 * i;
            int idx_id = 6 * i;

            int[] vi = new int[] {
                _vertexStartAt + vert_id + 0 * numVerts,
                _vertexStartAt + vert_id + 1 * numVerts,
                _vertexStartAt + vert_id + 2 * numVerts,
                _vertexStartAt + vert_id + 3 * numVerts,
                _vertexStartAt + vert_id + 4 * numVerts,
                _vertexStartAt + vert_id + 5 * numVerts,
                _vertexStartAt + vert_id + 6 * numVerts,
                _vertexStartAt + vert_id + 7 * numVerts
            };
            int[] ii = new int[] {
                _indexStartAt + idx_id + 0 * numIndices,
                _indexStartAt + idx_id + 1 * numIndices,
                _indexStartAt + idx_id + 2 * numIndices,
                _indexStartAt + idx_id + 3 * numIndices,
                _indexStartAt + idx_id + 4 * numIndices,
                _indexStartAt + idx_id + 5 * numIndices,
                _indexStartAt + idx_id + 6 * numIndices,
                _indexStartAt + idx_id + 7 * numIndices
            };

            //
            for ( int j = 0; j < 4; ++j ) {
                int srcVertexID = _srcVertexStartAt + vert_id + j;

                //
                _vertices[vi[0] + j] = _vertices[srcVertexID] + new Vector3( -outlineWidth_, 0.0f, 0.0f );
                _vertices[vi[1] + j] = _vertices[srcVertexID] + new Vector3(  outlineWidth_, 0.0f, 0.0f );
                _vertices[vi[2] + j] = _vertices[srcVertexID] + new Vector3( 0.0f, -outlineWidth_, 0.0f );
                _vertices[vi[3] + j] = _vertices[srcVertexID] + new Vector3( 0.0f,  outlineWidth_, 0.0f );

                //
                _vertices[vi[4] + j] = _vertices[srcVertexID] + new Vector3( -length, -length, 0.0f );
                _vertices[vi[5] + j] = _vertices[srcVertexID] + new Vector3( -length,  length, 0.0f );
                _vertices[vi[6] + j] = _vertices[srcVertexID] + new Vector3(  length,  length, 0.0f );
                _vertices[vi[7] + j] = _vertices[srcVertexID] + new Vector3(  length, -length, 0.0f );

                // build uv
                if ( _uvs != null ) {
                    for ( int k = 0; k < vi.Length; ++k ) {
                        _uvs[vi[k] + j] = _uvs[srcVertexID];
                    }
                }
            }

            // build indices
            if ( _indices != null ) {
                for ( int k = 0; k < ii.Length; ++k ) {
                    _indices[ii[k] + 0] = vi[k] + 0;
                    _indices[ii[k] + 1] = vi[k] + 1;
                    _indices[ii[k] + 2] = vi[k] + 2;
                    _indices[ii[k] + 3] = vi[k] + 2;
                    _indices[ii[k] + 4] = vi[k] + 1;
                    _indices[ii[k] + 5] = vi[k] + 3;
                }
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void UpdateShadow ( int _vertexStartAt, 
                        int _indexStartAt, 
                        int _srcVertexStartAt,
                        Vector3[] _vertices, 
                        Vector2[] _uvs, 
                        int[] _indices ) {

        for ( int i = 0; i < text_.Length; ++i ) {
            int vert_id = 4 * i;
            int idx_id = 6 * i;

            int vi = _vertexStartAt + vert_id;
            int ii = _indexStartAt + idx_id;

            //
            for ( int j = 0; j < 4; ++j ) {
                int srcVertexID = _srcVertexStartAt + vert_id + j;

                //
                _vertices[vi + j] = _vertices[srcVertexID] + new Vector3( shadowBias_.x, shadowBias_.y, 0.0f );

                // build uv
                if ( _uvs != null ) {
                    _uvs[vi + j] = _uvs[srcVertexID];
                }
            }

            // build indices
            if ( _indices != null ) {
                _indices[ii + 0] = vi + 0;
                _indices[ii + 1] = vi + 1;
                _indices[ii + 2] = vi + 2;
                _indices[ii + 3] = vi + 2;
                _indices[ii + 4] = vi + 1;
                _indices[ii + 5] = vi + 3;
            }
        }
    }

    // ------------------------------------------------------------------ 
    /// \param _mesh the mesh to update
    /// 
    /// Update the _mesh depends on the exPlane.updateFlags
    // ------------------------------------------------------------------ 

    public void UpdateMesh ( Mesh _mesh ) {

        // pre check fontInfo
        if ( fontInfo_ == null ) {
            _mesh.Clear();
            return;
        }

        // ======================================================== 
        // init value 
        // ======================================================== 

        int numVerts = text_.Length * 4;
        int numIndices = text_.Length * 6;
        int vertexCount = 0;
        int indexCount = 0;

        // first shadow
        int shadowVertexStartAt = -1;
        int shadowIndexStartAt = -1;
        if ( useShadow_ ) {
            shadowVertexStartAt = vertexCount;
            vertexCount += numVerts;

            shadowIndexStartAt = indexCount;
            indexCount += numIndices;
        }

        // second outline
        int outlineVertexStartAt = -1;
        int outlineIndexStartAt = -1;
        if ( useOutline_ ) {
            outlineVertexStartAt = vertexCount;
            vertexCount += 8 * numVerts;

            outlineIndexStartAt = indexCount; 
            indexCount += 8 * numIndices;
        }

        // finally normal
        int vertexStartAt = vertexCount;
        vertexCount += numVerts;

        int indexStartAt = indexCount;
        indexCount += numIndices;

        // ======================================================== 
        // Update Vertex, UV and Indices 
        // ======================================================== 

        if ( (updateFlags & UpdateFlags.Text) != 0 ) {
            updateFlags &= ~UpdateFlags.Vertex; // remove vertex update, if we have

            _mesh.Clear();

            float[] lineWidths;
            float[] kernings;
            float halfWidth;
            float halfHeight;
            float offsetX;
            float offsetY;
            CalculateSize ( out lineWidths,
                            out kernings, 
                            out halfWidth,
                            out halfHeight,
                            out offsetX,
                            out offsetY );

            // get clip info first
            float width = 2.0f * halfWidth;
            float height = 2.0f * halfHeight;

            float clipLeft   = 0.0f; 
            float clipRight  = 0.0f; 
            float clipTop    = 0.0f; 
            float clipBottom = 0.0f;

            float xMinClip = -halfWidth * scale_.x;
            float xMaxClip =  halfWidth * scale_.x;
            float yMinClip = -halfHeight * scale_.y;
            float yMaxClip =  halfHeight * scale_.y;

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

                //
                xMinClip = scale_.x * width  * ( -0.5f + clipLeft   );
                xMaxClip = scale_.x * width  * (  0.5f - clipRight  );
                yMinClip = scale_.y * height * ( -0.5f + clipTop    );
                yMaxClip = scale_.y * height * (  0.5f - clipBottom );
            }

            //
            Vector3[] vertices  = new Vector3[vertexCount];
            Vector2[] uvs       = new Vector2[vertexCount];
            int[] indices       = new int[indexCount];

            //
            int curLine = 0;
            float curX = 0.0f;
            if ( useMultiline_ ) {
                switch ( textAlign_ ) {
                case TextAlign.Left:
                    curX = 0.0f;
                    break;
                case TextAlign.Center:
                    curX = halfWidth - lineWidths[curLine] * 0.5f * scale_.x;
                    break;
                case TextAlign.Right:
                    curX = halfWidth * 2.0f - lineWidths[curLine] * scale_.x;
                    break;
                }
            }
            float curY = 0.0f;
            for ( int i = 0; i < text_.Length; ++i ) {
                int id = text_[i];

                // if next line
                if ( id == '\n' ) {
                    if ( useMultiline_ ) {
                        ++curLine;
                        switch ( textAlign_ ) {
                        case TextAlign.Left:
                            curX = 0.0f;
                            break;
                        case TextAlign.Center:
                            curX = halfWidth - lineWidths[curLine] * 0.5f * scale_.x;
                            break;
                        case TextAlign.Right:
                            curX = halfWidth * 2.0f - lineWidths[curLine] * scale_.x;
                            break;
                        }
                        curY = curY + fontInfo_.lineHeight * scale_.y + lineSpacing_;
                    }
                    continue;
                }

                int vert_id = vertexStartAt + 4 * i;
                int idx_id = indexStartAt + 6 * i;
                // if we don't have the character, it will become space.
                exBitmapFont.CharInfo charInfo = fontInfo_.GetCharInfo(id);

                //
                float charClipLeft   = 0.0f;
                float charClipRight  = 0.0f;
                float charClipTop    = 0.0f;
                float charClipBottom = 0.0f;

                //
                if ( charInfo != null ) {
                    // build vertices & normals
                    for ( int r = 0; r < 2; ++r ) {
                        for ( int c = 0; c < 2; ++c ) {
                            int j = r * 2 + c;

                            // calculate the base pos
                            float x = curX - halfWidth + c * charInfo.width * scale_.x + charInfo.xoffset * scale_.x;
                            float y = -curY + halfHeight - r * charInfo.height * scale_.y - charInfo.yoffset * scale_.y;

                            // do clip
                            if ( clipInfo_.clipped ) {
                                //
                                if ( x <= xMinClip ) {
                                    if ( j == 0 )
                                        charClipLeft = (xMinClip - x) / charInfo.width  * scale_.x; 
                                    x = xMinClip;
                                }
                                else if ( x >= xMaxClip ) {
                                    if ( j == 3 )
                                        charClipRight = (x - xMaxClip) / charInfo.width  * scale_.x; 
                                    x = xMaxClip;
                                }

                                //
                                if ( y <= yMinClip ) {
                                    if ( j == 3 )
                                        charClipTop = (yMinClip - y) / charInfo.height * scale_.y; 
                                    y = yMinClip;
                                }
                                else if ( y >= yMaxClip ) {
                                    if ( j == 0 )
                                        charClipBottom = (y - yMaxClip) / charInfo.height * scale_.y; 
                                    y = yMaxClip;
                                }
                            }

                            // calculate the pos affect by anchor
                            x -= offsetX;
                            y += offsetY;

                            // calculate the shear
                            x += y * shear_.x;
                            y += x * shear_.y;

                            // build vertices and normals
                            switch ( plane ) {
                            case Plane.XY:
                                vertices[vert_id+j] = new Vector3( x, y, 0.0f );
                                // normals[vert_id+j] = new Vector3( 0.0f, 0.0f, -1.0f );
                                break;
                            case Plane.XZ:
                                vertices[vert_id+j] = new Vector3( x, 0.0f, y );
                                // normals[vert_id+j] = new Vector3( 0.0f, 1.0f, 0.0f );
                                break;
                            case Plane.ZY:
                                vertices[vert_id+j] = new Vector3( 0.0f, y, x );
                                // normals[vert_id+j] = new Vector3( 1.0f, 0.0f, 0.0f );
                                break;
                            }
                        }
                    }

                    // build uv
                    float textureWidth = fontInfo_.pageInfos[0].texture.width;
                    float textureHeight = fontInfo_.pageInfos[0].texture.height;
                    float charUVWidth = (float)charInfo.width/(float)textureWidth;
                    float charUVHeight = (float)charInfo.height/(float)textureHeight;

                    float xStart  = charInfo.uv0.x;
                    float yStart  = charInfo.uv0.y;
                    float xEnd    = xStart + charUVWidth; 
                    float yEnd    = yStart + charUVHeight; 

                    // do uv clip
                    if ( clipInfo_.clipped ) {
                        xStart  += charUVWidth  * charClipLeft;
                        yStart  += charUVHeight * charClipTop;
                        xEnd    -= charUVWidth  * charClipRight;
                        yEnd    -= charUVHeight * charClipBottom;
                    }

                    //
                    uvs[vert_id + 0] = new Vector2 ( xStart,  yEnd );
                    uvs[vert_id + 1] = new Vector2 ( xEnd,    yEnd );
                    uvs[vert_id + 2] = new Vector2 ( xStart,  yStart );
                    uvs[vert_id + 3] = new Vector2 ( xEnd,    yStart );

                    // build indices
                    indices[idx_id + 0] = vert_id + 0;
                    indices[idx_id + 1] = vert_id + 1;
                    indices[idx_id + 2] = vert_id + 2;
                    indices[idx_id + 3] = vert_id + 2;
                    indices[idx_id + 4] = vert_id + 1;
                    indices[idx_id + 5] = vert_id + 3;

                    //
                    curX = curX + charInfo.xadvance * scale_.x + tracking_;
                    if ( useKerning_ ) {
                        if ( i < text_.Length - 1 ) {
                            curX += kernings[i] * scale_.x;
                        }
                    }
                }
            }

            // update outline
            if ( useOutline_ ) {
                UpdateOutline ( outlineVertexStartAt, 
                                outlineIndexStartAt, 
                                vertexStartAt,
                                vertices, 
                                uvs, 
                                indices );
            }

            // update shadow
            if ( useShadow_ ) {
                UpdateShadow ( shadowVertexStartAt, 
                               shadowIndexStartAt, 
                               vertexStartAt, 
                               vertices, 
                               uvs, 
                               indices );
            }

            //
            _mesh.vertices = vertices;
            // _mesh.normals = normals;
            _mesh.uv = uvs;
            _mesh.triangles = indices; 
            _mesh.bounds = GetMeshBounds ( offsetX, offsetY, halfWidth * 2.0f, halfHeight * 2.0f );

            // update box-collider if we have
            UpdateBoundRect ( offsetX, offsetY, halfWidth * 2.0f, halfHeight * 2.0f );
            if ( collisionHelper ) 
                collisionHelper.UpdateCollider();

// #if UNITY_EDITOR
//             _mesh.RecalculateBounds();
// #endif
        }

        // ======================================================== 
        // Update Vertex Only 
        // ======================================================== 

        else if ( (updateFlags & UpdateFlags.Vertex) != 0 ) {

            float[] lineWidths;
            float[] kernings;
            float halfWidth;
            float halfHeight;
            float offsetX;
            float offsetY;
            CalculateSize ( out lineWidths,
                            out kernings, 
                            out halfWidth,
                            out halfHeight,
                            out offsetX,
                            out offsetY );

            //
            Vector3[] vertices  = new Vector3[vertexCount];

            //
            int curLine = 0;
            float curX = 0.0f;
            if ( useMultiline_ ) {
                switch ( textAlign_ ) {
                case TextAlign.Left:
                    curX = 0.0f;
                    break;
                case TextAlign.Center:
                    curX = halfWidth - lineWidths[curLine] * 0.5f * scale_.x;
                    break;
                case TextAlign.Right:
                    curX = halfWidth * 2.0f - lineWidths[curLine] * scale_.x;
                    break;
                }
            }
            float curY = 0.0f;
            for ( int i = 0; i < text_.Length; ++i ) {
                int id = text_[i];

                // if next line
                if ( id == '\n' ) {
                    if ( useMultiline_ ) {
                        ++curLine;
                        switch ( textAlign_ ) {
                        case TextAlign.Left:
                            curX = 0.0f;
                            break;
                        case TextAlign.Center:
                            curX = halfWidth - lineWidths[curLine] * 0.5f * scale_.x;
                            break;
                        case TextAlign.Right:
                            curX = halfWidth * 2.0f - lineWidths[curLine] * scale_.x;
                            break;
                        }
                        curY = curY + fontInfo_.lineHeight * scale_.y + lineSpacing_;
                    }
                    continue;
                }

                int vert_id = vertexStartAt + 4 * i;
                // if we don't have the character, it will become space.
                exBitmapFont.CharInfo charInfo = fontInfo_.GetCharInfo(id);

                if ( charInfo != null ) {
                    // build vertices & normals
                    for ( int r = 0; r < 2; ++r ) {
                        for ( int c = 0; c < 2; ++c ) {
                            int j = r * 2 + c;

                            // calculate the base pos
                            float x = curX - halfWidth + c * charInfo.width * scale_.x + charInfo.xoffset * scale_.x;
                            float y = -curY + halfHeight - r * charInfo.height * scale_.y - charInfo.yoffset * scale_.y;

                            // calculate the pos affect by anchor
                            x -= offsetX;
                            y += offsetY;

                            // calculate the shear
                            x += y * shear_.x;
                            y += x * shear_.y;

                            // build vertices
                            switch ( plane ) {
                            case Plane.XY:
                                vertices[vert_id+j] = new Vector3( x, y, 0.0f );
                                break;
                            case Plane.XZ:
                                vertices[vert_id+j] = new Vector3( x, 0.0f, y );
                                break;
                            case Plane.ZY:
                                vertices[vert_id+j] = new Vector3( 0.0f, y, x );
                                break;
                            }
                        }
                    }

                    //
                    curX = curX + charInfo.xadvance * scale_.x + tracking_;
                    if ( useKerning_ ) {
                        if ( i < text_.Length - 1 ) {
                            curX += kernings[i] * scale_.x;
                        }
                    }
                }
            }

            // update outline
            if ( useOutline_ ) {
                UpdateOutline ( outlineVertexStartAt, 
                                -1, 
                                vertexStartAt,
                                vertices, 
                                null, 
                                null );
            }

            // update shadow
            if ( useShadow_ ) {
                UpdateShadow ( shadowVertexStartAt, 
                               -1, 
                               vertexStartAt, 
                               vertices, 
                               null, 
                               null );
            }

            _mesh.vertices = vertices;
            _mesh.bounds = GetMeshBounds ( offsetX, offsetY, halfWidth * 2.0f, halfHeight * 2.0f );

            // update collider if we have
            UpdateBoundRect ( offsetX, offsetY, halfWidth * 2.0f, halfHeight * 2.0f );
            if ( collisionHelper ) 
                collisionHelper.UpdateCollider();

// #if UNITY_EDITOR
//             _mesh.RecalculateBounds();
// #endif
        }

        // ======================================================== 
        // Update Color
        // ======================================================== 

        if ( (updateFlags & UpdateFlags.Color) != 0 ||
             (updateFlags & UpdateFlags.Text) != 0 ) {
            Color[] colors = new Color[vertexCount];
            for ( int i = 0; i < text_.Length; ++i ) {
                int vert_id = vertexStartAt + 4 * i;
                colors[vert_id+0] = colors[vert_id+1] = topColor_;
                colors[vert_id+2] = colors[vert_id+3] = botColor_;


                if ( outlineVertexStartAt != -1 ) {
                    vert_id = 4 * i;
                    int[] vi = new int[] {
                        outlineVertexStartAt + vert_id + 0 * numVerts,
                        outlineVertexStartAt + vert_id + 1 * numVerts,
                        outlineVertexStartAt + vert_id + 2 * numVerts,
                        outlineVertexStartAt + vert_id + 3 * numVerts,
                        outlineVertexStartAt + vert_id + 4 * numVerts,
                        outlineVertexStartAt + vert_id + 5 * numVerts,
                        outlineVertexStartAt + vert_id + 6 * numVerts,
                        outlineVertexStartAt + vert_id + 7 * numVerts
                    };
                    for ( int k = 0; k < vi.Length; ++k ) {
                        colors[vi[k]+0] = 
                        colors[vi[k]+1] = 
                        colors[vi[k]+2] = 
                        colors[vi[k]+3] = outlineColor_;
                    }
                }
                if ( shadowVertexStartAt != -1 ) {
                    vert_id = shadowVertexStartAt + 4 * i;
                    colors[vert_id+0] = 
                    colors[vert_id+1] = 
                    colors[vert_id+2] = 
                    colors[vert_id+3] = shadowColor_;
                }
            }
            _mesh.colors = colors;
        }

        // NOTE: though we set updateFlags to None at exPlane::LateUpdate, 
        //       the Editor still need this or it will caused editor keep dirty
        updateFlags = UpdateFlags.None;
    }

    // ------------------------------------------------------------------ 
    /// \param _mesh the mesh to update
    /// 
    /// Force to update the _mesh use the Text flags in exPlane.UpdateFlags
    // ------------------------------------------------------------------ 

    public void ForceUpdateMesh ( Mesh _mesh ) {
        if ( _mesh == null )
            return;

        _mesh.Clear();
        updateFlags = UpdateFlags.Text | UpdateFlags.Color;
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

        if ( fontInfo_ != null && fontInfo_.pageInfos.Count == 1 ) {
            renderer.sharedMaterial = fontInfo_.pageInfos[0].material;

            if ( meshFilter ) {
                meshFilter_.mesh = new Mesh();
                ForceUpdateMesh (meshFilter_.sharedMesh);
            }
        }
    }

    // ------------------------------------------------------------------ 
    /// Clear the text, fontInfo, material and mesh of the sprite font, make it empty
    // ------------------------------------------------------------------ 

    public void Clear () {
        text_ = ""; 
        fontInfo_ = null;

        if ( renderer != null )
            renderer.sharedMaterial = null;

        if ( meshFilter ) {
            DestroyImmediate( meshFilter_.sharedMesh, true );
            meshFilter_.sharedMesh = null;
        }
    }
}
