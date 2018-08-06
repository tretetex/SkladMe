using System;

namespace SkladMe.Common.Model
{
    public struct Range<T>
    {
        public T From { get; set; }
        public T To { get; set; }
    }

    public static class Range
    {
        public static bool IsInclude(Range<int?> range, int? value)
        {
            if (range.From != null)
            {
                if (value < range.From) { return false; }
            }
            if (range.To != null)
            {
                if (value > range.To) { return false; }
            }

            return true;
        }

        public static bool IsInclude(Range<DateTime?> range, DateTime? value)
        {
            if (range.From != null)
            {
                if (value < range.From) { return false; }
            }
            if (range.To != null)
            {
                if (value > range.To) { return false; }
            }

            return true;
        }

        public static bool IsInclude(Range<double?> range, double? value)
        {
            if (range.From != null)
            {
                if (value < range.From) { return false; }
            }
            if (range.To != null)
            {
                if (value > range.To) { return false; }
            }

            return true;
        }

        public static Range<int?> New(int? from, int? to)
        {
            var structTemp = new Range<int?>();

            if (from < 0)
            {
                from = 0;
            }
            if (from > to)
            {
                to = from;
            }

            structTemp.From = from;
            structTemp.To = to;

            return structTemp;
        }

        public static Range<double?> New(double? from, double? to)
        {
            var structTemp = new Range<double?>();

            if (from < 0)
            {
                from = 0;
            }
            if (from > to)
            {
                to = from;
            }

            structTemp.From = from;
            structTemp.To = to;

            return structTemp;
        }

        public static Range<DateTime?> New(DateTime? from, DateTime? to)
        {
            var structTemp = new Range<DateTime?>();

            if (from > to)
            {
                to = from;
            }

            structTemp.From = from;
            structTemp.To = to;

            return structTemp;
        }
    }
}
