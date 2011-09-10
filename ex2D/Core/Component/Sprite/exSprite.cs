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
            float halfWidth = width_ * scale_.x * 0.5f;
            float halfHeight = height_ * scale_.y * 0.5f;
            float offsetX = 0.0f;
            float offsetY = 0.0f;

            Vector3[] vertices = new Vector3[4];
            Vector3[] normals = new Vector3[4]; // TEMP

            // calculate anchor offset
            if ( useTextureOffset ) {
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
                    offsetX = -halfWidth - trimRect.x;
                    offsetY = -halfHeight - trimRect.y;
                    break;

                case Anchor.TopCenter:
                    offsetX = (originalWidth - trimRect.width) * 0.5f - trimRect.x;
                    offsetY = -halfHeight - trimRect.y;
                    break;

                case Anchor.TopRight:    
                    offsetX = halfWidth + originalWidth - trimRect.xMax;
                    offsetY = -halfHeight - trimRect.y;
                    break;
                    
                    //
                case Anchor.MidLeft:
                    offsetX = -halfWidth - trimRect.x;
                    offsetY = (originalHeight - trimRect.height) * 0.5f - trimRect.y;
                    break;

                case Anchor.MidCenter:
                    offsetX = (originalWidth - trimRect.width) * 0.5f - trimRect.x;
                    offsetY = (originalHeight - trimRect.height) * 0.5f - trimRect.y;
                    break;

                case Anchor.MidRight:
                    offsetX = halfWidth + originalWidth - trimRect.xMax;
                    offsetY = (originalHeight - trimRect.height) * 0.5f - trimRect.y;
                    break;

                    //
                case Anchor.BotLeft:
                    offsetX = -halfWidth - trimRect.x;
                    offsetY = halfHeight + originalHeight - trimRect.yMax;
                    break;

                case Anchor.BotCenter: 
                    offsetX = (originalWidth - trimRect.width) * 0.5f - trimRect.x;
                    offsetY = halfHeight + originalHeight - trimRect.yMax;
                    break;

                case Anchor.BotRight:
                    offsetX = halfWidth + originalWidth - trimRect.xMax;
                    offsetY = halfHeight + originalHeight - trimRect.yMax;
                    break;

                default:
                    offsetX = (originalWidth - trimRect.width) * 0.5f - trimRect.x;
                    offsetY = (originalHeight - trimRect.height) * 0.5f - trimRect.y;
                    break;
                }
            }
            else {
                switch ( anchor_ ) {
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
            }
            offsetX -= offset_.x;
            offsetY += offset_.y;

            //
            float xMinClip = scale_.x * width  * ( -0.5f + clipLeft   );
            float xMaxClip = scale_.x * width  * (  0.5f - clipRight  );
            float yMinClip = scale_.y * height * ( -0.5f + clipTop    );
            float yMaxClip = scale_.y * height * (  0.5f - clipBottom );

            // build vertices & normals
            for ( int r = 0; r < 2; ++r ) {
                for ( int c = 0; c < 2; ++c ) {
                    int i = r * 2 + c;

                    // calculate the base pos
                    float x = -halfWidth  + c * width_  * scale_.x;
                    float y =  halfHeight - r * height_ * scale_.y;

                    // do clip
                    if ( clipInfo_.clipped ) {
                        if ( x <= xMinClip ) {
                            x = xMinClip;
                        }
                        else if ( x >= xMaxClip ) {
                            x = xMaxClip;
                        }

                        if ( y <= yMinClip ) {
                            y = yMinClip;
                        }
                        else if ( y >= yMaxClip ) {
                            y = yMaxClip;
                        }
                    }

                    // calculate the pos affect by anchor
                    x -= offsetX;
                    y += offsetY;

                    // calculate the shear
                    x += y * shear_.x;
                    y += x * shear_.y;

                    // DISABLE: we use min,max clip above { 
                    // // do clip
                    // if ( clipInfo_.clipped ) {
                    //     switch (i) {
                    //     case 0: x += scale_.x * width_ * clipLeft;  y -= scale_.y * height_ * clipBottom; break; // bl
                    //     case 1: x -= scale_.x * width_ * clipRight; y -= scale_.y * height_ * clipBottom; break; // br
                    //     case 2: x += scale_.x * width_ * clipLeft;  y += scale_.y * height_ * clipTop; break; // tl
                    //     case 3: x -= scale_.x * width_ * clipRight; y += scale_.y * height_ * clipTop; break; // tr
                    //     }
                    // }
                    // } DISABLE end 

                    // build vertices, normals and uvs
                    switch ( plane ) {
                    case Plane.XY:
                        vertices[i] = new Vector3( x, y, 0.0f );
                        normals[i] = new Vector3( 0.0f, 0.0f, -1.0f ); // TEMP
                        break;
                    case Plane.XZ:
                        vertices[i] = new Vector3( x, 0.0f, y );
                        normals[i] = new Vector3( 0.0f, 1.0f, 0.0f ); // TEMP
                        break;
                    case Plane.ZY:
                        vertices[i] = new Vector3( 0.0f, y, x );
                        normals[i] = new Vector3( 1.0f, 0.0f, 0.0f ); // TEMP
                        break;
                    }
                }
            }
            _mesh.vertices = vertices;
            _mesh.normals = normals; // TEMP
            _mesh.bounds = GetBounds ( offsetX, offsetY, halfWidth * 2.0f, halfHeight * 2.0f );

            // update collider if we have
            UpdateBoxCollider (_mesh);
            UpdateBoundRect ( offsetX, offsetY, halfWidth * 2.0f, halfHeight * 2.0f );

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
        if ( meshFilter != null && 
             meshFilter.sharedMesh != null ) 
        {
            UpdateMesh (meshFilter.sharedMesh);
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
             ( renderer.sharedMaterial != null && renderer.sharedMaterial.mainTexture != null ) ) 
        {
            meshFilter.sharedMesh = new Mesh();
            ForceUpdateMesh( meshFilter.sharedMesh );
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

        if ( meshFilter == null )
            meshFilter = GetComponent<MeshFilter>();
        if ( meshFilter != null )
            meshFilter.sharedMesh = null;
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
        bool checkSize = false;

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
            checkSize = true;
        }

        //
        if ( index_ != _index ) {
            index_ = _index;
            updateFlags |= UpdateFlags.UV;
            checkSize = true;
        }

        //
        if ( checkSize && !customSize_ ) {
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
