using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.UI;

[SkipSaveFileSerialization]
public class BuildingCellVisualizer : KMonoBehaviour
{
	public bool RequiresPowerInput
	{
		get
		{
			return this.requiresPowerInput;
		}
	}

	public bool RequiresPowerOutput
	{
		get
		{
			return this.requiresPowerOutput;
		}
	}

	public bool RequiresPower
	{
		get
		{
			return this.requiresPowerInput || this.requiresPowerOutput;
		}
	}

	public bool RequiresGas
	{
		get
		{
			return this.requiresGasInput || this.requiresGasOutput;
		}
	}

	public bool RequiresLiquid
	{
		get
		{
			return this.requiresLiquidInput || this.requiresLiquidOutput;
		}
	}

	public bool RequiresGasOrLiquid
	{
		get
		{
			return this.RequiresGas || this.RequiresLiquid;
		}
	}

	public void ConnectedEventWithDelay(float delay, int connectionCount, int cell, string soundName)
	{
		base.StartCoroutine(this.ConnectedDelay(delay, connectionCount, cell, soundName));
	}

	private IEnumerator ConnectedDelay(float delay, int connectionCount, int cell, string soundName)
	{
		float startTime = Time.realtimeSinceStartup;
		float currentTime = startTime;
		while (currentTime < startTime + delay)
		{
			currentTime += Time.unscaledDeltaTime;
			yield return new WaitForEndOfFrame();
		}
		this.ConnectedEvent(cell);
		string connectedReleaseSound = GlobalAssets.GetSound(soundName, false);
		if (connectedReleaseSound != null)
		{
			Vector3 position = base.transform.position;
			EventInstance eventInstance = SoundEvent.BeginOneShot(connectedReleaseSound, position);
			eventInstance.setParameterValue("connectedCount", (float)connectionCount);
			SoundEvent.EndOneShot(eventInstance);
		}
		yield break;
	}

	public void ConnectedEvent(int cell)
	{
		GameObject gameObject = null;
		if (this.inputVisualizer != null && Grid.PosToCell(this.inputVisualizer) == cell)
		{
			gameObject = this.inputVisualizer;
		}
		else if (this.outputVisualizer != null && Grid.PosToCell(this.outputVisualizer) == cell)
		{
			gameObject = this.outputVisualizer;
		}
		else if (this.secondaryOutputVisualizer != null && Grid.PosToCell(this.secondaryOutputVisualizer) == cell)
		{
			gameObject = this.secondaryOutputVisualizer;
		}
		if (gameObject == null)
		{
			return;
		}
		SizePulse pulse = gameObject.gameObject.AddComponent<SizePulse>();
		pulse.speed = 20f;
		pulse.multiplier = 0.75f;
		pulse.updateWhenPaused = true;
		SizePulse pulse2 = pulse;
		pulse2.onComplete = (global::System.Action)Delegate.Combine(pulse2.onComplete, new global::System.Action(delegate
		{
			global::UnityEngine.Object.Destroy(pulse);
		}));
	}

	protected override void OnSpawn()
	{
		this.resources = BuildingCellVisualizerResources.Instance();
		this.enableRaycast = base.GetComponent<BuildingComplete>() != null;
		this.icons = new Dictionary<GameObject, Image>();
		this.RefreshState();
		base.Subscribe(-235298596, new Action<object>(this.OnBuildingUpgraded));
	}

	private void OnBuildingUpgraded(object data)
	{
		this.RefreshState();
	}

