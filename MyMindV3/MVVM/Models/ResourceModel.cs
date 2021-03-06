﻿using System.Collections.Generic;
using System;
using SQLite.Net.Attributes;

namespace MvvmFramework.Models
{
    public class ResourceCategory
    {
        public int ResourceCategoryID { get; set; }
        public int ResId { get; set; }
        public string ResourceCategoryTitle { get; set; }
        public string ResourceCategoryDescription { get; set; }
    }

    public class Resources
    {
        public int ResourceID { get; set; }
        [Ignore]
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
        public bool IsLocal { get; set; } = true;
        public DateTime ResDateTime { get; set; }
    }

    public class ResourcesModel
    {
        public List<Resources> Resources { get; set; }
        public int CurrentNationalPage { get; set; }
        public int TotalNationalResources { get; set; }
        public int TotalNationalPagesRequired { get; set; }
        public int CurrentLocalPage { get; set; }
        public int TotalLocalResources { get; set; }
        public int TotalLocalPagesRequired { get; set; }
    }
}
