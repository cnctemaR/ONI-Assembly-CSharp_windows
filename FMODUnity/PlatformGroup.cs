using System;
using UnityEngine;

namespace FMODUnity
{
	public class PlatformGroup : Platform
	{
		internal override string DisplayName
		{
			get
			{
				return this.displayName;
			}
		}

		internal override void DeclareRuntimePlatforms(Settings settings)
		{
		}

		[SerializeField]
		private string displayName;

		[SerializeField]
		private Legacy.Platform legacyIdentifier;
	}
}
