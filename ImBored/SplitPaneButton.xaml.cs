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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ImBored
{
    public sealed partial class SplitPaneButton : UserControl
    {
        private static SplitPaneButton clicked = null;
        private static readonly Windows.UI.Color indicatorBlue =
            Windows.UI.Color.FromArgb(255, 97, 177, 242);
        private static void changeClicked(SplitPaneButton spb)
        {
            if (clicked == null)
            {
                clicked = spb;
            }
            else if (clicked != spb)
            {
                clicked.Selected = false;
                clicked = spb;
            }
        }

        public SplitPaneButton()
        {
            this.InitializeComponent();
            this.Tapped += (s, e) => { this.Toggle(); };
        }

        void Toggle()
        {
            if (Selected)
                Selected = false;
            else
            {
                Selected = true;
                changeClicked(this);
            }
        }



        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string),
                typeof(SplitPaneButton), new PropertyMetadata("Haliho"));
        public string Text
        {
            get
            {
                return GetValue(TextProperty).ToString();
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        public bool Selected
        {
            get { return (bool)GetValue(SelectedProperty); }
            set
            {
                SetValue(SelectedProperty, value);
                if (value)
                {
                    selectionIndicator.Fill =
                        new SolidColorBrush(indicatorBlue);
                    changeClicked(this);
                }
                else
                {
                    selectionIndicator.Fill =
                        new SolidColorBrush(Windows.UI.Colors.Transparent);
                }
            }
        }

        public static readonly DependencyProperty SelectedProperty =
            DependencyProperty.Register("Selected", typeof(bool),
                typeof(SplitPaneButton), new PropertyMetadata(false));


        public String Icon
        {
            get { return (String)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(String),
                typeof(SplitPaneButton), null);
    }
}
