using System;
using FMODUnity;
using UnityEngine;

public class GlobalResources : ScriptableObject
{
	public static GlobalResources Instance()
	{
		if (GlobalResources._Instance == null)
		{
			GlobalResources._Instance = Resources.Load<GlobalResources>("GlobalResources");
		}
		return GlobalResources._Instance;
	}

	public Material AnimMaterial;

	public Material AnimUIMaterial;

	public Material AnimPlaceMaterial;

	public Material AnimMaterialUIDesaturated;

	public Material AnimSimpleMaterial;

	public Material AnimOverlayMaterial;

	public Texture2D WhiteTexture;

	public EventReference ConduitOverlaySoundLiquid;

	public EventReference ConduitOverlaySoundGas;

	public EventReference ConduitOverlaySoundSolid;

	public EventReference AcousticDisturbanceSound;

	public EventReference AcousticDisturbanceBubbleSound;

	public EventReference WallDamageLayerSound;

	public Sprite sadDupeAudio;

	public Sprite sadDupe;

	public Sprite baseGameLogoSmall;

	public Sprite expansion1LogoSmall;

	private static GlobalResources _Instance;
}
