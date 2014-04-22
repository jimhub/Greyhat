using UnityEngine;
using System.Collections.Generic;
using Vectrosity;

public class PinCurve
{
    public static Dictionary<GameObject, PinCurve> curveDict = new Dictionary<GameObject, PinCurve>();
    public static Camera guiCam;

    private GameObject _goA, _goB;

    private Vector2[] _curvePoints = new Vector2[4];
    private VectorLine _line;

    private Vector3 _goAScreenPos, _goBScreenPos;

    public void init(GameObject goA, GameObject goB, Color color)
    {
        setObjectA(goA);
        setObjectB(goB);

        Vector2[] linePoints = new Vector2[20];

        _line = new VectorLine("CurveLine", linePoints, color, null, 3.0f, LineType.Continuous);
        redraw();
    }

    public void redraw()
    {
        _goAScreenPos = guiCam.WorldToScreenPoint(_goA.transform.position);
        _goBScreenPos = guiCam.WorldToScreenPoint(_goB.transform.position);

        _curvePoints[0] = new Vector2(_goAScreenPos.x, _goAScreenPos.y);
        _curvePoints[1] = _curvePoints[0] + (Vector2)(_goA.transform.position - _goA.transform.parent.position).normalized * 50f;

        _curvePoints[2] = new Vector2(_goBScreenPos.x, _goBScreenPos.y);
        _curvePoints[3] = _curvePoints[2] + (Vector2)(_goB.transform.position - _goB.transform.parent.position).normalized * 50f;

        _line.MakeCurve(_curvePoints[0], _curvePoints[1], _curvePoints[2], _curvePoints[3], 19);
        _line.Draw();
    }

    public void destroy()
    {
        VectorLine.Destroy(ref _line);

        curveDict.Remove(_goA);
        curveDict.Remove(_goB);
    }

    public void setObjectA(GameObject go)
    {
        if (_goA != null)
            curveDict.Remove(_goA);

        _goA = go;
        curveDict.Add(go, this);
    }

    public void setObjectB(GameObject go)
    {
        if (_goB != null)
            curveDict.Remove(_goB);

        _goB = go;
        curveDict.Add(go, this);
    }

    public GameObject getObjectA()
    {
        return _goA;
    }

    public GameObject getObjectB()
    {
        return _goB;
    }

    public void swapObjects()
    {
        GameObject temp = _goA;
        _goA = _goB;
        _goB = temp;
    }

    public static PinCurve findCurve(GameObject go)
    {
        PinCurve curve = null;

        curveDict.TryGetValue(go, out curve);
        
        return curve;
    }

    public static void destroyLine(GameObject go)
    {
        PinCurve curve;

        if (curveDict.TryGetValue(go, out curve))
        {
            curve.destroy();
        }
    }

    public static void updateLine(GameObject go)
    {
        PinCurve curve;

        if (curveDict.TryGetValue(go, out curve))
        {
            curve.redraw();
        }
    }
}
