using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MoonLight
{
    public class SetMoonDate : MonoBehaviour
    {
        public Moon moon;
        public int year = DateTime.Now.Year;

        [Range(1, 12)]
        public int month = DateTime.Now.Month;
        [Range(1, 31)]
        public int day = DateTime.Now.Day;

        private void Start()
        {
            this.year = DateTime.Now.Year;
            this.month = DateTime.Now.Month;
            this.day = DateTime.Now.Day;

            DateTime d = new DateTime(year, month, day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
            if (moon) moon.SetDate(d);
        }
        private void OnValidate()
        {
            try
            {
                DateTime d = new DateTime(year, month, day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
                Debug.Log(d);
                if (moon) moon.SetDate(d);
            }
            catch (System.ArgumentOutOfRangeException e)
            {
                Debug.LogWarning("bad date");
            }
        }
    }
}
