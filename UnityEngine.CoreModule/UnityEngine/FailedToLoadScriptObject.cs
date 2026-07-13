using System;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeClass(null)]
	[ExcludeFromObjectFactory]
	[RequiredByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	internal class FailedToLoadScriptObject : Object
	{
		private FailedToLoadScriptObject()
		{
		}
	}
}
