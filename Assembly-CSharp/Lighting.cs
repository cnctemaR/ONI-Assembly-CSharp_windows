using System;
using UnityEngine;

[ExecuteInEditMode]
public class Lighting : MonoBehaviour
{
	private void Awake()
	{
		Lighting.Instance = this;
	}

	private void OnDestroy()
	{
		Lighting.Instance = null;
	}

	private Color PremultiplyAlpha(Color c)
	{
		return c * c.a;
	}

	private void Start()
	{
		this.UpdateLighting();
	}

	private void Update()
	{
		this.UpdateLighting();
	}

	private void UpdateLighting()
	{
		Shader.SetGlobalInt(Lighting._liquidZ, -28);
		Shader.SetGlobalInt(Lighting._SceneLayerMax, 34);
		Shader.SetGlobalVector(Lighting._DigMapMapParameters, new Vector4(this.Settings.DigMapColour.r, this.Settings.DigMapColour.g, this.Settings.DigMapColour.b, this.Settings.DigMapScale));
		Shader.SetGlobalTexture(Lighting._DigDamageMap, this.Settings.DigDamageMap);
		Shader.SetGlobalTexture(Lighting._StateTransitionMap, this.Settings.StateTransitionMap);
		Shader.SetGlobalColor(Lighting._StateTransitionColor, this.Settings.StateTransitionColor);
		Shader.SetGlobalVector(Lighting._StateTransitionParameters, new Vector4(1f / this.Settings.StateTransitionUVScale, this.Settings.StateTransitionUVOffsetRate.x, this.Settings.StateTransitionUVOffsetRate.y, 0f));
		Shader.SetGlobalTexture(Lighting._FallingSolidMap, this.Settings.FallingSolidMap);
		Shader.SetGlobalColor(Lighting._FallingSolidColor, this.Settings.FallingSolidColor);
		Shader.SetGlobalVector(Lighting._FallingSolidParameters, new Vector4(1f / this.Settings.FallingSolidUVScale, this.Settings.FallingSolidUVOffsetRate.x, this.Settings.FallingSolidUVOffsetRate.y, 0f));
		Shader.SetGlobalColor(Lighting._WaterTrimColor, this.Settings.WaterTrimColor);
		Shader.SetGlobalVector(Lighting._WaterParameters2, new Vector4(this.Settings.WaterTrimSize, this.Settings.WaterAlphaTrimSize, 0f, this.Settings.WaterAlphaThreshold));
		Shader.SetGlobalVector(Lighting._WaterWaveParameters, new Vector4(this.Settings.WaterWaveAmplitude, this.Settings.WaterWaveFrequency, this.Settings.WaterWaveSpeed, 0f));
		Shader.SetGlobalVector(Lighting._WaterWaveParameters2, new Vector4(this.Settings.WaterWaveAmplitude2, this.Settings.WaterWaveFrequency2, this.Settings.WaterWaveSpeed2, 0f));
		Shader.SetGlobalVector(Lighting._WaterDetailParameters, new Vector4(this.Settings.WaterCubeMapScale, this.Settings.WaterDetailTiling, this.Settings.WaterColorScale, this.Settings.WaterDetailTiling2));
		Shader.SetGlobalVector(Lighting._WaterDistortionParameters, new Vector4(this.Settings.WaterDistortionScaleStart, this.Settings.WaterDistortionScaleEnd, this.Settings.WaterDepthColorOpacityStart, this.Settings.WaterDepthColorOpacityEnd));
		Shader.SetGlobalVector(Lighting._BloomParameters, new Vector4(this.Settings.BloomScale, 0f, 0f, 0f));
		Shader.SetGlobalVector(Lighting._LiquidParameters2, new Vector4(this.Settings.LiquidMin, this.Settings.LiquidMax, this.Settings.LiquidCutoff, this.Settings.LiquidTransparency));
		Shader.SetGlobalVector(Lighting._GridParameters, new Vector4(this.Settings.GridLineWidth, this.Settings.GridSize, this.Settings.GridMinIntensity, this.Settings.GridMaxIntensity));
		Shader.SetGlobalColor(Lighting._GridColor, this.Settings.GridColor);
		Shader.SetGlobalVector(Lighting._EdgeGlowParameters, new Vector4(this.Settings.EdgeGlowCutoffStart, this.Settings.EdgeGlowCutoffEnd, this.Settings.EdgeGlowIntensity, 0f));
		if (this.disableLighting)
		{
			Shader.SetGlobalVector(Lighting._SubstanceParameters, Vector4.one);
			Shader.SetGlobalVector(Lighting._TileEdgeParameters, Vector4.one);
		}
		else
		{
			Shader.SetGlobalVector(Lighting._SubstanceParameters, new Vector4(this.Settings.substanceEdgeParameters.intensity, this.Settings.substanceEdgeParameters.edgeIntensity, this.Settings.substanceEdgeParameters.directSunlightScale, this.Settings.substanceEdgeParameters.power));
			Shader.SetGlobalVector(Lighting._TileEdgeParameters, new Vector4(this.Settings.tileEdgeParameters.intensity, this.Settings.tileEdgeParameters.edgeIntensity, this.Settings.tileEdgeParameters.directSunlightScale, this.Settings.tileEdgeParameters.power));
		}
		float num = ((SimDebugView.Instance != null && SimDebugView.Instance.GetMode() == OverlayModes.Disease.ID) ? 1f : 0f);
		if (this.disableLighting)
		{
			Shader.SetGlobalVector(Lighting._AnimParameters, new Vector4(1f, this.Settings.WorldZoneAnimBlend, 0f, num));
		}
		else
		{
			Shader.SetGlobalVector(Lighting._AnimParameters, new Vector4(this.Settings.AnimIntensity, this.Settings.WorldZoneAnimBlend, 0f, num));
		}
		Shader.SetGlobalVector(Lighting._GasOpacity, new Vector4(this.Settings.GasMinOpacity, this.Settings.GasMaxOpacity, 0f, 0f));
		Shader.SetGlobalColor(Lighting._DarkenTintBackground, this.Settings.DarkenTints[0]);
		Shader.SetGlobalColor(Lighting._DarkenTintMidground, this.Settings.DarkenTints[1]);
		Shader.SetGlobalColor(Lighting._DarkenTintForeground, this.Settings.DarkenTints[2]);
		Shader.SetGlobalColor(Lighting._BrightenOverlay, this.Settings.BrightenOverlayColour);
		Shader.SetGlobalColor(Lighting._ColdFG, this.PremultiplyAlpha(this.Settings.ColdColours[2]));
		Shader.SetGlobalColor(Lighting._ColdMG, this.PremultiplyAlpha(this.Settings.ColdColours[1]));
		Shader.SetGlobalColor(Lighting._ColdBG, this.PremultiplyAlpha(this.Settings.ColdColours[0]));
		Shader.SetGlobalColor(Lighting._HotFG, this.PremultiplyAlpha(this.Settings.HotColours[2]));
		Shader.SetGlobalColor(Lighting._HotMG, this.PremultiplyAlpha(this.Settings.HotColours[1]));
		Shader.SetGlobalColor(Lighting._HotBG, this.PremultiplyAlpha(this.Settings.HotColours[0]));
		Shader.SetGlobalVector(Lighting._TemperatureParallax, this.Settings.TemperatureParallax);
		Shader.SetGlobalVector(Lighting._ColdUVOffset1, new Vector4(this.Settings.ColdBGUVOffset.x, this.Settings.ColdBGUVOffset.y, this.Settings.ColdMGUVOffset.x, this.Settings.ColdMGUVOffset.y));
		Shader.SetGlobalVector(Lighting._ColdUVOffset2, new Vector4(this.Settings.ColdFGUVOffset.x, this.Settings.ColdFGUVOffset.y, 0f, 0f));
		Shader.SetGlobalVector(Lighting._HotUVOffset1, new Vector4(this.Settings.HotBGUVOffset.x, this.Settings.HotBGUVOffset.y, this.Settings.HotMGUVOffset.x, this.Settings.HotMGUVOffset.y));
		Shader.SetGlobalVector(Lighting._HotUVOffset2, new Vector4(this.Settings.HotFGUVOffset.x, this.Settings.HotFGUVOffset.y, 0f, 0f));
		Shader.SetGlobalColor(Lighting._DustColour, this.PremultiplyAlpha(this.Settings.DustColour));
		Shader.SetGlobalVector(Lighting._DustInfo, new Vector4(this.Settings.DustScale, this.Settings.DustMovement.x, this.Settings.DustMovement.y, this.Settings.DustMovement.z));
		Shader.SetGlobalTexture(Lighting._DustTex, this.Settings.DustTex);
		Shader.SetGlobalVector(Lighting._DebugShowInfo, new Vector4(this.Settings.ShowDust, this.Settings.ShowGas, this.Settings.ShowShadow, this.Settings.ShowTemperature));
		Shader.SetGlobalVector(Lighting._HeatHazeParameters, this.Settings.HeatHazeParameters);
		Shader.SetGlobalTexture(Lighting._HeatHazeTexture, this.Settings.HeatHazeTexture);
		Shader.SetGlobalVector(Lighting._ShineParams, new Vector4(this.Settings.ShineCenter.x, this.Settings.ShineCenter.y, this.Settings.ShineRange.x, this.Settings.ShineRange.y));
		Shader.SetGlobalVector(Lighting._ShineParams2, new Vector4(this.Settings.ShineZoomSpeed, 0f, 0f, 0f));
		Shader.SetGlobalFloat(Lighting._WorldZoneGasBlend, this.Settings.WorldZoneGasBlend);
		Shader.SetGlobalFloat(Lighting._WorldZoneLiquidBlend, this.Settings.WorldZoneLiquidBlend);
		Shader.SetGlobalFloat(Lighting._WorldZoneForegroundBlend, this.Settings.WorldZoneForegroundBlend);
		Shader.SetGlobalFloat(Lighting._WorldZoneSimpleAnimBlend, this.Settings.WorldZoneSimpleAnimBlend);
		Shader.SetGlobalColor(Lighting._CharacterLitColour, this.Settings.characterLighting.litColour);
		Shader.SetGlobalColor(Lighting._CharacterUnlitColour, this.Settings.characterLighting.unlitColour);
		Shader.SetGlobalTexture(Lighting._BuildingDamagedTex, this.Settings.BuildingDamagedTex);
		Shader.SetGlobalVector(Lighting._BuildingDamagedUVParameters, this.Settings.BuildingDamagedUVParameters);
		Shader.SetGlobalTexture(Lighting._DiseaseOverlayTex, this.Settings.DiseaseOverlayTex);
		Shader.SetGlobalVector(Lighting._DiseaseOverlayTexInfo, this.Settings.DiseaseOverlayTexInfo);
		if (this.Settings.ShowRadiation)
		{
			Shader.SetGlobalColor(Lighting._RadHazeColor, this.PremultiplyAlpha(this.Settings.RadColor));
		}
		else
		{
			Shader.SetGlobalColor(Lighting._RadHazeColor, Color.clear);
		}
		Shader.SetGlobalVector(Lighting._RadUVOffset1, new Vector4(this.Settings.Rad1UVOffset.x, this.Settings.Rad1UVOffset.y, this.Settings.Rad2UVOffset.x, this.Settings.Rad2UVOffset.y));
		Shader.SetGlobalVector(Lighting._RadUVOffset2, new Vector4(this.Settings.Rad3UVOffset.x, this.Settings.Rad3UVOffset.y, this.Settings.Rad4UVOffset.x, this.Settings.Rad4UVOffset.y));
		Shader.SetGlobalVector(Lighting._RadUVScales, new Vector4(1f / this.Settings.RadUVScales.x, 1f / this.Settings.RadUVScales.y, 1f / this.Settings.RadUVScales.z, 1f / this.Settings.RadUVScales.w));
		Shader.SetGlobalVector(Lighting._RadRange1, new Vector4(this.Settings.Rad1Range.x, this.Settings.Rad1Range.y, this.Settings.Rad2Range.x, this.Settings.Rad2Range.y));
		Shader.SetGlobalVector(Lighting._RadRange2, new Vector4(this.Settings.Rad3Range.x, this.Settings.Rad3Range.y, this.Settings.Rad4Range.x, this.Settings.Rad4Range.y));
		if (LightBuffer.Instance != null && LightBuffer.Instance.Texture != null)
		{
			Shader.SetGlobalTexture(Lighting._LightBufferTex, LightBuffer.Instance.Texture);
		}
	}

