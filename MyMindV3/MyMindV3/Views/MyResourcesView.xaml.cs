
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
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
        List<string> Categories = new List<string>();

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
            SetValues().ContinueWith((t) =>
                {
                    if (t.IsCompleted)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            FillCategories();
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

        void FillCategories()
        {
            var catList = new List<string>
            {
                "Self-help", "Self-harm", "Depression", "Bullying", "Support", "Anxiety", "Stress", "Parent resources", "Involvement", "Local Services",
                "Mood","Substance misuse", "Bereavement", "Well-being", "Young carers", "Peer support", "Parents resources", "Video resources", "Safeguarding",
                "Looked after children", "Apps", "Involvement", "Learning disabilities", "Autism", "Sleep", "ADHD", "General"
            };
            catList.Sort();

            var rt = resources.Where(t => !string.IsNullOrEmpty(t.ResourceCategory)).Select(t => t).ToList();
            resources = rt;

            foreach (var r in resources)
            {
                if (!string.IsNullOrEmpty(r.ResourceCategory))
                {
                    var splitRes = r.ResourceCategory.Split(',').ToList();
                    if (splitRes.Count == 1)
                        Categories.Add(splitRes[0]);
                    else
                    {
                        foreach (var s in splitRes)
                        {
                            foreach (var c in catList)
                                if (c.ToLowerInvariant() == s.ToLowerInvariant())
                                    Categories.Add(c);
                        }
                    }
                }
            }
            Categories = Categories.Distinct().ToList();
            Categories.Sort();

            if (Categories.Count == 0)
                Categories.AddRange(resources.Select(t => t.ResourceCategory).ToList());
        }

        void Handle_Clicked(object sender, System.EventArgs e)
        {
            var url = ((Button)sender).ClassId;
            var equals = url.IndexOf('=');
            var fromequal = url.Substring(equals + 1);
            var full = fromequal.Substring(0, fromequal.IndexOf("\""));
            Device.BeginInvokeOnMainThread(() => Device.OpenUri(new Uri(full)));
        }

        private async Task SetValues()
        {
            resources = await GetResources();
        }

        async Task<IEnumerable<ResourceModel>> GetResources()
        {
            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri("https://apps.nelft.nhs.uk/CareMapApi/api/MyMind/GetResources");

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await client.GetAsync("https://apps.nelft.nhs.uk/CareMapApi/api/MyMind/GetResources");

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<IEnumerable<ResourceModel>>(responseString);
                }

                return null;
            }
        }

    }
}
