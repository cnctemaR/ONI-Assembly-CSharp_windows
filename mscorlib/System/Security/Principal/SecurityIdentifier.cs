using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Security.Principal
{
	[ComVisible(false)]
	public sealed class SecurityIdentifier : IdentityReference, IComparable<SecurityIdentifier>
	{
		public SecurityIdentifier(string sddlForm)
		{
			if (sddlForm == null)
			{
				throw new ArgumentNullException("sddlForm");
			}
			this.buffer = SecurityIdentifier.ParseSddlForm(sddlForm);
		}

		public unsafe SecurityIdentifier(byte[] binaryForm, int offset)
		{
			if (binaryForm == null)
			{
				throw new ArgumentNullException("binaryForm");
			}
			if (offset < 0 || offset > binaryForm.Length - 2)
			{
				throw new ArgumentException("offset");
			}
			fixed (byte[] array = binaryForm)
			{
				byte* ptr;
				if (binaryForm == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				this.CreateFromBinaryForm((IntPtr)((void*)(ptr + offset)), binaryForm.Length - offset);
			}
		}

		public SecurityIdentifier(IntPtr binaryForm)
		{
			this.CreateFromBinaryForm(binaryForm, int.MaxValue);
		}

		private void CreateFromBinaryForm(IntPtr binaryForm, int length)
		{
			int num = (int)Marshal.ReadByte(binaryForm, 0);
			int num2 = (int)Marshal.ReadByte(binaryForm, 1);
			if (num != 1 || num2 > 15)
			{
				throw new ArgumentException("Value was invalid.");
			}
			if (length < 8 + num2 * 4)
			{
				throw new ArgumentException("offset");
			}
			this.buffer = new byte[8 + num2 * 4];
			Marshal.Copy(binaryForm, this.buffer, 0, this.buffer.Length);
		}

		public SecurityIdentifier(WellKnownSidType sidType, SecurityIdentifier domainSid)
		{
			WellKnownAccount wellKnownAccount = WellKnownAccount.LookupByType(sidType);
			if (wellKnownAccount == null)
			{
				throw new ArgumentException("Unable to convert SID type: " + sidType);
			}
			if (wellKnownAccount.IsAbsolute)
			{
				this.buffer = SecurityIdentifier.ParseSddlForm(wellKnownAccount.Sid);
				return;
			}
			if (domainSid == null)
			{
				throw new ArgumentNullException("domainSid");
			}
			this.buffer = SecurityIdentifier.ParseSddlForm(domainSid.Value + "-" + wellKnownAccount.Rid);
		}

		public SecurityIdentifier AccountDomainSid
		{
			get
			{
				if (!this.Value.StartsWith("S-1-5-21") || this.buffer[1] < 4)
				{
					return null;
				}
				byte[] array = new byte[24];
				Array.Copy(this.buffer, 0, array, 0, array.Length);
				array[1] = 4;
				return new SecurityIdentifier(array, 0);
			}
		}

		public int BinaryLength
		{
			get
			{
				return this.buffer.Length;
			}
		}

		public override string Value
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				ulong sidAuthority = this.GetSidAuthority();
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "S-1-{0}", sidAuthority);
				for (byte b = 0; b < this.GetSidSubAuthorityCount(); b += 1)
				{
					stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "-{0}", this.GetSidSubAuthority(b));
				}
				return stringBuilder.ToString();
			}
		}

		private ulong GetSidAuthority()
		{
			return ((ulong)this.buffer[2] << 40) | ((ulong)this.buffer[3] << 32) | ((ulong)this.buffer[4] << 24) | ((ulong)this.buffer[5] << 16) | ((ulong)this.buffer[6] << 8) | (ulong)this.buffer[7];
		}

		private byte GetSidSubAuthorityCount()
		{
			return this.buffer[1];
		}

		private uint GetSidSubAuthority(byte index)
		{
			int num = (int)(8 + index * 4);
			return (uint)((int)this.buffer[num] | ((int)this.buffer[num + 1] << 8) | ((int)this.buffer[num + 2] << 16) | ((int)this.buffer[num + 3] << 24));
		}

		public int CompareTo(SecurityIdentifier sid)
		{
			if (sid == null)
			{
				throw new ArgumentNullException("sid");
			}
			int num;
			if ((num = this.GetSidAuthority().CompareTo(sid.GetSidAuthority())) != 0)
			{
				return num;
			}
			if ((num = this.GetSidSubAuthorityCount().CompareTo(sid.GetSidSubAuthorityCount())) != 0)
			{
				return num;
			}
			for (byte b = 0; b < this.GetSidSubAuthorityCount(); b += 1)
			{
				if ((num = this.GetSidSubAuthority(b).CompareTo(sid.GetSidSubAuthority(b))) != 0)
				{
					return num;
				}
			}
			return 0;
		}

		public override bool Equals(object o)
		{
			return this.Equals(o as SecurityIdentifier);
		}

		public bool Equals(SecurityIdentifier sid)
		{
			return !(sid == null) && sid.Value == this.Value;
		}

		public void GetBinaryForm(byte[] binaryForm, int offset)
		{
			if (binaryForm == null)
			{
				throw new ArgumentNullException("binaryForm");
			}
			if (offset < 0 || offset > binaryForm.Length - this.buffer.Length)
			{
				throw new ArgumentException("offset");
			}
			Array.Copy(this.buffer, 0, binaryForm, offset, this.buffer.Length);
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public bool IsAccountSid()
		{
			return this.AccountDomainSid != null;
		}

		public bool IsEqualDomainSid(SecurityIdentifier sid)
		{
			SecurityIdentifier accountDomainSid = this.AccountDomainSid;
			return !(accountDomainSid == null) && accountDomainSid.Equals(sid.AccountDomainSid);
		}

		public override bool IsValidTargetType(Type targetType)
		{
			return targetType == typeof(SecurityIdentifier) || targetType == typeof(NTAccount);
		}

		public bool IsWellKnown(WellKnownSidType type)
		{
			WellKnownAccount wellKnownAccount = WellKnownAccount.LookupByType(type);
			if (wellKnownAccount == null)
			{
				return false;
			}
			string value = this.Value;
			if (wellKnownAccount.IsAbsolute)
			{
				return value == wellKnownAccount.Sid;
			}
			return value.StartsWith("S-1-5-21", StringComparison.OrdinalIgnoreCase) && value.EndsWith("-" + wellKnownAccount.Rid, StringComparison.OrdinalIgnoreCase);
		}

		public override string ToString()
		{
			return this.Value;
		}

		public override IdentityReference Translate(Type targetType)
		{
			if (targetType == typeof(SecurityIdentifier))
			{
				return this;
			}
			if (!(targetType == typeof(NTAccount)))
			{
				throw new ArgumentException("Unknown type.", "targetType");
			}
			WellKnownAccount wellKnownAccount = WellKnownAccount.LookupBySid(this.Value);
			if (wellKnownAccount == null || wellKnownAccount.Name == null)
			{
				throw new IdentityNotMappedException("Unable to map SID: " + this.Value);
			}
			return new NTAccount(wellKnownAccount.Name);
		}

		public static bool operator ==(SecurityIdentifier left, SecurityIdentifier right)
		{
			if (left == null)
			{
				return right == null;
			}
			return right != null && left.Value == right.Value;
		}

		public static bool operator !=(SecurityIdentifier left, SecurityIdentifier right)
		{
			if (left == null)
			{
				return right != null;
			}
			return right == null || left.Value != right.Value;
		}

		internal string GetSddlForm()
		{
			string value = this.Value;
			WellKnownAccount wellKnownAccount = WellKnownAccount.LookupBySid(value);
			if (wellKnownAccount == null || wellKnownAccount.SddlForm == null)
			{
				return value;
			}
			return wellKnownAccount.SddlForm;
		}

		internal static SecurityIdentifier ParseSddlForm(string sddlForm, ref int pos)
		{
			if (sddlForm.Length - pos < 2)
			{
				throw new ArgumentException("Invalid SDDL string.", "sddlForm");
			}
			string text = sddlForm.Substring(pos, 2).ToUpperInvariant();
			string text2;
			int num2;
			if (text == "S-")
			{
				int num = pos;
				char c = char.ToUpperInvariant(sddlForm[num]);
				while (c == 'S' || c == '-' || c == 'X' || (c >= '0' && c <= '9') || (c >= 'A' && c <= 'F'))
				{
					num++;
					c = char.ToUpperInvariant(sddlForm[num]);
				}
				text2 = sddlForm.Substring(pos, num - pos);
				num2 = num - pos;
			}
			else
			{
				text2 = text;
				num2 = 2;
			}
			SecurityIdentifier securityIdentifier = new SecurityIdentifier(text2);
			pos += num2;
			return securityIdentifier;
		}

		private static byte[] ParseSddlForm(string sddlForm)
		{
			string text = sddlForm;
			if (sddlForm.Length == 2)
			{
				WellKnownAccount wellKnownAccount = WellKnownAccount.LookupBySddlForm(sddlForm);
				if (wellKnownAccount == null)
				{
					throw new ArgumentException("Invalid SDDL string - unrecognized account: " + sddlForm, "sddlForm");
				}
				if (!wellKnownAccount.IsAbsolute)
				{
					throw new NotImplementedException("Mono unable to convert account to SID: " + ((wellKnownAccount.Name != null) ? wellKnownAccount.Name : sddlForm));
				}
				text = wellKnownAccount.Sid;
			}
			string[] array = text.ToUpperInvariant().Split(new char[] { '-' });
			int num = array.Length - 3;
			if (array.Length < 3 || array[0] != "S" || num > 15)
			{
				throw new ArgumentException("Value was invalid.");
			}
			if (array[1] != "1")
			{
				throw new ArgumentException("Only SIDs with revision 1 are supported");
			}
			byte[] array2 = new byte[8 + num * 4];
			array2[0] = 1;
			array2[1] = (byte)num;
			ulong num2;
			if (!SecurityIdentifier.TryParseAuthority(array[2], out num2))
			{
				throw new ArgumentException("Value was invalid.");
			}
			array2[2] = (byte)((num2 >> 40) & 255UL);
			array2[3] = (byte)((num2 >> 32) & 255UL);
			array2[4] = (byte)((num2 >> 24) & 255UL);
			array2[5] = (byte)((num2 >> 16) & 255UL);
			array2[6] = (byte)((num2 >> 8) & 255UL);
			array2[7] = (byte)(num2 & 255UL);
			for (int i = 0; i < num; i++)
			{
				uint num3;
				if (!SecurityIdentifier.TryParseSubAuthority(array[i + 3], out num3))
				{
					throw new ArgumentException("Value was invalid.");
				}
				int num4 = 8 + i * 4;
				array2[num4] = (byte)num3;
				array2[num4 + 1] = (byte)(num3 >> 8);
				array2[num4 + 2] = (byte)(num3 >> 16);
				array2[num4 + 3] = (byte)(num3 >> 24);
			}
			return array2;
		}

		private static bool TryParseAuthority(string s, out ulong result)
		{
			if (s.StartsWith("0X"))
			{
				return ulong.TryParse(s.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result);
			}
			return ulong.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
		}

		private static bool TryParseSubAuthority(string s, out uint result)
		{
			if (s.StartsWith("0X"))
			{
				return uint.TryParse(s.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result);
			}
			return uint.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
		}

		private byte[] buffer;

		public static readonly int MaxBinaryLength = 68;

		public static readonly int MinBinaryLength = 8;
	}
}
