using System;

namespace UnityEngine.UIElements
{
	[Obsolete("IDataWatchService is no longer supported and will be removed soon", true)]
	internal interface IDataWatchService
	{
		IDataWatchHandle AddWatch(Object watched, Action<Object> onDataChanged);

		void RemoveWatch(IDataWatchHandle handle);

		void ForceDirtyNextPoll(Object obj);
	}
}
