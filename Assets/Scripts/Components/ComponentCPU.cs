using UnityEngine;
using System.Collections.Generic;

public class ComponentCPU : MonoBehaviour {

    public GameObject pinGroup;
    public GameObject spriteObject;

    public PinType[] pinTypes;
    public int[] pinTypeSlots;

    private UISprite _spr;
    private Dictionary<PinType, List<ComponentPin>> _pins = new Dictionary<PinType, List<ComponentPin>>();

    private bool _currentlyActive = false;

	// Use this for initialization
	void Start () {
        _spr = spriteObject.GetComponent<UISprite>();

        for (int i = 0; i < pinTypes.Length; i++ )
        {
            addPin(pinTypes[i], i+1);
        }

        setAllPinsActive(false);
	}
	
	

    public void setActivePinType(PinType pinType)
    {
        foreach (KeyValuePair<PinType, List<ComponentPin>> pair in _pins)
        {
            foreach (ComponentPin pin in pair.Value)
            {
                NGUITools.SetActive(pin.gameObject, pair.Key == pinType);
            }
        }
    }

    public void setAllPinsActive(bool active=true, bool force=false)
    {
        foreach(KeyValuePair<PinType, List<ComponentPin>> pair in _pins) 
        {
            foreach (ComponentPin pin in pair.Value)
            {
                if (pin.spriteObject.GetComponent<UISprite>().spriteName.Equals("pinOpen"))
                    NGUITools.SetActive(pin.gameObject, active);
            }
        }
    }

    public void addPin(PinType pinType, int slot)
    {
        if (!_pins.ContainsKey(pinType))
            _pins.Add(pinType, new List<ComponentPin>());

        GameObject[] pins = new GameObject[4];
        ComponentPin[] compPins = new ComponentPin[4];

        for (int i = 0; i < 4; i++)
        {
            pins[i] = NGUITools.AddChild(pinGroup, SchemController.pinPrefab);
            ComponentPin compPin = pins[i].GetComponent<ComponentPin>();
            compPin.setType(pinType);
            compPin.rotate(i);

            _pins[pinType].Add(compPin);

            compPin.getPin().getSprite().alpha = 0.5f;
            compPin.getPin().setComponent(this);
        }

        pins[0].transform.localPosition = new Vector3(_spr.width - 12f, 16f * slot, 0f);
        pins[1].transform.localPosition = new Vector3(_spr.width - 16f*slot, _spr.height - 12f, 0f);
        pins[2].transform.localPosition = new Vector3(12f, _spr.height - 16f * slot, 0f);
        pins[3].transform.localPosition = new Vector3(16f * slot, 12f, 0f);

    }
}
