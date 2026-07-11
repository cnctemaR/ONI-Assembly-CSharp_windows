using System;
using System.Collections.ObjectModel;

namespace System.Runtime.Serialization
{
	public class ExportOptions
	{
		public IDataContractSurrogate DataContractSurrogate
		{
			get
			{
				return this.dataContractSurrogate;
			}
			set
			{
				this.dataContractSurrogate = value;
			}
		}

		internal IDataContractSurrogate GetSurrogate()
		{
			return this.dataContractSurrogate;
		}

		public Collection<Type> KnownTypes
		{
			get
			{
				if (this.knownTypes == null)
				{
					this.knownTypes = new Collection<Type>();
				}
				return this.knownTypes;
			}
		}

		private Collection<Type> knownTypes;

		private IDataContractSurrogate dataContractSurrogate;
	}
}
