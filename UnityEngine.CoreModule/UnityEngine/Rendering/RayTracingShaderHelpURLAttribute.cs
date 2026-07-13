using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Rendering
{
	[MovedFrom("UnityEngine.Experimental.Rendering")]
	[NativeHeader("Runtime/Shaders/RayTracing/RayTracingShader.h")]
	[NativeHeader("Runtime/Graphics/RayTracing/RayTracingAccelerationStructure.h")]
	[NativeHeader("Runtime/Graphics/ShaderScriptBindings.h")]
	internal class RayTracingShaderHelpURLAttribute : HelpURLAttribute
	{
		public RayTracingShaderHelpURLAttribute()
			: base(null)
		{
		}

		public override string URL
		{
			get
			{
				return string.Format("https://docs.unity3d.com//{0}.{1}/Documentation/ScriptReference/Rendering.RayTracingShader.html", Application.unityVersionVer, Application.unityVersionMaj);
			}
		}
	}
}
