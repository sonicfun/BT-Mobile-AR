using System.Collections.Generic;

namespace Fuzzy
{
    public class FuzzyVariable
    {
        private List<FuzzyValue> _values;
        public string Name { get; set; }
        public List<FuzzyValue> Values
        { get { return _values; } }

        public FuzzyVariable()
        {
            _values = new List<FuzzyValue>();
        }

        public void AddValue(FuzzyValue val)
        {
            _values.Add(val);
            val.Parent = this;
        }

        public void ApplyValue(double val)
        {
            foreach (var v in _values)
                v.MF.setMembershipValue(val);
        }
    }
}
