using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.EncoderInternal
{
	[NullableContext(1)]
	[Nullable(0)]
	internal abstract class OpCodeHandler
	{
		protected OpCodeHandler(EncFlags2 encFlags2, EncFlags3 encFlags3, bool isSpecialInstr, [Nullable(2)] TryConvertToDisp8N tryConvertToDisp8N, Op[] operands)
		{
			this.EncFlags3 = encFlags3;
			this.OpCode = OpCodeHandler.GetOpCode(encFlags2);
			this.Is2ByteOpCode = (encFlags2 & EncFlags2.OpCodeIs2Bytes) > EncFlags2.None;
			this.GroupIndex = (int)(((encFlags2 & (EncFlags2)2147483648U) == EncFlags2.None) ? ((EncFlags2)4294967295U) : ((encFlags2 >> 27) & EncFlags2.TableMask));
			this.RmGroupIndex = (int)(((encFlags3 & EncFlags3.HasRmGroupIndex) == EncFlags3.None) ? ((EncFlags2)4294967295U) : ((encFlags2 >> 27) & EncFlags2.TableMask));
			this.IsSpecialInstr = isSpecialInstr;
			this.OpSize = (CodeSize)((encFlags3 >> 3) & EncFlags3.OperandSizeShift);
			this.AddrSize = (CodeSize)((encFlags3 >> 5) & EncFlags3.OperandSizeShift);
			this.TryConvertToDisp8N = tryConvertToDisp8N;
			this.Operands = operands;
		}

		protected static uint GetOpCode(EncFlags2 encFlags2)
		{
			return (uint)((ushort)encFlags2);
		}

		public abstract void Encode(Encoder encoder, in Instruction instruction);

		internal readonly uint OpCode;

		internal readonly bool Is2ByteOpCode;

		internal readonly int GroupIndex;

		internal readonly int RmGroupIndex;

		internal readonly bool IsSpecialInstr;

		internal readonly EncFlags3 EncFlags3;

		internal readonly CodeSize OpSize;

		internal readonly CodeSize AddrSize;

		[Nullable(2)]
		internal readonly TryConvertToDisp8N TryConvertToDisp8N;

		internal readonly Op[] Operands;
	}
}
