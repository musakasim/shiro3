using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Shiro.View.ViewManagement
{
    public class Pencere : UserControl
    {
        public static int ActiveWindowOpacity;
        public string ShowStoryboardName { get; set; }
        public string HideStoryboardName { get; set; }

        public Window ParentWindow { get; set; }


        public void Hide()
        {
            if (Math.Abs(Opacity) < 0.01)
                return;

            if (!string.IsNullOrWhiteSpace(HideStoryboardName))
            {
                if (!IsVisible)
                    ((Storyboard) ParentWindow.Resources[HideStoryboardName]).Begin(ParentWindow);
            }
            else
            {
                //Visibility = Visibility.Hidden;
                HideWithDefaultStoryBoard();
            }
        }

        public void Show()
        {
            if (!string.IsNullOrWhiteSpace(ShowStoryboardName))
            {
                // Uses an animation to show the Control
                if (!IsVisible)
                    ((Storyboard) ParentWindow.Resources[ShowStoryboardName]).Begin(ParentWindow);
            }
            else
            {
                //Opacity = 1.0;
                //Visibility = Visibility.Visible;
                ShowWithDefaultStoryBoard();
            }
        }

        private void ShowWithDefaultStoryBoard()
        {
            Opacity = 0.0;
            //set visible but opacity is 0 so i expect that its not shown
            //with animation opacity will rise to 1 so it will be really visible
            Visibility = Visibility.Visible;

            var a = new DoubleAnimation
            {
                From = 0.0,
                To = 1.0,
                BeginTime = TimeSpan.FromSeconds(0.3),
                Duration = new Duration(TimeSpan.FromSeconds(0.9))
            };
            var storyboard = new Storyboard();

            storyboard.Children.Add(a);
            Storyboard.SetTarget(a, this);
            Storyboard.SetTargetProperty(a, new PropertyPath(OpacityProperty));
            // storyboard.Completed += delegate { Visibility = Visibility.Hidden; };
            storyboard.Begin();
        }

        private void HideWithDefaultStoryBoard()
        {
            Opacity = 1.0;
            //will be set to Hidden on storyboard.Completed
            Visibility = Visibility.Visible;

            var a = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                BeginTime = TimeSpan.FromSeconds(0),
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };
            var storyboard = new Storyboard();

            storyboard.Children.Add(a);
            Storyboard.SetTarget(a, this);
            Storyboard.SetTargetProperty(a, new PropertyPath(OpacityProperty));
            storyboard.Completed += delegate { Visibility = Visibility.Hidden; };
            storyboard.Begin();
        }
    }
}