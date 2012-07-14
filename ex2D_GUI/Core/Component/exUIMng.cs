// ======================================================================================
// File         : exUIMng.cs
// Author       : Wu Jie 
// Last Change  : 08/13/2011 | 09:52:10 AM | Saturday,August
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

public class RaycastSorter : IComparer {
    int IComparer.Compare ( object _a, object _b ) {
        if ( !(_a is RaycastHit) || !(_b is RaycastHit) ) 
            return 0;

        RaycastHit raycastHitA = (RaycastHit)_a;
        RaycastHit raycastHitB = (RaycastHit)_b;

        return (int)Mathf.Sign(raycastHitA.distance - raycastHitB.distance);
    }
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

public class ElementSorter: IComparer<exUIElement> {
    public int Compare( exUIElement _a, exUIElement _b ) {
        exUIElement parent = null;
        int level_a = 0;
        int level_b = 0;

        // a level
        parent = _a.parent;
        while ( parent ) {
            ++level_a; 
            parent = parent.parent;
        }

        // b level
        parent = _b.parent;
        while ( parent ) {
            ++level_b; 
            parent = parent.parent;
        }

        return level_a - level_b;
    }
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

public class ElementSorterByZ: IComparer<exUIElement> {
    public int Compare( exUIElement _a, exUIElement _b ) {
        int r = Mathf.CeilToInt(_a.transform.position.z - _b.transform.position.z);
        if ( r != 0 )
            return r;
        return _a.GetInstanceID() - _b.GetInstanceID();
    }
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

public class exUIEvent {

    ///////////////////////////////////////////////////////////////////////////////
    // structures
    ///////////////////////////////////////////////////////////////////////////////

    public enum Type {
        Unknown = -1,
        MouseDown = 0,
        MouseUp,
        MouseMove,
        MouseEnter,
        MouseExit,
        TouchDown,
        TouchUp,
        TouchMove, 
        TouchEnter, 
        TouchExit, 
        KeyDown,
        KeyUp,
        // GamePadButtonDown,
        // GamePadButtonUp,
    }

    public enum Category {
        None = 0,
        Mouse,
        Keyboard,
        GamePad,
        Touch
    }

	[System.FlagsAttribute]
    public enum MouseButtonFlags {
        None    = 0,
        Left    = 1,
        Middle  = 2,
        Right   = 4,
    }

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public Category category = Category.None; // the event category
    public Type type = Type.Unknown;
    public Vector2 position = Vector2.zero;
    public Vector2 delta = Vector2.zero;
    public MouseButtonFlags buttons = MouseButtonFlags.None;
    public int touchID = -1;
    public KeyCode keyCode; // TODO:
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

public class exUIMng : MonoBehaviour {

    protected static exUIMng instance_ = null; 
    public static exUIMng instance {
        get {
            if ( instance_ == null ) {
                instance_ = FindObjectOfType ( typeof(exUIMng) ) as exUIMng;
                // if ( instance_ == null && Application.isEditor )
                //     Debug.LogError ("Can't find exUIMng in the scene, please create one first.");
                if ( instance_ != null )
                    instance_.Init();
            }
            return instance_;
        }
    }

    ///////////////////////////////////////////////////////////////////////////////
    // structures
    ///////////////////////////////////////////////////////////////////////////////

    public class EventInfo { 
        public exUIElement primaryElement = null;
        public exUIEvent uiEvent = null;
    }

    //
    public class TouchState {
        public int touchID = -1;
        public exUIElement hotElement = null;
        public exUIElement focusElement = null;
    }

    //
    public class MouseState {
        public Vector2 currentPos = Vector2.zero;
        public exUIEvent.MouseButtonFlags currentButtons = exUIEvent.MouseButtonFlags.None;
        public exUIElement hotElement = null;
        public exUIElement focusElement = null;
    }

    ///////////////////////////////////////////////////////////////////////////////
    // serializable 
    ///////////////////////////////////////////////////////////////////////////////

    public bool useRayCast = false; /// if your UI element is in 3D space, turn this on.

    ///////////////////////////////////////////////////////////////////////////////
    // non-serialized
    ///////////////////////////////////////////////////////////////////////////////

    // private RaycastSorter raycastSorter = new RaycastSorter();
    private ElementSorter elementSorter = new ElementSorter();
    private ElementSorterByZ elementSorterByZ = new ElementSorterByZ();

    // internal ui status
    private bool initialized = false;
    private MouseState mouseState = new MouseState();
    private List<TouchState> touchStateList = new List<TouchState>();

