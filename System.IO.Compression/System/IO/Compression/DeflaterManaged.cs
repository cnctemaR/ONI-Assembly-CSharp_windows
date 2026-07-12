using System;

namespace System.IO.Compression
{
	internal sealed class DeflaterManaged : IDisposable
	{
		internal DeflaterManaged()
		{
			this._deflateEncoder = new FastEncoder();
			this._copyEncoder = new CopyEncoder();
			this._input = new DeflateInput();
			this._output = new OutputBuffer();
			this._processingState = DeflaterManaged.DeflaterState.NotStarted;
		}

		internal bool NeedsInput()
		{
			return this._input.Count == 0 && this._deflateEncoder.BytesInHistory == 0;
		}

		internal void SetInput(byte[] inputBuffer, int startIndex, int count)
		{
			this._input.Buffer = inputBuffer;
			this._input.Count = count;
			this._input.StartIndex = startIndex;
			if (count > 0 && count < 256)
			{
				DeflaterManaged.DeflaterState processingState = this._processingState;
				if (processingState != DeflaterManaged.DeflaterState.NotStarted)
				{
					if (processingState == DeflaterManaged.DeflaterState.CompressThenCheck)
					{
						this._processingState = DeflaterManaged.DeflaterState.HandlingSmallData;
						return;
					}
					if (processingState != DeflaterManaged.DeflaterState.CheckingForIncompressible)
					{
						return;
					}
				}
				this._processingState = DeflaterManaged.DeflaterState.StartingSmallData;
				return;
			}
		}

		internal int GetDeflateOutput(byte[] outputBuffer)
		{
			this._output.UpdateBuffer(outputBuffer);
			switch (this._processingState)
			{
			case DeflaterManaged.DeflaterState.NotStarted:
			{
				DeflateInput.InputState inputState = this._input.DumpState();
				OutputBuffer.BufferState bufferState = this._output.DumpState();
				this._deflateEncoder.GetBlockHeader(this._output);
				this._deflateEncoder.GetCompressedData(this._input, this._output);
				if (!this.UseCompressed(this._deflateEncoder.LastCompressionRatio))
				{
					this._input.RestoreState(inputState);
					this._output.RestoreState(bufferState);
					this._copyEncoder.GetBlock(this._input, this._output, false);
					this.FlushInputWindows();
					this._processingState = DeflaterManaged.DeflaterState.CheckingForIncompressible;
					goto IL_023A;
				}
				this._processingState = DeflaterManaged.DeflaterState.CompressThenCheck;
				goto IL_023A;
			}
			case DeflaterManaged.DeflaterState.SlowDownForIncompressible1:
				this._deflateEncoder.GetBlockFooter(this._output);
				this._processingState = DeflaterManaged.DeflaterState.SlowDownForIncompressible2;
				break;
			case DeflaterManaged.DeflaterState.SlowDownForIncompressible2:
				break;
			case DeflaterManaged.DeflaterState.StartingSmallData:
				this._deflateEncoder.GetBlockHeader(this._output);
				this._processingState = DeflaterManaged.DeflaterState.HandlingSmallData;
				goto IL_0223;
			case DeflaterManaged.DeflaterState.CompressThenCheck:
				this._deflateEncoder.GetCompressedData(this._input, this._output);
				if (!this.UseCompressed(this._deflateEncoder.LastCompressionRatio))
				{
					this._processingState = DeflaterManaged.DeflaterState.SlowDownForIncompressible1;
					this._inputFromHistory = this._deflateEncoder.UnprocessedInput;
					goto IL_023A;
				}
				goto IL_023A;
			case DeflaterManaged.DeflaterState.CheckingForIncompressible:
			{
				DeflateInput.InputState inputState2 = this._input.DumpState();
				OutputBuffer.BufferState bufferState2 = this._output.DumpState();
				this._deflateEncoder.GetBlock(this._input, this._output, 8072);
				if (!this.UseCompressed(this._deflateEncoder.LastCompressionRatio))
				{
					this._input.RestoreState(inputState2);
					this._output.RestoreState(bufferState2);
					this._copyEncoder.GetBlock(this._input, this._output, false);
					this.FlushInputWindows();
					goto IL_023A;
				}
				goto IL_023A;
			}
			case DeflaterManaged.DeflaterState.HandlingSmallData:
				goto IL_0223;
			default:
				goto IL_023A;
			}
			if (this._inputFromHistory.Count > 0)
			{
				this._copyEncoder.GetBlock(this._inputFromHistory, this._output, false);
			}
			if (this._inputFromHistory.Count == 0)
			{
				this._deflateEncoder.FlushInput();
				this._processingState = DeflaterManaged.DeflaterState.CheckingForIncompressible;
				goto IL_023A;
			}
			goto IL_023A;
			IL_0223:
			this._deflateEncoder.GetCompressedData(this._input, this._output);
			IL_023A:
			return this._output.BytesWritten;
		}

		internal bool Finish(byte[] outputBuffer, out int bytesRead)
		{
			if (this._processingState == DeflaterManaged.DeflaterState.NotStarted)
			{
				bytesRead = 0;
				return true;
			}
			this._output.UpdateBuffer(outputBuffer);
			if (this._processingState == DeflaterManaged.DeflaterState.CompressThenCheck || this._processingState == DeflaterManaged.DeflaterState.HandlingSmallData || this._processingState == DeflaterManaged.DeflaterState.SlowDownForIncompressible1)
			{
				this._deflateEncoder.GetBlockFooter(this._output);
			}
			this.WriteFinal();
			bytesRead = this._output.BytesWritten;
			return true;
		}

		private bool UseCompressed(double ratio)
		{
			return ratio <= 1.0;
		}

		private void FlushInputWindows()
		{
			this._deflateEncoder.FlushInput();
		}

		private void WriteFinal()
		{
			this._copyEncoder.GetBlock(null, this._output, true);
		}

		public void Dispose()
		{
		}

		private const int MinBlockSize = 256;

		private const int MaxHeaderFooterGoo = 120;

		private const int CleanCopySize = 8072;

		private const double BadCompressionThreshold = 1.0;

		private readonly FastEncoder _deflateEncoder;

		private readonly CopyEncoder _copyEncoder;

		private readonly DeflateInput _input;

		private readonly OutputBuffer _output;

		private DeflaterManaged.DeflaterState _processingState;

		private DeflateInput _inputFromHistory;

		private enum DeflaterState
		{
			NotStarted,
			SlowDownForIncompressible1,
			SlowDownForIncompressible2,
			StartingSmallData,
			CompressThenCheck,
			CheckingForIncompressible,
			HandlingSmallData
		}
	}
}
