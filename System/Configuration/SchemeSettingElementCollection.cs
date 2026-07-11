using System;
using Unity;

namespace System.Configuration
{
	[ConfigurationCollection(typeof(SchemeSettingElement), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap, AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
	public sealed class SchemeSettingElementCollection : ConfigurationElementCollection
	{
		public SchemeSettingElementCollection()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public SchemeSettingElement this[int index]
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		protected override ConfigurationElement CreateNewElement()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return null;
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return null;
		}

		public int IndexOf(SchemeSettingElement element)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return 0;
		}
	}
}
