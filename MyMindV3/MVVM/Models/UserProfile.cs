using System;

namespace MvvmFramework.Models
{
    public class UserProfile
    {
        public string UserGUID { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string DateOfBirth { get; set; }
        public string ContactNumber { get; set; }
        public string ReferralReason { get; set; }
        public string Likes { get; set; }
        public string Dislikes { get; set; }
        public string Goals { get; set; }
        public string Pincode { get; set; }
        public ClinicianProfile AssignedClinician { get; set; }
        public string UnreadMessages { get; set; }
        public int IsAuthenticated { get; set; }
        public string RIOID { get; set; }
        // added PFJ 25-8-16
        public string UserImage { get; set; } = string.Empty;
        public string PreferredName { get; set; }
        // missing - added
        public string APIToken { get; set; }
        public string PictureFilePath { get; set; }
        public string ProfilePictureUploadTimestamp { get; set; }
        public DateTime APITokenExpiry { get; set; }
        // needed for clinician login
        public string HCPID { get; set; }
        public string FunFact { get; set; }
        public string Guid { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string WhatIDo { get; set; }
        public string ClinicianGUID { get; set; }
        public string AssignedClinicianGUID { get; set; }
    }
}
