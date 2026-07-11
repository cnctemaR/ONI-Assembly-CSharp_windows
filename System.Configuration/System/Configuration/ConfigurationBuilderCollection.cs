using System;
using System.Configuration.Provider;
using System.Reflection;
using Unity;

namespace System.Configuration
{
	[DefaultMember("Item")]
	public class ConfigurationBuilderCollection : ProviderCollection
	{
		public ConfigurationBuilderCollection()
		{
			ThrowStub.ThrowNotSupportedException();
		}
	}
}
