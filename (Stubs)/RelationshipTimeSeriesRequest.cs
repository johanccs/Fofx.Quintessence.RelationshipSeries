using System;

namespace Fofx
{
    [Serializable]
    public class RelationshipTimeSeriesRequest : IComparable, IComparable<RelationshipTimeSeriesRequest>
    {
        public RelationshipDescriptor Relationship { get; }
        public TimeSeriesDescriptor TimeSeries { get; }
        public ITimeSeriesIterator Iterator { get; }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            RelationshipTimeSeriesRequest relationshipTimeSeriesRequest = obj as RelationshipTimeSeriesRequest;

            if (this.Relationship == relationshipTimeSeriesRequest.Relationship 
                && this.TimeSeries == relationshipTimeSeriesRequest.TimeSeries 
                && this.Iterator == relationshipTimeSeriesRequest.Iterator)
            {
                return 1;
            }

            return -1;
        }

        public int CompareTo(RelationshipTimeSeriesRequest other)
        {
            if (this.Relationship == other.Relationship
                && this.TimeSeries == other.TimeSeries
                && this.Iterator == other.Iterator)
            {
                return 1;
            }

            return -1;
        }
    }
}