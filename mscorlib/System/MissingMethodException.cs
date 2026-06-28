using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public class MissingMethodException : MissingMemberException
	{
		public MissingMethodException()
			: base(Locale.GetText("Cannot find the requested method."))
		{
			base.HResult = -2146233069;
		}

		public MissingMethodException(string message)
			: base(message)
		{
			base.HResult = -2146233069;
		}

		protected MissingMethodException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public MissingMethodException(string message, Exception inner)
			: base(message, inner)
		{
			base.HResult = -2146233069;
		}

		public MissingMethodException(string className, string methodName)
			: base(className, methodName)
		{
			base.HResult = -2146233069;
		}

		public override string Message
		{
			get
			{
				if (this.ClassName == null)
				{
					return base.Message;
				}
				string text = Locale.GetText("Method not found: '{0}.{1}'.");
				return string.Format(text, this.ClassName, this.MemberName);
			}
		}

		private const int Result = -2146233069;
	}
}
