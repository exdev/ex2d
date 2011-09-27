// ======================================================================================
// File         : exSprite.cs
// Author       : Wu Jie 
// Last Change  : 06/04/2011 | 23:44:11 PM | Saturday,June
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
/// A component to render exAtlas in the game
///
///////////////////////////////////////////////////////////////////////////////

[ExecuteInEditMode] // NOTE: without ExecuteInEditMode, we can't not drag and create mesh in the scene 
[RequireComponent (typeof(MeshRenderer))]
[RequireComponent (typeof(MeshFilter))]
[AddComponentMenu("ex2D Sprite/Sprite")]
public class exSprite : exSpriteBase {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    /// the GUID of the raw texture, this only used in Editor
    // ------------------------------------------------------------------ 

    public string textureGUID = ""; 

    // ------------------------------------------------------------------ 
    /// if true, exSprite will cut out the empty color of the texture, render it in trimmed rect.
    /// 
    /// \note this value only affect when exSprite.useAtlas is false. 
    // ------------------------------------------------------------------ 

    public bool trimTexture = true;

    // ------------------------------------------------------------------ 
    /// the trimmed uv coordination of the texture exSprite used.
    /// 
    /// \note this value only affect when exSprite.useAtlas is false. 
    // ------------------------------------------------------------------ 

    public Rect trimUV = new Rect(0,0,1,1);

    // ------------------------------------------------------------------ 
    [SerializeField] protected bool useTextureOffset_ = true;
    /// if useTextureOffset is true, the sprite calculate the anchor 
    /// position depends on the original size of texture instead of the trimmed size 
    // ------------------------------------------------------------------ 

