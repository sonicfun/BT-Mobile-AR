using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Fuzzy
{
    public class FuzzyValue
    {
        public string Name { get; set; }
         public IMembership MF { get; set; }
        public FuzzyVariable Parent { get; set; }

        public FuzzyValue()
        {
       
        }
    }
}
