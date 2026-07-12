using System;

namespace FMOD
{
	[Serializable]
	public struct GUID : IEquatable<GUID>
	{
		public GUID(Guid guid)
		{
			byte[] array = guid.ToByteArray();
			this.Data1 = BitConverter.ToInt32(array, 0);
			this.Data2 = BitConverter.ToInt32(array, 4);
			this.Data3 = BitConverter.ToInt32(array, 8);
			this.Data4 = BitConverter.ToInt32(array, 12);
		}

		public static GUID Parse(string s)
		{
			return new GUID(new Guid(s));
		}

		public bool IsNull
		{
			get
			{
				return this.Data1 == 0 && this.Data2 == 0 && this.Data3 == 0 && this.Data4 == 0;
			}
		}

		public override bool Equals(object other)
		{
			return other is GUID && this.Equals((GUID)other);
		}

		public bool Equals(GUID other)
		{
			return this.Data1 == other.Data1 && this.Data2 == other.Data2 && this.Data3 == other.Data3 && this.Data4 == other.Data4;
		}

		public static bool operator ==(GUID a, GUID b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(GUID a, GUID b)
		{
			return !a.Equals(b);
		}

		public override int GetHashCode()
		{
			return this.Data1 ^ this.Data2 ^ this.Data3 ^ this.Data4;
		}

		public static implicit operator Guid(GUID guid)
		{
			return new Guid(guid.Data1, (short)(guid.Data2 & 65535), (short)((guid.Data2 >> 16) & 65535), (byte)(guid.Data3 & 255), (byte)((guid.Data3 >> 8) & 255), (byte)((guid.Data3 >> 16) & 255), (byte)((guid.Data3 >> 24) & 255), (byte)(guid.Data4 & 255), (byte)((guid.Data4 >> 8) & 255), (byte)((guid.Data4 >> 16) & 255), (byte)((guid.Data4 >> 24) & 255));
		}

		public override string ToString()
		{
			return this.ToString("B");
		}

		public int Data1;

		public int Data2;

		public int Data3;

		public int Data4;
	}
}
