using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//SOURCE OF THE SCRIPT: 

//Script to simulate day-night cycle based on longitude, latitude (Sæterbakken 7057 - Jonsvatnet)
//Script updates date based on the current date
// http://guideving.blogspot.co.uk/2010/08/sun-position-in-c.html <-- ORIGINAL SOURCE
// http://www.astro.uio.no/~bgranslo/aares/calculate.html <-- THIS SIMULATION OF SUNRISE/SUNSET IS ONLY CORRECT FOR THE REGION OF March 1 1900 to February 28 2100


namespace SunLight
{

    [RequireComponent(typeof(Light))]
    [ExecuteInEditMode]
    public class Sun : MonoBehaviour
    {
        [SerializeField]
        float longitude = 10.50561f;

        [SerializeField]
        float latitude = 63.4027f;

        [SerializeField]
        [Range(0, 24)]
        int hour;

        [SerializeField]
        [Range(0, 60)]
        int minutes;

        private DateTime _time;
        private Light _light;

        [SerializeField]
        float timeSpeed = 1;

        [SerializeField]
        int frameSteps = 1;
        private int _frameStep;

        [SerializeField]
        DateTime date;

        public void SetTime(int hour, int minutes)
        {
            this.hour = hour;
            this.minutes = minutes;
            OnValidate();
        }

        public void SetLocation(float longitude, float latitude)
        {
            this.longitude = longitude;
            this.latitude = latitude;
        }

        public void SetDate(DateTime dateTime)
        {
            hour = dateTime.Hour;
            minutes = dateTime.Minute;
            date = dateTime.Date;
            OnValidate();
        }

        public void SetUpdateSteps(int i) =>
            frameSteps = i;

        public void SetTimeSpeed(float speed) =>
            timeSpeed = speed;


        private void Awake()
        {
            _light = GetComponent<Light>();
            _time = DateTime.Now;
            hour = _time.Hour;
            minutes = _time.Minute;
            date = _time.Date;
        }

        private void OnValidate()
        {
            _time = date + new TimeSpan(hour, minutes, 0);
            Debug.Log(_time);
        }

        private void Update()
        {
            _time = _time.AddSeconds(timeSpeed * Time.deltaTime);
            if (_frameStep == 0)
            {
                //Awake(); //Updates time automatically - Comment out to work with hypothetical time
                SetPosition();
            }
            _frameStep = (_frameStep + 1) % frameSteps;
        }

        void SetPosition()
        { 
            var angles = new Vector3();
            double alt, azi;
            SunPosition.CalculateSunPosition(_time, latitude, longitude, out azi, out alt);
            angles.x = (float)alt * Mathf.Rad2Deg;
            angles.y = (float)azi * Mathf.Rad2Deg;
            //UnityEngine.Debug.Log("SUN:" + angles);
            transform.localRotation = Quaternion.Euler(angles);
            _light.intensity = Mathf.InverseLerp(-12, 0, angles.x);
        }
    }

    /*
     * The following source came from this blog:
     * http://guideving.blogspot.co.uk/2010/08/sun-position-in-c.html
     */
    public static class SunPosition
    {
        private const double Deg2Rad = Math.PI / 180.0;
        private const double Rad2Deg = 180.0 / Math.PI;

        /*! 
         * \brief Calculates the sun light. 
         * 
         * CalcSunPosition calculates the suns "position" based on a 
         * given date and time in local time, latitude and longitude 
         * expressed in decimal degrees. It is based on the method 
         * found here: 
         * http://www.astro.uio.no/~bgranslo/aares/calculate.html 
         * The calculation is only satisfiably correct for dates in 
         * the range March 1 1900 to February 28 2100. 
         * \param dateTime Time and date in local time. 
         * \param latitude Latitude expressed in decimal degrees. 
         * \param longitude Longitude expressed in decimal degrees. 
         */
        public static void CalculateSunPosition(
            DateTime dateTime, double latitude, double longitude, out double outAzimuth, out double outAltitude)
        {
            // Convert to UTC  
            dateTime = dateTime.ToUniversalTime();

            // Number of days from J2000.0.  
            double julianDate = 367 * dateTime.Year -
                (int)((7.0 / 4.0) * (dateTime.Year +
                (int)((dateTime.Month + 9.0) / 12.0))) +
                (int)((275.0 * dateTime.Month) / 9.0) +
                dateTime.Day - 730531.5;

            double julianCenturies = julianDate / 36525.0;

            // Sidereal Time  
            double siderealTimeHours = 6.6974 + 2400.0513 * julianCenturies;

            double siderealTimeUT = siderealTimeHours +
                (366.2422 / 365.2422) * (double)dateTime.TimeOfDay.TotalHours;

            double siderealTime = siderealTimeUT * 15 + longitude;

            // Refine to number of days (fractional) to specific time.  
            julianDate += (double)dateTime.TimeOfDay.TotalHours / 24.0;
            julianCenturies = julianDate / 36525.0;

            // Solar Coordinates  
            double meanLongitude = CorrectAngle(Deg2Rad *
                (280.466 + 36000.77 * julianCenturies));

            double meanAnomaly = CorrectAngle(Deg2Rad *
                (357.529 + 35999.05 * julianCenturies));

            double equationOfCenter = Deg2Rad * ((1.915 - 0.005 * julianCenturies) *
                Math.Sin(meanAnomaly) + 0.02 * Math.Sin(2 * meanAnomaly));

            double elipticalLongitude =
                CorrectAngle(meanLongitude + equationOfCenter);

            double obliquity = (23.439 - 0.013 * julianCenturies) * Deg2Rad;

            // Right Ascension  
            double rightAscension = Math.Atan2(
                Math.Cos(obliquity) * Math.Sin(elipticalLongitude),
                Math.Cos(elipticalLongitude));

            double declination = Math.Asin(
                Math.Sin(rightAscension) * Math.Sin(obliquity));

            // Horizontal Coordinates  
            double hourAngle = CorrectAngle(siderealTime * Deg2Rad) - rightAscension;

            if (hourAngle > Math.PI)
            {
                hourAngle -= 2 * Math.PI;
            }

            double altitude = Math.Asin(Math.Sin(latitude * Deg2Rad) *
                Math.Sin(declination) + Math.Cos(latitude * Deg2Rad) *
                Math.Cos(declination) * Math.Cos(hourAngle));

            // Nominator and denominator for calculating Azimuth  
            // angle. Needed to test which quadrant the angle is in.  
            double aziNom = -Math.Sin(hourAngle);
            double aziDenom =
                Math.Tan(declination) * Math.Cos(latitude * Deg2Rad) -
                Math.Sin(latitude * Deg2Rad) * Math.Cos(hourAngle);

            double azimuth = Math.Atan(aziNom / aziDenom);

            if (aziDenom < 0) // In 2nd or 3rd quadrant  
            {
                azimuth += Math.PI;
            }
            else if (aziNom < 0) // In 4th quadrant  
            {
                azimuth += 2 * Math.PI;
            }

            outAltitude = altitude;
            outAzimuth = azimuth;
        }

        /*! 
        * \brief Corrects an angle. 
        * 
        * \param angleInRadians An angle expressed in radians. 
        * \return An angle in the range 0 to 2*PI. 
        */
        private static double CorrectAngle(double angleInRadians)
        {
            if (angleInRadians < 0)
            {
                return 2 * Math.PI - (Math.Abs(angleInRadians) % (2 * Math.PI));
            }
            else if (angleInRadians > 2 * Math.PI)
            {
                return angleInRadians % (2 * Math.PI);
            }
            else
            {
                return angleInRadians;
            }
        }
    }

}