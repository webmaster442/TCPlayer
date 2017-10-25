using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace TCPlayer.Controls
{
    /// <summary>
    /// Interaction logic for Overlay.xaml
    /// </summary>
    public partial class Overlay : UserControl
    {
        private Result _DialogResult;
        private string _CustomDialogResult;

        public static readonly DependencyProperty ContentWidthProperty =
            DependencyProperty.Register("ContentWidth", typeof(double), typeof(Overlay), new PropertyMetadata(280.0d));

        public static readonly DependencyProperty ContentHeightProperty =
            DependencyProperty.Register("ContentHeight", typeof(double), typeof(Overlay), new PropertyMetadata(280.0d));

        public static readonly DependencyProperty DialogContentProperty =
            DependencyProperty.Register("DialogContent", typeof(object), typeof(Overlay));

        public static readonly DependencyProperty CancelVisibleProperty =
            DependencyProperty.Register("CancelVisible", typeof(bool), typeof(Overlay), new PropertyMetadata(true));

        public double ContentWidth
        {
            get { return (double)GetValue(ContentWidthProperty); }
            set { SetValue(ContentWidthProperty, value); }
        }

        public double ContentHeight
        {
            get { return (double)GetValue(ContentHeightProperty); }
            set { SetValue(ContentHeightProperty, value); }
        }

        public object DialogContent
        {
            get { return GetValue(DialogContentProperty); }
            set { SetValue(DialogContentProperty, value); }
        }

        public bool CancelVisible
        {
            get { return (bool)GetValue(CancelVisibleProperty); }
            set { SetValue(CancelVisibleProperty, value); }
        }

        public Overlay()
        {
            InitializeComponent();
            Visibility = Visibility.Collapsed;
        }

        private void Hide()
        {
            Storyboard closeanim = FindResource("CloseAnim") as Storyboard;
            BeginStoryboard(closeanim);
        }

        public async Task<Result> Show()
        {
            _DialogResult = Result.None;
            BtnOk.Visibility = Visibility.Visible;
            CustomButtonContainer.Visibility = Visibility.Collapsed;
            Storyboard openanim = FindResource("OpenAnim") as Storyboard;
            BeginStoryboard(openanim);
            await Task.Delay(TimeSpan.FromSeconds(0.3));

            while (true)
            {
                if (_DialogResult == Result.OK)
                {
                    Hide();
                    return Result.OK;
                }
                else if (_DialogResult == Result.Cancel)
                {
                    Hide();
                    return Result.Cancel;
                }
                await Task.Delay(100);
            }
        }

        public async Task<string> Show(IDictionary<string, string> buttons)
        {
            _CustomDialogResult = null;
            BtnOk.Visibility = Visibility.Collapsed;
            CustomButtonContainer.Visibility = Visibility.Visible;
            if (DrawCustomButtons(buttons))
            {
                Storyboard openanim = FindResource("OpenAnim") as Storyboard;
                BeginStoryboard(openanim);
                await Task.Delay(TimeSpan.FromSeconds(0.3));

                while (true)
                {
                    if (!string.IsNullOrEmpty(_CustomDialogResult))
                    {
                        Hide();
                        return _CustomDialogResult;
                    }
                    if (_DialogResult == Result.Cancel)
                    {
                        Hide();
                        return null;
                    }
                    await Task.Delay(100);
                }
            }
            return null;
        }

        private bool DrawCustomButtons(IDictionary<string, string> buttons)
        {
            CustomButtonContainer.Children.Clear();
            foreach (var button in buttons)
            {
                Button b = new Button
                {
                    MinWidth = 70,
                    Margin = new Thickness(5),
                    Content = button.Value,
                    Name = button.Key
                };
                b.Click += B_Click;
                CustomButtonContainer.Children.Add(b);
            }
            return true;
        }

        private void B_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button b)
            {
                _CustomDialogResult = b.Name;
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            _DialogResult = Result.OK;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            _DialogResult = Result.Cancel;
        }
    }

    public enum Result
    {
        OK,
        Cancel,
        None,
    }
}
