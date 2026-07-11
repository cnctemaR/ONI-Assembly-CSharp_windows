using System;
using System.Runtime.InteropServices;

namespace System.Configuration.Assemblies
{
	[ComVisible(true)]
	[Obsolete]
	[Serializable]
	public struct AssemblyHash : ICloneable
	{
		[Obsolete]
		public AssemblyHashAlgorithm Algorithm
		{
			get
			{
				return this._algorithm;
			}
			set
			{
				this._algorithm = value;
			}
		}

		[Obsolete]
		public AssemblyHash(AssemblyHashAlgorithm algorithm, byte[] value)
		{
			this._algorithm = algorithm;
			if (value != null)
			{
				this._value = (byte[])value.Clone();
				return;
			}
			this._value = null;
		}

		[Obsolete]
		public AssemblyHash(byte[] value)
		{
			this = new AssemblyHash(AssemblyHashAlgorithm.SHA1, value);
		}

		[Obsolete]
		public object Clone()
		{
			return new AssemblyHash(this._algorithm, this._value);
		}

		[Obsolete]
		public byte[] GetValue()
		{
			return this._value;
		}

		[Obsolete]
		public void SetValue(byte[] value)
		{
			this._value = value;
		}

		private AssemblyHashAlgorithm _algorithm;

		private byte[] _value;

		[Obsolete]
		public static readonly AssemblyHash Empty = new AssemblyHash(AssemblyHashAlgorithm.None, null);
	}
}