    public bool useTextureOffset {
        get { return useTextureOffset_; }
        set {
            if ( useTextureOffset_ != value ) {
                useTextureOffset_ = value;
                updateFlags |= UpdateFlags.Vertex;
            }
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

    // ------------------------------------------------------------------ 
    [SerializeField] protected bool customSize_ = false;
    /// if customSize set to true, use are free to set the exSprite.width and exSprite.height of the sprite,
    /// otherwise there is no effect when assign value to width or height.
    // ------------------------------------------------------------------ 

    public bool customSize {
        get { return customSize_; }
        set {
            if ( customSize_ != value ) {
                customSize_ = value;
                if ( customSize_ == false) {
                    float newWidth = 0.0f;
                    float newHeight = 0.0f;

                    if ( useAtlas ) {
                        exAtlas.Element el = atlas_.elements[index_];
                        newWidth = el.coords.width * atlas_.texture.width;
                        newHeight = el.coords.height * atlas_.texture.height;
                        if ( el.rotated ) {
                            float tmp = newWidth;
                            newWidth = newHeight;
                            newHeight = tmp;
                        } 
                    }
                    else {
                        Texture texture = renderer.sharedMaterial.mainTexture;
                        newWidth = trimUV.width * texture.width;
                        newHeight = trimUV.height * texture.height;
                    }

                    if ( newWidth != width_ || newHeight != height_ ) {
                        width_ = newWidth;
                        height_ = newHeight;
                        updateFlags |= UpdateFlags.Vertex;
                    }
                }
            }
        }
    }

    // ------------------------------------------------------------------ 
    [SerializeField] protected float width_ = 1.0f;
    /// the width of the sprite
    /// 
    /// \note if you want to custom the width of it, you need to set exSprite.customSize to true
    // ------------------------------------------------------------------ 

    public float width {
        get { return width_; }
        set {
            if ( width_ != value ) {
                width_ = value;
                updateFlags |= UpdateFlags.Vertex;
            }
        }
    }

    // ------------------------------------------------------------------ 
    [SerializeField] protected float height_ = 1.0f;
    /// the height of the sprite
    /// 
    /// \note if you want to custom the height of it, you need to set exSprite.customSize to true
    // ------------------------------------------------------------------ 

    public float height {
        get { return height_; }
        set {
            if ( height_ != value ) {
                height_ = value;
                updateFlags |= UpdateFlags.Vertex;
            }
        }
    }

    // ------------------------------------------------------------------ 
    [SerializeField] protected exAtlas atlas_ = null;
    /// the atlas referenced in this sprite. (readonly)
    /// 
    /// \sa exSprite.SetSprite
    // ------------------------------------------------------------------ 

    public exAtlas atlas { get { return atlas_; } }

    // ------------------------------------------------------------------ 
    [SerializeField] protected int index_ = -1;
    /// the index of the element in atlas used in this sprite. (readonly)
    /// 
    /// \sa exSprite.SetSprite
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

    ///////////////////////////////////////////////////////////////////////////////
    // non-serialize
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    /// \memberof spanim
    /// The cached exSpriteAnimation component
    // ------------------------------------------------------------------ 

    [System.NonSerialized] public exSpriteAnimation spanim;

    ///////////////////////////////////////////////////////////////////////////////
    // mesh building functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void CalculateVertex ( out float _x, out float _y, 
                           float _widthScaled,
                           float _heightScaled,
                           float _col,
                           float _row,
                           float _xMinClip, 
                           float _xMaxClip,
                           float _yMinClip, 
                           float _yMaxClip, 
                           float _offsetX, 
                           float _offsetY ) 
    {

        // calculate the base pos
        _x = _widthScaled * (_col - 0.5f);
        _y =  _heightScaled * (0.5f - _row);

        // do clip
        if ( clipInfo_.clipped ) {
            if ( _x <= _xMinClip ) {
                _x = _xMinClip;
            }
            else if ( _x >= _xMaxClip ) {
                _x = _xMaxClip;
            }

            if ( _y <= _yMinClip ) {
                _y = _yMinClip;
            }
            else if ( _y >= _yMaxClip ) {
                _y = _yMaxClip;
            }
        }

        // calculate the pos affect by anchor
        _x -= _offsetX;
        _y += _offsetY;

        // calculate the shear
        float old_x = _x;
        _x += _y * shear_.x;
        _y += old_x * shear_.y;
    }

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
            float halfWidthScaled = width_ * scale_.x * 0.5f;
            float halfHeightScaled = height_ * scale_.y * 0.5f;
            float offsetX = 0.0f;
            float offsetY = 0.0f;

            Vector3[] vertices = new Vector3[4];
            Vector3[] normals = new Vector3[4]; // TEMP

            // calculate anchor offset
            if ( useTextureOffset_ ) {
                // get original width and height
                float originalWidth = 0.0f; 
                float originalHeight = 0.0f; 
                Rect trimRect = new Rect ( 0, 0, 1, 1 );

                if ( el != null ) {
                    originalWidth   = el.originalWidth * scale_.x;
                    originalHeight  = el.originalHeight * scale_.y;
                    trimRect        = new Rect( el.trimRect.x * scale_.x, 
                                                el.trimRect.y * scale_.y, 
                                                el.trimRect.width * scale_.x, 
                                                el.trimRect.height * scale_.y );
                }
                else {
                    if ( renderer.sharedMaterial != null ) {
                        Texture texture = renderer.sharedMaterial.mainTexture;
                        originalWidth   = texture.width * scale_.x;
                        originalHeight  = texture.height * scale_.y;
                        trimRect = new Rect ( trimUV.x * originalWidth,
                                              (1.0f - trimUV.height - trimUV.y ) * originalHeight,
                                              trimUV.width * originalWidth, 
                                              trimUV.height * originalHeight );
                    }
                }

                switch ( anchor_ ) {
                    //
                case Anchor.TopLeft:
                    offsetX = -halfWidthScaled - trimRect.x;
                    offsetY = -halfHeightScaled - trimRect.y;
                    break;

                case Anchor.TopCenter:
                    offsetX = (originalWidth - trimRect.width) * 0.5f - trimRect.x;
                    offsetY = -halfHeightScaled - trimRect.y;
                    break;

                case Anchor.TopRight:    
                    offsetX = halfWidthScaled + originalWidth - trimRect.xMax;
                    offsetY = -halfHeightScaled - trimRect.y;
                    break;
                    
                    //
                case Anchor.MidLeft:
                    offsetX = -halfWidthScaled - trimRect.x;
                    offsetY = (originalHeight - trimRect.height) * 0.5f - trimRect.y;
                    break;

                case Anchor.MidCenter:
                    offsetX = (originalWidth - trimRect.width) * 0.5f - trimRect.x;
                    offsetY = (originalHeight - trimRect.height) * 0.5f - trimRect.y;
                    break;

                case Anchor.MidRight:
                    offsetX = halfWidthScaled + originalWidth - trimRect.xMax;
                    offsetY = (originalHeight - trimRect.height) * 0.5f - trimRect.y;
                    break;

                    //
                case Anchor.BotLeft:
                    offsetX = -halfWidthScaled - trimRect.x;
                    offsetY = halfHeightScaled + originalHeight - trimRect.yMax;
                    break;

                case Anchor.BotCenter: 
                    offsetX = (originalWidth - trimRect.width) * 0.5f - trimRect.x;
                    offsetY = halfHeightScaled + originalHeight - trimRect.yMax;
                    break;

                case Anchor.BotRight:
                    offsetX = halfWidthScaled + originalWidth - trimRect.xMax;
                    offsetY = halfHeightScaled + originalHeight - trimRect.yMax;
                    break;

                default:
                    offsetX = (originalWidth - trimRect.width) * 0.5f - trimRect.x;
                    offsetY = (originalHeight - trimRect.height) * 0.5f - trimRect.y;
                    break;
                }
            }
            else {
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
            }
            offsetX -= offset_.x;
            offsetY += offset_.y;

            //
            float xMinClip = scale_.x * width_  * ( -0.5f + clipLeft   );
            float xMaxClip = scale_.x * width_  * (  0.5f - clipRight  );
            float yMinClip = scale_.y * height_ * ( -0.5f + clipTop    );
            float yMaxClip = scale_.y * height_ * (  0.5f - clipBottom );

            // build vertices & normals
            switch ( plane ) {
            case Plane.XY:
                for ( int r = 0; r < 2; ++r ) {
                    for ( int c = 0; c < 2; ++c ) {
                        int i = r * 2 + c;
                        float x, y;
                        CalculateVertex( out x, out y,
                                         width_ * scale_.x, height_ * scale_.y,
                                         c, r, 
                                         xMinClip, xMaxClip, yMinClip, yMaxClip, 
                                         offsetX, offsetY );
                        vertices[i] = new Vector3( x, y, 0.0f );
                        normals[i] = new Vector3( 0.0f, 0.0f, -1.0f ); // TEMP
                    }
                }
                break;
            case Plane.XZ:
                for ( int r = 0; r < 2; ++r ) {
                    for ( int c = 0; c < 2; ++c ) {
                        int i = r * 2 + c;
                        float x, y;
                        CalculateVertex( out x, out y,
                                         width_ * scale_.x, height_ * scale_.y,
                                         c, r, 
                                         xMinClip, xMaxClip, yMinClip, yMaxClip, 
                                         offsetX, offsetY );
                        vertices[i] = new Vector3( x, 0.0f, y );
                        normals[i] = new Vector3( 0.0f, 0.0f, -1.0f ); // TEMP
                    }
                }
                break;
            case Plane.ZY:
                for ( int r = 0; r < 2; ++r ) {
                    for ( int c = 0; c < 2; ++c ) {
                        int i = r * 2 + c;
                        float x, y;
                        CalculateVertex( out x, out y,
                                         width_ * scale_.x, height_ * scale_.y,
                                         c, r, 
                                         xMinClip, xMaxClip, yMinClip, yMaxClip, 
                                         offsetX, offsetY );
                        vertices[i] = new Vector3( 0.0f, y, x );
                        normals[i] = new Vector3( 0.0f, 0.0f, -1.0f ); // TEMP
                    }
                }
                break;
            }
            _mesh.vertices = vertices;
            _mesh.normals = normals; // TEMP
            _mesh.bounds = GetMeshBounds ( offsetX, offsetY, halfWidthScaled * 2.0f, halfHeightScaled * 2.0f );

            // update collider if we have
            UpdateBoundRect ( offsetX, offsetY, halfWidthScaled * 2.0f, halfHeightScaled * 2.0f );
            if ( collisionHelper ) 
                collisionHelper.UpdateCollider();

// #if UNITY_EDITOR
//             _mesh.RecalculateBounds();
// #endif
        }

