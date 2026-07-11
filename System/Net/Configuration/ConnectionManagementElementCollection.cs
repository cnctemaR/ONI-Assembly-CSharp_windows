using System;
using System.Configuration;

namespace System.Net.Configuration
{
	[ConfigurationCollection(typeof(ConnectionManagementElement), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
	public sealed class ConnectionManagementElementCollection : ConfigurationElementCollection
	{
		[MonoTODO]
		public ConnectionManagementElement this[int index]
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

		public ConnectionManagementElement this[string name]
		{
			get
			{
				return (ConnectionManagementElement)base[name];
			}
			set
			{
				base[name] = value;
			}
		}

		public void Add(ConnectionManagementElement element)
		{
			this.BaseAdd(element);
		}

		public void Clear()
		{
			base.BaseClear();
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new ConnectionManagementElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			if (!(element is ConnectionManagementElement))
			{
				throw new ArgumentException("element");
			}
			return ((ConnectionManagementElement)element).Address;
		}

		public int IndexOf(ConnectionManagementElement element)
		{
			return base.BaseIndexOf(element);
		}

		public void Remove(ConnectionManagementElement element)
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
