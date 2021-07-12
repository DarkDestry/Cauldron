using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Cauldron.Annotations;
using Cauldron.Core;
using Cauldron.CustomControls;

namespace Cauldron.UserControls.Windows
{
    /// <summary>
    /// Interaction logic for PropertyWindow.xaml
    /// </summary>
    public partial class PropertyWindow : UserControl, INotifyPropertyChanged
    {
        private Hierarchy.SceneObject _selectedObject;
        public ObservableCollection<object> SelectedObjectProperties
        {
            get => _selectedObject?.properties;
        }

        public PropertyWindow()
        {
            InitializeComponent();
            Hierarchy.FocusChangedEvent += o =>
            {
                _selectedObject = o;
                OnPropertyChanged(nameof(SelectedObjectProperties));
            };
            // Hierarchy.FocusChangedEvent += Hierarchy_FocusChangedEvent;
        }

        // private void Hierarchy_FocusChangedEvent(Hierarchy.SceneObject obj)
        // {
        //     PropertyList.Items.Clear();
        //
        //     if (obj.properties.ContainsKey("name"))
        //     {
        //         PropertyList.Items.Add(new Control_Property_Object());
        //     }
        //
        //     if (obj.properties.ContainsKey("transform"))
        //     {
        //         PropertyList.Items.Add(new Control_Property_Transform());
        //     }
        //
        //     if (obj.properties.ContainsKey("meshRenderer"))
        //     {
        //         PropertyList.Items.Add(new Control_Property_MeshRenderer());
        //     }
        //
        //     foreach (var propertyListChild in PropertyList.Items)
        //     {
        //         IProperty iProperty = propertyListChild as IProperty;
        //         iProperty.UpdateProperty(obj);
        //     }
        // }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
