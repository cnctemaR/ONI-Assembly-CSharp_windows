using System;
using System.Collections.Generic;

namespace System.Runtime.Serialization
{
	public sealed class ExtensionDataObject
	{
		internal ExtensionDataObject()
		{
		}

		internal IList<ExtensionDataMember> Members
		{
			get
			{
				return this.members;
			}
			set
			{
				this.members = value;
			}
		}

		private IList<ExtensionDataMember> members;
	}
}
