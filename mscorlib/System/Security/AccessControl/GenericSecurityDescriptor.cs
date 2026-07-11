using System;
using System.Globalization;
using System.Security.Principal;
using System.Text;

namespace System.Security.AccessControl
{
	public abstract class GenericSecurityDescriptor
	{
		public int BinaryLength
		{
			get
			{
				int num = 20;
				if (this.Owner != null)
				{
					num += this.Owner.BinaryLength;
				}
				if (this.Group != null)
				{
					num += this.Group.BinaryLength;
				}
				if (this.DaclPresent && !this.DaclIsUnmodifiedAefa)
				{
					num += this.InternalDacl.BinaryLength;
				}
				if (this.SaclPresent)
				{
					num += this.InternalSacl.BinaryLength;
				}
				return num;
			}
		}

		public abstract ControlFlags ControlFlags { get; }

		public abstract SecurityIdentifier Group { get; set; }

		public abstract SecurityIdentifier Owner { get; set; }

		public static byte Revision
		{
			get
			{
				return 1;
			}
		}

		internal virtual GenericAcl InternalDacl
		{
			get
			{
				return null;
			}
		}

		internal virtual GenericAcl InternalSacl
		{
			get
			{
				return null;
			}
		}

		internal virtual byte InternalReservedField
		{
			get
			{
				return 0;
			}
		}

		public void GetBinaryForm(byte[] binaryForm, int offset)
		{
			if (binaryForm == null)
			{
				throw new ArgumentNullException("binaryForm");
			}
			int binaryLength = this.BinaryLength;
			if (offset < 0 || offset > binaryForm.Length - binaryLength)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			ControlFlags controlFlags = this.ControlFlags;
			if (this.DaclIsUnmodifiedAefa)
			{
				controlFlags &= ~ControlFlags.DiscretionaryAclPresent;
			}
			binaryForm[offset] = GenericSecurityDescriptor.Revision;
			binaryForm[offset + 1] = this.InternalReservedField;
			this.WriteUShort((ushort)controlFlags, binaryForm, offset + 2);
			int num = 20;
			if (this.Owner != null)
			{
				this.WriteInt(num, binaryForm, offset + 4);
				this.Owner.GetBinaryForm(binaryForm, offset + num);
				num += this.Owner.BinaryLength;
			}
			else
			{
				this.WriteInt(0, binaryForm, offset + 4);
			}
			if (this.Group != null)
			{
				this.WriteInt(num, binaryForm, offset + 8);
				this.Group.GetBinaryForm(binaryForm, offset + num);
				num += this.Group.BinaryLength;
			}
			else
			{
				this.WriteInt(0, binaryForm, offset + 8);
			}
			GenericAcl internalSacl = this.InternalSacl;
			if (this.SaclPresent)
			{
				this.WriteInt(num, binaryForm, offset + 12);
				internalSacl.GetBinaryForm(binaryForm, offset + num);
				num += this.InternalSacl.BinaryLength;
			}
			else
			{
				this.WriteInt(0, binaryForm, offset + 12);
			}
			GenericAcl internalDacl = this.InternalDacl;
			if (this.DaclPresent && !this.DaclIsUnmodifiedAefa)
			{
				this.WriteInt(num, binaryForm, offset + 16);
				internalDacl.GetBinaryForm(binaryForm, offset + num);
				num += this.InternalDacl.BinaryLength;
				return;
			}
			this.WriteInt(0, binaryForm, offset + 16);
		}

		public string GetSddlForm(AccessControlSections includeSections)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if ((includeSections & AccessControlSections.Owner) != AccessControlSections.None && this.Owner != null)
			{
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "O:{0}", this.Owner.GetSddlForm());
			}
			if ((includeSections & AccessControlSections.Group) != AccessControlSections.None && this.Group != null)
			{
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "G:{0}", this.Group.GetSddlForm());
			}
			if ((includeSections & AccessControlSections.Access) != AccessControlSections.None && this.DaclPresent && !this.DaclIsUnmodifiedAefa)
			{
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "D:{0}", this.InternalDacl.GetSddlForm(this.ControlFlags, true));
			}
			if ((includeSections & AccessControlSections.Audit) != AccessControlSections.None && this.SaclPresent)
			{
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "S:{0}", this.InternalSacl.GetSddlForm(this.ControlFlags, false));
			}
			return stringBuilder.ToString();
		}

		public static bool IsSddlConversionSupported()
		{
			return true;
		}

		internal virtual bool DaclIsUnmodifiedAefa
		{
			get
			{
				return false;
			}
		}

		private bool DaclPresent
		{
			get
			{
				return this.InternalDacl != null && (this.ControlFlags & ControlFlags.DiscretionaryAclPresent) > ControlFlags.None;
			}
		}

		private bool SaclPresent
		{
			get
			{
				return this.InternalSacl != null && (this.ControlFlags & ControlFlags.SystemAclPresent) > ControlFlags.None;
			}
		}

		private void WriteUShort(ushort val, byte[] buffer, int offset)
		{
			buffer[offset] = (byte)val;
			buffer[offset + 1] = (byte)(val >> 8);
		}

		private void WriteInt(int val, byte[] buffer, int offset)
		{
			buffer[offset] = (byte)val;
			buffer[offset + 1] = (byte)(val >> 8);
			buffer[offset + 2] = (byte)(val >> 16);
			buffer[offset + 3] = (byte)(val >> 24);
		}
	}
}
