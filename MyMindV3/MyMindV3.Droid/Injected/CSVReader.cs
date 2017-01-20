﻿using System.Collections.Generic;
using MyMindV3.Droid;
using MyMindV3.Models;
using Xamarin.Forms;
using System.IO;
using System.Linq;
using System;

[assembly: Dependency(typeof(CSVReader))]
namespace MyMindV3.Droid
{
    public class CSVReader : IDataReader
    {
        public List<ResourceModel> LoadDataFile()
        {
            var data = new List<ResourceModel>();
            int count = 0;
            using (var stream = Forms.Context.Assets.Open("camhs.csv"))
            {
                using (var sr = new StreamReader(stream))
                {
                    var tt = sr.ReadToEnd();
                    var t = tt.Split(new string[] { "\"," }, StringSplitOptions.None).Select(str => str + "\"").ToArray(); ;
                    for (var c = 0; c < t.Length; c += 6)
                    {
                        try
                        {
                            data.Add(new ResourceModel
                            {
                                ResourceID = count,
                                ResourceCategory = t[c].Replace("\"", "").Replace("\n", ""),
                                ResourceLocation = t[c + 1].Replace("\"", "").Replace("\n", ""),
                                ResourceAddress = t[c + 2].Replace("\"", "").Replace("\n", ""),
                                ResourceServices = t[c + 3].Replace("\"", "").Replace("\n", ""),
                                ResourceReferrals = t[c + 4].Replace("\"", "").Replace("\n", ""),
                                ResourceContactInfo = t[c + 5].Replace("\"", "").Replace("\n", "")
                            });
                            count++;
#if DEBUG
                            Console.WriteLine("count = {0}", count);
#endif
                        }
                        catch (IndexOutOfRangeException ex)
                        {
                            Console.WriteLine("Exception : {0}--{1}", ex.Message, ex.InnerException);
                        }
                    }
                }
            }

            return data;
        }
    }
}
