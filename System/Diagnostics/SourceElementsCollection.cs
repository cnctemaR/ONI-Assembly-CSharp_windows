using System;
using System.Configuration;

namespace System.Diagnostics
{
	[ConfigurationCollection(typeof(SourceElement), AddItemName = "source", CollectionType = ConfigurationElementCollectionType.BasicMap)]
	internal class SourceElementsCollection : ConfigurationElementCollection
	{
		public SourceElement this[string name]
		{
			get
			{
				return (SourceElement)base.BaseGet(name);
			}
		}

		protected override string ElementName
		{
			get
			{
				return "source";
			}
		}

		public override ConfigurationElementCollectionType CollectionType
		{
			get
			{
				return ConfigurationElementCollectionType.BasicMap;
			}
		}

		protected override ConfigurationElement CreateNewElement()
		{
			SourceElement sourceElement = new SourceElement();
			sourceElement.Listeners.InitializeDefaultInternal();
			return sourceElement;
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((SourceElement)element).Name;
		}
	}
}
