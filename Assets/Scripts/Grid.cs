using UnityEngine;
using System.Collections;
using Vectrosity;

public class Grid : MonoBehaviour
{

    public GameObject spr;
    public Camera cam;

    // Use this for initialization
    void Start()
    {

        float gridSpace = 16f;

        float xOffset = 0f;
        float yOffset = -4f;

        int gridW = (int)(800 / gridSpace) + 1;
        int gridH = (int)(600 / gridSpace) + 1;

        Vector2[] points = new Vector2[gridW * gridH];

        for (int y = 0; y < gridH; y++)
        {
            for (int x = 0; x < gridW; x++)
            {
                points[y * gridW + x] = new Vector2(xOffset + x * gridSpace, yOffset + y * gridSpace);
            }
        }

        VectorPoints.SetCamera(cam);
        PinCurve.guiCam = cam;

        //VectorPoints.SetCamera3D(cam);
        
        VectorPoints p = new VectorPoints("Grid", points, new Color(1f, 0.5f, 0f, 0.3f), null, 1.0f);
        //p.vectorObject.layer = 8;
        p.Draw();
        VectorPoints.SetVectorCamDepth(0);

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void reDraw()
    {

    }
}
