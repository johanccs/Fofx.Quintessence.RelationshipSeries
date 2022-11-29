using System;
using System.Collections.Generic;

namespace Fofx
{
    [Serializable]
    public class Revision<T> : IEnumerable<KeyValuePair<DateTime, T>>
      where T : class, IDataPoint
    {
        #region Ctor
        internal Revision(DateTime valueDate, int capacity): this(valueDate)
        {
            
        }

        public Revision(DateTime valueDate)
        {
            ValueDate = valueDate;
        }

        #endregion

        #region Properties

        public DateTime ValueDate { get; set; }

        public DateTime Min { get; set; }

        public DateTime Max { get; set; }

        public IList<DateTime> Dates { get; set; }

        public int Count { get; set; }

        #endregion

        public void Add(DateTime date, T val)
            => throw new NotImplementedException();

        public bool TryGetValue(DateTime iaad, out T value)
            => throw new NotImplementedException();

        public T GetValue(DateTime declarationDate)
            => throw new NotImplementedException();

        internal Revision<T> Filter(NonKeyedAttributeSet nonKeyAttributes)
            => throw new NotImplementedException();

        public T this[DateTime declarationDate]
            => throw new NotImplementedException();

        public bool Contains(DateTime date)
            => throw new NotImplementedException();

        #region IEnumerable<KeyValuePair<DateTime,T>> Members

        public IEnumerator<KeyValuePair<DateTime, T>> GetEnumerator()
            => throw new NotImplementedException();

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => throw new NotImplementedException();

        #endregion
    }
}