	public global::LightingSettings Settings;

	public static Lighting Instance;

	[NonSerialized]
	public bool disableLighting;

	private static int _liquidZ = Shader.PropertyToID("_LiquidZ");

	private static int _SceneLayerMax = Shader.PropertyToID("_SceneLayerMax");

	private static int _DigMapMapParameters = Shader.PropertyToID("_DigMapMapParameters");

	private static int _DigDamageMap = Shader.PropertyToID("_DigDamageMap");

	private static int _StateTransitionMap = Shader.PropertyToID("_StateTransitionMap");

	private static int _StateTransitionColor = Shader.PropertyToID("_StateTransitionColor");

	private static int _StateTransitionParameters = Shader.PropertyToID("_StateTransitionParameters");

	private static int _FallingSolidMap = Shader.PropertyToID("_FallingSolidMap");

	private static int _FallingSolidColor = Shader.PropertyToID("_FallingSolidColor");

	private static int _FallingSolidParameters = Shader.PropertyToID("_FallingSolidParameters");

	private static int _WaterTrimColor = Shader.PropertyToID("_WaterTrimColor");

	private static int _WaterParameters2 = Shader.PropertyToID("_WaterParameters2");

	private static int _WaterWaveParameters = Shader.PropertyToID("_WaterWaveParameters");

