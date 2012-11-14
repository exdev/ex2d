// ======================================================================================
// File         : exUIScrollView.cs
// Author       : Wu Jie 
// Last Change  : 07/22/2012 | 14:22:52 PM | Sunday,July
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
public class exUIScrollView : exUIElement {

    public enum ScrollDirection {
        Vertical,
        Horizontal,
        Both
    }

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    // public exUIButton btnLeft = null;
    // public exUIButton btnRight = null;
    // public exUIButton btnUp = null;
    // public exUIButton btnDown = null;

    public exClipping clipRect;
    public exSpriteBorder horizontalBar;
    public exSpriteBorder horizontalSlider;
    public exSpriteBorder verticalBar;
    public exSpriteBorder verticalSlider;
    public Transform contentAnchor;

    public bool showSliderOnDragging = false;
    public ScrollDirection scrollDirection = ScrollDirection.Both;

    public float deceleration = 0.98f;
    public float bounce = 0.8f;
    public float bounceBackDuration = 0.5f;

    ///////////////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////////////

    protected static float flickingStop = 30.0f;

    // flicking
    protected bool isPressing = false;
    protected bool isDragging = false;
    protected Vector2 pressPoint = Vector2.zero;
    protected float pressTime = 0.0f;
    protected Vector2 velocity = Vector2.zero;
    protected bool flickingX = false;
    protected bool flickingY = false;

    // scroll to
    protected bool scrollingToX = false;
    protected bool scrollingToY = false;

    protected float srcX = 0.0f;
    protected float destX = 0.0f;
    protected float timerX = 0.0f;
    protected float srcY = 0.0f;
    protected float destY = 0.0f;
    protected float timerY = 0.0f;

