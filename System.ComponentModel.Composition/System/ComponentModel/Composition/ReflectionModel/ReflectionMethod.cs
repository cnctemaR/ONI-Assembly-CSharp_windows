using System;
using System.ComponentModel.Composition.Primitives;
using System.Reflection;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.ReflectionModel
{
	internal class ReflectionMethod : ReflectionMember
	{
		public ReflectionMethod(MethodInfo method)
		{
			Assumes.NotNull<MethodInfo>(method);
			this._method = method;
		}

		public MethodInfo UnderlyingMethod
		{
			get
			{
				return this._method;
			}
		}

		public override MemberInfo UnderlyingMember
		{
			get
			{
				return this.UnderlyingMethod;
			}
		}

		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		public override bool RequiresInstance
		{
			get
			{
				return !this.UnderlyingMethod.IsStatic;
			}
		}

		public override Type ReturnType
		{
			get
			{
				return this.UnderlyingMethod.ReturnType;
			}
		}

		public override ReflectionItemType ItemType
		{
			get
			{
				return ReflectionItemType.Method;
			}
		}

		public override object GetValue(object instance)
		{
			return ReflectionMethod.SafeCreateExportedDelegate(instance, this._method);
		}

		private static ExportedDelegate SafeCreateExportedDelegate(object instance, MethodInfo method)
		{
			ReflectionInvoke.DemandMemberAccessIfNeeded(method);
			return new ExportedDelegate(instance, method);
		}

		private readonly MethodInfo _method;
	}
}
