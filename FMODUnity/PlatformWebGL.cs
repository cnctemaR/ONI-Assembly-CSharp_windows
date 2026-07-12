using System;
using UnityEngine;

namespace FMODUnity
{
	public class PlatformWebGL : Platform
	{
		static PlatformWebGL()
		{
			Settings.AddPlatformTemplate<PlatformWebGL>("46fbfdf3fc43db0458918377fd40293e");
		}

		public override string DisplayName
		{
			get
			{
				return "WebGL";
			}
		}

		public override void DeclareRuntimePlatforms(Settings settings)
		{
			settings.DeclareRuntimePlatform(RuntimePlatform.WebGLPlayer, this);
		}

		public override string GetPluginPath(string pluginName)
		{
			return string.Format("{0}/{1}.bc", this.GetPluginBasePath(), pluginName);
		}
	}
}
