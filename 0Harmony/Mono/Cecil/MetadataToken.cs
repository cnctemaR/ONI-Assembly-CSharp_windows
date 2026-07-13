using System;

namespace Mono.Cecil
{
	internal struct MetadataToken : IEquatable<MetadataToken>
	{
		public uint RID
		{
			get
			{
				return this.token & 16777215U;
			}
		}

		public TokenType TokenType
		{
			get
			{
				return (TokenType)(this.token & 4278190080U);
			}
		}

		public MetadataToken(uint token)
		{
			this.token = token;
		}

		public MetadataToken(TokenType type)
		{
			this = new MetadataToken(type, 0);
		}

		public MetadataToken(TokenType type, uint rid)
		{
			this.token = (uint)(type | (TokenType)rid);
		}

		public MetadataToken(TokenType type, int rid)
		{
			this.token = (uint)(type | (TokenType)rid);
		}

		public int ToInt32()
		{
			return (int)this.token;
		}

		public uint ToUInt32()
		{
			return this.token;
		}

		public override int GetHashCode()
		{
			return (int)this.token;
		}

		public bool Equals(MetadataToken other)
		{
			return other.token == this.token;
		}

		public override bool Equals(object obj)
		{
			return obj is MetadataToken && ((MetadataToken)obj).token == this.token;
		}

		public static bool operator ==(MetadataToken one, MetadataToken other)
		{
			return one.token == other.token;
		}

		public static bool operator !=(MetadataToken one, MetadataToken other)
		{
			return one.token != other.token;
		}

		public override string ToString()
		{
			return string.Format("[{0}:0x{1}]", this.TokenType, this.RID.ToString("x4"));
		}

		private readonly uint token;

		public static readonly MetadataToken Zero = new MetadataToken(0U);
	}
}