    //
    private List<EventInfo> eventInfoList = new List<EventInfo>();
    private List<exUIElement> rootElements = new List<exUIElement>();

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void Init () {
        if ( initialized )
            return;

        //
        if ( camera == null ) {
            Debug.LogError ( "The exUIMng should attach to a camera" );
            return;
        }

        // recursively add ui-tree
        exUIElement[] elements = FindObjectsOfType(typeof(exUIElement)) as exUIElement[];
        for ( int i = 0; i < elements.Length; ++i ) {
            exUIElement el = elements[i];
            exUIElement parent_el = el.FindParent();
            if ( parent_el == null ) {
                exUIElement.FindAndAddChild (el);

                //
                if ( rootElements.IndexOf(el) == -1 ) {
                    rootElements.Add(el);
                }
            }
        }

        //
        mouseState.currentPos = Vector2.zero;
        mouseState.currentButtons = exUIEvent.MouseButtonFlags.None;
        mouseState.hotElement = null;
        mouseState.focusElement = null;

        initialized = true;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public exUIElement GetTouchFocus ( int _touchID ) { 
        for ( int i = 0; i < touchStateList.Count; ++i ) {
            TouchState state = touchStateList[i];
            if ( state.touchID == _touchID )
                return state.focusElement;
        }
        return null;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void SetTouchFocus ( int _touchID, exUIElement _el ) { 
        for ( int i = 0; i < touchStateList.Count; ++i ) {
            TouchState state = touchStateList[i];
            if ( state.touchID == _touchID ) {
                state.focusElement = _el;
                return;
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public exUIElement GetMouseFocus () {
        return mouseState.focusElement;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void SetMouseFocus ( exUIElement _el ) {
        mouseState.focusElement = _el;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Awake () {
        Init ();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Start () {
#if UNITY_IPHONE
        if ( Application.isEditor == false ) {
            touchStateList.Clear();
        } else {
#endif
            mouseState.currentPos = Input.mousePosition;
#if UNITY_IPHONE
        }
#endif
    }
	
    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	void Update () {
        QueueEvents ();
        DispatchEvents ();
	}

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void QueueEvents () {
        ProcessPointer ();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void DispatchEvents () {
        for ( int i = 0; i < eventInfoList.Count; ++i ) {
            EventInfo info = eventInfoList[i];
            bool used = info.primaryElement.OnEvent(info.uiEvent);
            exUIElement uiParent = info.primaryElement;
            while ( used == false ) {
                uiParent = uiParent.parent;
                if ( uiParent == null )
                    break;
                used = uiParent.OnEvent(info.uiEvent);
            }
        }
        eventInfoList.Clear();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void ProcessPointer () {
#if UNITY_IPHONE
        if ( Application.isEditor == false ) {
            ProcessTouch();
        } else {
#endif
            ProcessMouse();
#if UNITY_IPHONE
        }
#endif
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void ProcessTouch () {
        for ( int i = 0; i < Input.touches.Length; ++i ) {
            Touch touch = Input.touches[i];
            TouchState touchState = null;
            exUIElement hotElement = PickElement(touch.position);
            
            //
            if ( touch.phase == TouchPhase.Began ) {
                if ( hotElement != null ) {
                    exUIEvent e = new exUIEvent(); 
                    e.category = exUIEvent.Category.Touch;
                    e.type =  exUIEvent.Type.TouchDown;
                    e.position = touch.position;
                    e.delta = touch.deltaPosition;
                    e.touchID = touch.fingerId;

                    EventInfo info = new EventInfo();
                    info.primaryElement = hotElement;
                    info.uiEvent = e;
                    eventInfoList.Add(info);
                }

                // NOTE: it must be null
                touchState = new TouchState();
                touchState.touchID = touch.fingerId;
                touchState.hotElement = hotElement;
                touchState.focusElement = null;
                touchStateList.Add(touchState);
            }
            else {
                // find the touch state
                int touchStateIndex = -1;
                for ( int j = 0; j < touchStateList.Count; ++j ) {
                    touchState = touchStateList[j];
                    if ( touchState.touchID == touch.fingerId ) {
                        touchStateIndex = j;
                        break;
                    }
                }

                // set the last and current hot element 
                exUIElement focusElement = null;
                exUIElement lastHotElement = null;
                if ( touchState != null ) {
                    lastHotElement = touchState.hotElement;
                    touchState.hotElement = hotElement;
                    focusElement = touchState.focusElement;
                }

                if ( touch.phase == TouchPhase.Ended ) {
                    if ( touchState != null ) {
                        if ( focusElement != null ) {
                            exUIEvent e = new exUIEvent(); 
                            e.category = exUIEvent.Category.Touch;
                            e.type =  exUIEvent.Type.TouchUp;
                            e.position = touch.position;
                            e.delta = touch.deltaPosition;
                            e.touchID = touch.fingerId;

                            EventInfo info = new EventInfo();
                            info.primaryElement = focusElement;
                            info.uiEvent = e;
                            eventInfoList.Add(info);
                        }
                        touchStateList.RemoveAt(touchStateIndex);
                    }
                }
                else if ( touch.phase == TouchPhase.Canceled ) {
                    if ( touchState != null )
                        touchStateList.RemoveAt(touchStateIndex);
                }
                else if ( touch.phase == TouchPhase.Moved ) {
                    // process hover event
                    if ( lastHotElement != hotElement ) {
                        // add hover-in event
                        if ( hotElement != null ) {
                            exUIEvent e = new exUIEvent(); 
                            e.category = exUIEvent.Category.Touch;
                            e.type =  exUIEvent.Type.TouchEnter;
                            e.position = touch.position;
                            e.delta = touch.deltaPosition;
                            e.touchID = touch.fingerId;

                            EventInfo info = new EventInfo();
                            info.primaryElement = hotElement;
                            info.uiEvent = e;
                            eventInfoList.Add(info);
                        }

                        // add hover-out event
                        if ( lastHotElement != null ) {
                            exUIEvent e = new exUIEvent(); 
                            e.category = exUIEvent.Category.Touch;
                            e.type =  exUIEvent.Type.TouchExit;
                            e.position = touch.position;
                            e.delta = touch.deltaPosition;
                            e.touchID = touch.fingerId;

                            EventInfo info = new EventInfo();
                            info.primaryElement = lastHotElement;
                            info.uiEvent = e;
                            eventInfoList.Add(info);
                        }
                    }

                    //
                    if ( focusElement != null ) {
                        exUIEvent e = new exUIEvent(); 
                        e.category = exUIEvent.Category.Touch;
                        e.type =  exUIEvent.Type.TouchMove;
                        e.position = touch.position;
                        e.delta = touch.deltaPosition;
                        e.touchID = touch.fingerId;

                        EventInfo info = new EventInfo();
                        info.primaryElement = focusElement;
                        info.uiEvent = e;
                        eventInfoList.Add(info);
                    }
                }
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void ProcessMouse () {
        // get current position and delta pos
        Vector2 lastPointerPos = mouseState.currentPos;
        mouseState.currentPos = Input.mousePosition;
        Vector2 deltaPos = mouseState.currentPos - lastPointerPos;

        // get current mouse button
        exUIEvent.MouseButtonFlags lastButtons = mouseState.currentButtons;
        exUIEvent.MouseButtonFlags buttonDown = exUIEvent.MouseButtonFlags.None;
        exUIEvent.MouseButtonFlags buttonUp = exUIEvent.MouseButtonFlags.None;

        // handle pressed
        mouseState.currentButtons = exUIEvent.MouseButtonFlags.None;
        if ( Input.anyKey ) {
            if ( Input.GetMouseButton(0) )
                mouseState.currentButtons |= exUIEvent.MouseButtonFlags.Left;
            if ( Input.GetMouseButton(1) )
                mouseState.currentButtons |= exUIEvent.MouseButtonFlags.Right;
            if ( Input.GetMouseButton(2) )
                mouseState.currentButtons |= exUIEvent.MouseButtonFlags.Middle;
        }

        // handle press
        if ( Input.anyKeyDown ) {
            if ( Input.GetMouseButtonDown(0) )
                buttonDown = exUIEvent.MouseButtonFlags.Left;
            else if ( Input.GetMouseButtonDown(1) )
                buttonDown = exUIEvent.MouseButtonFlags.Right;
            else if ( Input.GetMouseButtonDown(2) )
                buttonDown = exUIEvent.MouseButtonFlags.Middle;
        }

        // handle release
        if ( lastButtons != mouseState.currentButtons ) {
            if ( Input.GetMouseButtonUp(0) )
                buttonUp = exUIEvent.MouseButtonFlags.Left;
            else if ( Input.GetMouseButtonUp(1) )
                buttonUp = exUIEvent.MouseButtonFlags.Right;
            else if ( Input.GetMouseButtonUp(2) )
                buttonUp = exUIEvent.MouseButtonFlags.Middle;
        }
        
        // get hot element
        exUIElement lastHotElement = mouseState.hotElement;
        mouseState.hotElement = PickElement(mouseState.currentPos);

        // process hover event
        if ( lastHotElement != mouseState.hotElement ) {
            // add hover-in event
            if ( mouseState.hotElement != null ) {
                exUIEvent e = new exUIEvent(); 
                e.category = exUIEvent.Category.Mouse;
                e.type =  exUIEvent.Type.MouseEnter;
                e.position = mouseState.currentPos;
                e.delta = deltaPos;
                e.buttons = mouseState.currentButtons;

                EventInfo info = new EventInfo();
                info.primaryElement = mouseState.hotElement;
                info.uiEvent = e;
                eventInfoList.Add(info);
            }

            // add hover-out event
            if ( lastHotElement != null ) {
                exUIEvent e = new exUIEvent(); 
                e.category = exUIEvent.Category.Mouse;
                e.type =  exUIEvent.Type.MouseExit;
                e.position = mouseState.currentPos;
                e.delta = deltaPos;
                e.buttons = mouseState.currentButtons;

                EventInfo info = new EventInfo();
                info.primaryElement = lastHotElement;
                info.uiEvent = e;
                eventInfoList.Add(info);
            }
        }

        // add pointer-move event
        if ( mouseState.focusElement != null && deltaPos != Vector2.zero ) {
            exUIEvent e = new exUIEvent(); 
            e.category = exUIEvent.Category.Mouse;
            e.type =  exUIEvent.Type.MouseMove;
            e.position = mouseState.currentPos;
            e.delta = deltaPos;
            e.buttons = mouseState.currentButtons;

            EventInfo info = new EventInfo();
            info.primaryElement = mouseState.focusElement;
            info.uiEvent = e;
            eventInfoList.Add(info);
        }

        // add pointer-press event
        if ( mouseState.hotElement != null && buttonDown != exUIEvent.MouseButtonFlags.None ) {
            exUIEvent e = new exUIEvent(); 
            e.category = exUIEvent.Category.Mouse;
            e.type =  exUIEvent.Type.MouseDown;
            e.position = mouseState.currentPos;
            e.delta = deltaPos;
            e.buttons = buttonDown;

            EventInfo info = new EventInfo();
            info.primaryElement = mouseState.hotElement;
            info.uiEvent = e;
            eventInfoList.Add(info);
        }

        // add pointer-release event
        if ( mouseState.focusElement != null && buttonUp != exUIEvent.MouseButtonFlags.None ) {
            exUIEvent e = new exUIEvent(); 
            e.category = exUIEvent.Category.Mouse;
            e.type =  exUIEvent.Type.MouseUp;
            e.position = mouseState.currentPos;
            e.delta = deltaPos;
            e.buttons = buttonUp;

            EventInfo info = new EventInfo();
            info.primaryElement = mouseState.focusElement;
            info.uiEvent = e;
            eventInfoList.Add(info);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    exUIElement PickElement ( Vector2 _pos ) {
        if ( useRayCast ) {
            Ray ray = camera.ScreenPointToRay ( _pos );
            ray.origin = new Vector3 ( ray.origin.x, ray.origin.y, camera.transform.position.z );
            RaycastHit[] hits = Physics.RaycastAll(ray);
            // DISABLE { 
            // System.Array.Sort(hits, raycastSorter);
            // if ( hits.Length > 0 ) {
            //     for ( int i = 0; i < hits.Length; ++i ) {
            //         RaycastHit hit = hits[i];
            //         GameObject go = hit.collider.gameObject;
            //         exUIElement el = go.GetComponent<exUIElement>();
            //         if ( el && el.enabled ) {
            //             return el;
            //         }
            //     }
            // }
            // return null;
            // } DISABLE end 

            // TODO: consider clipping plane

            List<exUIElement> elements = new List<exUIElement>();
            for ( int i = 0; i < hits.Length; ++i ) {
                RaycastHit hit = hits[i];
                GameObject go = hit.collider.gameObject;
                exUIElement el = go.GetComponent<exUIElement>();
                if ( el && el.isActive ) {
                    elements.Add(el);
                }
            }
            if ( elements.Count > 0 ) {
                elements.Sort(elementSorter);
                return elements[elements.Count-1]; 
            }
            return null;
        }
        else {
            Vector3 worldPointerPos = camera.ScreenToWorldPoint ( _pos );
            rootElements.Sort(elementSorterByZ);
            for ( int i = 0; i < rootElements.Count; ++i ) {
                exUIElement el = rootElements[i];
                exUIElement resultEL = RecursivelyGetUIElement ( el, worldPointerPos );
                if ( resultEL != null )
                    return resultEL;
            }
            return null;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    exUIElement RecursivelyGetUIElement ( exUIElement _el, Vector2 _pos ) {
        if ( _el.enabled == false )
            return null;

        // if we are out of clipping plane
        if ( _el.clippingPlane != null ) {
            Vector2 clipPos = new Vector2( _pos.x - _el.clippingPlane.transform.position.x, 
                                           _pos.y - _el.clippingPlane.transform.position.y );
            if ( _el.clippingPlane.boundingRect.Contains(clipPos) == false )
                return null;
        }

        //
        Vector2 localPos = new Vector2( _pos.x - _el.transform.position.x, 
                                        _pos.y - _el.transform.position.y );
        if ( _el.boundingRect.Contains(localPos) ) {
            _el.children.Sort(elementSorterByZ);
            for ( int i = 0; i < _el.children.Count; ++i ) {
                exUIElement childEL = _el.children[i];
                exUIElement resultEL = RecursivelyGetUIElement ( childEL, _pos );
                if ( resultEL != null )
                    return resultEL;
            }
            return _el;
        }

        return null;
    } 
}
