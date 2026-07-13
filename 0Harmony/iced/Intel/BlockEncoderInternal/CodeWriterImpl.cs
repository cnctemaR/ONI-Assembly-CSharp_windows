using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.BlockEncoderInternal
{
	internal sealed class CodeWriterImpl : CodeWriter
	{
		[NullableContext(1)]
		public CodeWriterImpl(CodeWriter codeWriter)
		{
			if (codeWriter == null)
			{
				ThrowHelper.ThrowArgumentNullException_codeWriter();
			}
			this.codeWriter = codeWriter;
		}

		public override void WriteByte(byte value)
		{
			this.BytesWritten += 1U;
			this.codeWriter.WriteByte(value);
		}

		public uint BytesWritten;

		private readonly CodeWriter codeWriter;
	}
}
