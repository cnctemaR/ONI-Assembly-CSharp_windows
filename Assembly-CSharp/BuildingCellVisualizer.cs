using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.UI;

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

	public bool RequiresGasOrLiquid
	{
		get
		{
			return this.requiresGasInput || this.requiresGasOutput || this.requiresLiquidInput || this.requiresLiquidOutput;
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
			Vector3 sound_pos = this.transform.position;
			EventInstance ev = SoundEvent.BeginOneShot(connectedReleaseSound, sound_pos);
			ev.setParameterValue("connectedCount", (float)connectionCount);
			SoundEvent.EndOneShot(ev);
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
		this.Subscribe(-235298596, new EventSystem.EventHandler(this.OnBuildingUpgraded));
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
		Component component = def.BuildingComplete.GetComponent<ElementFilter>();
		if (component != null)
		{
			this.requiresSecondGasOutput = component.GetComponent<Vent>().TransferType == Vent.Transfer.Gas;
			this.requiredSecondLiquidOutput = component.GetComponent<Vent>().TransferType == Vent.Transfer.Liquid;
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
		return BuildingCellVisualizer.CheckRequiresPowerInput(def) || BuildingCellVisualizer.CheckRequiresPowerOutput(def) || BuildingCellVisualizer.CheckRequiresGasInput(def) || BuildingCellVisualizer.CheckRequiresGasOutput(def) || BuildingCellVisualizer.CheckRequiresLiquidInput(def) || BuildingCellVisualizer.CheckRequiresLiquidOutput(def);
	}

	public static bool CheckRequiresPowerInput(BuildingDef def)
	{
		return def.RequiresPower;
	}

	public static bool CheckRequiresPowerOutput(BuildingDef def)
	{
		return def.GeneratorWattageRating > 0f;
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
		GameObject gameObject = Grid.Objects[cell, 6];
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

	private Dictionary<string, bool> CheckWireConnectors(int cell)
	{
		Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
		dictionary["Up"] = this.CompareWireConnection(Grid.CellAbove(cell), new UtilityConnections[]
		{
			UtilityConnections.Up,
			UtilityConnections.Up | UtilityConnections.Down
		});
		dictionary["Right"] = this.CompareWireConnection(Grid.CellRight(cell), new UtilityConnections[]
		{
			UtilityConnections.Right,
			UtilityConnections.Left | UtilityConnections.Right
		});
		dictionary["Down"] = this.CompareWireConnection(Grid.CellBelow(cell), new UtilityConnections[]
		{
			UtilityConnections.Down,
			UtilityConnections.Up | UtilityConnections.Down
		});
		dictionary["Left"] = this.CompareWireConnection(Grid.CellLeft(cell), new UtilityConnections[]
		{
			UtilityConnections.Left,
			UtilityConnections.Left | UtilityConnections.Right
		});
		return dictionary;
	}

	private Dictionary<string, bool> CheckConnectors(int cell, int objLayer)
	{
		Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
		dictionary["Up"] = Grid.Objects[Grid.CellAbove(cell), objLayer] != null;
		dictionary["Right"] = Grid.Objects[Grid.CellRight(cell), objLayer] != null;
		dictionary["Down"] = Grid.Objects[Grid.CellBelow(cell), objLayer] != null;
		dictionary["Left"] = Grid.Objects[Grid.CellLeft(cell), objLayer] != null;
		return dictionary;
	}

	private bool HasConnectionAround(int cell, int objLayer)
	{
		return Grid.Objects[Grid.CellAbove(cell), objLayer] != null || Grid.Objects[Grid.CellRight(cell), objLayer] != null || Grid.Objects[Grid.CellBelow(cell), objLayer] != null || Grid.Objects[Grid.CellLeft(cell), objLayer] != null;
	}

	private Color GetWireColor(int cell)
	{
		Color color = Color.white;
		GameObject gameObject = Grid.Objects[cell, 6];
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

	private void LateUpdate()
	{
		Building component = base.GetComponent<Building>();
		SimViewMode mode = OverlayScreen.Instance.GetMode();
		if (mode != this.previousMode)
		{
			this.DisableIcons();
		}
		SimViewMode simViewMode = mode;
		if (simViewMode != SimViewMode.LiquidVentMap)
		{
			if (simViewMode != SimViewMode.PowerMap)
			{
				if (simViewMode != SimViewMode.GasVentMap)
				{
					this.DisableIcons();
				}
				else if (this.requiresGasInput || this.requiresGasOutput || this.requiresSecondGasOutput)
				{
					if (this.requiresGasInput)
					{
						Sprite gasInputIcon = this.resources.gasInputIcon;
						bool flag = null != Grid.Objects[component.GetUtilityInputCell(), 10];
						BuildingCellVisualizerResources.ConnectedDisconnectedColours input = this.resources.gasIOColours.input;
						Color color = ((!flag) ? input.disconnected : input.connected);
						this.DrawUtilityIcon(component.GetUtilityInputCell(), gasInputIcon, ref this.inputVisualizer, color);
					}
					if (this.requiresGasOutput)
					{
						Sprite gasOutputIcon = this.resources.gasOutputIcon;
						bool flag2 = null != Grid.Objects[component.GetUtilityOutputCell(), 10];
						BuildingCellVisualizerResources.ConnectedDisconnectedColours output = this.resources.gasIOColours.output;
						Color color2 = ((!flag2) ? output.disconnected : output.connected);
						this.DrawUtilityIcon(component.GetUtilityOutputCell(), gasOutputIcon, ref this.outputVisualizer, color2);
					}
					if (this.requiresSecondGasOutput)
					{
						Sprite gasOutputIcon2 = this.resources.gasOutputIcon;
						int utilityOutputCell = component.GetUtilityOutputCell();
						CellOffset rotatedOffset = component.GetRotatedOffset(this.secondOutputOffset);
						int num = Grid.OffsetCell(utilityOutputCell, rotatedOffset);
						this.DrawUtilityIcon(num, gasOutputIcon2, ref this.secondaryOutputVisualizer, this.secondOutputColour, Color.white, null, 1.5f);
					}
				}
				else
				{
					this.DisableIcons();
				}
			}
			else if (this.requiresPowerInput || this.requiresPowerOutput)
			{
				int powerInputCell = component.GetPowerInputCell();
				Generator component2 = component.GetComponent<Generator>();
				EnergyConsumer component3 = component.GetComponent<EnergyConsumer>();
				bool flag3 = component.GetComponent<BuildingPreview>() != null;
				bool flag4 = (component2 != null && component2.HasWire) || (component3 != null && component3.HasWire);
				Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
				Sprite sprite = this.resources.electricityInputIcon;
				Color color3 = Color.white;
				BuildingEnabledButton component4 = component.GetComponent<BuildingEnabledButton>();
				ushort num2 = ushort.MaxValue;
				if (component3 != null)
				{
					num2 = component3.CircuitID;
				}
				else if (component2 != null)
				{
					num2 = component2.CircuitID;
				}
				if ((component4 != null && !component4.IsEnabled) || num2 == 65535)
				{
					color3 = Color.gray;
				}
				else if (component.Def.BuildingComplete.GetComponent<Generator>() != null && component.GetComponentInChildren<IEnergyConsumer>() == null)
				{
					color3 = this.resources.electricityOutputColor;
				}
				if (!flag3)
				{
					if (flag4)
					{
						if (this.HasConnectionAround(powerInputCell, 6))
						{
							sprite = this.resources.electricityConnectedIcon;
						}
						dictionary = this.CheckWireConnectors(powerInputCell);
					}
					else
					{
						dictionary["Up"] = false;
						dictionary["Right"] = false;
						dictionary["Down"] = false;
						dictionary["Left"] = false;
					}
				}
				if (this.requiresPowerInput)
				{
					this.DrawUtilityIcon(powerInputCell, sprite, ref this.inputVisualizer, color3, this.GetWireColor(powerInputCell), dictionary, 1f);
				}
				if (this.requiresPowerOutput)
				{
					this.DrawUtilityIcon(powerInputCell, sprite, ref this.outputVisualizer, color3, this.GetWireColor(powerInputCell), dictionary, 1f);
				}
			}
			else
			{
				this.DisableIcons();
			}
		}
		else if (this.requiresLiquidInput || this.requiresLiquidOutput || this.requiredSecondLiquidOutput)
		{
			if (this.requiresLiquidInput)
			{
				bool flag5 = null != Grid.Objects[component.GetUtilityInputCell(), 12];
				BuildingCellVisualizerResources.ConnectedDisconnectedColours input2 = this.resources.liquidIOColours.input;
				Color color4 = ((!flag5) ? input2.disconnected : input2.connected);
				this.DrawUtilityIcon(component.GetUtilityInputCell(), this.resources.liquidInputIcon, ref this.inputVisualizer, color4);
			}
			if (this.requiresLiquidOutput)
			{
				bool flag6 = null != Grid.Objects[component.GetUtilityOutputCell(), 12];
				BuildingCellVisualizerResources.ConnectedDisconnectedColours output2 = this.resources.liquidIOColours.output;
				Color color5 = ((!flag6) ? output2.disconnected : output2.connected);
				this.DrawUtilityIcon(component.GetUtilityOutputCell(), this.resources.liquidOutputIcon, ref this.outputVisualizer, color5);
			}
			if (this.requiredSecondLiquidOutput)
			{
				int utilityOutputCell2 = component.GetUtilityOutputCell();
				CellOffset rotatedOffset2 = component.GetRotatedOffset(this.secondOutputOffset);
				int num3 = Grid.OffsetCell(utilityOutputCell2, rotatedOffset2);
				this.DrawUtilityIcon(num3, this.resources.liquidOutputIcon, ref this.secondaryOutputVisualizer, this.secondOutputColour, Color.white, null, 1.5f);
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
		this.DrawUtilityIcon(cell, icon_img, ref visualizerObj, Color.white, Color.white, null, 1.5f);
	}

	private void DrawUtilityIcon(int cell, Sprite icon_img, ref GameObject visualizerObj, Color tint)
	{
		this.DrawUtilityIcon(cell, icon_img, ref visualizerObj, tint, Color.white, null, 1.5f);
	}

	private void DrawUtilityIcon(int cell, Sprite icon_img, ref GameObject visualizerObj, Color tint, Color connectorColor, Dictionary<string, bool> connectors = null, float scaleMultiplier = 1.5f)
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
		this.icons[visualizerObj].raycastTarget = this.enableRaycast;
		this.icons[visualizerObj].sprite = icon_img;
		Transform child = visualizerObj.transform.GetChild(0);
		Image component = child.gameObject.GetComponent<Image>();
		component.color = tint;
		visualizerObj.transform.SetPosition(vector);
		if (visualizerObj.GetComponent<SizePulse>() == null)
		{
			visualizerObj.transform.localScale = Vector3.one * scaleMultiplier;
		}
		if (connectors != null)
		{
			foreach (KeyValuePair<string, bool> keyValuePair in connectors)
			{
				Transform transform = visualizerObj.transform.FindChild("Connector_" + keyValuePair.Key);
				if (transform != null && transform.gameObject.activeInHierarchy != keyValuePair.Value)
				{
					transform.gameObject.SetActive(keyValuePair.Value);
				}
			}
			connectorColor.a = 0.85f;
			this.SetAllConnectorsTint(connectorColor);
		}
	}

	public void SetAllConnectorsTint(Color tint)
	{
		this.SetConnectorTint(this.inputVisualizer, tint);
		this.SetConnectorTint(this.outputVisualizer, tint);
		this.SetConnectorTint(this.secondaryOutputVisualizer, tint);
	}

	private void SetConnectorTint(GameObject connectorObj, Color tint)
	{
		if (connectorObj == null)
		{
			return;
		}
		List<Image> list = new List<Image>(connectorObj.GetComponentsInChildren<Image>());
		list.RemoveAll((Image img) => !img.name.Contains("Connector"));
		list.ForEach(delegate(Image img)
		{
			img.color = tint;
		});
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
	public Color32 secondOutputColour = new Color(0.5294118f, 0.27058825f, 0.4f);

	private bool requiresPowerInput;

	private bool requiresPowerOutput;

	private bool requiresGasInput;

	private bool requiresGasOutput;

	private bool requiresLiquidInput;

	private bool requiresLiquidOutput;

	private bool requiresSecondGasOutput;

	private bool requiredSecondLiquidOutput;

	private GameObject inputVisualizer;

	private GameObject outputVisualizer;

	private GameObject secondaryOutputVisualizer;

	private bool enableRaycast;

	private Dictionary<GameObject, Image> icons;

	private SimViewMode previousMode;
}
