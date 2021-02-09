using System.Collections.Generic;

namespace Fuzzy
{
    public interface IRule
    {
        List<FuzzyValue> Antecedents { get; }
        FuzzyValue Consequent { get; set; }
        double FireStrength { get; }
        void CalcFiringStrength();
    }
}
