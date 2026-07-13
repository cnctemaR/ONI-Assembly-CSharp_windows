using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = true)]
	[RequiredByNativeCode]
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal sealed class ExtensionOfNativeClassAttribute : Attribute
	{
	}
}
