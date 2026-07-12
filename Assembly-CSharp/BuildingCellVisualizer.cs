using System;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/BuildingCellVisualizer")]
public class BuildingCellVisualizer : EntityCellVisualizer
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void LoadDiseaseIcon()
	{
		DiseaseVisualization.Info info = Assets.instance.DiseaseVisualization.GetInfo(this.building.Def.DiseaseCellVisName);
		if (info.name != null)
		{
			this.diseaseSourceSprite = Assets.instance.DiseaseVisualization.overlaySprite;
			this.diseaseSourceColour = GlobalAssets.Instance.colorSet.GetColorByName(info.overlayColourName);
		}
	}

	protected override void DefinePorts()
	{
		BuildingDef def = this.building.Def;
		if (def.CheckRequiresPowerInput())
		{
			this.AddPort(EntityCellVisualizer.Ports.PowerIn, this.building.Def.PowerInputOffset, base.Resources.electricityInputColor, Color.gray, 1f, false);
		}
		if (def.CheckRequiresPowerOutput())
		{
			this.AddPort(EntityCellVisualizer.Ports.PowerOut, this.building.Def.PowerOutputOffset, this.building.Def.UseWhitePowerOutputConnectorColour ? base.Resources.electricityInputColor : base.Resources.electricityOutputColor, Color.gray, 1f, false);
		}
		if (def.CheckRequiresGasInput())
		{
			this.AddPort(EntityCellVisualizer.Ports.GasIn, this.building.Def.UtilityInputOffset, base.Resources.gasIOColours.input.connected, base.Resources.gasIOColours.input.disconnected, 1.5f, false);
		}
		if (def.CheckRequiresGasOutput())
		{
			this.AddPort(EntityCellVisualizer.Ports.GasOut, this.building.Def.UtilityOutputOffset, base.Resources.gasIOColours.output.connected, base.Resources.gasIOColours.output.disconnected, 1.5f, false);
		}
		if (def.CheckRequiresLiquidInput())
		{
			this.AddPort(EntityCellVisualizer.Ports.LiquidIn, this.building.Def.UtilityInputOffset, base.Resources.liquidIOColours.input.connected, base.Resources.liquidIOColours.input.disconnected, 1.5f, false);
		}
		if (def.CheckRequiresLiquidOutput())
		{
			this.AddPort(EntityCellVisualizer.Ports.LiquidOut, this.building.Def.UtilityOutputOffset, base.Resources.liquidIOColours.output.connected, base.Resources.liquidIOColours.output.disconnected, 1.5f, false);
		}
		if (def.CheckRequiresSolidInput())
		{
			this.AddPort(EntityCellVisualizer.Ports.SolidIn, this.building.Def.UtilityInputOffset, base.Resources.liquidIOColours.input.connected, base.Resources.liquidIOColours.input.disconnected, 1.5f, false);
		}
		if (def.CheckRequiresSolidOutput())
		{
			this.AddPort(EntityCellVisualizer.Ports.SolidOut, this.building.Def.UtilityOutputOffset, base.Resources.liquidIOColours.output.connected, base.Resources.liquidIOColours.output.disconnected, 1.5f, false);
		}
		if (def.CheckRequiresHighEnergyParticleInput())
		{
			this.AddPort(EntityCellVisualizer.Ports.HighEnergyParticleIn, this.building.Def.HighEnergyParticleInputOffset, base.Resources.highEnergyParticleInputColour, Color.white, 3f, false);
		}
		if (def.CheckRequiresHighEnergyParticleOutput())
		{
			this.AddPort(EntityCellVisualizer.Ports.HighEnergyParticleOut, this.building.Def.HighEnergyParticleOutputOffset, base.Resources.highEnergyParticleOutputColour, Color.white, 3f, false);
		}
		if (def.SelfHeatKilowattsWhenActive > 0f || def.ExhaustKilowattsWhenActive > 0f)
		{
			this.AddPort(EntityCellVisualizer.Ports.HeatSource, default(CellOffset));
		}
		if (def.SelfHeatKilowattsWhenActive < 0f || def.ExhaustKilowattsWhenActive < 0f)
		{
			this.AddPort(EntityCellVisualizer.Ports.HeatSink, default(CellOffset));
		}
		if (this.diseaseSourceSprite != null)
		{
			this.AddPort(EntityCellVisualizer.Ports.DiseaseOut, this.building.Def.UtilityOutputOffset, this.diseaseSourceColour);
		}
		foreach (ISecondaryInput secondaryInput in def.BuildingComplete.GetComponents<ISecondaryInput>())
		{
			if (secondaryInput != null)
			{
				if (secondaryInput.HasSecondaryConduitType(ConduitType.Gas))
				{
					BuildingCellVisualizerResources.ConnectedDisconnectedColours connectedDisconnectedColours = (def.CheckRequiresGasInput() ? base.Resources.alternateIOColours.input : base.Resources.gasIOColours.input);
					this.AddPort(EntityCellVisualizer.Ports.GasIn, secondaryInput.GetSecondaryConduitOffset(ConduitType.Gas), connectedDisconnectedColours.connected, connectedDisconnectedColours.disconnected, 1.5f, false);
				}
				if (secondaryInput.HasSecondaryConduitType(ConduitType.Liquid))
				{
					if (!def.CheckRequiresLiquidInput())
					{
						BuildingCellVisualizerResources.IOColours liquidIOColours = base.Resources.liquidIOColours;
					}
					else
					{
						BuildingCellVisualizerResources.IOColours alternateIOColours = base.Resources.alternateIOColours;
					}
					this.AddPort(EntityCellVisualizer.Ports.LiquidIn, secondaryInput.GetSecondaryConduitOffset(ConduitType.Liquid));
				}
				if (secondaryInput.HasSecondaryConduitType(ConduitType.Solid))
				{
					if (!def.CheckRequiresSolidInput())
					{
						BuildingCellVisualizerResources.IOColours liquidIOColours2 = base.Resources.liquidIOColours;
					}
					else
					{
						BuildingCellVisualizerResources.IOColours alternateIOColours2 = base.Resources.alternateIOColours;
					}
					this.AddPort(EntityCellVisualizer.Ports.SolidIn, secondaryInput.GetSecondaryConduitOffset(ConduitType.Solid));
				}
			}
		}
		foreach (ISecondaryOutput secondaryOutput in def.BuildingComplete.GetComponents<ISecondaryOutput>())
		{
			if (secondaryOutput != null)
			{
				if (secondaryOutput.HasSecondaryConduitType(ConduitType.Gas))
				{
					BuildingCellVisualizerResources.ConnectedDisconnectedColours connectedDisconnectedColours2 = (def.CheckRequiresGasOutput() ? base.Resources.alternateIOColours.output : base.Resources.gasIOColours.output);
					this.AddPort(EntityCellVisualizer.Ports.GasOut, secondaryOutput.GetSecondaryConduitOffset(ConduitType.Gas), connectedDisconnectedColours2.connected, connectedDisconnectedColours2.disconnected, 1.5f, false);
				}
				if (secondaryOutput.HasSecondaryConduitType(ConduitType.Liquid))
				{
					BuildingCellVisualizerResources.ConnectedDisconnectedColours connectedDisconnectedColours3 = (def.CheckRequiresLiquidOutput() ? base.Resources.alternateIOColours.output : base.Resources.liquidIOColours.output);
					this.AddPort(EntityCellVisualizer.Ports.LiquidOut, secondaryOutput.GetSecondaryConduitOffset(ConduitType.Liquid), connectedDisconnectedColours3.connected, connectedDisconnectedColours3.disconnected, 1.5f, false);
				}
				if (secondaryOutput.HasSecondaryConduitType(ConduitType.Solid))
				{
					BuildingCellVisualizerResources.ConnectedDisconnectedColours connectedDisconnectedColours4 = (def.CheckRequiresSolidOutput() ? base.Resources.alternateIOColours.output : base.Resources.liquidIOColours.output);
					this.AddPort(EntityCellVisualizer.Ports.SolidOut, secondaryOutput.GetSecondaryConduitOffset(ConduitType.Solid), connectedDisconnectedColours4.connected, connectedDisconnectedColours4.disconnected, 1.5f, false);
				}
			}
		}
	}

	protected override void OnCmpEnable()
	{
		this.enableRaycast = this.building as BuildingComplete != null;
		base.OnCmpEnable();
	}

	[MyCmpReq]
	private Building building;
}
