// ======================================================================================
// File         : Geometry2D.cs
// Author       : Wu Jie 
// Last Change  : 06/10/2011 | 17:13:16 PM | Friday,June
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
///
/// rect intersects helper class 
///
///////////////////////////////////////////////////////////////////////////////

public static class exIntersection2D {

    // ------------------------------------------------------------------ 
    /// \param _a rect a
    /// \param _b rect b
    /// \result the intersect result
    /// check if two rect intersection
    // ------------------------------------------------------------------ 

    public static bool RectRect ( Rect _a, Rect _b ) {
        if ( (_a.xMin <= _b.xMin && _a.xMax >= _b.xMin) ||
             (_b.xMin <= _a.xMin && _b.xMax >= _a.xMin ) ) 
        {
            if ( (_a.yMin <= _b.yMin && _a.yMax >= _b.yMin) ||
                 (_b.yMin <= _a.yMin && _b.yMax >= _a.yMin ) ) 
            {
                return true;
            }
        }
        return false;
    } 
}

///////////////////////////////////////////////////////////////////////////////
///
/// rect contains helper class 
///
///////////////////////////////////////////////////////////////////////////////

public static class exContains2D {

    // ------------------------------------------------------------------ 
    /// \param _a rect a
    /// \param _b rect b
    /// \result the contains result
    /// check if rect contains, 1 is _a contains _b, -1 is _b contains _a, 0 is no contains 
    // ------------------------------------------------------------------ 

    public static int RectRect ( Rect _a, Rect _b ) {
        if ( _a.xMin <= _b.xMin &&
             _a.xMax >= _b.xMax &&
             _a.yMin <= _b.yMin &&
             _a.yMax >= _b.yMax )
        {
            // a contains b
            return 1;
        }
        if ( _b.xMin <= _a.xMin &&
             _b.xMax >= _a.xMax &&
             _b.yMin <= _a.yMin &&
             _b.yMax >= _a.yMax )
        {
            // b contains a
            return -1;
        }
        return 0;
    }
}
