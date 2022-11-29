using System;

namespace Fofx
{
    [Serializable]
    public class Preference
    {
        public int Id { get; }

        public int[] Order { get; }

        public Preference(int id, int[] order)
        {
            Id = id;
            Order = order;
        }
    }
}