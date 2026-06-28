using System;
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace FileHelpers
{
	public abstract class FieldBase : ICloneable
	{
		public Type FieldType { get; private set; }

		public ConverterBase Converter { get; private set; }

		internal int CharsToDiscard { get; set; }

		internal Type FieldTypeInternal { get; set; }

		public bool IsArray { get; private set; }

		public int ArrayMinLength { get; set; }

		public int ArrayMaxLength { get; set; }

		internal Type ArrayType { get; set; }

		internal bool IsFirst { get; set; }

		internal bool IsLast { get; set; }

		public bool Discarded { get; set; }

		internal bool TrailingArray { get; set; }

		internal object NullValue { get; set; }

		internal bool IsStringField { get; set; }

		internal FieldInfo FieldInfo { get; set; }

		public TrimMode TrimMode { get; set; }

		internal char[] TrimChars { get; set; }

		public bool IsOptional { get; set; }

		internal bool NextIsOptional { get; set; }

		internal bool InNewLine { get; set; }

		internal int? FieldOrder { get; set; }

		internal bool IsNullableType { get; private set; }

		internal string FieldFriendlyName { get; set; }

		public bool IsNotEmpty { get; set; }

		internal string FieldName
		{
			get
			{
				return this.FieldInfo.Name;
			}
		}

		public static FieldBase CreateField(FieldInfo fi, TypedRecordAttribute recordAttribute)
		{
			if (fi.IsDefined(typeof(FieldNotInFileAttribute), true) || fi.IsDefined(typeof(FieldIgnoredAttribute), true))
			{
				return null;
			}
			FieldBase res = null;
			FieldAttribute[] array = (FieldAttribute[])fi.GetCustomAttributes(typeof(FieldAttribute), true);
			if (recordAttribute is FixedLengthRecordAttribute && array.Length == 0)
			{
				throw new BadUsageException("The field: '" + fi.Name + "' must be marked the FieldFixedLength attribute because the record class is marked with FixedLengthRecord.");
			}
			if (array.Length > 1)
			{
				throw new BadUsageException("The field: '" + fi.Name + "' has a FieldFixedLength and a FieldDelimiter attribute.");
			}
			if (recordAttribute is DelimitedRecordAttribute && fi.IsDefined(typeof(FieldAlignAttribute), false))
			{
				throw new BadUsageException("The field: '" + fi.Name + "' can't be marked with FieldAlign attribute, it is only valid for fixed length records and are used only for write purpose.");
			}
			if (!fi.FieldType.IsArray && fi.IsDefined(typeof(FieldArrayLengthAttribute), false))
			{
				throw new BadUsageException("The field: '" + fi.Name + "' can't be marked with FieldArrayLength attribute is only valid for array fields.");
			}
			if (array.Length > 0)
			{
				FieldAttribute fieldAttribute = array[0];
				if (fieldAttribute is FieldFixedLengthAttribute)
				{
					if (recordAttribute is DelimitedRecordAttribute)
					{
						throw new BadUsageException("The field: '" + fi.Name + "' can't be marked with FieldFixedLength attribute, it is only for the FixedLengthRecords not for delimited ones.");
					}
					FieldFixedLengthAttribute fieldFixedLengthAttribute = (FieldFixedLengthAttribute)fieldAttribute;
					FieldAlignAttribute first = Attributes.GetFirst<FieldAlignAttribute>(fi);
					res = new FixedLengthField(fi, fieldFixedLengthAttribute.Length, first);
					((FixedLengthField)res).FixedMode = ((FixedLengthRecordAttribute)recordAttribute).FixedMode;
				}
				else
				{
					if (!(fieldAttribute is FieldDelimiterAttribute))
					{
						throw new BadUsageException("Custom field attributes are not currently supported. Unknown attribute: " + fieldAttribute.GetType().Name + " on field: " + fi.Name);
					}
					if (recordAttribute is FixedLengthRecordAttribute)
					{
						throw new BadUsageException("The field: '" + fi.Name + "' can't be marked with FieldDelimiter attribute, it is only for DelimitedRecords not for fixed ones.");
					}
					res = new DelimitedField(fi, ((FieldDelimiterAttribute)fieldAttribute).Delimiter);
				}
			}
			else
			{
				DelimitedRecordAttribute delimitedRecordAttribute = recordAttribute as DelimitedRecordAttribute;
				if (delimitedRecordAttribute != null)
				{
					res = new DelimitedField(fi, delimitedRecordAttribute.Separator);
				}
			}
			if (res != null)
			{
				res.Discarded = fi.IsDefined(typeof(FieldValueDiscardedAttribute), false);
				Attributes.WorkWithFirst<FieldTrimAttribute>(fi, delegate(FieldTrimAttribute x)
				{
					res.TrimMode = x.TrimMode;
					res.TrimChars = x.TrimChars;
				});
				Attributes.WorkWithFirst<FieldQuotedAttribute>(fi, delegate(FieldQuotedAttribute x)
				{
					if (res is FixedLengthField)
					{
						throw new BadUsageException("The field: '" + fi.Name + "' can't be marked with FieldQuoted attribute, it is only for the delimited records.");
					}
					((DelimitedField)res).QuoteChar = x.QuoteChar;
					((DelimitedField)res).QuoteMode = x.QuoteMode;
					((DelimitedField)res).QuoteMultiline = x.QuoteMultiline;
				});
				Attributes.WorkWithFirst<FieldOrderAttribute>(fi, delegate(FieldOrderAttribute x)
				{
					res.FieldOrder = new int?(x.Order);
				});
				res.IsOptional = fi.IsDefined(typeof(FieldOptionalAttribute), false);
				res.InNewLine = fi.IsDefined(typeof(FieldInNewLineAttribute), false);
				res.IsNotEmpty = fi.IsDefined(typeof(FieldNotEmptyAttribute), false);
				if (fi.FieldType.IsArray)
				{
					res.IsArray = true;
					res.ArrayType = fi.FieldType.GetElementType();
					res.ArrayMinLength = int.MinValue;
					res.ArrayMaxLength = int.MaxValue;
					Attributes.WorkWithFirst<FieldArrayLengthAttribute>(fi, delegate(FieldArrayLengthAttribute x)
					{
						res.ArrayMinLength = x.MinLength;
						res.ArrayMaxLength = x.MaxLength;
						if (res.ArrayMaxLength < res.ArrayMinLength || res.ArrayMinLength < 0 || res.ArrayMaxLength <= 0)
						{
							throw new BadUsageException("The field: " + fi.Name + " has invalid length values in the [FieldArrayLength] attribute.");
						}
					});
				}
			}
			if (fi.IsDefined(typeof(CompilerGeneratedAttribute), false))
			{
				if (fi.Name.EndsWith("__BackingField") && fi.Name.StartsWith("<") && fi.Name.Contains(">"))
				{
					res.FieldFriendlyName = fi.Name.Substring(1, fi.Name.IndexOf(">") - 1);
				}
				res.IsAutoProperty = true;
				PropertyInfo property = fi.DeclaringType.GetProperty(res.FieldFriendlyName);
				if (property != null)
				{
					Attributes.WorkWithFirst<FieldOrderAttribute>(property, delegate(FieldOrderAttribute x)
					{
						res.FieldOrder = new int?(x.Order);
					});
				}
			}
			if (string.IsNullOrEmpty(res.FieldFriendlyName))
			{
				res.FieldFriendlyName = res.FieldName;
			}
			return res;
		}

		internal static string AutoPropertyName(FieldInfo fi)
		{
			if (fi.IsDefined(typeof(CompilerGeneratedAttribute), false) && fi.Name.EndsWith("__BackingField") && fi.Name.StartsWith("<") && fi.Name.Contains(">"))
			{
				return fi.Name.Substring(1, fi.Name.IndexOf(">") - 1);
			}
			return "";
		}

		internal bool IsAutoProperty { get; set; }

		internal FieldBase()
		{
			this.IsNullableType = false;
			this.TrimMode = TrimMode.None;
			this.FieldOrder = null;
			this.InNewLine = false;
			this.NextIsOptional = false;
			this.IsOptional = false;
			this.TrimChars = null;
			this.NullValue = null;
			this.TrailingArray = false;
			this.IsLast = false;
			this.IsFirst = false;
			this.IsArray = false;
			this.CharsToDiscard = 0;
			this.IsNotEmpty = false;
		}

		internal FieldBase(FieldInfo fi)
			: this()
		{
			this.FieldInfo = fi;
			this.FieldType = this.FieldInfo.FieldType;
			if (this.FieldType.IsArray)
			{
				this.FieldTypeInternal = this.FieldType.GetElementType();
			}
			else
			{
				this.FieldTypeInternal = this.FieldType;
			}
			this.IsStringField = this.FieldTypeInternal == typeof(string);
			object[] array = fi.GetCustomAttributes(typeof(FieldConverterAttribute), true);
			if (array.Length > 0)
			{
				FieldConverterAttribute fieldConverterAttribute = (FieldConverterAttribute)array[0];
				this.Converter = fieldConverterAttribute.Converter;
				fieldConverterAttribute.ValidateTypes(this.FieldInfo);
			}
			else
			{
				this.Converter = ConvertHelpers.GetDefaultConverter(fi.Name, this.FieldType);
			}
			if (this.Converter != null)
			{
				this.Converter.mDestinationType = this.FieldTypeInternal;
			}
			array = fi.GetCustomAttributes(typeof(FieldNullValueAttribute), true);
			if (array.Length > 0)
			{
				this.NullValue = ((FieldNullValueAttribute)array[0]).NullValue;
				if (this.NullValue != null && !this.FieldTypeInternal.IsAssignableFrom(this.NullValue.GetType()))
				{
					throw new BadUsageException(string.Concat(new string[]
					{
						"The NullValue is of type: ",
						this.NullValue.GetType().Name,
						" that is not asignable to the field ",
						this.FieldInfo.Name,
						" of type: ",
						this.FieldTypeInternal.Name
					}));
				}
			}
			this.IsNullableType = this.FieldTypeInternal.IsValueType && this.FieldTypeInternal.IsGenericType && this.FieldTypeInternal.GetGenericTypeDefinition() == typeof(Nullable<>);
		}

		internal abstract ExtractedInfo ExtractFieldString(LineInfo line);

		internal abstract void CreateFieldString(StringBuilder sb, object fieldValue, bool isLast);

		internal string CreateFieldString(object fieldValue)
		{
			if (this.Converter != null)
			{
				return this.Converter.FieldToString(fieldValue);
			}
			if (fieldValue == null)
			{
				return string.Empty;
			}
			return fieldValue.ToString();
		}

		internal object ExtractFieldValue(LineInfo line)
		{
			if (this.InNewLine)
			{
				if (!line.EmptyFromPos())
				{
					throw new BadUsageException(line, string.Concat(new string[]
					{
						"Text '",
						line.CurrentString,
						"' found before the new line of the field: ",
						this.FieldInfo.Name,
						" (this is not allowed when you use [FieldInNewLine])"
					}));
				}
				line.ReLoad(line.mReader.ReadNextLine());
				if (line.mLineStr == null)
				{
					throw new BadUsageException(line, "End of stream found parsing the field " + this.FieldInfo.Name + ". Please check the class record.");
				}
			}
			if (!this.IsArray)
			{
				ExtractedInfo extractedInfo = this.ExtractFieldString(line);
				if (extractedInfo.mCustomExtractedString == null)
				{
					line.mCurrentPos = extractedInfo.ExtractedTo + 1;
				}
				line.mCurrentPos += this.CharsToDiscard;
				if (this.Discarded)
				{
					return this.GetDiscardedNullValue();
				}
				return this.AssignFromString(extractedInfo, line).Value;
			}
			else
			{
				if (this.ArrayMinLength <= 0)
				{
					this.ArrayMinLength = 0;
				}
				int num = 0;
				ArrayList arrayList = new ArrayList(Math.Max(this.ArrayMinLength, 10));
				while (line.mCurrentPos - this.CharsToDiscard < line.mLineStr.Length && num < this.ArrayMaxLength)
				{
					ExtractedInfo extractedInfo2 = this.ExtractFieldString(line);
					if (extractedInfo2.mCustomExtractedString == null)
					{
						line.mCurrentPos = extractedInfo2.ExtractedTo + 1;
					}
					line.mCurrentPos += this.CharsToDiscard;
					try
					{
						FieldBase.AssignResult assignResult = this.AssignFromString(extractedInfo2, line);
						if (assignResult.NullValueUsed && num == 0 && line.IsEOL())
						{
							break;
						}
						arrayList.Add(assignResult.Value);
					}
					catch (NullValueNotFoundException)
					{
						if (num == 0)
						{
							break;
						}
						throw;
					}
					num++;
				}
				if (arrayList.Count < this.ArrayMinLength)
				{
					throw new InvalidOperationException(string.Format("Line: {0} Column: {1} Field: {2}. The array has only {3} values, less than the minimum length of {4}", new object[]
					{
						line.mReader.LineNumber.ToString(),
						line.mCurrentPos.ToString(),
						this.FieldInfo.Name,
						arrayList.Count,
						this.ArrayMinLength
					}));
				}
				if (this.IsLast && !line.IsEOL())
				{
					throw new InvalidOperationException(string.Format("Line: {0} Column: {1} Field: {2}. The array has more values than the maximum length of {3}", new object[]
					{
						line.mReader.LineNumber,
						line.mCurrentPos,
						this.FieldInfo.Name,
						this.ArrayMaxLength
					}));
				}
				if (this.Discarded)
				{
					return null;
				}
				return arrayList.ToArray(this.ArrayType);
			}
		}

		private FieldBase.AssignResult AssignFromString(ExtractedInfo fieldString, LineInfo line)
		{
			string text = fieldString.ExtractedString();
			FieldBase.AssignResult assignResult;
			try
			{
				if (this.IsNotEmpty && string.IsNullOrEmpty(text))
				{
					throw new InvalidOperationException("The value is empty and must be populated.");
				}
				object obj;
				if (this.Converter == null)
				{
					if (this.IsStringField)
					{
						obj = this.TrimString(text);
					}
					else
					{
						text = text.Trim();
						if (text.Length == 0)
						{
							return new FieldBase.AssignResult
							{
								Value = this.GetNullValue(line),
								NullValueUsed = true
							};
						}
						obj = Convert.ChangeType(text, this.FieldTypeInternal, null);
					}
				}
				else
				{
					string text2 = text.Trim();
					if (!this.Converter.CustomNullHandling && text2.Length == 0)
					{
						return new FieldBase.AssignResult
						{
							Value = this.GetNullValue(line),
							NullValueUsed = true
						};
					}
					if (this.TrimMode == TrimMode.Both)
					{
						obj = this.Converter.StringToField(text2);
					}
					else
					{
						obj = this.Converter.StringToField(this.TrimString(text));
					}
					if (obj == null)
					{
						return new FieldBase.AssignResult
						{
							Value = this.GetNullValue(line),
							NullValueUsed = true
						};
					}
				}
				assignResult = new FieldBase.AssignResult
				{
					Value = obj
				};
			}
			catch (ConvertException ex)
			{
				ex.FieldName = this.FieldInfo.Name;
				ex.LineNumber = line.mReader.LineNumber;
				ex.ColumnNumber = fieldString.ExtractedFrom + 1;
				throw;
			}
			catch (BadUsageException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				if (this.Converter == null || this.Converter.GetType().Assembly == typeof(FieldBase).Assembly)
				{
					throw new ConvertException(text, this.FieldTypeInternal, this.FieldInfo.Name, line.mReader.LineNumber, fieldString.ExtractedFrom + 1, ex2.Message, ex2);
				}
				throw new ConvertException(text, this.FieldTypeInternal, this.FieldInfo.Name, line.mReader.LineNumber, fieldString.ExtractedFrom + 1, string.Concat(new string[]
				{
					"Your custom converter: ",
					this.Converter.GetType().Name,
					" throws an ",
					ex2.GetType().Name,
					" with the message: ",
					ex2.Message
				}), ex2);
			}
			return assignResult;
		}

		private string TrimString(string extractedString)
		{
			switch (this.TrimMode)
			{
			case TrimMode.None:
				return extractedString;
			case TrimMode.Both:
				return extractedString.Trim();
			case TrimMode.Left:
				return extractedString.TrimStart(new char[0]);
			case TrimMode.Right:
				return extractedString.TrimEnd(new char[0]);
			default:
				throw new Exception("Trim mode invalid in FieldBase.TrimString -> " + this.TrimMode.ToString());
			}
		}

		private object GetNullValue(LineInfo line)
		{
			if (this.NullValue != null)
			{
				return this.NullValue;
			}
			if (!this.FieldTypeInternal.IsValueType)
			{
				return null;
			}
			if (this.IsNullableType)
			{
				return null;
			}
			string text = string.Concat(new string[]
			{
				"Not value found for the value type field: '",
				this.FieldInfo.Name,
				"' Class: '",
				this.FieldInfo.DeclaringType.Name,
				"'. ",
				Environment.NewLine,
				"You must use the [FieldNullValue] attribute because this is a value type and can't be null or use a Nullable Type instead of the current type."
			});
			throw new NullValueNotFoundException(line, text);
		}

		private object GetDiscardedNullValue()
		{
			if (this.NullValue != null)
			{
				return this.NullValue;
			}
			if (!this.FieldTypeInternal.IsValueType)
			{
				return null;
			}
			if (this.IsNullableType)
			{
				return null;
			}
			string text = string.Concat(new string[]
			{
				"The field: '",
				this.FieldInfo.Name,
				"' Class: '",
				this.FieldInfo.DeclaringType.Name,
				"' is from a value type: ",
				this.FieldInfo.FieldType.Name,
				" and is discarded (null) you must provide a [FieldNullValue] attribute."
			});
			throw new BadUsageException(text);
		}

		public object CreateValueForField(object fieldValue)
		{
			object obj = null;
			if (fieldValue == null)
			{
				if (this.NullValue == null)
				{
					if (this.FieldTypeInternal.IsValueType && Nullable.GetUnderlyingType(this.FieldTypeInternal) == null)
					{
						throw new BadUsageException(string.Concat(new string[]
						{
							"Null Value found. You must specify a FieldNullValueAttribute in the ",
							this.FieldInfo.Name,
							" field of type ",
							this.FieldTypeInternal.Name,
							", because this is a ValueType."
						}));
					}
					obj = null;
				}
				else
				{
					obj = this.NullValue;
				}
			}
			else if (this.FieldTypeInternal == fieldValue.GetType())
			{
				obj = fieldValue;
			}
			else if (this.Converter == null)
			{
				obj = Convert.ChangeType(fieldValue, this.FieldTypeInternal, null);
			}
			else
			{
				try
				{
					if (Nullable.GetUnderlyingType(this.FieldTypeInternal) != null && Nullable.GetUnderlyingType(this.FieldTypeInternal) == fieldValue.GetType())
					{
						obj = fieldValue;
					}
					else
					{
						obj = Convert.ChangeType(fieldValue, this.FieldTypeInternal, null);
					}
				}
				catch
				{
					obj = this.Converter.StringToField(fieldValue.ToString());
				}
			}
			return obj;
		}

		internal void AssignToString(StringBuilder sb, object fieldValue)
		{
			if (this.InNewLine)
			{
				sb.Append(StringHelper.NewLine);
			}
			if (!this.IsArray)
			{
				this.CreateFieldString(sb, fieldValue, this.IsLast);
				return;
			}
			if (fieldValue == null)
			{
				if (0 < this.ArrayMinLength)
				{
					throw new InvalidOperationException(string.Format("Field: {0}. The array is null, but the minimum length is {1}", this.FieldInfo.Name, this.ArrayMinLength));
				}
				return;
			}
			else
			{
				IList list = (IList)fieldValue;
				if (list.Count < this.ArrayMinLength)
				{
					throw new InvalidOperationException(string.Format("Field: {0}. The array has {1} values, but the minimum length is {2}", this.FieldInfo.Name, list.Count, this.ArrayMinLength));
				}
				if (list.Count > this.ArrayMaxLength)
				{
					throw new InvalidOperationException(string.Format("Field: {0}. The array has {1} values, but the maximum length is {2}", this.FieldInfo.Name, list.Count, this.ArrayMaxLength));
				}
				for (int i = 0; i < list.Count; i++)
				{
					object obj = list[i];
					this.CreateFieldString(sb, obj, this.IsLast && i == list.Count - 1);
				}
				return;
			}
		}

		public object Clone()
		{
			FieldBase fieldBase = this.CreateClone();
			fieldBase.FieldType = this.FieldType;
			fieldBase.CharsToDiscard = this.CharsToDiscard;
			fieldBase.Converter = this.Converter;
			fieldBase.FieldTypeInternal = this.FieldTypeInternal;
			fieldBase.IsArray = this.IsArray;
			fieldBase.ArrayType = this.ArrayType;
			fieldBase.ArrayMinLength = this.ArrayMinLength;
			fieldBase.ArrayMaxLength = this.ArrayMaxLength;
			fieldBase.IsFirst = this.IsFirst;
			fieldBase.IsLast = this.IsLast;
			fieldBase.TrailingArray = this.TrailingArray;
			fieldBase.NullValue = this.NullValue;
			fieldBase.IsStringField = this.IsStringField;
			fieldBase.FieldInfo = this.FieldInfo;
			fieldBase.TrimMode = this.TrimMode;
			fieldBase.TrimChars = this.TrimChars;
			fieldBase.IsOptional = this.IsOptional;
			fieldBase.NextIsOptional = this.NextIsOptional;
			fieldBase.InNewLine = this.InNewLine;
			fieldBase.FieldOrder = this.FieldOrder;
			fieldBase.IsNullableType = this.IsNullableType;
			fieldBase.Discarded = this.Discarded;
			fieldBase.FieldFriendlyName = this.FieldFriendlyName;
			fieldBase.IsNotEmpty = this.IsNotEmpty;
			return fieldBase;
		}

		protected abstract FieldBase CreateClone();

		private static readonly char[] mWhitespaceChars = new char[]
		{
			'\t', '\n', '\v', '\f', '\r', ' ', '\u00a0', '\u2000', '\u2001', '\u2002',
			'\u2003', '\u2004', '\u2005', '\u2006', '\u2007', '\u2008', '\u2009', '\u200a', '\u200b', '\u3000',
			'\ufeff'
		};

		private struct AssignResult
		{
			public object Value;

			public bool NullValueUsed;
		}
	}
}
