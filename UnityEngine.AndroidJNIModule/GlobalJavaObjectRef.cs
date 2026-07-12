using System;

namespace UnityEngine
{
	internal class GlobalJavaObjectRef
	{
		public GlobalJavaObjectRef(IntPtr jobject)
		{
			this.m_jobject = ((jobject == IntPtr.Zero) ? IntPtr.Zero : AndroidJNI.NewGlobalRef(jobject));
		}

		~GlobalJavaObjectRef()
		{
			this.Dispose();
		}

		public static implicit operator IntPtr(GlobalJavaObjectRef obj)
		{
			return obj.m_jobject;
		}

		public void Dispose()
		{
			bool disposed = this.m_disposed;
			if (!disposed)
			{
				this.m_disposed = true;
				bool flag = this.m_jobject != IntPtr.Zero;
				if (flag)
				{
					AndroidJNISafe.QueueDeleteGlobalRef(this.m_jobject);
				}
			}
		}

		private bool m_disposed = false;

		protected IntPtr m_jobject;
	}
}
