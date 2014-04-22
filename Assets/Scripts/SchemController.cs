using UnityEngine;
using System.Collections;

public class SchemController : MonoBehaviour
{

    public static GameObject pinPrefab;
    public static Vector2 screenHalf;

    public GameObject pinPrefabObject;

    static SchemController()
    {
        
    }

    // Use this for initialization
    void Awake()
    {
        pinPrefab = pinPrefabObject;

        screenHalf = new Vector2(Screen.width / 2, Screen.height / 2);
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
