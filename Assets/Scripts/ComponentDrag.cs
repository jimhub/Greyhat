using UnityEngine;
using System.Collections;

public class ComponentDrag : UIDragObject
{

    public ComponentCPU componentObject;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnHover(bool isOver)
    {
        componentObject.setAllPinsActive(isOver);
    }
}
