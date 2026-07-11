using System;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public sealed class RijndaelManagedTransform : IDisposable, ICryptoTransform
	{
		internal RijndaelManagedTransform(Rijndael algo, bool encryption, byte[] key, byte[] iv)
		{
			this._st = new RijndaelTransform(algo, encryption, key, iv);
			this._bs = algo.BlockSize;
		}

		void IDisposable.Dispose()
		{
			this._st.Clear();
		}

		public int BlockSizeValue
		{
			get
			{
				return this._bs;
			}
		}

		public bool CanTransformMultipleBlocks
		{
			get
			{
				return this._st.CanTransformMultipleBlocks;
			}
		}

		public bool CanReuseTransform
		{
			get
			{
				return this._st.CanReuseTransform;
			}
		}

		public int InputBlockSize
		{
			get
			{
				return this._st.InputBlockSize;
			}
		}

		public int OutputBlockSize
		{
			get
			{
				return this._st.OutputBlockSize;
			}
		}

		public void Clear()
		{
			this._st.Clear();
		}

		[MonoTODO("Reset does nothing since CanReuseTransform return false.")]
		public void Reset()
		{
		}

		public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
		{
			return this._st.TransformBlock(inputBuffer, inputOffset, inputCount, outputBuffer, outputOffset);
		}

		public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
		{
			return this._st.TransformFinalBlock(inputBuffer, inputOffset, inputCount);
		}

		private RijndaelTransform _st;

		private int _bs;
	}
}
