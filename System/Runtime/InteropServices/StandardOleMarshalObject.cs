using System;

namespace System.Runtime.InteropServices
{
	[MonoLimitation("The runtime does nothing special apart from what it already does with marshal-by-ref objects")]
	[ComVisible(true)]
	public class StandardOleMarshalObject : MarshalByRefObject
	{
		protected StandardOleMarshalObject()
		{
		}
	}
}
