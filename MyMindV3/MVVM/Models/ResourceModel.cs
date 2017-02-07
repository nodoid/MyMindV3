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
    }

    public class LocalResources
    {
        public int CurrentLocalPage { get; set; }
        public int TotalLocalResouces { get; set; }
        public int TotalLocalPagesRequired { get; set; }
    }

    public class ResourceModel
    {
        public int ResourceID { get; set; }
        public string ResourceType { get; set; }
        public string ResourceTitle { get; set; }
        public string ResourceDescription { get; set; }
        public string ResourceLogoLink { get; set; }
        public string ResourceURL { get; set; }
        public string ResourcePostcode { get; set; }
        public bool ResourceIsDead { get; set; }
        public bool ResourceIsNational { get; set; }
        public int ResourceDistance { get; set; }
        public List<ResourceCategory> ResourceCategory { get; set; }
        public NationalResources NationalResources { get; set; }
        public LocalResources LocalResources { get; set; }
        public int ResourceRating { get; set; }
        public int ResourceNumberOfRating { get; set; }
        public string ResourceCategorysPiped { get; set; }
    }
}
