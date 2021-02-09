using System.Collections.Generic;

namespace Fuzzy
{
    public class FuzzyRuleAnd: IRule
    {
        private List<FuzzyValue> _Antecedents;
        public List<FuzzyValue> Antecedents { get { return _Antecedents; } }
        public FuzzyValue Consequent { get; set; }

         public double FireStrength
        {
            get
            { return _firestrength; }
        }

        private double _firestrength; 

        public FuzzyRuleAnd()
        {
            _Antecedents = new List<FuzzyValue>();
            _firestrength = 0;
        }

         public void CalcFiringStrength()
        {
            double fs = 1;
            foreach(var ant in Antecedents)
            {
                double v = ant.MF.getMembershipValue();
                if (v < fs) // get the min
                    fs = v;
            }
            _firestrength = fs;
        }
    }
}