    protected Vector2 contentOffset = Vector2.zero;
    protected float contentWidth = 0.0f; 
    protected float contentHeight = 0.0f; 
    protected float minX = 0.0f;
    protected float maxX = 0.0f;
    protected float minY = 0.0f;
    protected float maxY = 0.0f;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////


    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected void Start () {
        UpdateLayout();
        if ( showSliderOnDragging ) {
            horizontalSlider.color = new Color(horizontalSlider.color.r, horizontalSlider.color.g, horizontalSlider.color.b, 0.0f);
            verticalSlider.color = new Color(verticalSlider.color.r, verticalSlider.color.g, verticalSlider.color.b, 0.0f);
        }
        else {
            horizontalSlider.color = new Color(horizontalSlider.color.r, horizontalSlider.color.g, horizontalSlider.color.b, 1.0f);
            verticalSlider.color = new Color(verticalSlider.color.r, verticalSlider.color.g, verticalSlider.color.b, 1.0f);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    protected struct MoveToParams {
        public Vector2 dest; 
        public float duration; 

        public MoveToParams ( Vector2 _dest, float _duration ) {
            dest = _dest;
            duration = _duration;
        }
    }
    // ------------------------------------------------------------------ 

    public void MoveTo ( Vector2 _destOffset, float _duration ) {
        StopCoroutine ( "MoveTo_CO" );
        StartCoroutine ( "MoveTo_CO", new MoveToParams ( _destOffset, _duration ) );
    } 

    protected IEnumerator MoveTo_CO ( MoveToParams _params ) {
        Vector2 start = contentOffset;
        Vector2 dest = _params.dest;
        float timer = 0.0f;
        if ( _params.duration != 0.0f  ) {
            while ( timer <= _params.duration ) {
                float ratio = timer/_params.duration;
                contentOffset = Vector2.Lerp ( start, dest, ratio );
                contentOffset.x = Mathf.Clamp ( contentOffset.x, minX, maxX );
                contentOffset.y = Mathf.Clamp ( contentOffset.y, minY, maxY );
                SetOffset(contentOffset);

                yield return 0;
                timer += Time.deltaTime;
            }
        }

        contentOffset = dest;
        contentOffset.x = Mathf.Clamp ( contentOffset.x, minX, maxX );
        contentOffset.y = Mathf.Clamp ( contentOffset.y, minY, maxY );
        SetOffset(contentOffset);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    protected struct FadeToParams {
        public exSpriteBorder slider;
        public float destAlpha; 
        public float duration;

        public FadeToParams ( exSpriteBorder _slider, float _destAlpha, float _duration ) {
            slider = _slider;
            destAlpha = _destAlpha; 
            duration = _duration;
        }

    }
    // ------------------------------------------------------------------ 
    protected IEnumerator FadeToHorizontal ( FadeToParams _params ) {
        float timer = 0.0f;
        float srcAlpha = _params.slider.color.a;

        while ( timer <= _params.duration ) {
            float ratio = timer / _params.duration;
            float alpha = Mathf.Lerp ( srcAlpha, _params.destAlpha, ratio );
            _params.slider.color = new Color( _params.slider.color.r, _params.slider.color.g, _params.slider.color.b, alpha );

            yield return 0;
            timer += Time.deltaTime;
        }
        _params.slider.color = new Color( _params.slider.color.r, _params.slider.color.g, _params.slider.color.b, _params.destAlpha );
    }

    protected IEnumerator FadeToVertical ( FadeToParams _params ) {
        float timer = 0.0f;
        float srcAlpha = _params.slider.color.a;

        while ( timer <= _params.duration ) {
            float ratio = timer / _params.duration;
            float alpha = Mathf.Lerp ( srcAlpha, _params.destAlpha, ratio );
            _params.slider.color = new Color( _params.slider.color.r, _params.slider.color.g, _params.slider.color.b, alpha );

            yield return 0;
            timer += Time.deltaTime;
        }
        _params.slider.color = new Color( _params.slider.color.r, _params.slider.color.g, _params.slider.color.b, _params.destAlpha );
    }

    protected IEnumerator FadeTo ( FadeToParams _params ) {
        float timer = 0.0f;
        float srcAlpha = _params.slider.color.a;

        while ( timer <= _params.duration ) {
            float ratio = timer / _params.duration;
            float alpha = Mathf.Lerp ( srcAlpha, _params.destAlpha, ratio );
            _params.slider.color = new Color( _params.slider.color.r, _params.slider.color.g, _params.slider.color.b, alpha );

            yield return 0;
            timer += Time.deltaTime;
        }
        _params.slider.color = new Color( _params.slider.color.r, _params.slider.color.g, _params.slider.color.b, _params.destAlpha );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    public static float easing ( float _t ) {
        _t-=1.0f;
        return _t*_t*_t + 1;
    }
    // ------------------------------------------------------------------ 

    protected void Update () {
        if ( flickingX || flickingY ) {
            Vector2 delta = velocity * Time.deltaTime;
            contentOffset += delta;
            SetOffset ( contentOffset );

            if ( flickingX ) {

                velocity.x *= deceleration;

                if ( scrollDirection != ScrollDirection.Vertical ) {
                    if ( contentOffset.x < minX || contentOffset.x > maxX ) {
                        velocity.x *= bounce;
                    }
                }


                if ( Mathf.Abs(velocity.x) <= flickingStop ) {
                    flickingX = false;
                    if ( contentOffset.x < minX || contentOffset.x > maxX ) {
                        scrollingToX = true;

                        srcX = contentOffset.x;
                        if ( contentOffset.x < minX )
                            destX = minX; 
                        else
                            destX = maxX; 
                        timerX = 0.0f;
                    }
                    else {
                        if ( showSliderOnDragging ) {
                            StopCoroutine ( "FadeToHorizontal" );
                            StartCoroutine ( "FadeToHorizontal", new FadeToParams( horizontalSlider, 0.0f, 0.3f ) );
                        }
                    }
                }
            }

            if ( flickingY ) {

                velocity.y *= deceleration;

                if ( scrollDirection != ScrollDirection.Horizontal ) {
                    if ( contentOffset.y < minY || contentOffset.y > maxY ) {
                        velocity.y *= bounce;
                    }
                }

                if ( Mathf.Abs(velocity.y) <= flickingStop ) {
                    flickingY = false;
                    if ( contentOffset.y < minY || contentOffset.y > maxY ) {
                        scrollingToY = true;

                        srcY = contentOffset.y;
                        if ( contentOffset.y < minY )
                            destY = minY; 
                        else
                            destY = maxY; 
                        timerY = 0.0f;
                    }
                    else {
                        if ( showSliderOnDragging ) {
                            StopCoroutine ( "FadeToVertical" );
                            StartCoroutine ( "FadeToVertical", new FadeToParams( verticalSlider, 0.0f, 0.3f ) );
                        }
                    }
                }
            }
        }

        if ( scrollingToX || scrollingToY ) {
            if ( scrollingToX ) {
                timerX += Time.deltaTime;
                if ( timerX >= bounceBackDuration ) {
                    contentOffset.x = destX;
                    scrollingToX = false;

                    if ( showSliderOnDragging ) {
                        StopCoroutine ( "FadeToHorizontal" );
                        StartCoroutine ( "FadeToHorizontal", new FadeToParams( horizontalSlider, 0.0f, 0.3f ) );
                    }
                }
                else {
                    float ratio = timerX / bounceBackDuration;
                    ratio = easing(ratio);
                    float x = Mathf.Lerp( srcX, destX, ratio );
                    contentOffset.x = x;
                }
            }

            if ( scrollingToY ) {
                timerY += Time.deltaTime;
                if ( timerY >= bounceBackDuration ) {
                    contentOffset.y = destY;
                    scrollingToY = false;

                    if ( showSliderOnDragging ) {
                        StopCoroutine ( "FadeToVertical" );
                        StartCoroutine ( "FadeToVertical", new FadeToParams( verticalSlider, 0.0f, 0.3f ) );
                    }
                }
                else {
                    float ratio = timerY / bounceBackDuration;
                    ratio = easing(ratio);
                    float y = Mathf.Lerp( srcY, destY, ratio );
                    contentOffset.y = y;
                }
            }

            SetOffset(contentOffset);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public override bool OnEvent ( exUIEvent _e ) {
        exUIMng uimng = exUIMng.instance;

        // ======================================================== 
        if ( _e.category == exUIEvent.Category.Mouse ) {
        // ======================================================== 

            if ( _e.type == exUIEvent.Type.MouseUp ) {
                if ( isPressing ) {
                    if ( uimng.GetMouseFocus() == this ) {
                        uimng.SetMouseFocus(null);
                    }
                    if ( isDragging ) {
                        isDragging = false;

                        if ( Time.time - pressTime < 0.01f ) {
                            velocity = Vector2.zero;
                        }
                        else {
                            velocity = (pressPoint - _e.position)/(Time.time - pressTime);
                        }

                        if ( scrollDirection == ScrollDirection.Vertical || contentOffset.x < minX || contentOffset.x > maxX ) {
                            velocity.x = 0.0f;
                        }
                        if ( scrollDirection == ScrollDirection.Horizontal || contentOffset.y < minY || contentOffset.y > maxY ) {
                            velocity.y = 0.0f;
                        }

                        flickingX = true;
                        flickingY = true;
                    }
                    isPressing = false;
                }
                return true;
            }
            else if ( _e.type == exUIEvent.Type.MouseDown &&
                      _e.buttons == exUIEvent.MouseButtonFlags.Left ) 
            {
                isPressing = true;
                scrollingToX = false;
                scrollingToY = false;
                velocity = Vector2.zero;
                pressPoint = _e.position;
                pressTime = Time.time;
                return true;
            }
            else if ( _e.type == exUIEvent.Type.MouseMove &&
                      _e.buttons == exUIEvent.MouseButtonFlags.Left ) 
            {
                if ( isDragging == false ) {
                    if ( (pressPoint - _e.position).sqrMagnitude > 1.0f ) {
                        pressPoint = _e.position;
                        pressTime = Time.time;
                        isDragging = true;
                        uimng.SetMouseFocus( this );

                        if ( showSliderOnDragging ) {
                            if ( scrollDirection != ScrollDirection.Vertical ) {
                                StopCoroutine ( "FadeToHorizontal" );
                                StartCoroutine ( "FadeToHorizontal", new FadeToParams( horizontalSlider, 1.0f, 0.3f ) );
                            }

                            if ( scrollDirection != ScrollDirection.Horizontal ) {
                                StopCoroutine ( "FadeToVertical" );
                                StartCoroutine ( "FadeToVertical", new FadeToParams( verticalSlider, 1.0f, 0.3f ) );
                            }
                        }
                    }
                }

                if ( isDragging ) {
                    float dragCoefX = 1.0f;
                    float dragCoefY = 1.0f;

                    if ( contentOffset.x < minX || contentOffset.x > maxX )
                        dragCoefX = 0.4f;
                    if ( contentOffset.y < minY || contentOffset.y > maxY )
                        dragCoefY = 0.4f;

                    float newX = -_e.delta.x * dragCoefX;
                    float newY = -_e.delta.y * dragCoefY;
                    Vector2 scrollDistance = Vector2.zero;

                    //
                    if ( scrollDirection == ScrollDirection.Vertical )
                        newX = 0.0f;
                    else if ( scrollDirection == ScrollDirection.Horizontal )
                        newY = 0.0f;

                    scrollDistance = new Vector2( newX, newY );
                    contentOffset += scrollDistance;
                    SetOffset ( contentOffset );
                }

                return true;
            }
        }

        // ======================================================== 
        else if ( _e.category == exUIEvent.Category.Touch ) {
        // ======================================================== 

            if ( _e.type == exUIEvent.Type.TouchUp ) {
                if ( isPressing ) {
                    if ( uimng.GetTouchFocus(_e.touchID) == this ) {
                        uimng.SetTouchFocus( _e.touchID, null );
                    }
                    if ( isDragging ) {
                        isDragging = false;

                        if ( Time.time - pressTime < 0.01f ) {
                            velocity = Vector2.zero;
                        }
                        else {
                            velocity = (pressPoint - _e.position)/(Time.time - pressTime);
                        }

                        if ( scrollDirection == ScrollDirection.Vertical || contentOffset.x < minX || contentOffset.x > maxX ) {
                            velocity.x = 0.0f;
                        }
                        if ( scrollDirection == ScrollDirection.Horizontal || contentOffset.y < minY || contentOffset.y > maxY ) {
                            velocity.y = 0.0f;
                        }

                        flickingX = true;
                        flickingY = true;
                    }
                    isPressing = false;
                }
                return true;
            }
            else if ( _e.type == exUIEvent.Type.TouchDown ) {
                isPressing = true;
                scrollingToX = false;
                scrollingToY = false;
                velocity = Vector2.zero;
                pressPoint = _e.position;
                pressTime = Time.time;
                return true;
            }
            else if ( _e.type == exUIEvent.Type.TouchMove ) {
                if ( isDragging == false ) {

                    if ( (pressPoint - _e.position).sqrMagnitude > 1.0f ) {
                        pressPoint = _e.position;
                        pressTime = Time.time;
                        isDragging = true;
                        uimng.SetTouchFocus( _e.touchID, this );

                        if ( showSliderOnDragging ) {
                            if ( scrollDirection != ScrollDirection.Vertical ) {
                                StopCoroutine ( "FadeToHorizontal" );
                                StartCoroutine ( "FadeToHorizontal", new FadeToParams( horizontalSlider, 1.0f, 0.3f ) );
                            }

                            if ( scrollDirection != ScrollDirection.Horizontal ) {
                                StopCoroutine ( "FadeToVertical" );
                                StartCoroutine ( "FadeToVertical", new FadeToParams( verticalSlider, 1.0f, 0.3f ) );
                            }
                        }
                    }
                }

                if ( isDragging ) {
                    float dragCoefX = 1.0f;
                    float dragCoefY = 1.0f;

                    if ( contentOffset.x < minX || contentOffset.x > maxX )
                        dragCoefX = 0.4f;
                    if ( contentOffset.y < minY || contentOffset.y > maxY )
                        dragCoefY = 0.4f;

                    float newX = -_e.delta.x * dragCoefX;
                    float newY = -_e.delta.y * dragCoefY;
                    Vector2 scrollDistance = Vector2.zero;

                    //
                    if ( scrollDirection == ScrollDirection.Vertical )
                        newX = 0.0f;
                    else if ( scrollDirection == ScrollDirection.Horizontal )
                        newY = 0.0f;

                    scrollDistance = new Vector2( newX, newY );
                    contentOffset += scrollDistance;
                    SetOffset ( contentOffset );
                }

                return true;
            }
        }

        return false;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected override void OnSizeChanged ( float _newWidth, float _newHeight ) {
        base.OnSizeChanged( _newWidth, _newHeight );
        Commit();

        float startX = clipRect.transform.localPosition.x;
        float startY = clipRect.transform.localPosition.y;
        float endX = startX + clipRect.width;
        float endY = startY + clipRect.height;

        // horizontalBar, verticalBar
        if ( contentAnchor )
            contentAnchor.transform.localPosition = new Vector3 ( 0.0f, 0.0f, contentAnchor.transform.localPosition.z );

        if ( horizontalBar )
            horizontalBar.transform.localPosition = new Vector3 ( startX, endY + style.padding.bottom, horizontalBar.transform.localPosition.z );

        if ( verticalBar )
            verticalBar.transform.localPosition = new Vector3 ( endX + style.padding.right, startY, verticalBar.transform.localPosition.z );

        if ( horizontalSlider )
            horizontalSlider.transform.localPosition = new Vector3 ( horizontalBar.transform.localPosition.x, 
                                                                     horizontalBar.transform.localPosition.y, 
                                                                     horizontalSlider.transform.localPosition.z );

        if ( verticalSlider )
            verticalSlider.transform.localPosition = new Vector3 ( verticalBar.transform.localPosition.x, 
                                                                   verticalBar.transform.localPosition.y, 
                                                                   verticalSlider.transform.localPosition.z );

        float hbarHeight = (horizontalBar && horizontalBar.guiBorder) ? horizontalBar.guiBorder.border.vertical : 0.0f;
        float vbarWidth = (verticalBar && verticalBar.guiBorder) ? verticalBar.guiBorder.border.horizontal : 0.0f;

        if ( horizontalBar ) {
            horizontalBar.width = clipRect.width - vbarWidth; 
            horizontalBar.height = hbarHeight;
        }
        if ( verticalBar ) {
            verticalBar.height = clipRect.height - hbarHeight;
            verticalBar.width = vbarWidth;
        }

        // resize clip rect
        clipRect.anchor = Anchor.TopLeft;
        clipRect.width = _newWidth - style.padding.left - style.padding.right - vbarWidth;
        clipRect.height = _newHeight - style.padding.top - style.padding.bottom - hbarHeight;
        clipRect.transform.localPosition = new Vector3 ( -_newWidth * 0.5f + style.padding.left,
                                                         -style.padding.top,
                                                         clipRect.transform.localPosition.z );

        //
        if ( horizontalSlider ) horizontalSlider.width = (contentWidth <= clipRect.width) ? clipRect.width : clipRect.width/contentWidth * clipRect.width;
        if ( verticalSlider ) verticalSlider.height = (contentHeight <= clipRect.height) ? clipRect.height : clipRect.height/contentHeight * clipRect.height;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected float GetContentWidth () {
        float maxWidth = 9999.0f;
        for ( int i = 0; i < children.Count; ++i ) {
            exUIElement el = children[i];
            if ( el.boundingRect.width > maxWidth )
                maxWidth = el.boundingRect.width;
        }
        return maxWidth;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected float GetContentHeight () {
        float cHeight = 0.0f;
        for ( int i = 0; i < children.Count; ++i ) {
            exUIElement el = children[i];
            cHeight += el.boundingRect.height;
        }
        return cHeight;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void Clear () {
        clipRect.Clear();
        children.Clear();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void AddElement ( exUIElement _el ) {
        int lastEl = children.Count-1;
        AddChild(_el);

        if ( lastEl == -1 ) {
            _el.transform.parent = contentAnchor;
            _el.transform.localPosition = new Vector3( 0.0f, -_el.style.margin.top, 0.0f );
        }
        else {
            _el.transform.parent = contentAnchor;
            float y = children[lastEl].transform.localPosition.y 
                - ( children[lastEl].boundingRect.height
                    + children[lastEl].style.margin.bottom
                    + _el.style.margin.top )
                ;
            _el.transform.localPosition = new Vector3 ( 0.0f, y, 0.0f );
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void UpdateChildPosition () {
        float y = 0.0f;

        for ( int i = 0; i < children.Count; ++i ) {

            exUIElement el = children[i];

            y -= el.style.margin.top;
            el.transform.localPosition = new Vector3( 0.0f, y, 0.0f );
            y -= el.boundingRect.height;
            y -= el.style.margin.bottom;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void UpdateClipList () {
        exPlane[] planes = contentAnchor.GetComponentsInChildren<exPlane>();
        foreach ( exPlane plane in planes ) {
            if ( plane as exUIElement == false )
                clipRect.AddPlane(plane);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void UpdateLayout () {
        float y = 0.0f;
        float x = 0.0f;

        for ( int i = 0; i < children.Count; ++i ) {
            exUIElement el = children[i];
            Vector3 pos = el.transform.localPosition; 
            pos.x = el.style.margin.left;
            pos.y = y + el.style.margin.top + el.style.margin.bottom;
            y = pos.y + el.boundingRect.height;
            if ( el.boundingRect.width > x )
                x = el.boundingRect.width; 
        }

        contentHeight = Mathf.Max( y, 0.1f );
        contentWidth = Mathf.Max( x, 0.1f );

        minX = 0.0f;
        maxX = Mathf.Max(contentWidth - clipRect.width, 0.0f);
        
        minY = -Mathf.Max(contentHeight - clipRect.height, 0.0f);
        maxY = 0.0f;

        if ( horizontalSlider ) horizontalSlider.width = (contentWidth <= clipRect.width) ? clipRect.width : clipRect.width/contentWidth * clipRect.width;
        if ( verticalSlider ) verticalSlider.height = (contentHeight <= clipRect.height) ? clipRect.height : clipRect.height/contentHeight * clipRect.height;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void SetOffset ( Vector2 _offset ) {
        float startX = clipRect.transform.localPosition.x;
        float startY = clipRect.transform.localPosition.y;

        // vslider
        if ( verticalSlider != null && verticalSlider.enabled ) {
            float vsliderX = verticalSlider.transform.localPosition.x;
            float vsliderY = startY + _offset.y * clipRect.height/contentHeight;
            float over = 0.0f;

            if ( _offset.y < minY )
                over = minY - _offset.y;
            else if ( _offset.y > maxY )
                over = _offset.y - maxY;

            float vsliderHeight = (contentHeight <= clipRect.height) ? clipRect.height : clipRect.height/contentHeight * clipRect.height;
            verticalSlider.height = vsliderHeight - over;

            if ( _offset.y < minY )
                vsliderY = -clipRect.height + verticalSlider.height - style.padding.top;
            else if ( _offset.y > maxY )
                vsliderY = - style.padding.top;

            verticalSlider.transform.localPosition = new Vector3( vsliderX, vsliderY, verticalSlider.transform.localPosition.z );
        }

        // hslider
        if ( horizontalSlider != null && horizontalSlider.enabled ) {
            float hsliderX = startX + _offset.x * clipRect.width/contentWidth;
            float hsliderY = horizontalSlider.transform.localPosition.y;
            float over = 0.0f;

            if ( _offset.x < minX )
                over = minX - _offset.x;
            else if ( _offset.x > maxX )
                over = _offset.x - maxX;

            float hsliderWidth = (contentWidth <= clipRect.width) ? clipRect.width : clipRect.width/contentWidth * clipRect.width;
            verticalSlider.width = hsliderWidth - over;

            if ( _offset.x < minX )
                hsliderX = clipRect.width - horizontalSlider.width + style.padding.left;
            else if ( _offset.x > maxX )
                hsliderX = 0.0f;

            horizontalSlider.transform.localPosition = new Vector3( hsliderX, hsliderY, horizontalSlider.transform.localPosition.z );
        }

        contentAnchor.transform.localPosition = new Vector3 ( -_offset.x, -_offset.y, contentAnchor.transform.localPosition.z );
    }
}
