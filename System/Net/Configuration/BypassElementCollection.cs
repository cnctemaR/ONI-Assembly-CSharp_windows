using System;
using System.Configuration;

namespace System.Net.Configuration
{
	[ConfigurationCollection(typeof(BypassElement), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
	public sealed class BypassElementCollection : ConfigurationElementCollection
	{
		[global::System.MonoTODO]
		public BypassElement this[int index]
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public BypassElement this[string name]
		{
			get
			{
				return (BypassElement)base[name];
			}
			set
			{
				base[name] = value;
			}
		}

		protected override bool ThrowOnDuplicate
		{
			get
			{
				return false;
			}
		}

		public void Add(BypassElement element)
		{
			this.BaseAdd(element);
		}

		public void Clear()
		{
			base.BaseClear();
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new BypassElement();
		}

		[global::System.MonoTODO("argument exception?")]
		protected override object GetElementKey(ConfigurationElement element)
		{
			if (!(element is BypassElement))
			{
				throw new ArgumentException("element");
			}
			return ((BypassElement)element).Address;
		}

		public int IndexOf(BypassElement element)
		{
			return base.BaseIndexOf(element);
		}

		public void Remove(BypassElement element)
		{
			base.BaseRemove(element);
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
