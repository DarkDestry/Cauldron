using System;
using SharpDX;
using Quaternion = Microsoft.Xna.Framework.Quaternion;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace Cauldron
{
	public struct CldVector3
    {
		public float x, y, z;

		public CldVector3(float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public CldVector3(float x, float y)
		{
			this.x = x;
			this.y = y;
			z = 0;
		}

        public static CldVector3 operator + (CldVector3 left, CldVector3 right)
        {
            return new CldVector3(left.x + right.x, left.y + right.y, left.z + right.z);
        }

        public static implicit operator Microsoft.Xna.Framework.Vector3(CldVector3 val)
        {
            return new Microsoft.Xna.Framework.Vector3(val.x, val.y, val.z);
        }

        public static implicit operator CldVector3(Vector3 val)
        {
            return new CldVector3(val.X, val.Y, val.Z);
        }

        public static implicit operator Quaternion(CldVector3 val)
        {
            return toQuaternion(val.x,val.y, val.z);
        }

        private static Quaternion toQuaternion(double yaw, double pitch, double roll) // yaw (Z), pitch (Y), roll (X)
        {
            // Abbreviations for the various angular functions
            float cy = (float) Math.Cos(yaw * 0.5);
            float sy = (float) Math.Sin(yaw * 0.5);
            float cp = (float) Math.Cos(pitch * 0.5);
            float sp = (float) Math.Sin(pitch * 0.5);
            float cr = (float) Math.Cos(roll * 0.5);
            float sr = (float) Math.Sin(roll * 0.5);

            float w = cy * cp * cr + sy * sp * sr;
            float x = cy * cp * sr - sy * sp * cr;
            float y = sy * cp * sr + cy * sp * cr;
            float z = sy * cp * cr - cy * sp * sr;
            return new Quaternion(x,y,z,w);
        }
    }
}
