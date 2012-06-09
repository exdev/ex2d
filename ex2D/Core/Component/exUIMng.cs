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

public class exUIEvent {

    ///////////////////////////////////////////////////////////////////////////////
    // structures
    ///////////////////////////////////////////////////////////////////////////////

    public enum Type {
        Unknown = -1,
        PointerPress = 0,
        PointerRelease,
        PointerMove,
        HoverIn,
        HoverOut,
        KeyPress,
        KeyRelease,
    }

	[System.FlagsAttribute]
    public enum PointerButtonFlags {
        None    = 0,
        Left    = 1,
        Middle  = 2,
        Right   = 4,
        Touch   = 8,
    }

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public exUIElement target = null;
    public Type type = Type.Unknown;
    public Vector2 position = Vector2.zero;
    public Vector2 delta = Vector2.zero;
    public PointerButtonFlags buttons = 0; // the pressed buttons
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
    // properties
    ///////////////////////////////////////////////////////////////////////////////


    //
    // private RaycastSorter raycastSorter = new RaycastSorter();
    private ElementSorter elementSorter = new ElementSorter();

    // internal ui status
    private bool initialized = false;
    private int touchID = -1;
    private Vector2 curPointerPos = Vector2.zero;
    private exUIEvent.PointerButtonFlags curPointerPressed = 0;
    private exUIElement hotElement = null;
    [System.NonSerialized] public exUIElement activeElement = null;

