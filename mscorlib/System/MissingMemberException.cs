using System;
using System.Runtime.Serialization;
using System.Security;

namespace System
{
	[Serializable]
	public class MissingMemberException : MemberAccessException
	{
		public MissingMemberException()
			: base("Attempted to access a missing member.")
		{
			base.HResult = -2146233070;
		}

		public MissingMemberException(string message)
			: base(message)
		{
			base.HResult = -2146233070;
		}

		public MissingMemberException(string message, Exception inner)
			: base(message, inner)
		{
			base.HResult = -2146233070;
		}

		public MissingMemberException(string className, string memberName)
		{
			this.ClassName = className;
			this.MemberName = memberName;
		}

		protected MissingMemberException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.ClassName = info.GetString("MMClassName");
			this.MemberName = info.GetString("MMMemberName");
			this.Signature = (byte[])info.GetValue("MMSignature", typeof(byte[]));
		}

		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("MMClassName", this.ClassName, typeof(string));
			info.AddValue("MMMemberName", this.MemberName, typeof(string));
			info.AddValue("MMSignature", this.Signature, typeof(byte[]));
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
				return SR.Format("Member '{0}' not found.", this.ClassName + "." + this.MemberName + ((this.Signature != null) ? (" " + MissingMemberException.FormatSignature(this.Signature)) : string.Empty));
			}
		}

		internal static string FormatSignature(byte[] signature)
		{
			return string.Empty;
		}

		protected string ClassName;

		protected string MemberName;

		protected byte[] Signature;
	}
}
