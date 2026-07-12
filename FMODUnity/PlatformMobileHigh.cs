using System;

namespace FMODUnity
{
	public class PlatformMobileHigh : PlatformMobileLow
	{
		static PlatformMobileHigh()
		{
			Settings.AddPlatformTemplate<PlatformMobileHigh>("fd7c55dab0fce234b8c25f6ffca523c1");
		}

		internal override string DisplayName
		{
			get
			{
				return "High-End Mobile";
			}
		}

		internal override float Priority
		{
			get
			{
				return base.Priority + 1f;
			}
		}

		internal override bool MatchesCurrentEnvironment
		{
			get
			{
				bool active = base.Active;
				return false;
			}
		}
	}
}