    //
    private List<exUIEvent> eventList = new List<exUIEvent>();

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
            }
        }

        initialized = true;
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
            touchID = -1;
        } else {
#endif
            curPointerPos = Input.mousePosition;
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
        for ( int i = 0; i < eventList.Count; ++i ) {
            exUIEvent e = eventList[i];
            bool used = e.target.OnEvent(e);
            exUIElement uiParent = e.target;
            while ( used == false ) {
                uiParent = uiParent.parent;
                if ( uiParent == null )
                    break;
                used = uiParent.OnEvent(e);
            }
        }
        eventList.Clear();
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
        //
        exUIElement lastHotElement = hotElement;
        Vector2 deltaPos = Vector2.zero;
        bool touchPress = false;
        bool touchRelease = false;

        //
        if ( touchID != -1 ) {
            bool found = false;
            for ( int i = 0; i < Input.touches.Length; ++i ) {
                Touch touch = Input.touches[i];
                if ( touchID == touch.fingerId ) {
                    found = true;
                    if ( touch.phase == TouchPhase.Ended ||
                         touch.phase == TouchPhase.Canceled ) 
                    {
                        deltaPos = touch.deltaPosition;
                        touchRelease = true;
                        touchID = -1;
                        break;
                    }
                    else if ( touch.phase == TouchPhase.Moved ) {
                        deltaPos = touch.deltaPosition;
                    }
                    curPointerPos = touch.position;
                    hotElement = PickElement(touch.position);
                }
            }

            // if not found
            if ( found == false ) {
                touchRelease = true;
                touchID = -1;
            }
        }
        else {
            for ( int i = 0; i < Input.touches.Length; ++i ) {
                Touch touch = Input.touches[i];
                if ( touch.phase == TouchPhase.Began ) {
                    hotElement = PickElement(touch.position);
                    if ( hotElement != null ) {
                        curPointerPos = touch.position;
                        touchID = touch.fingerId;
                        touchPress = true;
                        break;
                    }
                }
            }
        }

        // process hover event
        if ( lastHotElement != hotElement ) {
            // add hover-in event
            if ( hotElement != null ) {
                exUIEvent e = new exUIEvent(); 
                e.type =  exUIEvent.Type.HoverIn;
                e.position = curPointerPos;
                e.delta = deltaPos;
                e.target = hotElement;
                e.buttons = exUIEvent.PointerButtonFlags.Touch;
                eventList.Add(e);
            }

            // add hover-out event
            if ( lastHotElement != null ) {
                exUIEvent e = new exUIEvent(); 
                e.type =  exUIEvent.Type.HoverOut;
                e.position = curPointerPos;
                e.delta = deltaPos;
                e.target = lastHotElement;
                e.buttons = exUIEvent.PointerButtonFlags.Touch;
                eventList.Add(e);
            }
        }

        // add pointer-move event
        if ( activeElement != null && deltaPos != Vector2.zero ) {
            exUIEvent e = new exUIEvent(); 
            e.type =  exUIEvent.Type.PointerMove;
            e.position = curPointerPos;
            e.delta = deltaPos;
            e.target = activeElement;
            e.buttons = exUIEvent.PointerButtonFlags.Touch;
            eventList.Add(e);
        }

        // add pointer-press event
        if ( hotElement != null && touchPress ) {
            exUIEvent e = new exUIEvent(); 
            e.type =  exUIEvent.Type.PointerPress;
            e.position = curPointerPos;
            e.delta = deltaPos;
            e.target = hotElement;
            e.buttons = exUIEvent.PointerButtonFlags.Touch;
            eventList.Add(e);
        }

        // add pointer-press event
        if ( activeElement != null && touchRelease ) {
            exUIEvent e = new exUIEvent(); 
            e.type =  exUIEvent.Type.PointerRelease;
            e.position = curPointerPos;
            e.delta = deltaPos;
            e.target = activeElement;
            e.buttons = exUIEvent.PointerButtonFlags.Touch;
            eventList.Add(e);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void ProcessMouse () {
        //
        Vector2 lastPointerPos = curPointerPos;

        // get current position
        curPointerPos = Input.mousePosition;
        Vector2 deltaPos = curPointerPos - lastPointerPos;

        // get current mouse button
        exUIEvent.PointerButtonFlags lastPointerPressed = curPointerPressed;
        exUIEvent.PointerButtonFlags buttonDown = exUIEvent.PointerButtonFlags.None;
        exUIEvent.PointerButtonFlags buttonUp = exUIEvent.PointerButtonFlags.None;

        // handle pressed
        curPointerPressed = 0;
        if ( Input.anyKey ) {
            if ( Input.GetMouseButton(0) )
                curPointerPressed |= exUIEvent.PointerButtonFlags.Left;
            if ( Input.GetMouseButton(1) )
                curPointerPressed |= exUIEvent.PointerButtonFlags.Right;
            if ( Input.GetMouseButton(2) )
                curPointerPressed |= exUIEvent.PointerButtonFlags.Middle;
        }

        // handle press
        if ( Input.anyKeyDown ) {
            if ( Input.GetMouseButtonDown(0) )
                buttonDown = exUIEvent.PointerButtonFlags.Left;
            else if ( Input.GetMouseButton(1) )
                buttonDown = exUIEvent.PointerButtonFlags.Right;
            else if ( Input.GetMouseButton(2) )
                buttonDown = exUIEvent.PointerButtonFlags.Middle;
        }

        // handle release
        if ( lastPointerPressed != curPointerPressed ) {
            if ( Input.GetMouseButtonUp(0) )
                buttonUp = exUIEvent.PointerButtonFlags.Left;
            else if ( Input.GetMouseButtonUp(1) )
                buttonUp = exUIEvent.PointerButtonFlags.Right;
            else if ( Input.GetMouseButtonUp(2) )
                buttonUp = exUIEvent.PointerButtonFlags.Middle;
        }
        
        // get hot element
        exUIElement lastHotElement = hotElement;
        hotElement = PickElement(curPointerPos);

        // process hover event
        if ( lastHotElement != hotElement ) {
            // add hover-in event
            if ( hotElement != null ) {
                exUIEvent e = new exUIEvent(); 
                e.type =  exUIEvent.Type.HoverIn;
                e.position = curPointerPos;
                e.delta = deltaPos;
                e.target = hotElement;
                e.buttons = curPointerPressed;
                eventList.Add(e);
            }

            // add hover-out event
            if ( lastHotElement != null ) {
                exUIEvent e = new exUIEvent(); 
                e.type =  exUIEvent.Type.HoverOut;
                e.position = curPointerPos;
                e.delta = deltaPos;
                e.target = lastHotElement;
                e.buttons = curPointerPressed;
                eventList.Add(e);
            }
        }

        // add pointer-move event
        if ( activeElement != null && deltaPos != Vector2.zero ) {
            exUIEvent e = new exUIEvent(); 
            e.type =  exUIEvent.Type.PointerMove;
            e.position = curPointerPos;
            e.delta = deltaPos;
            e.target = activeElement;
            e.buttons = curPointerPressed;
            eventList.Add(e);
        }

        // add pointer-press event
        if ( hotElement != null && buttonDown != exUIEvent.PointerButtonFlags.None ) {
            exUIEvent e = new exUIEvent(); 
            e.type =  exUIEvent.Type.PointerPress;
            e.position = curPointerPos;
            e.delta = deltaPos;
            e.target = hotElement;
            e.buttons = buttonDown;
            eventList.Add(e);
        }

        // add pointer-press event
        if ( activeElement != null && buttonUp != exUIEvent.PointerButtonFlags.None ) {
            exUIEvent e = new exUIEvent(); 
            e.type =  exUIEvent.Type.PointerRelease;
            e.position = curPointerPos;
            e.delta = deltaPos;
            e.target = activeElement;
            e.buttons = buttonUp;
            eventList.Add(e);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    exUIElement PickElement ( Vector2 _pos ) {
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
}
