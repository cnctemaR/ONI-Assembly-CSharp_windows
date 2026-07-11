using System;

namespace UnityEngine.Lumin
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class UsesLuminPlatformLevelAttribute : Attribute
	{
		public UsesLuminPlatformLevelAttribute(uint platformLevel)
		{
			this.m_PlatformLevel = platformLevel;
		}

		public uint platformLevel
		{
			get
			{
				return this.m_PlatformLevel;
			}
		}

		private readonly uint m_PlatformLevel;
	}
}
