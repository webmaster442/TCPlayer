using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace TCPlayer.Controls
{
    /// <summary>
    /// Interaction logic for SlideUp.xaml
    /// </summary>
    public partial class ActionBar : UserControl
    {
        public ActionBar()
        {
            InitializeComponent();
            Visibility = Visibility.Collapsed;
            ActionItems = new ObservableCollection<ActionItem>();
            ActionItems.CollectionChanged += Render;
        }

        private void Render(object sender, NotifyCollectionChangedEventArgs e)
        {
            DisplayPanel.Children.Clear();
            for (int i=0; i<ActionItems.Count; i++)
            {
                var ai = ActionItems[i];
                Button btn = new Button
                {
                    Content = ai.Content,
                    ToolTip = ai.ToolTip,
                    Margin = new Thickness(2.5, 0, 2.5, 2.5),
                    Padding = new Thickness(5),
                    TabIndex = i
                };
                btn.Click += ExecuteAction;
                DisplayPanel.Children.Add(btn);
            }
            BtnCancel.TabIndex = ActionItems.Count;
        }

        private void ExecuteAction(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;
            var ai = ActionItems[btn.TabIndex];
            ai.Click?.Invoke();
            var closeAnim = FindResource("CloseAnim") as Storyboard;
            BeginStoryboard(closeAnim);
        }

        public ObservableCollection<ActionItem> ActionItems { get; private set; }

        public void Open()
        {
            var openanim = FindResource("OpenAnim") as Storyboard;
            BeginStoryboard(openanim);
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            var closeAnim = FindResource("CloseAnim") as Storyboard;
            BeginStoryboard(closeAnim);
        }
    }

    public class ActionItem
    {
        public string Content { get; set; }
        public string ToolTip { get; set; }
        public Action Click { get; set; }
    }
}
