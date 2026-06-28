using System;

namespace System.ComponentModel
{
	public interface IExtenderProvider
	{
		bool CanExtend(object extendee);
	}
}
