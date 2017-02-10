using MvvmFramework.Models;
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

        int maxPages;
        public int MaxPages
        {
            get { return maxPages; }
            set { Set(() => MaxPages, ref maxPages, value); }
        }

        int currentPage;
        public int CurrentPage
        {
            get { return currentPage; }
            set
            {
                Set(() => CurrentPage, ref currentPage, value);
                DisableNextPageButton = currentPage + 1 > maxPages;
                DisableBackPageButton = currentPage == 0;
            }
        }

        bool disableBackPageButton;
        public bool DisableBackPageButton
        {
            get { return disableBackPageButton; }
            set { Set(() => DisableBackPageButton, ref disableBackPageButton, value, true); }
        }


        bool disableNextPageButton;
        public bool DisableNextPageButton
        {
            get { return disableNextPageButton; }
            set { Set(() => DisableNextPageButton, ref disableNextPageButton, value, true); }
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

        double speed;
        public double Speed
        {
            get { return speed; }
            set { Set(() => Speed, ref speed, value); }
        }

        public bool PositionChanged(double lon, double lat)
        {
            if (Speed > 4) // a stroll
                return false;

            var changed = false;
            if ((lon != Longitude && lat != Latitude)
                || (lon != Longitude) || (lat != Latitude))
                changed = true;

            if (changed)
            {
                Latitude = lat;
                Longitude = lon;
                var _ = GetMyPostcode;
                GetAvailablePostcodes();
            }

            return changed;
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

        IEnumerable<Models.Resources> resources;
        public IEnumerable<Models.Resources> Resources
        {
            get { return resources; }
            set { Set(() => Resources, ref resources, value); }
        }

        public void GetResources(bool isClinicial = false, bool isLocal = true)
        {
            var url = isLocal ? "GetLocalResources" : "GetNationalResources";
            var param = new List<string>{"UserGUID", isClinicial ? ClinicianUser.ClinicianGUID : SystemUser.Guid, "AuthToken", isClinicial ? ClinicianUser.APIToken : SystemUser.APIToken,
                "AccountType", SystemUser.IsAuthenticated.ToString(), "Page", currentPage.ToString(), "Sorting", "AZ", "Postcode", SearchPostcode, "Title", null, "Categorys", "null"};
            Resources = GetData.GetLocalNationalResources(url, param.ToArray()).Result;
        }

        public IEnumerable<Models.Resources> GetNationalResources
        {
            get
            {
                return Resources.Where(t => t.ResourceIsNational) as IEnumerable<Models.Resources>;
            }
        }

        public IEnumerable<Models.Resources> GetLocalResources
        {
            get { return Resources.Where(t => !t.ResourceIsNational) as IEnumerable<Models.Resources>; }
        }

        public string SearchBy { get; set; }

        public IEnumerable<Models.Resources> GetSearchByResources
        {
            get { return Resources.Select(t => t.ResourceCategory.FirstOrDefault(w => w.ResourceCategoryTitle == SearchBy)) as IEnumerable<Models.Resources>; }
        }

        public IEnumerable<Models.Resources> GetSearchByRatings
        {
            get
            {
                return Resources.ToList().OrderBy(t => t.ResourceRating) as IEnumerable<Models.Resources>;

            }
        }

        public IEnumerable<Models.Resources> GetSearchByAZ
        {
            get
            {
                return Resources.ToList().OrderBy(t => t) as IEnumerable<Models.Resources>;
            }
        }

        public IEnumerable<Models.Resources> GetSearchByDistance
        {
            get
            {
                var res = Resources.ToList();
                IsBusy = true;
                foreach (var r in res)
                {
                    r.ResourceDistance = GetData.GetDistanceFromPostcodes(SearchPostcode, r.ResourcePostcode).Result.ToString();
                }
                IsBusy = false;
                return res.OrderBy(t => t.ResourceDistance) as IEnumerable<Models.Resources>;
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
            var res = Resources?.ToList();
            var count = 0;
            foreach (var c in cat)
            {
                try
                {
                    var resid = res[count].ResourceID;
                    var spn = c.Split(',');
                    var filename = string.Empty;

                    foreach (var s in spn)
                    {
                        if (filenames.IndexOf(s.ToLowerInvariant()) != -1)
                        {
                            filename = filenames[filenames.IndexOf(s.ToLowerInvariant())];
                            break;
                        }
                    }

                    if (string.IsNullOrEmpty(filename))
                        filename = "general";
                    names.Add(string.Format("{0}|{1}", filename, resid));
                    count++;
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
            /*if (AvailablePostcodes.Count == 0)
                return;*/

            var res = Resources?.ToList();
            if (res != null)
            {
                try
                {
                    for (var n = 0; n < res.Count; ++n)
                    {
                        dataList.Add(new ListviewModel
                        {
                            Title = res[n].ResourceTitle,
                            ImageIcon = !string.IsNullOrEmpty(res[n].ResourceLogoLink) ? res[n].ResourceLogoLink : string.Empty,
                            Category = res[n].ResourceCategorysPiped.Replace('|', ',').Replace(" ", ""),
                            CurrentRating = res[n].ResourceRating,
                            StarRatings = new List<string>(),
                            Id = n,
                            Postcode = res[n].ResourcePostcode,
                            //Distance = GetData.GetDistanceFromPostcodes(SearchPostcode, res[n].ResourcePostcode).Result,
                            Distance = Convert.ToDouble(res[n].ResourceDistance),
                            HasW = res[n].ResourceIsDead,
                            Url = res[n].ResourceIsDead ? string.Empty : res[n].ResourceURL
                        });
                        dataList[n].StarRatings = ConvertRatingToStars(dataList[n].CurrentRating);
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine(ex.Message);
#endif
                }
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
            var currentRespond = Resources.FirstOrDefault(t => t.ResourceID == ResId);
            var data = new List<string>{"UserGUID", isClinician ? ClinicianUser.ClinicianGUID : SystemUser.Guid, "AuthToken", isClinician ? ClinicianUser.APIToken : SystemUser.APIToken,
                "AccountType", SystemUser.IsAuthenticated.ToString(), "ResourceID", ResId.ToString(), "Rating", NewRating.ToString()};
            await Send.Rated("RateResource", ResId, data.ToArray()).ContinueWith((t) =>
            {
                if (t.IsCompleted)
                {
                    currentRespond.ResourceRating = t.Result.Rating;
                    currentRespond.ResourceNumberOfRating = t.Result.Respondents;
                }
            });
        }

        public async Task ReportLink(bool isClinician)
        {
            var data = new List<string>{"UserGUID", isClinician ? ClinicianUser.ClinicianGUID : SystemUser.Guid, "AuthToken", isClinician ? ClinicianUser.APIToken : SystemUser.APIToken,
                "AccountType", SystemUser.IsAuthenticated.ToString(), "ResourceID", ResId.ToString()};
            await Send.ReportBrokenLink("ReportBrokenLink", data.ToArray());
            var res = Resources.FirstOrDefault(t => t.ResourceID == ResId);
            res.ResourceIsDead = true;
            res.ResourceLogoLink = string.Empty;
            SetUpdateResource = res;
        }

        Models.Resources setUpdateResource;
        public Models.Resources SetUpdateResource
        {
            get { return setUpdateResource; }
            set { Set(() => SetUpdateResource, ref setUpdateResource, value, true); }
        }

        public List<string> GetCategtoriesFromResource
        {
            get
            {
                return Resources.FirstOrDefault(t => t.ResourceID == ResId).ResourceCategorysPiped.Split('|').ToList();
            }
        }

        public void GetAllDetails()
        {
            GetAvailablePostcodes();
            GetResources();
            GetUIList();
        }
    }
}
