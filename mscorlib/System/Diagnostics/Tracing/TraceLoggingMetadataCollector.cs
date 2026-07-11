using System;
using System.Collections.Generic;

namespace System.Diagnostics.Tracing
{
	internal class TraceLoggingMetadataCollector
	{
		internal TraceLoggingMetadataCollector()
		{
			this.impl = new TraceLoggingMetadataCollector.Impl();
		}

		private TraceLoggingMetadataCollector(TraceLoggingMetadataCollector other, FieldMetadata group)
		{
			this.impl = other.impl;
			this.currentGroup = group;
		}

		internal EventFieldTags Tags { get; set; }

		internal int ScratchSize
		{
			get
			{
				return (int)this.impl.scratchSize;
			}
		}

		internal int DataCount
		{
			get
			{
				return (int)this.impl.dataCount;
			}
		}

		internal int PinCount
		{
			get
			{
				return (int)this.impl.pinCount;
			}
		}

		private bool BeginningBufferedArray
		{
			get
			{
				return this.bufferedArrayFieldCount == 0;
			}
		}

		public TraceLoggingMetadataCollector AddGroup(string name)
		{
			TraceLoggingMetadataCollector traceLoggingMetadataCollector = this;
			if (name != null || this.BeginningBufferedArray)
			{
				FieldMetadata fieldMetadata = new FieldMetadata(name, TraceLoggingDataType.Struct, this.Tags, this.BeginningBufferedArray);
				this.AddField(fieldMetadata);
				traceLoggingMetadataCollector = new TraceLoggingMetadataCollector(this, fieldMetadata);
			}
			return traceLoggingMetadataCollector;
		}

		public void AddScalar(string name, TraceLoggingDataType type)
		{
			TraceLoggingDataType traceLoggingDataType = type & (TraceLoggingDataType)31;
			int num;
			switch (traceLoggingDataType)
			{
			case TraceLoggingDataType.Int8:
			case TraceLoggingDataType.UInt8:
				break;
			case TraceLoggingDataType.Int16:
			case TraceLoggingDataType.UInt16:
				goto IL_006F;
			case TraceLoggingDataType.Int32:
			case TraceLoggingDataType.UInt32:
			case TraceLoggingDataType.Float:
			case TraceLoggingDataType.Boolean32:
			case TraceLoggingDataType.HexInt32:
				num = 4;
				goto IL_008B;
			case TraceLoggingDataType.Int64:
			case TraceLoggingDataType.UInt64:
			case TraceLoggingDataType.Double:
			case TraceLoggingDataType.FileTime:
			case TraceLoggingDataType.HexInt64:
				num = 8;
				goto IL_008B;
			case TraceLoggingDataType.Binary:
			case (TraceLoggingDataType)16:
			case (TraceLoggingDataType)19:
				goto IL_0080;
			case TraceLoggingDataType.Guid:
			case TraceLoggingDataType.SystemTime:
				num = 16;
				goto IL_008B;
			default:
				if (traceLoggingDataType != TraceLoggingDataType.Char8)
				{
					if (traceLoggingDataType != TraceLoggingDataType.Char16)
					{
						goto IL_0080;
					}
					goto IL_006F;
				}
				break;
			}
			num = 1;
			goto IL_008B;
			IL_006F:
			num = 2;
			goto IL_008B;
			IL_0080:
			throw new ArgumentOutOfRangeException("type");
			IL_008B:
			this.impl.AddScalar(num);
			this.AddField(new FieldMetadata(name, type, this.Tags, this.BeginningBufferedArray));
		}

		public void AddBinary(string name, TraceLoggingDataType type)
		{
			TraceLoggingDataType traceLoggingDataType = type & (TraceLoggingDataType)31;
			if (traceLoggingDataType != TraceLoggingDataType.Binary && traceLoggingDataType - TraceLoggingDataType.CountedUtf16String > 1)
			{
				throw new ArgumentOutOfRangeException("type");
			}
			this.impl.AddScalar(2);
			this.impl.AddNonscalar();
			this.AddField(new FieldMetadata(name, type, this.Tags, this.BeginningBufferedArray));
		}

