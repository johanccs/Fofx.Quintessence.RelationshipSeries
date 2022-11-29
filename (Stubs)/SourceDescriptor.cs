using System;

namespace Fofx
{
    [Serializable]
    public class SourceDescriptor : IComparable
    {
        public int SourceID { get; }

        public Preference CodePreference { get; set; }

        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;

            SourceDescriptor other = obj as SourceDescriptor;

            if (this.SourceID > other.SourceID)
            {
                return 1;
            }
            else if (this.SourceID == other.SourceID)
            {
                return 0;
            }

            return -1;

        }
    }
}