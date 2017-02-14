using MvvmFramework.Models;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
#if DEBUG
using System.Diagnostics;
#endif

namespace MvvmFramework.ViewModel
{
    public class MyResourcesViewModel : BaseViewModel
    {
        bool showingLocal;
        public bool ShowingLocal
        {
            get { return showingLocal; }
            set { Set(() => ShowingLocal, ref showingLocal, value); }
        }

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

        IEnumerable<Resources> localResources;
        public IEnumerable<Resources> ListLocalResources
        {
            get { return localResources; }
            set { Set(() => ListLocalResources, ref localResources, value, true); }
        }

        IEnumerable<Resources> nationalResources;
        public IEnumerable<Resources> ListNationalResources
        {
            get { return nationalResources; }
            set { Set(() => ListNationalResources, ref nationalResources, value, true); }
        }

        public void GetResources(bool isClinicial = false, bool isLocal = true)
        {
            var url = isLocal ? "GetLocalResources" : "GetNationalResources";
            var param = new List<string>{"UserGUID", isClinicial ? ClinicianUser.ClinicianGUID : SystemUser.Guid, "AuthToken", isClinicial ? ClinicianUser.APIToken : SystemUser.APIToken,
                "AccountType", SystemUser.IsAuthenticated.ToString(), "Page", currentPage.ToString(), "Sorting", "AZ", "Postcode", SearchPostcode, "Title", null, "Categorys", "null"};
            var res = GetData.GetLocalNationalResources(url, param.ToArray()).Result;
            if (isLocal)
                ListLocalResources = res;
            else
                ListNationalResources = res;
        }

        public string SearchBy { get; set; }

        public IEnumerable<Resources> GetSearchByResources
        {
            get
            {
                return ShowingLocal ? ListLocalResources.Select(t => t.ResourceCategory.FirstOrDefault(w => w.ResourceCategoryTitle == SearchBy)) as IEnumerable<Resources> :
                                                        ListNationalResources.Select(t => t.ResourceCategory.FirstOrDefault(w => w.ResourceCategoryTitle == SearchBy)) as IEnumerable<Resources>;
            }
        }

        public IEnumerable<Resources> GetSearchByRatings
        {
            get
            {
                return ShowingLocal ? ListLocalResources.ToList().OrderBy(t => t.ResourceRating) as IEnumerable<Resources> :
                                                        ListNationalResources.ToList().OrderBy(t => t.ResourceRating) as IEnumerable<Resources>;

            }
        }

        public IEnumerable<Resources> GetSearchByAZ
        {
            get
            {
                return ShowingLocal ? ListLocalResources.ToList().OrderBy(t => t) as IEnumerable<Resources> :
                                                        ListNationalResources.ToList().OrderBy(t => t) as IEnumerable<Resources>;
            }
        }

        public IEnumerable<Resources> GetSearchByDistance
        {
            get
            {
                var res = ShowingLocal ? ListLocalResources.ToList() : ListNationalResources.ToList();
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
            var res = ShowingLocal ? ListLocalResources?.ToList() : ListNationalResources?.ToList();
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

        public string GetResourceFilename(string category)
        {
            var filenames = new List<string>{"anxiety","depression","apps","autism","bereavement","bullying","general","involvement", "learning_disabilities",
                "local_services", "looked_after_children","mental_health","safeguarding","mood", "parent_resources", "peer_support", "self_harm", "self_help", "www", "stress", "substance_abuse",
                "video", "well_being", "youth_support", "sleeping","adhd","young_carer", "domestic_abuse", "spousal_abuse", "solvent_abuse"};
            var cats = category.Split(',').ToList();
            var cat = cats.Select(t => t.Replace('-', '_')).Select(t => t.Replace(' ', '_')).ToList();
            var name = string.Empty;
            var count = 0;
            foreach (var c in cat)
            {
                try
                {
                    foreach (var s in cat)
                    {
                        if (filenames.IndexOf(s.ToLowerInvariant()) != -1)
                        {
                            name = filenames[filenames.IndexOf(s.ToLowerInvariant())];
                            break;
                        }
                    }

                    if (string.IsNullOrEmpty(name))
                        name = "general";
                    count++;
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine("Exception - {0}::{1}", ex.Message, ex.InnerException);
#endif
                }
            }

            return name;
        }

        List<ListviewModel> uiList;
        public List<ListviewModel> UIList
        {
            get { return uiList; }
            set { Set(() => UIList, ref uiList, value, true); }
        }

        public void GetUIList(UIType ui = UIType.Global)
        {
            var dataList = new List<ListviewModel>();
            if (UIList != null)
                UIList.Clear();
            else
                UIList = new List<ListviewModel>();
            List<Resources> res = new List<Resources>();

            if (ShowingLocal)
                res = ui == UIType.Global ? ListLocalResources?.ToList() : (ui == UIType.Global ? ListNationalResources?.ToList() : ListLocalResources?.ToList());

            if (res != null)
            {
                try
                {
                    for (var n = 0; n < res.Count; ++n)
                    {
                        var cat = res[n].ResourceCategorysPiped.Replace('|', ',').Replace(" ", "");
                        dataList.Add(new ListviewModel
                        {
                            Title = res[n].ResourceTitle,
                            ImageIcon = !string.IsNullOrEmpty(res[n].ResourceLogoLink) ? res[n].ResourceLogoLink : GetResourceFilename(cat),
                            Category = cat,
                            CurrentRating = res[n].ResourceRating,
                            StarRatings = new List<string>(),
                            Id = n,
                            Postcode = res[n].ResourcePostcode,
                            Distance = Convert.ToDouble(res[n].ResourceDistance),
                            HasW = res[n].ResourceIsDead,
                            HasH = false,
                            HasR = false,
                            Url = (string.IsNullOrEmpty(res[n].ResourceURL)) ? string.Empty : res[n].ResourceURL
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
            var currentRespond = ShowingLocal ? ListLocalResources.FirstOrDefault(t => t.ResourceID == ResId) : ListNationalResources.FirstOrDefault(t => t.ResourceID == ResId);
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
            var res = ShowingLocal ? ListLocalResources.FirstOrDefault(t => t.ResourceID == ResId) : ListNationalResources.FirstOrDefault(t => t.ResourceID == ResId);
            res.ResourceIsDead = true;
            res.ResourceLogoLink = string.Empty;
            SetUpdateResource = res;
        }

        Resources setUpdateResource;
        public Resources SetUpdateResource
        {
            get { return setUpdateResource; }
            set { Set(() => SetUpdateResource, ref setUpdateResource, value, true); }
        }

        public List<string> GetCategtoriesFromResource
        {
            get
            {
                return ShowingLocal ? ListLocalResources.FirstOrDefault(t => t.ResourceID == ResId).ResourceCategorysPiped.Split('|').ToList() :
                                                        ListNationalResources.FirstOrDefault(t => t.ResourceID == ResId).ResourceCategorysPiped.Split('|').ToList();
            }
        }

        public void GetAllDetails()
        {
            //GetAvailablePostcodes();
            GetResources(GetIsClinician, ShowingLocal);
            GetUIList();
        }
    }
}
