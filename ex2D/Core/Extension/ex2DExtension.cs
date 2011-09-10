// ======================================================================================
// File         : ex2DExtension.cs
// Author       : Wu Jie 
// Last Change  : 08/29/2011 | 11:55:07 AM | Monday,August
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
/// 
/// ex2D Extensions
/// 
///////////////////////////////////////////////////////////////////////////////

public static class ex2DExtension {

    // ------------------------------------------------------------------ 
    /// \param _sp the base sprite
    /// \param _camera the camera
    /// \param _screenWidth the screen width
    /// \param _screenHeight the screen height
    /// Scale the exSpriteBase.scale to make it have renderer in perfect pixel in _camera
    // ------------------------------------------------------------------ 

    public static void MakePixelPerfect ( this exSpriteBase _sp, 
                                          Camera _camera, 
                                          float _screenWidth, 
                                          float _screenHeight ) {
        float s = 1.0f;
        if ( _camera.orthographic ) {
            s =  2.0f * _camera.orthographicSize / _screenHeight;
        }
        else {
            float ratio = 2.0f * Mathf.Tan(Mathf.Deg2Rad * _camera.fov * 0.5f) / _screenHeight;
            s = ratio * ( _sp.transform.position.z - _camera.transform.position.z );
        }
		_sp.scale = new Vector2( Mathf.Sign(_sp.scale.x) * s, Mathf.Sign(_sp.scale.y) * s );
    }

    // ------------------------------------------------------------------ 
    /// \param _plane the in plane
    /// \param _camera the in camera
    /// \param _screen_x the screen x position
    /// \param _screen_y the screen y position
    /// \return the world position
    /// Convert the screen position to world position in _camera depends on exPlane.plane
    // ------------------------------------------------------------------ 

    public static Vector3 ScreenToWorldPoint ( this exPlane _plane, 
                                               Camera _camera,
                                               float _screen_x,
                                               float _screen_y ) 
    {
        Vector3 result = Vector3.zero;

        switch ( _plane.plane ) {
        case exPlane.Plane.XY:
            result = _camera.ScreenToWorldPoint( new Vector3(_screen_x, _screen_y, 0.0f) );
            result.z = _plane.transform.position.z;
            break;

        case exPlane.Plane.XZ:
            result = _camera.ScreenToWorldPoint( new Vector3(_screen_x, _screen_y, 0.0f) );
            result.y = _plane.transform.position.y;
            break;

        case exPlane.Plane.ZY:
            result = _camera.ScreenToWorldPoint( new Vector3(_screen_x, _screen_y, 0.0f) );
            result.x = _plane.transform.position.x;
            break;
        }

        return result;
    }

    // ------------------------------------------------------------------ 
    /// \param _plane the in plane
    /// \param _camera the in camera
    /// \param _viewport_x the viewport x position
    /// \param _viewport_y the viewport y position
    /// \return the world position
    /// Convert the viewport position to world position in _camera depends on exPlane.plane
    // ------------------------------------------------------------------ 

    public static Vector3 ViewportToWorldPoint ( this exPlane _plane, 
                                                 Camera _camera,
                                                 float _viewport_x,
                                                 float _viewport_y ) 
    {
        Vector3 result = Vector3.zero;

        switch ( _plane.plane ) {
        case exPlane.Plane.XY:
            result = _camera.ViewportToWorldPoint( new Vector3(_viewport_x, _viewport_y, 0.0f) );
            result.z = _plane.transform.position.z;
            break;

        case exPlane.Plane.XZ:
            result = _camera.ViewportToWorldPoint( new Vector3(_viewport_x, _viewport_y, 0.0f) );
            result.y = _plane.transform.position.y;
            break;

        case exPlane.Plane.ZY:
            result = _camera.ViewportToWorldPoint( new Vector3(_viewport_x, _viewport_y, 0.0f) );
            result.x = _plane.transform.position.x;
            break;
        }

        return result; 
    }
}
