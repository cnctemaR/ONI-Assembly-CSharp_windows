using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class RocketSimpleInfoPanel : SimpleInfoPanel
{
	public RocketSimpleInfoPanel(SimpleInfoScreen simpleInfoScreen)
		: base(simpleInfoScreen)
	{
	}

	public override void Refresh(CollapsibleDetailContentPanel rocketStatusContainer, GameObject selectedTarget)
	{
		if (selectedTarget == null)
		{
			this.simpleInfoRoot.StoragePanel.gameObject.SetActive(false);
			return;
		}
		RocketModuleCluster rocketModuleCluster = null;
		Clustercraft clustercraft = null;
		CraftModuleInterface craftModuleInterface = null;
		RocketSimpleInfoPanel.GetRocketStuffFromTarget(selectedTarget, ref rocketModuleCluster, ref clustercraft, ref craftModuleInterface);
		rocketStatusContainer.gameObject.SetActive(craftModuleInterface != null || rocketModuleCluster != null);
		if (craftModuleInterface != null)
		{
			RocketEngineCluster engine = craftModuleInterface.GetEngine();
			string text;
			string text2;
			if (engine != null && engine.GetComponent<HEPFuelTank>() != null)
			{
				text = GameUtil.GetFormattedHighEnergyParticles(craftModuleInterface.FuelPerHex, GameUtil.TimeSlice.None, true);
				text2 = GameUtil.GetFormattedHighEnergyParticles(craftModuleInterface.FuelRemaining, GameUtil.TimeSlice.None, true);
			}
			else
			{
				text = GameUtil.GetFormattedMass(craftModuleInterface.FuelPerHex, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}");
				text2 = GameUtil.GetFormattedMass(craftModuleInterface.FuelRemaining, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}");
			}
			string text3 = string.Concat(new string[]
			{
				UI.CLUSTERMAP.ROCKETS.RANGE.TOOLTIP,
				"\n    • ",
				string.Format(UI.CLUSTERMAP.ROCKETS.FUEL_PER_HEX.NAME, text),
				"\n    • ",
				UI.CLUSTERMAP.ROCKETS.FUEL_REMAINING.NAME,
				text2,
				"\n    • ",
				UI.CLUSTERMAP.ROCKETS.OXIDIZER_REMAINING.NAME,
				GameUtil.GetFormattedMass(craftModuleInterface.OxidizerPowerRemaining, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")
			});
			bool flag;
			RocketModuleCluster primaryPilotModule = craftModuleInterface.GetPrimaryPilotModule(out flag);
			if (flag)
			{
				RoboPilotModule component = primaryPilotModule.GetComponent<RoboPilotModule>();
				text3 = text3 + "\n" + string.Format(UI.CLUSTERMAP.ROCKETS.RANGE.ROBO_PILOTED_TOOLTIP, component.dataBankConsumption, component.GetDataBanksStored());
			}
			rocketStatusContainer.SetLabel("RangeRemaining", UI.CLUSTERMAP.ROCKETS.RANGE.NAME + GameUtil.GetFormattedRocketRange(craftModuleInterface.RangeInTiles, true), text3);
			string text4 = string.Concat(new string[]
			{
				UI.CLUSTERMAP.ROCKETS.SPEED.TOOLTIP,
				"\n    • ",
				UI.CLUSTERMAP.ROCKETS.POWER_TOTAL.NAME,
				craftModuleInterface.EnginePower.ToString(),
				"\n    • ",
				UI.CLUSTERMAP.ROCKETS.BURDEN_TOTAL.NAME,
				craftModuleInterface.TotalBurden.ToString()
			});
			Clustercraft component2 = craftModuleInterface.GetComponent<Clustercraft>();
			if (component2 != null)
			{
				text4 += UI.CLUSTERMAP.ROCKETS.SPEED.PILOT_SPEED_MODIFIER;
				bool flag2 = craftModuleInterface.GetPassengerModule();
				bool flag3 = craftModuleInterface.GetRobotPilotModule();
				bool flag4;
				bool flag5;
				component2.GetPilotedStatus(out flag4, out flag5);
				if (flag4)
				{
					text4 = text4 + "\n    • " + UI.CLUSTERMAP.ROCKETS.SPEED.DUPEPILOT_SPEED_TOOLTIP.Replace("{speed_boost}", GameUtil.GetFormattedPercent(component2.PilotSkillMultiplier - 1f, GameUtil.TimeSlice.None));
				}
				if (flag4 && flag5)
				{
					text4 = text4 + "\n    • " + UI.CLUSTERMAP.ROCKETS.SPEED.SUPERPILOTED_SPEED_TOOLTIP.Replace("{speed_boost}", GameUtil.GetFormattedPercent(50f, GameUtil.TimeSlice.None));
				}
				else if (flag2 && !flag4)
				{
					text4 = text4 + "\n    • " + UI.CLUSTERMAP.ROCKETS.SPEED.UNPILOTED_SPEED_TOOLTIP.Replace("{speed_boost}", GameUtil.GetFormattedPercent(50f, GameUtil.TimeSlice.None));
				}
				else if (flag5)
				{
					text4 = text4 + "\n    • " + UI.CLUSTERMAP.ROCKETS.SPEED.ROBO_PILOT_ONLY_SPEED_TOOLTIP;
				}
				else if (flag3 && !flag2)
				{
					text4 = text4 + "\n    • " + UI.CLUSTERMAP.ROCKETS.SPEED.DEAD_ROBO_PILOT_ONLY_SPEED_TOOLTIP;
				}
			}
			rocketStatusContainer.SetLabel("Speed", UI.CLUSTERMAP.ROCKETS.SPEED.NAME + GameUtil.GetFormattedRocketRangePerCycle(craftModuleInterface.Speed, true), text4);
			if (craftModuleInterface.GetEngine() != null)
			{
				string text5 = string.Format(UI.CLUSTERMAP.ROCKETS.MAX_HEIGHT.TOOLTIP, craftModuleInterface.GetEngine().GetProperName(), craftModuleInterface.MaxHeight.ToString());
				rocketStatusContainer.SetLabel("MaxHeight", string.Format(UI.CLUSTERMAP.ROCKETS.MAX_HEIGHT.NAME, craftModuleInterface.RocketHeight.ToString(), craftModuleInterface.MaxHeight.ToString()), text5);
			}
			rocketStatusContainer.SetLabel("RocketSpacer2", "", "");
			if (clustercraft != null)
			{
				foreach (KeyValuePair<string, GameObject> keyValuePair in this.artifactModuleLabels)
				{
					keyValuePair.Value.SetActive(false);
				}
				int num = 0;
				foreach (Ref<RocketModuleCluster> @ref in clustercraft.ModuleInterface.ClusterModules)
				{
					ArtifactModule component3 = @ref.Get().GetComponent<ArtifactModule>();
					if (component3 != null)
					{
						string text6;
						if (component3.Occupant != null)
						{
							text6 = component3.GetProperName() + ": " + component3.Occupant.GetProperName();
						}
						else
						{
							text6 = string.Format("{0}: {1}", component3.GetProperName(), UI.CLUSTERMAP.ROCKETS.ARTIFACT_MODULE.EMPTY);
						}
						rocketStatusContainer.SetLabel("artifactModule_" + num.ToString(), text6, "");
						num++;
					}
				}
				List<CargoBayCluster> allCargoBays = clustercraft.GetAllCargoBays();
				bool flag6 = allCargoBays != null && allCargoBays.Count > 0;
				foreach (KeyValuePair<string, GameObject> keyValuePair2 in this.cargoBayLabels)
				{
					keyValuePair2.Value.SetActive(false);
				}
				if (flag6)
				{
					ListPool<global::Tuple<string, TextStyleSetting>, SimpleInfoScreen>.PooledList pooledList = ListPool<global::Tuple<string, TextStyleSetting>, SimpleInfoScreen>.Allocate();
					int num2 = 0;
					foreach (CargoBayCluster cargoBayCluster in allCargoBays)
					{
						pooledList.Clear();
						Storage storage = cargoBayCluster.storage;
						string text7 = string.Format("{0}: {1}/{2}", cargoBayCluster.GetComponent<KPrefabID>().GetProperName(), GameUtil.GetFormattedMass(storage.MassStored(), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), GameUtil.GetFormattedMass(storage.capacityKg, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
						foreach (GameObject gameObject in storage.GetItems())
						{
							KPrefabID component4 = gameObject.GetComponent<KPrefabID>();
							PrimaryElement component5 = gameObject.GetComponent<PrimaryElement>();
							string text8 = string.Format("{0} : {1}", component4.GetProperName(), GameUtil.GetFormattedMass(component5.Mass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
							pooledList.Add(new global::Tuple<string, TextStyleSetting>(text8, PluginAssets.Instance.defaultTextStyleSetting));
						}
						string text9 = "";
						for (int i = 0; i < pooledList.Count; i++)
						{
							text9 += pooledList[i].first;
							if (i != pooledList.Count - 1)
							{
								text9 += "\n";
							}
						}
						rocketStatusContainer.SetLabel("cargoBay_" + num2.ToString(), text7, text9);
						num2++;
					}
					pooledList.Recycle();
				}
			}
		}
		if (rocketModuleCluster != null)
		{
			rocketStatusContainer.SetLabel("ModuleStats", UI.CLUSTERMAP.ROCKETS.MODULE_STATS.NAME + selectedTarget.GetProperName(), UI.CLUSTERMAP.ROCKETS.MODULE_STATS.TOOLTIP);
			float burden = rocketModuleCluster.performanceStats.Burden;
			float enginePower = rocketModuleCluster.performanceStats.EnginePower;
			if (burden != 0f)
			{
				rocketStatusContainer.SetLabel("LocalBurden", "    • " + UI.CLUSTERMAP.ROCKETS.BURDEN_MODULE.NAME + burden.ToString(), string.Format(UI.CLUSTERMAP.ROCKETS.BURDEN_MODULE.TOOLTIP, burden));
			}
			if (enginePower != 0f)
			{
				rocketStatusContainer.SetLabel("LocalPower", "    • " + UI.CLUSTERMAP.ROCKETS.POWER_MODULE.NAME + enginePower.ToString(), string.Format(UI.CLUSTERMAP.ROCKETS.POWER_MODULE.TOOLTIP, enginePower));
			}
		}
		rocketStatusContainer.Commit();
	}

	public static void GetRocketStuffFromTarget(GameObject selectedTarget, ref RocketModuleCluster rocketModuleCluster, ref Clustercraft clusterCraft, ref CraftModuleInterface craftModuleInterface)
	{
		rocketModuleCluster = selectedTarget.GetComponent<RocketModuleCluster>();
		clusterCraft = selectedTarget.GetComponent<Clustercraft>();
		craftModuleInterface = null;
		if (rocketModuleCluster != null)
		{
			craftModuleInterface = rocketModuleCluster.CraftInterface;
			if (clusterCraft == null)
			{
				clusterCraft = craftModuleInterface.GetComponent<Clustercraft>();
				return;
			}
		}
		else if (clusterCraft != null)
		{
			craftModuleInterface = clusterCraft.ModuleInterface;
		}
	}

	private Dictionary<string, GameObject> cargoBayLabels = new Dictionary<string, GameObject>();

	private Dictionary<string, GameObject> artifactModuleLabels = new Dictionary<string, GameObject>();
}
