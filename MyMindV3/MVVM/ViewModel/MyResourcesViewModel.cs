using MvvmFramework.Models;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using MvvmFramework.Enums;
using GalaSoft.MvvmLight.Views;
using MvvmFramework.Interfaces;
#if DEBUG
using System.Diagnostics;
#endif
using System.Collections.ObjectModel;

namespace MvvmFramework.ViewModel
{
    public class MyResourcesViewModel : BaseViewModel
    {
        IRepository sqlRepo;
        IConnectivity connectService;
        IDialogService diaService;

        public MyResourcesViewModel(IRepository repo, IConnectivity con, IDialogService dia)
        {
            sqlRepo = repo;
            diaService = dia;
            connectService = con;

            if (con.IsConnected)
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
            set
            {
                Set(() => MaxLocalPages, ref maxLocalPages, value, true);
                if (value > 1)
                    DisableLocalNextPageButton = false;
            }
        }

        int currentLocalPage;
        public int CurrentLocalPage
        {
            get { return currentLocalPage; }
            set
            {
                Set(() => CurrentLocalPage, ref currentLocalPage, value);
                DisableLocalNextPageButton = currentLocalPage + 1 > MaxLocalPages;
                DisableLocalBackPageButton = currentLocalPage == 1;
            }
        }

        int maxNationalPages;
        public int MaxNataionalPages
        {
            get { return maxNationalPages; }
            set
            {
                Set(() => MaxNataionalPages, ref maxNationalPages, value, true);
                if (value > 1)
                    DisableNationalNextPageButton = false;
            }
        }

        int currentNationalPage;
        public int CurrentNationalPage
        {
            get { return currentNationalPage; }
            set
            {
                Set(() => CurrentLocalPage, ref currentNationalPage, value);
                DisableNationalNextPageButton = currentNationalPage + 1 > MaxNataionalPages;
                DisableNationalBackPageButton = currentNationalPage == 1;
            }
        }

        bool disableNationalBackPageButton;
        public bool DisableNationalBackPageButton
        {
            get { return disableNationalBackPageButton; }
            set { Set(() => DisableNationalBackPageButton, ref disableNationalBackPageButton, value, true); }
        }


        bool disableNationalNextPageButton;
        public bool DisableNationalNextPageButton
        {
            get { return disableNationalNextPageButton; }
            set { Set(() => DisableNationalNextPageButton, ref disableNationalNextPageButton, value, true); }
        }

        bool disableLocalBackPageButton;
        public bool DisableLocalBackPageButton
        {
            get { return disableLocalBackPageButton; }
            set { Set(() => DisableLocalBackPageButton, ref disableLocalBackPageButton, value, true); }
        }


        bool disableLocalNextPageButton;
        public bool DisableLocalNextPageButton
        {
            get { return disableLocalNextPageButton; }
            set { Set(() => DisableLocalNextPageButton, ref disableLocalNextPageButton, value, true); }
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
                if (connectService.IsConnected)
                    SendTrackingInformation(GetIsClinician ? ActionCodes.Clinician_Resources_Searched_By_Postcode :
                    (SystemUser.IsAuthenticated == 2 ? ActionCodes.User_Resources_Searched_By_Postcode :
                    ActionCodes.Member_Resources_Searched_By_Postcode));
            }
        }

        public void GetAvailablePostcodes()
        {
            if (connectService.IsConnected)
                AvailablePostcodes = GetData.GetSurroundingPostcodes(SearchPostcode).Result;
            else
                diaService.ShowMessage(NetworkErrors[1], NetworkErrors[0]);
        }

        public string GetMyPostcode
        {
            get
            {
                if (connectService.IsConnected)
                {
                    var postcode = GetData.GetPostcode(Longitude, Latitude).Result;
                    SearchPostcode = postcode;
                    return postcode;
                }
                else
                {
                    diaService.ShowMessage(NetworkErrors[1], NetworkErrors[0]);
                    return string.Empty;
                }
            }
        }

