using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public class ArgumentOutOfRangeException : ArgumentException
	{
		public ArgumentOutOfRangeException()
			: base(Locale.GetText("Argument is out of range."))
		{
			base.HResult = -2146233086;
		}

		public ArgumentOutOfRangeException(string paramName)
			: base(Locale.GetText("Argument is out of range."), paramName)
		{
			base.HResult = -2146233086;
		}

		public ArgumentOutOfRangeException(string paramName, string message)
			: base(message, paramName)
		{
			base.HResult = -2146233086;
		}

		public ArgumentOutOfRangeException(string paramName, object actualValue, string message)
			: base(message, paramName)
		{
			this.actual_value = actualValue;
			base.HResult = -2146233086;
		}

		protected ArgumentOutOfRangeException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.actual_value = info.GetString("ActualValue");
		}

		public ArgumentOutOfRangeException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.HResult = -2146233086;
		}

		public virtual object ActualValue
		{
			get
			{
				return this.actual_value;
			}
		}

		public override string Message
		{
			get
			{
				string message = base.Message;
				if (this.actual_value == null)
				{
					return message;
				}
				return message + Environment.NewLine + this.actual_value;
			}
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ActualValue", this.actual_value);
		}

		private const int Result = -2146233086;

		private object actual_value;
	}
}
