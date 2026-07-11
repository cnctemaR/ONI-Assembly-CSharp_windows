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

		public override void DeclareUnityMappings(Settings settings)
		{
		}

		[SerializeField]
		public string displayName;

		[SerializeField]
		private Legacy.Platform legacyIdentifier;
	}
}
