using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public class NotFiniteNumberException : ArithmeticException
	{
		public NotFiniteNumberException()
			: base(Locale.GetText("The number encountered was not a finite quantity."))
		{
			base.HResult = -2146233048;
		}

		public NotFiniteNumberException(double offendingNumber)
		{
			this.offending_number = offendingNumber;
			base.HResult = -2146233048;
		}

		public NotFiniteNumberException(string message)
			: base(message)
		{
			base.HResult = -2146233048;
		}

		public NotFiniteNumberException(string message, double offendingNumber)
			: base(message)
		{
			this.offending_number = offendingNumber;
			base.HResult = -2146233048;
		}

		public NotFiniteNumberException(string message, double offendingNumber, Exception innerException)
			: base(message, innerException)
		{
			this.offending_number = offendingNumber;
			base.HResult = -2146233048;
		}

		protected NotFiniteNumberException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.offending_number = info.GetDouble("OffendingNumber");
		}

		public NotFiniteNumberException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.HResult = -2146233048;
		}

		public double OffendingNumber
		{
			get
			{
				return this.offending_number;
			}
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("OffendingNumber", this.offending_number);
		}

		private const int Result = -2146233048;

		private double offending_number;
	}
}