        // ======================================================== 
        // Update UV
        // ======================================================== 

        if ( (updateFlags & UpdateFlags.UV) != 0 ) {
            Vector2[] uvs = new Vector2[4];

            // if the sprite is in an atlas
            if ( el != null ) {
                float xStart  = el.coords.x;
                float yStart  = el.coords.y;
                float xEnd    = el.coords.xMax;
                float yEnd    = el.coords.yMax;

                // do uv clip
                if ( clipInfo_.clipped ) {
                    xStart  += el.coords.width  * clipLeft;
                    yStart  += el.coords.height * clipTop;
                    xEnd    -= el.coords.width  * clipRight;
                    yEnd    -= el.coords.height * clipBottom;
                }

                if ( el.rotated ) {
                    uvs[0] = new Vector2 ( xEnd,    yEnd );
                    uvs[1] = new Vector2 ( xEnd,    yStart );
                    uvs[2] = new Vector2 ( xStart,  yEnd );
                    uvs[3] = new Vector2 ( xStart,  yStart );
                }
                else {
                    uvs[0] = new Vector2 ( xStart,  yEnd );
                    uvs[1] = new Vector2 ( xEnd,    yEnd );
                    uvs[2] = new Vector2 ( xStart,  yStart );
                    uvs[3] = new Vector2 ( xEnd,    yStart );
                }
            }
            else {
                float xStart  = trimUV.x;
                float yStart  = trimUV.y;
                float xEnd    = trimUV.xMax;
                float yEnd    = trimUV.yMax;

                // do uv clip
                if ( clipInfo_.clipped ) {
                    xStart  += trimUV.width  * clipLeft;
                    yStart  += trimUV.height * clipTop;
                    xEnd    -= trimUV.width  * clipRight;
                    yEnd    -= trimUV.height * clipBottom;
                }

                uvs[0] = new Vector2 ( xStart,  yEnd );
                uvs[1] = new Vector2 ( xEnd,    yEnd );
                uvs[2] = new Vector2 ( xStart,  yStart );
                uvs[3] = new Vector2 ( xEnd,    yStart );
            }
            _mesh.uv = uvs;
        }

