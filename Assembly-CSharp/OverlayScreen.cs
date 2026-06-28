using System;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using OverlayModes;
using UnityEngine;

public class OverlayScreen : KMonoBehaviour
{
	public SimViewMode mode
	{
		get
		{
			return this.currentMode.ViewMode();
		}
	}

	protected override void OnPrefabInit()
	{
		OverlayScreen.Instance = this;
		this.powerLabelParent = GameObject.Find("WorldSpaceCanvas").GetComponent<Canvas>();
	}

	protected override void OnLoadLevel()
	{
		this.currentMode = null;
		this.harvestableNotificationPrefab = null;
		this.powerLabelParent = null;
		OverlayScreen.Instance = null;
		Mode.Clear();
		this.modes = null;
		base.OnLoadLevel();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.techViewSound = KFMOD.CreateInstance(this.techViewSoundPath);
		this.techViewSoundPlaying = false;
		Shader.SetGlobalVector("_OverlayParams", Vector4.zero);
		this.RegisterModes();
	}

	private void RegisterModes()
	{
		this.modes.Clear();
		None none = new None();
		this.RegisterMode(none);
		this.RegisterMode(new Oxygen());
		this.RegisterMode(new Power(this.powerLabelParent, this.powerLabelPrefab, this.batUIPrefab, this.powerLabelOffset, this.batteryUIOffset, this.batteryUITransformerOffset, this.consumerColour, this.generatorColour, this.buildingDisabledColour, this.circuitUnpoweredColour, this.circuitSafeColour, this.circuitStrainingColour));
		this.RegisterMode(new Temperature());
		this.RegisterMode(new ThermalConductivity());
		this.RegisterMode(new global::OverlayModes.Light());
		this.RegisterMode(new LiquidConduitMode());
		this.RegisterMode(new GasConduitMode());
		this.RegisterMode(new Decor());
		this.RegisterMode(new Disease(this.powerLabelParent, this.diseaseOverlayPrefab));
		this.RegisterMode(new global::OverlayModes.Crop(this.powerLabelParent, this.harvestableNotificationPrefab));
		this.RegisterMode(new Harvest());
		this.RegisterMode(new Priorities());
		this.RegisterMode(new HeatFlow());
		this.RegisterMode(new Rooms());
		this.RegisterMode(new Suit(this.powerLabelParent, this.suitOverlayPrefab));
		foreach (object obj in Enum.GetValues(typeof(SimViewMode)))
		{
			SimViewMode simViewMode = (SimViewMode)((int)obj);
			if (!this.modes.ContainsKey(simViewMode))
			{
				this.modes[simViewMode] = none;
			}
		}
	}

	private void RegisterMode(Mode mode)
	{
		this.modes[mode.ViewMode()] = mode;
	}

	private void Update()
	{
		this.currentMode.Update();
	}

	public void ToggleOverlay(SimViewMode newMode)
	{
		bool flag = this.currentMode.ViewMode() != newMode;
		if (newMode != SimViewMode.None)
		{
			ManagementMenu.Instance.CloseAll();
		}
		this.currentMode.Disable();
		if (newMode != this.currentMode.ViewMode() && newMode == SimViewMode.None)
		{
			ManagementMenu.Instance.CloseAll();
		}
		ResourceCategoryScreen.Instance.Show(newMode == SimViewMode.None);
		SimDebugView.Instance.SetMode(newMode);
		this.currentMode = this.modes[newMode];
		this.currentMode.Enable();
		if (flag)
		{
			this.UpdateOverlaySounds();
		}
		if (this.currentMode.ViewMode() == SimViewMode.None)
		{
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().TechFilterOnMigrated, STOP_MODE.ALLOWFADEOUT);
			MusicManager.instance.SetDynamicMusicOverlayInactive();
			this.techViewSound.stop(STOP_MODE.ALLOWFADEOUT);
			this.techViewSoundPlaying = false;
		}
		else if (!this.techViewSoundPlaying)
		{
			AudioMixer.instance.Start(AudioMixerSnapshots.Get().TechFilterOnMigrated);
			MusicManager.instance.SetDynamicMusicOverlayActive();
			this.techViewSound.start();
			this.techViewSound.setParameterValue("View", (float)this.currentMode.ViewMode());
			this.techViewSoundPlaying = true;
		}
		if (this.OnOverlayChanged != null)
		{
			this.OnOverlayChanged(this.currentMode.ViewMode());
		}
		this.ActivateLegend();
	}

	private void ActivateLegend()
	{
		if (OverlayLegend.Instance == null)
		{
			return;
		}
		OverlayLegend.Instance.SetLegend(this.currentMode.ViewMode(), false);
	}

	public void Refresh()
	{
		this.Update();
	}

	public SimViewMode GetMode()
	{
		return this.currentMode.ViewMode();
	}

	private void UpdateOverlaySounds()
	{
		string text = this.currentMode.GetSoundName();
		if (text != string.Empty)
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

	[SerializeField]
	[EventRef]
	public string techViewSoundPath;

	private EventInstance techViewSound;

	private bool techViewSoundPlaying;

	public static OverlayScreen Instance;

	[SerializeField]
	[Header("Power")]
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

	[Header("Crops")]
	[SerializeField]
	private GameObject harvestableNotificationPrefab;

	[Header("Disease")]
	[SerializeField]
	private GameObject diseaseOverlayPrefab;

	[SerializeField]
	[Header("Suit")]
	private GameObject suitOverlayPrefab;

	[Header("ToolTip")]
	[SerializeField]
	private TextStyleSetting TooltipHeader;

	[SerializeField]
	private TextStyleSetting TooltipDescription;

	public Action<SimViewMode> OnOverlayChanged;

	private Mode currentMode = new None();

	private Dictionary<SimViewMode, Mode> modes = new Dictionary<SimViewMode, Mode>();
}