	private void RefreshState()
	{
		BuildingDef def = base.GetComponent<Building>().Def;
		this.requiresPowerInput = BuildingCellVisualizer.CheckRequiresPowerInput(def);
		this.requiresPowerOutput = BuildingCellVisualizer.CheckRequiresPowerOutput(def);
		this.requiresGasInput = BuildingCellVisualizer.CheckRequiresGasInput(def);
		this.requiresGasOutput = BuildingCellVisualizer.CheckRequiresGasOutput(def);
		this.requiresLiquidInput = BuildingCellVisualizer.CheckRequiresLiquidInput(def);
		this.requiresLiquidOutput = BuildingCellVisualizer.CheckRequiresLiquidOutput(def);
		DiseaseVisualization.Info info = Assets.instance.DiseaseVisualization.GetInfo(def.DiseaseCellVisName);
		if (info.name != null)
		{
			this.diseaseSourceSprite = Assets.instance.DiseaseVisualization.overlaySprite;
			this.diseaseSourceColour = info.overlayColour;
		}
		Component component = def.BuildingComplete.GetComponent<ElementFilter>();
		if (component != null)
		{
			this.requiresSecondGasOutput = def.OutputConduitType == ConduitType.Gas;
			this.requiredSecondLiquidOutput = def.OutputConduitType == ConduitType.Liquid;
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (this.inputVisualizer != null)
		{
			global::UnityEngine.Object.Destroy(this.inputVisualizer);
		}
		if (this.outputVisualizer != null)
		{
			global::UnityEngine.Object.Destroy(this.outputVisualizer);
		}
		if (this.secondaryOutputVisualizer != null)
		{
			global::UnityEngine.Object.Destroy(this.secondaryOutputVisualizer);
		}
	}

	public static bool CheckRequiresComponent(BuildingDef def)
	{
		return BuildingCellVisualizer.CheckRequiresPowerInput(def) || BuildingCellVisualizer.CheckRequiresPowerOutput(def) || BuildingCellVisualizer.CheckRequiresGasInput(def) || BuildingCellVisualizer.CheckRequiresGasOutput(def) || BuildingCellVisualizer.CheckRequiresLiquidInput(def) || BuildingCellVisualizer.CheckRequiresLiquidOutput(def) || def.DiseaseCellVisName != null;
	}

	public static bool CheckRequiresPowerInput(BuildingDef def)
	{
		return def.RequiresPowerInput;
	}

	public static bool CheckRequiresPowerOutput(BuildingDef def)
	{
		return def.GeneratorWattageRating > 0f || def.RequiresPowerOutput;
	}

	public static bool CheckRequiresGasInput(BuildingDef def)
	{
		return def.InputConduitType == ConduitType.Gas;
	}

	public static bool CheckRequiresGasOutput(BuildingDef def)
	{
		return def.OutputConduitType == ConduitType.Gas;
	}

	public static bool CheckRequiresLiquidInput(BuildingDef def)
	{
		return def.InputConduitType == ConduitType.Liquid;
	}

	public static bool CheckRequiresLiquidOutput(BuildingDef def)
	{
		return def.OutputConduitType == ConduitType.Liquid;
	}

	private bool CompareWireConnection(int cell, UtilityConnections[] connections)
	{
		GameObject gameObject = Grid.Objects[cell, 20];
		if (gameObject != null)
		{
			Wire component = gameObject.GetComponent<Wire>();
			if (component != null)
			{
				for (int i = 0; i < connections.Length; i++)
				{
					UtilityConnections wireConnections = component.GetWireConnections();
					if ((wireConnections & connections[i]) != (UtilityConnections)0)
					{
						return false;
					}
				}
			}
		}
		return false;
	}

	private Color GetWireColor(int cell)
	{
		Color color = Color.white;
		GameObject gameObject = Grid.Objects[cell, 20];
		if (gameObject != null)
		{
			KBatchedAnimController component = gameObject.GetComponent<KBatchedAnimController>();
			if (component != null)
			{
				color = component.TintColour;
			}
		}
		return color;
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		Components.BuildingCellVisualizers.Add(this);
	}

	protected override void OnCmpDisable()
	{
		base.OnCmpDisable();
		Components.BuildingCellVisualizers.Remove(this);
	}

	public void Tick()
	{
		Building component = base.GetComponent<Building>();
		SimViewMode mode = OverlayScreen.Instance.GetMode();
		if (mode != this.previousMode)
		{
			this.DisableIcons();
		}
		if (mode != SimViewMode.LiquidVentMap)
		{
			if (mode != SimViewMode.PowerMap)
			{
				if (mode != SimViewMode.GasVentMap)
				{
					if (mode != SimViewMode.Disease)
					{
						this.DisableIcons();
					}
					else if (this.diseaseSourceSprite != null)
					{
						int utilityOutputCell = component.GetUtilityOutputCell();
						this.DrawUtilityIcon(utilityOutputCell, this.diseaseSourceSprite, ref this.inputVisualizer, this.diseaseSourceColour);
					}
				}
				else if (this.requiresGasInput || this.requiresGasOutput || this.requiresSecondGasOutput)
				{
					if (this.requiresGasInput)
					{
						Sprite gasInputIcon = this.resources.gasInputIcon;
						bool flag = null != Grid.Objects[component.GetUtilityInputCell(), 12];
						BuildingCellVisualizerResources.ConnectedDisconnectedColours input = this.resources.gasIOColours.input;
						Color color = ((!flag) ? input.disconnected : input.connected);
						this.DrawUtilityIcon(component.GetUtilityInputCell(), gasInputIcon, ref this.inputVisualizer, color);
					}
					if (this.requiresGasOutput)
					{
						Sprite gasOutputIcon = this.resources.gasOutputIcon;
						bool flag2 = null != Grid.Objects[component.GetUtilityOutputCell(), 12];
						BuildingCellVisualizerResources.ConnectedDisconnectedColours output = this.resources.gasIOColours.output;
						Color color2 = ((!flag2) ? output.disconnected : output.connected);
						this.DrawUtilityIcon(component.GetUtilityOutputCell(), gasOutputIcon, ref this.outputVisualizer, color2);
					}
					if (this.requiresSecondGasOutput)
					{
						Sprite gasOutputIcon2 = this.resources.gasOutputIcon;
						int utilityOutputCell2 = component.GetUtilityOutputCell();
						CellOffset rotatedOffset = component.GetRotatedOffset(this.secondOutputOffset);
						int num = Grid.OffsetCell(utilityOutputCell2, rotatedOffset);
						this.DrawUtilityIcon(num, gasOutputIcon2, ref this.secondaryOutputVisualizer, BuildingCellVisualizer.secondOutputColour, Color.white, 1.5f, false);
					}
				}
				else
				{
					this.DisableIcons();
				}
			}
			else if (this.requiresPowerInput || this.requiresPowerOutput)
			{
				bool flag3 = component.GetComponent<BuildingPreview>() != null;
				BuildingEnabledButton component2 = component.GetComponent<BuildingEnabledButton>();
				int powerInputCell = component.GetPowerInputCell();
				if (this.requiresPowerInput)
				{
					int circuitID = (int)Game.Instance.circuitManager.GetCircuitID(powerInputCell);
					Color color3 = ((!(component2 != null) || component2.IsEnabled) ? Color.white : Color.gray);
					Sprite sprite = ((flag3 || circuitID == 65535) ? this.resources.electricityInputIcon : this.resources.electricityConnectedIcon);
					this.DrawUtilityIcon(powerInputCell, sprite, ref this.inputVisualizer, color3, this.GetWireColor(powerInputCell), 1f, false);
				}
				if (this.requiresPowerOutput)
				{
					int powerOutputCell = component.GetPowerOutputCell();
					int circuitID2 = (int)Game.Instance.circuitManager.GetCircuitID(powerOutputCell);
					Color color4 = ((!component.Def.UseWhitePowerOutputConnectorColour) ? this.resources.electricityOutputColor : Color.white);
					Color32 color5 = ((!(component2 != null) || component2.IsEnabled) ? color4 : Color.gray);
					Sprite sprite2 = ((flag3 || circuitID2 == 65535) ? this.resources.electricityInputIcon : this.resources.electricityConnectedIcon);
					this.DrawUtilityIcon(powerOutputCell, sprite2, ref this.outputVisualizer, color5, this.GetWireColor(powerOutputCell), 1f, false);
				}
			}
			else
			{
				bool flag4 = true;
				Switch component3 = base.GetComponent<Switch>();
				if (component3 != null)
				{
					int num2 = Grid.PosToCell(base.transform.position);
					Color32 color6 = ((!component3.IsHandlerOn()) ? this.resources.switchOffColor : this.resources.switchColor);
					this.DrawUtilityIcon(num2, this.resources.switchIcon, ref this.outputVisualizer, color6, Color.white, 1f, false);
					flag4 = false;
				}
				else
				{
					WireUtilityNetworkLink component4 = base.GetComponent<WireUtilityNetworkLink>();
					if (component4 != null)
					{
						int num3;
						int num4;
						component4.GetCells(out num3, out num4);
						this.DrawUtilityIcon(num3, (Game.Instance.circuitManager.GetCircuitID(num3) != ushort.MaxValue) ? this.resources.electricityConnectedIcon : this.resources.electricityBridgeIcon, ref this.inputVisualizer, this.resources.electricityInputColor, Color.white, 1f, false);
						this.DrawUtilityIcon(num4, (Game.Instance.circuitManager.GetCircuitID(num4) != ushort.MaxValue) ? this.resources.electricityConnectedIcon : this.resources.electricityBridgeIcon, ref this.outputVisualizer, this.resources.electricityInputColor, Color.white, 1f, false);
						flag4 = false;
					}
				}
				if (flag4)
				{
					this.DisableIcons();
				}
			}
		}
		else if (this.requiresLiquidInput || this.requiresLiquidOutput || this.requiredSecondLiquidOutput)
		{
			if (this.requiresLiquidInput)
			{
				bool flag5 = null != Grid.Objects[component.GetUtilityInputCell(), 16];
				BuildingCellVisualizerResources.ConnectedDisconnectedColours input2 = this.resources.liquidIOColours.input;
				Color color7 = ((!flag5) ? input2.disconnected : input2.connected);
				this.DrawUtilityIcon(component.GetUtilityInputCell(), this.resources.liquidInputIcon, ref this.inputVisualizer, color7);
			}
			if (this.requiresLiquidOutput)
			{
				bool flag6 = null != Grid.Objects[component.GetUtilityOutputCell(), 16];
				BuildingCellVisualizerResources.ConnectedDisconnectedColours output2 = this.resources.liquidIOColours.output;
				Color color8 = ((!flag6) ? output2.disconnected : output2.connected);
				this.DrawUtilityIcon(component.GetUtilityOutputCell(), this.resources.liquidOutputIcon, ref this.outputVisualizer, color8);
			}
			if (this.requiredSecondLiquidOutput)
			{
				int utilityOutputCell3 = component.GetUtilityOutputCell();
				CellOffset rotatedOffset2 = component.GetRotatedOffset(this.secondOutputOffset);
				int num5 = Grid.OffsetCell(utilityOutputCell3, rotatedOffset2);
				this.DrawUtilityIcon(num5, this.resources.liquidOutputIcon, ref this.secondaryOutputVisualizer, BuildingCellVisualizer.secondOutputColour, Color.white, 1.5f, false);
			}
		}
		else
		{
			this.DisableIcons();
		}
		this.previousMode = mode;
	}

	private void DisableIcons()
	{
		if (this.inputVisualizer != null && this.inputVisualizer.activeInHierarchy)
		{
			this.inputVisualizer.SetActive(false);
		}
		if (this.outputVisualizer != null && this.outputVisualizer.activeInHierarchy)
		{
			this.outputVisualizer.SetActive(false);
		}
		if (this.secondaryOutputVisualizer != null && this.secondaryOutputVisualizer.activeInHierarchy)
		{
			this.secondaryOutputVisualizer.SetActive(false);
		}
	}

	private void DrawUtilityIcon(int cell, Sprite icon_img, ref GameObject visualizerObj)
	{
		this.DrawUtilityIcon(cell, icon_img, ref visualizerObj, Color.white, Color.white, 1.5f, false);
	}

	private void DrawUtilityIcon(int cell, Sprite icon_img, ref GameObject visualizerObj, Color tint)
	{
		this.DrawUtilityIcon(cell, icon_img, ref visualizerObj, tint, Color.white, 1.5f, false);
	}

	private void DrawUtilityIcon(int cell, Sprite icon_img, ref GameObject visualizerObj, Color tint, Color connectorColor, float scaleMultiplier = 1.5f, bool hideBG = false)
	{
		Vector3 vector = Grid.CellToPosCCC(cell, Grid.SceneLayer.Building);
		if (visualizerObj == null)
		{
			visualizerObj = global::Util.KInstantiate(Assets.UIPrefabs.ResourceVisualizer, GameScreenManager.Instance.worldSpaceCanvas, null);
			visualizerObj.transform.SetAsFirstSibling();
			this.icons.Add(visualizerObj, visualizerObj.transform.GetChild(0).GetComponent<Image>());
		}
		if (!visualizerObj.gameObject.activeInHierarchy)
		{
			visualizerObj.gameObject.SetActive(true);
		}
		Image image = visualizerObj.GetComponent<Image>();
		image.enabled = !hideBG;
		this.icons[visualizerObj].raycastTarget = this.enableRaycast;
		this.icons[visualizerObj].sprite = icon_img;
		Transform child = visualizerObj.transform.GetChild(0);
		image = child.gameObject.GetComponent<Image>();
		image.color = tint;
		visualizerObj.transform.SetPosition(vector);
		if (visualizerObj.GetComponent<SizePulse>() == null)
		{
			visualizerObj.transform.localScale = Vector3.one * scaleMultiplier;
		}
	}

	public Image GetOutputIcon()
	{
		return (!(this.outputVisualizer == null)) ? this.outputVisualizer.transform.GetChild(0).GetComponent<Image>() : null;
	}

	public Image GetInputIcon()
	{
		return (!(this.inputVisualizer == null)) ? this.inputVisualizer.transform.GetChild(0).GetComponent<Image>() : null;
	}

	private BuildingCellVisualizerResources resources;

	[SerializeField]
	public CellOffset secondOutputOffset = new CellOffset(-1, 0);

	[SerializeField]
	public static Color32 secondOutputColour = new Color(0.9843137f, 0.6901961f, 0.23137255f);

	private bool requiresPowerInput;

	private bool requiresPowerOutput;

	private bool requiresGasInput;

	private bool requiresGasOutput;

	private bool requiresLiquidInput;

	private bool requiresLiquidOutput;

	private Sprite diseaseSourceSprite;

	private Color32 diseaseSourceColour;

	private bool requiresSecondGasOutput;

	private bool requiredSecondLiquidOutput;

	private GameObject inputVisualizer;

	private GameObject outputVisualizer;

	private GameObject secondaryOutputVisualizer;

	private bool enableRaycast;

	private Dictionary<GameObject, Image> icons;

	private SimViewMode previousMode;
}
