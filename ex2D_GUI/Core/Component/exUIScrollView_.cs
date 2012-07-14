// ======================================================================================
// File         : exUIScrollView_.cs
// Author       : Wu Jie 
// Last Change  : 06/23/2012 | 17:36:09 PM | Saturday,June
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
// Interactions Data
///////////////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

[ExecuteInEditMode]
[AddComponentMenu("ex2D GUI/Button")]
public class exUIScrollView_ : exUIElement {

    // delegates
	public delegate void EventHandler ();
	public delegate void StateUpdate ();

    // events
	public event EventHandler OnHoverIn;
	public event EventHandler OnHoverOut;

    public enum ScrollDirection {
        Vertical,
        Horizontal,
        Both
    }

    ///////////////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////////////

    public float contentWidth = 100.0f;
    public float contentHeight = 100.0f;
    public bool bounce = true;
    public float bounceDuration = 0.5f;
    public ScrollDirection scrollDirection = ScrollDirection.Both;
    public float damping = 0.95f;
    public float elasticity = 0.2f;

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public exClipping clipRect;
    public exSpriteBorder horizontalBar;
    public exSpriteBorder horizontalSlider;
    public exSpriteBorder verticalBar;
    public exSpriteBorder verticalSlider;
    public GameObject contentAnchor;

    protected Vector2 contentOffset = Vector2.zero;
    protected Vector2 startPos;
    protected Vector2 destPos;
    protected float duration;
    protected StateUpdate stateUpdate;
    protected float pressTime = 0.0f;
    protected Vector2 pressPoint = Vector2.zero;
    protected Vector2 velocity = Vector2.zero;
    protected bool isDragging = false;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Start () {
        if ( verticalSlider != null ) {
            if ( height < contentHeight ) {
                verticalSlider.enabled = true;
                verticalSlider.height = height/contentHeight * height;
            }
            else {
                verticalSlider.enabled = false;
            }
        }

        if ( horizontalSlider != null ) {
            if ( width < contentWidth ) {
                horizontalSlider.enabled = true;
                horizontalSlider.width = width/contentWidth * width;
            }
            else {
                horizontalSlider.enabled = false;
            }
        }
    }

    // DELME { 
    // // ------------------------------------------------------------------ 
    // // Desc: 
    // // ------------------------------------------------------------------ 

    // public override void Sync () {
    //     base.Sync();

    //     float startX = boundingRect.xMin;
    //     float startY = boundingRect.yMax;
    //     float endX = boundingRect.xMax;
    //     float endY = boundingRect.yMin;

    //     clipRect.anchor = anchor;
    //     clipRect.width = width;
    //     clipRect.height = height;
    //     clipRect.transform.localPosition = new Vector3 ( 0.0f, 0.0f, clipRect.transform.localPosition.z );

    //     if ( contentAnchor )
    //         contentAnchor.transform.localPosition = new Vector3 ( startX, startY, contentAnchor.transform.localPosition.z );
    //     if ( horizontalBar )
    //         horizontalBar.transform.localPosition = new Vector3 ( startX, endY, horizontalBar.transform.localPosition.z );
    //     if ( horizontalSlider )
    //         horizontalSlider.transform.localPosition = new Vector3 ( startX, endY, horizontalSlider.transform.localPosition.z );
    //     if ( verticalBar )
    //         verticalBar.transform.localPosition = new Vector3 ( endX, startY, verticalBar.transform.localPosition.z );
    //     if ( verticalSlider )
    //         verticalSlider.transform.localPosition = new Vector3 ( endX, startY, verticalSlider.transform.localPosition.z );

    //     // DELME { 
    //     // switch ( plane ) {
    //     // case exPlane.Plane.XY:
    //     //     if ( contentAnchor )
    //     //         contentAnchor.transform.localPosition = new Vector3 ( startX, startY, 0.0f );
    //     //     if ( horizontalBar )
    //     //         horizontalBar.transform.localPosition = new Vector3 ( startX, endY, 0.0f );
    //     //     if ( horizontalSlider )
    //     //         horizontalSlider.transform.localPosition = new Vector3 ( startX, endY, 0.0f );
    //     //     if ( verticalBar )
    //     //         verticalBar.transform.localPosition = new Vector3 ( endX, startY, 0.0f );
    //     //     if ( verticalSlider )
    //     //         verticalSlider.transform.localPosition = new Vector3 ( endX, startY, 0.0f );
    //     //     break;

