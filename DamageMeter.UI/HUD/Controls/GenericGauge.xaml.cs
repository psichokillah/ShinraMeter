﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DamageMeter.UI.HUD.Controls
{
    public partial class GenericGauge : INotifyPropertyChanged
    {
        public static readonly DependencyProperty BarColorProperty =
            DependencyProperty.Register("BarColor", typeof(SolidColorBrush), typeof(GenericGauge));

        public static readonly DependencyProperty GaugeNameProperty =
            DependencyProperty.Register("GaugeName", typeof(string), typeof(GenericGauge));

        public static readonly DependencyProperty MaxValProperty =
            DependencyProperty.Register("MaxVal", typeof(int), typeof(GenericGauge));

        public static readonly DependencyProperty CurrentValProperty =
            DependencyProperty.Register("CurrentVal", typeof(float), typeof(GenericGauge));

        public static readonly DependencyProperty ShowPercentageProperty =
            DependencyProperty.Register("ShowPercentage", typeof(bool), typeof(GenericGauge));

        public static readonly DependencyProperty ShowValuesProperty =
            DependencyProperty.Register("ShowValues", typeof(bool), typeof(GenericGauge));

        public static readonly DependencyProperty ShowNameProperty =
            DependencyProperty.Register("ShowName", typeof(bool), typeof(GenericGauge));

        private readonly DependencyPropertyWatcher<float> _curValwatcher
            ; //https://blogs.msdn.microsoft.com/flaviencharlon/2012/12/07/getting-change-notifications-from-any-dependency-property-in-windows-store-apps/

        private readonly DoubleAnimation a;

        private readonly int animTime = 200;

        private double _factor;

        public GenericGauge()
        {
            InitializeComponent();

            _curValwatcher = new DependencyPropertyWatcher<float>(this, "CurrentVal");
            _curValwatcher.PropertyChanged += CurValWatcher_PropertyChanged;
            var maxValwatcher = new DependencyPropertyWatcher<float>(this, "MaxVal");
            maxValwatcher.PropertyChanged += CurValWatcher_PropertyChanged;
            a = new DoubleAnimation(0, TimeSpan.FromMilliseconds(animTime)) {EasingFunction = new QuadraticEase()};
            bar.RenderTransform = new ScaleTransform(1, 1, 0, .5);
        }

        public double Factor
        {
            get => _factor;
            set
            {
                if (Math.Abs(_factor - value) < 0.0001) return;
                _factor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Factor"));
                AnimateBar(value);
            }
        }

        public SolidColorBrush BarColor
        {
            get => (SolidColorBrush) GetValue(BarColorProperty);
            set => SetValue(BarColorProperty, value);
        }

        public string GaugeName
        {
            get => (string) GetValue(GaugeNameProperty);
            set => SetValue(GaugeNameProperty, value);
        }

        public int MaxVal
        {
            get => (int) GetValue(MaxValProperty);
            set => SetValue(MaxValProperty, value);
        }

        public float CurrentVal
        {
            get => (float) GetValue(CurrentValProperty);
            set => SetValue(CurrentValProperty, value);
        }

        public bool ShowPercentage
        {
            get => (bool) GetValue(ShowPercentageProperty);
            set => SetValue(ShowPercentageProperty, value);
        }

        public bool ShowValues
        {
            get => (bool) GetValue(ShowValuesProperty);
            set => SetValue(ShowValuesProperty, value);
        }

        public bool ShowName
        {
            get => (bool) GetValue(ShowNameProperty);
            set => SetValue(ShowNameProperty, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void CurValWatcher_PropertyChanged(object sender, EventArgs e)
        {
            if (MaxVal > 0)
                Factor = _curValwatcher.Value / MaxVal;
            else
                Factor = 0;
        }

        private void AnimateBar(double factor)
        {
            a.To = factor > 1 ? 1 : factor;

            bar.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, a);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}