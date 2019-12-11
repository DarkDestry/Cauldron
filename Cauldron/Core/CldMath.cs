using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cauldron.Core
{
    public static class CldMath
    {
        public static double DegToRad(double d) => d * Math.PI / 180;
        public static float DegToRad(float d) => (float) (d * Math.PI / 180);
        public static double RadToDeg(double r) => r * 180 / Math.PI;
        public static float RadToDeg(float r) => (float) (r * 180 / Math.PI);
        public static double Clamp(double val, double min, double max) => Math.Min(Math.Max(val, min), max);
        public static float Clamp(float val, float min, float max) => Math.Min(Math.Max(val, min), max);
    }
}
