using System;

namespace System.Runtime.Serialization
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
	public sealed class CollectionDataContractAttribute : Attribute
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

		public string ItemName
		{
			get
			{
				return this.item_name;
			}
			set
			{
				this.item_name = value;
			}
		}

		public string KeyName
		{
			get
			{
				return this.key_name;
			}
			set
			{
				this.key_name = value;
			}
		}

		public string ValueName
		{
			get
			{
				return this.value_name;
			}
			set
			{
				this.value_name = value;
			}
		}

		public bool IsReference { get; set; }

		private string name;

		private string ns;

		private string item_name;

		private string key_name;

		private string value_name;

		private bool is_reference;
	}
}
