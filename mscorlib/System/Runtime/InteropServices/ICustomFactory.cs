using System;

namespace System.Runtime.InteropServices
{
	public interface ICustomFactory
	{
		MarshalByRefObject CreateInstance(Type serverType);
	}
}
