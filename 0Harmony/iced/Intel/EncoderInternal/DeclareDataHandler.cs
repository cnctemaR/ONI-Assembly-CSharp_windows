using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.EncoderInternal
{
	internal sealed class DeclareDataHandler : OpCodeHandler
	{
		public DeclareDataHandler(Code code)
			: base(EncFlags2.None, EncFlags3.Bit16or32 | EncFlags3.Bit64, true, null, Array2.Empty<Op>())
		{
			int num;
			switch (code)
			{
			case Code.DeclareByte:
				num = 1;
				break;
			case Code.DeclareWord:
				num = 2;
				break;
			case Code.DeclareDword:
				num = 4;
				break;
			case Code.DeclareQword:
				num = 8;
				break;
			default:
				throw new InvalidOperationException();
			}
			this.elemLength = num;
			this.maxLength = 16 / this.elemLength;
		}

		[NullableContext(1)]
		public override void Encode(Encoder encoder, in Instruction instruction)
		{
			int declareDataCount = instruction.DeclareDataCount;
			if (declareDataCount < 1 || declareDataCount > this.maxLength)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(54, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Invalid db/dw/dd/dq data count. Count = ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(declareDataCount);
				defaultInterpolatedStringHandler.AppendLiteral(", max count = ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(this.maxLength);
				encoder.ErrorMessage = defaultInterpolatedStringHandler.ToStringAndClear();
				return;
			}
			int num = declareDataCount * this.elemLength;
			for (int i = 0; i < num; i++)
			{
				encoder.WriteByteInternal((uint)instruction.GetDeclareByteValue(i));
			}
		}

		private readonly int elemLength;

		private readonly int maxLength;
	}
}
