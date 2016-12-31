﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace JaebeMusicStudio.UI
{
    /// <summary>
    /// Interaction logic for scroll.xaml
    /// </summary>
    public partial class Slider : UserControl
    {
        [Bindable(true)]
        public event RoutedPropertyChangedEventHandler<double> ValueChanged;
        public Slider()
        {
            InitializeComponent();
        }

        public double Value
        {
            get { return slider.Value; }
            set { slider.Value = value; }
        }

        private void Slider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ValueChanged?.Invoke(sender,e);
        }
    }
}
