using System;
using System.Configuration;

namespace System.Net.Configuration
{
	[ConfigurationCollection(typeof(WebRequestModuleElement), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
	public sealed class WebRequestModuleElementCollection : ConfigurationElementCollection
	{
		[global::System.MonoTODO]
		public WebRequestModuleElement this[int index]
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

		[global::System.MonoTODO]
		public WebRequestModuleElement this[string name]
		{
			get
			{
				return (WebRequestModuleElement)base[name];
			}
			set
			{
				base[name] = value;
			}
		}

		public void Add(WebRequestModuleElement element)
		{
			this.BaseAdd(element);
		}

		public void Clear()
		{
			base.BaseClear();
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new WebRequestModuleElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			if (!(element is WebRequestModuleElement))
			{
				throw new ArgumentException("element");
			}
			return ((WebRequestModuleElement)element).Prefix;
		}

		public int IndexOf(WebRequestModuleElement element)
		{
			return base.BaseIndexOf(element);
		}

		public void Remove(WebRequestModuleElement element)
		{
			base.BaseRemove(element.Prefix);
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
