using System;
using System.Runtime.Serialization;
using System.Security;

namespace System
{
	[Serializable]
	public class ArgumentOutOfRangeException : ArgumentException
	{
		public ArgumentOutOfRangeException()
			: base("Specified argument was out of the range of valid values.")
		{
			base.HResult = -2146233086;
		}

		public ArgumentOutOfRangeException(string paramName)
			: base("Specified argument was out of the range of valid values.", paramName)
		{
			base.HResult = -2146233086;
		}

		public ArgumentOutOfRangeException(string paramName, string message)
			: base(message, paramName)
		{
			base.HResult = -2146233086;
		}

		public ArgumentOutOfRangeException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.HResult = -2146233086;
		}

		public ArgumentOutOfRangeException(string paramName, object actualValue, string message)
			: base(message, paramName)
		{
			this._actualValue = actualValue;
			base.HResult = -2146233086;
		}

		protected ArgumentOutOfRangeException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this._actualValue = info.GetValue("ActualValue", typeof(object));
		}

		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ActualValue", this._actualValue, typeof(object));
		}

		public override string Message
		{
			get
			{
				string message = base.Message;
				if (this._actualValue == null)
				{
					return message;
				}
				string text = SR.Format("Actual value was {0}.", this._actualValue.ToString());
				if (message == null)
				{
					return text;
				}
				return message + Environment.NewLine + text;
			}
		}

		public virtual object ActualValue
		{
			get
			{
				return this._actualValue;
			}
		}

		private object _actualValue;
	}
}
