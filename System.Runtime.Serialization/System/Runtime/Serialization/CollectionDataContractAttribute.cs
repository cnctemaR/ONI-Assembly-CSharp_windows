using System;

namespace System.Runtime.Serialization
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
	public sealed class CollectionDataContractAttribute : Attribute
	{
		public string Namespace
		{
			get
			{
				return this.ns;
			}
			set
			{
				this.ns = value;
				this.isNamespaceSetExplicitly = true;
			}
		}

		public bool IsNamespaceSetExplicitly
		{
			get
			{
				return this.isNamespaceSetExplicitly;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
				this.isNameSetExplicitly = true;
			}
		}

		public bool IsNameSetExplicitly
		{
			get
			{
				return this.isNameSetExplicitly;
			}
		}

		public string ItemName
		{
			get
			{
				return this.itemName;
			}
			set
			{
				this.itemName = value;
				this.isItemNameSetExplicitly = true;
			}
		}

		public bool IsItemNameSetExplicitly
		{
			get
			{
				return this.isItemNameSetExplicitly;
			}
		}

		public string KeyName
		{
			get
			{
				return this.keyName;
			}
			set
			{
				this.keyName = value;
				this.isKeyNameSetExplicitly = true;
			}
		}

		public bool IsReference
		{
			get
			{
				return this.isReference;
			}
			set
			{
				this.isReference = value;
				this.isReferenceSetExplicitly = true;
			}
		}

		public bool IsReferenceSetExplicitly
		{
			get
			{
				return this.isReferenceSetExplicitly;
			}
		}

		public bool IsKeyNameSetExplicitly
		{
			get
			{
				return this.isKeyNameSetExplicitly;
			}
		}

		public string ValueName
		{
			get
			{
				return this.valueName;
			}
			set
			{
				this.valueName = value;
				this.isValueNameSetExplicitly = true;
			}
		}

		public bool IsValueNameSetExplicitly
		{
			get
			{
				return this.isValueNameSetExplicitly;
			}
		}

		private string name;

		private string ns;

		private string itemName;

		private string keyName;

		private string valueName;

		private bool isReference;

		private bool isNameSetExplicitly;

		private bool isNamespaceSetExplicitly;

		private bool isReferenceSetExplicitly;

		private bool isItemNameSetExplicitly;

		private bool isKeyNameSetExplicitly;

		private bool isValueNameSetExplicitly;
	}
}
