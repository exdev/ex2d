// ======================================================================================
// File         : exDebugHelper.cs
// Author       : Wu Jie 
// Last Change  : 06/05/2011 | 11:08:21 AM | Sunday,June
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

public class exDebugHelper : MonoBehaviour {

    ///////////////////////////////////////////////////////////////////////////////
    // static
    ///////////////////////////////////////////////////////////////////////////////

    // static instance
    private static exDebugHelper instance = null;

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static void ScreenPrint ( string _text ) {
        instance.txtPrint.text = instance.txtPrint.text + _text + "\n"; 
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static void ScreenLog ( string _text ) {
        instance.logs.Add(_text);
        if ( instance.logs.Count > instance.logCount ) {
            instance.logs.RemoveAt(0);
        }
        instance.updateLogText = true;
    }

    ///////////////////////////////////////////////////////////////////////////////
    // serialized
    ///////////////////////////////////////////////////////////////////////////////

    public exSpriteFont txtPrint;
    public exSpriteFont txtFPS;
    public exSpriteFont txtLog;

    [SerializeField] protected bool showFps_ = true;
    public bool showFps {
        get { return showFps_; }
        set {
            if ( showFps_ != value ) {
                showFps_ = value;
                if ( txtFPS != null )
                    txtFPS.enabled = showFps_;
            }
        }
    }

    [SerializeField] protected bool showScreenPrint_ = true;
    public bool showScreenPrint {
        get { return showScreenPrint_; }
        set {
            if ( showScreenPrint_ != value ) {
                showScreenPrint_ = value;
                if ( txtPrint != null )
                    txtPrint.enabled = showScreenPrint_;
            }
        }
    }

    [SerializeField] protected bool showScreenLog_ = true;
    public bool showScreenLog {
        get { return showScreenLog_; }
        set {
            if ( showScreenLog_ != value ) {
                showScreenLog_ = value;
                if ( txtLog != null ) 
                    txtLog.enabled = showScreenLog_;
            }
        }
    }

    public int logCount = 10;

    ///////////////////////////////////////////////////////////////////////////////
    // non-serialized
    ///////////////////////////////////////////////////////////////////////////////

    [System.NonSerialized] public List<string> logs = new List<string>();
    [System.NonSerialized] public bool updateLogText = false; 
    private int frames = 0;
    private float fps = 0.0f;
    private float lastInterval = 0.0f;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Awake () {
        if ( instance == null )
            instance = this;

        txtPrint.text = "";
        txtFPS.text = "";
        txtLog.text = "";
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Start () {
        InvokeRepeating("UpdateFPS", 0.0f, 1.0f );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Update () {
        // count fps
        ++frames;

        // update log
        UpdateLog ();

        // NOTE: the OnGUI call multiple times in one frame, so we just clear text here.
        StartCoroutine ( CleanDebugText() );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void UpdateFPS () {
        float timeNow = Time.realtimeSinceStartup;
        fps = frames / (timeNow - lastInterval);
        frames = 0;
        lastInterval = timeNow;
        txtFPS.text = "fps: " + fps.ToString("f2");
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    IEnumerator CleanDebugText () {
        yield return new WaitForEndOfFrame();
        txtPrint.text = "";
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void UpdateLog () {
        if ( updateLogText ) {
            string text = "";
            foreach ( string l in logs ) {
                text = text + l + "\n";
            }
            txtLog.text = text;
            updateLogText = false;
        }
    }
}
