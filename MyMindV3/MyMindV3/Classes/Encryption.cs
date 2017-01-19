using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMindV3.Classes
{
    public class Encryption
    {
        public string Category { get; set; }

        public string Value { get; set; }

        public string Key { get; set; }

        public string IV { get; set; }

        public string Hashmac { get; set; }
    }
}
