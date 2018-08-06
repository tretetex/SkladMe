using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace SkladMe.Infrastructure
{
    public static class AnimatableDoubleHelper
    {
        // Это attached property OriginalProperty. К нему мы будем привязывать свойство из VM,
        // и получать нотификацию об его изменении
        public static double GetOriginalProperty(DependencyObject obj) =>
            (double)obj.GetValue(OriginalPropertyProperty);
        public static void SetOriginalProperty(DependencyObject obj, double value) =>
            obj.SetValue(OriginalPropertyProperty, value);
        public static readonly DependencyProperty OriginalPropertyProperty =
            DependencyProperty.RegisterAttached(
                "OriginalProperty", typeof(double), typeof(AnimatableDoubleHelper),
                new PropertyMetadata(OnOriginalUpdated));

        // это "производное" attached property, которое будет
        // анимированно "догонять" OriginalProperty
        public static double GetAnimatedProperty(DependencyObject obj) =>
            (double)obj.GetValue(AnimatedPropertyProperty);
        public static void SetAnimatedProperty(DependencyObject obj, double value) =>
            obj.SetValue(AnimatedPropertyProperty, value);
        public static readonly DependencyProperty AnimatedPropertyProperty =
            DependencyProperty.RegisterAttached(
                "AnimatedProperty", typeof(double), typeof(AnimatableDoubleHelper));

        // это вызывается когда значение OriginalProperty меняется
        static void OnOriginalUpdated(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var newValue = (double)e.NewValue;
            var oldValue = (double)e.OldValue;
            // находим элемент, на котором меняется свойство
            var self = (FrameworkElement)o;
            var animation = // создаём анимацию...
                new DoubleAnimation(oldValue, newValue, new Duration(TimeSpan.FromSeconds(0.3)));
            // и запускаем её на AnimatedProperty
            self.BeginAnimation(AnimatedPropertyProperty, animation);
        }
    }
}