        IEnumerable<Resources> localResources;
        public IEnumerable<Resources> ListLocalResources
        {
            get { return localResources; }
            set { Set(() => ListLocalResources, ref localResources, value, true); GetUIList(ShowingLocal ? UIType.Local : UIType.National); }
        }

        IEnumerable<Resources> nationalResources;
        public IEnumerable<Resources> ListNationalResources
        {
            get { return nationalResources; }
            set { Set(() => ListNationalResources, ref nationalResources, value, true); GetUIList(ShowingLocal ? UIType.Local : UIType.National); }
        }

        string GetSearchType(Sorting type)
        {
            var rv = string.Empty;

            switch (type)
            {
                case Sorting.AZ:
                    rv = "AZ";
                    break;
                case Sorting.Distance:
                    rv = "Distance";
                    break;
                case Sorting.Rating:
                    rv = "Rating";
                    break;
                case Sorting.MostPopular:
                    rv = "Popular";
                    break;
            }

            return rv;
        }

        string ConvertCategoryToInt(string cat)
        {
            var rv = string.Empty;
            var carList = new List<string>
            {
                "Abuse","Addiction/Drugs","ADHD","Adoption","Advice-General","Apps","Autism",
                "Bereavement", "Bullying", "CSS","Counselling&Advice", "Disabilities","Divorce&Separation",
                "EatingDisorders&FeedingDifficulties","EducationSupport","Grandparents","Internet","LGBT",
                "Money","Relationships","Podcasts","SelfHarm","SexualHealth","SuicidalIdeation/Thoughts/Intent",
                "Support","Depression","emergencysupport/Outofhours","AdviceandSupport","Parent/CarerSupport",
                "LocalSupport","Anxiety","learningdisabilities","Self-esteem","Anger","LowMood", "Friendship",
                "groupwork","ParentalMentalhealth","ParentalOffending","DomesticAbuse","BehaviouralConcerns",
                "SupportforTeachers","ChildSexualExploitation","YoungRefugeesandMigrants","ChildProtection",
                "YoungCarers","Involvement","Mindfullness","Excerises","Self-Help","Family","YouthOffending",
                "Onlinesafety","SexualAssualt","Stress","Volunteering","PeerSupport","Vlog","Videos",
                "Celebrity","Sleep","Exams","School","BodyImage","EatingDisorder","OCD"
            };
            var catNums = new List<int>
            {
                1,2,3,4,5,6,7,8,9,11,12,14,15,16,17,18,21,23,24,26,27,29,30,32,33,34,35,36,37,38,
                72,73,74,75,76,77,79,80,81,82,83,84,85,86,87,88,89,90,91,92,93,94,95,96,97,98,99,
                100,101,102,103,104,105,106,107,108
            };

            var cats = cat.Replace(" ", "").ToLowerInvariant().Split(',');
            var res = (from c in cats
                       from cl in carList
                       where c == cl.ToLowerInvariant()
                       select catNums.IndexOf(cl.ToLowerInvariant().IndexOf(c, StringComparison.CurrentCulture))).ToList();

            if (res.Count == 0)
                rv = "null";
            else
            {
                if (res.Count == 1)
                    rv = res[0].ToString();
                else
                {
                    foreach (var r in res)
                        rv += string.Format("{0},", r);
                    rv = rv.Substring(0, rv.LastIndexOf(','));
                }
            }
            return rv;
        }

