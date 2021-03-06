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
using JaebeMusicStudio.Sound;

namespace JaebeMusicStudio.Widgets
{
    /// <summary>
    /// Interaction logic for NonlinearDistortionUI.xaml
    /// </summary>
    public partial class NonlinearDistortionUI : Page
    {
        private NonlinearDistortion Effect;
        public NonlinearDistortionUI(NonlinearDistortion effect)
        {
            Effect = effect;
            InitializeComponent();

            PowerExponentiation.Value = effect.PowerExponentiation;
        }

        private void PowerExponentiation_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Effect.PowerExponentiation = (float)PowerExponentiation.Value;
        }

        private void LimiterThreshold_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void power_Checked(object sender, RoutedEventArgs e)
        {
            Effect.EffectType = NonlinearDistortionType.Power;
        }

        private void arctan_Checked(object sender, RoutedEventArgs e)
        {
            Effect.EffectType = NonlinearDistortionType.ArcTan;
        }
    }
}
