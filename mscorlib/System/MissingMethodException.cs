using System;
using System.Runtime.Serialization;
using System.Security;

namespace System
{
	[Serializable]
	public class MissingMethodException : MissingMemberException
	{
		public MissingMethodException()
			: base("Attempted to access a missing method.")
		{
			base.HResult = -2146233069;
		}

		public MissingMethodException(string message)
			: base(message)
		{
			base.HResult = -2146233069;
		}

		public MissingMethodException(string message, Exception inner)
			: base(message, inner)
		{
			base.HResult = -2146233069;
		}

		public MissingMethodException(string className, string methodName)
		{
			this.ClassName = className;
			this.MemberName = methodName;
		}

		protected MissingMethodException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public override string Message
		{
			[SecuritySafeCritical]
			get
			{
				if (this.ClassName != null)
				{
					return SR.Format("Method '{0}' not found.", this.ClassName + "." + this.MemberName + ((this.Signature != null) ? (" " + MissingMemberException.FormatSignature(this.Signature)) : string.Empty));
				}
				return base.Message;
			}
		}
	}
}
