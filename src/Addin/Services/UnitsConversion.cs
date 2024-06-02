using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autojenzi.src.Addin.Services
{
    internal class UnitsConversion
    {
        private const double _inchToMm = 25.4;
        private const double _footToMm = 12 * _inchToMm;
        private const double _footToMeter = _footToMm * 0.001;
        private const double _sqfToSqm = _footToMeter * _footToMeter;
        private const double _cubicFootToCubicMeter = _footToMeter * _sqfToSqm;


        /// <summary>
        ///     Convert a given length in feet to millimetres.
        /// </summary>
        public static double FootToMm(double length)
        {
            return length * _footToMm;
        }

        /// <summary>
        ///     Convert a given length in feet to millimetres,
        ///     rounded to the closest millimetre.
        /// </summary>
        public static int FootToMmInt(double length)
        {
            //return (int) ( _feet_to_mm * d + 0.5 );
            return (int)Math.Round(_footToMm * length,
                MidpointRounding.AwayFromZero);
        }

        /// <summary>
        ///     Convert a given length in feet to metres.
        /// </summary>
        public static double FootToMetre(double length)
        {
            return length * _footToMeter;
        }

        /// <summary>
        ///     Convert a given length in millimetres to feet.
        /// </summary>
        public static double MmToFoot(double length)
        {
            return length / _footToMm;
        }

        /// <summary>
        ///     Convert a given point or vector from millimetres to feet.
        /// </summary>
        public static XYZ MmToFoot(XYZ v)
        {
            return v.Divide(_footToMm);
        }

        /// <summary>
        ///     Convert a given volume in feet to cubic meters.
        /// </summary>
        public static double CubicFootToCubicMeter(double volume)
        {
            return volume * _cubicFootToCubicMeter;
        }

        public static double gramsToKg(double weight)
        {
            return (weight / 1000);
        }
    }
}

