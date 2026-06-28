using System;
using System.Collections;
using System.Xml;

namespace System.Configuration
{
	public class DictionarySectionHandler : IConfigurationSectionHandler
	{
		public virtual object Create(object parent, object context, XmlNode section)
		{
			return ConfigHelper.GetDictionary(parent as IDictionary, section, this.KeyAttributeName, this.ValueAttributeName);
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
