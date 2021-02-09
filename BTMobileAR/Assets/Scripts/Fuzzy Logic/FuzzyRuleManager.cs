using System;
using System.Collections.Generic;

namespace Fuzzy
{
    public class FuzzyRuleManager
    {
        private List<FuzzyVariable> _Antecedents;
        private List<IRule> _Rules;
        public List<FuzzyVariable> Antecedents { get { return _Antecedents; } }
        public FuzzyVariable Consequent { get; set; }
        public List<IRule> Rules { get { return _Rules; } }

        public FuzzyRuleManager()
        {
            _Antecedents = new List<FuzzyVariable>();
            _Rules = new List<IRule>();
        }

        public void AddNewAndRule(FuzzyValue ant1, FuzzyValue ant2, FuzzyValue cont)
        {
            FuzzyRuleAnd r = new FuzzyRuleAnd();
            if(ant1 != null)
                r.Antecedents.Add(ant1);
            if (ant2 != null)
                r.Antecedents.Add(ant2);
            r.Consequent = cont;
            Rules.Add(r);
        }

        public void AddNewOrRule(FuzzyValue ant1, FuzzyValue ant2, FuzzyValue cont)
        {
            FuzzyRuleOr r = new FuzzyRuleOr();
            if (ant1 != null)
                r.Antecedents.Add(ant1);
            if (ant2 != null)
                r.Antecedents.Add(ant2);
            r.Consequent = cont;
            Rules.Add(r);
        }

        // strings should be like this -> food.Rancid
        public void AddNewRule(string ant1, string ant2, string scont, bool UseAndRule=true)
        {
            string Set, sValue;
            FuzzyValue vant1 = null, vant2 = null;
            if (ant1 != "none")
            {
                ParseRuleString(ant1, out Set, out sValue);
                vant1 = FindAntFuzzyValue(Set, sValue);
                if (vant1 == null)
                    throw new Exception("Value was not found!");
            }

            if (ant2 != "none")
            {
                ParseRuleString(ant2, out Set, out sValue);
                vant2 = FindAntFuzzyValue(Set, sValue);
                if (vant2 == null)
                    throw new Exception("Value was not found!");
            }

            ParseRuleString(scont, out Set, out sValue);
            if(Consequent.Name != Set)
                throw new Exception("Wrong Set Name!");
            FuzzyValue cont = FindContFuzzyValue(sValue);
            // finally call the previous method
            if (UseAndRule)
                AddNewAndRule(vant1, vant2, cont);
            else
                AddNewOrRule(vant1, vant2, cont);
        }

        private void ParseRuleString(string srule, out string sVariable, out string sValue)
        {
            string[] svals = srule.Split('.');
            if (svals.Length != 2)
                throw new Exception("Rules should be {Variable}.{Value}");
            sVariable = svals[0];
            sValue = svals[1];
        }


        public void ApplyVariableValue(string Name, double value)
        {
            foreach(var v in Antecedents)
            {
                if(string.Compare(v.Name, Name, true) == 0)
                {
                    v.ApplyValue(value);
                    return;
                }
            }
        }

        private FuzzyValue FindAntFuzzyValue(string Name, string sValue)
        {
            foreach (var v in Antecedents)
            {
                if (string.Compare(v.Name,Name, true) == 0)
                {
                    foreach(var vv in v.Values)
                    {
                        if (string.Compare(vv.Name,sValue, true)==0)
                            return vv;
                    }
                    return null;
                }
            }
            return null;
        }

        private FuzzyValue FindContFuzzyValue(string sValue)
        {
            FuzzyValue cont = null;
            foreach (var vv in Consequent.Values)
            {
                if (string.Compare(vv.Name, sValue, true) == 0)
                {
                    cont = vv;
                    break;
                }
            }
            if (cont == null)
                throw new Exception("Value was not found!");
            return cont;
        }


        /// <summary>
        /// Performs the Centroid Defuzzification
        /// Returns a crisp number, applying formula
        /// Centroid = Sum(RulesFireString * Membership Centroid of Area) / Sum(RulesFireString)
        /// </summary>
        /// <returns></returns>
        public double DefuzzifierCentroid()
        {
            double value = 0;

            // first calculate each rule firing strength
            foreach (var r in Rules)
                r.CalcFiringStrength();
            // now calculate the result using centroid method
            double CentreOfGravityNume = 0;
            double CentreOfGravityDenom = 0;
            foreach (var r in Rules)
            {
                // use only Rules that have fired
                if(r.FireStrength > 0)
                {
                    CentreOfGravityNume += r.Consequent.MF.centroid() * r.FireStrength;
                    CentreOfGravityDenom += r.FireStrength;
                }
            }

            // avoid divide by zero
            if (CentreOfGravityDenom != 0)
                value = CentreOfGravityNume / CentreOfGravityDenom;
            else
                value = 0;

            return value;
        }

   
        public bool AddNewStringRule(string srule)
        {
            string[] sparts = srule.Split(' ');
            bool isAndRule = CheckIsAndRule(sparts);
            ParseVariables(sparts, isAndRule);
            return true;
        }

        private bool CheckIsAndRule(string[] sparts)
        {
            for (int i = 0; i < sparts.Length; i++)
            {
                if (string.Compare(sparts[i], "and", true) == 0)
                    return true;
                else if (string.Compare(sparts[i], "or", true) == 0)
                    return false;
            }
            return true;
        }

        private void ParseVariables(string[] sparts, bool isAndRule)
        {
            bool bant = true; // start with ant
            FuzzyValue vant1 = null, vant2 = null;
            FuzzyValue cont = null;
            string Set, sValue;
            for (int i = 0; i < sparts.Length; i++)
            {
                if (string.Compare(sparts[i], "then", true) == 0)
                {
                    bant = false;
                    continue;
                }
                if (sparts[i].Contains(".") == false)
                    continue;
                ParseRuleString(sparts[i], out Set, out sValue);
                if (bant)
                {
                    if (vant1 == null)
                        vant1 = FindAntFuzzyValue(Set, sValue);
                    else
                        vant2 = FindAntFuzzyValue(Set, sValue);
                }
                else
                {
                    cont = FindContFuzzyValue(sValue);
                }
            }
            if (vant1 == null && vant2 == null)
                throw new Exception("No Antecedents found!");
            if (cont == null)
                throw new Exception("No Consequent found!");
            if (isAndRule)
                AddNewAndRule(vant1, vant2, cont);
            else
                AddNewOrRule(vant1, vant2, cont);
        }
    }
}
