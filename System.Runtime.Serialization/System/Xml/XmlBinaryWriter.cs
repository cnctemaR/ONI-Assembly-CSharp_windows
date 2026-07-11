using System;
using System.IO;
using System.Runtime.Serialization;
using System.Security;

namespace System.Xml
{
	internal class XmlBinaryWriter : XmlBaseWriter, IXmlBinaryWriterInitializer
	{
		public void SetOutput(Stream stream, IXmlDictionary dictionary, XmlBinaryWriterSession session, bool ownsStream)
		{
			if (stream == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("stream"));
			}
			if (this.writer == null)
			{
				this.writer = new XmlBinaryNodeWriter();
			}
			this.writer.SetOutput(stream, dictionary, session, ownsStream);
			base.SetOutput(this.writer);
		}

		protected override XmlSigningNodeWriter CreateSigningNodeWriter()
		{
			return new XmlSigningNodeWriter(false);
		}

		protected override void WriteTextNode(XmlDictionaryReader reader, bool attribute)
		{
			Type valueType = reader.ValueType;
			if (valueType == typeof(string))
			{
				XmlDictionaryString xmlDictionaryString;
				if (reader.TryGetValueAsDictionaryString(out xmlDictionaryString))
				{
					this.WriteString(xmlDictionaryString);
				}
				else if (reader.CanReadValueChunk)
				{
					if (this.chars == null)
					{
						this.chars = new char[256];
					}
					int num;
					while ((num = reader.ReadValueChunk(this.chars, 0, this.chars.Length)) > 0)
					{
						this.WriteChars(this.chars, 0, num);
					}
				}
				else
				{
					this.WriteString(reader.Value);
				}
				if (!attribute)
				{
					reader.Read();
					return;
				}
			}
			else if (valueType == typeof(byte[]))
			{
				if (reader.CanReadBinaryContent)
				{
					if (this.bytes == null)
					{
						this.bytes = new byte[384];
					}
					int num2;
					while ((num2 = reader.ReadValueAsBase64(this.bytes, 0, this.bytes.Length)) > 0)
					{
						this.WriteBase64(this.bytes, 0, num2);
					}
				}
				else
				{
					this.WriteString(reader.Value);
				}
				if (!attribute)
				{
					reader.Read();
					return;
				}
			}
			else
			{
				if (valueType == typeof(int))
				{
					this.WriteValue(reader.ReadContentAsInt());
					return;
				}
				if (valueType == typeof(long))
				{
					this.WriteValue(reader.ReadContentAsLong());
					return;
				}
				if (valueType == typeof(bool))
				{
					this.WriteValue(reader.ReadContentAsBoolean());
					return;
				}
				if (valueType == typeof(double))
				{
					this.WriteValue(reader.ReadContentAsDouble());
					return;
				}
				if (valueType == typeof(DateTime))
				{
					this.WriteValue(reader.ReadContentAsDateTime());
					return;
				}
				if (valueType == typeof(float))
				{
					this.WriteValue(reader.ReadContentAsFloat());
					return;
				}
				if (valueType == typeof(decimal))
				{
					this.WriteValue(reader.ReadContentAsDecimal());
					return;
				}
				if (valueType == typeof(UniqueId))
				{
					this.WriteValue(reader.ReadContentAsUniqueId());
					return;
				}
				if (valueType == typeof(Guid))
				{
					this.WriteValue(reader.ReadContentAsGuid());
					return;
				}
				if (valueType == typeof(TimeSpan))
				{
					this.WriteValue(reader.ReadContentAsTimeSpan());
					return;
				}
				this.WriteValue(reader.ReadContentAsObject());
			}
		}

		private void WriteStartArray(string prefix, string localName, string namespaceUri, int count)
		{
			base.StartArray(count);
			this.writer.WriteArrayNode();
			this.WriteStartElement(prefix, localName, namespaceUri);
			this.WriteEndElement();
		}

