using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;

namespace System.Xml
{
	internal class MimeHeaderReader
	{
		public MimeHeaderReader(Stream stream)
		{
			if (stream == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("stream");
			}
			this.stream = stream;
		}

		public string Value
		{
			get
			{
				return this.value;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public void Close()
		{
			this.stream.Close();
			this.readState = MimeHeaderReader.ReadState.EOF;
		}

		public bool Read(int maxBuffer, ref int remaining)
		{
			this.name = null;
			this.value = null;
			while (this.readState != MimeHeaderReader.ReadState.EOF)
			{
				if (this.offset == this.maxOffset)
				{
					this.maxOffset = this.stream.Read(this.buffer, 0, this.buffer.Length);
					this.offset = 0;
					if (this.BufferEnd())
					{
						break;
					}
				}
				if (this.ProcessBuffer(maxBuffer, ref remaining))
				{
					break;
				}
			}
			return this.value != null;
		}

		[SecuritySafeCritical]
		[PermissionSet(SecurityAction.Demand, Unrestricted = true)]
		private unsafe bool ProcessBuffer(int maxBuffer, ref int remaining)
		{
			byte[] array;
			byte* ptr;
			if ((array = this.buffer) == null || array.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &array[0];
			}
			byte* ptr2 = ptr + this.offset;
			byte* ptr3 = ptr + this.maxOffset;
			byte* ptr4 = ptr2;
			switch (this.readState)
			{
			case MimeHeaderReader.ReadState.ReadName:
				while (ptr4 < ptr3)
				{
					if (*ptr4 == 58)
					{
						this.AppendName(new string((sbyte*)ptr2, 0, (int)((long)(ptr4 - ptr2))), maxBuffer, ref remaining);
						ptr4++;
						goto IL_016E;
					}
					if (*ptr4 >= 65 && *ptr4 <= 90)
					{
						byte* ptr5 = ptr4;
						*ptr5 += 32;
					}
					else if (*ptr4 < 33 || *ptr4 > 126)
					{
						if (this.name != null || *ptr4 != 13)
						{
							string text = "MIME header has an invalid character ('{0}', {1} in hexadecimal value).";
							object[] array2 = new object[2];
							array2[0] = (char)(*ptr4);
							int num = 1;
							int num2 = (int)(*ptr4);
							array2[num] = num2.ToString("X", CultureInfo.InvariantCulture);
							throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new FormatException(global::System.Runtime.Serialization.SR.GetString(text, array2)));
						}
						ptr4++;
						if (ptr4 >= ptr3 || *ptr4 != 10)
						{
							throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new FormatException(global::System.Runtime.Serialization.SR.GetString("Malformed MIME header.")));
						}
						goto IL_025F;
					}
					ptr4++;
				}
				this.AppendName(new string((sbyte*)ptr2, 0, (int)((long)(ptr4 - ptr2))), maxBuffer, ref remaining);
				this.readState = MimeHeaderReader.ReadState.ReadName;
				goto IL_0276;
			case MimeHeaderReader.ReadState.SkipWS:
				break;
			case MimeHeaderReader.ReadState.ReadValue:
				goto IL_017F;
			case MimeHeaderReader.ReadState.ReadLF:
				goto IL_01F4;
			case MimeHeaderReader.ReadState.ReadWS:
				goto IL_0226;
			case MimeHeaderReader.ReadState.EOF:
				goto IL_025F;
			default:
				goto IL_0276;
			}
			IL_016E:
			while (ptr4 < ptr3)
			{
				if (*ptr4 != 9 && *ptr4 != 32)
				{
					goto IL_017F;
				}
				ptr4++;
			}
			this.readState = MimeHeaderReader.ReadState.SkipWS;
			goto IL_0276;
			IL_017F:
			ptr2 = ptr4;
			while (ptr4 < ptr3)
			{
				if (*ptr4 == 13)
				{
					this.AppendValue(new string((sbyte*)ptr2, 0, (int)((long)(ptr4 - ptr2))), maxBuffer, ref remaining);
					ptr4++;
					goto IL_01F4;
				}
				if (*ptr4 == 10)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new FormatException(global::System.Runtime.Serialization.SR.GetString("Malformed MIME header.")));
				}
				ptr4++;
			}
			this.AppendValue(new string((sbyte*)ptr2, 0, (int)((long)(ptr4 - ptr2))), maxBuffer, ref remaining);
			this.readState = MimeHeaderReader.ReadState.ReadValue;
			goto IL_0276;
			IL_01F4:
			if (ptr4 >= ptr3)
			{
				this.readState = MimeHeaderReader.ReadState.ReadLF;
				goto IL_0276;
			}
			if (*ptr4 != 10)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new FormatException(global::System.Runtime.Serialization.SR.GetString("Malformed MIME header.")));
			}
			ptr4++;
			IL_0226:
			if (ptr4 >= ptr3)
			{
				this.readState = MimeHeaderReader.ReadState.ReadWS;
				goto IL_0276;
			}
			if (*ptr4 != 32 && *ptr4 != 9)
			{
				this.readState = MimeHeaderReader.ReadState.ReadName;
				this.offset = (int)((long)(ptr4 - ptr));
				return true;
			}
			goto IL_017F;
			IL_025F:
			this.readState = MimeHeaderReader.ReadState.EOF;
			this.offset = (int)((long)(ptr4 - ptr));
			return true;
			IL_0276:
			this.offset = (int)((long)(ptr4 - ptr));
			array = null;
			return false;
		}

		private bool BufferEnd()
		{
			if (this.maxOffset != 0)
			{
				return false;
			}
			if (this.readState != MimeHeaderReader.ReadState.ReadWS && this.readState != MimeHeaderReader.ReadState.ReadValue)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new FormatException(global::System.Runtime.Serialization.SR.GetString("Malformed MIME header.")));
			}
			this.readState = MimeHeaderReader.ReadState.EOF;
			return true;
		}

		public void Reset(Stream stream)
		{
			if (stream == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("stream");
			}
			if (this.readState != MimeHeaderReader.ReadState.EOF)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("On MimeReader, Reset method is called before EOF.")));
			}
			this.stream = stream;
			this.readState = MimeHeaderReader.ReadState.ReadName;
			this.maxOffset = 0;
			this.offset = 0;
		}

		private void AppendValue(string value, int maxBuffer, ref int remaining)
		{
			XmlMtomReader.DecrementBufferQuota(maxBuffer, ref remaining, value.Length * 2);
			if (this.value == null)
			{
				this.value = value;
				return;
			}
			this.value += value;
		}

		private void AppendName(string value, int maxBuffer, ref int remaining)
		{
			XmlMtomReader.DecrementBufferQuota(maxBuffer, ref remaining, value.Length * 2);
			if (this.name == null)
			{
				this.name = value;
				return;
			}
			this.name += value;
		}

		private string value;

		private byte[] buffer = new byte[1024];

		private int maxOffset;

		private string name;

		private int offset;

		private MimeHeaderReader.ReadState readState;

		private Stream stream;

		private enum ReadState
		{
			ReadName,
			SkipWS,
			ReadValue,
			ReadLF,
			ReadWS,
			EOF
		}
	}
}
