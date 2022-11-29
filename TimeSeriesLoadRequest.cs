using System;
using System.Collections.Generic;
using System.Text;

namespace Fofx
{
    public sealed class TimeSeriesDatabaseContext
    {
        #region Fields

        private SortedList<int, IEntityDescriptor> m_QuickEntityLookup;

        #endregion

        #region Ctor

        public TimeSeriesDatabaseContext()
        {
            m_QuickEntityLookup = new SortedList<int, IEntityDescriptor>();
        }

        #endregion

        #region Public methods

        public SortedList<int, IEntityDescriptor> EntityLookup
        {
            get { return m_QuickEntityLookup; }
        }

        public void AddQuickLookups(IEntityDescriptor[] constituents)
        {
            foreach (IEntityDescriptor constituent in constituents)
            {
                if (!m_QuickEntityLookup.ContainsKey(constituent.ID))
                {
                    m_QuickEntityLookup.Add(constituent.ID, constituent);
                }
            }
        }

        #endregion

        #region Internal methods

        internal void AddQuickLookup(IEntityDescriptor entity)
        {
            m_QuickEntityLookup[entity.ID] = entity;
        }

        #endregion
    }
}
