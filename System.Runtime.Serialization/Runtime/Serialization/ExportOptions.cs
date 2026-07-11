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
				return this.surrogate;
			}
			set
			{
				this.surrogate = value;
			}
		}

		public Collection<Type> KnownTypes
		{
			get
			{
				return this.known_types;
			}
		}

		private IDataContractSurrogate surrogate;

		private KnownTypeCollection known_types;
	}
}
