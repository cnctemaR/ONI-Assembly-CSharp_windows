using System;

namespace UnityEngine.UIElements
{
	[Obsolete("IDataWatchHandle is no longer supported and will be removed soon", true)]
	internal interface IDataWatchHandle : IDisposable
	{
		Object watched { get; }

		bool disposed { get; }
	}
}
