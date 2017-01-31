using GalaSoft.MvvmLight.Views;
using MvvmFramework.Models;
using MvvmFramework.Webservices;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Diagnostics;

namespace MvvmFramework.ViewModel
{
    public class MyResourcesViewModel : BaseViewModel
    {
        INavigationService navService;
        public MyResourcesViewModel(INavigationService nav)
        {
            navService = nav;
        }

        string searchPostcode;
        public string SearchPostcode
        {
            get { return searchPostcode; }
            set { Set(() => SearchPostcode, ref searchPostcode, value, true); }
        }

        double longitude;
        public double Longitude
        {
            get { return longitude; }
            set { Set(() => Longitude, ref longitude, value, true); }
        }

        double latitude;
        public double Latitude
        {
            get { return latitude; }
            set { Set(() => Latitude, ref latitude, value, true); }
        }

        List<Postcodes> availablePostcodes;
        public List<Postcodes> AvailablePostcodes
        {
            get { return availablePostcodes; }
            set { Set(() => AvailablePostcodes, ref availablePostcodes, value); }
        }

        public void GetAvailablePostcodes()
        {
            AvailablePostcodes = GetData.GetSurroundingPostcodes(SearchPostcode).Result;
        }

        public string GetMyPostcode
        {
            get
            {
                var postcode = GetData.GetPostcode(Longitude, Latitude).Result;
                SearchPostcode = postcode;
                return postcode;
            }
        }

        IEnumerable<ResourceModel> resources;
        public IEnumerable<ResourceModel> Resources
        {
            get { return resources; }
            set { Set(() => Resources, ref resources, value); }
        }

        void GetResources()
        {
            Resources = new UsersWebservice().GetResources().Result;
        }

        public string SearchBy { get; set; }

        public IEnumerable<ResourceModel> GetSearchByResources
        {
            get { return Resources.Where(t => t.ResourceCategory == SearchBy); }
        }

        public List<string> GetResourceFilenames(List<string> categories)
        {
            if (categories == null)
                return new List<string>();
            if (categories.Count == 0)
                return new List<string>();

            var filenames = new List<string>{"anxiety","depression","apps","autism","bereavement","bullying","general","involvement", "learning_disabilities",
                "local_services", "looked_after_children","mental_health","safeguarding","mood", "parent_resources", "peer_support", "self_harm", "self_help", "www", "stress", "substance_abuse",
                "video", "well_being", "youth_support", "sleeping","adhd","young_carer", "domestic_abuse", "spousal_abuse", "solvent_abuse"};

            var cats = categories.Distinct().ToList();
            var cat = cats.Select(t => t.Replace('-', '_')).Select(t => t.Replace(' ', '_')).ToList();

            var names = new List<string>();
            foreach (var c in cat)
            {
                try
                {
                    names.Add(filenames[filenames.IndexOf(c.ToLowerInvariant())]);
                }
                catch (Exception)
                {

                }
            }

            return names;
        }

        public List<string> FillCategories
        {
            get
            {
                var Categories = new List<string>();
                var catList = new List<string>
            {
                "Self-help", "Self-harm", "Depression", "Bullying", "Support", "Anxiety", "Stress", "Parent resources", "Involvement", "Local Services",
                "Mood","Substance misuse", "Bereavement", "Well-being", "Young carers", "Peer support", "Video resources", "Safeguarding",
                "Looked after children", "Apps", "Involvement", "Learning disabilities", "Autism", "Sleep", "ADHD", "General", "Spousal Abuse", "Domestic Abuse", "Solvent Abuse"
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

        List<ListviewModel> uiList;
        public List<ListviewModel> UIList
        {
            get { return uiList; }
            set { Set(() => UIList, ref uiList, value); }
        }

        public void GetUIList()
        {
            var dataList = new List<ListviewModel>();
            if (UIList != null)
                UIList.Clear();
            else
                UIList = new List<ListviewModel>();
            if (AvailablePostcodes.Count == 0)
                return;

            var nums = AvailablePostcodes.Count / 10;

            var lastPostcode = 0;
            var c = 1;
            var titles = new List<string>
            {
                "Mental health facilites in London",
                "Dealing with loss",
                "Recognising depression",
                "Coping with senility",
                "ADHD - strategies for learning",
                "Your visit to hospital",
                "Care for the elderly",
                "What to do if you notice self harm",
                "Recognising the signs of drug misuse",
                "Solvent abuse. How to help",
                "You are not alone"
            };
            var icons = new List<string>{"anxiety","depression","apps","autism","bereavement","bullying","general","involvement", "learning_disabilities",
                "local_services", "looked_after_children","mental_health","safeguarding","mood", "parent_resources", "peer_support", "self_harm", "self_help", "www", "stress", "substance_abuse",
                "video", "well_being", "young_carer", "sleeping","adhd","domestic_abuse","spousal_abuse","solvent_abuse"};
            var cats = new List<string> { "Anxiety", "Depression", "Self-harm", "Solvent abuse", "Young carer", "Domestic abuse", "Well being", "Spousal abuse", "Autism", "ADHD" };
            var rnd = new Random();
            try
            {
                for (var n = 0; n < 10; ++n)
                {
                    var pc = rnd.Next(lastPostcode, nums * c);
                    dataList.Add(new ListviewModel
                    {
                        Title = titles[rnd.Next(0, titles.Count)],
                        ImageIcon = icons[rnd.Next(0, icons.Count)],
                        Category = cats[rnd.Next(0, titles.Count)],
                        CurrentRating = rnd.Next(0, 6),
                        StarRatings = new List<string>(),
                        Id = n,
                        Postcode = AvailablePostcodes[pc].postcode,
                        Distance = AvailablePostcodes[pc].distance,
                        HasH = rnd.Next(0, 10) > 5,
                        HasR = rnd.Next(0, 10) < 5,
                        HasW = rnd.Next(0, 10) > 6
                    });
                    lastPostcode += nums;
                    c++;
                    dataList[n].StarRatings = ConvertRatingToStars(dataList[n].CurrentRating);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine(ex.Message);
#endif
            }
            UIList = dataList;
        }

        public List<string> ConvertRatingToStars(int rating)
        {
            var ratingStars = new List<string>() { "emptystar", "emptystar", "emptystar", "emptystar", "emptystar", "emptystar" };
            if (rating == 0)
                return ratingStars;

            switch (rating)
            {
                case 1:
                    ratingStars[0] = "pinkstar";
                    break;
                case 2:
                    ratingStars[0] = "pinkstar";
                    ratingStars[1] = "purplestar";
                    break;
                case 3:
                    ratingStars[0] = "pinkstar";
                    ratingStars[1] = "purplestar";
                    ratingStars[2] = "greenstar";
                    break;
                case 4:
                    ratingStars[0] = "pinkstar";
                    ratingStars[1] = "purplestar";
                    ratingStars[2] = "greenstar";
                    ratingStars[3] = "bluestar";
                    break;
                case 5:
                    ratingStars[0] = "pinkstar";
                    ratingStars[1] = "purplestar";
                    ratingStars[2] = "greenstar";
                    ratingStars[3] = "bluestar";
                    ratingStars[4] = "yellowstar";
                    break;
                case 6:
                    ratingStars[0] = "pinkstar";
                    ratingStars[1] = "purplestar";
                    ratingStars[2] = "greenstar";
                    ratingStars[3] = "bluestar";
                    ratingStars[4] = "yellowstar";
                    ratingStars[5] = "orangestar";
                    break;
            }

            for (var i = rating; i < ratingStars.Count; ++i)
                ratingStars[i] = "emptystar";

            return ratingStars;
        }
    }
}
