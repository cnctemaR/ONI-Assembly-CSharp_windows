using System;

namespace UnityEngine
{
	internal class AndroidJavaRunnableProxy : AndroidJavaProxy
	{
		public AndroidJavaRunnableProxy(AndroidJavaRunnable runnable)
			: base("java/lang/Runnable")
		{
			this.mRunnable = runnable;
		}

		public void run()
		{
			this.mRunnable();
		}

		public override IntPtr Invoke(string methodName, IntPtr javaArgs)
		{
			int num = 0;
			bool flag = javaArgs != IntPtr.Zero;
			if (flag)
			{
				num = AndroidJNISafe.GetArrayLength(javaArgs);
			}
			bool flag2 = num == 0 && methodName == "run";
			IntPtr intPtr;
			if (flag2)
			{
				this.run();
				intPtr = IntPtr.Zero;
			}
			else
			{
				intPtr = base.Invoke(methodName, javaArgs);
			}
			return intPtr;
		}

		private AndroidJavaRunnable mRunnable;
	}
}
