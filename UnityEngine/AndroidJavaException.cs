using System;

namespace UnityEngine
{
	public sealed class AndroidJavaException : Exception
	{
		internal AndroidJavaException(string message, string javaStackTrace)
			: base(message)
		{
			this.mJavaStackTrace = javaStackTrace;
		}

		public override string StackTrace
		{
			get
			{
				return this.mJavaStackTrace + base.StackTrace;
			}
		}

		private string mJavaStackTrace;
	}
}
