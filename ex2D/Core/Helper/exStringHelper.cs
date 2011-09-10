// ======================================================================================
// File         : exStringHelper.cs
// Author       : Wu Jie 
// Last Change  : 07/26/2011 | 12:00:30 PM | Tuesday,July
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
///
/// exStringHelper
///
///////////////////////////////////////////////////////////////////////////////

public static class exStringHelper {

    // ------------------------------------------------------------------ 
    /// \param _text the input text
    /// \return the number of lines
    /// count lines of the input text
    // ------------------------------------------------------------------ 

    public static long CountLinesInString ( string _text ) {
        long count = 1;
        int start = 0;
        while ((start = _text.IndexOf('\n', start)) != -1) {
            ++count;
            ++start;
        }
        return count;
    }
}

