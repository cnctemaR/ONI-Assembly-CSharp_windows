using System;
using Unity;

namespace System.Configuration
{
	public sealed class ContextInformation
	{
		internal ContextInformation(Configuration config, object ctx)
		{
			this.ctx = ctx;
			this.config = config;
		}

		public object GetSection(string sectionName)
		{
			return this.config.GetSection(sectionName);
		}

		public object HostingContext
		{
			get
			{
				return this.ctx;
			}
		}

		[MonoInternalNote("should this use HostingContext instead?")]
		public bool IsMachineLevel
		{
			get
			{
				return this.config.ConfigPath == "machine";
			}
		}

		internal ContextInformation()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private object ctx;

		private Configuration config;
	}
}
