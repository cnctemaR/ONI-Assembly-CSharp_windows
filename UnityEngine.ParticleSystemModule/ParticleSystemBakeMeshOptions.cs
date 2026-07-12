using System;

namespace UnityEngine
{
	[Flags]
	public enum ParticleSystemBakeMeshOptions
	{
		BakeRotationAndScale = 1,
		BakePosition = 2,
		Default = 0
	}
}
