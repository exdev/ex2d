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

            //
            exLayer rootLayer = null;

            // check if it is parent layer or child
            exLayer parentLayer = value;
            while ( parentLayer != null ) {
                if ( parentLayer == this ) {
                    Debug.LogWarning("can't add self or child as parent");
                    return;
                } 
                rootLayer = parentLayer;
                parentLayer = rootLayer.parent;
            }

            // remove self from old parent's child list
            if ( parent_ ) {
                parent_.children_.Remove(this);
            }

            // add self to new parent
            if ( value ) {
                value.children_.Add(this);
            }

            // get layer mng
            if ( rootLayer == null ) {
                parentLayer = parent_;
                rootLayer = this;
                while ( parentLayer != null ) {
                    rootLayer = parentLayer;
                    parentLayer = rootLayer.parent;
                }
            }
            exLayerMng layerMng = rootLayer as exLayerMng;

            // get special layer to add
            exLayer specialLayerToAdd = null;
            parentLayer = value;
            while ( parentLayer != null ) {
                specialLayerToAdd = parentLayer;
                if ( specialLayerToAdd.type != Type.Normal )
                    break;
                parentLayer = specialLayerToAdd.parent;
            }
            if ( specialLayerToAdd != null && specialLayerToAdd.isDirty == false ) {
                if ( layerMng ) layerMng.AddDirtyLayer(specialLayerToAdd);
            }

            // get special layer to remove
            exLayer specialLayerToRemove = null;
            parentLayer = parent_;
            while ( parentLayer != null ) {
                specialLayerToRemove = parentLayer;
                if ( specialLayerToRemove.type != Type.Normal )
                    break;
                parentLayer = specialLayerToRemove.parent;
            }
            if ( specialLayerToRemove != null && specialLayerToRemove.isDirty == false ) {
                if ( layerMng ) layerMng.AddDirtyLayer(specialLayerToRemove);
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
    [SerializeField] protected Type type_ = Type.Normal;
    // Desc: 
    // NOTE: dynamic/abstract NEVER contains child with dynamic
    // ------------------------------------------------------------------ 

    public Type type {
        set {
            if ( value != Type.Normal ) {
                bool isChildOfSpecialLayer = false;
                exLayer parentLayer = parent_;
                while ( parentLayer != null ) {
                    if ( parentLayer.type != Type.Normal ) {
                        isChildOfSpecialLayer = true;
                        break;
                    }
                    parentLayer = parentLayer.parent;
                }
                if ( isChildOfSpecialLayer == false ) {
                    type_ = value;
                }
                else {
                    type_ = Type.Normal;
                }

                //
                parentLayer = parent_;
                exLayer rootLayer = this;
                while ( parentLayer != null ) {
                    rootLayer = parentLayer;
                    parentLayer = rootLayer.parent;
                }

                exLayerMng layerMng = rootLayer as exLayerMng;
                if ( layerMng != null ) {
                    layerMng.AddDirtyLayer(layerMng);
                }
            }
            else {
                type_ = value;
            }
        }
        get { return type_; }
    }

    // ------------------------------------------------------------------ 
    [SerializeField] protected int range_ = 1;
    // Desc: 
    // ------------------------------------------------------------------ 

    public int range {
        set { 
            int clampValue = Mathf.Clamp ( value, 1, 1000 ); 
            if ( range_ != clampValue ) {
                range_ = clampValue;

                exLayer parentLayer = parent_;
                exLayer rootLayer = this;
                while ( parentLayer != null ) {
                    rootLayer = parentLayer;
                    parentLayer = rootLayer.parent;
                }

                exLayerMng layerMng = rootLayer as exLayerMng;
                if ( layerMng != null ) {
                    layerMng.AddDirtyLayer(layerMng);
                }
            }
        }
        get { return range_; }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public float depth = 0.0f; // depth calculated

    // ======================================================== 
    // editor only data
    // ======================================================== 

    public bool foldout = true;
    [System.NonSerialized] public int indentLevel = 0;
    [System.NonSerialized] public bool isDirty = false;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected void Awake () {
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
            Dirty ();
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
            int curIndex = children_.IndexOf (_layer);
            if ( curIndex > _index ) {
                _layer.parent = null;
                children_.Insert ( _index, _layer );
                _layer.parent_ = this;
            }
            else if ( curIndex < _index ) {
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
    // NOTE: only used in Editor
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
            layerMng.AddDirtyLayer(layerMng);
        }

        //
        parent_ = _parent;
    }

    // ------------------------------------------------------------------ 
    // Desc: manually add the layer to layerMng's dirty layer list, useful when rotate the sprite by transform.rotation
    // ------------------------------------------------------------------ 

    public void Dirty () {
        exLayer lastLayer = this;
        exLayer parentLayer = parent_;

        while ( parentLayer != null ) {
            lastLayer = parentLayer;
            parentLayer = lastLayer.parent;
        }

        exLayerMng layerMng = lastLayer as exLayerMng;
        if ( layerMng ) {
            layerMng.AddDirtyLayer(this);
        }
    }
}
