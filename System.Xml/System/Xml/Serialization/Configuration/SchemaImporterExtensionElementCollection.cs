using System;
using System.Configuration;

namespace System.Xml.Serialization.Configuration
{
	[ConfigurationCollection(typeof(SchemaImporterExtensionElement))]
	public sealed class SchemaImporterExtensionElementCollection : ConfigurationElementCollection
	{
		public SchemaImporterExtensionElement this[int index]
		{
			get
			{
				return (SchemaImporterExtensionElement)base.BaseGet(index);
			}
			set
			{
				if (base.BaseGet(index) != null)
				{
					base.BaseRemoveAt(index);
				}
				this.BaseAdd(index, value);
			}
		}

		public SchemaImporterExtensionElement this[string name]
		{
			get
			{
				return (SchemaImporterExtensionElement)base.BaseGet(name);
			}
			set
			{
				if (base.BaseGet(name) != null)
				{
					base.BaseRemove(name);
				}
				this.BaseAdd(value);
			}
		}

		public void Add(SchemaImporterExtensionElement element)
		{
			this.BaseAdd(element);
		}

		public void Clear()
		{
			base.BaseClear();
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new SchemaImporterExtensionElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((SchemaImporterExtensionElement)element).Key;
		}

		public int IndexOf(SchemaImporterExtensionElement element)
		{
			return base.BaseIndexOf(element);
		}

		public void Remove(SchemaImporterExtensionElement element)
		{
			base.BaseRemove(element.Key);
		}

		public void Remove(string name)
		{
			base.BaseRemove(name);
		}

		public void RemoveAt(int index)
		{
			base.BaseRemoveAt(index);
		}
	}
}
