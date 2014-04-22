using UnityEngine;
using System.Collections;

public class GhaxlLoopBehavior : GhaxlBlockBehavior
{
    private static int minInnerHeight = 10;

    public UISprite leftSpr;
    public BoxCollider leftCollider;

    public UISprite bottomSpr;
    public BoxCollider bottomCollider;
    private Vector3 _bcOffset;

    private static Vector3 _lowerSnapPointOffset = new Vector3(0, -10, 0);

    private GhaxlSnapPoint _gsp;

    // Use this for initialization
    void Start()
    {
        _gsp = GetComponent<GhaxlSnapPoint>();

        _bcOffset = bottomCollider.center - bottomSpr.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setInnerHeight(int h)
    {
        h = Mathf.Max(h - 4, minInnerHeight);

        leftSpr.height = h;
        leftCollider.center = leftSpr.transform.localPosition + new Vector3(leftSpr.width / 2f, -leftSpr.height / 2f + 3f, 0f);
        leftCollider.size = new Vector3(leftSpr.width, leftSpr.height + 4, 1f);

        bottomSpr.transform.localPosition = leftSpr.transform.localPosition - new Vector3(0, h - 1, 0);
        bottomCollider.center = bottomSpr.transform.localPosition + _bcOffset;

        _gsp.lowerSnapPoint.transform.localPosition = bottomSpr.transform.localPosition + _lowerSnapPointOffset;
        
        if (_gsp.nextSibling != null)
        {
            _gsp.nextSibling.transform.position = _gsp.lowerSnapPoint.transform.position;
        }

        GhaxlSnapPoint p = _gsp.getFirstSibling().parent;

        if (p != null)
        {
            p.GetComponent<GhaxlLoopBehavior>().clampInnerHeight();
        }

    }

    public void clampInnerHeight()
    {
        setInnerHeight(_gsp.calculateChildrenHeight());
    }

    public override int getHeight()
    {
        return mainSpr.height + leftSpr.height + bottomSpr.height + heightModifier;
    }
}
