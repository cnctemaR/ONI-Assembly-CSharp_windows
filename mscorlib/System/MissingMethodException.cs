using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public class MissingMethodException : MissingMemberException, ISerializable
	{
		public MissingMethodException()
			: base(Environment.GetResourceString("Attempted to access a missing method."))
		{
			base.SetErrorCode(-2146233069);
		}

		public MissingMethodException(string message)
			: base(message)
		{
			base.SetErrorCode(-2146233069);
		}

		public MissingMethodException(string message, Exception inner)
			: base(message, inner)
		{
			base.SetErrorCode(-2146233069);
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
				if (this.ClassName == null)
				{
					return base.Message;
				}
				string text = this.ClassName + "." + this.MemberName;
				if (!string.IsNullOrEmpty(this.signature))
				{
					text = string.Format(CultureInfo.InvariantCulture, this.signature, text);
				}
				if (!string.IsNullOrEmpty(this._message))
				{
					text = text + " Due to: " + this._message;
				}
				return text;
			}
		}

		private MissingMethodException(string className, string methodName, byte[] signature)
		{
			this.ClassName = className;
			this.MemberName = methodName;
			this.Signature = signature;
		}

		public MissingMethodException(string className, string methodName)
		{
			this.ClassName = className;
			this.MemberName = methodName;
		}

		private MissingMethodException(string className, string methodName, string signature, string message)
			: base(message)
		{
			this.ClassName = className;
			this.MemberName = methodName;
			this.signature = signature;
		}

		[NonSerialized]
		private string signature;
	}
}
