using System;
using System.Diagnostics;

namespace Unity.Burst.Intrinsics
{
	internal class V64DebugView
	{
		public V64DebugView(v64 value)
		{
			this.m_Value = value;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
		public byte[] Byte
		{
			get
			{
				return new byte[]
				{
					this.m_Value.Byte0,
					this.m_Value.Byte1,
					this.m_Value.Byte2,
					this.m_Value.Byte3,
					this.m_Value.Byte4,
					this.m_Value.Byte5,
					this.m_Value.Byte6,
					this.m_Value.Byte7
				};
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
		public sbyte[] SByte
		{
			get
			{
				return new sbyte[]
				{
					this.m_Value.SByte0,
					this.m_Value.SByte1,
					this.m_Value.SByte2,
					this.m_Value.SByte3,
					this.m_Value.SByte4,
					this.m_Value.SByte5,
					this.m_Value.SByte6,
					this.m_Value.SByte7
				};
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
		public ushort[] UShort
		{
			get
			{
				return new ushort[]
				{
					this.m_Value.UShort0,
					this.m_Value.UShort1,
					this.m_Value.UShort2,
					this.m_Value.UShort3
				};
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
		public short[] SShort
		{
			get
			{
				return new short[]
				{
					this.m_Value.SShort0,
					this.m_Value.SShort1,
					this.m_Value.SShort2,
					this.m_Value.SShort3
				};
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
		public uint[] UInt
		{
			get
			{
				return new uint[]
				{
					this.m_Value.UInt0,
					this.m_Value.UInt1
				};
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
		public int[] SInt
		{
			get
			{
				return new int[]
				{
					this.m_Value.SInt0,
					this.m_Value.SInt1
				};
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
		public float[] Float
		{
			get
			{
				return new float[]
				{
					this.m_Value.Float0,
					this.m_Value.Float1
				};
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
		public long[] SLong
		{
			get
			{
				return new long[] { this.m_Value.SLong0 };
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
		public ulong[] ULong
		{
			get
			{
				return new ulong[] { this.m_Value.ULong0 };
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
		public double[] Double
		{
			get
			{
				return new double[] { this.m_Value.Double0 };
			}
		}

		private v64 m_Value;
	}
}
