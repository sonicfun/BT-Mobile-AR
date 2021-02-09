using System;

namespace Fuzzy
{
    public class TrapezoidMemberShip : IMembership
    {
        private double a, b, c, d;
        double value;

        public TrapezoidMemberShip(double a, double b, double c, double d)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            if (!(a <= b && b <= c && c <= d))
                throw new Exception("Number should be in ascending order!");
            value = 0;
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
            if(v > d)
            {
                value = 0;
                return;
            }
            if( v>=b && v <= c)
            {
                value = 1;
                return;
            }
            if (v < b)
            {
                value = (v - a) / (b - a);
            }
            else // v  < d && v > c
            {
                value = (d - v) / (d - c);
            }
        }

        public double centroid()
        {
            double A1 = (b - a) * 1 / 2;
            double A2 = (c - b) * 1;
            double A3 = (d - c) * 1 / 2;

            double C1 = b - (b - a) / 3;
            double C2 = b + (c - b) / 2;
            double C3 = c + (d - c) / 3;
            double Ca = (A1 * C1 + A2 * C2 + A3 * C3) / (A1 + A2 + A3);

            return Ca;
        }
    }
}
