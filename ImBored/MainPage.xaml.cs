using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace ImBored
{

    //TODO: change visibility to converter binding
    //nice ui
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private void AloneButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            alone.Visibility = Visibility.Visible;
            group.Visibility = Visibility.Collapsed;
            play.Visibility = Visibility.Collapsed;
        }

        private void FriendsButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            alone.Visibility = Visibility.Collapsed;
            group.Visibility = Visibility.Visible;
            play.Visibility = Visibility.Collapsed;
        }

        private void PlayButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            alone.Visibility = Visibility.Collapsed;
            group.Visibility = Visibility.Collapsed;
            play.Visibility = Visibility.Visible;
        }
    }
}
