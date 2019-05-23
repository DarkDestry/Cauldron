using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Cauldron
{
	public static class Hierarchy
	{
		public static List<SceneObject> hierarchyObjectList = new List<SceneObject>();



		public class SceneObject
		{
			public SceneObject(string name)
			{
				properties.Add("name", name);
				properties.Add("transform", new Transform());
				properties.Add("guid", System.Guid.NewGuid().ToString());
			}

			public Dictionary<string,object> properties = new Dictionary<string, object>();

			public Transform Transform
			{
				get => properties["transform"] as Transform;
				set => properties["transform"] = value;
			}

			public string Name
			{
				get => properties["name"] as string;
				set => properties["name"] = value;
			}
			public string Guid
			{
				get => properties["guid"] as string;
				set => properties["guid"] = value;
			}
		}

		public static void RemoveObject(string guid)
		{
			SceneObject objectToBeRemoved = null;

			foreach (var sceneObject in hierarchyObjectList)
			{
				if (sceneObject.Guid == guid) objectToBeRemoved = sceneObject;
			}

			if (objectToBeRemoved != null) hierarchyObjectList.Remove(objectToBeRemoved);
			else MessageBox.Show($"Attempted to remove object of GUID {guid} but object is not in hierarchy");
		}

		public static SceneObject GetObject(string guid)
		{
			foreach (var sceneObject in hierarchyObjectList)
			{
				if (sceneObject.Guid == guid) return sceneObject;
			}

			MessageBox.Show($"Attempted to obtain object of GUID {guid} but object is not in hierarchy");
			return null;
		}
	}
}
