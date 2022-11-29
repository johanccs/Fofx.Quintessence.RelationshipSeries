using System;

namespace Fofx
{
    [Serializable]
    public class TimeSeriesRequest : IComparable, IComparable<TimeSeriesRequest>
    {
        public ITimeSeriesIterator Iterator { get; }
        public TimeSeriesDescriptor TimeSeries { get; }

        public int CompareTo(object obj)
        {
            if(obj == null)
            {
                return 1;
            }

            TimeSeriesRequest other = obj as TimeSeriesRequest;

            if(this.Iterator == other.Iterator && this.TimeSeries == other.TimeSeries)
            {
                return 1;
            }

            return -1;
        }

        public int CompareTo(TimeSeriesRequest other)
        {
            if (this.Iterator == other.Iterator && this.TimeSeries == other.TimeSeries)
            {
                return 1;
            }

            return -1;
        }
    }
}
