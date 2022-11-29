using System;
using System.Collections.Generic;

namespace Fofx
{
    [Serializable]
    public class EntityDescriptor : IEntityDescriptor
    {
        #region Proprties


        public int ID => default;

        public string Code { get; set; }

        public int CodeTypeID { get; set; }

        public string CodeType { get; set; }

        public string FullCode { get; set; }

        public string Value { get; set; }

        public bool IsVirtual { get; set; }

        public bool IsDefault { get; set; }

        public IDictionary<string, string> Properties { get; set; }

        #endregion

        #region Ctor

        public EntityDescriptor(int id) 
        { 
        }

        #endregion

        #region Methods

        public IEntityDescriptor Clone()
        {
            throw new NotImplementedException();
        }

        public int CompareTo(IEntityDescriptor other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        public IEntityDescriptor CreateAlias(string alias)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}