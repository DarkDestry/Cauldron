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
        public static double RadToDeg(double r) => r * 180 / Math.PI;
    }
}
