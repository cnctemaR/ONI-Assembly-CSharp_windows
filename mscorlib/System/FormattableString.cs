using System;
using System.Globalization;

namespace System
{
	public abstract class FormattableString : IFormattable
	{
		public abstract string Format { get; }

		public abstract object[] GetArguments();

		public abstract int ArgumentCount { get; }

		public abstract object GetArgument(int index);

		public abstract string ToString(IFormatProvider formatProvider);

		string IFormattable.ToString(string ignored, IFormatProvider formatProvider)
		{
			return this.ToString(formatProvider);
		}

		public static string Invariant(FormattableString formattable)
		{
			if (formattable == null)
			{
				throw new ArgumentNullException("formattable");
			}
			return formattable.ToString(CultureInfo.InvariantCulture);
		}

		public override string ToString()
		{
			return this.ToString(CultureInfo.CurrentCulture);
		}
	}
}
