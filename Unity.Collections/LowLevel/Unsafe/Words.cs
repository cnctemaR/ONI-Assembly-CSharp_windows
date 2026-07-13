using System;

namespace Unity.Collections.LowLevel.Unsafe
{
	[Obsolete("This storage will no longer be used. (RemovedAfter 2021-06-01)")]
	public struct Words
	{
		public void ToFixedString<T>(ref T value) where T : IUTF8Bytes, INativeList<byte>
		{
			WordStorage.Instance.GetFixedString<T>(this.Index, ref value);
		}

		public override string ToString()
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			this.ToFixedString<FixedString512Bytes>(ref fixedString512Bytes);
			return fixedString512Bytes.ToString();
		}

		public void SetFixedString<T>(ref T value) where T : IUTF8Bytes, INativeList<byte>
		{
			this.Index = WordStorage.Instance.GetOrCreateIndex<T>(ref value);
		}

		public void SetString(string value)
		{
			FixedString512Bytes fixedString512Bytes = value;
			this.SetFixedString<FixedString512Bytes>(ref fixedString512Bytes);
		}

		private int Index;
	}
}
