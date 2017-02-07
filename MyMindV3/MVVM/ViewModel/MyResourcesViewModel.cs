﻿using MvvmFramework.Models;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MvvmFramework.ViewModel
{
    public class MyResourcesViewModel : BaseViewModel
    {
        int searchSelected;
        public int SearchSelected
        {
            get { return searchSelected; }
            set { Set(() => SearchSelected, ref searchSelected, value); }
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

        string searchPostcode;
        public string SearchPostcode
        {
            get { return searchPostcode; }
            set { Set(() => SearchPostcode, ref searchPostcode, value, true); }
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

        public void GetResources(bool isClinicial = false, bool isLocal = false)
        {
            var url = isLocal ? "GetLocalResources" : "GetNationalResources";
            var param = new List<string>{"UserGUID", isClinicial ? ClinicianUser.ClinicianGUID : SystemUser.Guid, "AuthToken", isClinicial ? ClinicianUser.APIToken : SystemUser.APIToken,
                "AccountType", SystemUser.IsAuthenticated.ToString(), "Page", "1", "Sorting", "null", "Categorys", "1,2,3"};
            Resources = GetData.GetLocalNationalResources(url, param.ToArray()).Result;
        }

        public IEnumerable<ResourceModel> GetNationalResources
        {
            get
            {
                return Resources.Where(t => t.ResourceIsNational) as IEnumerable<ResourceModel>;
            }
        }

        public IEnumerable<ResourceModel> GetLocalResources
        {
            get { return Resources.Where(t => !t.ResourceIsNational) as IEnumerable<ResourceModel>; }
        }

        public string SearchBy { get; set; }

        public IEnumerable<ResourceModel> GetSearchByResources
        {
            get { return Resources.Select(t => t.ResourceCategory.FirstOrDefault(w => w.ResourceCategoryTitle == SearchBy)) as IEnumerable<ResourceModel>; }
        }

        public IEnumerable<ResourceModel> GetSearchByRatings
        {
            get
            {
                return Resources.ToList().OrderBy(t => t.ResourceRating.ResourceRating) as IEnumerable<ResourceModel>;

            }
        }

        public IEnumerable<ResourceModel> GetSearchByAZ
        {
            get
            {
                return Resources.ToList().OrderBy(t => t) as IEnumerable<ResourceModel>;
            }
        }

        public IEnumerable<ResourceModel> GetSearchByDistance
        {
            get
            {
                var res = Resources.ToList();
                IsBusy = true;
                foreach (var r in res)
                {
                    r.ResourceDistance = GetData.GetDistanceFromPostcodes(SearchPostcode, r.ResourcePostcode).Result;
                }
                IsBusy = false;
                return res.OrderBy(t => t.ResourceDistance) as IEnumerable<ResourceModel>;
            }
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
            var res = Resources.ToList();
            foreach (var c in cat)
            {
                try
                {
                    var resid = res.Select(t => t.ResourceCategory.FirstOrDefault(w => w.ResourceCategoryTitle == categories[cat.IndexOf(c)]).ResourceCategoryId);
                    names.Add(string.Format("{0}|{1}", filenames[filenames.IndexOf(c.ToLowerInvariant())], resid));
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine("Exception - {0}::{1}", ex.Message, ex.InnerException);
#endif
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

                var rt = (from Res in Resources
                          from Rc in Res.ResourceCategory
                          where !string.IsNullOrEmpty(Rc.ResourceCategoryTitle)
                          select Res).ToList();

                resources = rt;

                foreach (var r in resources)
                {
                    foreach (var c in r.ResourceCategory)
                    {
                        if (!string.IsNullOrEmpty(c.ResourceCategoryTitle))
                        {
                            var splitRes = c.ResourceCategoryTitle.Split(',').ToList();
                            if (splitRes.Count == 1)
                                Categories.Add(splitRes[0]);
                            else
                            {
                                foreach (var s in splitRes)
                                {
                                    foreach (var l in catList)
                                        if (l.ToLowerInvariant() == s.ToLowerInvariant())
                                            Categories.Add(l);
                                }
                            }
                        }
                    }
                }
                Categories = Categories.Distinct().ToList();
                Categories.Sort();

                if (Categories.Count == 0)
                    Categories.AddRange((from Res in Resources
                                         from Rc in Res.ResourceCategory
                                         where !string.IsNullOrEmpty(Rc.ResourceCategoryTitle)
                                         select Rc.ResourceCategoryTitle).ToList());

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
                        CurrentRating = rnd.Next(0, 5),
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
            var ratingStars = new List<string>() { "emptystar", "emptystar", "emptystar", "emptystar", "emptystar" };
            if (rating == 0)
                return ratingStars;

            switch (rating)
            {
                case 1:
                    ratingStars[0] = "greenstar";
                    break;
                case 2:
                    ratingStars[0] = "greenstar";
                    ratingStars[1] = "yellowstar";
                    break;
                case 3:
                    ratingStars[0] = "greenstar";
                    ratingStars[1] = "yellowstar";
                    ratingStars[2] = "orangestar";
                    break;
                case 4:
                    ratingStars[0] = "greenstar";
                    ratingStars[1] = "yellowstar";
                    ratingStars[2] = "orangestar";
                    ratingStars[3] = "purplestar";
                    break;
                case 5:
                    ratingStars[0] = "greenstar";
                    ratingStars[1] = "yellowstar";
                    ratingStars[2] = "orangestar";
                    ratingStars[3] = "purplestar";
                    ratingStars[4] = "bluestar";
                    break;
            }

            for (var i = rating; i < ratingStars.Count; ++i)
                ratingStars[i] = "emptystar";

            return ratingStars;
        }

        int newRating;
        public int NewRating
        {
            get { return newRating; }
            set { Set(() => NewRating, ref newRating, value); }
        }

        int resId;
        public int ResId
        {
            get { return resId; }
            set { Set(() => ResId, ref resId, value); }
        }

        public async Task SetRating(bool isClinician)
        {
            var currentRespond = Resources.FirstOrDefault(t => t.ResourceRating.ResourceRatingId == ResId).ResourceRating;
            var data = new List<string>{"UserGUID", isClinician ? ClinicianUser.ClinicianGUID : SystemUser.Guid, "AuthToken", isClinician ? ClinicianUser.APIToken : SystemUser.APIToken,
                "AccountType", SystemUser.IsAuthenticated.ToString(), "ResourceID", ResId.ToString(), "Rating", NewRating.ToString()};
            await Send.Rated("RateResource", ResId, data.ToArray()).ContinueWith((t) =>
            {
                if (t.IsCompleted)
                {
                    currentRespond.ResourceRating = t.Result.Rating;
                    currentRespond.ResourceResponded = t.Result.Respondents;
                }
            });
        }

        public async Task ReportLink(bool isClinician)
        {
            var data = new List<string>{"UserGUID", isClinician ? ClinicianUser.ClinicianGUID : SystemUser.Guid, "AuthToken", isClinician ? ClinicianUser.APIToken : SystemUser.APIToken,
                "AccountType", SystemUser.IsAuthenticated.ToString(), "ResourceID", ResId.ToString()};
            await Send.ReportBrokenLink("ReportBrokenLink", data.ToArray());
        }
    }
}
