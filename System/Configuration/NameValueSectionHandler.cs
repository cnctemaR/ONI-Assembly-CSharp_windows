using System;
using System.Collections.Specialized;
using System.Xml;

namespace System.Configuration
{
	public class NameValueSectionHandler : IConfigurationSectionHandler
	{
		public object Create(object parent, object context, XmlNode section)
		{
			return ConfigHelper.GetNameValueCollection(parent as NameValueCollection, section, this.KeyAttributeName, this.ValueAttributeName);
		}

		protected virtual string KeyAttributeName
		{
			get
			{
				return "key";
			}
		}

		protected virtual string ValueAttributeName
		{
			get
			{
				return "value";
			}
		}
	}
}
