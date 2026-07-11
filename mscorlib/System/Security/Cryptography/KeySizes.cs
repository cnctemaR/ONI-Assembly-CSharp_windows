using System;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public sealed class KeySizes
	{
		public int MinSize
		{
			get
			{
				return this.m_minSize;
			}
		}

		public int MaxSize
		{
			get
			{
				return this.m_maxSize;
			}
		}

		public int SkipSize
		{
			get
			{
				return this.m_skipSize;
			}
		}

		public KeySizes(int minSize, int maxSize, int skipSize)
		{
			this.m_minSize = minSize;
			this.m_maxSize = maxSize;
			this.m_skipSize = skipSize;
		}

		internal bool IsLegal(int keySize)
		{
			int num = keySize - this.MinSize;
			bool flag = num >= 0 && keySize <= this.MaxSize;
			if (this.SkipSize != 0)
			{
				return flag && num % this.SkipSize == 0;
			}
			return flag;
		}

		internal static bool IsLegalKeySize(KeySizes[] legalKeys, int size)
		{
			for (int i = 0; i < legalKeys.Length; i++)
			{
				if (legalKeys[i].IsLegal(size))
				{
					return true;
				}
			}
			return false;
		}

		private int m_minSize;

		private int m_maxSize;

		private int m_skipSize;
	}
}