		public void AddArray(string name, TraceLoggingDataType type)
		{
			TraceLoggingDataType traceLoggingDataType = type & (TraceLoggingDataType)31;
			switch (traceLoggingDataType)
			{
			case TraceLoggingDataType.Utf16String:
			case TraceLoggingDataType.MbcsString:
			case TraceLoggingDataType.Int8:
			case TraceLoggingDataType.UInt8:
			case TraceLoggingDataType.Int16:
			case TraceLoggingDataType.UInt16:
			case TraceLoggingDataType.Int32:
			case TraceLoggingDataType.UInt32:
			case TraceLoggingDataType.Int64:
			case TraceLoggingDataType.UInt64:
			case TraceLoggingDataType.Float:
			case TraceLoggingDataType.Double:
			case TraceLoggingDataType.Boolean32:
			case TraceLoggingDataType.Guid:
			case TraceLoggingDataType.FileTime:
			case TraceLoggingDataType.HexInt32:
			case TraceLoggingDataType.HexInt64:
				goto IL_007C;
			case TraceLoggingDataType.Binary:
			case (TraceLoggingDataType)16:
			case TraceLoggingDataType.SystemTime:
			case (TraceLoggingDataType)19:
				break;
			default:
				if (traceLoggingDataType == TraceLoggingDataType.Char8 || traceLoggingDataType == TraceLoggingDataType.Char16)
				{
					goto IL_007C;
				}
				break;
			}
			throw new ArgumentOutOfRangeException("type");
			IL_007C:
			if (this.BeginningBufferedArray)
			{
				throw new NotSupportedException(Environment.GetResourceString("Nested arrays/enumerables are not supported."));
			}
			this.impl.AddScalar(2);
			this.impl.AddNonscalar();
			this.AddField(new FieldMetadata(name, type, this.Tags, true));
		}

		public void BeginBufferedArray()
		{
			if (this.bufferedArrayFieldCount >= 0)
			{
				throw new NotSupportedException(Environment.GetResourceString("Nested arrays/enumerables are not supported."));
			}
			this.bufferedArrayFieldCount = 0;
			this.impl.BeginBuffered();
		}

		public void EndBufferedArray()
		{
			if (this.bufferedArrayFieldCount != 1)
			{
				throw new InvalidOperationException(Environment.GetResourceString("Incorrectly-authored TypeInfo - a type should be serialized as one field or as one group"));
			}
			this.bufferedArrayFieldCount = int.MinValue;
			this.impl.EndBuffered();
		}

		public void AddCustom(string name, TraceLoggingDataType type, byte[] metadata)
		{
			if (this.BeginningBufferedArray)
			{
				throw new NotSupportedException(Environment.GetResourceString("Enumerables of custom-serialized data are not supported"));
			}
			this.impl.AddScalar(2);
			this.impl.AddNonscalar();
			this.AddField(new FieldMetadata(name, type, this.Tags, metadata));
		}

		internal byte[] GetMetadata()
		{
			byte[] array = new byte[this.impl.Encode(null)];
			this.impl.Encode(array);
			return array;
		}

		private void AddField(FieldMetadata fieldMetadata)
		{
			this.Tags = EventFieldTags.None;
			this.bufferedArrayFieldCount++;
			this.impl.fields.Add(fieldMetadata);
			if (this.currentGroup != null)
			{
				this.currentGroup.IncrementStructFieldCount();
			}
		}

		private readonly TraceLoggingMetadataCollector.Impl impl;

		private readonly FieldMetadata currentGroup;

		private int bufferedArrayFieldCount = int.MinValue;

		private class Impl
		{
			public void AddScalar(int size)
			{
				checked
				{
					if (this.bufferNesting == 0)
					{
						if (!this.scalar)
						{
							this.dataCount += 1;
						}
						this.scalar = true;
						this.scratchSize = (short)((int)this.scratchSize + size);
					}
				}
			}

			public void AddNonscalar()
			{
				checked
				{
					if (this.bufferNesting == 0)
					{
						this.scalar = false;
						this.pinCount += 1;
						this.dataCount += 1;
					}
				}
			}

			public void BeginBuffered()
			{
				if (this.bufferNesting == 0)
				{
					this.AddNonscalar();
				}
				this.bufferNesting++;
			}

			public void EndBuffered()
			{
				this.bufferNesting--;
			}

			public int Encode(byte[] metadata)
			{
				int num = 0;
				foreach (FieldMetadata fieldMetadata in this.fields)
				{
					fieldMetadata.Encode(ref num, metadata);
				}
				return num;
			}

			internal readonly List<FieldMetadata> fields = new List<FieldMetadata>();

			internal short scratchSize;

			internal sbyte dataCount;

			internal sbyte pinCount;

			private int bufferNesting;

			private bool scalar;
		}
	}
}
