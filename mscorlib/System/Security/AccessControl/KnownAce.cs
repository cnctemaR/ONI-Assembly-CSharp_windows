using System;
using System.Globalization;
using System.Security.Principal;
using System.Text;
using Unity;

namespace System.Security.AccessControl
{
	public abstract class KnownAce : GenericAce
	{
		internal KnownAce(AceType type, AceFlags flags)
			: base(type, flags)
		{
		}

		internal KnownAce(byte[] binaryForm, int offset)
			: base(binaryForm, offset)
		{
		}

		public int AccessMask
		{
			get
			{
				return this.access_mask;
			}
			set
			{
				this.access_mask = value;
			}
		}

		public SecurityIdentifier SecurityIdentifier
		{
			get
			{
				return this.identifier;
			}
			set
			{
				this.identifier = value;
			}
		}

		internal static string GetSddlAccessRights(int accessMask)
		{
			string sddlAliasRights = KnownAce.GetSddlAliasRights(accessMask);
			if (!string.IsNullOrEmpty(sddlAliasRights))
			{
				return sddlAliasRights;
			}
			return string.Format(CultureInfo.InvariantCulture, "0x{0:x}", accessMask);
		}

		private static string GetSddlAliasRights(int accessMask)
		{
			SddlAccessRight[] array = SddlAccessRight.Decompose(accessMask);
			if (array == null)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (SddlAccessRight sddlAccessRight in array)
			{
				stringBuilder.Append(sddlAccessRight.Name);
			}
			return stringBuilder.ToString();
		}

		internal KnownAce()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private int access_mask;

		private SecurityIdentifier identifier;
	}
}
