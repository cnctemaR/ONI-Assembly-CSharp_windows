using System;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public class ToBase64Transform : IDisposable, ICryptoTransform
	{
		void IDisposable.Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		~ToBase64Transform()
		{
			this.Dispose(false);
		}

		public bool CanTransformMultipleBlocks
		{
			get
			{
				return false;
			}
		}

		public virtual bool CanReuseTransform
		{
			get
			{
				return true;
			}
		}

		public int InputBlockSize
		{
			get
			{
				return 3;
			}
		}

		public int OutputBlockSize
		{
			get
			{
				return 4;
			}
		}

		public void Clear()
		{
			this.Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.m_disposed)
			{
				if (disposing)
				{
				}
				this.m_disposed = true;
			}
		}

		public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
		{
			if (this.m_disposed)
			{
				throw new ObjectDisposedException("TransformBlock");
			}
			if (inputBuffer == null)
			{
				throw new ArgumentNullException("inputBuffer");
			}
			if (outputBuffer == null)
			{
				throw new ArgumentNullException("outputBuffer");
			}
			if (inputCount < 0)
			{
				throw new ArgumentException("inputCount", "< 0");
			}
			if (inputCount > inputBuffer.Length)
			{
				throw new ArgumentException("inputCount", Locale.GetText("Overflow"));
			}
			if (inputOffset < 0)
			{
				throw new ArgumentOutOfRangeException("inputOffset", "< 0");
			}
			if (inputOffset > inputBuffer.Length - inputCount)
			{
				throw new ArgumentException("inputOffset", Locale.GetText("Overflow"));
			}
			if (outputOffset < 0)
			{
				throw new ArgumentOutOfRangeException("outputOffset", "< 0");
			}
			if (outputOffset > outputBuffer.Length - inputCount)
			{
				throw new ArgumentException("outputOffset", Locale.GetText("Overflow"));
			}
			ToBase64Transform.InternalTransformBlock(inputBuffer, inputOffset, inputCount, outputBuffer, outputOffset);
			return this.OutputBlockSize;
		}

		internal static void InternalTransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
		{
			byte[] encodeTable = Base64Constants.EncodeTable;
			int num = (int)inputBuffer[inputOffset];
			int num2 = (int)inputBuffer[inputOffset + 1];
			int num3 = (int)inputBuffer[inputOffset + 2];
			outputBuffer[outputOffset] = encodeTable[num >> 2];
			outputBuffer[outputOffset + 1] = encodeTable[((num << 4) & 48) | (num2 >> 4)];
			outputBuffer[outputOffset + 2] = encodeTable[((num2 << 2) & 60) | (num3 >> 6)];
			outputBuffer[outputOffset + 3] = encodeTable[num3 & 63];
		}

		public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
		{
			if (this.m_disposed)
			{
				throw new ObjectDisposedException("TransformFinalBlock");
			}
			if (inputBuffer == null)
			{
				throw new ArgumentNullException("inputBuffer");
			}
			if (inputCount < 0)
			{
				throw new ArgumentException("inputCount", "< 0");
			}
			if (inputOffset > inputBuffer.Length - inputCount)
			{
				throw new ArgumentException("inputCount", Locale.GetText("Overflow"));
			}
			if (inputCount > this.InputBlockSize)
			{
				throw new ArgumentOutOfRangeException(Locale.GetText("Invalid input length"));
			}
			return ToBase64Transform.InternalTransformFinalBlock(inputBuffer, inputOffset, inputCount);
		}

		internal static byte[] InternalTransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
		{
			int num = 3;
			int num2 = 4;
			int num3 = inputCount / num;
			int num4 = inputCount % num;
			byte[] array = new byte[(inputCount == 0) ? 0 : ((inputCount + 2) / num * num2)];
			int num5 = 0;
			for (int i = 0; i < num3; i++)
			{
				ToBase64Transform.InternalTransformBlock(inputBuffer, inputOffset, num, array, num5);
				inputOffset += num;
				num5 += num2;
			}
			byte[] encodeTable = Base64Constants.EncodeTable;
			switch (num4)
			{
			case 1:
			{
				int num6 = (int)inputBuffer[inputOffset];
				array[num5] = encodeTable[num6 >> 2];
				array[num5 + 1] = encodeTable[(num6 << 4) & 48];
				array[num5 + 2] = 61;
				array[num5 + 3] = 61;
				break;
			}
			case 2:
			{
				int num6 = (int)inputBuffer[inputOffset];
				int num7 = (int)inputBuffer[inputOffset + 1];
				array[num5] = encodeTable[num6 >> 2];
				array[num5 + 1] = encodeTable[((num6 << 4) & 48) | (num7 >> 4)];
				array[num5 + 2] = encodeTable[(num7 << 2) & 60];
				array[num5 + 3] = 61;
				break;
			}
			}
			return array;
		}

		private const int inputBlockSize = 3;

		private const int outputBlockSize = 4;

		private bool m_disposed;
	}
}
