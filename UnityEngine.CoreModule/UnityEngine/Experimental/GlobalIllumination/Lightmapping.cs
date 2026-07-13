using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.GlobalIllumination
{
	public static class Lightmapping
	{
		[RequiredByNativeCode]
		public static void SetDelegate(Lightmapping.RequestLightsDelegate del)
		{
			Lightmapping.s_RequestLightsDelegate = ((del != null) ? del : Lightmapping.s_DefaultDelegate);
		}

		[RequiredByNativeCode]
		public static Lightmapping.RequestLightsDelegate GetDelegate()
		{
			return Lightmapping.s_RequestLightsDelegate;
		}

		[RequiredByNativeCode]
		public static void ResetDelegate()
		{
			Lightmapping.s_RequestLightsDelegate = Lightmapping.s_DefaultDelegate;
		}

		[RequiredByNativeCode]
		internal unsafe static void RequestLights(Light[] lights, IntPtr outLightsPtr, int outLightsCount)
		{
			NativeArray<LightDataGI> nativeArray = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<LightDataGI>((void*)outLightsPtr, outLightsCount, Allocator.None);
			Lightmapping.s_RequestLightsDelegate(lights, nativeArray);
		}

		[RequiredByNativeCode]
		private static readonly Lightmapping.RequestLightsDelegate s_DefaultDelegate = delegate(Light[] requests, NativeArray<LightDataGI> lightsOutput)
		{
			DirectionalLight directionalLight = default(DirectionalLight);
			PointLight pointLight = default(PointLight);
			SpotLight spotLight = default(SpotLight);
			RectangleLight rectangleLight = default(RectangleLight);
			DiscLight discLight = default(DiscLight);
			Cookie cookie = default(Cookie);
			LightDataGI lightDataGI = default(LightDataGI);
			for (int i = 0; i < requests.Length; i++)
			{
				Light light = requests[i];
				switch (light.type)
				{
				case LightType.Spot:
					LightmapperUtils.Extract(light, ref spotLight);
					LightmapperUtils.Extract(light, out cookie);
					lightDataGI.Init(ref spotLight, ref cookie);
					break;
				case LightType.Directional:
					LightmapperUtils.Extract(light, ref directionalLight);
					LightmapperUtils.Extract(light, out cookie);
					lightDataGI.Init(ref directionalLight, ref cookie);
					break;
				case LightType.Point:
					LightmapperUtils.Extract(light, ref pointLight);
					LightmapperUtils.Extract(light, out cookie);
					lightDataGI.Init(ref pointLight, ref cookie);
					break;
				case LightType.Area:
					LightmapperUtils.Extract(light, ref rectangleLight);
					LightmapperUtils.Extract(light, out cookie);
					lightDataGI.Init(ref rectangleLight, ref cookie);
					break;
				case LightType.Disc:
					LightmapperUtils.Extract(light, ref discLight);
					LightmapperUtils.Extract(light, out cookie);
					lightDataGI.Init(ref discLight, ref cookie);
					break;
				default:
					lightDataGI.InitNoBake(light.GetEntityId());
					break;
				}
				lightsOutput[i] = lightDataGI;
			}
		};

		[RequiredByNativeCode]
		private static Lightmapping.RequestLightsDelegate s_RequestLightsDelegate = Lightmapping.s_DefaultDelegate;

		public delegate void RequestLightsDelegate(Light[] requests, NativeArray<LightDataGI> lightsOutput);
	}
}
