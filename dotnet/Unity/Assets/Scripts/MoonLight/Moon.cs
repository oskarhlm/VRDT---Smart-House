using System;
using UnityEngine;

namespace MoonLight
{
    [RequireComponent(typeof(Light))]
    [ExecuteInEditMode]
    public class Moon : MonoBehaviour
    {
        [SerializeField]
        float longitude = 10.50561f;

        [SerializeField]
        float latitude = 63.4027f;

        private int _hour;  //= TimeManager.Instance.Hour;
        private int _minutes; // = TimeManager.Instance.Minutes;

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
            this._hour = hour;
            this._minutes = minutes;
            OnValidate();
        }

        public void SetLocation(float longitude, float latitude)
        {
            this.longitude = longitude;
            this.latitude = latitude;
        }

        public void SetDate(DateTime dateTime)
        {
            _hour = dateTime.Hour;
            _minutes = dateTime.Minute;
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
            _hour = _time.Hour;
            _minutes = _time.Minute;
            date = _time.Date;
        }

        private void OnValidate()
        {
            _time = date + new TimeSpan(_hour, _minutes, 0);
            Debug.Log(_time);
        }

        private void Update()
        {
            _time = _time.AddSeconds(timeSpeed * Time.deltaTime);
            if (_frameStep == 0)
                SetPosition();
            _frameStep = (_frameStep + 1) % frameSteps;
        }