    //     // case exPlane.Plane.XZ:
    //     //     if ( contentAnchor )
    //     //         contentAnchor.transform.localPosition = new Vector3 ( startX, 0.0f, startY );
    //     //     if ( horizontalBar )
    //     //         horizontalBar.transform.localPosition = new Vector3 ( startX, 0.0f, endY );
    //     //     if ( horizontalSlider )
    //     //         horizontalSlider.transform.localPosition = new Vector3 ( startX, 0.0f, endY );
    //     //     if ( verticalBar )
    //     //         verticalBar.transform.localPosition = new Vector3 ( endX, 0.0f, startY );
    //     //     if ( verticalSlider )
    //     //         verticalSlider.transform.localPosition = new Vector3 ( endX, 0.0f, startY );
    //     //     break;

    //     // case exPlane.Plane.ZY:
    //     //     if ( contentAnchor )
    //     //         contentAnchor.transform.localPosition = new Vector3 ( 0.0f, startY, startX );
    //     //     if ( horizontalBar )
    //     //         horizontalBar.transform.localPosition = new Vector3 ( 0.0f, endY, startX );
    //     //     if ( horizontalSlider )
    //     //         horizontalSlider.transform.localPosition = new Vector3 ( 0.0f, endY, startX );
    //     //     if ( verticalBar )
    //     //         verticalBar.transform.localPosition = new Vector3 ( 0.0f, endX, startY );
    //     //     if ( verticalSlider )
    //     //         verticalSlider.transform.localPosition = new Vector3 ( 0.0f, endX, startY );
    //     //     break;
    //     // }
    //     // } DELME end 

