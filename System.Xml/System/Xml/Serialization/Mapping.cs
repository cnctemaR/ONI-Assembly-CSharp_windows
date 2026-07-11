using System;

namespace System.Xml.Serialization
{
	internal abstract class Mapping
	{
		internal Mapping()
		{
		}

		protected Mapping(Mapping mapping)
		{
			this.isSoap = mapping.isSoap;
		}

		internal bool IsSoap
		{
			get
			{
				return this.isSoap;
			}
			set
			{
				this.isSoap = value;
			}
		}

		private bool isSoap;
	}
}
