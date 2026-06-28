using System;
using System.Collections;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

namespace OverlayModes
{
	public class Power : Mode
	{
		public Power(Canvas powerLabelParent, LocText powerLabelPrefab, BatteryUI batteryUIPrefab, Vector3 powerLabelOffset, Vector3 batteryUIOffset, Vector3 batteryUITransformerOffset, Color consumerColour, Color generatorColour, Color buildingDisabledColour, Color32 circuitUnpoweredColour, Color32 circuitSafeColour, Color32 circuitStrainingColour)
		{
			this.powerLabelParent = powerLabelParent;
			this.powerLabelPrefab = powerLabelPrefab;
			this.batteryUIPrefab = batteryUIPrefab;
			this.powerLabelOffset = powerLabelOffset;
			this.batteryUIOffset = batteryUIOffset;
			this.batteryUITransformerOffset = batteryUITransformerOffset;
			this.consumerColour = consumerColour;
			this.generatorColour = generatorColour;
			this.buildingDisabledColour = buildingDisabledColour;
			this.circuitUnpoweredColour = circuitUnpoweredColour;
			this.circuitSafeColour = circuitSafeColour;
			this.circuitStrainingColour = circuitStrainingColour;
			this.targetLayer = LayerMask.NameToLayer("MaskedOverlay");
			this.cameraLayerMask = LayerMask.GetMask(new string[] { "MaskedOverlay", "MaskedOverlayBG" });
			this.selectionMask = this.cameraLayerMask;
		}

		public override SimViewMode ViewMode()
		{
			return SimViewMode.PowerMap;
		}

		public override string GetSoundName()
		{
			return "Power";
		}

		public override void Enable()
		{
			Camera.main.cullingMask |= this.cameraLayerMask;
			SelectTool.Instance.SetLayerMask(this.selectionMask);
			DragTool.SetLayerMask(this.selectionMask);
			base.RegisterSaveLoadListeners();
			this.partition = Mode.PopulatePartition<SaveLoadRoot>(OverlayScreen.WireIDs);
			GridCompositor.Instance.ToggleMinor(true);
		}

		public override void Disable()
		{
			Mode.ResetDisplayValues<SaveLoadRoot>(this.layerTargets);
			Camera.main.cullingMask &= ~this.cameraLayerMask;
			SelectTool.Instance.ClearLayerMask();
			DragTool.ClearLayerMask();
			base.UnregisterSaveLoadListeners();
			this.partition.Clear();
			this.layerTargets.Clear();
			this.privateTargets.Clear();
			this.queuedAdds.Clear();
			this.DisablePowerLabels();
			this.DisableBatteryUIs();
			GridCompositor.Instance.ToggleMinor(false);
		}

		protected override void OnSaveLoadRootRegistered(SaveLoadRoot item)
		{
			Tag saveLoadTag = item.GetComponent<KPrefabID>().GetSaveLoadTag();
			if (OverlayScreen.WireIDs.Contains(saveLoadTag))
			{
				this.partition.Add(item);
			}
		}

		protected override void OnSaveLoadRootUnregistered(SaveLoadRoot item)
		{
			if (item == null || item.gameObject == null)
			{
				return;
			}
			if (this.layerTargets.Contains(item))
			{
				this.layerTargets.Remove(item);
			}
			this.partition.Remove(item);
		}

