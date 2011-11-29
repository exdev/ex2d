// ======================================================================================
// File         : exLayer.cs
// Author       : Wu Jie 
// Last Change  : 11/07/2011 | 14:50:39 PM | Monday,November
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
/// The layer classes
///
///////////////////////////////////////////////////////////////////////////////

[ExecuteInEditMode]
[AddComponentMenu("ex2D Helper/Layer")]
public class exLayer : MonoBehaviour {

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public enum Type {
        Normal = 0,
        Abstract,
        Dynamic
    }

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    [SerializeField] protected exLayer parent_ = null;
    /// the parent of the layer
    // ------------------------------------------------------------------ 

    public exLayer parent {
        set {
            // already the parent
            if ( parent_ == value ) {
                return;
            }

            // check if it is parent layer or child
            exLayer parentLayer = value;
            exLayer lastLayer = value;
            while ( parentLayer != null ) {
                if ( parentLayer == this ) {
                    Debug.LogWarning("can't add self or child as parent");
                    return;
                } 
                lastLayer = parentLayer;
                parentLayer = lastLayer.parent;
            }

            // remove self from old parent's child list
            if ( parent_ ) {
                parent_.children_.Remove(this);
            }

            // add self to new parent
            if ( value ) {
                value.children_.Add(this);
            }

            // update layer mng
            if ( lastLayer == null ) {
                parentLayer = parent_;
                lastLayer = this;
                while ( parentLayer != null ) {
                    lastLayer = parentLayer;
                    parentLayer = lastLayer.parent;
                }
            }
            exLayerMng layerMng = lastLayer as exLayerMng;
            if ( layerMng ) {
                layerMng.UpdateLayer();
            }

            //
            parent_ = value;
        }
        get {
            return parent_;
        }
    }

    // ------------------------------------------------------------------ 
    [SerializeField] protected List<exLayer> children_ = new List<exLayer>();
    /// the children of the layer
    // ------------------------------------------------------------------ 

    public List<exLayer> children {
        get {
            return children_;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public Type type = Type.Normal;
    public float range = 100.0f;

    // editor only data
    public bool foldout = true;
    [System.NonSerialized] public int indentLevel = -1;
    [System.NonSerialized] public bool updated = false;
    [System.NonSerialized] public int index = -1;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected virtual void Awake () {
        // relink layers
        // NOTE: this happends when we clone a GameObject

        if ( children_ == null )
            children_ = new List<exLayer>();

        //
        if ( parent_ ) {
            if ( parent_.children_ == null ) {
                parent_.children_ = new List<exLayer>();
            }
            if ( parent_.children.IndexOf(this) == -1 ) {
                parent_.children_.Add(this);
            }
        }

        //
        for ( int i = children_.Count-1; i >= 0; --i ) {
            exLayer childLayer = children_[i];
            if ( childLayer == null || childLayer.parent != this ) {
                children_.RemoveAt(i);
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnDestroy () {
        parent = null;
    }

    // ------------------------------------------------------------------ 
    /// \param _index insert layer before the index of the children 
    /// \param _layer the layer to insert
    /// 
    /// Insert the _layer at the given _index of the children
    // ------------------------------------------------------------------ 

    public void InsertAt ( int _index, exLayer _layer ) {
        if ( _layer.parent == this ) {
            int index = children_.IndexOf (_layer);
            if ( index > _index ) {
                _layer.parent = null;
                children_.Insert ( _index, _layer );
                _layer.parent_ = this;
            }
            else {
                children_.Insert ( _index, _layer );
                _layer.parent = null;
                _layer.parent_ = this;
            }
        }
        else {
            _layer.parent = null;
            children_.Insert ( _index, _layer );
            _layer.parent_ = this;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // for editor
    // ------------------------------------------------------------------ 

    public void ForceSetParent ( exLayer _parent ) {
        // check if it is parent layer or child
        exLayer parentLayer = _parent;
        exLayer lastLayer = this;
        while ( parentLayer != null ) {
            if ( parentLayer == this ) {
                Debug.LogWarning("can't add self or child as parent");
                return;
            } 
            lastLayer = parentLayer;
            parentLayer = lastLayer.parent;
        }

        //
        if ( parent_ ) {
            parent_.children_.Remove(this);
        }

        //
        if ( _parent ) {
            _parent.children_.Add(this);
        }

        // update layer mng
        parentLayer = (parent_ == null ? _parent : parent_);
        lastLayer = this;
        while ( parentLayer != null ) {
            lastLayer = parentLayer;
            parentLayer = lastLayer.parent;
        }
        exLayerMng layerMng = lastLayer as exLayerMng;
        if ( layerMng ) {
            layerMng.UpdateLayer();
        }

        //
        parent_ = _parent;
    }
}
