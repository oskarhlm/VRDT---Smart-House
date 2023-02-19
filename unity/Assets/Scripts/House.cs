using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class House : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            var child = gameObject.transform.GetChild(i).gameObject;
            //child.AddComponent<Rigidbody>();
            child.AddComponent<MeshCollider>();
            //child.layer = LayerMask.NameToLayer("House");
        }
    }
}