        // ======================================================== 
        // Update Color
        // ======================================================== 

        if ( (updateFlags & UpdateFlags.Color) != 0 ) {

            Color[] colors = new Color[4];
            for ( int i = 0; i < 4; ++i ) {
                colors[i] = color_;
            }
            _mesh.colors = colors;
        }

        // ======================================================== 
        // Update Index 
        // ======================================================== 

        if (  (updateFlags & UpdateFlags.Index) != 0 ) {
            int[] indices = new int[6];
            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 2;
            indices[3] = 2;
            indices[4] = 1;
            indices[5] = 3;
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

    override protected void OnEnable () {
        base.OnEnable();

        // NOTE: though we have ExecuteInEditMode, user can Add/Remove spanim in Editor
        if ( spanim == null ) {
            spanim = GetComponent<exSpriteAnimation>(); 
        }
        if ( spanim ) {
            spanim.enabled = true;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    override protected void OnDisable () {
        base.OnDisable();

        // NOTE: though we have ExecuteInEditMode, user can Add/Remove spanim in Editor
        if ( spanim == null ) {
            spanim = GetComponent<exSpriteAnimation>(); 
        }
        if ( spanim ) {
            spanim.enabled = false;
            spanim.Stop();
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    override protected void Awake () {

// DISABLE { 
// #if UNITY_EDITOR
//         if ( EditorApplication.isPlaying == false &&
//              string.IsNullOrEmpty(textureGUID) == false &&
//              exAtlasDBBasic.Initialized() ) 
//         {
//             exAtlasDBBasic.ElementInfo elInfo = exAtlasDBBasic.GetElementInfoNoInit ( textureGUID );
//             string path = AssetDatabase.GetAssetPath(atlas_);
//             string guid = AssetDatabase.AssetPathToGUID(path);

//             //
//             if ( elInfo != null ) {
//                 if ( elInfo.indexInAtlas != index_ || elInfo.guidAtlas != guid ) {
//                     path = AssetDatabase.GUIDToAssetPath(elInfo.guidAtlas);
//                     SetSprite( (exAtlas)AssetDatabase.LoadAssetAtPath( path, typeof(exAtlas) ),
//                                elInfo.indexInAtlas );
//                     Build(null);
//                 }
//             }
//             else {
//                 Clear();
//                 path = AssetDatabase.GUIDToAssetPath(textureGUID);
//                 Texture2D texture = (Texture2D)AssetDatabase.LoadAssetAtPath( path, typeof(Texture2D) );
//                 Build(texture);
//             }
//         }
// #endif
// } DISABLE end 

        base.Awake();
        spanim = GetComponent<exSpriteAnimation>();

        if ( atlas_ != null ||
             ( renderer.sharedMaterial && renderer.sharedMaterial.mainTexture != null ) ) 
        {
            if ( meshFilter ) {
                // create mesh ( in editor, this can duplicate mesh to prevent shared mesh for sprite)
                meshFilter_.mesh = new Mesh();
                ForceUpdateMesh( meshFilter_.sharedMesh );

                // check if update mesh collider
                MeshCollider meshCollider = collider as MeshCollider;  
                if ( meshCollider && meshCollider.sharedMesh == null ) {
                    this.UpdateColliderSize(0.2f);
                }
            }
        }
    }

    // ------------------------------------------------------------------ 
    /// Clear the altas, material and mesh of the sprite, make it empty
    // ------------------------------------------------------------------ 

    public void Clear () {
        atlas_ = null;
        index_ = -1;
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
    /// \param _atlas the new atlas
    /// \param _index the index of the element in the new atlas
    /// Set a new picture in an atlas to this sprite 
    // ------------------------------------------------------------------ 

    public void SetSprite ( exAtlas _atlas, int _index ) {
        bool checkVertex = false;

        // pre-check
        if ( _atlas == null || 
             _atlas.elements == null || 
             _index < 0 || 
             _index >= _atlas.elements.Length ) 
        {
            Debug.LogWarning ( "Invalid input in SetSprite." );
            return;
        }

        //
        if ( atlas_ != _atlas ) {
            atlas_ = _atlas;
            renderer.sharedMaterial = _atlas.material;
            updateFlags |= UpdateFlags.UV;
            checkVertex = true;
        }

        //
        if ( index_ != _index ) {
            index_ = _index;
            updateFlags |= UpdateFlags.UV;
            checkVertex = true;
        }

        //
        if ( checkVertex  ) {

            // NOTE: if we use texture offset, it always need to update vertex
            if ( useTextureOffset_ ) {
                updateFlags |= UpdateFlags.Vertex;
            }

            if ( !customSize_ ) {
                exAtlas.Element el = atlas_.elements[index_];

                float newWidth = el.trimRect.width;
                float newHeight = el.trimRect.height;
                // float newWidth = el.coords.width * atlas_.texture.width;
                // float newHeight = el.coords.height * atlas_.texture.height;

                if ( el.rotated ) {
                    float tmp = newWidth;
                    newWidth = newHeight;
                    newHeight = tmp;
                } 

                if ( newWidth != width_ || newHeight != height_ ) {
                    width_ = newWidth;
                    height_ = newHeight;
                    updateFlags |= UpdateFlags.Vertex;
                }
            }
        }
    }

    // ------------------------------------------------------------------ 
    /// horizontal flip the sprite
    // ------------------------------------------------------------------ 

    public void HFlip () { scale_.x = -scale_.x; }

    // ------------------------------------------------------------------ 
    /// vertical flip the sprite
    // ------------------------------------------------------------------ 

    public void VFlip () { scale_.y = -scale_.y; }
}
