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
			return (byte)(this.ports & BuildingCellVisualizer.Ports.PowerIn) != 0;
		}
	}

	public bool RequiresPowerOutput
	{
		get
		{
			return (byte)(this.ports & BuildingCellVisualizer.Ports.PowerOut) != 0;
		}
	}

	public bool RequiresPower
	{
		get
		{
			return (byte)(this.ports & (BuildingCellVisualizer.Ports.PowerIn | BuildingCellVisualizer.Ports.PowerOut)) != 0;
		}
	}

	public bool RequiresGas
	{
		get
		{
			return (byte)(this.ports & (BuildingCellVisualizer.Ports.GasIn | BuildingCellVisualizer.Ports.GasOut)) != 0;
		}
	}

	public bool RequiresLiquid
	{
		get
		{
			return (byte)(this.ports & (BuildingCellVisualizer.Ports.LiquidIn | BuildingCellVisualizer.Ports.LiquidOut)) != 0;
		}
	}

	public bool RequiresSolid
	{
		get
		{
			return (byte)(this.ports & (BuildingCellVisualizer.Ports.SolidIn | BuildingCellVisualizer.Ports.SolidOut)) != 0;
		}
	}

	public bool RequiresUtilityConnection
	{
		get
		{
			return (byte)(this.ports & ~(BuildingCellVisualizer.Ports.PowerIn | BuildingCellVisualizer.Ports.PowerOut)) != 0;
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
	}

	private void MapBuilding()
	{
		BuildingDef def = this.building.Def;
		if (def.CheckRequiresPowerInput())
		{
			this.ports |= BuildingCellVisualizer.Ports.PowerIn;
		}
		if (def.CheckRequiresPowerOutput())
		{
			this.ports |= BuildingCellVisualizer.Ports.PowerOut;
		}
		if (def.CheckRequiresGasInput())
		{
			this.ports |= BuildingCellVisualizer.Ports.GasIn;
		}
		if (def.CheckRequiresGasOutput())
		{
			this.ports |= BuildingCellVisualizer.Ports.GasOut;
		}
		if (def.CheckRequiresLiquidInput())
		{
			this.ports |= BuildingCellVisualizer.Ports.LiquidIn;
		}
		if (def.CheckRequiresLiquidOutput())
		{
			this.ports |= BuildingCellVisualizer.Ports.LiquidOut;
		}
		if (def.CheckRequiresSolidInput())
		{
			this.ports |= BuildingCellVisualizer.Ports.SolidIn;
		}
		if (def.CheckRequiresSolidOutput())
		{
			this.ports |= BuildingCellVisualizer.Ports.SolidOut;
		}
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
			if (secondaryConduitType == ConduitType.Gas)
			{
				this.secondary_ports |= BuildingCellVisualizer.Ports.GasIn;
			}
			else if (secondaryConduitType == ConduitType.Liquid)
			{
				this.secondary_ports |= BuildingCellVisualizer.Ports.LiquidIn;
			}
		}
		ISecondaryOutput component2 = def.BuildingComplete.GetComponent<ISecondaryOutput>();
		if (component2 != null)
		{
			ConduitType secondaryConduitType2 = component2.GetSecondaryConduitType();
			if (secondaryConduitType2 == ConduitType.Gas)
			{
				this.secondary_ports |= BuildingCellVisualizer.Ports.GasOut;
			}
			else if (secondaryConduitType2 == ConduitType.Liquid)
			{
				this.secondary_ports |= BuildingCellVisualizer.Ports.LiquidOut;
			}
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

	private Color GetWireColor(int cell)
	{
		GameObject gameObject = Grid.Objects[cell, 26];
		if (gameObject == null)
		{
			return Color.white;
		}
		KBatchedAnimController component = gameObject.GetComponent<KBatchedAnimController>();
		return (!(component != null)) ? Color.white : component.TintColour;
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		this.MapBuilding();
		Components.BuildingCellVisualizers.Add(this);
	}

	protected override void OnCmpDisable()
	{
		base.OnCmpDisable();
		Components.BuildingCellVisualizers.Remove(this);
	}

	public void DrawIcons(HashedString mode)
	{
		if (mode == OverlayModes.Power.ID)
		{
			if (this.RequiresPower)
			{
				bool flag = this.building as BuildingPreview != null;
				BuildingEnabledButton component = this.building.GetComponent<BuildingEnabledButton>();
				int powerInputCell = this.building.GetPowerInputCell();
				if (this.RequiresPowerInput)
				{
					int circuitID = (int)Game.Instance.circuitManager.GetCircuitID(powerInputCell);
					Color color = ((!(component != null) || component.IsEnabled) ? Color.white : Color.gray);
					Sprite sprite = ((flag || circuitID == 65535) ? this.resources.electricityInputIcon : this.resources.electricityConnectedIcon);
					this.DrawUtilityIcon(powerInputCell, sprite, ref this.inputVisualizer, color, this.GetWireColor(powerInputCell), 1f, false);
				}
				if (this.RequiresPowerOutput)
				{
					int powerOutputCell = this.building.GetPowerOutputCell();
					int circuitID2 = (int)Game.Instance.circuitManager.GetCircuitID(powerOutputCell);
					Color color2 = ((!this.building.Def.UseWhitePowerOutputConnectorColour) ? this.resources.electricityOutputColor : Color.white);
					Color32 color3 = ((!(component != null) || component.IsEnabled) ? color2 : Color.gray);
					Sprite sprite2 = ((flag || circuitID2 == 65535) ? this.resources.electricityInputIcon : this.resources.electricityConnectedIcon);
					this.DrawUtilityIcon(powerOutputCell, sprite2, ref this.outputVisualizer, color3, this.GetWireColor(powerOutputCell), 1f, false);
				}
			}
			else
			{
				bool flag2 = true;
				Switch component2 = base.GetComponent<Switch>();
				if (component2 != null)
				{
					int num = Grid.PosToCell(base.transform.GetPosition());
					Color32 color4 = ((!component2.IsHandlerOn()) ? this.resources.switchOffColor : this.resources.switchColor);
					this.DrawUtilityIcon(num, this.resources.switchIcon, ref this.outputVisualizer, color4, Color.white, 1f, false);
					flag2 = false;
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
						flag2 = false;
					}
				}
				if (flag2)
				{
					this.DisableIcons();
				}
			}
		}
		else if (mode == OverlayModes.GasConduits.ID)
		{
			if (this.RequiresGas || (byte)(this.secondary_ports & (BuildingCellVisualizer.Ports.GasIn | BuildingCellVisualizer.Ports.GasOut)) != 0)
			{
				if ((byte)(this.ports & BuildingCellVisualizer.Ports.GasIn) != 0)
				{
					bool flag3 = null != Grid.Objects[this.building.GetUtilityInputCell(), 12];
					BuildingCellVisualizerResources.ConnectedDisconnectedColours input = this.resources.gasIOColours.input;
					Color color5 = ((!flag3) ? input.disconnected : input.connected);
					this.DrawUtilityIcon(this.building.GetUtilityInputCell(), this.resources.gasInputIcon, ref this.inputVisualizer, color5);
				}
				if ((byte)(this.ports & BuildingCellVisualizer.Ports.GasOut) != 0)
				{
					bool flag4 = null != Grid.Objects[this.building.GetUtilityOutputCell(), 12];
					BuildingCellVisualizerResources.ConnectedDisconnectedColours output = this.resources.gasIOColours.output;
					Color color6 = ((!flag4) ? output.disconnected : output.connected);
					this.DrawUtilityIcon(this.building.GetUtilityOutputCell(), this.resources.gasOutputIcon, ref this.outputVisualizer, color6);
				}
				if ((byte)(this.secondary_ports & BuildingCellVisualizer.Ports.GasIn) != 0)
				{
					CellOffset secondaryConduitOffset = this.building.GetComponent<ISecondaryInput>().GetSecondaryConduitOffset();
					int visualizerCell = this.GetVisualizerCell(this.building, secondaryConduitOffset);
					this.DrawUtilityIcon(visualizerCell, this.resources.gasInputIcon, ref this.secondaryInputVisualizer, BuildingCellVisualizer.secondInputColour, Color.white, 1.5f, false);
				}
				if ((byte)(this.secondary_ports & BuildingCellVisualizer.Ports.GasOut) != 0)
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
		else if (mode == OverlayModes.LiquidConduits.ID)
		{
			if (this.RequiresLiquid || (byte)(this.secondary_ports & (BuildingCellVisualizer.Ports.LiquidIn | BuildingCellVisualizer.Ports.LiquidOut)) != 0)
			{
				if ((byte)(this.ports & BuildingCellVisualizer.Ports.LiquidIn) != 0)
				{
					bool flag5 = null != Grid.Objects[this.building.GetUtilityInputCell(), 16];
					BuildingCellVisualizerResources.ConnectedDisconnectedColours input2 = this.resources.liquidIOColours.input;
					Color color7 = ((!flag5) ? input2.disconnected : input2.connected);
					this.DrawUtilityIcon(this.building.GetUtilityInputCell(), this.resources.liquidInputIcon, ref this.inputVisualizer, color7);
				}
				if ((byte)(this.ports & BuildingCellVisualizer.Ports.LiquidOut) != 0)
				{
					bool flag6 = null != Grid.Objects[this.building.GetUtilityOutputCell(), 16];
					BuildingCellVisualizerResources.ConnectedDisconnectedColours output2 = this.resources.liquidIOColours.output;
					Color color8 = ((!flag6) ? output2.disconnected : output2.connected);
					this.DrawUtilityIcon(this.building.GetUtilityOutputCell(), this.resources.liquidOutputIcon, ref this.outputVisualizer, color8);
				}
				if ((byte)(this.secondary_ports & BuildingCellVisualizer.Ports.LiquidIn) != 0)
				{
					CellOffset secondaryConduitOffset3 = this.building.GetComponent<ISecondaryInput>().GetSecondaryConduitOffset();
					int visualizerCell3 = this.GetVisualizerCell(this.building, secondaryConduitOffset3);
					this.DrawUtilityIcon(visualizerCell3, this.resources.liquidInputIcon, ref this.secondaryInputVisualizer, BuildingCellVisualizer.secondInputColour, Color.white, 1.5f, false);
				}
				if ((byte)(this.secondary_ports & BuildingCellVisualizer.Ports.LiquidOut) != 0)
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
		}
		else if (mode == OverlayModes.SolidConveyor.ID)
		{
			if (this.RequiresSolid)
			{
				if ((byte)(this.ports & BuildingCellVisualizer.Ports.SolidIn) != 0)
				{
					bool flag7 = null != Grid.Objects[this.building.GetUtilityInputCell(), 20];
					BuildingCellVisualizerResources.ConnectedDisconnectedColours input3 = this.resources.liquidIOColours.input;
					Color color9 = ((!flag7) ? input3.disconnected : input3.connected);
					this.DrawUtilityIcon(this.building.GetUtilityInputCell(), this.resources.liquidInputIcon, ref this.inputVisualizer, color9);
				}
				if ((byte)(this.ports & BuildingCellVisualizer.Ports.SolidOut) != 0)
				{
					bool flag8 = null != Grid.Objects[this.building.GetUtilityOutputCell(), 20];
					BuildingCellVisualizerResources.ConnectedDisconnectedColours output3 = this.resources.liquidIOColours.output;
					Color color10 = ((!flag8) ? output3.disconnected : output3.connected);
					this.DrawUtilityIcon(this.building.GetUtilityOutputCell(), this.resources.liquidOutputIcon, ref this.outputVisualizer, color10);
				}
			}
			else
			{
				this.DisableIcons();
			}
		}
		else if (mode == OverlayModes.Disease.ID && this.diseaseSourceSprite != null)
		{
			int utilityOutputCell = this.building.GetUtilityOutputCell();
			this.DrawUtilityIcon(utilityOutputCell, this.diseaseSourceSprite, ref this.inputVisualizer, this.diseaseSourceColour);
		}
	}

	private int GetVisualizerCell(Building building, CellOffset offset)
	{
		CellOffset rotatedOffset = building.GetRotatedOffset(offset);
		int cell = building.GetCell();
		return Grid.OffsetCell(cell, rotatedOffset);
	}

	public void DisableIcons()
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

	private const BuildingCellVisualizer.Ports POWER_PORTS = BuildingCellVisualizer.Ports.PowerIn | BuildingCellVisualizer.Ports.PowerOut;

	private const BuildingCellVisualizer.Ports GAS_PORTS = BuildingCellVisualizer.Ports.GasIn | BuildingCellVisualizer.Ports.GasOut;

	private const BuildingCellVisualizer.Ports LIQUID_PORTS = BuildingCellVisualizer.Ports.LiquidIn | BuildingCellVisualizer.Ports.LiquidOut;

	private const BuildingCellVisualizer.Ports SOLID_PORTS = BuildingCellVisualizer.Ports.SolidIn | BuildingCellVisualizer.Ports.SolidOut;

	private const BuildingCellVisualizer.Ports MATTER_PORTS = ~(BuildingCellVisualizer.Ports.PowerIn | BuildingCellVisualizer.Ports.PowerOut);

	private BuildingCellVisualizer.Ports ports;

	private BuildingCellVisualizer.Ports secondary_ports;

	private Sprite diseaseSourceSprite;

	private Color32 diseaseSourceColour;

	private GameObject inputVisualizer;

	private GameObject outputVisualizer;

	private GameObject secondaryInputVisualizer;

	private GameObject secondaryOutputVisualizer;

	private bool enableRaycast;

	private Dictionary<GameObject, Image> icons;

	[Flags]
	private enum Ports : byte
	{
		PowerIn = 1,
		PowerOut = 2,
		GasIn = 4,
		GasOut = 8,
		LiquidIn = 16,
		LiquidOut = 32,
		SolidIn = 64,
		SolidOut = 128
	}
}
