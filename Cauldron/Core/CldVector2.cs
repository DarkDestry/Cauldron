using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cauldron.Core
{
    public struct CldVector2
    {
        public float x, y;

        public CldVector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public CldVector2(double[] vector)
        {
            if (vector.Length != 3) throw new Exception("Invalid array length in vector constructor");
            this.x = (float)vector[0];
            this.y = (float)vector[1];
        }

        public CldVector2(double[,] vector)
        {
            if (vector.GetLength(0) != 3 && vector.GetLength(1) != 1) throw new Exception("Invalid array length in vector constructor");
            this.x = (float)vector[0, 0];
            this.y = (float)vector[0, 1];
        }

        public static CldVector2 operator +(CldVector2 left, CldVector2 right)
        {
            return new CldVector2(left.x + right.x, left.y + right.y);
        }

        public static CldVector2 operator -(CldVector2 left, CldVector2 right)
        {
            return new CldVector2(left.x - right.x, left.y - right.y);
        }

        public static CldVector2 operator *(CldVector2 left, float scale)
        {
            return new CldVector2(left.x + scale, left.y + scale);
        }

        public static CldVector2 operator *(CldVector2 left, double scale)
        {
            float s = (float)scale;
            return new CldVector2(left.x + s, left.y + s);
        }

        public static explicit operator CldVector3(CldVector2 v)
        {
            return new CldVector3(v.x, v.y, 0);
        }
    }
}
