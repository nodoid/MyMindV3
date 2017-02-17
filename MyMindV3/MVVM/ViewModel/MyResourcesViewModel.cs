using MvvmFramework.Models;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using MvvmFramework.Enums;
#if DEBUG
using System.Diagnostics;
#endif

namespace MvvmFramework.ViewModel
{
    public class MyResourcesViewModel : BaseViewModel
    {
        IRepository sqlRepo;

        public MyResourcesViewModel(IRepository repo)
        {
            sqlRepo = repo;
            SendTrackingInformation(GetIsClinician ? ActionCodes.Clinician_Resources_Page_View :
                (SystemUser.IsAuthenticated == 2 ? ActionCodes.User_Resources_Page_View : ActionCodes.Member_Resources_Page_View));
        }

        Sorting currentSort;
        public Sorting CurrentSort
        {
            get { return currentSort; }
            set { Set(() => CurrentSort, ref currentSort, value); }
        }

        bool backForwardChanged;
        public bool BackForwardChanged
        {
            get { return backForwardChanged; }
            set { Set(() => BackForwardChanged, ref backForwardChanged, value, true); }
        }

        DateTime lastUpdated;
        public DateTime LastUpdated
        {
            get { return lastUpdated; }
            set { Set(() => LastUpdated, ref lastUpdated, value); }
        }

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

        int searchCategory;
        public int SearchCategory
        {
            get { return searchCategory; }
            set { Set(() => SearchCategory, ref searchCategory, value); }
        }

        int maxLocalPages;
        public int MaxLocalPages
        {
            get { return maxLocalPages; }
            set { Set(() => MaxLocalPages, ref maxLocalPages, value); }
        }

        int currentLocalPage;
        public int CurrentLocalPage
        {
            get { return currentLocalPage; }
            set
            {
                Set(() => CurrentLocalPage, ref currentLocalPage, value);
                DisableNextPageButton = currentLocalPage + 1 > MaxLocalPages;
                DisableBackPageButton = currentLocalPage == 0;
            }
        }

        int maxNationalPages;
        public int MaxNataionalPages
        {
            get { return maxNationalPages; }
            set { Set(() => MaxNataionalPages, ref maxNationalPages, value); }
        }

