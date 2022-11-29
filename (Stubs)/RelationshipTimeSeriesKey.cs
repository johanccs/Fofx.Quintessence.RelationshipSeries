using System;

namespace Fofx
{
    [Serializable]
    public class RelationshipTimeSeriesKey : TimeSeriesKey, IComparable<RelationshipTimeSeriesKey>
    {
        public RelationshipDescriptor Relationship { get; }

        public int CompareTo(RelationshipTimeSeriesKey other)
        {
            if(other == null)
            {
                return 1;
            }

            if(Relationship == other.Relationship)
            {
                return 1;
            }

            return -1;
        }
    }
}
