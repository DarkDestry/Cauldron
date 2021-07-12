using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;
using Cauldron.Annotations;
using Cauldron.Core;
using Transform = Cauldron.Core.Transform;

namespace Cauldron.Core
{
	public static class Hierarchy
	{
        public static ObservableCollection<SceneObject> HierarchyObjectList { get; } =
            new ObservableCollection<SceneObject>();

        public delegate void FocusChangedEventHandler(SceneObject obj);

        public delegate void HierarchyUpdateEventHandler();

        public static event FocusChangedEventHandler FocusChangedEvent;

        public static event HierarchyUpdateEventHandler HierarchyUpdateEvent;

        public static Dictionary<string, string> guidColorDictionary = new Dictionary<string, string>();

		public class SceneObject : INotifyPropertyChanged
		{
			public SceneObject(string name)
            {
                var objectInfo = new ObjectInfo()
                {
                    Guid = System.Guid.NewGuid().ToString(),
                    Name = name
                };
                objectInfo.PropertyChanged += (sender, args) => OnPropertyChanged(args.PropertyName);
                properties.Add(objectInfo);
				properties.Add(new Transform());
            }

            public ObservableCollection<object> properties = new ObservableCollection<object>();

			public Transform Transform
            {
                get => properties.First(o => o is Transform) as Transform;
                set
                {
                    var trans = properties.First(o => o is Transform);
                    properties[properties.IndexOf(trans)] = value;
                }
            }

            public string Name
            {
                get => ((ObjectInfo)properties.First(o => o is ObjectInfo)).Name;
                set
                {
                    var objectInfo = (ObjectInfo)properties.First(o => o is ObjectInfo);
                    var index = properties.IndexOf(objectInfo);
                    objectInfo.Name = value;
                    properties[index] = objectInfo;
                    OnPropertyChanged();
                }
            }

            public string Guid => ((ObjectInfo)properties.First(o => o is ObjectInfo)).Guid;

            public MeshRenderer MeshRenderer
            {
                get => properties.First(o => o is MeshRenderer) as MeshRenderer;
                set
                {
                    var mr = properties.FirstOrDefault(o => o is MeshRenderer);
                    int index = properties.IndexOf(mr);
                    if (index > -1)
                        properties[index] = value;
                    else
                        properties.Add(value);
                }
            }

            public struct ObjectInfo : INotifyPropertyChanged
            {
                private string name;
                private string guid;

                public string Name
                {
                    get => name;
                    set
                    {
                        name = value;
                        OnPropertyChanged();
                    }
                }

                public string Guid
                {
                    get => guid;
                    set
                    {
                        guid = value;
                        OnPropertyChanged();
                    }
                }

                public event PropertyChangedEventHandler PropertyChanged;

                [NotifyPropertyChangedInvocator]
                private void OnPropertyChanged([CallerMemberName] string propertyName = null)
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            [NotifyPropertyChangedInvocator]
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

		public static void RemoveObject(string guid)
		{
			SceneObject objectToBeRemoved = null;

			foreach (var sceneObject in HierarchyObjectList)
			{
				if (sceneObject.Guid == guid) objectToBeRemoved = sceneObject;
			}

			if (objectToBeRemoved != null) HierarchyObjectList.Remove(objectToBeRemoved);
			else MessageBox.Show($"Attempted to remove object of GUID {guid} but object is not in hierarchy");
		}

		public static SceneObject GetObject(string guid)
		{
			foreach (var sceneObject in HierarchyObjectList)
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

        //public static string GetGuidFromColor(Color color)
        //{
        //    string colorString = color.PackedValue.ToString();
        //    if (guidColorDictionary.ContainsKey(colorString)) return guidColorDictionary[colorString];
        //    else return "";
        //}

        //public static Color GetColorFromGuid(string guid)
        //{
        //    if (!guidColorDictionary.ContainsKey(guid))
        //    {
        //        string colorString = "";
        //        while (colorString == "" || guidColorDictionary.ContainsKey(colorString))
        //        {
        //            SharpDX.Color color = new Random().NextColor();
        //            color.A = 255;
        //            colorString = new Color(color.R, color.G, color.B, color.A).PackedValue.ToString();
        //        }
                
        //        guidColorDictionary.Add(guid, colorString);
        //        guidColorDictionary.Add(colorString, guid);
        //    }
        //    return new Color(uint.Parse(guidColorDictionary[guid]));
        //}

        public static SceneObject selectedObject;
    }

    public enum Geometry
    {
        Sphere,
        Cube,
    }
}
