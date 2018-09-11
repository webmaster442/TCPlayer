using System;
using System.Windows;
using System.Windows.Controls;

namespace TCPlayer.Controls.Notification
{
    /// <summary>
    /// Interaction logic for NotificationPlaceSelector.xaml
    /// </summary>
    public partial class NotificationPlaceSelector : UserControl
    {
        public NotificationPlaceSelector()
        {
            InitializeComponent();
        }

        public NotificationPosition Position
        {
            get { return (NotificationPosition)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Position.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(NotificationPosition), typeof(NotificationPlaceSelector), new PropertyMetadata(NotificationPosition.LeftTop, PositionChanged));

        private static void PositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as NotificationPlaceSelector;
            sender.SetSelection((int)e.NewValue);
        }

        private void SetSelection(int value)
        {
            foreach (RadioButton rb in SelectorGrid.Children)
            {
                if (Convert.ToInt32(rb.Tag) == value)
                {
                    rb.IsChecked = true;
                }
                else
                {
                    rb.IsChecked = false;
                }
            }
        }

        private void Update(object sender, RoutedEventArgs e)
        {
            int ret = 0;
            foreach (RadioButton rb in SelectorGrid.Children)
            {
                if (rb.IsChecked == true)
                {
                    ret = Convert.ToInt32(rb.Tag);
                    break;
                }
            }
            Position = (NotificationPosition)ret;
        }
    }
}
