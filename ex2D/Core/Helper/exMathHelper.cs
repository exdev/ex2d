// ======================================================================================
// File         : exMathHelper.cs
// Author       : Wu Jie 
// Last Change  : 09/08/2011 | 22:19:17 PM | Thursday,September
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
/// time helper
///
///////////////////////////////////////////////////////////////////////////////

public static class exMathHelper {

    // ------------------------------------------------------------------ 
    /// \param _list int list
    /// \return the result
    /// get greatest common divisor
    // ------------------------------------------------------------------ 

    // public static int GetGCD ( int[] _list ) {
    public static int GetGCD ( List<int> _list ) {
        // System.Array.Sort(_list);
        _list.Sort();
        int min = _list[0];
        int result = 1;
        for ( int i = 2; i < min+1; ++i ) {
            bool flag = true;
            foreach ( int j in _list ) {
                if ( j % i != 0 ) {
                    flag = false;
                }
            }
            if ( flag == true ) {
                result = i;
            }
        }
        return result;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static int GetGCD( int _a, int _b ) { 
        //arrange so that _a>_b 
        if( _a < _b ) {
            int temp = _a; 
            _a = _b; 
            _b=temp; 
        } 

        //the base case 
        if( _b == 0 )
            return _a; 

        //_a and _b are even 
        if( _a%2 == 0 && _b%2 == 0 )
            return 2 * GetGCD( _a/2, _b/2 ); 

        // only _a is even 
        if ( _a%2 == 0 )
            return GetGCD( _a/2, _b ); 

        // only _b is even 
        if ( _b%2==0 )
            return GetGCD( _a, _b/2 ); 

        // _a and _b are odd 
        return GetGCD( (_a+_b)/2, (_a-_b)/2 );
    }
}
