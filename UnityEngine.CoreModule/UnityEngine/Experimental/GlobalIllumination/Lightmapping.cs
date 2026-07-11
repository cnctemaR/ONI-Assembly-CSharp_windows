using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.GlobalIllumination
{
	public static class Lightmapping
	{
		public static void SetDelegate(Lightmapping.RequestLightsDelegate del)
		{
			Lightmapping.s_RequestLightsDelegate = ((del == null) ? Lightmapping.s_DefaultDelegate : del);
		}

		public static Lightmapping.RequestLightsDelegate GetDelegate()
		{
			return Lightmapping.s_RequestLightsDelegate;
		}

		public static void ResetDelegate()
		{
			Lightmapping.s_RequestLightsDelegate = Lightmapping.s_DefaultDelegate;
		}

		[UsedByNativeCode]
		internal unsafe static void RequestLights(Light[] lights, IntPtr outLightsPtr, int outLightsCount)
		{
			NativeArray<LightDataGI> nativeArray = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<LightDataGI>((void*)outLightsPtr, outLightsCount, Allocator.None);
			Lightmapping.s_RequestLightsDelegate(lights, nativeArray);
		}

		private static readonly Lightmapping.RequestLightsDelegate s_DefaultDelegate = delegate(Light[] requests, NativeArray<LightDataGI> lightsOutput)
		{
			DirectionalLight directionalLight = default(DirectionalLight);
			PointLight pointLight = default(PointLight);
			SpotLight spotLight = default(SpotLight);
			RectangleLight rectangleLight = default(RectangleLight);
			DiscLight discLight = default(DiscLight);
			LightDataGI lightDataGI = default(LightDataGI);
			for (int i = 0; i < requests.Length; i++)
			{
				Light light = requests[i];
				switch (light.type)
				{
				case LightType.Spot:
					LightmapperUtils.Extract(light, ref spotLight);
					lightDataGI.Init(ref spotLight);
					break;
				case LightType.Directional:
					LightmapperUtils.Extract(light, ref directionalLight);
					lightDataGI.Init(ref directionalLight);
					break;
				case LightType.Point:
					LightmapperUtils.Extract(light, ref pointLight);
					lightDataGI.Init(ref pointLight);
					break;
				case LightType.Area:
					LightmapperUtils.Extract(light, ref rectangleLight);
					lightDataGI.Init(ref rectangleLight);
					break;
				case LightType.Disc:
					LightmapperUtils.Extract(light, ref discLight);
					lightDataGI.Init(ref discLight);
					break;
				default:
					lightDataGI.InitNoBake(light.GetInstanceID());
					break;
				}
				lightsOutput[i] = lightDataGI;
			}
		};

		private static Lightmapping.RequestLightsDelegate s_RequestLightsDelegate = Lightmapping.s_DefaultDelegate;

		public delegate void RequestLightsDelegate(Light[] requests, NativeArray<LightDataGI> lightsOutput);
	}
}
