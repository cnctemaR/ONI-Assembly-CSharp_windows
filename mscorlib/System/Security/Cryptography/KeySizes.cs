using System;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public sealed class KeySizes
	{
		public KeySizes(int minSize, int maxSize, int skipSize)
		{
			this._maxSize = maxSize;
			this._minSize = minSize;
			this._skipSize = skipSize;
		}

		public int MaxSize
		{
			get
			{
				return this._maxSize;
			}
		}

		public int MinSize
		{
			get
			{
				return this._minSize;
			}
		}

		public int SkipSize
		{
			get
			{
				return this._skipSize;
			}
		}

		internal bool IsLegal(int keySize)
		{
			int num = keySize - this.MinSize;
			bool flag = num >= 0 && keySize <= this.MaxSize;
			return (this.SkipSize != 0) ? (flag && num % this.SkipSize == 0) : flag;
		}

		internal static bool IsLegalKeySize(KeySizes[] legalKeys, int size)
		{
			foreach (KeySizes keySizes in legalKeys)
			{
				if (keySizes.IsLegal(size))
				{
					return true;
				}
			}
			return false;
		}

		private int _maxSize;

		private int _minSize;

		private int _skipSize;
	}
}
