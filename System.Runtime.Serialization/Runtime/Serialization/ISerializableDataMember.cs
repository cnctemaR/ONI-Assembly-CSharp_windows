using System;

namespace System.Runtime.Serialization
{
	internal class ISerializableDataMember
	{
		internal string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		internal IDataNode Value
		{
			get
			{
				return this.value;
			}
			set
			{
				this.value = value;
			}
		}

		private string name;

		private IDataNode value;
	}
}
