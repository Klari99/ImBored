using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
    public sealed partial class GroupActivityComponent : StackPanel
    {
        private string _type = "random";
        private double _accessibility = -1;
        private bool _free = false;
        private int _participants = 2;
        List<string> types = new List<string>();

        public string Activity
        {
            get { return (string)GetValue(ActivityProperty); }
            set { SetValue(ActivityProperty, value); }
        }

        public static readonly DependencyProperty ActivityProperty =
            DependencyProperty.Register("Activity", typeof(string),
                typeof(GroupActivityComponent), null);

        public string Type
        {
            get { return (string)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register("Type", typeof(string),
                typeof(GroupActivityComponent), null);

        public string Link
        {
            get { return (string)GetValue(LinkProperty); }
            set { SetValue(LinkProperty, value); }
        }

        public static readonly DependencyProperty LinkProperty =
            DependencyProperty.Register("Link", typeof(string),
                typeof(GroupActivityComponent), null);

        public string Accessibility
        {
            get { return (string)GetValue(AccessibilityProperty); }
            set { SetValue(AccessibilityProperty, value); }
        }

        public static readonly DependencyProperty AccessibilityProperty =
            DependencyProperty.Register("Accessibility", typeof(string),
                typeof(GroupActivityComponent), null);

        public string Price
        {
            get { return (string)GetValue(PriceProperty); }
            set { SetValue(PriceProperty, value); }
        }

        public static readonly DependencyProperty PriceProperty =
            DependencyProperty.Register("Price", typeof(string),
                typeof(GroupActivityComponent), null);

        public string Participants
        {
            get { return (string)GetValue(ParticipantsProperty); }
            set { SetValue(ParticipantsProperty, value); }
        }

        public static readonly DependencyProperty ParticipantsProperty =
            DependencyProperty.Register("Participants", typeof(string),
                typeof(GroupActivityComponent), null);

        public GroupActivityComponent()
        {
            this.InitializeComponent();

            types.Add("random");
            types.Add("charity");
            types.Add("cooking");
            types.Add("busywork");
            types.Add("diy");
            types.Add("education");
            types.Add("music");
            types.Add("recreational");
            types.Add("relaxation");
            types.Add("social");

            typeComboBox.ItemsSource = types;
        }

        private void TypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            _type = comboBox.SelectedValue.ToString();
        }

        private void AccessibilitySlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Slider slider = sender as Slider;
            _accessibility = 1 - (slider.Value - (slider.Value % 5)) / 100;
            slider.Header = "Minimum accessibility: " + slider.Value.ToString() + "%";
        }

        private void ParticipantsSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Slider slider = sender as Slider;
            _participants = (int)slider.Value;
            slider.Header = "Participants: " + slider.Value.ToString();
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            _free = freeCheckBox.IsChecked.GetValueOrDefault(false);
            await getAPIAnswer();
        }

        private async Task getAPIAnswer()
        {
            HttpClient hc = new HttpClient();
            String url = "http://www.boredapi.com/api/activity?participants=" + _participants;

            if (_type != "random" && types.Contains(_type))
            {
                url += "&type=" + _type;
            }

            if (_free)
            {
                url += "&price=0.0";
            }

            if (_accessibility != -1)
            {
                url += "&maxaccessibility=" + _accessibility;
            }

            var response = await hc.GetAsync(url);
            string responseText = await response.Content.ReadAsStringAsync();

            ActivityResult activity = Newtonsoft.Json.JsonConvert.DeserializeObject<ActivityResult>(responseText);
            Activity = activity.Activity;
            Type = activity.Type;
            if (activity.Link != "" && activity.Link != null)
            {
                linkTextBlock.Visibility = Visibility.Visible;
                Link = activity.Link;
            }
            else
            {
                linkTextBlock.Visibility = Visibility.Collapsed;
                Link = "";
            }

            double access = (1 - activity.Accessibility) * 100;
            if (access == 0) access++;
            Accessibility = access.ToString() + "%";
            if (activity.Price != 0.0)
            {
                Price = "Not free";
            }
            else
            {
                Price = "Free";
            }

            if (activity.Activity == null)
            {
                resultTextBlock.Visibility = Visibility.Visible;
                result.Visibility = Visibility.Collapsed;
            }
            else
            {
                result.Visibility = Visibility.Visible;
                resultTextBlock.Visibility = Visibility.Collapsed;
            }
            Participants = activity.Participants.ToString();
        }
    }
}
