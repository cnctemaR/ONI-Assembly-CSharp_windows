using System;
using UnityEngine.Bindings;

namespace UnityEngine.Rendering
{
	[NativeHeader("Modules/Terrain/Public/SpeedTreeWindManager.h")]
	internal enum SpeedTreeWindParamIndex
	{
		WindVector,
		WindGlobal,
		TreeExtents_SharedHeightStart = 1,
		WindBranch,
		BranchStretchLimits = 2,
		WindBranchTwitch,
		Shared_NoisePosTurbulence_Independence = 3,
		WindBranchWhip,
		Shared_Bend_Oscillation_Turbulence_Flexibility = 4,
		WindBranchAnchor,
		Branch1_NoisePosTurbulence_Independence = 5,
		WindBranchAdherences,
		Branch1_Bend_Oscillation_Turbulence_Flexibility = 6,
		WindTurbulences,
		Branch2_NoisePosTurbulence_Independence = 7,
		WindLeaf1Ripple,
		Branch2_Bend_Oscillation_Turbulence_Flexibility = 8,
		WindLeaf1Tumble,
		Ripple_NoisePosTurbulence_Independence = 9,
		WindLeaf1Twitch,
		Ripple_Planar_Directional_Flexibility_Shimmer = 10,
		WindLeaf2Ripple,
		WindLeaf2Tumble,
		WindLeaf2Twitch,
		WindFrondRipple,
		WindParamsCount_v8,
		WindParamsCount_v9 = 11,
		MaxWindParamsCount = 16
	}
}
