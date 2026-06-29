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

	public bool RequiresSolid
	{
		get
		{
			return this.requiresSolidInput || this.requiresSolidOutput;
		}
	}

	public bool RequiresUtilityConnection
	{
		get
		{
			return this.RequiresGas || this.RequiresLiquid || this.RequiresSolid;
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
			Vector3 position = base.transform.GetPosition();
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
		else if (this.secondaryInputVisualizer != null && Grid.PosToCell(this.secondaryInputVisualizer) == cell)
		{
			gameObject = this.secondaryInputVisualizer;
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
		this.enableRaycast = this.building as BuildingComplete != null;
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
		BuildingDef def = this.building.Def;
		this.requiresPowerInput = BuildingCellVisualizer.CheckRequiresPowerInput(def);
		this.requiresPowerOutput = BuildingCellVisualizer.CheckRequiresPowerOutput(def);
		this.requiresGasInput = BuildingCellVisualizer.CheckRequiresGasInput(def);
		this.requiresGasOutput = BuildingCellVisualizer.CheckRequiresGasOutput(def);
		this.requiresLiquidInput = BuildingCellVisualizer.CheckRequiresLiquidInput(def);
		this.requiresLiquidOutput = BuildingCellVisualizer.CheckRequiresLiquidOutput(def);
		this.requiresSolidInput = BuildingCellVisualizer.CheckRequiresSolidInput(def);
		this.requiresSolidOutput = BuildingCellVisualizer.CheckRequiresSolidOutput(def);
		DiseaseVisualization.Info info = Assets.instance.DiseaseVisualization.GetInfo(def.DiseaseCellVisName);
		if (info.name != null)
		{
			this.diseaseSourceSprite = Assets.instance.DiseaseVisualization.overlaySprite;
			this.diseaseSourceColour = info.overlayColour;
		}
		ISecondaryInput component = def.BuildingComplete.GetComponent<ISecondaryInput>();
		if (component != null)
		{
			ConduitType secondaryConduitType = component.GetSecondaryConduitType();
			this.requiresSecondGasInput = secondaryConduitType == ConduitType.Gas;
			this.requiresSecondLiquidInput = secondaryConduitType == ConduitType.Liquid;
		}
		ISecondaryOutput component2 = def.BuildingComplete.GetComponent<ISecondaryOutput>();
		if (component2 != null)
		{
			ConduitType secondaryConduitType2 = component2.GetSecondaryConduitType();
			this.requiresSecondGasOutput = secondaryConduitType2 == ConduitType.Gas;
			this.requiresSecondLiquidOutput = secondaryConduitType2 == ConduitType.Liquid;
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
		if (this.secondaryInputVisualizer != null)
		{
			global::UnityEngine.Object.Destroy(this.secondaryInputVisualizer);
		}
		if (this.secondaryOutputVisualizer != null)
		{
			global::UnityEngine.Object.Destroy(this.secondaryOutputVisualizer);
		}
	}

	public static bool CheckRequiresComponent(BuildingDef def)
	{
		return BuildingCellVisualizer.CheckRequiresPowerInput(def) || BuildingCellVisualizer.CheckRequiresPowerOutput(def) || BuildingCellVisualizer.CheckRequiresGasInput(def) || BuildingCellVisualizer.CheckRequiresGasOutput(def) || BuildingCellVisualizer.CheckRequiresLiquidInput(def) || BuildingCellVisualizer.CheckRequiresLiquidOutput(def) || BuildingCellVisualizer.CheckRequiresSolidInput(def) || BuildingCellVisualizer.CheckRequiresSolidOutput(def) || def.DiseaseCellVisName != null;
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

	public static bool CheckRequiresSolidInput(BuildingDef def)
	{
		return def.InputConduitType == ConduitType.Solid;
	}

	public static bool CheckRequiresSolidOutput(BuildingDef def)
	{
		return def.OutputConduitType == ConduitType.Solid;
	}

	private bool CompareWireConnection(int cell, UtilityConnections[] connections)
	{
		GameObject gameObject = Grid.Objects[cell, 24];
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
		GameObject gameObject = Grid.Objects[cell, 24];
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

	public void Tick(SimViewMode mode)
	{
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
					if (mode != SimViewMode.SolidConveyorMap)
					{
						if (mode != SimViewMode.Disease)
						{
							this.DisableIcons();
						}
						else if (this.diseaseSourceSprite != null)
						{
							int utilityOutputCell = this.building.GetUtilityOutputCell();
							this.DrawUtilityIcon(utilityOutputCell, this.diseaseSourceSprite, ref this.inputVisualizer, this.diseaseSourceColour);
						}
					}
					else if (this.requiresSolidInput || this.requiresSolidOutput)
					{
						if (this.requiresSolidInput)
						{
							bool flag = null != Grid.Objects[this.building.GetUtilityInputCell(), 20];
							BuildingCellVisualizerResources.ConnectedDisconnectedColours input = this.resources.liquidIOColours.input;
							Color color = ((!flag) ? input.disconnected : input.connected);
							this.DrawUtilityIcon(this.building.GetUtilityInputCell(), this.resources.liquidInputIcon, ref this.inputVisualizer, color);
						}
						if (this.requiresSolidOutput)
						{
							bool flag2 = null != Grid.Objects[this.building.GetUtilityOutputCell(), 20];
							BuildingCellVisualizerResources.ConnectedDisconnectedColours output = this.resources.liquidIOColours.output;
							Color color2 = ((!flag2) ? output.disconnected : output.connected);
							this.DrawUtilityIcon(this.building.GetUtilityOutputCell(), this.resources.liquidOutputIcon, ref this.outputVisualizer, color2);
						}
					}
					else
					{
						this.DisableIcons();
					}
				}
				else if (this.requiresGasInput || this.requiresGasOutput || this.requiresSecondGasOutput || this.requiresSecondGasInput)
				{
					if (this.requiresGasInput)
					{
						bool flag3 = null != Grid.Objects[this.building.GetUtilityInputCell(), 12];
						BuildingCellVisualizerResources.ConnectedDisconnectedColours input2 = this.resources.gasIOColours.input;
						Color color3 = ((!flag3) ? input2.disconnected : input2.connected);
						this.DrawUtilityIcon(this.building.GetUtilityInputCell(), this.resources.gasInputIcon, ref this.inputVisualizer, color3);
					}
					if (this.requiresGasOutput)
					{
						bool flag4 = null != Grid.Objects[this.building.GetUtilityOutputCell(), 12];
						BuildingCellVisualizerResources.ConnectedDisconnectedColours output2 = this.resources.gasIOColours.output;
						Color color4 = ((!flag4) ? output2.disconnected : output2.connected);
						this.DrawUtilityIcon(this.building.GetUtilityOutputCell(), this.resources.gasOutputIcon, ref this.outputVisualizer, color4);
					}
					if (this.requiresSecondGasInput)
					{
						CellOffset secondaryConduitOffset = this.building.GetComponent<ISecondaryInput>().GetSecondaryConduitOffset();
						int visualizerCell = this.GetVisualizerCell(this.building, secondaryConduitOffset);
						this.DrawUtilityIcon(visualizerCell, this.resources.gasInputIcon, ref this.secondaryInputVisualizer, BuildingCellVisualizer.secondInputColour, Color.white, 1.5f, false);
					}
					if (this.requiresSecondGasOutput)
					{
						CellOffset secondaryConduitOffset2 = this.building.GetComponent<ISecondaryOutput>().GetSecondaryConduitOffset();
						int visualizerCell2 = this.GetVisualizerCell(this.building, secondaryConduitOffset2);
						this.DrawUtilityIcon(visualizerCell2, this.resources.gasOutputIcon, ref this.secondaryOutputVisualizer, BuildingCellVisualizer.secondOutputColour, Color.white, 1.5f, false);
					}
				}
				else
				{
					this.DisableIcons();
				}
			}
			else if (this.requiresPowerInput || this.requiresPowerOutput)
			{
				bool flag5 = this.building as BuildingPreview != null;
				BuildingEnabledButton component = this.building.GetComponent<BuildingEnabledButton>();
				int powerInputCell = this.building.GetPowerInputCell();
				if (this.requiresPowerInput)
				{
					int circuitID = (int)Game.Instance.circuitManager.GetCircuitID(powerInputCell);
					Color color5 = ((!(component != null) || component.IsEnabled) ? Color.white : Color.gray);
					Sprite sprite = ((flag5 || circuitID == 65535) ? this.resources.electricityInputIcon : this.resources.electricityConnectedIcon);
					this.DrawUtilityIcon(powerInputCell, sprite, ref this.inputVisualizer, color5, this.GetWireColor(powerInputCell), 1f, false);
				}
				if (this.requiresPowerOutput)
				{
					int powerOutputCell = this.building.GetPowerOutputCell();
					int circuitID2 = (int)Game.Instance.circuitManager.GetCircuitID(powerOutputCell);
					Color color6 = ((!this.building.Def.UseWhitePowerOutputConnectorColour) ? this.resources.electricityOutputColor : Color.white);
					Color32 color7 = ((!(component != null) || component.IsEnabled) ? color6 : Color.gray);
					Sprite sprite2 = ((flag5 || circuitID2 == 65535) ? this.resources.electricityInputIcon : this.resources.electricityConnectedIcon);
					this.DrawUtilityIcon(powerOutputCell, sprite2, ref this.outputVisualizer, color7, this.GetWireColor(powerOutputCell), 1f, false);
				}
			}
			else
			{
				bool flag6 = true;
				Switch component2 = base.GetComponent<Switch>();
				if (component2 != null)
				{
					int num = Grid.PosToCell(base.transform.GetPosition());
					Color32 color8 = ((!component2.IsHandlerOn()) ? this.resources.switchOffColor : this.resources.switchColor);
					this.DrawUtilityIcon(num, this.resources.switchIcon, ref this.outputVisualizer, color8, Color.white, 1f, false);
					flag6 = false;
				}
				else
				{
					WireUtilityNetworkLink component3 = base.GetComponent<WireUtilityNetworkLink>();
					if (component3 != null)
					{
						int num2;
						int num3;
						component3.GetCells(out num2, out num3);
						this.DrawUtilityIcon(num2, (Game.Instance.circuitManager.GetCircuitID(num2) != ushort.MaxValue) ? this.resources.electricityConnectedIcon : this.resources.electricityBridgeIcon, ref this.inputVisualizer, this.resources.electricityInputColor, Color.white, 1f, false);
						this.DrawUtilityIcon(num3, (Game.Instance.circuitManager.GetCircuitID(num3) != ushort.MaxValue) ? this.resources.electricityConnectedIcon : this.resources.electricityBridgeIcon, ref this.outputVisualizer, this.resources.electricityInputColor, Color.white, 1f, false);
						flag6 = false;
					}
				}
				if (flag6)
				{
					this.DisableIcons();
				}
			}
		}
		else if (this.requiresLiquidInput || this.requiresLiquidOutput || this.requiresSecondLiquidOutput || this.requiresSecondLiquidInput)
		{
			if (this.requiresLiquidInput)
			{
				bool flag7 = null != Grid.Objects[this.building.GetUtilityInputCell(), 16];
				BuildingCellVisualizerResources.ConnectedDisconnectedColours input3 = this.resources.liquidIOColours.input;
				Color color9 = ((!flag7) ? input3.disconnected : input3.connected);
				this.DrawUtilityIcon(this.building.GetUtilityInputCell(), this.resources.liquidInputIcon, ref this.inputVisualizer, color9);
			}
			if (this.requiresLiquidOutput)
			{
				bool flag8 = null != Grid.Objects[this.building.GetUtilityOutputCell(), 16];
				BuildingCellVisualizerResources.ConnectedDisconnectedColours output3 = this.resources.liquidIOColours.output;
				Color color10 = ((!flag8) ? output3.disconnected : output3.connected);
				this.DrawUtilityIcon(this.building.GetUtilityOutputCell(), this.resources.liquidOutputIcon, ref this.outputVisualizer, color10);
			}
			if (this.requiresSecondLiquidInput)
			{
				CellOffset secondaryConduitOffset3 = this.building.GetComponent<ISecondaryInput>().GetSecondaryConduitOffset();
				int visualizerCell3 = this.GetVisualizerCell(this.building, secondaryConduitOffset3);
				this.DrawUtilityIcon(visualizerCell3, this.resources.liquidInputIcon, ref this.secondaryInputVisualizer, BuildingCellVisualizer.secondInputColour, Color.white, 1.5f, false);
			}
			if (this.requiresSecondLiquidOutput)
			{
				CellOffset secondaryConduitOffset4 = this.building.GetComponent<ISecondaryOutput>().GetSecondaryConduitOffset();
				int visualizerCell4 = this.GetVisualizerCell(this.building, secondaryConduitOffset4);
				this.DrawUtilityIcon(visualizerCell4, this.resources.liquidOutputIcon, ref this.secondaryOutputVisualizer, BuildingCellVisualizer.secondOutputColour, Color.white, 1.5f, false);
			}
		}
		else
		{
			this.DisableIcons();
		}
		this.previousMode = mode;
	}

	private int GetVisualizerCell(Building building, CellOffset offset)
	{
		CellOffset rotatedOffset = building.GetRotatedOffset(offset);
		int cell = building.GetCell();
		return Grid.OffsetCell(cell, rotatedOffset);
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
		if (this.secondaryInputVisualizer != null && this.secondaryInputVisualizer.activeInHierarchy)
		{
			this.secondaryInputVisualizer.SetActive(false);
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

	[MyCmpReq]
	private Building building;

	[SerializeField]
	public static Color32 secondOutputColour = new Color(0.9843137f, 0.6901961f, 0.23137255f);

	[SerializeField]
	public static Color32 secondInputColour = new Color(0.9843137f, 0.6901961f, 0.23137255f);

	private bool requiresPowerInput;

	private bool requiresPowerOutput;

	private bool requiresGasInput;

	private bool requiresGasOutput;

	private bool requiresLiquidInput;

	private bool requiresLiquidOutput;

	private bool requiresSolidInput;

	private bool requiresSolidOutput;

	private Sprite diseaseSourceSprite;

	private Color32 diseaseSourceColour;

	private bool requiresSecondGasInput;

	private bool requiresSecondGasOutput;

	private bool requiresSecondLiquidInput;

	private bool requiresSecondLiquidOutput;

	private GameObject inputVisualizer;

	private GameObject outputVisualizer;

	private GameObject secondaryInputVisualizer;

	private GameObject secondaryOutputVisualizer;

	private bool enableRaycast;

	private Dictionary<GameObject, Image> icons;

	private SimViewMode previousMode;
}
