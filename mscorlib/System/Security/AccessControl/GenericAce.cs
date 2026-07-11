using System;

namespace System.Security.AccessControl
{
	public abstract class GenericAce
	{
		internal GenericAce(InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags)
		{
			this.inheritance = inheritanceFlags;
			this.propagation = propagationFlags;
		}

		internal GenericAce(AceType type)
		{
			if (type <= AceType.SystemAlarmCallbackObject)
			{
				throw new ArgumentOutOfRangeException("type");
			}
			this.ace_type = type;
		}

		public AceFlags AceFlags
		{
			get
			{
				return this.aceflags;
			}
			set
			{
				this.aceflags = value;
			}
		}

		public AceType AceType
		{
			get
			{
				return this.ace_type;
			}
		}

		public AuditFlags AuditFlags
		{
			get
			{
				AuditFlags auditFlags = AuditFlags.None;
				if ((byte)(this.aceflags & AceFlags.SuccessfulAccess) != 0)
				{
					auditFlags |= AuditFlags.Success;
				}
				if ((byte)(this.aceflags & AceFlags.FailedAccess) != 0)
				{
					auditFlags |= AuditFlags.Failure;
				}
				return auditFlags;
			}
		}

		public abstract int BinaryLength { get; }

		public InheritanceFlags InheritanceFlags
		{
			get
			{
				return this.inheritance;
			}
		}

		[MonoTODO]
		public bool IsInherited
		{
			get
			{
				return false;
			}
		}

		public PropagationFlags PropagationFlags
		{
			get
			{
				return this.propagation;
			}
		}

		[MonoTODO]
		public GenericAce Copy()
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public static GenericAce CreateFromBinaryForm(byte[] binaryForm, int offset)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public sealed override bool Equals(object o)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public abstract void GetBinaryForm(byte[] binaryForm, int offset);

		[MonoTODO]
		public sealed override int GetHashCode()
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public static bool operator ==(GenericAce left, GenericAce right)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public static bool operator !=(GenericAce left, GenericAce right)
		{
			throw new NotImplementedException();
		}

		private InheritanceFlags inheritance;

		private PropagationFlags propagation;

		private AceFlags aceflags;

		private AceType ace_type;
	}
}
