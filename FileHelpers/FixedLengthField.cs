using System;
using System.Reflection;
using System.Text;

namespace FileHelpers
{
	public sealed class FixedLengthField : FieldBase
	{
		internal int FieldLength { get; private set; }

		internal FieldAlignAttribute Align { get; private set; }

		internal FixedMode FixedMode { get; set; }

		private FixedLengthField()
		{
		}

		internal FixedLengthField(FieldInfo fi, int length, FieldAlignAttribute align)
			: base(fi)
		{
			this.FixedMode = FixedMode.ExactLength;
			this.Align = new FieldAlignAttribute(AlignMode.Left, ' ');
			this.FieldLength = length;
			if (align != null)
			{
				this.Align = align;
				return;
			}
			if (TypeHelper.IsNumericType(fi.FieldType))
			{
				this.Align = new FieldAlignAttribute(AlignMode.Right, ' ');
			}
		}

		internal override ExtractedInfo ExtractFieldString(LineInfo line)
		{
			if (line.CurrentLength == 0)
			{
				if (base.IsOptional)
				{
					return ExtractedInfo.Empty;
				}
				throw new BadUsageException(string.Concat(new string[]
				{
					"End Of Line found processing the field: ",
					base.FieldInfo.Name,
					" at line ",
					line.mReader.LineNumber.ToString(),
					". (You need to mark it as [FieldOptional] if you want to avoid this exception)"
				}));
			}
			else if (line.CurrentLength < this.FieldLength)
			{
				if (this.FixedMode == FixedMode.AllowLessChars || this.FixedMode == FixedMode.AllowVariableLength)
				{
					return new ExtractedInfo(line);
				}
				throw new BadUsageException(string.Concat(new string[]
				{
					"The string '",
					line.CurrentString,
					"' (length ",
					line.CurrentLength.ToString(),
					") at line ",
					line.mReader.LineNumber.ToString(),
					" has less chars than the defined for ",
					base.FieldInfo.Name,
					" (",
					this.FieldLength.ToString(),
					"). You can use the [FixedLengthRecord(FixedMode.AllowLessChars)] to avoid this problem."
				}));
			}
			else
			{
				if (line.CurrentLength > this.FieldLength && !base.IsArray && base.IsLast && this.FixedMode != FixedMode.AllowMoreChars && this.FixedMode != FixedMode.AllowVariableLength)
				{
					throw new BadUsageException(string.Concat(new string[]
					{
						"The string '",
						line.CurrentString,
						"' (length ",
						line.CurrentLength.ToString(),
						") at line ",
						line.mReader.LineNumber.ToString(),
						" has more chars than the defined for the last field ",
						base.FieldInfo.Name,
						" (",
						this.FieldLength.ToString(),
						").You can use the [FixedLengthRecord(FixedMode.AllowMoreChars)] to avoid this problem."
					}));
				}
				return new ExtractedInfo(line, line.mCurrentPos + this.FieldLength);
			}
		}

		internal override void CreateFieldString(StringBuilder sb, object fieldValue, bool isLast)
		{
			string text = base.CreateFieldString(fieldValue);
			if (text.Length > this.FieldLength)
			{
				text = text.Substring(0, this.FieldLength);
			}
			if (this.Align.Align == AlignMode.Left)
			{
				sb.Append(text);
				sb.Append(this.Align.AlignChar, this.FieldLength - text.Length);
				return;
			}
			if (this.Align.Align == AlignMode.Right)
			{
				sb.Append(this.Align.AlignChar, this.FieldLength - text.Length);
				sb.Append(text);
				return;
			}
			int num = (this.FieldLength - text.Length) / 2;
			sb.Append(this.Align.AlignChar, num);
			sb.Append(text);
			sb.Append(this.Align.AlignChar, this.FieldLength - text.Length - num);
		}

		protected override FieldBase CreateClone()
		{
			return new FixedLengthField
			{
				Align = this.Align,
				FieldLength = this.FieldLength,
				FixedMode = this.FixedMode
			};
		}
	}
}
