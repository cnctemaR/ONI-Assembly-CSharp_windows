using System;
using System.Runtime.InteropServices;

namespace System.Configuration.Internal
{
	[ComVisible(false)]
	public interface IInternalConfigSystem
	{
		object GetSection(string configKey);

		void RefreshConfig(string sectionName);

		bool SupportsUserConfig { get; }
	}
}
