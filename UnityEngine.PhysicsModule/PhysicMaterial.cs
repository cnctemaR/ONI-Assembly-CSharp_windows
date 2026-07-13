using System;

namespace UnityEngine
{
	[Obsolete("PhysicMaterial has been renamed to PhysicsMaterial. Please use PhysicsMaterial instead. (UnityUpgradable) -> PhysicsMaterial", true)]
	[NativeClass(null)]
	public class PhysicMaterial : Object
	{
		public PhysicMaterial()
		{
		}

		public PhysicMaterial(string name)
		{
		}

		public float bounciness { get; set; }

		public float dynamicFriction { get; set; }

		public float staticFriction { get; set; }

		public PhysicMaterialCombine frictionCombine { get; set; }

		public PhysicMaterialCombine bounceCombine { get; set; }

		[Obsolete("Use PhysicMaterial.bounciness instead (UnityUpgradable) -> bounciness")]
		public float bouncyness { get; set; }
	}
}
