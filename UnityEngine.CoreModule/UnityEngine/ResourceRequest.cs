using System;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	public class ResourceRequest : AsyncOperation
	{
		protected virtual Object GetResult()
		{
			return Resources.Load(this.m_Path, this.m_Type);
		}

		public Object asset
		{
			get
			{
				return this.GetResult();
			}
		}

		public ResourceRequest()
		{
		}

		protected ResourceRequest(IntPtr ptr)
			: base(ptr)
		{
		}

		internal string m_Path;

		internal Type m_Type;

		internal new static class BindingsMarshaller
		{
			public static ResourceRequest ConvertToManaged(IntPtr ptr)
			{
				return new ResourceRequest(ptr);
			}
		}
	}
}
