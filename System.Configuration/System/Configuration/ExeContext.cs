using System;
using Unity;

namespace System.Configuration
{
	public sealed class ExeContext
	{
		internal ExeContext(string path, ConfigurationUserLevel level)
		{
			this.path = path;
			this.level = level;
		}

		public string ExePath
		{
			get
			{
				return this.path;
			}
		}

		public ConfigurationUserLevel UserLevel
		{
			get
			{
				return this.level;
			}
		}

		internal ExeContext()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private string path;

		private ConfigurationUserLevel level;
	}
}
