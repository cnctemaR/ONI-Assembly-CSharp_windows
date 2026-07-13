using System;
using UnityEngine.Rendering;

namespace UnityEngine.Experimental.GlobalIllumination
{
	public static class LightmapperUtils
	{
		public static LightMode Extract(LightmapBakeType baketype)
		{
			return (baketype == LightmapBakeType.Realtime) ? LightMode.Realtime : ((baketype == LightmapBakeType.Mixed) ? LightMode.Mixed : LightMode.Baked);
		}

		public static LinearColor ExtractIndirect(Light l)
		{
			return LinearColor.Convert(l.color, l.intensity * l.bounceIntensity);
		}

		public static float ExtractInnerCone(Light l)
		{
			return 2f * Mathf.Atan(Mathf.Tan(l.spotAngle * 0.5f * 0.017453292f) * 46f / 64f);
		}

		private static Color ExtractColorTemperature(Light l)
		{
			Color color = new Color(1f, 1f, 1f);
			bool flag = l.useColorTemperature && GraphicsSettings.lightsUseLinearIntensity;
			if (flag)
			{
				color = Mathf.CorrelatedColorTemperatureToRGB(l.colorTemperature);
			}
			return color;
		}

		private static void ApplyColorTemperature(Color cct, ref LinearColor lightColor)
		{
			lightColor.red *= cct.r;
			lightColor.green *= cct.g;
			lightColor.blue *= cct.b;
		}

		public static void Extract(Light l, ref DirectionalLight dir)
		{
			dir.entityId = l.GetEntityId();
			dir.mode = LightmapperUtils.Extract(l.bakingOutput.lightmapBakeType);
			dir.shadow = l.shadows > LightShadows.None;
			dir.position = l.transform.position;
			dir.orientation = l.transform.rotation;
			Color color = LightmapperUtils.ExtractColorTemperature(l);
			LinearColor linearColor = LinearColor.Convert(l.color, l.intensity);
			LinearColor linearColor2 = LightmapperUtils.ExtractIndirect(l);
			LightmapperUtils.ApplyColorTemperature(color, ref linearColor);
			LightmapperUtils.ApplyColorTemperature(color, ref linearColor2);
			dir.color = linearColor;
			dir.indirectColor = linearColor2;
			dir.penumbraWidthRadian = 0f;
		}

		public static void Extract(Light l, ref PointLight point)
		{
			point.entityId = l.GetEntityId();
			point.mode = LightmapperUtils.Extract(l.bakingOutput.lightmapBakeType);
			point.shadow = l.shadows > LightShadows.None;
			point.position = l.transform.position;
			point.orientation = l.transform.rotation;
			Color color = LightmapperUtils.ExtractColorTemperature(l);
			LinearColor linearColor = LinearColor.Convert(l.color, l.intensity);
			LinearColor linearColor2 = LightmapperUtils.ExtractIndirect(l);
			LightmapperUtils.ApplyColorTemperature(color, ref linearColor);
			LightmapperUtils.ApplyColorTemperature(color, ref linearColor2);
			point.color = linearColor;
			point.indirectColor = linearColor2;
			point.range = l.range;
			point.sphereRadius = 0f;
			point.falloff = FalloffType.Legacy;
		}

		public static void Extract(Light l, ref SpotLight spot)
		{
			spot.entityId = l.GetEntityId();
			spot.mode = LightmapperUtils.Extract(l.bakingOutput.lightmapBakeType);
			spot.shadow = l.shadows > LightShadows.None;
			spot.position = l.transform.position;
			spot.orientation = l.transform.rotation;
			Color color = LightmapperUtils.ExtractColorTemperature(l);
			LinearColor linearColor = LinearColor.Convert(l.color, l.intensity);
			LinearColor linearColor2 = LightmapperUtils.ExtractIndirect(l);
			LightmapperUtils.ApplyColorTemperature(color, ref linearColor);
			LightmapperUtils.ApplyColorTemperature(color, ref linearColor2);
			spot.color = linearColor;
			spot.indirectColor = linearColor2;
			spot.range = l.range;
			spot.sphereRadius = 0f;
			spot.coneAngle = l.spotAngle * 0.017453292f;
			spot.innerConeAngle = LightmapperUtils.ExtractInnerCone(l);
			spot.falloff = FalloffType.Legacy;
			spot.angularFalloff = AngularFalloffType.LUT;
		}

		public static void Extract(Light l, ref RectangleLight rect)
		{
			rect.entityId = l.GetEntityId();
			rect.mode = LightmapperUtils.Extract(l.bakingOutput.lightmapBakeType);
			rect.shadow = l.shadows > LightShadows.None;
			rect.position = l.transform.position;
			rect.orientation = l.transform.rotation;
			Color color = LightmapperUtils.ExtractColorTemperature(l);
			LinearColor linearColor = LinearColor.Convert(l.color, l.intensity);
			LinearColor linearColor2 = LightmapperUtils.ExtractIndirect(l);
			LightmapperUtils.ApplyColorTemperature(color, ref linearColor);
			LightmapperUtils.ApplyColorTemperature(color, ref linearColor2);
			rect.color = linearColor;
			rect.indirectColor = linearColor2;
			rect.range = l.dilatedRange;
			rect.width = 0f;
			rect.height = 0f;
			rect.falloff = FalloffType.Legacy;
		}

		public static void Extract(Light l, ref DiscLight disc)
		{
			disc.entityId = l.GetEntityId();
			disc.mode = LightmapperUtils.Extract(l.bakingOutput.lightmapBakeType);
			disc.shadow = l.shadows > LightShadows.None;
			disc.position = l.transform.position;
			disc.orientation = l.transform.rotation;
			Color color = LightmapperUtils.ExtractColorTemperature(l);
			LinearColor linearColor = LinearColor.Convert(l.color, l.intensity);
			LinearColor linearColor2 = LightmapperUtils.ExtractIndirect(l);
			LightmapperUtils.ApplyColorTemperature(color, ref linearColor);
			LightmapperUtils.ApplyColorTemperature(color, ref linearColor2);
			disc.color = linearColor;
			disc.indirectColor = linearColor2;
			disc.range = l.dilatedRange;
			disc.radius = 0f;
			disc.falloff = FalloffType.Legacy;
		}

		public static void Extract(Light l, out Cookie cookie)
		{
			cookie.entityId = (l.cookie ? l.cookie.GetInstanceID() : EntityId.None);
			cookie.scale = 1f;
			cookie.sizes = ((l.type == LightType.Directional && l.cookie) ? l.cookieSize2D : new Vector2(1f, 1f));
		}
	}
}
