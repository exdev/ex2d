// ======================================================================================
// File         : exLayerMng.cs
// Author       : Wu Jie 
// Last Change  : 11/06/2011 | 17:23:35 PM | Sunday,November
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
/// A component to manage draw order
/// 
///////////////////////////////////////////////////////////////////////////////

[ExecuteInEditMode]
[AddComponentMenu("ex2D Helper/Layer Manager")]
public class exLayerMng : exLayer {

    bool needsUpdate = false;
    // TODO: in the new version, we need to provide layer range, ( but same for dynamic layer ), this will prevent camera near/far clip changes

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected override void Awake () {
        base.Awake();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnPreRender () {
        if ( needsUpdate ) {
            needsUpdate = false;

            //
            List<exLayer> layerList = new List<exLayer>();
            // RecursivelyAddLayerForUpdate ( ref layerList, this ); // DISABLE: don't add exLayerMng (root) 
            foreach ( exLayer childLayer in children ) {
                RecursivelyAddLayerForUpdate ( ref layerList, childLayer );
            }

            //
            float dist = camera.farClipPlane - camera.nearClipPlane;
            dist -= 0.2f; // we leave 1.0 for both near and far clip
            float startFrom = transform.position.z + camera.nearClipPlane + 0.1f;
            float unitLayer = dist/layerList.Count;
            for ( int i = 0; i < layerList.Count; ++i ) {
                exLayer layer = layerList[i];
                if ( layer.updated )
                    continue;

                RecursivelyUpdateLayer ( layer.transform.root, startFrom, unitLayer );
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void RecursivelyUpdateLayer ( Transform _trans, float _startDepth, float _unitLayer ) {
        //
        exLayer layer = _trans.GetComponent<exLayer>(); 
        if ( layer != null && layer.updated == false ) {
            layer.updated = true;
            _trans.position = new Vector3 ( _trans.position.x, 
                                            _trans.position.y,
                                            _startDepth + _unitLayer * layer.index );
        }

        //
        foreach ( Transform child in _trans ) {
            RecursivelyUpdateLayer ( child, _startDepth, _unitLayer );
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void RecursivelyAddLayerForUpdate ( ref List<exLayer> _layerList, exLayer _curLayer ) {
        _curLayer.updated = false;
        _curLayer.index = _layerList.Count;
        _layerList.Add ( _curLayer );

        foreach ( exLayer childLayer in _curLayer.children ) {
            RecursivelyAddLayerForUpdate ( ref _layerList, childLayer );
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void UpdateLayer () {
        needsUpdate = true;
    }
}
