using System;
using System.Runtime.InteropServices;

namespace System.Security.Principal
{
	[ComVisible(false)]
	[MonoTODO("not implemented")]
	public sealed class SecurityIdentifier : IdentityReference, IComparable<SecurityIdentifier>
	{
		public SecurityIdentifier(string sddlForm)
		{
			if (sddlForm == null)
			{
				throw new ArgumentNullException("sddlForm");
			}
			this._value = sddlForm.ToUpperInvariant();
		}

		public SecurityIdentifier(byte[] binaryForm, int offset)
		{
			if (binaryForm == null)
			{
				throw new ArgumentNullException("binaryForm");
			}
			if (offset < 0 || offset > binaryForm.Length - 1)
			{
				throw new ArgumentException("offset");
			}
			throw new NotImplementedException();
		}

		public SecurityIdentifier(IntPtr binaryForm)
		{
			throw new NotImplementedException();
		}

		public SecurityIdentifier(WellKnownSidType sidType, SecurityIdentifier domainSid)
		{
			switch (sidType)
			{
			case WellKnownSidType.AccountAdministratorSid:
			case WellKnownSidType.AccountGuestSid:
			case WellKnownSidType.AccountKrbtgtSid:
			case WellKnownSidType.AccountDomainAdminsSid:
			case WellKnownSidType.AccountDomainUsersSid:
			case WellKnownSidType.AccountDomainGuestsSid:
			case WellKnownSidType.AccountComputersSid:
			case WellKnownSidType.AccountControllersSid:
			case WellKnownSidType.AccountCertAdminsSid:
			case WellKnownSidType.AccountSchemaAdminsSid:
			case WellKnownSidType.AccountEnterpriseAdminsSid:
			case WellKnownSidType.AccountPolicyAdminsSid:
			case WellKnownSidType.AccountRasAndIasServersSid:
				if (domainSid == null)
				{
					throw new ArgumentNullException("domainSid");
				}
				break;
			default:
				if (sidType == WellKnownSidType.LogonIdsSid)
				{
					throw new ArgumentException("sidType");
				}
				break;
			}
		}

		public SecurityIdentifier AccountDomainSid
		{
			get
			{
				throw new ArgumentNullException("AccountDomainSid");
			}
		}

		public int BinaryLength
		{
			get
			{
				return -1;
			}
		}

		public override string Value
		{
			get
			{
				return this._value;
			}
		}

		public int CompareTo(SecurityIdentifier sid)
		{
			return this.Value.CompareTo(sid.Value);
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
			if (offset < 0 || offset > binaryForm.Length - 1 - this.BinaryLength)
			{
				throw new ArgumentException("offset");
			}
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public bool IsAccountSid()
		{
			throw new NotImplementedException();
		}

		public bool IsEqualDomainSid(SecurityIdentifier sid)
		{
			throw new NotImplementedException();
		}

		public override bool IsValidTargetType(Type targetType)
		{
			return targetType == typeof(SecurityIdentifier) || targetType == typeof(NTAccount);
		}

		public bool IsWellKnown(WellKnownSidType type)
		{
			throw new NotImplementedException();
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
			return null;
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

		private string _value;

		public static readonly int MaxBinaryLength;

		public static readonly int MinBinaryLength;
	}
}
