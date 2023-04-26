using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SolarPanel : MonoBehaviour
{
    public void SetSnapPlane(GameObject gameObject)
    {
        var panelComponentTransform = transform.Find("Panel_50");
        if (panelComponentTransform is null)
        {
            Debug.LogError("Could not find panel component");
            return;
        }
        var snapPlane = panelComponentTransform.Find("SnapPlane").gameObject;
        
        //var roofCollider 
        //snapPlane.GetComponent<EdgeSnapping>().TargetCollider;
    }
}
