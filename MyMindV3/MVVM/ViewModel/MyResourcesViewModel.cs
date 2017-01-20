using GalaSoft.MvvmLight.Views;
using MvvmFramework.Models;
using MvvmFramework.Webservices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MvvmFramework.ViewModel
{
    public class MyResourcesViewModel : BaseViewModel
    {
        INavigationService navService;
        public MyResourcesViewModel(INavigationService nav)
        {
            navService = nav;
        }

        IEnumerable<ResourceModel> resources;
        public IEnumerable<ResourceModel> Resources
        {
            get { Resources = new UsersWebservice().GetResources().Result ; return resources; }
            set { Set(() => Resources, ref resources, value); }
        }

        public List<string> FillCategories
        {
            get
            {
                var Categories = new List<string>();
                var catList = new List<string>
            {
                "Self-help", "Self-harm", "Depression", "Bullying", "Support", "Anxiety", "Stress", "Parent resources", "Involvement", "Local Services",
                "Mood","Substance misuse", "Bereavement", "Well-being", "Young carers", "Peer support", "Parents resources", "Video resources", "Safeguarding",
                "Looked after children", "Apps", "Involvement", "Learning disabilities", "Autism", "Sleep", "ADHD", "General"
            };
                catList.Sort();

                var rt = Resources.Where(t => !string.IsNullOrEmpty(t.ResourceCategory)).Select(t => t).ToList();
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

                return Categories;
            }
        }
    }
}
