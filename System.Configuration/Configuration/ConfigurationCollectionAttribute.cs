using System;

namespace System.Configuration
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
	public sealed class ConfigurationCollectionAttribute : Attribute
	{
		public ConfigurationCollectionAttribute(Type itemType)
		{
			this.itemType = itemType;
		}

		public string AddItemName
		{
			get
			{
				return this.addItemName;
			}
			set
			{
				this.addItemName = value;
			}
		}

		public string ClearItemsName
		{
			get
			{
				return this.clearItemsName;
			}
			set
			{
				this.clearItemsName = value;
			}
		}

		public string RemoveItemName
		{
			get
			{
				return this.removeItemName;
			}
			set
			{
				this.removeItemName = value;
			}
		}

		public ConfigurationElementCollectionType CollectionType
		{
			get
			{
				return this.collectionType;
			}
			set
			{
				this.collectionType = value;
			}
		}

		[MonoInternalNote("Do something with this in ConfigurationElementCollection")]
		public Type ItemType
		{
			get
			{
				return this.itemType;
			}
		}

		private string addItemName = "add";

		private string clearItemsName = "clear";

		private string removeItemName = "remove";

		private ConfigurationElementCollectionType collectionType;

		private Type itemType;
	}
}
