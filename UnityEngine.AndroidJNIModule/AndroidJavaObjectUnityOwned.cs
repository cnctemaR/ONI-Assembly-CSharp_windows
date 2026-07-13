using System;

namespace UnityEngine
{
	internal class AndroidJavaObjectUnityOwned : AndroidJavaObject
	{
		public AndroidJavaObjectUnityOwned(IntPtr jobject)
			: base(jobject)
		{
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				throw new Exception("The object is owned by Unity runtime, you shouldn't call Dispose on it.");
			}
			base.Dispose(disposing);
		}
	}
}
