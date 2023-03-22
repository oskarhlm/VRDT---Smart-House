using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class EdgeSnapping : MonoBehaviour
{
    [SerializeField] private Collider targetCollider;
    private Collider _myCollider;
    private XRGrabInteractable _grabInteractable;
    public float SnapDistance = 1;
    [SerializeField] private bool hasSnapped = false;

    private void Start()
    {
        _myCollider = GetComponent<BoxCollider>();
        _grabInteractable = GetComponent<XRGrabInteractable>();
    }

    private void FixedUpdate()
    {
        var myClosestPoint = _myCollider.ClosestPoint(targetCollider.transform.position);
        var targetClosestPoint = targetCollider.ClosestPoint(myClosestPoint);
        var offset = targetClosestPoint - myClosestPoint;
        if (offset.magnitude < SnapDistance)
        {
            _grabInteractable.trackRotation = false;
            _grabInteractable.trackPosition = false;

            if (hasSnapped)
            {
                _grabInteractable.trackPosition = true;
                var rBody = GetComponent<Rigidbody>();
                rBody.constraints = RigidbodyConstraints.FreezePositionY;
            }
            else
            {
                transform.eulerAngles = targetCollider.transform.eulerAngles;
                myClosestPoint = _myCollider.ClosestPoint(targetCollider.transform.position);
                targetClosestPoint = targetCollider.ClosestPoint(myClosestPoint);
                offset = targetClosestPoint - myClosestPoint;
                transform.position += offset;
            }

            hasSnapped = true;
        }
    }
}
