using System;
using System.Runtime.InteropServices;

namespace System.Configuration.Internal
{
	[ComVisible(false)]
	public interface IInternalConfigRoot
	{
		event InternalConfigEventHandler ConfigChanged;

		event InternalConfigEventHandler ConfigRemoved;

		IInternalConfigRecord GetConfigRecord(string configPath);

		object GetSection(string section, string configPath);

		string GetUniqueConfigPath(string configPath);

		IInternalConfigRecord GetUniqueConfigRecord(string configPath);

		void Init(IInternalConfigHost host, bool isDesignTime);

		bool IsDesignTime { get; }

		void RemoveConfig(string configPath);
	}
}
