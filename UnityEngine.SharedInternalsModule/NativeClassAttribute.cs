using System;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[VisibleToOtherModules]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
	internal sealed class NativeClassAttribute : Attribute
	{
		public string QualifiedNativeName { get; private set; }

		public string Declaration { get; private set; }

		public NativeClassAttribute(string qualifiedCppName)
		{
			this.QualifiedNativeName = qualifiedCppName;
			this.Declaration = "class " + qualifiedCppName;
		}

		public NativeClassAttribute(string qualifiedCppName, string declaration)
		{
			this.QualifiedNativeName = qualifiedCppName;
			this.Declaration = declaration;
		}
	}
}
