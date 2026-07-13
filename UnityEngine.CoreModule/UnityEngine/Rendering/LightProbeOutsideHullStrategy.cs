using System;

namespace UnityEngine.Rendering
{
	public enum LightProbeOutsideHullStrategy
	{
		[InspectorName("Find closest Light Probe")]
		kLightProbeSearchTetrahedralHull,
		[InspectorName("Use Ambient Probe")]
		kLightProbeUseAmbientProbe
	}
}
