using System;
using System.Configuration;
using Unity;

namespace System.Net.Configuration
{
	public sealed class WindowsAuthenticationElement : ConfigurationElement
	{
		public WindowsAuthenticationElement()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public int DefaultCredentialsHandleCacheSize
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return 0;
			}
			set
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
			}
		}
	}
}
