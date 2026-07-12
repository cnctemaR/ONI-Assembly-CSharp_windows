using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.UI;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/BuildingCellVisualizer")]
public class BuildingCellVisualizer : KMonoBehaviour
{
	public bool RequiresPowerInput
	{
		get
		{
			return (this.ports & BuildingCellVisualizer.Ports.PowerIn) > (BuildingCellVisualizer.Ports)0;
		}
	}

	public bool RequiresPowerOutput
	{
		get
		{
			return (this.ports & BuildingCellVisualizer.Ports.PowerOut) > (BuildingCellVisualizer.Ports)0;
		}
	}

	public bool RequiresPower
	{
		get
		{
			return (this.ports & (BuildingCellVisualizer.Ports.PowerIn | BuildingCellVisualizer.Ports.PowerOut)) > (BuildingCellVisualizer.Ports)0;
		}
	}

	public bool RequiresGas
	{
		get
		{
			return (this.ports & (BuildingCellVisualizer.Ports.GasIn | BuildingCellVisualizer.Ports.GasOut)) > (BuildingCellVisualizer.Ports)0;
		}
	}

	public bool RequiresLiquid
	{
		get
		{
			return (this.ports & (BuildingCellVisualizer.Ports.LiquidIn | BuildingCellVisualizer.Ports.LiquidOut)) > (BuildingCellVisualizer.Ports)0;
		}
	}

	public bool RequiresSolid
	{
		get
		{
			return (this.ports & (BuildingCellVisualizer.Ports.SolidIn | BuildingCellVisualizer.Ports.SolidOut)) > (BuildingCellVisualizer.Ports)0;
		}
	}

	public bool RequiresUtilityConnection
	{
		get
		{
			return (this.ports & (BuildingCellVisualizer.Ports.GasIn | BuildingCellVisualizer.Ports.GasOut | BuildingCellVisualizer.Ports.LiquidIn | BuildingCellVisualizer.Ports.LiquidOut | BuildingCellVisualizer.Ports.SolidIn | BuildingCellVisualizer.Ports.SolidOut)) > (BuildingCellVisualizer.Ports)0;
		}
	}

