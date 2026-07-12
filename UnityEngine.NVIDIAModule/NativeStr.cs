using System;
using System.Runtime.InteropServices;

namespace UnityEngine.NVIDIA
{
	internal class NativeStr : IDisposable
	{
		public string Str
		{
			set
			{
				this.m_Str = value;
				this.Dispose();
				bool flag = value != null;
				if (flag)
				{
					this.m_MarshalledString = Marshal.StringToHGlobalUni(this.m_Str);
				}
			}
		}

		public IntPtr Ptr
		{
			get
			{
				return this.m_MarshalledString;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			bool flag = this.m_MarshalledString != IntPtr.Zero;
			if (flag)
			{
				Marshal.FreeHGlobal(this.m_MarshalledString);
				this.m_MarshalledString = IntPtr.Zero;
			}
		}

		~NativeStr()
		{
			this.Dispose(false);
		}

		private string m_Str = null;

		private IntPtr m_MarshalledString = IntPtr.Zero;
	}
}
