using System;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/OverlayScreen")]
public class OverlayScreen : KMonoBehaviour
{
	public HashedString mode
	{
		get
		{
			return this.currentModeInfo.mode.ViewMode();
		}
	}

	protected override void OnPrefabInit()
	{
		global::Debug.Assert(OverlayScreen.Instance == null);
		OverlayScreen.Instance = this;
		this.powerLabelParent = GameObject.Find("WorldSpaceCanvas").GetComponent<Canvas>();
	}

	protected override void OnLoadLevel()
	{
		this.harvestableNotificationPrefab = null;
		this.powerLabelParent = null;
		OverlayScreen.Instance = null;
		OverlayModes.Mode.Clear();
		this.modeInfos = null;
		this.currentModeInfo = default(OverlayScreen.ModeInfo);
		base.OnLoadLevel();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.techViewSound = KFMOD.CreateInstance(this.techViewSoundPath);
		this.techViewSoundPlaying = false;
		Shader.SetGlobalVector("_OverlayParams", Vector4.zero);
		this.RegisterModes();
		this.currentModeInfo = this.modeInfos[OverlayModes.None.ID];
	}

	private void RegisterModes()
	{
		this.modeInfos.Clear();
		OverlayModes.None none = new OverlayModes.None();
		this.RegisterMode(none);
		this.RegisterMode(new OverlayModes.Oxygen());
		this.RegisterMode(new OverlayModes.Power(this.powerLabelParent, this.powerLabelPrefab, this.batUIPrefab, this.powerLabelOffset, this.batteryUIOffset, this.batteryUITransformerOffset, this.batteryUISmallTransformerOffset));
		this.RegisterMode(new OverlayModes.Temperature());
		this.RegisterMode(new OverlayModes.ThermalConductivity());
		this.RegisterMode(new OverlayModes.Light());
		this.RegisterMode(new OverlayModes.LiquidConduits());
		this.RegisterMode(new OverlayModes.GasConduits());
		this.RegisterMode(new OverlayModes.Decor());
		this.RegisterMode(new OverlayModes.Disease(this.powerLabelParent, this.diseaseOverlayPrefab));
		this.RegisterMode(new OverlayModes.Crop(this.powerLabelParent, this.harvestableNotificationPrefab));
		this.RegisterMode(new OverlayModes.Harvest());
		this.RegisterMode(new OverlayModes.Priorities());
		this.RegisterMode(new OverlayModes.HeatFlow());
		this.RegisterMode(new OverlayModes.Rooms());
		this.RegisterMode(new OverlayModes.Suit(this.powerLabelParent, this.suitOverlayPrefab));
		this.RegisterMode(new OverlayModes.Logic(this.logicModeUIPrefab));
		this.RegisterMode(new OverlayModes.SolidConveyor());
		this.RegisterMode(new OverlayModes.TileMode());
		this.RegisterMode(new OverlayModes.Radiation());
	}

	private void RegisterMode(OverlayModes.Mode mode)
	{
		this.modeInfos[mode.ViewMode()] = new OverlayScreen.ModeInfo
		{
			mode = mode
		};
	}

	private void LateUpdate()
	{
		this.currentModeInfo.mode.Update();
	}

	public void RunPostProcessEffects(RenderTexture src, RenderTexture dest)
	{
		this.currentModeInfo.mode.OnRenderImage(src, dest);
	}

	public void ToggleOverlay(HashedString newMode, bool allowSound = true)
	{
		bool flag = allowSound && !(this.currentModeInfo.mode.ViewMode() == newMode);
		if (newMode != OverlayModes.None.ID)
		{
			ManagementMenu.Instance.CloseAll();
		}
		this.currentModeInfo.mode.Disable();
		if (newMode != this.currentModeInfo.mode.ViewMode() && newMode == OverlayModes.None.ID)
		{
			ManagementMenu.Instance.CloseAll();
		}
		SimDebugView.Instance.SetMode(newMode);
		if (!this.modeInfos.TryGetValue(newMode, out this.currentModeInfo))
		{
			this.currentModeInfo = this.modeInfos[OverlayModes.None.ID];
		}
		this.currentModeInfo.mode.Enable();
		if (flag)
		{
			this.UpdateOverlaySounds();
		}
		if (OverlayModes.None.ID == this.currentModeInfo.mode.ViewMode())
		{
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().TechFilterOnMigrated, FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			MusicManager.instance.SetDynamicMusicOverlayInactive();
			this.techViewSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			this.techViewSoundPlaying = false;
		}
		else if (!this.techViewSoundPlaying)
		{
			AudioMixer.instance.Start(AudioMixerSnapshots.Get().TechFilterOnMigrated);
			MusicManager.instance.SetDynamicMusicOverlayActive();
			this.techViewSound.start();
			this.techViewSoundPlaying = true;
		}
		if (this.OnOverlayChanged != null)
		{
			this.OnOverlayChanged(this.currentModeInfo.mode.ViewMode());
		}
		this.ActivateLegend();
	}

