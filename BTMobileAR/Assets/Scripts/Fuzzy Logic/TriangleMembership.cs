using System;

namespace Fuzzy
{
    public class TriangleMembership : IMembership
    {
        private double a, b, c;
        private double value;
        private double _centroid;

        public TriangleMembership(double a, double b, double c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.value = 0;
            if (!(a <= b && b <= c))
                throw new Exception("Numbers should be in ascending order");
            _centroid = calccentroid();
        }
     

        public double getMembershipValue()
        {
            return value;
        }

        public void setMembershipValue(double v)
        {
            if (v < a)
            {
                value = 0;
                return;
            }
            if (v > c)
            {
                value = 0;
                return;
            }
            if (v < b)
            {
                value = 1 - (b - v) / (b-a);
            }
            else // v  < d
            {
                value = 1 - (v - b) / (c-a);
            }
        }

        public double centroid()
        {
            return _centroid;
        }
        public double calccentroid()
        {
            double A1 = (b - a) * 1 / 2;
            double A2 = (c - b) * 1 / 2;
            double C1 = b - (b - a) / 3;
            double C2 = b + (c - b) / 3;
            double C = (A1 * C1 + A2 * C2) / (A1 + A2);
            return C;
        }
    }
}
