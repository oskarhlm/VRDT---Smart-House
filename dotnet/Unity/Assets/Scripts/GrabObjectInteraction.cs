using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class GrabObjectInteraction : MonoBehaviour
{
    public void HoverOver()
    {
        transform.position += new Vector3(0, 0.1f, 0);
    }

    public void HoverExit()
    {
        transform.position -= new Vector3(0, 0.1f, 0);
    }
}
