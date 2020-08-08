using GlmSharp;
using SharpGL.SceneGraph;

namespace Cauldron.Core
{
	public class Transform
	{
		public CldVector3 Position;
		public CldVector3 Rotation;
		public CldVector3 Scale;

		public Transform()
		{
			Position = new CldVector3(0,0,0);
			Rotation = new CldVector3(0,0,0);
			Scale = new CldVector3(1,1,1);
		}

        private mat4 matRot => mat4.RotateX(CldMath.DegToRad(Rotation.x)) * 
                               mat4.RotateY(CldMath.DegToRad(Rotation.y)) * 
                               mat4.RotateZ(CldMath.DegToRad(Rotation.z));
        private mat4 matTrans => mat4.Translate(Position);
        private mat4 matScale => mat4.Scale(Scale);
        public mat4 Matrix => matTrans * matRot * matScale;

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
