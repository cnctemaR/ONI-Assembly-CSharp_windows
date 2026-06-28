using System;
using System.Security.Principal;

namespace System.Security.AccessControl
{
	public abstract class ObjectAccessRule : AccessRule
	{
		protected ObjectAccessRule(IdentityReference identity, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, Guid objectType, Guid inheritedObjectType, AccessControlType type)
			: base(identity, accessMask, isInherited, inheritanceFlags, propagationFlags, type)
		{
			this.object_type = objectType;
			this.inherited_object_type = inheritedObjectType;
		}

		public Guid InheritedObjectType
		{
			get
			{
				return this.inherited_object_type;
			}
		}

		public ObjectAceFlags ObjectFlags
		{
			get
			{
				ObjectAceFlags objectAceFlags = ObjectAceFlags.None;
				if (this.object_type != Guid.Empty)
				{
					objectAceFlags |= ObjectAceFlags.ObjectAceTypePresent;
				}
				if (this.inherited_object_type != Guid.Empty)
				{
					objectAceFlags |= ObjectAceFlags.InheritedObjectAceTypePresent;
				}
				return objectAceFlags;
			}
		}

		public Guid ObjectType
		{
			get
			{
				return this.object_type;
			}
		}

		private Guid object_type;

		private Guid inherited_object_type;
	}
}
