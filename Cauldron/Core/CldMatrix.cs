using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;

namespace Cauldron.Core
{
    public static class CldMatrix
    {
        public static double[,] MatrixFromPitchYawRoll(double pitch, double yaw, double roll)
        {
            return new double[,] {
            { Cos(pitch) * Cos(yaw)   , Cos(pitch) * Sin(yaw) * Sin(roll) - Sin(pitch) * Cos(roll)   , Cos(pitch) * Sin(yaw) * Cos(roll) + Sin(pitch) * Sin(roll) ,0 },
            { Sin(pitch) * Cos(yaw)   , Sin(pitch) * Sin(yaw) * Sin(roll) - Cos(pitch) * Cos(roll)   , Sin(pitch) * Sin(yaw) * Cos(roll) + Cos(pitch) * Sin(roll) ,0 },
            { -Sin(yaw)               , Cos(yaw) * Sin(roll)                                         , Cos(yaw) * Cos(roll)                                       ,0 },
            {0                        , 0                                                            , 0                                                          ,1 }
            };
        }
    }
}
