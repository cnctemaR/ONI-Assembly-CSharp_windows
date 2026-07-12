using System;
using System.Runtime.Serialization;
using System.Security;

namespace System
{
	[Serializable]
	public class NotFiniteNumberException : ArithmeticException
	{
		public NotFiniteNumberException()
			: base("Arg_NotFiniteNumberException = Number encountered was not a finite quantity.")
		{
			this._offendingNumber = 0.0;
			base.HResult = -2146233048;
		}

		public NotFiniteNumberException(double offendingNumber)
		{
			this._offendingNumber = offendingNumber;
			base.HResult = -2146233048;
		}

		public NotFiniteNumberException(string message)
			: base(message)
		{
			this._offendingNumber = 0.0;
			base.HResult = -2146233048;
		}

		public NotFiniteNumberException(string message, double offendingNumber)
			: base(message)
		{
			this._offendingNumber = offendingNumber;
			base.HResult = -2146233048;
		}

		public NotFiniteNumberException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.HResult = -2146233048;
		}

		public NotFiniteNumberException(string message, double offendingNumber, Exception innerException)
			: base(message, innerException)
		{
			this._offendingNumber = offendingNumber;
			base.HResult = -2146233048;
		}

		protected NotFiniteNumberException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this._offendingNumber = (double)info.GetInt32("OffendingNumber");
		}

		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("OffendingNumber", this._offendingNumber, typeof(int));
		}

		public double OffendingNumber
		{
			get
			{
				return this._offendingNumber;
			}
		}

		private double _offendingNumber;
	}
}