	public bool RequiresHighEnergyParticles
	{
		get
		{
			return (this.ports & BuildingCellVisualizer.Ports.HighEnergyParticle) > (BuildingCellVisualizer.Ports)0;
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
		string sound = GlobalAssets.GetSound(soundName, false);
		if (sound != null)
		{
			Vector3 position = base.transform.GetPosition();
			position.z = 0f;
			EventInstance eventInstance = SoundEvent.BeginOneShot(sound, position, 1f, false);
			eventInstance.setParameterByName("connectedCount", (float)connectionCount, false);
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
		if (def.CheckRequiresHighEnergyParticleInput())
		{
			this.ports |= BuildingCellVisualizer.Ports.HighEnergyParticle;
		}
		if (def.CheckRequiresHighEnergyParticleOutput())
		{
			this.ports |= BuildingCellVisualizer.Ports.HighEnergyParticle;
		}
		DiseaseVisualization.Info info = Assets.instance.DiseaseVisualization.GetInfo(def.DiseaseCellVisName);
		if (info.name != null)
		{
			this.diseaseSourceSprite = Assets.instance.DiseaseVisualization.overlaySprite;
			this.diseaseSourceColour = GlobalAssets.Instance.colorSet.GetColorByName(info.overlayColourName);
		}
		foreach (ISecondaryInput secondaryInput in def.BuildingComplete.GetComponents<ISecondaryInput>())
		{
			if (secondaryInput != null)
			{
				if (secondaryInput.HasSecondaryConduitType(ConduitType.Gas))
				{
					this.secondary_ports |= BuildingCellVisualizer.Ports.GasIn;
				}
				if (secondaryInput.HasSecondaryConduitType(ConduitType.Liquid))
				{
					this.secondary_ports |= BuildingCellVisualizer.Ports.LiquidIn;
				}
				if (secondaryInput.HasSecondaryConduitType(ConduitType.Solid))
				{
					this.secondary_ports |= BuildingCellVisualizer.Ports.SolidIn;
				}
			}
		}
		foreach (ISecondaryOutput secondaryOutput in def.BuildingComplete.GetComponents<ISecondaryOutput>())
		{
			if (secondaryOutput != null)
			{
				if (secondaryOutput.HasSecondaryConduitType(ConduitType.Gas))
				{
					this.secondary_ports |= BuildingCellVisualizer.Ports.GasOut;
				}
				if (secondaryOutput.HasSecondaryConduitType(ConduitType.Liquid))
				{
					this.secondary_ports |= BuildingCellVisualizer.Ports.LiquidOut;
				}
				if (secondaryOutput.HasSecondaryConduitType(ConduitType.Solid))
				{
					this.secondary_ports |= BuildingCellVisualizer.Ports.SolidOut;
				}
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
		if (!(component != null))
		{
			return Color.white;
		}
		return component.TintColour;
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		if (this.resources == null)
		{
			this.resources = BuildingCellVisualizerResources.Instance();
		}
		if (this.icons == null)
		{
			this.icons = new Dictionary<GameObject, Image>();
		}
		this.enableRaycast = this.building as BuildingComplete != null;
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
		if (base.gameObject.GetMyWorldId() != ClusterManager.Instance.activeWorldId)
		{
			this.DisableIcons();
			return;
		}
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
					Color color = ((component != null && !component.IsEnabled) ? Color.gray : Color.white);
					Sprite sprite = ((!flag && circuitID != 65535) ? this.resources.electricityConnectedIcon : this.resources.electricityInputIcon);
					this.DrawUtilityIcon(powerInputCell, sprite, ref this.inputVisualizer, color, this.GetWireColor(powerInputCell), 1f, false);
				}
				if (this.RequiresPowerOutput)
				{
					int powerOutputCell = this.building.GetPowerOutputCell();
					int circuitID2 = (int)Game.Instance.circuitManager.GetCircuitID(powerOutputCell);
					Color color2 = (this.building.Def.UseWhitePowerOutputConnectorColour ? Color.white : this.resources.electricityOutputColor);
					Color32 color3 = ((component != null && !component.IsEnabled) ? Color.gray : color2);
					Sprite sprite2 = ((!flag && circuitID2 != 65535) ? this.resources.electricityConnectedIcon : this.resources.electricityInputIcon);
					this.DrawUtilityIcon(powerOutputCell, sprite2, ref this.outputVisualizer, color3, this.GetWireColor(powerOutputCell), 1f, false);
					return;
				}
			}
			else
			{
				bool flag2 = true;
				Switch component2 = base.GetComponent<Switch>();
				if (component2 != null)
				{
					int num = Grid.PosToCell(base.transform.GetPosition());
					Color32 color4 = (component2.IsHandlerOn() ? this.resources.switchColor : this.resources.switchOffColor);
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
						this.DrawUtilityIcon(num2, (Game.Instance.circuitManager.GetCircuitID(num2) == ushort.MaxValue) ? this.resources.electricityBridgeIcon : this.resources.electricityConnectedIcon, ref this.inputVisualizer, this.resources.electricityInputColor, Color.white, 1f, false);
						this.DrawUtilityIcon(num3, (Game.Instance.circuitManager.GetCircuitID(num3) == ushort.MaxValue) ? this.resources.electricityBridgeIcon : this.resources.electricityConnectedIcon, ref this.outputVisualizer, this.resources.electricityInputColor, Color.white, 1f, false);
						flag2 = false;
					}
				}
				if (flag2)
				{
					this.DisableIcons();
					return;
				}
			}
		}
		else if (mode == OverlayModes.GasConduits.ID)
		{
			if (!this.RequiresGas && (this.secondary_ports & (BuildingCellVisualizer.Ports.GasIn | BuildingCellVisualizer.Ports.GasOut)) == (BuildingCellVisualizer.Ports)0)
			{
				this.DisableIcons();
				return;
			}
			if ((this.ports & BuildingCellVisualizer.Ports.GasIn) != (BuildingCellVisualizer.Ports)0)
			{
				bool flag3 = null != Grid.Objects[this.building.GetUtilityInputCell(), 12];
				BuildingCellVisualizerResources.ConnectedDisconnectedColours input = this.resources.gasIOColours.input;
				Color color5 = (flag3 ? input.connected : input.disconnected);
				this.DrawUtilityIcon(this.building.GetUtilityInputCell(), this.resources.gasInputIcon, ref this.inputVisualizer, color5);
			}
			if ((this.ports & BuildingCellVisualizer.Ports.GasOut) != (BuildingCellVisualizer.Ports)0)
			{
				bool flag4 = null != Grid.Objects[this.building.GetUtilityOutputCell(), 12];
				BuildingCellVisualizerResources.ConnectedDisconnectedColours output = this.resources.gasIOColours.output;
				Color color6 = (flag4 ? output.connected : output.disconnected);
				this.DrawUtilityIcon(this.building.GetUtilityOutputCell(), this.resources.gasOutputIcon, ref this.outputVisualizer, color6);
			}
			if ((this.secondary_ports & BuildingCellVisualizer.Ports.GasIn) != (BuildingCellVisualizer.Ports)0)
			{
				ISecondaryInput[] components = this.building.GetComponents<ISecondaryInput>();
				CellOffset cellOffset = CellOffset.none;
				ISecondaryInput[] array = components;
				for (int i = 0; i < array.Length; i++)
				{
					cellOffset = array[i].GetSecondaryConduitOffset(ConduitType.Gas);
					if (cellOffset != CellOffset.none)
					{
						break;
					}
				}
				Color color7 = BuildingCellVisualizer.secondInputColour;
				if ((this.ports & BuildingCellVisualizer.Ports.GasIn) == (BuildingCellVisualizer.Ports)0)
				{
					bool flag5 = null != Grid.Objects[Grid.OffsetCell(Grid.PosToCell(this.building.transform.GetPosition()), cellOffset), 12];
					BuildingCellVisualizerResources.ConnectedDisconnectedColours input2 = this.resources.gasIOColours.input;
					color7 = (flag5 ? input2.connected : input2.disconnected);
				}
				int visualizerCell = this.GetVisualizerCell(this.building, cellOffset);
				this.DrawUtilityIcon(visualizerCell, this.resources.gasInputIcon, ref this.secondaryInputVisualizer, color7, Color.white, 1.5f, false);
			}
			if ((this.secondary_ports & BuildingCellVisualizer.Ports.GasOut) != (BuildingCellVisualizer.Ports)0)
			{
				ISecondaryOutput[] components2 = this.building.GetComponents<ISecondaryOutput>();
				CellOffset cellOffset2 = CellOffset.none;
				ISecondaryOutput[] array2 = components2;
				for (int i = 0; i < array2.Length; i++)
				{
					cellOffset2 = array2[i].GetSecondaryConduitOffset(ConduitType.Gas);
					if (cellOffset2 != CellOffset.none)
					{
						break;
					}
				}
				Color color8 = BuildingCellVisualizer.secondOutputColour;
				if ((this.ports & BuildingCellVisualizer.Ports.GasOut) == (BuildingCellVisualizer.Ports)0)
				{
					bool flag6 = null != Grid.Objects[Grid.OffsetCell(Grid.PosToCell(this.building.transform.GetPosition()), cellOffset2), 12];
					BuildingCellVisualizerResources.ConnectedDisconnectedColours output2 = this.resources.gasIOColours.output;
					color8 = (flag6 ? output2.connected : output2.disconnected);
				}
				int visualizerCell2 = this.GetVisualizerCell(this.building, cellOffset2);
				this.DrawUtilityIcon(visualizerCell2, this.resources.gasOutputIcon, ref this.secondaryOutputVisualizer, color8, Color.white, 1.5f, false);
				return;
			}
		}
		else if (mode == OverlayModes.LiquidConduits.ID)
		{
			if (!this.RequiresLiquid && (this.secondary_ports & (BuildingCellVisualizer.Ports.LiquidIn | BuildingCellVisualizer.Ports.LiquidOut)) == (BuildingCellVisualizer.Ports)0)
			{
				this.DisableIcons();
				return;
			}
			if ((this.ports & BuildingCellVisualizer.Ports.LiquidIn) != (BuildingCellVisualizer.Ports)0)
			{
				bool flag7 = null != Grid.Objects[this.building.GetUtilityInputCell(), 16];
				BuildingCellVisualizerResources.ConnectedDisconnectedColours input3 = this.resources.liquidIOColours.input;
				Color color9 = (flag7 ? input3.connected : input3.disconnected);
				this.DrawUtilityIcon(this.building.GetUtilityInputCell(), this.resources.liquidInputIcon, ref this.inputVisualizer, color9);
			}
			if ((this.ports & BuildingCellVisualizer.Ports.LiquidOut) != (BuildingCellVisualizer.Ports)0)
			{
				bool flag8 = null != Grid.Objects[this.building.GetUtilityOutputCell(), 16];
				BuildingCellVisualizerResources.ConnectedDisconnectedColours output3 = this.resources.liquidIOColours.output;
				Color color10 = (flag8 ? output3.connected : output3.disconnected);
				this.DrawUtilityIcon(this.building.GetUtilityOutputCell(), this.resources.liquidOutputIcon, ref this.outputVisualizer, color10);
			}
			if ((this.secondary_ports & BuildingCellVisualizer.Ports.LiquidIn) != (BuildingCellVisualizer.Ports)0)
			{
				ISecondaryInput[] components3 = this.building.GetComponents<ISecondaryInput>();
				CellOffset cellOffset3 = CellOffset.none;
				ISecondaryInput[] array = components3;
				for (int i = 0; i < array.Length; i++)
				{
					cellOffset3 = array[i].GetSecondaryConduitOffset(ConduitType.Liquid);
					if (cellOffset3 != CellOffset.none)
					{
						break;
					}
				}
				Color color11 = BuildingCellVisualizer.secondInputColour;
				if ((this.ports & BuildingCellVisualizer.Ports.LiquidIn) == (BuildingCellVisualizer.Ports)0)
				{
					bool flag9 = null != Grid.Objects[Grid.OffsetCell(Grid.PosToCell(this.building.transform.GetPosition()), cellOffset3), 16];
					BuildingCellVisualizerResources.ConnectedDisconnectedColours input4 = this.resources.liquidIOColours.input;
					color11 = (flag9 ? input4.connected : input4.disconnected);
				}
				int visualizerCell3 = this.GetVisualizerCell(this.building, cellOffset3);
				this.DrawUtilityIcon(visualizerCell3, this.resources.liquidInputIcon, ref this.secondaryInputVisualizer, color11, Color.white, 1.5f, false);
			}
			if ((this.secondary_ports & BuildingCellVisualizer.Ports.LiquidOut) != (BuildingCellVisualizer.Ports)0)
			{
				ISecondaryOutput[] components4 = this.building.GetComponents<ISecondaryOutput>();
				CellOffset cellOffset4 = CellOffset.none;
				ISecondaryOutput[] array2 = components4;
				for (int i = 0; i < array2.Length; i++)
				{
					cellOffset4 = array2[i].GetSecondaryConduitOffset(ConduitType.Liquid);
					if (cellOffset4 != CellOffset.none)
					{
						break;
					}
				}
				Color color12 = BuildingCellVisualizer.secondOutputColour;
				if ((this.ports & BuildingCellVisualizer.Ports.LiquidOut) == (BuildingCellVisualizer.Ports)0)
				{
					bool flag10 = null != Grid.Objects[Grid.OffsetCell(Grid.PosToCell(this.building.transform.GetPosition()), cellOffset4), 16];
					BuildingCellVisualizerResources.ConnectedDisconnectedColours output4 = this.resources.liquidIOColours.output;
					color12 = (flag10 ? output4.connected : output4.disconnected);
				}
				int visualizerCell4 = this.GetVisualizerCell(this.building, cellOffset4);
				this.DrawUtilityIcon(visualizerCell4, this.resources.liquidOutputIcon, ref this.secondaryOutputVisualizer, color12, Color.white, 1.5f, false);
				return;
			}
		}
		else if (mode == OverlayModes.SolidConveyor.ID)
		{
			if (!this.RequiresSolid && (this.secondary_ports & (BuildingCellVisualizer.Ports.SolidIn | BuildingCellVisualizer.Ports.SolidOut)) == (BuildingCellVisualizer.Ports)0)
			{
				this.DisableIcons();
				return;
			}
			if ((this.ports & BuildingCellVisualizer.Ports.SolidIn) != (BuildingCellVisualizer.Ports)0)
			{
				bool flag11 = null != Grid.Objects[this.building.GetUtilityInputCell(), 20];
				BuildingCellVisualizerResources.ConnectedDisconnectedColours input5 = this.resources.liquidIOColours.input;
				Color color13 = (flag11 ? input5.connected : input5.disconnected);
				this.DrawUtilityIcon(this.building.GetUtilityInputCell(), this.resources.liquidInputIcon, ref this.inputVisualizer, color13);
			}
			if ((this.ports & BuildingCellVisualizer.Ports.SolidOut) != (BuildingCellVisualizer.Ports)0)
			{
				bool flag12 = null != Grid.Objects[this.building.GetUtilityOutputCell(), 20];
				BuildingCellVisualizerResources.ConnectedDisconnectedColours output5 = this.resources.liquidIOColours.output;
				Color color14 = (flag12 ? output5.connected : output5.disconnected);
				this.DrawUtilityIcon(this.building.GetUtilityOutputCell(), this.resources.liquidOutputIcon, ref this.outputVisualizer, color14);
			}
			if ((this.secondary_ports & BuildingCellVisualizer.Ports.SolidIn) != (BuildingCellVisualizer.Ports)0)
			{
				ISecondaryInput[] components5 = this.building.GetComponents<ISecondaryInput>();
				CellOffset cellOffset5 = CellOffset.none;
				ISecondaryInput[] array = components5;
				for (int i = 0; i < array.Length; i++)
				{
					cellOffset5 = array[i].GetSecondaryConduitOffset(ConduitType.Solid);
					if (cellOffset5 != CellOffset.none)
					{
						break;
					}
				}
				Color color15 = BuildingCellVisualizer.secondInputColour;
				if ((this.ports & BuildingCellVisualizer.Ports.SolidIn) == (BuildingCellVisualizer.Ports)0)
				{
					bool flag13 = null != Grid.Objects[Grid.OffsetCell(Grid.PosToCell(this.building.transform.GetPosition()), cellOffset5), 20];
					BuildingCellVisualizerResources.ConnectedDisconnectedColours input6 = this.resources.liquidIOColours.input;
					color15 = (flag13 ? input6.connected : input6.disconnected);
				}
				int visualizerCell5 = this.GetVisualizerCell(this.building, cellOffset5);
				this.DrawUtilityIcon(visualizerCell5, this.resources.liquidInputIcon, ref this.secondaryInputVisualizer, color15, Color.white, 1.5f, false);
			}
			if ((this.secondary_ports & BuildingCellVisualizer.Ports.SolidOut) != (BuildingCellVisualizer.Ports)0)
			{
				ISecondaryOutput[] components6 = this.building.GetComponents<ISecondaryOutput>();
				CellOffset cellOffset6 = CellOffset.none;
				ISecondaryOutput[] array2 = components6;
				for (int i = 0; i < array2.Length; i++)
				{
					cellOffset6 = array2[i].GetSecondaryConduitOffset(ConduitType.Solid);
					if (cellOffset6 != CellOffset.none)
					{
						break;
					}
				}
				Color color16 = BuildingCellVisualizer.secondOutputColour;
				if ((this.ports & BuildingCellVisualizer.Ports.SolidOut) == (BuildingCellVisualizer.Ports)0)
				{
					bool flag14 = null != Grid.Objects[Grid.OffsetCell(Grid.PosToCell(this.building.transform.GetPosition()), cellOffset6), 20];
					BuildingCellVisualizerResources.ConnectedDisconnectedColours output6 = this.resources.liquidIOColours.output;
					color16 = (flag14 ? output6.connected : output6.disconnected);
				}
				int visualizerCell6 = this.GetVisualizerCell(this.building, cellOffset6);
				this.DrawUtilityIcon(visualizerCell6, this.resources.liquidOutputIcon, ref this.secondaryOutputVisualizer, color16, Color.white, 1.5f, false);
				return;
			}
		}
		else if (mode == OverlayModes.Disease.ID)
		{
			if (this.diseaseSourceSprite != null)
			{
				int utilityOutputCell = this.building.GetUtilityOutputCell();
				this.DrawUtilityIcon(utilityOutputCell, this.diseaseSourceSprite, ref this.inputVisualizer, this.diseaseSourceColour);
				return;
			}
		}
		else if (mode == OverlayModes.Radiation.ID && this.RequiresHighEnergyParticles)
		{
			int num4 = 3;
			if (this.building.Def.UseHighEnergyParticleInputPort)
			{
				int highEnergyParticleInputCell = this.building.GetHighEnergyParticleInputCell();
				this.DrawUtilityIcon(highEnergyParticleInputCell, this.resources.highEnergyParticleInputIcon, ref this.inputVisualizer, this.resources.highEnergyParticleInputColour, Color.white, (float)num4, true);
			}
			if (this.building.Def.UseHighEnergyParticleOutputPort)
			{
				int highEnergyParticleOutputCell = this.building.GetHighEnergyParticleOutputCell();
				IHighEnergyParticleDirection component4 = this.building.GetComponent<IHighEnergyParticleDirection>();
				Sprite sprite3 = this.resources.highEnergyParticleOutputIcons[0];
				if (component4 != null)
				{
					int directionIndex = EightDirectionUtil.GetDirectionIndex(component4.Direction);
					sprite3 = this.resources.highEnergyParticleOutputIcons[directionIndex];
				}
				this.DrawUtilityIcon(highEnergyParticleOutputCell, sprite3, ref this.outputVisualizer, this.resources.highEnergyParticleOutputColour, Color.white, (float)num4, true);
			}
		}
	}

