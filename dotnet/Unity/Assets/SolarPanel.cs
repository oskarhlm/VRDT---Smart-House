using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class SolarPanel : MonoBehaviour
{
    public void SetSnapPlane(BoxCollider collider)
    {
        var panelComponentTransform = transform.Find("Panel_50");
        var snapPlane = panelComponentTransform.GetComponentInChildren<EdgeSnapping>();

        snapPlane.TargetCollider = collider;
    }

    public (float width, float length) GetWidthAndHeight()
    {
        Renderer renderer = gameObject.GetComponentInChildren<Renderer>();
        Bounds bounds = renderer.bounds;

        float width = bounds.size.x;
        float length = bounds.size.z;
        
        return (width, length);
    }
}
