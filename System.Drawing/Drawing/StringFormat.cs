using System;
using System.ComponentModel;
using System.Drawing.Text;

namespace System.Drawing
{
	public sealed class StringFormat : MarshalByRefObject, IDisposable, ICloneable
	{
		public StringFormat()
			: this((StringFormatFlags)0, 0)
		{
		}

		public StringFormat(StringFormatFlags options, int language)
		{
			this.nativeStrFmt = IntPtr.Zero;
			base..ctor();
			Status status = GDIPlus.GdipCreateStringFormat(options, language, out this.nativeStrFmt);
			GDIPlus.CheckStatus(status);
		}

		internal StringFormat(IntPtr native)
		{
			this.nativeStrFmt = IntPtr.Zero;
			base..ctor();
			this.nativeStrFmt = native;
		}

		public StringFormat(StringFormat format)
		{
			this.nativeStrFmt = IntPtr.Zero;
			base..ctor();
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			Status status = GDIPlus.GdipCloneStringFormat(format.NativeObject, out this.nativeStrFmt);
			GDIPlus.CheckStatus(status);
		}

		public StringFormat(StringFormatFlags options)
		{
			this.nativeStrFmt = IntPtr.Zero;
			base..ctor();
			Status status = GDIPlus.GdipCreateStringFormat(options, 0, out this.nativeStrFmt);
			GDIPlus.CheckStatus(status);
		}

		~StringFormat()
		{
			this.Dispose(false);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (this.nativeStrFmt != IntPtr.Zero)
			{
				Status status = GDIPlus.GdipDeleteStringFormat(this.nativeStrFmt);
				this.nativeStrFmt = IntPtr.Zero;
				GDIPlus.CheckStatus(status);
			}
		}

		public StringAlignment Alignment
		{
			get
			{
				StringAlignment stringAlignment;
				Status status = GDIPlus.GdipGetStringFormatAlign(this.nativeStrFmt, out stringAlignment);
				GDIPlus.CheckStatus(status);
				return stringAlignment;
			}
			set
			{
				if (value < StringAlignment.Near || value > StringAlignment.Far)
				{
					throw new InvalidEnumArgumentException("Alignment");
				}
				Status status = GDIPlus.GdipSetStringFormatAlign(this.nativeStrFmt, value);
				GDIPlus.CheckStatus(status);
			}
		}

		public StringAlignment LineAlignment
		{
			get
			{
				StringAlignment stringAlignment;
				Status status = GDIPlus.GdipGetStringFormatLineAlign(this.nativeStrFmt, out stringAlignment);
				GDIPlus.CheckStatus(status);
				return stringAlignment;
			}
			set
			{
				if (value < StringAlignment.Near || value > StringAlignment.Far)
				{
					throw new InvalidEnumArgumentException("Alignment");
				}
				Status status = GDIPlus.GdipSetStringFormatLineAlign(this.nativeStrFmt, value);
				GDIPlus.CheckStatus(status);
			}
		}

		public StringFormatFlags FormatFlags
		{
			get
			{
				StringFormatFlags stringFormatFlags;
				Status status = GDIPlus.GdipGetStringFormatFlags(this.nativeStrFmt, out stringFormatFlags);
				GDIPlus.CheckStatus(status);
				return stringFormatFlags;
			}
			set
			{
				Status status = GDIPlus.GdipSetStringFormatFlags(this.nativeStrFmt, value);
				GDIPlus.CheckStatus(status);
			}
		}

		public HotkeyPrefix HotkeyPrefix
		{
			get
			{
				HotkeyPrefix hotkeyPrefix;
				Status status = GDIPlus.GdipGetStringFormatHotkeyPrefix(this.nativeStrFmt, out hotkeyPrefix);
				GDIPlus.CheckStatus(status);
				return hotkeyPrefix;
			}
			set
			{
				if (value < HotkeyPrefix.None || value > HotkeyPrefix.Hide)
				{
					throw new InvalidEnumArgumentException("HotkeyPrefix");
				}
				Status status = GDIPlus.GdipSetStringFormatHotkeyPrefix(this.nativeStrFmt, value);
				GDIPlus.CheckStatus(status);
			}
		}

