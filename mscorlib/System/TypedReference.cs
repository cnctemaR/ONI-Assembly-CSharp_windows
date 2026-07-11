using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System
{
	[CLSCompliant(false)]
	[ComVisible(true)]
	public struct TypedReference
	{
		public override bool Equals(object o)
		{
			throw new NotSupportedException(Locale.GetText("This operation is not supported for this type."));
		}

		public override int GetHashCode()
		{
			if (this.type.Value == IntPtr.Zero)
			{
				return 0;
			}
			return Type.GetTypeFromHandle(this.type).GetHashCode();
		}

		public static Type GetTargetType(TypedReference value)
		{
			return Type.GetTypeFromHandle(value.type);
		}

		[MonoTODO]
		[CLSCompliant(false)]
		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"MemberAccess\"/>\n</PermissionSet>\n")]
		public static TypedReference MakeTypedReference(object target, FieldInfo[] flds)
		{
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}
			if (flds == null)
			{
				throw new ArgumentNullException("flds");
			}
			if (flds.Length == 0)
			{
				throw new ArgumentException(Locale.GetText("flds has no elements"));
			}
			throw new NotImplementedException();
		}

		[MonoTODO]
		[CLSCompliant(false)]
		public static void SetTypedReference(TypedReference target, object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			throw new NotImplementedException();
		}

		public static RuntimeTypeHandle TargetTypeToken(TypedReference value)
		{
			return value.type;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern object ToObject(TypedReference value);

		private RuntimeTypeHandle type;

		private IntPtr value;

		private IntPtr klass;
	}
}
