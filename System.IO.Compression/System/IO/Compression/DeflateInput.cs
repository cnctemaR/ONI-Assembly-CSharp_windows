using System;

namespace System.IO.Compression
{
	internal sealed class DeflateInput
	{
		internal byte[] Buffer { get; set; }

		internal int Count { get; set; }

		internal int StartIndex { get; set; }

		internal void ConsumeBytes(int n)
		{
			this.StartIndex += n;
			this.Count -= n;
		}

		internal DeflateInput.InputState DumpState()
		{
			return new DeflateInput.InputState(this.Count, this.StartIndex);
		}

		internal void RestoreState(DeflateInput.InputState state)
		{
			this.Count = state._count;
			this.StartIndex = state._startIndex;
		}

		internal readonly struct InputState
		{
			internal InputState(int count, int startIndex)
			{
				this._count = count;
				this._startIndex = startIndex;
			}

			internal readonly int _count;

			internal readonly int _startIndex;
		}
	}
}
