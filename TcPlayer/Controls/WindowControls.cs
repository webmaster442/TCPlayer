using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace TcPlayer.Controls
{

    internal class WindowControls : ExtendedControl
    {
        public Window ControlledWindow
        {
            get { return (Window)GetValue(ControlledWindowProperty); }
            set { SetValue(ControlledWindowProperty, value); }
        }

        public static readonly DependencyProperty ControlledWindowProperty =
            DependencyProperty.Register("ControlledWindow", typeof(Window), typeof(WindowControls), new PropertyMetadata(null));

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            GetTemplateChild<Button>("PART_Minimize").Click += OnMinimize;
            GetTemplateChild<Button>("PART_Maximize").Click += OnMaximize;
            GetTemplateChild<Button>("PART_Close").Click += OnClose;
            GetTemplateChild<Border>("PART_Border").MouseLeftButtonDown += OnWindowMove;
            GetTemplateChild<ToggleButton>("PART_Pin").Click += OnPin;
        }

        private void OnPin(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton button
                && ControlledWindow != null
                && button.IsChecked.HasValue)
            {
                ControlledWindow.Topmost = (bool)button.IsChecked;
            }
        }

        private void OnWindowMove(object sender, MouseButtonEventArgs e)
            => ControlledWindow?.DragMove();

        private void OnClose(object sender, RoutedEventArgs e)
            => ControlledWindow?.Close();

        private void OnMaximize(object sender, RoutedEventArgs e)
        {
            if (ControlledWindow == null)
                return;

            if (ControlledWindow.WindowState == WindowState.Normal)
                ControlledWindow.WindowState = WindowState.Maximized;
            else
                ControlledWindow.WindowState = WindowState.Normal;
        }

        private void OnMinimize(object sender, RoutedEventArgs e)
            => ControlledWindow.WindowState = WindowState.Minimized;
    }
}
