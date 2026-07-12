using System;
using UnityEngine;

namespace FMODUnity
{
	public class PlatformGroup : Platform
	{
		public override string DisplayName
		{
			get
			{
				return this.displayName;
			}
		}

		public override void DeclareRuntimePlatforms(Settings settings)
		{
		}

		[SerializeField]
		private string displayName;

		[SerializeField]
		private Legacy.Platform legacyIdentifier;
	}
}
