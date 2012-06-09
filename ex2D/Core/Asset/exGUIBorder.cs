// ======================================================================================
// File         : exGUIBorder.cs
// Author       : Wu Jie 
// Last Change  : 09/20/2011 | 14:48:27 PM | Tuesday,September
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
/// The GUI Border asset used in exSpriteBorder
///
///////////////////////////////////////////////////////////////////////////////

public class exGUIBorder : ScriptableObject {

    public string textureGUID = ""; ///< the guid of the source texture 
    public RectOffset border = new RectOffset(0, 0, 0, 0); ///< the border for GUI grid

    // editor data
    public bool editorNeedRebuild = false; ///< check if the gui border need rebuild

}

