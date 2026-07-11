using System;
using System.Collections;
using System.Collections.Specialized;

namespace System.Configuration
{
	internal class ConfigInfoCollection : NameObjectCollectionBase
	{
		public ConfigInfoCollection()
			: base(StringComparer.Ordinal)
		{
		}

		public ICollection AllKeys
		{
			get
			{
				return this.Keys;
			}
		}

		public ConfigInfo this[string name]
		{
			get
			{
				return (ConfigInfo)base.BaseGet(name);
			}
			set
			{
				base.BaseSet(name, value);
			}
		}

		public ConfigInfo this[int index]
		{
			get
			{
				return (ConfigInfo)base.BaseGet(index);
			}
			set
			{
				base.BaseSet(index, value);
			}
		}

		public void Add(string name, ConfigInfo config)
		{
			base.BaseAdd(name, config);
		}

		public void Clear()
		{
			base.BaseClear();
		}

		public string GetKey(int index)
		{
			return base.BaseGetKey(index);
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
