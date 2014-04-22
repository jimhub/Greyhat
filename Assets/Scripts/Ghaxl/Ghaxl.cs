using UnityEngine;
using System.Collections;

public class Ghaxl : MonoBehaviour
{

    public static LayerMask dropMask;

    public LayerMask dropContainerMask;

    // Use this for initialization
    void Start()
    {
        dropMask = dropContainerMask;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static int getPanelDepth(GameObject go)
    {
        UIPanel p = go.GetComponent<UIPanel>();

        if (p != null)
        {
            return p.depth;
        }

        return -666;
    }

    public static GameObject dropContainerUnderPoint(Vector3 pt)
    {
        GameObject dc = null;
        int maxDepth = -1;

        float dist = 100f;
        Vector3 inPos = Input.mousePosition;

        Ray ray = UICamera.currentCamera.ScreenPointToRay(inPos);
        RaycastHit[] hits = Physics.RaycastAll(ray, dist, Ghaxl.dropMask);

        foreach (RaycastHit hit in hits)
        {
            int curD = getPanelDepth(hit.collider.gameObject);

            if (curD > maxDepth)
            {
                maxDepth = curD;
                dc = hit.collider.gameObject;
            }
        }

        return dc;
    }
}