		private void WriteStartArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, int count)
		{
			base.StartArray(count);
			this.writer.WriteArrayNode();
			this.WriteStartElement(prefix, localName, namespaceUri);
			this.WriteEndElement();
		}

		private void WriteEndArray()
		{
			base.EndArray();
		}

		[SecurityCritical]
		private unsafe void UnsafeWriteArray(string prefix, string localName, string namespaceUri, XmlBinaryNodeType nodeType, int count, byte* array, byte* arrayMax)
		{
			this.WriteStartArray(prefix, localName, namespaceUri, count);
			this.writer.UnsafeWriteArray(nodeType, count, array, arrayMax);
			this.WriteEndArray();
		}

		[SecurityCritical]
		private unsafe void UnsafeWriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, XmlBinaryNodeType nodeType, int count, byte* array, byte* arrayMax)
		{
			this.WriteStartArray(prefix, localName, namespaceUri, count);
			this.writer.UnsafeWriteArray(nodeType, count, array, arrayMax);
			this.WriteEndArray();
		}

		private void CheckArray(Array array, int offset, int count)
		{
			if (array == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("array"));
			}
			if (offset < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (offset > array.Length)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", global::System.Runtime.Serialization.SR.GetString("The specified offset exceeds the buffer size ({0} bytes).", new object[] { array.Length })));
			}
			if (count < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (count > array.Length - offset)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The specified size exceeds the remaining buffer space ({0} bytes).", new object[] { array.Length - offset })));
			}
		}

		[SecuritySafeCritical]
		public unsafe override void WriteArray(string prefix, string localName, string namespaceUri, bool[] array, int offset, int count)
		{
			if (base.Signing)
			{
				base.WriteArray(prefix, localName, namespaceUri, array, offset, count);
				return;
			}
			this.CheckArray(array, offset, count);
			if (count > 0)
			{
				fixed (bool* ptr = &array[offset])
				{
					bool* ptr2 = ptr;
					this.UnsafeWriteArray(prefix, localName, namespaceUri, XmlBinaryNodeType.BoolTextWithEndElement, count, (byte*)ptr2, (byte*)(ptr2 + count));
				}
			}
		}

		[SecuritySafeCritical]
		public unsafe override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, bool[] array, int offset, int count)
		{
			if (base.Signing)
			{
				base.WriteArray(prefix, localName, namespaceUri, array, offset, count);
				return;
			}
			this.CheckArray(array, offset, count);
			if (count > 0)
			{
				fixed (bool* ptr = &array[offset])
				{
					bool* ptr2 = ptr;
					this.UnsafeWriteArray(prefix, localName, namespaceUri, XmlBinaryNodeType.BoolTextWithEndElement, count, (byte*)ptr2, (byte*)(ptr2 + count));
				}
			}
		}

		[SecuritySafeCritical]
		public unsafe override void WriteArray(string prefix, string localName, string namespaceUri, short[] array, int offset, int count)
		{
			if (base.Signing)
			{
				base.WriteArray(prefix, localName, namespaceUri, array, offset, count);
				return;
			}
			this.CheckArray(array, offset, count);
			if (count > 0)
			{
				fixed (short* ptr = &array[offset])
				{
					short* ptr2 = ptr;
					this.UnsafeWriteArray(prefix, localName, namespaceUri, XmlBinaryNodeType.Int16TextWithEndElement, count, (byte*)ptr2, (byte*)(ptr2 + count));
				}
			}
		}

		[SecuritySafeCritical]
		public unsafe override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, short[] array, int offset, int count)
		{
			if (base.Signing)
			{
				base.WriteArray(prefix, localName, namespaceUri, array, offset, count);
				return;
			}
			this.CheckArray(array, offset, count);
			if (count > 0)
			{
				fixed (short* ptr = &array[offset])
				{
					short* ptr2 = ptr;
					this.UnsafeWriteArray(prefix, localName, namespaceUri, XmlBinaryNodeType.Int16TextWithEndElement, count, (byte*)ptr2, (byte*)(ptr2 + count));
				}
			}
		}

		[SecuritySafeCritical]
		public unsafe override void WriteArray(string prefix, string localName, string namespaceUri, int[] array, int offset, int count)
		{
			if (base.Signing)
			{
				base.WriteArray(prefix, localName, namespaceUri, array, offset, count);
				return;
			}
			this.CheckArray(array, offset, count);
			if (count > 0)
			{
				fixed (int* ptr = &array[offset])
				{
					int* ptr2 = ptr;
					this.UnsafeWriteArray(prefix, localName, namespaceUri, XmlBinaryNodeType.Int32TextWithEndElement, count, (byte*)ptr2, (byte*)(ptr2 + count));
				}
			}
		}

		[SecuritySafeCritical]
		public unsafe override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, int[] array, int offset, int count)
		{
			if (base.Signing)
			{
				base.WriteArray(prefix, localName, namespaceUri, array, offset, count);
				return;
			}
			this.CheckArray(array, offset, count);
			if (count > 0)
			{
				fixed (int* ptr = &array[offset])
				{
					int* ptr2 = ptr;
					this.UnsafeWriteArray(prefix, localName, namespaceUri, XmlBinaryNodeType.Int32TextWithEndElement, count, (byte*)ptr2, (byte*)(ptr2 + count));
				}
			}
		}

		[SecuritySafeCritical]
		public unsafe override void WriteArray(string prefix, string localName, string namespaceUri, long[] array, int offset, int count)
		{
			if (base.Signing)
			{
				base.WriteArray(prefix, localName, namespaceUri, array, offset, count);
				return;
			}
			this.CheckArray(array, offset, count);
			if (count > 0)
			{
				fixed (long* ptr = &array[offset])
				{
					long* ptr2 = ptr;
					this.UnsafeWriteArray(prefix, localName, namespaceUri, XmlBinaryNodeType.Int64TextWithEndElement, count, (byte*)ptr2, (byte*)(ptr2 + count));
				}
			}
		}

		[SecuritySafeCritical]
		public unsafe override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, long[] array, int offset, int count)
		{
			if (base.Signing)
			{
				base.WriteArray(prefix, localName, namespaceUri, array, offset, count);
				return;
			}
			this.CheckArray(array, offset, count);
			if (count > 0)
			{
				fixed (long* ptr = &array[offset])
				{
					long* ptr2 = ptr;
					this.UnsafeWriteArray(prefix, localName, namespaceUri, XmlBinaryNodeType.Int64TextWithEndElement, count, (byte*)ptr2, (byte*)(ptr2 + count));
				}
			}
		}

		[SecuritySafeCritical]
		public unsafe override void WriteArray(string prefix, string localName, string namespaceUri, float[] array, int offset, int count)
		{
			if (base.Signing)
			{
				base.WriteArray(prefix, localName, namespaceUri, array, offset, count);
				return;
			}
			this.CheckArray(array, offset, count);
			if (count > 0)
			{
				fixed (float* ptr = &array[offset])
				{
					float* ptr2 = ptr;
					this.UnsafeWriteArray(prefix, localName, namespaceUri, XmlBinaryNodeType.FloatTextWithEndElement, count, (byte*)ptr2, (byte*)(ptr2 + count));
				}
			}
		}

		[SecuritySafeCritical]
		public unsafe override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, float[] array, int offset, int count)
		{
			if (base.Signing)
			{
				base.WriteArray(prefix, localName, namespaceUri, array, offset, count);
				return;
			}
			this.CheckArray(array, offset, count);
			if (count > 0)
			{
				fixed (float* ptr = &array[offset])
				{
					float* ptr2 = ptr;
					this.UnsafeWriteArray(prefix, localName, namespaceUri, XmlBinaryNodeType.FloatTextWithEndElement, count, (byte*)ptr2, (byte*)(ptr2 + count));
				}
			}
		}

		[SecuritySafeCritical]
		public unsafe override void WriteArray(string prefix, string localName, string namespaceUri, double[] array, int offset, int count)
		{
			if (base.Signing)
			{
				base.WriteArray(prefix, localName, namespaceUri, array, offset, count);
				return;
			}
			this.CheckArray(array, offset, count);
			if (count > 0)
			{
				fixed (double* ptr = &array[offset])
				{
					double* ptr2 = ptr;
					this.UnsafeWriteArray(prefix, localName, namespaceUri, XmlBinaryNodeType.DoubleTextWithEndElement, count, (byte*)ptr2, (byte*)(ptr2 + count));
				}
			}
		}

		[SecuritySafeCritical]
		public unsafe override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, double[] array, int offset, int count)
		{
			if (base.Signing)
			{
				base.WriteArray(prefix, localName, namespaceUri, array, offset, count);
				return;
			}
			this.CheckArray(array, offset, count);
			if (count > 0)
			{
				fixed (double* ptr = &array[offset])
				{
					double* ptr2 = ptr;
					this.UnsafeWriteArray(prefix, localName, namespaceUri, XmlBinaryNodeType.DoubleTextWithEndElement, count, (byte*)ptr2, (byte*)(ptr2 + count));
				}
			}
		}

		[SecuritySafeCritical]
		public unsafe override void WriteArray(string prefix, string localName, string namespaceUri, decimal[] array, int offset, int count)
		{
			if (base.Signing)
			{
				base.WriteArray(prefix, localName, namespaceUri, array, offset, count);
				return;
			}
			this.CheckArray(array, offset, count);
			if (count > 0)
			{
				fixed (decimal* ptr = &array[offset])
				{
					decimal* ptr2 = ptr;
					this.UnsafeWriteArray(prefix, localName, namespaceUri, XmlBinaryNodeType.DecimalTextWithEndElement, count, (byte*)ptr2, (byte*)(ptr2 + (IntPtr)count * 16 / (IntPtr)sizeof(decimal)));
				}
			}
		}

		[SecuritySafeCritical]
		public unsafe override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, decimal[] array, int offset, int count)
		{
			if (base.Signing)
			{
				base.WriteArray(prefix, localName, namespaceUri, array, offset, count);
				return;
			}
			this.CheckArray(array, offset, count);
			if (count > 0)
			{
				fixed (decimal* ptr = &array[offset])
				{
					decimal* ptr2 = ptr;
					this.UnsafeWriteArray(prefix, localName, namespaceUri, XmlBinaryNodeType.DecimalTextWithEndElement, count, (byte*)ptr2, (byte*)(ptr2 + (IntPtr)count * 16 / (IntPtr)sizeof(decimal)));
				}
			}
		}

		public override void WriteArray(string prefix, string localName, string namespaceUri, DateTime[] array, int offset, int count)
		{
			if (base.Signing)
			{
				base.WriteArray(prefix, localName, namespaceUri, array, offset, count);
				return;
			}
			this.CheckArray(array, offset, count);
			if (count > 0)
			{
				this.WriteStartArray(prefix, localName, namespaceUri, count);
				this.writer.WriteDateTimeArray(array, offset, count);
				this.WriteEndArray();
			}
		}

		public override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, DateTime[] array, int offset, int count)
		{
			if (base.Signing)
			{
				base.WriteArray(prefix, localName, namespaceUri, array, offset, count);
				return;
			}
			this.CheckArray(array, offset, count);
			if (count > 0)
			{
				this.WriteStartArray(prefix, localName, namespaceUri, count);
				this.writer.WriteDateTimeArray(array, offset, count);
				this.WriteEndArray();
			}
		}

		public override void WriteArray(string prefix, string localName, string namespaceUri, Guid[] array, int offset, int count)
		{
			if (base.Signing)
			{
				base.WriteArray(prefix, localName, namespaceUri, array, offset, count);
				return;
			}
			this.CheckArray(array, offset, count);
			if (count > 0)
			{
				this.WriteStartArray(prefix, localName, namespaceUri, count);
				this.writer.WriteGuidArray(array, offset, count);
				this.WriteEndArray();
			}
		}

		public override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, Guid[] array, int offset, int count)
		{
			if (base.Signing)
			{
				base.WriteArray(prefix, localName, namespaceUri, array, offset, count);
				return;
			}
			this.CheckArray(array, offset, count);
			if (count > 0)
			{
				this.WriteStartArray(prefix, localName, namespaceUri, count);
				this.writer.WriteGuidArray(array, offset, count);
				this.WriteEndArray();
			}
		}

		public override void WriteArray(string prefix, string localName, string namespaceUri, TimeSpan[] array, int offset, int count)
		{
			if (base.Signing)
			{
				base.WriteArray(prefix, localName, namespaceUri, array, offset, count);
				return;
			}
			this.CheckArray(array, offset, count);
			if (count > 0)
			{
				this.WriteStartArray(prefix, localName, namespaceUri, count);
				this.writer.WriteTimeSpanArray(array, offset, count);
				this.WriteEndArray();
			}
		}

		public override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, TimeSpan[] array, int offset, int count)
		{
			if (base.Signing)
			{
				base.WriteArray(prefix, localName, namespaceUri, array, offset, count);
				return;
			}
			this.CheckArray(array, offset, count);
			if (count > 0)
			{
				this.WriteStartArray(prefix, localName, namespaceUri, count);
				this.writer.WriteTimeSpanArray(array, offset, count);
				this.WriteEndArray();
			}
		}

		private XmlBinaryNodeWriter writer;

		private char[] chars;

		private byte[] bytes;
	}
}
