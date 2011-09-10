// ======================================================================================
// File         : exTimeHelper.cs
// Author       : Wu Jie 
// Last Change  : 06/16/2011 | 10:09:06 AM | Thursday,June
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
///
/// time helper
///
///////////////////////////////////////////////////////////////////////////////

public static class exTimeHelper {

    // ------------------------------------------------------------------ 
    /// \param _seconds input seoncds
    /// \return the minutes
    /// get minutes from input seconds
    // ------------------------------------------------------------------ 

    public static int GetMinutes ( float _seconds ) {
        return Mathf.FloorToInt(_seconds / 60.0f);
    }

    // ------------------------------------------------------------------ 
    /// \param _seconds input seoncds
    /// \return the minutes in string
    /// get minutes in string from input seconds
    // ------------------------------------------------------------------ 

    public static string ToString_Minutes ( float _seconds ) {
        int min = Mathf.FloorToInt(_seconds / 60.0f);
        int sec = Mathf.FloorToInt(_seconds % 60.0f);
        return min + ":" + sec.ToString("d2");
    }

    // ------------------------------------------------------------------ 
    /// \param _seconds input seoncds
    /// \return the seconds in string
    /// get seconds in string from input seconds
    // ------------------------------------------------------------------ 

    public static string ToString_Seconds ( float _seconds ) {
        int sec1 = Mathf.FloorToInt(_seconds);
        int sec2 = Mathf.FloorToInt((_seconds - sec1) * 60.0f % 60.0f);
        return sec1 + ":" + sec2.ToString("d2");
    }

    // ------------------------------------------------------------------ 
    /// \param _seconds input seoncds
    /// \param _sampleRate input sample rate
    /// \return the frame in string
    /// get seconds in string from input seconds
    // ------------------------------------------------------------------ 

    public static string ToString_Frames ( float _seconds, float _sampleRate ) {
        int sec1 = Mathf.FloorToInt(_seconds);
        int sec2 = Mathf.FloorToInt((_seconds - sec1) * _sampleRate % _sampleRate);

        int d = 1;
        int dd = 10;
        while ( _sampleRate / dd >= 1.0f ) {
            d += 1;
            dd *= 10;
        }
        return sec1 + ":" + sec2.ToString("d"+d);
    }
}
