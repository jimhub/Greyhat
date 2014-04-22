using UnityEngine;
using System.Collections;

public class PinDrag : UIDragDropItem
{

    private static float offAlpha = 0.5f;
    private static Color disabledColor = Color.gray;

    public GameObject pinDotPrefab;
    public GameObject linkPoint;

    private UISprite _pinSpr;
    private UISprite _rootSpr;

    private bool _disabled = false;

    public PinType pinType;

    private ComponentCPU _comp;

    void Awake()
    {
        _pinSpr = GetComponent<UISprite>();
        _pinSpr.alpha = 0.5f;
    }

    public void setComponent(ComponentCPU comp)
    {
        _comp = comp;
    }

    public void setRootSprite(UISprite spr)
    {
        _rootSpr = spr;
    }

    public UISprite getSprite()
    {
        return _pinSpr;
    }

    void OnDragOver()
    {

        PinDrag pinDot = UICamera.currentTouch.dragged.GetComponent<PinDrag>();

        if (pinDot != null && pinDot._rootSpr != _pinSpr && pinDot.pinType == pinType)
        {
            _pinSpr.alpha = 1f;
            PinCurve curve = PinCurve.findCurve(pinDot.linkPoint);

            if (curve != null)
            {
                curve.setObjectB(linkPoint);
                curve.redraw();
            }
        }
            
    }

    void OnDragOut()
    {

        PinDrag pinDot = UICamera.currentTouch.dragged.GetComponent<PinDrag>();

        if (pinDot != null && pinDot._rootSpr != _pinSpr && pinDot.pinType == pinType)
        {
            _pinSpr.alpha = offAlpha;
            PinCurve curve = PinCurve.findCurve(linkPoint);

            if (curve != null)
            {
                curve.setObjectB(pinDot.linkPoint);
                curve.redraw();
            }
        }
            
    }

    void OnHover(bool isOver)
    {
        if (_comp != null && isOver)
        {
            _comp.setAllPinsActive(isOver);
            Debug.Log("WTF");
        }


        if (!_disabled && _pinSpr.spriteName.Equals("pinOpen"))
        {
            _pinSpr.alpha = (isOver ? 1f : offAlpha);
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

        GameObject clone = NGUITools.AddChild(transform.parent.parent.gameObject, pinDotPrefab);
        clone.transform.position = transform.position;
        //clone.transform.localEulerAngles = transform.localEulerAngles*-1f;
        clone.transform.localScale = transform.localScale;

        UISprite spr = clone.GetComponent<UISprite>();
        spr.color = _pinSpr.color;

        UICamera.Notify(UICamera.currentTouch.pressed, "OnPress", false);

        UICamera.currentTouch.pressed = clone;
        UICamera.currentTouch.dragged = clone;

        PinDrag item = clone.GetComponent<PinDrag>();
        item.Start();
        item.OnDragDropStart();

        item._pinSpr.alpha = 1f;
        item.pinType = pinType;

        _pinSpr.spriteName = "pinClosed";

        PinCurve curve = PinCurve.findCurve(linkPoint);

        if (curve == null)
        {
            curve = new PinCurve();
            curve.init(linkPoint, item.linkPoint, _pinSpr.color);
            item.setRootSprite(_pinSpr);
        }
        else
        {
            if (curve.getObjectA().Equals(linkPoint))
            {
                curve.swapObjects();
            }

            curve.setObjectB(item.linkPoint);
            _pinSpr.spriteName = "pinOpen";
            _pinSpr.alpha = offAlpha;
            item.setRootSprite(curve.getObjectA().transform.parent.GetComponent<UISprite>());
        }

    }

    protected override void OnDragDropStart()
    {
        base.OnDragDropStart();


    }

    protected override void OnDragDropMove(Vector3 delta)
    {
        base.OnDragDropMove(delta);

        PinCurve.updateLine(linkPoint);
    }

    protected override void OnDragDropRelease(GameObject surface)
    {

        UIDragDropContainer container = surface ? NGUITools.FindInParents<UIDragDropContainer>(surface) : null;

        if (container != null 
            && !container.gameObject.Equals(_rootSpr.gameObject) 
            && container.gameObject.GetComponent<PinDrag>().pinType == pinType)
        {
            PinCurve curve = PinCurve.findCurve(linkPoint);

            PinDrag pin = container.gameObject.GetComponent<PinDrag>();

            curve.setObjectB(pin.linkPoint);
            curve.redraw();

            UISprite spr = container.GetComponent<UISprite>();
            spr.spriteName = "pinClosed";
            spr.alpha = 1f;
        }
        else
        {
            PinCurve.destroyLine(linkPoint);
            _rootSpr.spriteName = "pinOpen";
            _rootSpr.alpha = offAlpha;
        }
        
        NGUITools.Destroy(gameObject);
    }
}
