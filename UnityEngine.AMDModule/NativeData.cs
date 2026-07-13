using System;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.AMD
{
	internal class NativeData<T> : IDisposable where T : struct
	{
		public IntPtr Ptr
		{
			get
			{
				UnsafeUtility.CopyStructureToPtr<T>(ref this.Value, this.m_MarshalledValue.ToPointer());
				return this.m_MarshalledValue;
			}
		}

		public NativeData()
		{
			this.m_MarshalledValue = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(T)));
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			bool flag = this.m_MarshalledValue != IntPtr.Zero;
			if (flag)
			{
				Marshal.FreeHGlobal(this.m_MarshalledValue);
				this.m_MarshalledValue = IntPtr.Zero;
			}
		}

		~NativeData()
		{
			this.Dispose(false);
		}

		private IntPtr m_MarshalledValue = IntPtr.Zero;

		public T Value = new T();
	}
}
