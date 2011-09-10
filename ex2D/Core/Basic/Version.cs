// ======================================================================================
// File         : Version.cs
// Author       : Wu Jie 
// Last Change  : 09/04/2011 | 14:09:39 PM | Sunday,September
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
// exVersion
// 
// a structure descrip the version in ex2D
// 
///////////////////////////////////////////////////////////////////////////////

[System.Serializable]
public class exVersion {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public int major = -1;
    public int minor = -1;
    public int patch = -1;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    public static bool operator == ( exVersion _a, exVersion _b ) { return _a.Equals(_b); }
    public static bool operator != ( exVersion _a, exVersion _b ) { return !_a.Equals(_b); }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public exVersion ( int _major, int _minor, int _patch ) {
        major = _major; 
        minor = _minor; 
        patch = _patch;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public override int GetHashCode() { return major ^ minor ^ patch; }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public override bool Equals ( object _obj ) {
        if ( !(_obj is exVersion) )
            return false;

        return Equals((exVersion)_obj);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public bool Equals ( exVersion _other ) {
        if ( major != _other.major ||
             minor != _other.minor ||
             patch != _other.patch )
        {
            return false;
        }
        return true;
    }
}
