using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace System.Diagnostics.CodeAnalysis
{
	[Nullable(0)]
	[NullableContext(2)]
	[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
	[VisibleToOtherModules]
	internal sealed class DynamicDependencyAttribute : Attribute
	{
		[NullableContext(1)]
		public DynamicDependencyAttribute(string memberSignature)
		{
			this.MemberSignature = memberSignature;
		}

		[NullableContext(1)]
		public DynamicDependencyAttribute(string memberSignature, Type type)
		{
			this.MemberSignature = memberSignature;
			this.Type = type;
		}

		[NullableContext(1)]
		public DynamicDependencyAttribute(string memberSignature, string typeName, string assemblyName)
		{
			this.MemberSignature = memberSignature;
			this.TypeName = typeName;
			this.AssemblyName = assemblyName;
		}

		public string MemberSignature { get; }

		public Type Type { get; }

		public string TypeName { get; }

		public string AssemblyName { get; }

		public string Condition { get; set; }
	}
}
