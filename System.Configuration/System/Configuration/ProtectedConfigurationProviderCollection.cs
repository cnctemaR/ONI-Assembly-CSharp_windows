using System;
using System.Configuration.Provider;

namespace System.Configuration
{
	public class ProtectedConfigurationProviderCollection : ProviderCollection
	{
		[MonoTODO]
		public ProtectedConfigurationProvider this[string name]
		{
			get
			{
				return (ProtectedConfigurationProvider)base[name];
			}
		}

		[MonoTODO]
		public override void Add(ProviderBase provider)
		{
			base.Add(provider);
		}
	}
}
