using System;
using Unity.Properties;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal readonly struct BindingTypeResult
	{
		internal BindingTypeResult(Type type, in PropertyPath resolvedPath)
		{
			this.type = type;
			this.resolvedPath = resolvedPath;
			this.returnCode = VisitReturnCode.Ok;
			this.errorIndex = -1;
		}

		internal BindingTypeResult(VisitReturnCode returnCode, int errorIndex, in PropertyPath resolvedPath)
		{
			this.type = null;
			this.resolvedPath = resolvedPath;
			this.returnCode = returnCode;
			this.errorIndex = errorIndex;
		}

		public readonly Type type;

		public readonly VisitReturnCode returnCode;

		public readonly int errorIndex;

		public readonly PropertyPath resolvedPath;
	}
}
