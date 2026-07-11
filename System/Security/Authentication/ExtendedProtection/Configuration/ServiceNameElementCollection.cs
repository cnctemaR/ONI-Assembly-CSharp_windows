using System;
using System.Configuration;

namespace System.Security.Authentication.ExtendedProtection.Configuration
{
	[ConfigurationCollection(typeof(ServiceNameElement))]
	public sealed class ServiceNameElementCollection : ConfigurationElementCollection
	{
		public ServiceNameElement this[int index]
		{
			get
			{
				return (ServiceNameElement)base.BaseGet(index);
			}
		}

		public ServiceNameElement this[string name]
		{
			get
			{
				return (ServiceNameElement)base.BaseGet(name);
			}
		}

		public void Add(ServiceNameElement element)
		{
			throw new NotImplementedException();
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new ServiceNameElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			return ((ServiceNameElement)element).Name;
		}

		public int IndexOf(ServiceNameElement element)
		{
			throw new NotImplementedException();
		}

		public void Remove(string name)
		{
			throw new NotImplementedException();
		}

		public void Remove(ServiceNameElement element)
		{
			throw new NotImplementedException();
		}

		public void RemoveAt(int index)
		{
			throw new NotImplementedException();
		}
	}
}
