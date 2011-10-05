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

    // DISABLE { 
    // // ------------------------------------------------------------------ 
    // /// \param _sp the base sprite
    // /// \param _camera the camera
    // /// \param _screenWidth the screen width
    // /// \param _screenHeight the screen height
    // /// Scale the exSpriteBase.scale to make it have renderer in perfect pixel in _camera
    // // ------------------------------------------------------------------ 

    // public static void MakePixelPerfect ( this exSpriteBase _sp, 
    //                                       Camera _camera, 
    //                                       float _screenWidth, 
    //                                       float _screenHeight ) {
    //     float s = 1.0f;
    //     if ( _camera.orthographic ) {
    //         s = 2.0f * _camera.orthographicSize / _screenHeight;
    //     }
    //     else {
    //         float ratio = 2.0f * Mathf.Tan(Mathf.Deg2Rad * _camera.fov * 0.5f) / _screenHeight;
    //         float depth = 0.0f;
    //         switch ( _sp.plane ) {
    //         case exPlane.Plane.XY: depth = _sp.transform.position.z - _camera.transform.position.z; break;
    //         case exPlane.Plane.XZ: depth = _sp.transform.position.y - _camera.transform.position.y; break;
    //         case exPlane.Plane.ZY: depth = _sp.transform.position.x - _camera.transform.position.x; break;
    //         }
    //         s = ratio * depth;
    //     }
    //     _sp.scale = new Vector2( Mathf.Sign(_sp.scale.x) * s, Mathf.Sign(_sp.scale.y) * s );
    // }
    // } DISABLE end 

    // ------------------------------------------------------------------ 
    /// \param _plane the in plane
    /// \param _camera the in camera
    /// \param _anchor the anchor of the plane
    /// \param _screen_x the screen x position
    /// \param _screen_y the screen y position
    /// \return the world position
    /// Convert the screen position to world position in _camera depends on exPlane.plane
    // ------------------------------------------------------------------ 

    public static Vector3 ScreenToWorldPoint ( this exPlane _plane, 
                                               Camera _camera,
                                               exPlane.Anchor _anchor,
                                               float _screen_x,
                                               float _screen_y ) 
    {
        Vector3 result = Vector3.zero;
        float offsetX = 0.0f;
        float offsetY = 0.0f;

        // ======================================================== 
        // anchor 
        // ======================================================== 

        switch ( _anchor ) {
            //
        case exPlane.Anchor.TopLeft:
            offsetX = _screen_x;
            offsetY = Screen.height - _screen_y;
            break;

        case exPlane.Anchor.TopCenter:
            offsetX = Screen.width * 0.5f + _screen_x;
            offsetY = Screen.height - _screen_y;
            break;

        case exPlane.Anchor.TopRight:    
            offsetX = Screen.width - _screen_x;
            offsetY = Screen.height - _screen_y;
            break;

            //
        case exPlane.Anchor.MidLeft:
            offsetX = _screen_x;
            offsetY = Screen.height * 0.5f + _screen_y;
            break;

        case exPlane.Anchor.MidCenter:
            offsetX = Screen.width * 0.5f + _screen_x;
            offsetY = Screen.height * 0.5f + _screen_y;
            break;

        case exPlane.Anchor.MidRight:
            offsetX = Screen.width - _screen_x;
            offsetY = Screen.height * 0.5f + _screen_y;
            break;

            //
        case exPlane.Anchor.BotLeft:
            offsetX = _screen_x;
            offsetY = _screen_y;
            break;

        case exPlane.Anchor.BotCenter: 
            offsetX = Screen.width * 0.5f + _screen_x;
            offsetY = _screen_y;
            break;

        case exPlane.Anchor.BotRight:
            offsetX = Screen.width - _screen_x;
            offsetY = _screen_y;
            break;
        }

        // ======================================================== 
        // plane
        // ======================================================== 

        switch ( _plane.plane ) {
        case exPlane.Plane.XY:
            result = _camera.ScreenToWorldPoint( new Vector3(offsetX, offsetY, _camera.nearClipPlane) );
            result.z = _plane.transform.position.z;
            break;

        case exPlane.Plane.XZ:
            result = _camera.ScreenToWorldPoint( new Vector3(offsetX, offsetY, _camera.nearClipPlane) );
            result.y = _plane.transform.position.y;
            break;

        case exPlane.Plane.ZY:
            result = _camera.ScreenToWorldPoint( new Vector3(offsetX, offsetY, _camera.nearClipPlane) );
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
            result = _camera.ViewportToWorldPoint( new Vector3(_viewport_x, _viewport_y, _camera.nearClipPlane) );
            result.z = _plane.transform.position.z;
            break;

        case exPlane.Plane.XZ:
            result = _camera.ViewportToWorldPoint( new Vector3(_viewport_x, _viewport_y, _camera.nearClipPlane) );
            result.y = _plane.transform.position.y;
            break;

        case exPlane.Plane.ZY:
            result = _camera.ViewportToWorldPoint( new Vector3(_viewport_x, _viewport_y, _camera.nearClipPlane) );
            result.x = _plane.transform.position.x;
            break;
        }

        return result; 
    }

    // ------------------------------------------------------------------ 
    /// \param _plane the in plane
    /// \param _dx the delta x
    /// \param _dy the delta y
    /// translate the 2D pos by plane
    // ------------------------------------------------------------------ 

    public static void Translate ( this exPlane _plane, 
                                   float _dx, 
                                   float _dy )
    {
        switch ( _plane.plane ) {
        case exPlane.Plane.XY:
            _plane.transform.Translate ( _dx, _dy, 0.0f );
            break;

        case exPlane.Plane.XZ:
            _plane.transform.Translate ( _dx, 0.0f, _dy );
            break;

        case exPlane.Plane.ZY:
            _plane.transform.Translate ( 0.0f, _dy, _dx );
            break;
        }
    }

    // ------------------------------------------------------------------ 
    /// \param _plane the in plane
    /// \param _length the length of the collider
    /// sync the collider size
    // ------------------------------------------------------------------ 

    public static void UpdateColliderSize ( this exPlane _plane, float _length ) {
        Mesh mesh = _plane.meshFilter.sharedMesh;
        Collider collider = _plane.collider;

        // update box collider
        if ( collider is BoxCollider ) {
            BoxCollider boxCollider = collider as BoxCollider;
            boxCollider.center = mesh.bounds.center;
            boxCollider.size = mesh.bounds.size;

            switch ( _plane.plane ) {
            case exSprite.Plane.XY:
                boxCollider.size = new Vector3( mesh.bounds.size.x, mesh.bounds.size.y, _length );
                break;

            case exSprite.Plane.XZ:
                boxCollider.size = new Vector3( mesh.bounds.size.x, _length, mesh.bounds.size.z );
                break;

            case exSprite.Plane.ZY:
                boxCollider.size = new Vector3( _length, mesh.bounds.size.y, mesh.bounds.size.z );
                break;
            }

            return;
        }

        // update mesh collider
        if ( collider is MeshCollider ) {
            MeshCollider meshCollider = collider as MeshCollider;
            // NOTE: only in this way, mesh collider changes
            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = mesh;
            return;
        }
    }
}
