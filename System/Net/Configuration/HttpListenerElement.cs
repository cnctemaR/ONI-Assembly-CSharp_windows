using System;
using System.Configuration;
using Unity;

namespace System.Net.Configuration
{
	public sealed class HttpListenerElement : ConfigurationElement
	{
		public HttpListenerElement()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public HttpListenerTimeoutsElement Timeouts
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		public bool UnescapeRequestUrl
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return default(bool);
			}
		}
	}
}
