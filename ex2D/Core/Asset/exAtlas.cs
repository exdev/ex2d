// ======================================================================================
// File         : exAtlas.cs
// Author       : Wu Jie 
// Last Change  : 07/03/2011 | 23:09:42 PM | Sunday,July
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
///
/// The atlas asset used in exSprite
///
///////////////////////////////////////////////////////////////////////////////

public class exAtlas : ScriptableObject {

    ///////////////////////////////////////////////////////////////////////////////
    ///
    /// The element in the atlas
    ///
    ///////////////////////////////////////////////////////////////////////////////

    [System.Serializable]
    public class Element {
        public string name = "";       ///< the name of the element
        public int originalWidth = 0;  ///< the original width of the texture 
        public int originalHeight = 0; ///< the original height of the texture
        public Rect trimRect = new Rect( 0, 0, 1, 1 );  ///< the trimmed rect
        public Rect coords = new Rect( 0, 0, 1, 1 ); ///< the uv coordination = Rect (xStart, yStart, xEnd, yEnd)
        public bool rotated = false; ///< if rotate the element  
        // public bool trimmed = false; // TODO:
    }

    public Element[] elements; ///< the list of element in ths atlas
    public Material material; ///< the default material of the atlas
    public Texture2D texture; ///< the atlas texture

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    /// \param _name the name of the expect element
    /// \return the expect index
    /// Get the element index in the atlas by _name. If no element marched 
    /// the name, it will return -1. 
    // ------------------------------------------------------------------ 

    public int GetIndexByName ( string _name ) {
        for ( int i = 0; i < elements.Length; ++i ) {
            if ( elements[i].name == _name )
                return i;
        }
        return -1;
    }
}
