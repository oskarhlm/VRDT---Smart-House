using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

[ExecuteInEditMode]
public class TimeManager : Singleton<TimeManager>
{
    [SerializeField]
    public float Longitude = 10.50561f;

    [SerializeField]
    public float Latitude = 63.4027f;

    [SerializeField]
    [Range(0, 24)]
    public int Hour;

    [SerializeField]
    [Range(0, 60)]
    public int Minutes;

    public DateTime Date = DateTime.Today;

    [HideInInspector]
    public DateTime Time;

    [SerializeField]
    float TimeSpeed = 1;

    [SerializeField]
    private int FrameSteps = 1;

    [HideInInspector]
    public int FrameStep = 0;

    // Start is called before the first frame update
    void Start()
    {
        Time = DateTime.Now;
    }

    // Update is called once per frame
    void Update()
    {
        //Time = Time.AddSeconds(TimeSpeed * UnityEngine.Time.deltaTime);
        Time = Date + new TimeSpan(Hour, Minutes, 0);
        FrameStep = (FrameStep + 1) % FrameSteps;
    }
}
