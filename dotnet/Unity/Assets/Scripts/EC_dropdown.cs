using GrpcBase;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EC_dropdown : MonoBehaviour
{
    public TibberMessages.Types.TimeResolution dd_output;
    // Start is called before the first frame update
    public void HandleInputDD(int val)
    {
        Debug.Log(val);
        dd_output = (TibberMessages.Types.TimeResolution) Enum.Parse(typeof(TibberMessages.Types.TimeResolution), val.ToString());
        Debug.Log(val);
    }

    public TibberMessages.Types.TimeResolution GetDDVal(){
        return dd_output;
    }
}
