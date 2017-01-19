using System;
namespace MvvmFramework.Models
{
    public class ClinicianProfile
    {
        public string ClinicianGUID { get; set; }
        public string Name { get; set; }
        public string WhatIDo { get; set; }
        public string FunFact { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public string HCPID { get; set; }
        // added PFJ 25-8-16
        public string UserImage { get; set; } = string.Empty;
        // added PFJ 24-11-16
        public string APIToken { get; set; }
        public string PictureFilePath { get; set; }
        public DateTime ProfilePictureTimetamp { get; set; }
        public DateTime APITokenExpiry { get; set; }
    }
}