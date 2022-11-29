using System;

namespace Fofx
{
    [Serializable]
    public class RelationshipDescriptor : IComparable
    {
        public int ValueDefinition { get; set; }
        public SourceDescriptor Source { get; set; }

        public int CompareTo(object obj)
        {
            if(obj == null)
            {
                return 1;
            }

            RelationshipDescriptor other = obj as RelationshipDescriptor;

            if(this.ValueDefinition > other.ValueDefinition)
            {
                return 1;
            }
            else if(this.ValueDefinition == other.ValueDefinition)
            {
                return 0;
            }

            return -1;
        }
    }
}