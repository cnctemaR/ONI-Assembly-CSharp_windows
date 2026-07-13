using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Iced.Intel.Internal;

namespace Iced.Intel.DecoderInternal
{
	[NullableContext(1)]
	[Nullable(0)]
	internal ref struct TableDeserializer
	{
		[NullableContext(0)]
		public TableDeserializer([Nullable(1)] OpCodeHandlerReader handlerReader, int maxIds, global::System.ReadOnlySpan<byte> data)
		{
			this.handlerReader = handlerReader;
			this.reader = new DataReader(data);
			this.idToHandler = new List<HandlerInfo>(maxIds);
			this.handlerArray = new OpCodeHandler[1];
		}

		public void Deserialize()
		{
			while (this.reader.CanRead)
			{
				SerializedDataKind serializedDataKind = (SerializedDataKind)this.reader.ReadByte();
				if (serializedDataKind != SerializedDataKind.HandlerReference)
				{
					if (serializedDataKind != SerializedDataKind.ArrayReference)
					{
						throw new InvalidOperationException();
					}
					this.idToHandler.Add(new HandlerInfo(this.ReadHandlers((int)this.reader.ReadCompressedUInt32())));
				}
				else
				{
					this.idToHandler.Add(new HandlerInfo(this.ReadHandler()));
				}
			}
			if (this.reader.CanRead)
			{
				throw new InvalidOperationException();
			}
		}

		public LegacyOpCodeHandlerKind ReadLegacyOpCodeHandlerKind()
		{
			return (LegacyOpCodeHandlerKind)this.reader.ReadByte();
		}

		public VexOpCodeHandlerKind ReadVexOpCodeHandlerKind()
		{
			return (VexOpCodeHandlerKind)this.reader.ReadByte();
		}

		public EvexOpCodeHandlerKind ReadEvexOpCodeHandlerKind()
		{
			return (EvexOpCodeHandlerKind)this.reader.ReadByte();
		}

		public Code ReadCode()
		{
			return (Code)this.reader.ReadCompressedUInt32();
		}

		public Register ReadRegister()
		{
			return (Register)this.reader.ReadByte();
		}

		public DecoderOptions ReadDecoderOptions()
		{
			return (DecoderOptions)this.reader.ReadCompressedUInt32();
		}

		public HandlerFlags ReadHandlerFlags()
		{
			return (HandlerFlags)this.reader.ReadCompressedUInt32();
		}

		public LegacyHandlerFlags ReadLegacyHandlerFlags()
		{
			return (LegacyHandlerFlags)this.reader.ReadCompressedUInt32();
		}

		public TupleType ReadTupleType()
		{
			return (TupleType)this.reader.ReadByte();
		}

		public bool ReadBoolean()
		{
			return this.reader.ReadByte() > 0;
		}

		public int ReadInt32()
		{
			return (int)this.reader.ReadCompressedUInt32();
		}

		public OpCodeHandler ReadHandler()
		{
			OpCodeHandler opCodeHandler = this.ReadHandlerOrNull();
			if (opCodeHandler == null)
			{
				throw new InvalidOperationException();
			}
			return opCodeHandler;
		}

		[NullableContext(2)]
		public OpCodeHandler ReadHandlerOrNull()
		{
			if (this.handlerReader.ReadHandlers(ref this, this.handlerArray, 0) != 1)
			{
				throw new InvalidOperationException();
			}
			return this.handlerArray[0];
		}

		[return: Nullable(new byte[] { 1, 2 })]
		public OpCodeHandler[] ReadHandlers(int count)
		{
			OpCodeHandler[] array = new OpCodeHandler[count];
			int num;
			for (int i = 0; i < array.Length; i += num)
			{
				num = this.handlerReader.ReadHandlers(ref this, array, i);
				if (num <= 0 || i + num > array.Length)
				{
					throw new InvalidOperationException();
				}
			}
			return array;
		}

		public OpCodeHandler ReadHandlerReference()
		{
			uint num = (uint)this.reader.ReadByte();
			OpCodeHandler handler = this.idToHandler[(int)num].handler;
			if (handler == null)
			{
				throw new InvalidOperationException();
			}
			return handler;
		}

		public OpCodeHandler[] ReadArrayReference(uint kind)
		{
			if ((uint)this.reader.ReadByte() != kind)
			{
				throw new InvalidOperationException();
			}
			return this.GetTable((uint)this.reader.ReadByte());
		}

		public OpCodeHandler[] GetTable(uint index)
		{
			OpCodeHandler[] handlers = this.idToHandler[(int)index].handlers;
			if (handlers == null)
			{
				throw new InvalidOperationException();
			}
			return handlers;
		}

		private DataReader reader;

		private readonly OpCodeHandlerReader handlerReader;

		private readonly List<HandlerInfo> idToHandler;

		private readonly OpCodeHandler[] handlerArray;
	}
}
