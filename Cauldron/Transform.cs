using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cauldron
{
	public class Transform
	{
		public Vector3 Position;
		public Vector3 Rotation;
		public Vector3 Scale;

		public Transform()
		{
			Position = new Vector3(0,0,0);
			Rotation = new Vector3(0,0,0);
			Scale = new Vector3(0,0,0);
		}
	}
}
