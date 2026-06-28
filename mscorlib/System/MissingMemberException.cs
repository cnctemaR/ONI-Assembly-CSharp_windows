using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public class MissingMemberException : MemberAccessException
	{
		public MissingMemberException()
			: base(Locale.GetText("Cannot find the requested class member."))
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

		protected MissingMemberException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.ClassName = info.GetString("MMClassName");
			this.MemberName = info.GetString("MMMemberName");
			this.Signature = (byte[])info.GetValue("MMSignature", typeof(byte[]));
		}

		public MissingMemberException(string className, string memberName)
		{
			this.ClassName = className;
			this.MemberName = memberName;
			base.HResult = -2146233070;
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("MMClassName", this.ClassName);
			info.AddValue("MMMemberName", this.MemberName);
			info.AddValue("MMSignature", this.Signature);
		}

		public override string Message
		{
			get
			{
				if (this.ClassName == null)
				{
					return base.Message;
				}
				string text = Locale.GetText("Member {0}.{1} not found.");
				return string.Format(text, this.ClassName, this.MemberName);
			}
		}

		private const int Result = -2146233070;

		protected string ClassName;

		protected string MemberName;

		protected byte[] Signature;
	}
}
