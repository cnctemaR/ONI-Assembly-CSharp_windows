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

		private AndroidJavaRunnable mRunnable;
	}
}