	private static int _WaterWaveParameters2 = Shader.PropertyToID("_WaterWaveParameters2");

	private static int _WaterDetailParameters = Shader.PropertyToID("_WaterDetailParameters");

	private static int _WaterDistortionParameters = Shader.PropertyToID("_WaterDistortionParameters");

	private static int _BloomParameters = Shader.PropertyToID("_BloomParameters");

	private static int _LiquidParameters2 = Shader.PropertyToID("_LiquidParameters2");

	private static int _GridParameters = Shader.PropertyToID("_GridParameters");

	private static int _GridColor = Shader.PropertyToID("_GridColor");

	private static int _EdgeGlowParameters = Shader.PropertyToID("_EdgeGlowParameters");

	private static int _SubstanceParameters = Shader.PropertyToID("_SubstanceParameters");

	private static int _TileEdgeParameters = Shader.PropertyToID("_TileEdgeParameters");

	private static int _AnimParameters = Shader.PropertyToID("_AnimParameters");

	private static int _GasOpacity = Shader.PropertyToID("_GasOpacity");

	private static int _DarkenTintBackground = Shader.PropertyToID("_DarkenTintBackground");

	private static int _DarkenTintMidground = Shader.PropertyToID("_DarkenTintMidground");

	private static int _DarkenTintForeground = Shader.PropertyToID("_DarkenTintForeground");

