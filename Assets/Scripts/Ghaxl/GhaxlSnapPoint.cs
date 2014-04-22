using UnityEngine;
using System.Collections;

public class GhaxlSnapPoint : MonoBehaviour
{

    public GhaxlSnapPoint prevSibling;
    public GhaxlSnapPoint nextSibling;

    public GhaxlSnapPoint parent;
    public GhaxlSnapPoint child;

    public GameObject lowerSnapPoint;
    public GameObject innerSnapPoint;

    private GhaxlBlockBehavior _gbb;

    // Use this for initialization
    void Start()
    {
        _gbb = GetComponent<GhaxlBlockBehavior>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SnapToInner(GameObject go)
    {
        if (innerSnapPoint != null)
        {
            go.transform.parent = transform;
            go.transform.position = innerSnapPoint.transform.position;

            GhaxlSnapPoint gsp = go.GetComponent<GhaxlSnapPoint>();

            gsp.parent = this;

            if (child != null)
            {
                gsp.getLastSibling().SendMessage("SnapToLower", child.gameObject);
            }
            
            child = gsp;
        }
    }

    public void SnapToLower(GameObject go)
    {
        if (lowerSnapPoint != null)
        {
            go.transform.parent = transform;
            go.transform.position = lowerSnapPoint.transform.position;

            GhaxlSnapPoint gsp = go.GetComponent<GhaxlSnapPoint>();
            
            gsp.prevSibling = this;

            if (nextSibling != null)
            {
                gsp.getLastSibling().SendMessage("SnapToLower", nextSibling.gameObject);
            }

            nextSibling = gsp;
        }
    }

    public int calculateChildrenHeight()
    {
        int h = 0;

        GhaxlSnapPoint curSib = child;

        while (curSib != null)
        {
            h += curSib._gbb.getHeight();
            curSib = curSib.nextSibling;
        }

        return h;
    }

    public GhaxlSnapPoint getLastSibling()
    {
        GhaxlSnapPoint sibling = this;

        while (sibling.nextSibling != null)
            sibling = sibling.nextSibling;

        return sibling;
    }

    public GhaxlSnapPoint getFirstSibling()
    {
        GhaxlSnapPoint sibling = this;

        while (sibling.prevSibling != null)
            sibling = sibling.prevSibling;

        return sibling;
    }
}
