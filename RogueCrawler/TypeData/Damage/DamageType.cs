using CommandEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueCrawler
{
    internal struct DamageTypeData
    {
        public string Name { get; set; }
        public DamageCategory Category { get; set; }
        public DamageFlags Flags { get; set; }

        public DamageTypeData() { }
        public DamageTypeData(string name) { Name = name; }
    }
}
