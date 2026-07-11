using System;
using System.Configuration.Internal;

namespace System.Configuration
{
	internal class InternalConfigurationSystem : IConfigSystem
	{
		public void Init(Type typeConfigHost, params object[] hostInitParams)
		{
			this.hostInitParams = hostInitParams;
			this.host = (IInternalConfigHost)Activator.CreateInstance(typeConfigHost);
			this.root = new InternalConfigurationRoot();
			this.root.Init(this.host, false);
		}

		public void InitForConfiguration(ref string locationConfigPath, out string parentConfigPath, out string parentLocationConfigPath)
		{
			this.host.InitForConfiguration(ref locationConfigPath, out parentConfigPath, out parentLocationConfigPath, this.root, this.hostInitParams);
		}

		public IInternalConfigHost Host
		{
			get
			{
				return this.host;
			}
		}

		public IInternalConfigRoot Root
		{
			get
			{
				return this.root;
			}
		}

		private IInternalConfigHost host;

		private IInternalConfigRoot root;

		private object[] hostInitParams;
	}
}
