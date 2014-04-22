using UnityEngine;
using System.Collections.Generic;

public class ComponentPin : MonoBehaviour
{

    public static Vector3[] labelPositions = new Vector3[4];
    public static Dictionary<PinType, PinTypeInfo> pinTypes;

    public GameObject spriteObject;
    public GameObject labelObject;

    public int startRotation = 0;

    private PinDrag _pin;
    private bool _isHidden = false;
    private float _prevAlpha = 0f;

    static ComponentPin()
    {
        pinTypes = new Dictionary<PinType, PinTypeInfo>();

        labelPositions[0] = new Vector3(-7f, -2f, 0f);
        labelPositions[1] = new Vector3(-2.4f, -5f, 0f);
        labelPositions[2] = new Vector3(0f, 0.3f, 0f);
        labelPositions[3] = new Vector3(-4.8f, 3f, 0f);

        pinTypes.Add(PinType.Bool, new PinTypeInfo("B", new Color(0.5f, 0.75f, 1f)));
        pinTypes.Add(PinType.Int, new PinTypeInfo("I", new Color(0.75f, 0.5f, 1f)));
        pinTypes.Add(PinType.Float, new PinTypeInfo("F", new Color(0.6f, 1f, 0.6f)));
        pinTypes.Add(PinType.String, new PinTypeInfo("S", new Color(1f, 0.75f, 0.5f)));
    }

    // Use this for initialization
    void Awake()
    {
        _pin = spriteObject.GetComponent<PinDrag>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setHidden(bool hidden = true)
    {
        _isHidden = hidden;

        UIPanel panel = GetComponent<UIPanel>();

        if (hidden)
        {
            _prevAlpha = panel.alpha;
            panel.alpha = 0f;
        }
        else
        {
            panel.alpha = _prevAlpha;
        }
        
    }

    public PinDrag getPin()
    {
        return _pin;
    }

    public void setType(PinType pinType)
    {
        UISprite spr = spriteObject.GetComponent<UISprite>();
        spr.color = pinTypes[pinType].pinColor;

        UILabel lbl = labelObject.GetComponent<UILabel>();
        lbl.text = pinTypes[pinType].labelChar;
    }

    public void rotate(int ri)
    {
        spriteObject.transform.localEulerAngles = new Vector3(0f, 0f, 90f * ri);

        labelObject.transform.localPosition = labelPositions[ri];
    }
}
