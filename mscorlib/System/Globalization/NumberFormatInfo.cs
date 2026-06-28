using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.Globalization
{
	[ComVisible(true)]
	[Serializable]
	public sealed class NumberFormatInfo : ICloneable, IFormatProvider
	{
		internal NumberFormatInfo(int lcid, bool read_only)
		{
			this.isReadOnly = read_only;
			if (lcid != 127)
			{
				lcid = 127;
			}
			int num = lcid;
			if (num == 127)
			{
				this.isReadOnly = false;
				this.currencyDecimalDigits = 2;
				this.currencyDecimalSeparator = ".";
				this.currencyGroupSeparator = ",";
				this.currencyGroupSizes = new int[] { 3 };
				this.currencyNegativePattern = 0;
				this.currencyPositivePattern = 0;
				this.currencySymbol = "$";
				this.nanSymbol = "NaN";
				this.negativeInfinitySymbol = "-Infinity";
				this.negativeSign = "-";
				this.numberDecimalDigits = 2;
				this.numberDecimalSeparator = ".";
				this.numberGroupSeparator = ",";
				this.numberGroupSizes = new int[] { 3 };
				this.numberNegativePattern = 1;
				this.percentDecimalDigits = 2;
				this.percentDecimalSeparator = ".";
				this.percentGroupSeparator = ",";
				this.percentGroupSizes = new int[] { 3 };
				this.percentNegativePattern = 0;
				this.percentPositivePattern = 0;
				this.percentSymbol = "%";
				this.perMilleSymbol = "‰";
				this.positiveInfinitySymbol = "Infinity";
				this.positiveSign = "+";
			}
		}

		internal NumberFormatInfo(bool read_only)
			: this(127, read_only)
		{
		}

		public NumberFormatInfo()
			: this(false)
		{
		}

		private void InitPatterns()
		{
			string[] array = this.decimalFormats.Split(new char[] { ';' }, 2);
			string[] array2;
			if (array.Length == 2)
			{
				array2 = array[0].Split(new char[] { '.' }, 2);
				if (array2.Length == 2)
				{
					this.numberDecimalDigits = 0;
					for (int i = 0; i < array2[1].Length; i++)
					{
						if (array2[1][i] != this.digitPattern[0])
						{
							break;
						}
						this.numberDecimalDigits++;
					}
					string[] array3 = array2[0].Split(new char[] { ',' });
					if (array3.Length > 1)
					{
						this.numberGroupSizes = new int[array3.Length - 1];
						for (int j = 0; j < this.numberGroupSizes.Length; j++)
						{
							string text = array3[j + 1];
							this.numberGroupSizes[j] = text.Length;
						}
					}
					else
					{
						this.numberGroupSizes = new int[1];
					}
					if (array[1].StartsWith("(") && array[1].EndsWith(")"))
					{
						this.numberNegativePattern = 0;
					}
					else if (array[1].StartsWith("- "))
					{
						this.numberNegativePattern = 2;
					}
					else if (array[1].StartsWith("-"))
					{
						this.numberNegativePattern = 1;
					}
					else if (array[1].EndsWith(" -"))
					{
						this.numberNegativePattern = 4;
					}
					else if (array[1].EndsWith("-"))
					{
						this.numberNegativePattern = 3;
					}
					else
					{
						this.numberNegativePattern = 1;
					}
				}
			}
			array = this.currencyFormats.Split(new char[] { ';' }, 2);
			if (array.Length == 2)
			{
				array2 = array[0].Split(new char[] { '.' }, 2);
				if (array2.Length == 2)
				{
					this.currencyDecimalDigits = 0;
					for (int k = 0; k < array2[1].Length; k++)
					{
						if (array2[1][k] != this.zeroPattern[0])
						{
							break;
						}
						this.currencyDecimalDigits++;
					}
					string[] array3 = array2[0].Split(new char[] { ',' });
					if (array3.Length > 1)
					{
						this.currencyGroupSizes = new int[array3.Length - 1];
						for (int l = 0; l < this.currencyGroupSizes.Length; l++)
						{
							string text2 = array3[l + 1];
							this.currencyGroupSizes[l] = text2.Length;
						}
					}
					else
					{
						this.currencyGroupSizes = new int[1];
					}
					if (array[1].StartsWith("(¤ ") && array[1].EndsWith(")"))
					{
						this.currencyNegativePattern = 14;
					}
					else if (array[1].StartsWith("(¤") && array[1].EndsWith(")"))
					{
						this.currencyNegativePattern = 0;
					}
					else if (array[1].StartsWith("¤ ") && array[1].EndsWith("-"))
					{
						this.currencyNegativePattern = 11;
					}
					else if (array[1].StartsWith("¤") && array[1].EndsWith("-"))
					{
						this.currencyNegativePattern = 3;
					}
					else if (array[1].StartsWith("(") && array[1].EndsWith(" ¤"))
					{
						this.currencyNegativePattern = 15;
					}
					else if (array[1].StartsWith("(") && array[1].EndsWith("¤"))
					{
						this.currencyNegativePattern = 4;
					}
					else if (array[1].StartsWith("-") && array[1].EndsWith(" ¤"))
					{
						this.currencyNegativePattern = 8;
					}
					else if (array[1].StartsWith("-") && array[1].EndsWith("¤"))
					{
						this.currencyNegativePattern = 5;
					}
					else if (array[1].StartsWith("-¤ "))
					{
						this.currencyNegativePattern = 9;
					}
					else if (array[1].StartsWith("-¤"))
					{
						this.currencyNegativePattern = 1;
					}
					else if (array[1].StartsWith("¤ -"))
					{
						this.currencyNegativePattern = 12;
					}
					else if (array[1].StartsWith("¤-"))
					{
						this.currencyNegativePattern = 2;
					}
					else if (array[1].EndsWith(" ¤-"))
					{
						this.currencyNegativePattern = 10;
					}
					else if (array[1].EndsWith("¤-"))
					{
						this.currencyNegativePattern = 7;
					}
					else if (array[1].EndsWith("- ¤"))
					{
						this.currencyNegativePattern = 13;
					}
					else if (array[1].EndsWith("-¤"))
					{
						this.currencyNegativePattern = 6;
					}
					else
					{
						this.currencyNegativePattern = 0;
					}
					if (array[0].StartsWith("¤ "))
					{
						this.currencyPositivePattern = 2;
					}
					else if (array[0].StartsWith("¤"))
					{
						this.currencyPositivePattern = 0;
					}
					else if (array[0].EndsWith(" ¤"))
					{
						this.currencyPositivePattern = 3;
					}
					else if (array[0].EndsWith("¤"))
					{
						this.currencyPositivePattern = 1;
					}
					else
					{
						this.currencyPositivePattern = 0;
					}
				}
			}
			if (this.percentFormats.StartsWith("%"))
			{
				this.percentPositivePattern = 2;
				this.percentNegativePattern = 2;
			}
			else if (this.percentFormats.EndsWith(" %"))
			{
				this.percentPositivePattern = 0;
				this.percentNegativePattern = 0;
			}
			else if (this.percentFormats.EndsWith("%"))
			{
				this.percentPositivePattern = 1;
				this.percentNegativePattern = 1;
			}
			else
			{
				this.percentPositivePattern = 0;
				this.percentNegativePattern = 0;
			}
			array2 = this.percentFormats.Split(new char[] { '.' }, 2);
			if (array2.Length == 2)
			{
				this.percentDecimalDigits = 0;
				for (int m = 0; m < array2[1].Length; m++)
				{
					if (array2[1][m] != this.digitPattern[0])
					{
						break;
					}
					this.percentDecimalDigits++;
				}
				string[] array3 = array2[0].Split(new char[] { ',' });
				if (array3.Length > 1)
				{
					this.percentGroupSizes = new int[array3.Length - 1];
					for (int n = 0; n < this.percentGroupSizes.Length; n++)
					{
						string text3 = array3[n + 1];
						this.percentGroupSizes[n] = text3.Length;
					}
				}
				else
				{
					this.percentGroupSizes = new int[1];
				}
			}
		}

		public int CurrencyDecimalDigits
		{
			get
			{
				return this.currencyDecimalDigits;
			}
			set
			{
				if (value < 0 || value > 99)
				{
					throw new ArgumentOutOfRangeException("The value specified for the property is less than 0 or greater than 99");
				}
				if (this.isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				this.currencyDecimalDigits = value;
			}
		}

		public string CurrencyDecimalSeparator
		{
			get
			{
				return this.currencyDecimalSeparator;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("The value specified for the property is a null reference");
				}
				if (this.isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				this.currencyDecimalSeparator = value;
			}
		}

		public string CurrencyGroupSeparator
		{
			get
			{
				return this.currencyGroupSeparator;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("The value specified for the property is a null reference");
				}
				if (this.isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				this.currencyGroupSeparator = value;
			}
		}

		public int[] CurrencyGroupSizes
		{
			get
			{
				return (int[])this.RawCurrencyGroupSizes.Clone();
			}
			set
			{
				this.RawCurrencyGroupSizes = value;
			}
		}

		internal int[] RawCurrencyGroupSizes
		{
			get
			{
				return this.currencyGroupSizes;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("The value specified for the property is a null reference");
				}
				if (this.isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				if (value.Length == 0)
				{
					this.currencyGroupSizes = new int[0];
					return;
				}
				int num = value.Length - 1;
				for (int i = 0; i < num; i++)
				{
					if (value[i] < 1 || value[i] > 9)
					{
						throw new ArgumentOutOfRangeException("One of the elements in the array specified is not between 1 and 9");
					}
				}
				if (value[num] < 0 || value[num] > 9)
				{
					throw new ArgumentOutOfRangeException("Last element in the array specified is not between 0 and 9");
				}
				this.currencyGroupSizes = (int[])value.Clone();
			}
		}

		public int CurrencyNegativePattern
		{
			get
			{
				return this.currencyNegativePattern;
			}
			set
			{
				if (value < 0 || value > 15)
				{
					throw new ArgumentOutOfRangeException("The value specified for the property is less than 0 or greater than 15");
				}
				if (this.isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				this.currencyNegativePattern = value;
			}
		}

		public int CurrencyPositivePattern
		{
			get
			{
				return this.currencyPositivePattern;
			}
			set
			{
				if (value < 0 || value > 3)
				{
					throw new ArgumentOutOfRangeException("The value specified for the property is less than 0 or greater than 3");
				}
				if (this.isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				this.currencyPositivePattern = value;
			}
		}

		public string CurrencySymbol
		{
			get
			{
				return this.currencySymbol;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("The value specified for the property is a null reference");
				}
				if (this.isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				this.currencySymbol = value;
			}
		}

		public static NumberFormatInfo CurrentInfo
		{
			get
			{
				NumberFormatInfo numberFormat = Thread.CurrentThread.CurrentCulture.NumberFormat;
				numberFormat.isReadOnly = true;
				return numberFormat;
			}
		}

		public static NumberFormatInfo InvariantInfo
		{
			get
			{
				return new NumberFormatInfo
				{
					NumberNegativePattern = 1,
					isReadOnly = true
				};
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this.isReadOnly;
			}
		}

		public string NaNSymbol
		{
			get
			{
				return this.nanSymbol;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("The value specified for the property is a null reference");
				}
				if (this.isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				this.nanSymbol = value;
			}
		}

		[ComVisible(false)]
		[MonoNotSupported("We don't have native digit info")]
		public string[] NativeDigits
		{
			get
			{
				return this.nativeDigits;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (value.Length != 10)
				{
					throw new ArgumentException("Argument array length must be 10");
				}
				for (int i = 0; i < value.Length; i++)
				{
					string text = value[i];
					if (string.IsNullOrEmpty(text))
					{
						throw new ArgumentException("Argument array contains one or more null strings");
					}
				}
				this.nativeDigits = value;
			}
		}

		[MonoNotSupported("We don't have native digit info")]
		[ComVisible(false)]
		public DigitShapes DigitSubstitution
		{
			get
			{
				return (DigitShapes)this.digitSubstitution;
			}
			set
			{
				this.digitSubstitution = (int)value;
			}
		}

		public string NegativeInfinitySymbol
		{
			get
			{
				return this.negativeInfinitySymbol;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("The value specified for the property is a null reference");
				}
				if (this.isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				this.negativeInfinitySymbol = value;
			}
		}

		public string NegativeSign
		{
			get
			{
				return this.negativeSign;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("The value specified for the property is a null reference");
				}
				if (this.isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				this.negativeSign = value;
			}
		}

		public int NumberDecimalDigits
		{
			get
			{
				return this.numberDecimalDigits;
			}
			set
			{
				if (value < 0 || value > 99)
				{
					throw new ArgumentOutOfRangeException("The value specified for the property is less than 0 or greater than 99");
				}
				if (this.isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				this.numberDecimalDigits = value;
			}
		}

		public string NumberDecimalSeparator
		{
			get
			{
				return this.numberDecimalSeparator;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("The value specified for the property is a null reference");
				}
				if (this.isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				this.numberDecimalSeparator = value;
			}
		}

		public string NumberGroupSeparator
		{
			get
			{
				return this.numberGroupSeparator;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("The value specified for the property is a null reference");
				}
				if (this.isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				this.numberGroupSeparator = value;
			}
		}

		public int[] NumberGroupSizes
		{
			get
			{
				return (int[])this.RawNumberGroupSizes.Clone();
			}
			set
			{
				this.RawNumberGroupSizes = value;
			}
		}

		internal int[] RawNumberGroupSizes
		{
			get
			{
				return this.numberGroupSizes;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("The value specified for the property is a null reference");
				}
				if (this.isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				if (value.Length == 0)
				{
					this.numberGroupSizes = new int[0];
					return;
				}
				int num = value.Length - 1;
				for (int i = 0; i < num; i++)
				{
					if (value[i] < 1 || value[i] > 9)
					{
						throw new ArgumentOutOfRangeException("One of the elements in the array specified is not between 1 and 9");
					}
				}
				if (value[num] < 0 || value[num] > 9)
				{
					throw new ArgumentOutOfRangeException("Last element in the array specified is not between 0 and 9");
				}
				this.numberGroupSizes = (int[])value.Clone();
			}
		}

		public int NumberNegativePattern
		{
			get
			{
				return this.numberNegativePattern;
			}
			set
			{
				if (value < 0 || value > 4)
				{
					throw new ArgumentOutOfRangeException("The value specified for the property is less than 0 or greater than 15");
				}
				if (this.isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				this.numberNegativePattern = value;
			}
		}

		public int PercentDecimalDigits
		{
			get
			{
				return this.percentDecimalDigits;
			}
			set
			{
				if (value < 0 || value > 99)
				{
					throw new ArgumentOutOfRangeException("The value specified for the property is less than 0 or greater than 99");
				}
				if (this.isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				this.percentDecimalDigits = value;
			}
		}

		public string PercentDecimalSeparator
		{
			get
			{
				return this.percentDecimalSeparator;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("The value specified for the property is a null reference");
				}
				if (this.isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				this.percentDecimalSeparator = value;
			}
		}

		public string PercentGroupSeparator
		{
			get
			{
				return this.percentGroupSeparator;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("The value specified for the property is a null reference");
				}
				if (this.isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				this.percentGroupSeparator = value;
			}
		}

		public int[] PercentGroupSizes
		{
			get
			{
				return (int[])this.RawPercentGroupSizes.Clone();
			}
			set
			{
				this.RawPercentGroupSizes = value;
			}
		}

		internal int[] RawPercentGroupSizes
		{
			get
			{
				return this.percentGroupSizes;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("The value specified for the property is a null reference");
				}
				if (this.isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				if (this == CultureInfo.CurrentCulture.NumberFormat)
				{
					throw new Exception("HERE the value was modified");
				}
				if (value.Length == 0)
				{
					this.percentGroupSizes = new int[0];
					return;
				}
				int num = value.Length - 1;
				for (int i = 0; i < num; i++)
				{
					if (value[i] < 1 || value[i] > 9)
					{
						throw new ArgumentOutOfRangeException("One of the elements in the array specified is not between 1 and 9");
					}
				}
				if (value[num] < 0 || value[num] > 9)
				{
					throw new ArgumentOutOfRangeException("Last element in the array specified is not between 0 and 9");
				}
				this.percentGroupSizes = (int[])value.Clone();
			}
		}

		public int PercentNegativePattern
		{
			get
			{
				return this.percentNegativePattern;
			}
			set
			{
				if (value < 0 || value > 2)
				{
					throw new ArgumentOutOfRangeException("The value specified for the property is less than 0 or greater than 15");
				}
				if (this.isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				this.percentNegativePattern = value;
			}
		}

		public int PercentPositivePattern
		{
			get
			{
				return this.percentPositivePattern;
			}
			set
			{
				if (value < 0 || value > 2)
				{
					throw new ArgumentOutOfRangeException("The value specified for the property is less than 0 or greater than 3");
				}
				if (this.isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				this.percentPositivePattern = value;
			}
		}

		public string PercentSymbol
		{
			get
			{
				return this.percentSymbol;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("The value specified for the property is a null reference");
				}
				if (this.isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				this.percentSymbol = value;
			}
		}

		public string PerMilleSymbol
		{
			get
			{
				return this.perMilleSymbol;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("The value specified for the property is a null reference");
				}
				if (this.isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				this.perMilleSymbol = value;
			}
		}

		public string PositiveInfinitySymbol
		{
			get
			{
				return this.positiveInfinitySymbol;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("The value specified for the property is a null reference");
				}
				if (this.isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				this.positiveInfinitySymbol = value;
			}
		}

		public string PositiveSign
		{
			get
			{
				return this.positiveSign;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("The value specified for the property is a null reference");
				}
				if (this.isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				this.positiveSign = value;
			}
		}

		public object GetFormat(Type formatType)
		{
			return (formatType != typeof(NumberFormatInfo)) ? null : this;
		}

		public object Clone()
		{
			NumberFormatInfo numberFormatInfo = (NumberFormatInfo)base.MemberwiseClone();
			numberFormatInfo.isReadOnly = false;
			return numberFormatInfo;
		}

		public static NumberFormatInfo ReadOnly(NumberFormatInfo nfi)
		{
			NumberFormatInfo numberFormatInfo = (NumberFormatInfo)nfi.Clone();
			numberFormatInfo.isReadOnly = true;
			return numberFormatInfo;
		}

		public static NumberFormatInfo GetInstance(IFormatProvider formatProvider)
		{
			if (formatProvider != null)
			{
				NumberFormatInfo numberFormatInfo = (NumberFormatInfo)formatProvider.GetFormat(typeof(NumberFormatInfo));
				if (numberFormatInfo != null)
				{
					return numberFormatInfo;
				}
			}
			return NumberFormatInfo.CurrentInfo;
		}

		private bool isReadOnly;

		private string decimalFormats;

		private string currencyFormats;

		private string percentFormats;

		private string digitPattern = "#";

		private string zeroPattern = "0";

		private int currencyDecimalDigits;

		private string currencyDecimalSeparator;

		private string currencyGroupSeparator;

		private int[] currencyGroupSizes;

		private int currencyNegativePattern;

		private int currencyPositivePattern;

		private string currencySymbol;

		private string nanSymbol;

		private string negativeInfinitySymbol;

		private string negativeSign;

		private int numberDecimalDigits;

		private string numberDecimalSeparator;

		private string numberGroupSeparator;

		private int[] numberGroupSizes;

		private int numberNegativePattern;

		private int percentDecimalDigits;

		private string percentDecimalSeparator;

		private string percentGroupSeparator;

		private int[] percentGroupSizes;

		private int percentNegativePattern;

		private int percentPositivePattern;

		private string percentSymbol;

		private string perMilleSymbol;

		private string positiveInfinitySymbol;

		private string positiveSign;

		private string ansiCurrencySymbol;

		private int m_dataItem;

		private bool m_useUserOverride;

		private bool validForParseAsNumber;

		private bool validForParseAsCurrency;

		private string[] nativeDigits = NumberFormatInfo.invariantNativeDigits;

		private int digitSubstitution = 1;

		private static readonly string[] invariantNativeDigits = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
	}
}