        int currentNationalPage;
        public int CurrentNationalPage
        {
            get { return currentNationalPage; }
            set
            {
                Set(() => CurrentLocalPage, ref currentNationalPage, value);
                DisableNextPageButton = currentNationalPage + 1 > MaxNataionalPages;
                DisableBackPageButton = currentNationalPage == 0;
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
            set
            {
                Set(() => SearchPostcode, ref searchPostcode, value, true);
                SendTrackingInformation(GetIsClinician ? ActionCodes.Clinician_Resources_Searched_By_Postcode :
                (SystemUser.IsAuthenticated == 2 ? ActionCodes.User_Resources_Searched_By_Postcode :
                ActionCodes.Member_Resources_Searched_By_Postcode));
            }
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

        public void GetLocalResources(bool isClinicial = false)
        {
            var param = new List<string>{"UserGUID", isClinicial ? ClinicianUser.ClinicianGUID : SystemUser.Guid, "AuthToken", isClinicial ? ClinicianUser.APIToken : SystemUser.APIToken,
                    "AccountType", SystemUser.IsAuthenticated.ToString(), "Page", CurrentLocalPage.ToString(),
                    "Sorting", "AZ", "Postcode", SearchPostcode, "Title", null, "Categorys", "null"};

            var local = GetData.GetLocalNationalResources("GetLocalResources", param.ToArray()).Result;

            if (local != null)
            {
                MaxLocalPages = local.TotalLocalPagesRequired;
                ListLocalResources = local.Resources;
            }
        }

        public void GetNationalResources(bool isClinicial = false)
        {
            var param = new List<string>{"UserGUID", isClinicial ? ClinicianUser.ClinicianGUID : SystemUser.Guid, "AuthToken", isClinicial ? ClinicianUser.APIToken : SystemUser.APIToken,
                "AccountType", SystemUser.IsAuthenticated.ToString(), "Page", CurrentNationalPage.ToString(),
                    "Sorting", "AZ", "Postcode", SearchPostcode, "Title", null, "Categorys", "null"};

            var national = GetData.GetLocalNationalResources("GetNationalResources", param.ToArray()).Result;

            if (national != null)
            {
                MaxNataionalPages = national.TotalNationalPagesRequired;
                ListNationalResources = national.Resources;
            }
        }

        public string SearchBy { get; set; }

        List<Resources> SortByRatings(List<Resources> res)
        {
            return res?.OrderByDescending(t => t.ResourceRating).ToList();
        }

        List<Resources> SortByAZ(List<Resources> res)
        {
            return res?.OrderBy(t => t.ResourceTitle).ToList();
        }

        List<Resources> SortByDistance(List<Resources> res)
        {
            return res?.OrderBy(t => t.ResourceDistance).ToList();
        }

        List<Resources> SortByMostPopular(List<Resources> res)
        {
            return res?.OrderByDescending(t => t.ResourceNumberOfRating).ThenBy(t => t.ResourceRating).ToList();
        }

        public List<string> GetCategoryList
        {
            get
            {
                var cats = ShowingLocal ? ListLocalResources?.Select(t => t.ResourceCategorysPiped).ToList() : ListNationalResources?.Select(t => t.ResourceCategorysPiped).ToList();
                var catlist = new List<string> { "None" };
                if (cats != null)
                {
                    foreach (var cat in cats)
                    {
                        var cs = cat.Split('|');
                        foreach (var c in cs)
                        {
                            if (c.Contains(" "))
                            {
#if DEBUG
                                Debug.WriteLine("LastIndex = {0}, Length = {1}", c.LastIndexOf(' '), c.Length);
#endif
                                catlist.Add(c.TrimStart(' ').TrimEnd(' '));
                            }
                            else
                                catlist.Add(c);
                        }
                    }
                }
                return catlist.Distinct().ToList();
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

        public void UpdateUIList(ListviewModel lvm)
        {
            var list = UIList;
            var rep = list.IndexOf(list.FirstOrDefault(t => t.Id == lvm.Id));
            list[rep] = lvm;
            UIList = list;
        }

        public void GetUIList(UIType ui = UIType.Global, Sorting sort = Sorting.AZ)
        {
            var dataList = new List<ListviewModel>();
            if (UIList != null)
                UIList.Clear();
            else
                UIList = new List<ListviewModel>();
            List<Resources> res = new List<Resources>();

            if (ShowingLocal)
            {
                res = ui == UIType.Global ? ListLocalResources?.ToList() : (ui == UIType.National ? ListNationalResources?.ToList() : ListLocalResources?.ToList());
            }

            if (SearchCategory == 0)
            {
                switch (sort)
                {
                    case Sorting.AZ:
                        res = SortByAZ(res);
                        break;
                    case Sorting.Rating:
                        res = SortByRatings(res);
                        break;
                    case Sorting.Distance:
                        res = SortByDistance(res);
                        break;
                    case Sorting.MostPopular:
                        res = SortByMostPopular(res);
                        break;
                }
            }
            else
            {
                var searchby = GetCategoryList[SearchCategory];
                switch (sort)
                {
                    case Sorting.AZ:
                        res = SortByAZ(res).Where(t => t.ResourceCategorysPiped.Contains(searchby)).ToList();
                        break;
                    case Sorting.Rating:
                        res = SortByRatings(res).Where(t => t.ResourceCategorysPiped.Contains(searchby)).ToList();
                        break;
                    case Sorting.Distance:
                        res = SortByDistance(res).Where(t => t.ResourceCategorysPiped.Contains(searchby)).ToList();
                        break;
                    case Sorting.MostPopular:
                        res = SortByMostPopular(res);
                        break;
                }
            }

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
                        dataList[n].StarRatings = ConvertRatingToStars(dataList[n].CurrentRating, n);
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

        public List<string> ConvertRatingToStars(int rating, int id)
        {
            var ratingStars = new List<string>() { "emptystar", "emptystar", "emptystar", "emptystar", "emptystar" };
            SendTrackingInformation(GetIsClinician ? ActionCodes.Clinician_Rated_Resource : (SystemUser.IsAuthenticated == 1 ? ActionCodes.Member_Rated_Resource : ActionCodes.User_Rated_Resource), id, rating.ToString());
            RaisePropertyChanged("StarRatings");
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
            set { Set(() => NewRating, ref newRating, value, true); }
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

        public void ReportLink(int ResId)
        {
            SendBrokenLink(ResId, GetIsClinician ? ActionCodes.Clinician_Resource_Reported_Broken :
                (SystemUser.IsAuthenticated == 2 ? ActionCodes.User_Resource_Reported_Broken : ActionCodes.Member_Resource_Reported_Broken));
            var res = ShowingLocal ? ListLocalResources.FirstOrDefault(t => t.ResourceID == ResId) :
                ListNationalResources.FirstOrDefault(t => t.ResourceID == ResId);
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
            if (ShowingLocal)
                GetLocalResources(GetIsClinician);
            else
                GetNationalResources(GetIsClinician);
            GetUIList();
        }

        RelayCommand btnBackCommand;
        public RelayCommand BtnBackCommand
        {
            get
            {
                return btnBackCommand ??
                    (
                        btnBackCommand = new RelayCommand(() =>
                {
                    if (ShowingLocal)
                    {
                        if (CurrentLocalPage - 1 >= 0)
                        {
                            CurrentLocalPage--;
                            LastUpdated = DateTime.Now;
                            GetLocalResources(GetIsClinician);
                            BackForwardChanged = true;
                        }
                        else
                            DisableBackPageButton = true;
                    }
                    else
                    {
                        if (CurrentNationalPage - 1 >= 0)
                        {
                            CurrentNationalPage--;
                            LastUpdated = DateTime.Now;
                            GetNationalResources(GetIsClinician);
                            BackForwardChanged = true;
                        }
                        else
                            DisableBackPageButton = true;
                    }
                })
                    );
            }
        }

        RelayCommand btnForwardCommand;
        public RelayCommand BtnForwardCommand
        {
            get
            {
                return btnForwardCommand ??
                    (
                        btnForwardCommand = new RelayCommand(() =>
                {
                    if (ShowingLocal)
                    {
                        if (CurrentLocalPage + 1 < MaxLocalPages)
                        {
                            CurrentLocalPage++;
                            LastUpdated = DateTime.Now;
                            GetLocalResources(GetIsClinician);
                            BackForwardChanged = true;
                        }
                        else
                            DisableNextPageButton = true;
                    }
                    else
                    {
                        if (CurrentNationalPage + 1 < MaxNataionalPages)
                        {
                            CurrentNationalPage++;
                            LastUpdated = DateTime.Now;
                            GetNationalResources(GetIsClinician);
                            BackForwardChanged = true;
                        }
                        else
                            DisableNextPageButton = true;
                    }
                })
                    );
            }
        }
    }
}
