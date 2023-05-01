using GrpcBase;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GrpcBase.TibberMessages.Types;


public class EC_dropdown : MonoBehaviour
{
    public TimeResolution dd_output;

    public void HandleInputDD(int val)
    {
        dd_output = (TimeResolution) Enum.Parse(typeof(TimeResolution), val.ToString());
    }

    public TimeResolution GetDDVal()
    {
        return dd_output;
    }
}
