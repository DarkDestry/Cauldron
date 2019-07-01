using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cauldron
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
	}
}
