using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class GhaxlBlock : MonoBehaviour
{

    public List<Vector3> newVertices = new List<Vector3>();
    public List<int> newTriangles = new List<int>();
    public List<Vector2> newUV = new List<Vector2>();
    private Mesh mesh;

    private float blockWidth, blockHeight;
    private float texWidth, texHeight;

    private float meshSizeX;
    private float meshSizeY;

    private Dictionary<int, List<int>> vConstraints = new Dictionary<int, List<int>>();
    private Dictionary<int, List<int>> hConstraints = new Dictionary<int, List<int>>();

    private BoxCollider _bc;

    // Use this for initialization
    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;

        texWidth = 512f;
        texHeight = 512f;


        doNineSliceBlock(4, 457, 72, 21, 27, 6, 6, 6, (72f/42f));
        //doNineSliceBlock(4, 432, 72, 14, 7, 7, 6, 6);

        mesh.Clear();
        mesh.vertices = newVertices.ToArray();
        mesh.triangles = newTriangles.ToArray();
        mesh.uv = newUV.ToArray();
        mesh.Optimize();
        mesh.RecalculateNormals();

        _bc = gameObject.AddComponent<BoxCollider>();

    }

    // Update is called once per frame
    void Update()
    {
        moveVert(2, Time.deltaTime, 0f);
        moveVert(8, 0f, Time.deltaTime);

        refreshMesh();

        if (Input.GetKeyDown(KeyCode.R))
            Application.LoadLevel(0);
    }

    /// <summary>
    /// TODO: Turn these "do" functions into external definitions
    /// </summary>
    void doLoopBlock()
    {
        blockWidth = 72f;
        blockHeight = 42f;

        meshSizeX = blockWidth / blockHeight;
        meshSizeY = 1.0f;

        // Uppermost-left verts
        addVertex(0, 0, 4, 506);
        addVertex(37, 0, 41, 506);
        addVertex(37, 6, 41, 500);
        addVertex(0, 6, 4, 500);

        // Uppermost-middle (expandable verts)
        addVertex(66, 0, 70, 506);
        addVertex(66, 6, 70, 500);

        // Uppermost-right
        addVertex(72, 0, 76, 506);
        addVertex(72, 6, 76, 500);

        // Uppermain-left
        addVertex(0, 15, 4, 491);
        addVertex(37, 15, 41, 491);

        // Uppermain-middle (expandable verts)
        addVertex(66, 15, 70, 491);

        // Uppermain-right
        addVertex(72, 15, 76, 491);


        // Upperbottom-left
        addVertex(0, 21, 4, 485);
        addVertex(37, 21, 41, 485);

        // Upperbottom-middle (expandable verts)
        addVertex(66, 21, 70, 485);

        // Upperbottom-right
        addVertex(72, 21, 76, 485);


        // Left side (expandable in the vertical)
        addVertex(0, 29, 4, 477);
        addVertex(37, 29, 41, 477);


        // Bottom-left
        addVertex(0, 46, 4, 460);
        addVertex(37, 46, 41, 460);

        // Bottom-middle
        addVertex(66, 29, 70, 477);
        addVertex(66, 46, 70, 460);

        // Bottom-right
        addVertex(72, 29, 76, 477);
        addVertex(72, 46, 76, 460);

        // Uppermost
        addTriQuad(0, 1, 2, 3);
        addTriQuad(1, 4, 5, 2);
        addTriQuad(4, 6, 7, 5);

        // Upper
        addTriQuad(3, 2, 9, 8);
        addTriQuad(2, 5, 10, 9);
        addTriQuad(5, 7, 11, 10);

        // Upper-bottom
        addTriQuad(8, 9, 13, 12);
        addTriQuad(9, 10, 14, 13);
        addTriQuad(10, 11, 15, 14);

        // left
        addTriQuad(12, 13, 17, 16);

        // Bottom
        addTriQuad(16, 17, 19, 18);
        addTriQuad(17, 20, 21, 19);
        addTriQuad(20, 22, 23, 21);

        // Set constraints for bottom portion to move when
        // left side expands
        addVConstraint(16, 17, 20, 22, 18, 19, 21, 23);

        // Set vertical constraints for upper-bottom portion
        // for when main inner area has to expand vertically
        addVConstraint(8, 9, 10, 11, 12, 13, 14, 15, 16);

        // Set horizontal contraints for right side of upper portion
        // for when main inner area has to expand horizontally
        addHConstraint(4, 6, 5, 7, 10, 11, 14, 15);
    }

    void doNineSliceBlock(
        float uvX, float uvY,
        float w, float h, 
        float lPad, float rPad, float tPad, float bPad,
        float meshSizeX = 2.0f)
    {
        blockWidth = w;
        blockHeight = h;

        this.meshSizeX = meshSizeX;
        meshSizeY = (blockHeight / blockWidth) * meshSizeX;

        float x = 0f;
        float y = 0f;

        addVertex(x, y, uvX, uvY);
        addVertex(x + lPad, y, uvX + lPad, uvY);
        addVertex((x + w) - rPad, y, (uvX + w) - rPad, uvY);
        addVertex((x + w), y, (uvX + w), uvY);

        addVertex(x, y + tPad, uvX, uvY - tPad);
        addVertex(x + lPad, y + tPad, uvX + lPad, uvY - tPad);
        addVertex((x + w) - rPad, y + tPad, (uvX + w) - rPad, uvY - tPad);
        addVertex((x + w), y + tPad, (uvX + w), uvY - tPad);

        addVertex(x, (y + h) - tPad, uvX, (uvY - h) + bPad);
        addVertex(x + lPad, (y + h) - tPad, uvX + lPad, (uvY - h) + bPad);
        addVertex((x + w) - rPad, (y + h) - tPad, (uvX + w) - rPad, (uvY - h) + bPad);
        addVertex((x + w), (y + h) - tPad, (uvX + w), (uvY - h) + bPad);

        addVertex(x, (y + h), uvX, (uvY - h));
        addVertex(x + lPad, (y + h), uvX + lPad, (uvY - h));
        addVertex((x + w) - rPad, (y + h), (uvX + w) - rPad, (uvY - h));
        addVertex((x + w), (y + h), (uvX + w), (uvY - h));

        //// upper
        addTriQuad(0, 1, 5, 4);
        addTriQuad(1, 2, 6, 5);
        addTriQuad(2, 3, 7, 6);

        //// middle
        addTriQuad(4, 5, 9, 8);
        addTriQuad(5, 6, 10, 9);
        addTriQuad(6, 7, 11, 10);

        //// bottom
        addTriQuad(8, 9, 13, 12);
        addTriQuad(9, 10, 14, 13);
        addTriQuad(10, 11, 15, 14);

        //// Set vertical constraints for bottom portion
        //// for when main inner area has to expand vertically
        addVConstraint(8, 9, 10, 11, 12, 13, 14, 15);

        //// Set horizontal contraints for right side of upper portion
        //// for when main inner area has to expand horizontally
        addHConstraint(2, 3, 6, 7, 10, 11, 14, 15);
    }


    void addVertex(float x, float y, float uvX, float uvY)
    {
        newVertices.Add(new Vector3((x / blockWidth) * meshSizeX, -(y / blockHeight) * meshSizeY, 0f));
        newUV.Add(new Vector2(uvX / texWidth, uvY / texHeight));
    }

    void addTriQuad(int tl, int tr, int br, int bl)
    {
        newTriangles.Add(tl);
        newTriangles.Add(tr);
        newTriangles.Add(bl);
        newTriangles.Add(tr);
        newTriangles.Add(br);
        newTriangles.Add(bl);
    }

    void addHConstraint(int vert, params int[] constrainedVerts)
    {
        foreach(int cv in constrainedVerts)
            addConstraint(hConstraints, vert, cv);
    }

    void addVConstraint(int vert, params int[] constrainedVerts)
    {
        foreach (int cv in constrainedVerts)
            addConstraint(vConstraints, vert, cv);
    }

    void addConstraint(Dictionary<int, List<int>> dict, int v, int cv)
    {
        List<int> vList;

        if (!dict.TryGetValue(v, out vList))
        {
            vList = new List<int>();
            dict.Add(v, vList);
        }

        vList.Add(cv);
    }

    void moveVert(int v, float x, float y)
    {
        Vector3 move = new Vector3(pixelToMeshX(x), pixelToMeshY(y), 0f);

        newVertices[v] += move;

        List<int> vList;

        if (hConstraints.TryGetValue(v, out vList))
        {
            foreach (int cv in vList)
                moveVert(cv, x, 0f);
        }

        if (vConstraints.TryGetValue(v, out vList))
        {
            foreach (int cv in vList)
                moveVert(cv, 0f, y);
        }
    }

    float pixelToMeshX(float x)
    {
        return (x / blockWidth) * meshSizeX;
    }

    float pixelToMeshY(float y)
    {
        return (-y / blockHeight) * meshSizeY;
    }

    float meshToPixelX(float x)
    {
        return (x * blockWidth / meshSizeX);
    }

    float meshToPixelY(float y)
    {
        return (-y * blockHeight / meshSizeY);
    }

    void setVertPosition(int v, float x, float y)
    {
        Vector3 move = newVertices[v] - new Vector3(pixelToMeshX(x), pixelToMeshY(y), 0f);
        moveVert(v, meshToPixelX(move.x), meshToPixelY(-move.y));
    }

    void refreshMesh()
    {
        mesh.vertices = newVertices.ToArray();
        //mesh.Optimize();
        //mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        _bc.size = mesh.bounds.size;
        _bc.center = mesh.bounds.center;

    }

    //void OnDrawGizmos()
    //{
    //    Vector3 curVert;
    //    int x, y;

    //    Vector3 lineOffset = new Vector3(0.002f, 0, 0);

    //    for (int i = 0; i < newVertices.Count; i++)
    //    {
    //        curVert = newVertices[i];
    //        x = (int)(curVert.x * blockWidth / meshSizeX + 0.5f);
    //        y = (int)(-curVert.y * blockHeight / meshSizeY + 0.5f);

    //        Handles.DrawLine(curVert - lineOffset, curVert + lineOffset);

    //        Handles.Label(curVert, i + ": (" + x + ", " + y+")");
    //    }
            

    //}

    //void OnMouseDrag()
    //{
    //    Debug.Log("MEW");
    //}
}
