using System;

namespace UnityEngine
{
	[Obsolete("PhysicMaterialCombine has been renamed to PhysicsMaterialCombine. Please use PhysicsMaterialCombine instead. (UnityUpgradable) -> PhysicsMaterialCombine", true)]
	public enum PhysicMaterialCombine
	{
		Average,
		Minimum = 2,
		Multiply = 1,
		Maximum = 3
	}
}