	private int GetVisualizerCell(Building building, CellOffset offset)
	{
		CellOffset rotatedOffset = building.GetRotatedOffset(offset);
		return Grid.OffsetCell(building.GetCell(), rotatedOffset);
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
		visualizerObj.GetComponent<Image>().enabled = !hideBG;
		this.icons[visualizerObj].raycastTarget = this.enableRaycast;
		this.icons[visualizerObj].sprite = icon_img;
		visualizerObj.transform.GetChild(0).gameObject.GetComponent<Image>().color = tint;
		visualizerObj.transform.SetPosition(vector);
		if (visualizerObj.GetComponent<SizePulse>() == null)
		{
			visualizerObj.transform.localScale = Vector3.one * scaleMultiplier;
		}
	}

	public Image GetOutputIcon()
	{
		if (!(this.outputVisualizer == null))
		{
			return this.outputVisualizer.transform.GetChild(0).GetComponent<Image>();
		}
		return null;
	}

	public Image GetInputIcon()
	{
		if (!(this.inputVisualizer == null))
		{
			return this.inputVisualizer.transform.GetChild(0).GetComponent<Image>();
		}
		return null;
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

	private const BuildingCellVisualizer.Ports MATTER_PORTS = BuildingCellVisualizer.Ports.GasIn | BuildingCellVisualizer.Ports.GasOut | BuildingCellVisualizer.Ports.LiquidIn | BuildingCellVisualizer.Ports.LiquidOut | BuildingCellVisualizer.Ports.SolidIn | BuildingCellVisualizer.Ports.SolidOut;

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
	private enum Ports
	{
		PowerIn = 1,
		PowerOut = 2,
		GasIn = 4,
		GasOut = 8,
		LiquidIn = 16,
		LiquidOut = 32,
		SolidIn = 64,
		SolidOut = 128,
		HighEnergyParticle = 256
	}
}
