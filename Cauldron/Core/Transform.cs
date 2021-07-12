using GlmSharp;
using SharpGL.SceneGraph;

namespace Cauldron.Core
{
	public class Transform
	{
		public CldVector3 _position;
		public CldVector3 _rotation;
		public CldVector3 _scale;

		public Transform()
		{
			_position = new CldVector3(0,0,0);
			_rotation = new CldVector3(0,0,0);
			_scale = new CldVector3(1,1,1);
		}

        private mat4 matRot => mat4.RotateX(CldMath.DegToRad(_rotation.X)) * 
                               mat4.RotateY(CldMath.DegToRad(_rotation.Y)) * 
                               mat4.RotateZ(CldMath.DegToRad(_rotation.Z));
        private mat4 matTrans => mat4.Translate(_position);
        private mat4 matScale => mat4.Scale(_scale);
        public mat4 Matrix => matTrans * matRot * matScale;

        public CldVector3 Position
        {
            get => _position;
            set => _position = value;
        }

        public CldVector3 Rotation
        {
            get => _rotation;
            set => _rotation = value;
        }

        public CldVector3 Scale
        {
            get => _scale;
            set => _scale = value;
        }

        public override string ToString()
        {
            return $"Transform {{P[{Position}] R[{Rotation}] S[{Scale}]}}";
        }

        //        private double[,] matRot => CldMatrix.MatrixFromPitchYawRoll(Rotation.x, Rotation.y, Rotation.z);
//        private double[,] matTrans => new double[,] {{0,0,0,Position.x},
//                                                     {0,0,0,Position.y},
//                                                     {0,0,0,Position.z},
//                                                     {0,0,0,1}};
//        private double[,] matScale => new double[,] {{Scale.x,0,0,0},
//                                                     {0,Scale.y,0,0},
//                                                     {0,0,Scale.z,0},
//                                                     {0,0,0,1}};

//        public double[,] matrix => Matrix.Multiply(matTrans, Matrix.Multiply(matRot, matScale));
    }
}
