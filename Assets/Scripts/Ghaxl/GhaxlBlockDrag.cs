using UnityEngine;
using System.Collections;

public class GhaxlBlockDrag : UIDragDropItem
{

    private GameObject _dropArea;

    // Use this for initialization
    void Start()
    {
        base.Start();

        _dropArea = GameObject.Find("DropArea");
    }

    // Update is called once per frame
    void Update()
    {
        if (UICamera.hoveredObject != null)
        {
            //Debug.Log(UICamera.hoveredObject.name);
        }
    }

    void enableBoxColliders(bool enable=true)
    {
        BoxCollider[] bcs = gameObject.GetComponents<BoxCollider>();
        
        foreach(BoxCollider bc in bcs)
            bc.enabled = enable;

    }

    void enableSnapPoints(bool enable = true)
    {
        GhaxlBlockDrag dragChild;

        foreach (Transform child in transform)
        {
            if (child.tag.Equals("BlockSnapPoint"))
            {
                child.gameObject.collider.enabled = enable;
            }
            else
            {
                dragChild = child.GetComponent<GhaxlBlockDrag>();

                if (dragChild != null)
                    dragChild.enableSnapPoints(enable);
            }
        }
    }

    void OnDragStart()
    {
        if (!enabled || mTouchID != int.MinValue) return;

        // If we have a restriction, check to see if its condition has been met first
        if (restriction != Restriction.None)
        {
            if (restriction == Restriction.Horizontal)
            {
                Vector2 delta = UICamera.currentTouch.totalDelta;
                if (Mathf.Abs(delta.x) < Mathf.Abs(delta.y)) return;
            }
            else if (restriction == Restriction.Vertical)
            {
                Vector2 delta = UICamera.currentTouch.totalDelta;
                if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y)) return;
            }
            else if (restriction == Restriction.PressAndHold)
            {
                if (mPressTime + 1f > RealTime.time) return;
            }
        }

        if (cloneOnDrag)
        {
            GameObject clone = NGUITools.AddChild(transform.parent.gameObject, gameObject);
            clone.transform.localPosition = transform.localPosition;
            clone.transform.localRotation = transform.localRotation;
            clone.transform.localScale = transform.localScale;

            UIButtonColor bc = clone.GetComponent<UIButtonColor>();
            if (bc != null) bc.defaultColor = GetComponent<UIButtonColor>().defaultColor;

            UICamera.Notify(UICamera.currentTouch.pressed, "OnPress", false);

            UICamera.currentTouch.pressed = clone;
            UICamera.currentTouch.dragged = clone;

            GhaxlBlockDrag item = clone.GetComponent<GhaxlBlockDrag>();
            item.Start();
            item.OnDragDropStart();
            item.cloneOnDrag = false;

            

        }
        else
        {
            OnDragDropStart();
        }

    }

    protected override void OnDragDropStart()
    {
        // Automatically disable the scroll view
        if (mDragScrollView != null) mDragScrollView.enabled = false;

        enableBoxColliders(false);
        enableSnapPoints(false);

        mTouchID = UICamera.currentTouchID;
        mParent = mTrans.parent;
        mRoot = NGUITools.FindInParents<UIRoot>(mParent);
        mGrid = NGUITools.FindInParents<UIGrid>(mParent);
        mTable = NGUITools.FindInParents<UITable>(mParent);

        // Re-parent the item
        if (UIDragDropRoot.root != null)
            mTrans.parent = UIDragDropRoot.root;

        Vector3 pos = mTrans.localPosition;
        pos.z = 0f;
        mTrans.localPosition = pos;

        // Notify the widgets that the parent has changed
        NGUITools.MarkParentAsChanged(gameObject);

        if (mTable != null) mTable.repositionNow = true;
        if (mGrid != null) mGrid.repositionNow = true;

        NGUITools.AdjustDepth(gameObject, 1000);

        GhaxlSnapPoint gsp = GetComponent<GhaxlSnapPoint>();

        GhaxlSnapPoint gbbParent = null;

        if (gsp.prevSibling != null)
        {
            gbbParent = gsp.getFirstSibling().parent;

            gsp.prevSibling.nextSibling = null;
            gsp.prevSibling = null;

            
        }
        else
        {
            gbbParent = gsp.parent;

            if (gsp.parent != null)
            {
                gsp.parent.child = null;
                gsp.parent = null;
            }
        }

        if(gbbParent != null)
            gbbParent.GetComponent<GhaxlLoopBehavior>().clampInnerHeight();
    }

    protected override void OnDragDropRelease(GameObject surface)
    {

        GameObject dropBox = Ghaxl.dropContainerUnderPoint(Input.mousePosition);

        if (dropBox != null)
            surface = dropBox;
        else if (surface != null && !surface.name.Equals("DropArea"))
        {
            surface = _dropArea;
        }

        if (!cloneOnDrag)
        {
            mTouchID = int.MinValue;
            enableBoxColliders(true);
            enableSnapPoints(true);

            // Is there a droppable container?
            UIDragDropContainer container = surface ? NGUITools.FindInParents<UIDragDropContainer>(surface) : null;

            if (container != null)
            {
                
                if (container.tag.Equals("BlockSnapPoint"))
                {
                    if (container.name.Equals("InnerSnapPoint"))
                    {
                        container.transform.parent.SendMessage("SnapToInner", gameObject);

                        container.transform.parent.GetComponent<GhaxlLoopBehavior>().clampInnerHeight();
                    }
                    else
                    {
                        container.transform.parent.SendMessage("SnapToLower", gameObject);

                        GhaxlSnapPoint gsp = GetComponent<GhaxlSnapPoint>().getFirstSibling().parent;

                        if (gsp != null)
                        {
                            gsp.GetComponent<GhaxlLoopBehavior>().clampInnerHeight();
                        }
                    }
                }
                else
                {
                    // Container found -- parent this object to the container
                    mTrans.parent = (container.reparentTarget != null) ? container.reparentTarget : container.transform;

                    Vector3 pos = mTrans.localPosition;
                    pos.z = 0f;
                    mTrans.localPosition = pos;
                }
            }
            else
            {
                NGUITools.Destroy(gameObject);
            }

            // Update the grid and table references
            mParent = mTrans.parent;
            mGrid = NGUITools.FindInParents<UIGrid>(mParent);
            mTable = NGUITools.FindInParents<UITable>(mParent);

            // Re-enable the drag scroll view script
            if (mDragScrollView != null)
                mDragScrollView.enabled = true;

            // Notify the widgets that the parent has changed
            NGUITools.MarkParentAsChanged(gameObject);

            if (mTable != null) mTable.repositionNow = true;
            if (mGrid != null) mGrid.repositionNow = true;
        }
        else NGUITools.Destroy(gameObject);

        NGUITools.NormalizeDepths();

    }
}