        public async Task ReloadResources(Sorting type, string cats = "null")
        {
            if (connectService.IsConnected)
            {
                var catlist = cats == "null" ? "null" : ConvertCategoryToInt(cats);
                var param = new List<string>{"UserGUID", GetIsClinician ? ClinicianUser.ClinicianGUID : SystemUser.Guid, "AuthToken", GetIsClinician ? ClinicianUser.APIToken : SystemUser.APIToken,
                    "AccountType", SystemUser.IsAuthenticated.ToString(), "Page", "1",
                    "Sorting", GetSearchType(type), "Postcode", SearchPostcode, "Title", "null", "Categorys", catlist};
                await GetData.GetLocalNationalResources("GetLocalResources", param.ToArray()).ContinueWith(async (obj) =>
                {
                    if (obj.IsCompleted && (!obj.IsFaulted || !obj.IsCanceled))
                    {
                        if (obj.Result.Resources?.Count != 0)
                        {
                            ListLocalResources = obj.Result.Resources;
                            CurrentLocalPage = 1;
                        }
                        await GetData.GetLocalNationalResources("GetNationalResources", param.ToArray()).ContinueWith((t) =>
                        {
                            if (t.IsCompleted && (!t.IsFaulted || !t.IsCanceled))
                            {
                                if (t.Result.Resources?.Count != 0)
                                {
                                    ListNationalResources = t.Result.Resources;
                                    CurrentNationalPage = 1;
                                }
                            }
                        });
                    }
                });
            }
            else
                await diaService.ShowMessage(NetworkErrors[1], NetworkErrors[0]);
        }

        public void GetLocalResources(bool isClinicial = false, Sorting type = Sorting.AZ)
        {
            if (connectService.IsConnected)
            {
                var param = new List<string>{"UserGUID", isClinicial ? ClinicianUser.ClinicianGUID : SystemUser.Guid, "AuthToken", isClinicial ? ClinicianUser.APIToken : SystemUser.APIToken,
                    "AccountType", SystemUser.IsAuthenticated.ToString(), "Page", CurrentLocalPage.ToString(),
                    "Sorting", GetSearchType(type), "Postcode", SearchPostcode, "Title", "null", "Categorys", "null"};

                var local = GetData.GetLocalNationalResources("GetLocalResources", param.ToArray()).Result;

                if (local.Resources?.Count != 0)
                {
                    MaxLocalPages = local.TotalLocalPagesRequired;
                    ListLocalResources = local.Resources;
                }
            }
            else
                diaService.ShowMessage(NetworkErrors[1], NetworkErrors[0]);
        }

