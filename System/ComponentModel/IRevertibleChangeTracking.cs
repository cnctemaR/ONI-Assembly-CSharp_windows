using System;

namespace System.ComponentModel
{
	public interface IRevertibleChangeTracking : IChangeTracking
	{
		void RejectChanges();
	}
}
