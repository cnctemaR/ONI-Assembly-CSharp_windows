using System;

namespace Unity.Collections
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
	public class GenerateTestsForBurstCompatibilityAttribute : Attribute
	{
		public Type[] GenericTypeArguments { get; set; }

		public string RequiredUnityDefine;

		public GenerateTestsForBurstCompatibilityAttribute.BurstCompatibleCompileTarget CompileTarget;

		public enum BurstCompatibleCompileTarget
		{
			Player,
			Editor,
			PlayerAndEditor
		}
	}
}
