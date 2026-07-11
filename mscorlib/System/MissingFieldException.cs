using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public class MissingFieldException : MissingMemberException, ISerializable
	{
		public MissingFieldException()
			: base(Environment.GetResourceString("Attempted to access a non-existing field."))
		{
			base.SetErrorCode(-2146233071);
		}

		public MissingFieldException(string message)
			: base(message)
		{
			base.SetErrorCode(-2146233071);
		}

		public MissingFieldException(string message, Exception inner)
			: base(message, inner)
		{
			base.SetErrorCode(-2146233071);
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
				return Environment.GetResourceString("Field '{0}' not found.", new object[] { ((this.Signature != null) ? (MissingMemberException.FormatSignature(this.Signature) + " ") : "") + this.ClassName + "." + this.MemberName });
			}
		}

		private MissingFieldException(string className, string fieldName, byte[] signature)
		{
			this.ClassName = className;
			this.MemberName = fieldName;
			this.Signature = signature;
		}

		public MissingFieldException(string className, string fieldName)
		{
			this.ClassName = className;
			this.MemberName = fieldName;
		}
	}
}
