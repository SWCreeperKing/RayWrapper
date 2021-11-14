using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace RayWrapper.Vars
{
#pragma warning disable 660,661
    public class NumberClass
#pragma warning restore 660,661
    {
        // NumberClass made by SW_CreeperKing#5787
        // Special thanks to Number Engineer#9999 (developer of Incremental Unlimited 1 & 2 and Line Maze Idle) for math help
        // and Reinhardt#2924 for math help and teaching me the meaning of life
        public enum Format
        {
            Scientific,
            ScientificStatic,
            ScientificExtended,
            Engineering,
        }

        public static readonly float Version = .26f;
        public static bool CutOff1E = true; // format; 1e1e30 => 1ee30 
        public static int SciStaticLeng = 4;
        public static Format format = Format.Scientific;

        public static readonly NumberClass MaxValue = new(9.99, double.MaxValue);
        public static readonly NumberClass Double = new(double.MaxValue);
        public static readonly NumberClass Float = new(float.MaxValue);
        public static readonly NumberClass Long = new(long.MaxValue);
        public static readonly NumberClass Int = new(int.MaxValue);
        public static readonly NumberClass One = new(1);
        public static readonly NumberClass Zero = new();

        private static readonly Regex RegReplace = new Regex(@"e(1|1.0*)e");
        
        public double mantissa;
        public double exponent;

        public NumberClass() : this(0)
        {
        }

        public NumberClass(double mantissa = 0, double exponent = 0)
        {
            (this.mantissa, this.exponent) = (mantissa, exponent);
            Update();
        }

        public NumberClass(NumberClass nc) : this(nc.mantissa, nc.exponent)
        {
        }

        public NumberClass(string s)
        {
            if ((s = s.ToLower().Replace("ee", "e1e")).Contains('e'))
            {
                var split = s.Split('e');
                if (split.Length == 2) (mantissa, exponent) = (double.Parse(split[0]), double.Parse(split[1]));
                else
                    (mantissa, exponent) = (double.Parse(split[0]),
                        double.Parse(split[1]) * Math.Pow(10, double.Parse(split[2])));
            }
            else mantissa = double.Parse(s);

            Update();
        }

        private void Update()
        {
            mantissa = Math.Round(mantissa, 5);

            var isNeg = mantissa < 0;
            if (isNeg) mantissa = -mantissa;
            var log = (long)Math.Log10(mantissa);
            if (log < 0) log = (long)Math.Max(0, Math.Min(log, exponent));
            mantissa /= Math.Pow(10, log);
            if (isNeg) mantissa = -mantissa;
            exponent += log;

            if (mantissa is <= 0 or >= 1 || exponent < 1) return;
            exponent--;
            mantissa *= 10;
        }

        public static NumberClass operator +(NumberClass n1, NumberClass n2)
        {
            var delta = Math.Abs(n1.exponent - n2.exponent);
            return delta switch
            {
                > 12 => n1.Max(n2),
                0 => new NumberClass(n1.mantissa + n2.mantissa, n1.exponent),
                _ => n1 > n2
                    ? new NumberClass(n1.GetSignedMantissa() + n2.GetSignedMantissa() / Math.Pow(10, delta),
                        n1.exponent)
                    : new NumberClass(n2.GetSignedMantissa() + n1.GetSignedMantissa() / Math.Pow(10, delta),
                        n2.exponent)
            };
        }

        public static NumberClass operator -(NumberClass n1, NumberClass n2) =>
            n1 + new NumberClass(-n2.mantissa, n2.exponent);

        public static NumberClass operator *(NumberClass n1, NumberClass n2) =>
            n1 == Zero || n2 == Zero
                ? Zero
                : n1 == One || n2 == One
                    ? n1 == One
                        ? n2
                        : n1
                    : new NumberClass(n1.mantissa * n2.mantissa, n1.exponent + n2.exponent);

        public static NumberClass operator /(NumberClass n1, NumberClass n2) =>
            n2 == Zero
                ? throw new DivideByZeroException("NumberClass: Can not divide by 0")
                : n1 == Zero
                    ? Zero
                    : n2 == One
                        ? n1
                        : new NumberClass(n1.mantissa / n2.mantissa, n1.exponent - n2.exponent);

        public NumberClass Pow(NumberClass n)
        {
            if (n == One || this == One || this == Zero) return this;
            if (n == Zero) return One;
            if (exponent == 0 && n.exponent == 0) return Math.Pow(mantissa, n.mantissa);

            var tempExpo = exponent + Math.Log10(mantissa);
            if (Math.Max(Math.Log10(exponent), 0) + n.exponent < 300)
            {
                tempExpo *= n.GetRealMantissa();
                return tempExpo < 1e17
                    ? new NumberClass(Math.Pow(10, tempExpo % 1), Math.Floor(tempExpo))
                    : new NumberClass(mantissa, tempExpo);
            }

            tempExpo = Math.Log10(tempExpo);
            tempExpo += n.exponent + Math.Log10(n.exponent);
            return new NumberClass(mantissa, tempExpo);
        }

        public NumberClass Root(long @base)
        {
            var mod = exponent % @base;
            return new NumberClass(Math.Pow(mantissa * Math.Pow(10, mod), 1f / @base), (exponent - mod) / @base);
        }

        public NumberClass Sqrt() => Root(2);
        public NumberClass Cbrt() => Root(3);
        public NumberClass Log10() => exponent + Math.Log10(mantissa);
        public NumberClass Log(NumberClass @base) => this == Zero ? Zero : Log10() / @base.Log10();
        public NumberClass Log2() => Log(2);
        public static NumberClass operator ++(NumberClass n) => n += One;
        public static NumberClass operator --(NumberClass n) => n -= One;

        private int BaseNegComp(NumberClass n) =>
            !isNeg() && n.isNeg()
                ? 1
                : isNeg() && !n.isNeg()
                    ? -1
                    : 0;

        public static bool operator >(NumberClass n1, NumberClass n2) =>
            n1.BaseNegComp(n2) >= 0 &&
            (n1.exponent > n2.exponent || n1.exponent == n2.exponent && n1.mantissa > n2.mantissa);

        public static bool operator <(NumberClass n1, NumberClass n2) =>
            n1.BaseNegComp(n2) <= 0 &&
            (n1.exponent < n2.exponent || n1.exponent == n2.exponent && n1.mantissa < n2.mantissa);

        public static bool operator ==(NumberClass n1, NumberClass n2) =>
            n1.BaseNegComp(n2) != 0 ? false : n1.mantissa == n2.mantissa && n1.exponent == n2.exponent;

        public static bool operator !=(NumberClass n1, NumberClass n2) =>
            n1.mantissa != n2.mantissa || n1.exponent != n2.exponent;

        public static bool operator >=(NumberClass n1, NumberClass n2) => n1 == n2 || n1 > n2;
        public static bool operator <=(NumberClass n1, NumberClass n2) => n1 == n2 || n1 < n2;

        public static implicit operator NumberClass(double d) => new(d);
        public static implicit operator NumberClass(string s) => new(s);

        public static explicit operator int(NumberClass n) =>
            (int)(n > Int ? int.MaxValue : n.mantissa * Math.Pow(10, n.exponent));

        public static explicit operator long(NumberClass n) =>
            (long)(n > Long ? long.MaxValue : n.mantissa * Math.Pow(10, n.exponent));

        public static explicit operator double(NumberClass n) =>
            n > Double
                ? double.MaxValue
                : n.mantissa * Math.Pow(10, n.exponent);

        public static explicit operator float(NumberClass n) =>
            (float)(n > Float ? float.MaxValue : n.mantissa * Math.Pow(10, n.exponent));

        public double GetRealMantissa() => exponent > 308 ? mantissa : mantissa * Math.Pow(10, exponent);
        public double GetSignedMantissa() => mantissa;
        public NumberClass Ceiling() => new(Math.Ceiling(mantissa), exponent);
        public NumberClass Floor() => new(Math.Floor(mantissa), exponent);
        public NumberClass Round() => new(Math.Round(mantissa), exponent);
        public NumberClass Max(NumberClass n) => n > this ? n : this;
        public NumberClass Min(NumberClass n) => n < this ? n : this;
        public NumberClass Abs() => new(Math.Abs(mantissa), exponent);
        public NumberClass Clone() => new(mantissa, exponent);
        public bool IsNaN() => double.IsNaN(mantissa) || double.IsNaN(exponent);
        public bool isNeg() => mantissa < 0;
        public override string ToString() => FormatNc(format);
        public string ToString(Func<double, double, string> format) => format.Invoke(mantissa, exponent);

        // mantissa becomes useless once exponent precision is lost
        public bool IsMantissaUseless() => Math.Log10(exponent) >= 15;

        public string FormatNc(Format format)
        {
            if (exponent < 5) return $"{mantissa * Math.Pow(10, exponent):#,##0.##}";
            var useMan = !IsMantissaUseless(); // if take mantissa or leave it

            // does not catch engineering but w.e. is probs fine
            string CutOff1Check(string s) => !CutOff1E ? s : RegReplace.Replace(s, "ee");

            // get proper format
            // can be #.000 or #.### 
            string GetFormatFromCount(int count, bool optional = false) =>
                $"#.{string.Join("", Enumerable.Repeat(optional ? '#' : '0', count))}";

            string formatMantissa;
            string formatExponent;
            switch (format)
            {
                case Format.Engineering:
                    var extended = exponent % 3;
                    formatMantissa = useMan ? $"{mantissa * Math.Pow(10, extended):##0.00#}" : "";
                    formatExponent = new NumberClass(exponent - extended).FormatNc(Format.Engineering)
                        .Replace("1e", "e");
                    return CutOff1Check($"{formatMantissa}e{formatExponent}");
                case Format.ScientificStatic:
                    // format to keep numclass the same leng
                    formatExponent = new NumberClass(exponent).FormatNc(Format.Scientific);
                    formatMantissa = useMan ? $"{mantissa.ToString(GetFormatFromCount(SciStaticLeng))}" : "";
                    return CutOff1Check($"{formatMantissa}e{formatExponent}");
                default:
                    formatMantissa = useMan
                        ? $"{mantissa.ToString(GetFormatFromCount(format != Format.ScientificExtended ? 2 : 6))}"
                        : "";
                    formatExponent = new NumberClass(exponent).FormatNc(Format.Scientific);
                    return CutOff1Check($"{formatMantissa}e{formatExponent}");
            }
        }
    }
}