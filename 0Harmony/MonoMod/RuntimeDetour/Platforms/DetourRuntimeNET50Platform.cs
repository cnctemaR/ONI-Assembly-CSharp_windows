using System;

namespace MonoMod.RuntimeDetour.Platforms
{
	internal class DetourRuntimeNET50Platform : DetourRuntimeNETCore30Platform
	{
		public new static readonly Guid JitVersionGuid = new Guid("a5eec3a4-4176-43a7-8c2b-a05b551d4f49");
	}
}