        public void GetNationalResources(bool isClinicial = false, Sorting type = Sorting.AZ)
        {
            if (connectService.IsConnected)
            {
                var param = new List<string>{"UserGUID", isClinicial ? ClinicianUser.ClinicianGUID : SystemUser.Guid, "AuthToken", isClinicial ? ClinicianUser.APIToken : SystemUser.APIToken,
                "AccountType", SystemUser.IsAuthenticated.ToString(), "Page", CurrentNationalPage.ToString(),
                    "Sorting", GetSearchType(type), "Postcode", SearchPostcode, "Title", "null", "Categorys", "null"};

                var national = GetData.GetLocalNationalResources("GetNationalResources", param.ToArray()).Result;

                if (national != null)
                {
                    MaxNataionalPages = national.TotalNationalPagesRequired;
                    ListNationalResources = national.Resources;
                }
            }
            else
                diaService.ShowMessage(NetworkErrors[1], NetworkErrors[0]);
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
            return res?.OrderBy(t => Convert.ToDouble(t.ResourceDistance)).ToList();
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

        ObservableCollection<ListviewModel> uiList;
        public ObservableCollection<ListviewModel> UIList
        {
            get { return uiList; }
            set { Set(() => UIList, ref uiList, value, true); }
        }

        public void UpdateUIList(ListviewModel lvm)
        {
            var list = UIList;
            var rep = list.IndexOf(list.FirstOrDefault(t => t.Id == lvm.Id));
            list.Insert(rep, lvm);
            list.RemoveAt(rep + 1);
            UIList = list;
            NewRating = lvm.CurrentRating;
            ResId = lvm.Id;
            RaisePropertyChanged("HasW");
        }

        public void GetUIList(UIType ui = UIType.Global, Sorting sort = Sorting.AZ)
        {
            //var dataList = new List<ListviewModel>();
            var dataList = new ObservableCollection<ListviewModel>();
            /*if (UIList != null)
                UIList.Clear();
            else
                UIList = new ObservableCollection<ListviewModel>();*/
            //UIList = new List<ListviewModel>();
            var res = new List<Resources>();

            res = ui == UIType.Global ? ListLocalResources?.ToList() : (ui == UIType.National ? ListNationalResources?.ToList() : ListLocalResources?.ToList());

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
                            Id = res[n].ResourceID,
                            Postcode = res[n].ResourcePostcode,
                            Distance = Convert.ToDouble(res[n].ResourceDistance),
                            HasW = res[n].ResourceType == 1,
                            HasH = res[n].ResourceType == 2,
                            HasR = res[n].ResourceType == 3,
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

            if (dataList.Count != 0)
                UIList = dataList;
        }

        public List<string> ConvertRatingToStars(int rating, int id)
        {
            var ratingStars = new List<string>() { "emptystar", "emptystar", "emptystar", "emptystar", "emptystar" };
            SendTrackingInformation(GetIsClinician ? ActionCodes.Clinician_Rated_Resource : (SystemUser.IsAuthenticated == 1 ? ActionCodes.Member_Rated_Resource : ActionCodes.User_Rated_Resource), id, rating.ToString());

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

        public void SetRating(bool isClinician)
        {

            Task.Run(async () =>
            {
                if (connectService.IsConnected)
                {
                    var currentRespond = ShowingLocal ? ListLocalResources.FirstOrDefault(t => t.ResourceID == ResId) : ListNationalResources.FirstOrDefault(t => t.ResourceID == ResId);
                    var data = new List<string>{"UserGUID", isClinician ? ClinicianUser.ClinicianGUID : SystemUser.Guid, "AuthToken", isClinician ? ClinicianUser.APIToken : SystemUser.APIToken,
                "AccountType", SystemUser.IsAuthenticated.ToString(), "ResourceID", ResId.ToString(), "Rating", NewRating.ToString()};
                    await Send.Rated("api/MyMind/RateResource", ResId, data.ToArray()).ContinueWith((t) =>
                    {
                        if (t.IsCompleted)
                        {
                            currentRespond.ResourceRating = t.Result.Rating;
                            currentRespond.ResourceNumberOfRating = t.Result.Respondents;
                        }
                    });
                }
                else
                    await diaService.ShowMessage(NetworkErrors[1], NetworkErrors[0]);

            });
        }

        public void ReportLink(int ResId)
        {
            if (connectService.IsConnected)
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
                return GetCategoryList;
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
                        if (CurrentLocalPage - 1 >= 1)
                        {
                            CurrentLocalPage--;
                            LastUpdated = DateTime.Now;
                            GetLocalResources(GetIsClinician);
                            BackForwardChanged = true;
                            DisableLocalBackPageButton = CurrentLocalPage == 1;
                        }
                        else
                        {
                            DisableLocalBackPageButton = true;
                        }
                    }
                    else
                    {
                        if (CurrentNationalPage - 1 >= 1)
                        {
                            CurrentNationalPage--;
                            LastUpdated = DateTime.Now;
                            GetNationalResources(GetIsClinician);
                            BackForwardChanged = true;
                            DisableNationalBackPageButton = CurrentNationalPage == 1;
                        }
                        else
                            DisableNationalBackPageButton = true;

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
                        if (CurrentLocalPage + 1 <= MaxLocalPages)
                        {
                            CurrentLocalPage++;
                            LastUpdated = DateTime.Now;
                            GetLocalResources(GetIsClinician);
                            BackForwardChanged = true;
                            DisableLocalNextPageButton = CurrentLocalPage == MaxLocalPages;
                        }
                        else
                            DisableLocalNextPageButton = true;
                    }
                    else
                    {
                        if (CurrentNationalPage + 1 <= MaxNataionalPages)
                        {
                            CurrentNationalPage++;
                            LastUpdated = DateTime.Now;
                            GetNationalResources(GetIsClinician);
                            BackForwardChanged = true;
                            DisableNationalNextPageButton = CurrentNationalPage == MaxNataionalPages;
                        }
                        else
                            DisableNationalNextPageButton = true;
                    }
                })
                    );
            }
        }
    }
}
