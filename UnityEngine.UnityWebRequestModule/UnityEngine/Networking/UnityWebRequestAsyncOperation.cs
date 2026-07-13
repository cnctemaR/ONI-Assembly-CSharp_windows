using System;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Networking
{
	[UsedByNativeCode]
	[NativeHeader("Modules/UnityWebRequest/Public/UnityWebRequestAsyncOperation.h")]
	[NativeHeader("UnityWebRequestScriptingClasses.h")]
	[StructLayout(LayoutKind.Sequential)]
	public class UnityWebRequestAsyncOperation : AsyncOperation
	{
		public UnityWebRequestAsyncOperation()
		{
		}

		private UnityWebRequestAsyncOperation(IntPtr ptr)
			: base(ptr)
		{
		}

		public UnityWebRequest webRequest { get; internal set; }

		internal new static class BindingsMarshaller
		{
			public static UnityWebRequestAsyncOperation ConvertToManaged(IntPtr ptr)
			{
				return new UnityWebRequestAsyncOperation(ptr);
			}
		}
	}
}
