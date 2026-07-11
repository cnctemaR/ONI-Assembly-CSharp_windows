using System;

namespace UnityEngine.Experimental.VFX
{
	internal enum VFXTaskType
	{
		None,
		Spawner = 268435456,
		Initialize = 536870912,
		Update = 805306368,
		Output = 1073741824,
		CameraSort = 805306369,
		ParticlePointOutput = 1073741824,
		ParticleLineOutput,
		ParticleQuadOutput,
		ParticleHexahedronOutput,
		ParticleMeshOutput,
		ConstantRateSpawner = 268435456,
		BurstSpawner,
		PeriodicBurstSpawner,
		VariableRateSpawner,
		CustomCallbackSpawner,
		SetAttributeSpawner
	}
}
