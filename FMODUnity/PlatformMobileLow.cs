using System;
using UnityEngine;

namespace FMODUnity
{
	public class PlatformMobileLow : Platform
	{
		static PlatformMobileLow()
		{
			Settings.AddPlatformTemplate<PlatformMobileLow>("c88d16e5272a4e241b0ef0ac2e53b73d");
		}

		public override string DisplayName
		{
			get
			{
				return "Low-End Mobile";
			}
		}

		public override void DeclareRuntimePlatforms(Settings settings)
		{
			settings.DeclareRuntimePlatform(RuntimePlatform.IPhonePlayer, this);
			settings.DeclareRuntimePlatform(RuntimePlatform.Android, this);
		}

		public override float Priority
		{
			get
			{
				return 1f;
			}
		}

		public override bool MatchesCurrentEnvironment
		{
			get
			{
				return base.Active;
			}
		}
	}
}
