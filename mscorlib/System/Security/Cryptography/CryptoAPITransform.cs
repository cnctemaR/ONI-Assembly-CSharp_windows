using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public sealed class CryptoAPITransform : ICryptoTransform, IDisposable
	{
		internal CryptoAPITransform()
		{
			this.m_disposed = false;
		}

		public bool CanReuseTransform
		{
			get
			{
				return true;
			}
		}

		public bool CanTransformMultipleBlocks
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
				return 0;
			}
		}

		public IntPtr KeyHandle
		{
			[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
			get
			{
				return IntPtr.Zero;
			}
		}

		public int OutputBlockSize
		{
			get
			{
				return 0;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Clear()
		{
			this.Dispose(false);
		}

		private void Dispose(bool disposing)
		{
			if (!this.m_disposed)
			{
				this.m_disposed = true;
			}
		}

		public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
		{
			return 0;
		}

		public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
		{
			return null;
		}

		[ComVisible(false)]
		public void Reset()
		{
		}

		private bool m_disposed;
	}
}
