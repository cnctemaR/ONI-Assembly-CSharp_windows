using System;
using System.Security.Principal;

namespace System.Security.AccessControl
{
	public sealed class RawSecurityDescriptor : GenericSecurityDescriptor
	{
		public RawSecurityDescriptor(string sddlForm)
		{
			if (sddlForm == null)
			{
				throw new ArgumentNullException("sddlForm");
			}
			this.ParseSddl(sddlForm.Replace(" ", ""));
			this.control_flags |= ControlFlags.SelfRelative;
		}

		public RawSecurityDescriptor(byte[] binaryForm, int offset)
		{
			if (binaryForm == null)
			{
				throw new ArgumentNullException("binaryForm");
			}
			if (offset < 0 || offset > binaryForm.Length - 20)
			{
				throw new ArgumentOutOfRangeException("offset", offset, "Offset out of range");
			}
			if (binaryForm[offset] != 1)
			{
				throw new ArgumentException("Unrecognized Security Descriptor revision.", "binaryForm");
			}
			this.resourcemgr_control = binaryForm[offset + 1];
			this.control_flags = (ControlFlags)this.ReadUShort(binaryForm, offset + 2);
			int num = this.ReadInt(binaryForm, offset + 4);
			int num2 = this.ReadInt(binaryForm, offset + 8);
			int num3 = this.ReadInt(binaryForm, offset + 12);
			int num4 = this.ReadInt(binaryForm, offset + 16);
			if (num != 0)
			{
				this.owner_sid = new SecurityIdentifier(binaryForm, num);
			}
			if (num2 != 0)
			{
				this.group_sid = new SecurityIdentifier(binaryForm, num2);
			}
			if (num3 != 0)
			{
				this.system_acl = new RawAcl(binaryForm, num3);
			}
			if (num4 != 0)
			{
				this.discretionary_acl = new RawAcl(binaryForm, num4);
			}
		}

		public RawSecurityDescriptor(ControlFlags flags, SecurityIdentifier owner, SecurityIdentifier group, RawAcl systemAcl, RawAcl discretionaryAcl)
		{
			this.control_flags = flags;
			this.owner_sid = owner;
			this.group_sid = group;
			this.system_acl = systemAcl;
			this.discretionary_acl = discretionaryAcl;
		}

		public override ControlFlags ControlFlags
		{
			get
			{
				return this.control_flags;
			}
		}

		public RawAcl DiscretionaryAcl
		{
			get
			{
				return this.discretionary_acl;
			}
			set
			{
				this.discretionary_acl = value;
			}
		}

		public override SecurityIdentifier Group
		{
			get
			{
				return this.group_sid;
			}
			set
			{
				this.group_sid = value;
			}
		}

		public override SecurityIdentifier Owner
		{
			get
			{
				return this.owner_sid;
			}
			set
			{
				this.owner_sid = value;
			}
		}

		public byte ResourceManagerControl
		{
			get
			{
				return this.resourcemgr_control;
			}
			set
			{
				this.resourcemgr_control = value;
			}
		}

		public RawAcl SystemAcl
		{
			get
			{
				return this.system_acl;
			}
			set
			{
				this.system_acl = value;
			}
		}

		public void SetFlags(ControlFlags flags)
		{
			this.control_flags = flags | ControlFlags.SelfRelative;
		}

		internal override GenericAcl InternalDacl
		{
			get
			{
				return this.DiscretionaryAcl;
			}
		}

		internal override GenericAcl InternalSacl
		{
			get
			{
				return this.SystemAcl;
			}
		}

		internal override byte InternalReservedField
		{
			get
			{
				return this.ResourceManagerControl;
			}
		}

		private void ParseSddl(string sddlForm)
		{
			ControlFlags controlFlags = ControlFlags.None;
			int i = 0;
			while (i < sddlForm.Length - 2)
			{
				string text = sddlForm.Substring(i, 2);
				if (!(text == "O:"))
				{
					if (!(text == "G:"))
					{
						if (!(text == "D:"))
						{
							if (!(text == "S:"))
							{
								throw new ArgumentException("Invalid SDDL.", "sddlForm");
							}
							i += 2;
							this.SystemAcl = RawAcl.ParseSddlForm(sddlForm, false, ref controlFlags, ref i);
							controlFlags |= ControlFlags.SystemAclPresent;
						}
						else
						{
							i += 2;
							this.DiscretionaryAcl = RawAcl.ParseSddlForm(sddlForm, true, ref controlFlags, ref i);
							controlFlags |= ControlFlags.DiscretionaryAclPresent;
						}
					}
					else
					{
						i += 2;
						this.Group = SecurityIdentifier.ParseSddlForm(sddlForm, ref i);
					}
				}
				else
				{
					i += 2;
					this.Owner = SecurityIdentifier.ParseSddlForm(sddlForm, ref i);
				}
			}
			if (i != sddlForm.Length)
			{
				throw new ArgumentException("Invalid SDDL.", "sddlForm");
			}
			this.SetFlags(controlFlags);
		}

		private ushort ReadUShort(byte[] buffer, int offset)
		{
			return (ushort)((int)buffer[offset] | ((int)buffer[offset + 1] << 8));
		}

		private int ReadInt(byte[] buffer, int offset)
		{
			return (int)buffer[offset] | ((int)buffer[offset + 1] << 8) | ((int)buffer[offset + 2] << 16) | ((int)buffer[offset + 3] << 24);
		}

		private ControlFlags control_flags;

		private SecurityIdentifier owner_sid;

		private SecurityIdentifier group_sid;

		private RawAcl system_acl;

		private RawAcl discretionary_acl;

		private byte resourcemgr_control;
	}
}
