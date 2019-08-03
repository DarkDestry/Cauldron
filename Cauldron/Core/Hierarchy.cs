using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;
using Cauldron.Core;
using Microsoft.Xna.Framework;
using SharpDX;
using SharpDX.Direct2D1;
using Color = Microsoft.Xna.Framework.Color;
using Transform = Cauldron.Core.Transform;

namespace Cauldron
{
	public static class Hierarchy
	{
		public static List<SceneObject> hierarchyObjectList = new List<SceneObject>();

        public delegate void FocusChangedEventHandler(SceneObject obj);

        public delegate void HierarchyUpdateEventHandler();

        public static event FocusChangedEventHandler FocusChangedEvent;

        public static event HierarchyUpdateEventHandler HierarchyUpdateEvent;

        public static Dictionary<string, string> guidColorDictionary = new Dictionary<string, string>();

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

            public MeshRenderer MeshRenderer
            {
                get => properties["meshRenderer"] as MeshRenderer;
                set => properties["meshRenderer"] = value;
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

        private static void OnFocusChanged(SceneObject obj)
        {
            selectedObject = obj;
            FocusChangedEvent?.Invoke(obj);
        }

        public static void ChangeObjectFocus(SceneObject obj) => OnFocusChanged(obj);

        private static void OnHierarchyUpdate() => HierarchyUpdateEvent?.Invoke();

        public static void TriggerHierarchyUpdate() => OnHierarchyUpdate();

        public static string GetGuidFromColor(Color color)
        {
            string colorString = color.PackedValue.ToString();
            if (guidColorDictionary.ContainsKey(colorString)) return guidColorDictionary[colorString];
            else return "";
        }

        public static Color GetColorFromGuid(string guid)
        {
            if (!guidColorDictionary.ContainsKey(guid))
            {
                string colorString = "";
                while (colorString == "" || guidColorDictionary.ContainsKey(colorString))
                {
                    SharpDX.Color color = new Random().NextColor();
                    color.A = 255;
                    colorString = new Color(color.R, color.G, color.B, color.A).PackedValue.ToString();
                }
                
                guidColorDictionary.Add(guid, colorString);
                guidColorDictionary.Add(colorString, guid);
            }
            return new Color(uint.Parse(guidColorDictionary[guid]));
        }

        public static SceneObject selectedObject;
    }

    public enum Geometry
    {
        Sphere,
        Cube,
    }
}
