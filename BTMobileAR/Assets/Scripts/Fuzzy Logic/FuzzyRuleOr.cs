using System.Collections.Generic;

namespace Fuzzy
{
    public class FuzzyRuleOr : IRule
    {
        private List<FuzzyValue> _Antecedents;
        public List<FuzzyValue> Antecedents { get { return _Antecedents; } }

        public FuzzyValue Consequent { get; set; }

        public double FireStrength
        {
            get  { return _firestrength; }
        }
     private double _firestrength;

    public FuzzyRuleOr()
        {
            _firestrength = 0;
            _Antecedents = new List<FuzzyValue>();
        }

        public void ApplyMembers(FuzzyValue ant1, FuzzyValue ant2, FuzzyValue cont)
        {
            Antecedents.Add(ant1);
            Antecedents.Add(ant2);
            Consequent = cont;
        }

        public void CalcFiringStrength()
        {
            double fs = 0;
            foreach (var ant in Antecedents)
            {
                double v = ant.MF.getMembershipValue();
                if (v < fs) // get the max
                    v = fs;
            }
            _firestrength = fs;
        }
    }
}