	private static int _BrightenOverlay = Shader.PropertyToID("_BrightenOverlay");

	private static int _ColdFG = Shader.PropertyToID("_ColdFG");

	private static int _ColdMG = Shader.PropertyToID("_ColdMG");

	private static int _ColdBG = Shader.PropertyToID("_ColdBG");

	private static int _HotFG = Shader.PropertyToID("_HotFG");

	private static int _HotMG = Shader.PropertyToID("_HotMG");

	private static int _HotBG = Shader.PropertyToID("_HotBG");

	private static int _TemperatureParallax = Shader.PropertyToID("_TemperatureParallax");

	private static int _ColdUVOffset1 = Shader.PropertyToID("_ColdUVOffset1");

	private static int _ColdUVOffset2 = Shader.PropertyToID("_ColdUVOffset2");

	private static int _HotUVOffset1 = Shader.PropertyToID("_HotUVOffset1");

	private static int _HotUVOffset2 = Shader.PropertyToID("_HotUVOffset2");

	private static int _DustColour = Shader.PropertyToID("_DustColour");

	private static int _DustInfo = Shader.PropertyToID("_DustInfo");

	private static int _DustTex = Shader.PropertyToID("_DustTex");

	private static int _DebugShowInfo = Shader.PropertyToID("_DebugShowInfo");

	private static int _HeatHazeParameters = Shader.PropertyToID("_HeatHazeParameters");

	private static int _HeatHazeTexture = Shader.PropertyToID("_HeatHazeTexture");

	private static int _ShineParams = Shader.PropertyToID("_ShineParams");

	private static int _ShineParams2 = Shader.PropertyToID("_ShineParams2");

	private static int _WorldZoneGasBlend = Shader.PropertyToID("_WorldZoneGasBlend");

	private static int _WorldZoneLiquidBlend = Shader.PropertyToID("_WorldZoneLiquidBlend");

	private static int _WorldZoneForegroundBlend = Shader.PropertyToID("_WorldZoneForegroundBlend");

	private static int _WorldZoneSimpleAnimBlend = Shader.PropertyToID("_WorldZoneSimpleAnimBlend");

	private static int _CharacterLitColour = Shader.PropertyToID("_CharacterLitColour");

	private static int _CharacterUnlitColour = Shader.PropertyToID("_CharacterUnlitColour");

	private static int _BuildingDamagedTex = Shader.PropertyToID("_BuildingDamagedTex");

	private static int _BuildingDamagedUVParameters = Shader.PropertyToID("_BuildingDamagedUVParameters");

	private static int _DiseaseOverlayTex = Shader.PropertyToID("_DiseaseOverlayTex");

	private static int _DiseaseOverlayTexInfo = Shader.PropertyToID("_DiseaseOverlayTexInfo");

	private static int _RadHazeColor = Shader.PropertyToID("_RadHazeColor");

	private static int _RadUVOffset1 = Shader.PropertyToID("_RadUVOffset1");

	private static int _RadUVOffset2 = Shader.PropertyToID("_RadUVOffset2");

	private static int _RadUVScales = Shader.PropertyToID("_RadUVScales");

	private static int _RadRange1 = Shader.PropertyToID("_RadRange1");

	private static int _RadRange2 = Shader.PropertyToID("_RadRange2");

	private static int _LightBufferTex = Shader.PropertyToID("_LightBufferTex");
}