		public StringTrimming Trimming
		{
			get
			{
				StringTrimming stringTrimming;
				Status status = GDIPlus.GdipGetStringFormatTrimming(this.nativeStrFmt, out stringTrimming);
				GDIPlus.CheckStatus(status);
				return stringTrimming;
			}
			set
			{
				if (value < StringTrimming.None || value > StringTrimming.EllipsisPath)
				{
					throw new InvalidEnumArgumentException("Trimming");
				}
				Status status = GDIPlus.GdipSetStringFormatTrimming(this.nativeStrFmt, value);
				GDIPlus.CheckStatus(status);
			}
		}

		public static StringFormat GenericDefault
		{
			get
			{
				IntPtr intPtr;
				Status status = GDIPlus.GdipStringFormatGetGenericDefault(out intPtr);
				GDIPlus.CheckStatus(status);
				return new StringFormat(intPtr);
			}
		}

		public int DigitSubstitutionLanguage
		{
			get
			{
				return this.language;
			}
		}

		public static StringFormat GenericTypographic
		{
			get
			{
				IntPtr intPtr;
				Status status = GDIPlus.GdipStringFormatGetGenericTypographic(out intPtr);
				GDIPlus.CheckStatus(status);
				return new StringFormat(intPtr);
			}
		}

		public StringDigitSubstitute DigitSubstitutionMethod
		{
			get
			{
				StringDigitSubstitute stringDigitSubstitute;
				Status status = GDIPlus.GdipGetStringFormatDigitSubstitution(this.nativeStrFmt, this.language, out stringDigitSubstitute);
				GDIPlus.CheckStatus(status);
				return stringDigitSubstitute;
			}
		}

		public void SetMeasurableCharacterRanges(CharacterRange[] ranges)
		{
			Status status = GDIPlus.GdipSetStringFormatMeasurableCharacterRanges(this.nativeStrFmt, ranges.Length, ranges);
			GDIPlus.CheckStatus(status);
		}

		internal int GetMeasurableCharacterRangeCount()
		{
			int num;
			Status status = GDIPlus.GdipGetStringFormatMeasurableCharacterRangeCount(this.nativeStrFmt, out num);
			GDIPlus.CheckStatus(status);
			return num;
		}

		public object Clone()
		{
			IntPtr intPtr;
			Status status = GDIPlus.GdipCloneStringFormat(this.nativeStrFmt, out intPtr);
			GDIPlus.CheckStatus(status);
			return new StringFormat(intPtr);
		}

		public override string ToString()
		{
			return "[StringFormat, FormatFlags=" + this.FormatFlags.ToString() + "]";
		}

		internal IntPtr NativeObject
		{
			get
			{
				return this.nativeStrFmt;
			}
			set
			{
				this.nativeStrFmt = value;
			}
		}

		public void SetTabStops(float firstTabOffset, float[] tabStops)
		{
			Status status = GDIPlus.GdipSetStringFormatTabStops(this.nativeStrFmt, firstTabOffset, tabStops.Length, tabStops);
			GDIPlus.CheckStatus(status);
		}

		public void SetDigitSubstitution(int language, StringDigitSubstitute substitute)
		{
			Status status = GDIPlus.GdipSetStringFormatDigitSubstitution(this.nativeStrFmt, this.language, substitute);
			GDIPlus.CheckStatus(status);
		}

		public float[] GetTabStops(out float firstTabOffset)
		{
			int num = 0;
			firstTabOffset = 0f;
			Status status = GDIPlus.GdipGetStringFormatTabStopCount(this.nativeStrFmt, out num);
			GDIPlus.CheckStatus(status);
			float[] array = new float[num];
			if (num != 0)
			{
				status = GDIPlus.GdipGetStringFormatTabStops(this.nativeStrFmt, num, out firstTabOffset, array);
				GDIPlus.CheckStatus(status);
			}
			return array;
		}

		private IntPtr nativeStrFmt;

		private int language;
	}
}
