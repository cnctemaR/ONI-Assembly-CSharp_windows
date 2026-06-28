using System;
using System.Collections.Generic;
using UnityEngine;

public class LightingSettings : ScriptableObject
{
	public float DigMapScale;

	public Color DigMapColour;

	public Texture2D DigDamageMap;

	public Texture2D StateTransitionMap;

	public Color StateTransitionColor;

	public float StateTransitionUVScale;

	public Vector2 StateTransitionUVOffsetRate;

	public Vector2 ShineCenter;

	public Vector2 ShineRange;

	public float ShineZoomSpeed;

	public List<LightSettings> LightSettings;

	public bool UpdateLightSettings;

	public Color WaterTrimColor;

	public float WaterTrimSize;

	public float WaterAlphaTrimSize;

	public float WaterAlphaThreshold;

	public float WaterCubesAlphaThreshold;

	public float WaterWaveAmplitude;

	public float WaterWaveFrequency;

	public float WaterWaveSpeed;

	public float WaterDetailSpeed;

	public float WaterDetailTiling;

	public float WaterDetailTiling2;

	public Vector2 WaterDetailDirection;

	public float WaterWaveAmplitude2;

	public float WaterWaveFrequency2;

	public float WaterWaveSpeed2;

	public Texture WaterCubeMap;

	public float WaterCubeMapScale;

	public float WaterColorScale;

	public float WaterDistortionScaleStart;

	public float WaterDistortionScaleEnd;

	public float WaterDepthColorOpacityStart;

	public float WaterDepthColorOpacityEnd;

	public float BloomScale;

	public float LiquidMin;

	public float LiquidMax;

	public float LiquidCutoff;

	public float LiquidTransparency;

	public float LiquidAmountOffset;

	public float LiquidMaxMass;

	public float GridLineWidth;

	public float GridSize;

	public float GridMaxIntensity;

	public float GridMinIntensity;

	public Color GridColor;

	public float EdgeGlowCutoffStart;

	public float EdgeGlowCutoffEnd;

	public float EdgeGlowIntensity;

	public int BackgroundLayers;

	public float BackgroundBaseParallax;

	public float BackgroundLayerParallax;

	public float BackgroundDarkening;

	public float BackgroundClip;

	public float BackgroundUVScale;

	public LightingSettings.EdgeLighting substanceEdgeParameters;

	public LightingSettings.EdgeLighting tileEdgeParameters;

	public float AnimIntensity;

	public float GasMinOpacity;

	public float GasMaxOpacity;

	public Color DetectorMaskColor;

	public float DetectorEdge;

	public Color[] DarkenTints;

	public LightingSettings.LightingColours characterLighting;

	public Color BrightenOverlayColour;

	public Color[] ColdColours;

	public Color[] HotColours;

	public Vector4 TemperatureParallax;

	public Texture2D EmberTex;

	public Texture2D FrostTex;

	public Texture2D Thermal1Tex;

	public Texture2D Thermal2Tex;

	public Vector2 ColdFGUVOffset;

	public Vector2 ColdMGUVOffset;

	public Vector2 ColdBGUVOffset;

	public Vector2 HotFGUVOffset;

	public Vector2 HotMGUVOffset;

	public Vector2 HotBGUVOffset;

	public Texture2D DustTex;

	public Color DustColour;

	public float DustScale;

	public Vector3 DustMovement;

	public float ShowGas;

	public float ShowTemperature;

	public float ShowDust;

	public float ShowShadow;

	public Vector4 HeatHazeParameters;

	public Texture2D HeatHazeTexture;

	public float WorldZoneGasBlend;

	public float WorldZoneLiquidBlend;

	public float WorldZoneForegroundBlend;

	public float WorldZoneSimpleAnimBlend;

	public float WorldZoneAnimBlend;

	[Serializable]
	public struct EdgeLighting
	{
		public float intensity;

		public float edgeIntensity;

		public float diffuseIntensity;

		public float power;
	}

	public enum TintLayers
	{
		Background,
		Midground,
		Foreground,
		NumLayers
	}

	[Serializable]
	public struct LightingColours
	{
		public Color32 litColour;

		public Color32 unlitColour;
	}
}