	private void ActivateLegend()
	{
		if (OverlayLegend.Instance == null)
		{
			return;
		}
		OverlayLegend.Instance.SetLegend(this.currentModeInfo.mode, false);
	}

	public void Refresh()
	{
		this.LateUpdate();
	}

	public HashedString GetMode()
	{
		if (this.currentModeInfo.mode == null)
		{
			return OverlayModes.None.ID;
		}
		return this.currentModeInfo.mode.ViewMode();
	}

	private void UpdateOverlaySounds()
	{
		string text = this.currentModeInfo.mode.GetSoundName();
		if (text != "")
		{
			text = GlobalAssets.GetSound(text, false);
			KMonoBehaviour.PlaySound(text);
		}
	}

	public static HashSet<Tag> WireIDs = new HashSet<Tag>();

	public static HashSet<Tag> GasVentIDs = new HashSet<Tag>();

	public static HashSet<Tag> LiquidVentIDs = new HashSet<Tag>();

	public static HashSet<Tag> HarvestableIDs = new HashSet<Tag>();

	public static HashSet<Tag> DiseaseIDs = new HashSet<Tag>();

	public static HashSet<Tag> SuitIDs = new HashSet<Tag>();

	public static HashSet<Tag> SolidConveyorIDs = new HashSet<Tag>();

	public static HashSet<Tag> RadiationIDs = new HashSet<Tag>();

	[SerializeField]
	public EventReference techViewSoundPath;

	private EventInstance techViewSound;

	private bool techViewSoundPlaying;

	public static OverlayScreen Instance;

	[Header("Power")]
	[SerializeField]
	private Canvas powerLabelParent;

	[SerializeField]
	private LocText powerLabelPrefab;

	[SerializeField]
	private BatteryUI batUIPrefab;

	[SerializeField]
	private Vector3 powerLabelOffset;

	[SerializeField]
	private Vector3 batteryUIOffset;

	[SerializeField]
	private Vector3 batteryUITransformerOffset;

	[SerializeField]
	private Vector3 batteryUISmallTransformerOffset;

	[SerializeField]
	private Color consumerColour;

	[SerializeField]
	private Color generatorColour;

	[SerializeField]
	private Color buildingDisabledColour = Color.gray;

	[Header("Circuits")]
	[SerializeField]
	private Color32 circuitUnpoweredColour;

	[SerializeField]
	private Color32 circuitSafeColour;

	[SerializeField]
	private Color32 circuitStrainingColour;

	[SerializeField]
	private Color32 circuitOverloadingColour;

	[Header("Crops")]
	[SerializeField]
	private GameObject harvestableNotificationPrefab;

	[Header("Disease")]
	[SerializeField]
	private GameObject diseaseOverlayPrefab;

	[Header("Suit")]
	[SerializeField]
	private GameObject suitOverlayPrefab;

	[Header("ToolTip")]
	[SerializeField]
	private TextStyleSetting TooltipHeader;

	[SerializeField]
	private TextStyleSetting TooltipDescription;

	[Header("Logic")]
	[SerializeField]
	private LogicModeUI logicModeUIPrefab;

	public Action<HashedString> OnOverlayChanged;

	private OverlayScreen.ModeInfo currentModeInfo;

	private Dictionary<HashedString, OverlayScreen.ModeInfo> modeInfos = new Dictionary<HashedString, OverlayScreen.ModeInfo>();

	private struct ModeInfo
	{
		public OverlayModes.Mode mode;
	}
}
