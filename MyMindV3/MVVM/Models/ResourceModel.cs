using System.Collections.Generic;
namespace MvvmFramework.Models
{
    public class ResourceCategory
    {
        public int ResourceCategoryId { get; set; }
        public string ResourceCategoryTitle { get; set; }
        public string ResourceCategoryDescription { get; set; }
    }

    public class NationalResources
    {
        public int CurrentNationalPage { get; set; }
        public int TotalNationalResouces { get; set; }
        public int TotalNationalPagesRequired { get; set; }
        public List<ResourceModel> Resources { get; set; }
    }

    public class LocalResources
    {
        public int CurrentLocalPage { get; set; }
        public int TotalLocalResouces { get; set; }
        public int TotalLocalPagesRequired { get; set; }
        public List<ResourceModel> Resources { get; set; }
    }

    public class ResourceModel
    {
        public int ResourceID { get; set; }
        public List<ResourceCategory> ResourceCategory { get; set; }
        public string ResourceCategorysPiped { get; set; }
        public int ResourceType { get; set; }
        public string ResourceTypeTitle { get; set; }
        public string ResourceTitle { get; set; }
        public string ResourceDescription { get; set; }
        public string ResourceLogoLink { get; set; }
        public string ResourceURL { get; set; }
        public int ResourceRating { get; set; }
        public int ResourceNumberOfRating { get; set; }
        public string ResourcePostcode { get; set; }
        public bool ResourceIsDead { get; set; }
        public bool ResourceIsNational { get; set; }
        public string ResourceDistance { get; set; }
    }


}
