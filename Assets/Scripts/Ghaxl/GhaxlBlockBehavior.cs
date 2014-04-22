using UnityEngine;
using System.Collections;

public class GhaxlBlockBehavior : MonoBehaviour
{
    public int heightModifier = -6;

    public UISprite mainSpr;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setHeight(int h)
    {
        
    }

    public void setWidth(int w)
    {

    }

    public virtual int getHeight()
    {
        return mainSpr.height + heightModifier;
    }
}
