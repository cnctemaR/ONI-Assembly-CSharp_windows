using System;
using System.Runtime.Serialization;
using System.Security;

namespace System
{
	[Serializable]
	public class ArgumentException : SystemException
	{
		public ArgumentException()
			: base("Value does not fall within the expected range.")
		{
			base.HResult = -2147024809;
		}

		public ArgumentException(string message)
			: base(message)
		{
			base.HResult = -2147024809;
		}

		public ArgumentException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.HResult = -2147024809;
		}

		public ArgumentException(string message, string paramName, Exception innerException)
			: base(message, innerException)
		{
			this._paramName = paramName;
			base.HResult = -2147024809;
		}

		public ArgumentException(string message, string paramName)
			: base(message)
		{
			this._paramName = paramName;
			base.HResult = -2147024809;
		}

		protected ArgumentException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this._paramName = info.GetString("ParamName");
		}

		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ParamName", this._paramName, typeof(string));
		}

		public override string Message
		{
			get
			{
				string message = base.Message;
				if (!string.IsNullOrEmpty(this._paramName))
				{
					string text = SR.Format("Parameter name: {0}", this._paramName);
					return message + Environment.NewLine + text;
				}
				return message;
			}
		}

		public virtual string ParamName
		{
			get
			{
				return this._paramName;
			}
		}

		private string _paramName;
	}
}
