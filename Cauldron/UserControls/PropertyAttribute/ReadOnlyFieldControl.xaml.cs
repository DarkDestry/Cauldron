﻿using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Cauldron.UserControls.PropertyAttribute
{
    /// <summary>
    /// Interaction logic for ReadOnlyFieldControl.xaml
    /// </summary>
    public partial class ReadOnlyFieldControl : UserControl
    {
        #region DependencyProperty
        public static readonly DependencyProperty NameLabelProperty = DependencyProperty.Register(
            "NameLabel",
            typeof(string),
            typeof(ReadOnlyFieldControl));
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value",
            typeof(string),
            typeof(ReadOnlyFieldControl));
        #endregion

        #region CLR Wrapper
        public string Value
        {
            get => (string) GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        public string NameLabel
        {
            get => (string) GetValue(NameLabelProperty);
            set => SetValue(NameLabelProperty, value);
        }
        #endregion 
        public ReadOnlyFieldControl()
        {
            InitializeComponent();
        }
    }
}
