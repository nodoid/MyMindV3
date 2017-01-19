using MyMindV3.Models;
using System.Collections.Generic;
namespace MyMindV3
{
    public interface IDataReader
    {
        List<ResourceModel> LoadDataFile();
    }
}

