using System;
using System.Net;

namespace System.Xml
{
	internal class XmlNullResolver : XmlResolver
	{
		private XmlNullResolver()
		{
		}

		public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
		{
			throw new XmlException("Resolving of external URIs was prohibited.", string.Empty);
		}

		public override ICredentials Credentials
		{
			set
			{
			}
		}

		public static readonly XmlNullResolver Singleton = new XmlNullResolver();
	}
}
