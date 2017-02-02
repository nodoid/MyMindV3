namespace MvvmFramework.Models
{
    public class ResourceModel
    {
        public int ResourceID { get; set; }
        public string ResourceCategory { get; set; }
        public string ResourceLocation { get; set; }
        public string ResourceAddress { get; set; }
        public string ResourceServices { get; set; }
        public string ResourceReferrals { get; set; }
        public string ResourceContactInfo { get; set; }
        public int ResourceRating { get; set; }
    }

    public class NewResourceModel
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
    }
}
