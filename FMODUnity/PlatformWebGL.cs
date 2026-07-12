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

		internal override string DisplayName
		{
			get
			{
				return "WebGL";
			}
		}

		internal override void DeclareRuntimePlatforms(Settings settings)
		{
			settings.DeclareRuntimePlatform(RuntimePlatform.WebGLPlayer, this);
		}

		internal override string GetPluginPath(string pluginName)
		{
			return string.Format("{0}/{1}.a", this.GetPluginBasePath(), pluginName);
		}
	}
}