        void SetPosition()
        {
            Vector3 angles = new Vector3();
            double alt, azi;
            MoonPosition.CalculateMoonPosition(_time, latitude, longitude, out azi, out alt);
            angles.x = (float)alt * Mathf.Rad2Deg;
            angles.y = (float)azi * Mathf.Rad2Deg;
            Debug.Log("MOON:" + angles);
            transform.localRotation = Quaternion.Euler(angles);
            _light.intensity = Mathf.InverseLerp(-12, 0, angles.x);
        }
    }

    //SOURCE: Montenbruck, Pfleger: Astronomy on the Personal Computer
    //Valid between 1.march 1900 - 28 february 2100
    public static class MoonPosition
    {
        private const double Deg2Rad = Math.PI / 180.0;
        private const double Rad2Deg = 180.0 / Math.PI;

        public static void CalculateMoonPosition(
            DateTime dateTime, double latitude, double longitude, out double outAzimuth, out double outAltitude)
        {
            // Convert to UTC  
            dateTime = dateTime.ToUniversalTime();

            //**JULIAN STUFF**
            // Number of days from J2000.0.  
            double julianDate = 367 * dateTime.Year -
                (int)((7.0 / 4.0) * (dateTime.Year +
                (int)((dateTime.Month + 9.0) / 12.0))) +
                (int)((275.0 * dateTime.Month) / 9.0) +
                dateTime.Day - 730531.5;

            double julianCenturies = (julianDate) / 36525.0;
            //**JULIAN STUFF**

            //**SIDE REAL TIME**
            // Sidereal Time  
            double siderealTimeHours = 6.6974 + 2400.0513 * julianCenturies;

            double siderealTimeUT = siderealTimeHours +
                (366.2422 / 365.2422) * (double)dateTime.TimeOfDay.TotalHours;

            double siderealTime = siderealTimeUT * 15 + longitude;
            //**SIDE REAL TIME**

            // Refine to number of days (fractional) to specific time.  
            julianDate += (double)dateTime.TimeOfDay.TotalHours / 24.0;
            julianCenturies = julianDate / 36525.0;

            /* Higher accuracy but a lot more complicated 
            //**LUNAR - ECLIPTIC LONGITUDE AND ECLIPTIC LATITUDE**
             double meanLongitude =  CorrectAngle(Deg2Rad *(218.31617 + 481267.88088* julianCenturies - 4.06* julianCenturies* julianCenturies/3600.0));

             double moonMeanAnomaly = CorrectAngle(Deg2Rad * (134.96292 + 477198.86753*julianCenturies + 33.25*julianCenturies*julianCenturies/3600.0));

             double sunMeanAnomaly = CorrectAngle(Deg2Rad * (357.52543 + 35999.04944*julianCenturies - 0.58*julianCenturies*julianCenturies/3600.0));

            //the mean distance of the Moon from the ascending node
             double F = CorrectAngle(Deg2Rad * (93.27283 + 483202.01873*julianCenturies - 11.56*julianCenturies*julianCenturies/3600.0));

            //the difference between the mean longitudes of the Sun and the Moon
             double D = CorrectAngle(Deg2Rad * (297.85027 + 445267.11135*julianCenturies - 5.15*julianCenturies*julianCenturies/3600.0));

            //terms to account for pertrubations in the moons orbit
            double MajorInequality = CorrectAngle((22640/3600.0*Math.Sin(moonMeanAnomaly) + 769*Math.Sin(2*moonMeanAnomaly)));
            double Evection        = CorrectAngle((-4586/3600.0*Math.Sin(moonMeanAnomaly-2*D)));
            double Variation       = CorrectAngle((2370/3600.0*Math.Sin(2*D)));
            double AnnualInequality = CorrectAngle((-668/3600.0*Math.Sin(sunMeanAnomaly)));
            double ReductionToTheEcliptic = CorrectAngle((-412/3600.0*Math.Sin(2*F)));
            double ParallacticInequality = CorrectAngle((1/3600*(-125*Math.Sin(D)-
            212*Math.Sin(2*moonMeanAnomaly-2*D)
            -206*Math.Sin(moonMeanAnomaly+sunMeanAnomaly-2*D)
            +192*Math.Sin(moonMeanAnomaly+2*D)
            -165*Math.Sin(sunMeanAnomaly-2*D)
            +148*Math.Sin(moonMeanAnomaly-sunMeanAnomaly)
            -110*Math.Sin(moonMeanAnomaly+sunMeanAnomaly)
            -55*Math.Sin(2*F-2*D))));

             double deltaLambda = MajorInequality + Evection + Variation + AnnualInequality + ReductionToTheEcliptic + ParallacticInequality;
             double eclipticLongitude = CorrectAngle(meanLongitude + deltaLambda);
             N incorporates a small number of additional latitude variations which are caused by an oscillation of the inclination of the orbit 
             double N = 1/3600.0*(526*Math.Sin(F-2*D) + 44*Math.Sin(moonMeanAnomaly + F - 2*D)); //this series expansion is much longer....
             double eclipticLatitude  = CorrectAngle(18520/3600.0 *Math.Sin(F + deltaLambda + 412/3600.0*Math.Sin(2*F)+541/3600.0*Math.Sin(sunMeanAnomaly)) + N);
            */

            var meanAnomaly = CorrectAngle(Deg2Rad * (134.963 + 13.064993 * julianCenturies));
            var meanDistance = CorrectAngle(Deg2Rad * (93.272 + 13.229350 * julianCenturies));

            var eclipticLongitude = CorrectAngle((218.316 + 13.176396 * julianCenturies) * Deg2Rad * 6.289 * Math.Sin(meanAnomaly));
            var eclipticLatitude = CorrectAngle(Deg2Rad * 5.128 * Math.Sin(meanDistance));

            //TODO: account for variations in the moon distance 385001 - 20905 * Math.Cos(meanAnomaly)
            //**LUNAR - ECLIPTIC LONGITUDE AND ECLIPTIC LATITUDE**

            //**CONVERTING Ecliptic Longitude and Latitude to Right ascension and Declination**

            //obliquity of the earth (ish 23.4397)
            double obliquity = (23.439 - 0.013 * julianCenturies) * Deg2Rad;
            //**CONVERTING Ecliptic Longitude and Latitude to Right ascension and Declination**


            // Right Ascension  and declination 
            double rightAscension = Math.Atan2((Math.Sin(eclipticLongitude) * Math.Cos(obliquity) - Math.Tan(eclipticLatitude) * Math.Sin(obliquity)), Math.Cos(eclipticLongitude));
            double declination = Math.Asin(Math.Cos(obliquity) * Math.Sin(eclipticLatitude) + Math.Sin(obliquity) * Math.Cos(eclipticLatitude) * Math.Sin(eclipticLongitude));


            // Horizontal Coordinates  
            double hourAngle = CorrectAngle(siderealTime * Deg2Rad) - rightAscension;

            if (hourAngle > Math.PI)
                hourAngle -= 2 * Math.PI;

            double altitude = CorrectAngle(Math.Asin(Math.Sin(latitude * Deg2Rad) *
                Math.Sin(declination) + Math.Cos(latitude * Deg2Rad) *
                Math.Cos(declination) * Math.Cos(hourAngle)));

            // Nominator and denominator for calculating Azimuth  
            // angle. Needed to test which quadrant the angle is in.  
            double aziNom = -Math.Sin(hourAngle);
            double aziDenom =
                Math.Tan(declination) * Math.Cos(latitude * Deg2Rad) -
                Math.Sin(latitude * Deg2Rad) * Math.Cos(hourAngle);

            double azimuth = CorrectAngle(Math.Atan(aziNom / aziDenom));

            if (aziDenom < 0) // In 2nd or 3rd quadrant  
                azimuth += Math.PI;
            else if (aziNom < 0) // In 4th quadrant  
                azimuth += 2 * Math.PI;

            outAltitude = altitude - Math.PI; //To make the moon-rise at what we have defined as the east side and set at what we have defined as west side
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