    //     //
    //     float hbarHeight = (horizontalBar && horizontalBar.guiBorder) ? horizontalBar.guiBorder.border.vertical : 0.0f;
    //     float vbarWidth = (verticalBar && verticalBar.guiBorder) ? verticalBar.guiBorder.border.horizontal : 0.0f;
    //     if ( horizontalBar ) {
    //         horizontalBar.width = width - vbarWidth; 
    //         horizontalBar.height = hbarHeight;
    //     }
    //     if ( verticalBar ) {
    //         verticalBar.height = height - hbarHeight;
    //         verticalBar.width = vbarWidth;
    //     }
    // }
    // } DELME end 

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public override bool OnEvent ( exUIEvent _e ) {
        // switch ( _e.type ) {
        // case exUIEvent.Type.PointerEnter: 
        //     if ( OnHoverIn != null )
        //         OnHoverIn ();
        //     return true;

        // case exUIEvent.Type.PointerExit: 
        //     if ( OnHoverOut != null )
        //         OnHoverOut ();
        //     return true;

        // case exUIEvent.Type.PointerPress: 
        //     if ( _e.buttons == exUIEvent.PointerButtonFlags.Left ||
        //          _e.buttons == exUIEvent.PointerButtonFlags.Touch ) 
        //     {
        //         exUIMng.focus = this;
        //         velocity = Vector2.zero;
        //         pressTime = Time.time;
        //         pressPoint = _e.position;
        //         isDragging = false;
        //         stateUpdate = null;
        //     }
        //     return true;

        // case exUIEvent.Type.PointerRelease: 
        //     if ( _e.buttons == exUIEvent.PointerButtonFlags.Left ||
        //          _e.buttons == exUIEvent.PointerButtonFlags.Touch ) 
        //     {
        //         exUIMng.focus = null;
        //         horizontalSlider.width = width/contentWidth * width;
        //         verticalSlider.height = height/contentHeight * height;
        //         if ( isDragging ) {
        //             isDragging = false;
        //             if ( Time.time - pressTime < 0.01f ) {
        //                 velocity = Vector2.zero;
        //             }
        //             else {
        //                 velocity = (pressPoint - _e.position)/(Time.time - pressTime);
        //             }

        //             if ( scrollDirection == ScrollDirection.Vertical )
        //                 velocity.x = 0.0f;
        //             else if ( scrollDirection == ScrollDirection.Horizontal )
        //                 velocity.y = 0.0f;
        //         }
        //         else {
        //             velocity = Vector2.zero;
        //         }
        //         stateUpdate = DeaccelerateScrolling;
        //     }
        //     return true;

        // case exUIEvent.Type.PointerMove: 
        //     if ( _e.buttons == exUIEvent.PointerButtonFlags.Left ||
        //          _e.buttons == exUIEvent.PointerButtonFlags.Touch ) 
        //     {
        //         if ( _e.delta.magnitude > 1.0f ) {
        //             if ( isDragging == false ) {
        //                 pressTime = Time.time;
        //                 pressPoint = _e.position;
        //                 isDragging = true;
        //             }
        //         }
        //         else {
        //             pressTime = Time.time;
        //             pressPoint = _e.position;
        //             isDragging = false;
        //         }

        //         float maxOffsetX = Mathf.Max(contentWidth - width, 0.0f);
        //         float maxOffsetY = Mathf.Max(contentHeight - height, 0.0f);
        //         float newX = -_e.delta.x;
        //         float newY = -_e.delta.y;
        //         Vector2 scrollDistance = Vector2.zero;

        //         //
        //         if ( scrollDirection == ScrollDirection.Vertical )
        //             newX = 0.0f;
        //         else if ( scrollDirection == ScrollDirection.Horizontal )
        //             newY = 0.0f;

        //         //
        //         if ( bounce ) {
        //             //
        //             float bounceX = 0.0f;
        //             if ( contentOffset.x > 0.0f ) {
        //                 bounceX = contentOffset.x;
        //                 // newX *= elasticity / bounceX;
        //                 newX *= elasticity;
        //             }
        //             else if (  contentOffset.x < maxOffsetX ) {
        //                 bounceX = maxOffsetX - contentOffset.x;
        //                 // newX *= elasticity / bounceX;
        //                 newX *= elasticity;
        //             }
        //             horizontalSlider.width = (width - bounceX)/contentWidth * (width - bounceX);

        //             //
        //             float bounceY = 0.0f;
        //             if ( contentOffset.y > 0.0f ) {
        //                 bounceY = contentOffset.y;
        //                 // newY *= elasticity / bounceY; 
        //                 newY *= elasticity;
        //             }
        //             else if (  contentOffset.y < -maxOffsetY ) {
        //                 bounceY = -maxOffsetY - contentOffset.y;
        //                 // newY *= elasticity / bounceY; 
        //                 newY *= elasticity;
        //             }
        //             verticalSlider.height = (height - bounceY)/contentHeight * (height - bounceY);

        //             //
        //             scrollDistance = new Vector2( newX, newY );
        //             contentOffset += scrollDistance;
        //         }
        //         else {
        //             scrollDistance = new Vector2( newX, newY );
        //             contentOffset += scrollDistance;
        //             contentOffset = new Vector2 ( Mathf.Clamp ( contentOffset.x, 0.0f, maxOffsetX ),
        //                                           Mathf.Clamp ( contentOffset.y, -maxOffsetY, 0.0f ) );
        //         }
        //         SetOffset ( contentOffset );
        //     }
        //     return true;
        // }

        return false;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void SetOffset ( Vector2 _offset ) {
        float maxOffsetX = Mathf.Max(contentWidth - width, 0.0f);
        float maxOffsetY = Mathf.Max(contentHeight - height, 0.0f);

        float startX = boundingRect.xMin;
        float startY = boundingRect.yMax;

        float vsliderX = verticalSlider.transform.localPosition.x;
        float vsliderY = startY + _offset.y * height/contentHeight;
        if ( _offset.y < -maxOffsetY ) {
            vsliderY = -height + verticalSlider.height;
        }
        else if ( _offset.y > 0 ) {
            vsliderY = 0.0f;
        }

        float hsliderX = startX + _offset.x * width/contentWidth;
        if ( _offset.x < -maxOffsetX ) {
            hsliderX = width - horizontalSlider.width;
        }
        else if ( _offset.y > 0 ) {
            hsliderX = 0.0f;
        }
        float hsliderY = horizontalSlider.transform.localPosition.y;

        contentAnchor.transform.localPosition = new Vector3( startX-_offset.x, startY-_offset.y, contentAnchor.transform.localPosition.z );
        verticalSlider.transform.localPosition = new Vector3( vsliderX, vsliderY, verticalSlider.transform.localPosition.z );
        horizontalSlider.transform.localPosition = new Vector3( hsliderX, hsliderY, horizontalSlider.transform.localPosition.z );

        // DELME { 
        // switch ( plane ) {
        // case exPlane.Plane.XY:
        //     contentAnchor.transform.localPosition = new Vector3( startX-_offset.x, startY-_offset.y, 0.0f );
        //     verticalSlider.transform.localPosition = new Vector3( vsliderX, vsliderY, 0.0f );
        //     horizontalSlider.transform.localPosition = new Vector3( hsliderX, hsliderY, 0.0f );
        //     break;

        // case exPlane.Plane.XZ:
        //     contentAnchor.transform.localPosition = new Vector3( startX-_offset.x, 0.0f, startY-_offset.y );
        //     verticalSlider.transform.localPosition = new Vector3( vsliderX, 0.0f, vsliderY );
        //     horizontalSlider.transform.localPosition = new Vector3( hsliderX, 0.0f, hsliderY );
        //     break;

        // case exPlane.Plane.ZY:
        //     contentAnchor.transform.localPosition = new Vector3( 0.0f, startY-_offset.y, startX-_offset.x );
        //     verticalSlider.transform.localPosition = new Vector3( 0.0f, vsliderY, vsliderX ); 
        //     horizontalSlider.transform.localPosition = new Vector3( 0.0f, hsliderY, hsliderX );
        //     break;
        // }
        // } DELME end 
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void DeaccelerateScrolling () {
        float maxOffsetX = Mathf.Max(contentWidth - boundingRect.width, 0.0f);
        float maxOffsetY = Mathf.Max(contentHeight - boundingRect.height, 0.0f);

        //
        bool needRelocate = false;
        if ( bounce ) {
            //
            float newX = velocity.x;
            float newY = velocity.y;
            float bounceX = 0.0f;
            if ( contentOffset.x > 0.0f ) {
                bounceX = contentOffset.x;
                newX *= elasticity;
                needRelocate = true;
            }
            else if (  contentOffset.x < maxOffsetX ) {
                bounceX = maxOffsetX - contentOffset.x;
                newX *= elasticity;
                needRelocate = true;
            }
            else {
                newX *= damping;
            }
            horizontalSlider.width = (width - bounceX)/contentWidth * (width - bounceX);

            //
            float bounceY = 0.0f;
            if ( contentOffset.y > 0.0f ) {
                bounceY = contentOffset.y;
                newY *= elasticity;
                needRelocate = true;
            }
            else if (  contentOffset.y < -maxOffsetY ) {
                bounceY = -maxOffsetY - contentOffset.y;
                newY *= elasticity;
                needRelocate = true;
            }
            else {
                newY *= damping;
            }
            verticalSlider.height = (height - bounceY)/contentHeight * (height - bounceY);

            //
            velocity = new Vector2( newX, newY );
            contentOffset += velocity * Time.deltaTime;
        }
        else {
            velocity *= damping;
            contentOffset += velocity * Time.deltaTime;
            contentOffset = new Vector2 ( Mathf.Clamp ( contentOffset.x, 0.0f, maxOffsetX ),
                                   Mathf.Clamp ( contentOffset.y, -maxOffsetY, 0.0f ) );
        }
        SetOffset ( contentOffset );

        //
        if ( (Mathf.Abs(velocity.x) <= 0.1f && 
              Mathf.Abs(velocity.y) <= 0.1f) ) {

            if ( needRelocate ) {
                duration = 0.0f;
                startPos = contentOffset;
                destPos = new Vector2 ( Mathf.Clamp ( contentOffset.x, 0.0f, maxOffsetX ),
                                        Mathf.Clamp ( contentOffset.y, -maxOffsetY, 0.0f ) );
                stateUpdate = RelocateContent;
            }
            else {
                stateUpdate = null;
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    public static float ExpoOut ( float _t ) {
        return (_t==1.0f) ? 1.0f : 1.001f * (-Mathf.Pow(2.0f, -10 * _t) + 1);
    }
    // ------------------------------------------------------------------ 

    void RelocateContent () {
        duration += Time.deltaTime;
        float ratio = Mathf.Clamp( duration/bounceDuration, 0.0f, 1.0f );
        float maxOffsetX = Mathf.Max(contentWidth - boundingRect.width, 0.0f);
        float maxOffsetY = Mathf.Max(contentHeight - boundingRect.height, 0.0f);

        contentOffset = new Vector2 ( Mathf.Lerp ( startPos.x, destPos.x, ExpoOut(ratio) ),
                                      Mathf.Lerp ( startPos.y, destPos.y, ExpoOut(ratio) ) );

        //
        float bounceX = 0.0f;
        if ( contentOffset.x > 0.0f ) {
            bounceX = contentOffset.x;
        }
        else if (  contentOffset.x < maxOffsetX ) {
            bounceX = maxOffsetX - contentOffset.x;
        }
        horizontalSlider.width = (width - bounceX)/contentWidth * (width - bounceX);

        //
        float bounceY = 0.0f;
        if ( contentOffset.y > 0.0f ) {
            bounceY = contentOffset.y;
        }
        else if (  contentOffset.y < -maxOffsetY ) {
            bounceY = -maxOffsetY - contentOffset.y;
        }
        verticalSlider.height = (height - bounceY)/contentHeight * (height - bounceY);

        //
        SetOffset ( contentOffset );

        if ( duration >= bounceDuration ) {
            stateUpdate = null;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void LateUpdate () {
        if ( stateUpdate != null ) {
            stateUpdate ();
        }
    }
}
