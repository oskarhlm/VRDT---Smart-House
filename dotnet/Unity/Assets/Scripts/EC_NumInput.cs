using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EC_NumInput : MonoBehaviour
{
    private int input;

    public void ReadNumInput(string num)
    {
        input = int.Parse(num);
    }

    public int GetNumInput(){
        return input;
    }
}
