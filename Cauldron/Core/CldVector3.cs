using System;
using System.Runtime.CompilerServices;
using System.Windows;
using Cauldron.Core;
using GlmSharp;
using SharpGL.SceneGraph;

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

        public CldVector3(double[] vector)
        {
            if(vector.Length != 4) throw new Exception("Invalid array length in vector constructor");
            this.x = (float) vector[0];
            this.y = (float) vector[1];
            this.z = (float) vector[2];
        }

        public CldVector3(double[,] vector)
        {
            if (vector.GetLength(0) != 4 && vector.GetLength(1) != 1) throw new Exception("Invalid array length in vector constructor");
            this.x = (float)vector[0,0];
            this.y = (float)vector[1,0];
            this.z = (float)vector[2,0];
        }

        public static CldVector3 operator + (CldVector3 left, CldVector3 right)
        {
            return new CldVector3(left.x + right.x, left.y + right.y, left.z + right.z);
        }

        public static CldVector3 operator -(CldVector3 left, CldVector3 right)
        {
            return new CldVector3(left.x - right.x, left.y - right.y, left.z - right.z);
        }

        public static CldVector3 operator *(CldVector3 left, float scale)
        {
            return new CldVector3(left.x + scale, left.y + scale, left.z + scale);
        }

        public static CldVector3 operator *(CldVector3 left, double scale)
        {
            float s = (float) scale;
            return new CldVector3(left.x + s, left.y + s, left.z + s);
        }

        public static implicit operator double[](CldVector3 v)
        {
            return new double[] {v.x, v.y, v.z, 1};
        }

        public static implicit operator double[,](CldVector3 v)
        {
            return new double[,] {{v.x}, {v.y}, {v.z}, {1}};
        }

        public static implicit operator vec3(CldVector3 v) => new vec3(v.x, v.y, v.z);
        public static implicit operator vec4(CldVector3 v) => new vec4(v.x, v.y, v.z, 0);
        public static implicit operator CldVector3(vec3 v) => new CldVector3(v.x, v.y, v.z);
        public static implicit operator CldVector3(vec4 v) => new CldVector3(v.x, v.y, v.z);

        public static explicit operator GLColor (CldVector3 v)
        {
            return new GLColor(v.x, v.y, v.z, 1);
        }

        public static explicit operator CldVector3(GLColor c)
        {
            return new CldVector3(c.R, c.G, c.B);
        }

        public static explicit operator CldVector2(CldVector3 v)
        {
            return new CldVector2(v.x, v.y);
        }


        //        private static Quaternion toQuaternion(double yaw, double pitch, double roll) // yaw (Z), pitch (Y), roll (X)
        //        {
        //            // Abbreviations for the various angular functions
        //            float cy = (float) Math.Cos(yaw * 0.5);
        //            float sy = (float) Math.Sin(yaw * 0.5);
        //            float cp = (float) Math.Cos(pitch * 0.5);
        //            float sp = (float) Math.Sin(pitch * 0.5);
        //            float cr = (float) Math.Cos(roll * 0.5);
        //            float sr = (float) Math.Sin(roll * 0.5);
        //
        //            float w = cy * cp * cr + sy * sp * sr;
        //            float x = cy * cp * sr - sy * sp * cr;
        //            float y = sy * cp * sr + cy * sp * cr;
        //            float z = sy * cp * cr - cy * sp * sr;
        //            return new Quaternion(x,y,z,w);
        //        }

        public static readonly CldVector3 Forward = new CldVector3(0,0,1);
        public static readonly CldVector3 Right = new CldVector3(1,0,0);
        public static readonly CldVector3 Up = new CldVector3(0,1,0);

        public static CldVector3 Transform(CldVector3 vector ,double[,] matrix)
        {
            return new CldVector3(Matrix.Multiply(matrix, vector));
        }

        public CldVector3 Transform(double[,] matrix)
        {
            return new CldVector3(Matrix.Multiply(matrix, this));
        }
    }
}
