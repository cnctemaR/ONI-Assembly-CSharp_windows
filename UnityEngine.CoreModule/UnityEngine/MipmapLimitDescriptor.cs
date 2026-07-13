using System;

namespace UnityEngine
{
	public struct MipmapLimitDescriptor
	{
		public readonly bool useMipmapLimit { get; }

		public readonly string groupName { get; }

		public MipmapLimitDescriptor(bool useMipmapLimit, string groupName)
		{
			this.useMipmapLimit = useMipmapLimit;
			this.groupName = groupName;
		}
	}
}
