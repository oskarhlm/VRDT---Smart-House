using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunLight
{
    public class SetDate : MonoBehaviour
    {
        public Sun sun;
        public int year = DateTime.Now.Year;

        [Range(1, 12)]
        public int month = DateTime.Now.Month;
        
        [Range(1, 31)]
        public int day = DateTime.Now.Day;

        private void Start()
        {
            year = DateTime.Now.Year;
            month = DateTime.Now.Month;
            day = DateTime.Now.Day;

            var d = new DateTime(year, month, day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
            if (sun) sun.SetDate(d);
        }
        private void OnValidate()
        {
            try
            {
                var d = new DateTime(year, month, day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
                Debug.Log(d);
                if (sun) 
                    sun.SetDate(d);
            }
            catch (System.ArgumentOutOfRangeException e)
            {
                Debug.LogWarning("bad date");
            }
        }
    }
}