		public override void Update()
		{
			Vector2I vector2I;
			Vector2I vector2I2;
			Grid.GetVisibleExtents(out vector2I, out vector2I2);
			Mode.RemoveOffscreenTargets<SaveLoadRoot>(this.layerTargets, vector2I, vector2I2, null);
			using (new KProfiler.Region("UpdatePowerOverlay", null))
			{
				IEnumerable allIntersecting = this.partition.GetAllIntersecting(new Vector2((float)vector2I.x, (float)vector2I.y), new Vector2((float)vector2I2.x, (float)vector2I2.y));
				IEnumerator enumerator = allIntersecting.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						SaveLoadRoot saveLoadRoot = (SaveLoadRoot)obj;
						base.AddTargetIfVisible<SaveLoadRoot>(saveLoadRoot, vector2I, vector2I2, this.layerTargets, this.targetLayer, null, null);
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = enumerator as IDisposable) != null)
					{
						disposable.Dispose();
					}
				}
				CircuitManager circuitManager = Game.Instance.circuitManager;
				foreach (SaveLoadRoot saveLoadRoot2 in this.layerTargets)
				{
					if (!(saveLoadRoot2 == null))
					{
						Wire component = saveLoadRoot2.GetComponent<Wire>();
						if (component != null)
						{
							KBatchedAnimController component2 = component.GetComponent<KBatchedAnimController>();
							ushort networkID = component.NetworkID;
							bool flag = circuitManager.HasGenerators(networkID) || circuitManager.HasBatteries(networkID);
							Color32 color;
							if (flag)
							{
								float potentialWattsGeneratedByCircuit = circuitManager.GetPotentialWattsGeneratedByCircuit(networkID);
								float wattsUsedByCircuit = circuitManager.GetWattsUsedByCircuit(networkID);
								float num = wattsUsedByCircuit / potentialWattsGeneratedByCircuit;
								color = ((num >= 0.85f) ? this.circuitStrainingColour : this.circuitSafeColour);
							}
							else
							{
								color = this.circuitUnpoweredColour;
							}
							component2.TintColour = color;
						}
					}
				}
			}
			this.queuedAdds.Clear();
			using (new KProfiler.Region("BatteryUI", null))
			{
				foreach (Battery battery in Components.Batteries)
				{
					Vector2I vector2I3 = Grid.PosToXY(battery.transform.position);
					if (vector2I <= vector2I3 && vector2I3 <= vector2I2)
					{
						SaveLoadRoot component3 = battery.GetComponent<SaveLoadRoot>();
						if (!this.privateTargets.Contains(component3))
						{
							this.AddBatteryUI(battery);
							this.queuedAdds.Add(component3);
						}
					}
				}
				foreach (Generator generator in Components.Generators)
				{
					Vector2I vector2I4 = Grid.PosToXY(generator.transform.position);
					if (vector2I <= vector2I4 && vector2I4 <= vector2I2)
					{
						SaveLoadRoot component4 = generator.GetComponent<SaveLoadRoot>();
						if (!this.privateTargets.Contains(component4))
						{
							this.privateTargets.Add(component4);
							if (generator.GetComponent<PowerTransformer>() == null)
							{
								this.AddPowerLabels(generator);
							}
						}
					}
				}
				foreach (EnergyConsumer energyConsumer in Components.EnergyConsumers)
				{
					Vector2I vector2I5 = Grid.PosToXY(energyConsumer.transform.position);
					if (vector2I <= vector2I5 && vector2I5 <= vector2I2)
					{
						SaveLoadRoot component5 = energyConsumer.GetComponent<SaveLoadRoot>();
						if (!this.privateTargets.Contains(component5))
						{
							this.privateTargets.Add(component5);
							this.AddPowerLabels(energyConsumer);
						}
					}
				}
			}
			foreach (SaveLoadRoot saveLoadRoot3 in this.queuedAdds)
			{
				this.privateTargets.Add(saveLoadRoot3);
			}
			this.queuedAdds.Clear();
			this.UpdatePowerLabels();
		}

		private LocText GetFreePowerLabel()
		{
			LocText locText;
			if (this.freePowerLabelIdx < this.powerLabels.Count)
			{
				locText = this.powerLabels[this.freePowerLabelIdx];
				this.freePowerLabelIdx++;
			}
			else
			{
				locText = Util.KInstantiateUI<LocText>(this.powerLabelPrefab.gameObject, this.powerLabelParent.transform.gameObject, false);
				this.powerLabels.Add(locText);
				this.freePowerLabelIdx++;
			}
			return locText;
		}

		private void UpdatePowerLabels()
		{
			foreach (Power.UpdatePowerInfo updatePowerInfo in this.updatePowerInfo)
			{
				KMonoBehaviour item = updatePowerInfo.item;
				LocText powerLabel = updatePowerInfo.powerLabel;
				LocText unitLabel = updatePowerInfo.unitLabel;
				Generator generator = updatePowerInfo.generator;
				IEnergyConsumer consumer = updatePowerInfo.consumer;
				if (updatePowerInfo.item == null)
				{
					powerLabel.gameObject.SetActive(false);
				}
				else
				{
					if (generator != null && consumer == null)
					{
						ManualGenerator component = generator.GetComponent<ManualGenerator>();
						int num;
						if (component == null)
						{
							generator.GetComponent<Operational>();
							num = Mathf.Max(0, Mathf.RoundToInt(generator.WattageRating));
						}
						else
						{
							num = Mathf.Max(0, Mathf.RoundToInt(generator.WattageRating));
						}
						powerLabel.text = ((num == 0) ? num.ToString() : ("+" + num.ToString()));
						BuildingEnabledButton component2 = item.GetComponent<BuildingEnabledButton>();
						Color color = ((!(component2 != null) || component2.IsEnabled) ? this.generatorColour : this.buildingDisabledColour);
						powerLabel.color = color;
						unitLabel.color = color;
						Image outputIcon = generator.GetComponent<BuildingCellVisualizer>().GetOutputIcon();
						if (outputIcon != null)
						{
							outputIcon.color = color;
						}
					}
					if (consumer != null)
					{
						BuildingEnabledButton component3 = item.GetComponent<BuildingEnabledButton>();
						Color color2 = ((!(component3 != null) || component3.IsEnabled) ? this.consumerColour : this.buildingDisabledColour);
						int num2 = Mathf.Max(0, Mathf.RoundToInt(consumer.WattsNeededWhenActive));
						string text = num2.ToString();
						powerLabel.text = ((num2 == 0) ? text : ("-" + text));
						powerLabel.color = color2;
						unitLabel.color = color2;
						Image inputIcon = item.GetComponentInChildren<BuildingCellVisualizer>().GetInputIcon();
						if (inputIcon != null)
						{
							inputIcon.color = color2;
						}
					}
				}
			}
			foreach (Power.UpdateBatteryInfo updateBatteryInfo in this.updateBatteryInfo)
			{
				updateBatteryInfo.ui.SetContent(updateBatteryInfo.battery);
			}
		}

		private void AddPowerLabels(KMonoBehaviour item)
		{
			IEnergyConsumer componentInChildren = item.gameObject.GetComponentInChildren<IEnergyConsumer>();
			Generator componentInChildren2 = item.gameObject.GetComponentInChildren<Generator>();
			if (componentInChildren != null || componentInChildren2 != null)
			{
				float num = -10f;
				if (componentInChildren2 != null)
				{
					LocText freePowerLabel = this.GetFreePowerLabel();
					freePowerLabel.gameObject.SetActive(true);
					freePowerLabel.gameObject.name = item.gameObject.name + "power label";
					LocText component = freePowerLabel.transform.GetChild(0).GetComponent<LocText>();
					component.gameObject.SetActive(true);
					freePowerLabel.enabled = true;
					component.enabled = true;
					Vector3 vector = Grid.CellToPos(componentInChildren2.PowerCell, 0.5f, 0f, 0f);
					freePowerLabel.rectTransform.position = vector + this.powerLabelOffset + Vector3.up * (num * 0.02f);
					if (componentInChildren != null && componentInChildren.PowerCell == componentInChildren2.PowerCell)
					{
						num -= 15f;
					}
					this.SetToolTip(freePowerLabel, UI.OVERLAYS.POWER.WATTS_GENERATED);
					this.updatePowerInfo.Add(new Power.UpdatePowerInfo(item, freePowerLabel, component, componentInChildren2, null));
				}
				if (componentInChildren != null && componentInChildren.GetType() != typeof(Battery))
				{
					LocText freePowerLabel2 = this.GetFreePowerLabel();
					LocText component2 = freePowerLabel2.transform.GetChild(0).GetComponent<LocText>();
					freePowerLabel2.gameObject.SetActive(true);
					component2.gameObject.SetActive(true);
					freePowerLabel2.gameObject.name = item.gameObject.name + "power label";
					freePowerLabel2.enabled = true;
					component2.enabled = true;
					Vector3 vector2 = Grid.CellToPos(componentInChildren.PowerCell, 0.5f, 0f, 0f);
					freePowerLabel2.rectTransform.position = vector2 + this.powerLabelOffset + Vector3.up * (num * 0.02f);
					this.SetToolTip(freePowerLabel2, UI.OVERLAYS.POWER.WATTS_CONSUMED);
					this.updatePowerInfo.Add(new Power.UpdatePowerInfo(item, freePowerLabel2, component2, null, componentInChildren));
				}
			}
		}

		private void DisablePowerLabels()
		{
			this.freePowerLabelIdx = 0;
			foreach (LocText locText in this.powerLabels)
			{
				locText.gameObject.SetActive(false);
			}
			this.updatePowerInfo.Clear();
		}

		private void AddBatteryUI(Battery bat)
		{
			BatteryUI freeBatteryUI = this.GetFreeBatteryUI();
			freeBatteryUI.SetContent(bat);
			Vector3 vector = Grid.CellToPos(bat.PowerCell, 0.5f, 0f, 0f);
			bool flag = bat.GetComponent<PowerTransformer>() != null;
			float num = 1f;
			Rotatable component = bat.GetComponent<Rotatable>();
			if (component != null && component.GetVisualizerFlipX())
			{
				num = -1f;
			}
			Vector3 vector2 = ((!flag) ? this.batteryUIOffset : this.batteryUITransformerOffset);
			vector2.x *= num;
			freeBatteryUI.GetComponent<RectTransform>().position = Vector3.up + vector + vector2;
			this.updateBatteryInfo.Add(new Power.UpdateBatteryInfo(bat, freeBatteryUI));
		}

		private void SetToolTip(LocText label, string text)
		{
			ToolTip component = label.GetComponent<ToolTip>();
			if (component != null)
			{
				component.toolTip = text;
			}
		}

		private void DisableBatteryUIs()
		{
			this.freeBatteryUIIdx = 0;
			foreach (BatteryUI batteryUI in this.batteryUIList)
			{
				batteryUI.gameObject.SetActive(false);
			}
			this.updateBatteryInfo.Clear();
		}

		private BatteryUI GetFreeBatteryUI()
		{
			BatteryUI batteryUI;
			if (this.freeBatteryUIIdx < this.batteryUIList.Count)
			{
				batteryUI = this.batteryUIList[this.freeBatteryUIIdx];
				batteryUI.gameObject.SetActive(true);
				this.freeBatteryUIIdx++;
			}
			else
			{
				batteryUI = Util.KInstantiateUI<BatteryUI>(this.batteryUIPrefab.gameObject, this.powerLabelParent.transform.gameObject, false);
				this.batteryUIList.Add(batteryUI);
				this.freeBatteryUIIdx++;
			}
			return batteryUI;
		}

		private int targetLayer;

		private int cameraLayerMask;

		private int selectionMask;

		private List<Power.UpdatePowerInfo> updatePowerInfo = new List<Power.UpdatePowerInfo>();

		private List<Power.UpdateBatteryInfo> updateBatteryInfo = new List<Power.UpdateBatteryInfo>();

		private Canvas powerLabelParent;

		private LocText powerLabelPrefab;

		private Vector3 powerLabelOffset;

		private BatteryUI batteryUIPrefab;

		private Vector3 batteryUIOffset;

		private Vector3 batteryUITransformerOffset;

		private Color32 consumerColour;

		private Color32 generatorColour;

		private Color32 buildingDisabledColour;

		private Color32 circuitUnpoweredColour;

		private Color32 circuitSafeColour;

		private Color32 circuitStrainingColour;

		private int freePowerLabelIdx;

		private int freeBatteryUIIdx;

		private List<LocText> powerLabels = new List<LocText>();

		private List<BatteryUI> batteryUIList = new List<BatteryUI>();

		private UniformGrid<SaveLoadRoot> partition;

		private List<SaveLoadRoot> queuedAdds = new List<SaveLoadRoot>();

		private HashSet<SaveLoadRoot> layerTargets = new HashSet<SaveLoadRoot>();

		private HashSet<SaveLoadRoot> privateTargets = new HashSet<SaveLoadRoot>();

		private struct UpdatePowerInfo
		{
			public UpdatePowerInfo(KMonoBehaviour item, LocText power_label, LocText unit_label, Generator g, IEnergyConsumer c)
			{
				this.item = item;
				this.powerLabel = power_label;
				this.unitLabel = unit_label;
				this.generator = g;
				this.consumer = c;
			}

			public KMonoBehaviour item;

			public LocText powerLabel;

			public LocText unitLabel;

			public Generator generator;

			public IEnergyConsumer consumer;
		}

		private struct UpdateBatteryInfo
		{
			public UpdateBatteryInfo(Battery battery, BatteryUI ui)
			{
				this.battery = battery;
				this.ui = ui;
			}

			public Battery battery;

			public BatteryUI ui;
		}
	}
}
