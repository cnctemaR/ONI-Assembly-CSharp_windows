using System;
using System.ComponentModel;

namespace UnityEngine.VFX
{
	[Flags]
	internal enum VFXInstancingDisabledReason
	{
		None = 0,
		[Description("A system is using indirect draw.")]
		IndirectDraw = 1,
		[Description("The effect is using output events.")]
		OutputEvent = 2,
		[Description("The effect is using GPU events.")]
		GPUEvent = 4,
		[Description("An Initialize node has Bounds Mode set to 'Automatic'.")]
		AutomaticBounds = 8,
		[Description("The effect contains a mesh output.")]
		MeshOutput = 16,
		[Description("The effect has exposed texture, mesh or graphics buffer properties.")]
		ExposedObject = 32,
		[Description("The effect uses Shader Keywords in particle output.")]
		ShaderKeyword = 64,
		[Description("Unknown reason.")]
		Unknown = -1
	}
}
