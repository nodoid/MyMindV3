using System;
using System.Collections.Generic;
using System.Net.Http;
using Xamarin.Forms;
using MyMindV3.Languages;
using System.Linq;
using MvvmFramework.Models;
using MvvmFramework.ViewModel;

namespace MyMindV3.Views
{
    public partial class MyResourcesView : ContentPage
    {
        IEnumerable<ResourceModel> resources;
        List<string> Categories;

        MyResourcesViewModel ViewModel => App.Locator.MyResources;

        public MyResourcesView()
        {
            InitializeComponent();
            BindingContext = ViewModel;

            if (Device.OS == TargetPlatform.iOS)
            {
                stkPicker.BackgroundColor = Color.White;
                stkPicker.WidthRequest = App.ScreenSize.Width;
            }
            var cats = new List<string>();
            resources = ViewModel.Resources;
            
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            Categories = ViewModel.FillCategories;
                            ResourcesListView.ItemTemplate = new DataTemplate(typeof(ResourcesView));
                            ResourcesListView.ItemsSource = resources;
                            ResourcesListView.ItemSelected += (sender, e) =>
                            {
                                if (e.SelectedItem == null) return; // don't do anything if we just de-selected the row
                                var resource = (ResourceModel)e.SelectedItem;
                                //await DisplayAlert(Langs.MyResources_AlertSelected, string.Format("{0} {1}", Langs.MyResources_AlertSelectedMsg, resource.ResourceID), Langs.Gen_OK);
                                ((ListView)sender).SelectedItem = null; // de-select the row
                            };

                            cats.AddRange(Categories);
                            pickFilter.Items.Add(Langs.MyResources_FilterOff);
                            foreach (var c in cats)
                                pickFilter.Items.Add(c);
                        });
        }

        void Handle_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            var s = sender as Picker;
            var idx = s.SelectedIndex == 0 ? 0 : s.SelectedIndex - 1;
            var cat = Categories[idx];
            var res = resources?.Where(t => t.ResourceCategory.ToLowerInvariant().Contains(cat.ToLowerInvariant())).ToList();
            ResourcesListView.ItemsSource = null;
            ResourcesListView.ItemsSource = s.SelectedIndex != 0 ? res : resources;
        }

        void Handle_Clicked(object sender, System.EventArgs e)
        {
            var url = ((Button)sender).ClassId;
            var equals = url.IndexOf('=');
            var fromequal = url.Substring(equals + 1);
            var full = fromequal.Substring(0, fromequal.IndexOf("\""));
            Device.BeginInvokeOnMainThread(() => Device.OpenUri(new Uri(full)));
        }
    }
}
