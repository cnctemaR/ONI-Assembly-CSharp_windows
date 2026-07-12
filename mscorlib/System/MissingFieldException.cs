using System;
using System.Runtime.Serialization;
using System.Security;

namespace System
{
	[Serializable]
	public class MissingFieldException : MissingMemberException, ISerializable
	{
		public MissingFieldException()
			: base("Attempted to access a non-existing field.")
		{
			base.HResult = -2146233071;
		}

		public MissingFieldException(string message)
			: base(message)
		{
			base.HResult = -2146233071;
		}

		public MissingFieldException(string message, Exception inner)
			: base(message, inner)
		{
			base.HResult = -2146233071;
		}

		public MissingFieldException(string className, string fieldName)
		{
			this.ClassName = className;
			this.MemberName = fieldName;
		}

		protected MissingFieldException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public override string Message
		{
			[SecuritySafeCritical]
			get
			{
				if (this.ClassName == null)
				{
					return base.Message;
				}
				return SR.Format("Field '{0}' not found.", ((this.Signature != null) ? (MissingMemberException.FormatSignature(this.Signature) + " ") : "") + this.ClassName + "." + this.MemberName);
			}
		}
	}
}
