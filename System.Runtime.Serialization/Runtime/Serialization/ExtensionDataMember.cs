using System;

namespace System.Runtime.Serialization
{
	internal class ExtensionDataMember
	{
		public string Name
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

		public string Namespace
		{
			get
			{
				return this.ns;
			}
			set
			{
				this.ns = value;
			}
		}

		public IDataNode Value
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

		public int MemberIndex
		{
			get
			{
				return this.memberIndex;
			}
			set
			{
				this.memberIndex = value;
			}
		}

		private string name;

		private string ns;

		private IDataNode value;

		private int memberIndex;
	}
}
