using System;
using System.Collections.Generic;
using FMOD.Studio;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;
using UnityEngine.UI;

public abstract class OverlayModes
{
	public class GasConduits : OverlayModes.ConduitMode
	{
		public override HashedString ViewMode()
		{
			return OverlayModes.GasConduits.ID;
		}

		public override string GetSoundName()
		{
			return "GasVent";
		}

		public GasConduits()
			: base(OverlayScreen.GasVentIDs)
		{
		}

		public static readonly HashedString ID = "GasConduit";
	}

	public class LiquidConduits : OverlayModes.ConduitMode
	{
		public override HashedString ViewMode()
		{
			return OverlayModes.LiquidConduits.ID;
		}

		public override string GetSoundName()
		{
			return "LiquidVent";
		}

		public LiquidConduits()
			: base(OverlayScreen.LiquidVentIDs)
		{
		}

		public static readonly HashedString ID = "LiquidConduit";
	}

	public abstract class ConduitMode : OverlayModes.Mode
	{
		public ConduitMode(ICollection<Tag> ids)
		{
			this.objectTargetLayer = LayerMask.NameToLayer("MaskedOverlayBG");
			this.conduitTargetLayer = LayerMask.NameToLayer("MaskedOverlay");
			this.cameraLayerMask = LayerMask.GetMask(new string[] { "MaskedOverlay", "MaskedOverlayBG" });
			this.selectionMask = this.cameraLayerMask;
			this.targetIDs = ids;
		}

		public override void Enable()
		{
			base.RegisterSaveLoadListeners();
			this.partition = OverlayModes.Mode.PopulatePartition<SaveLoadRoot>(this.targetIDs);
			Camera.main.cullingMask |= this.cameraLayerMask;
			SelectTool.Instance.SetLayerMask(this.selectionMask);
			GridCompositor.Instance.ToggleMinor(false);
			base.Enable();
		}

		protected override void OnSaveLoadRootRegistered(SaveLoadRoot item)
		{
			Tag saveLoadTag = item.GetComponent<KPrefabID>().GetSaveLoadTag();
			if (this.targetIDs.Contains(saveLoadTag))
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

		public override void Disable()
		{
			foreach (SaveLoadRoot saveLoadRoot in this.layerTargets)
			{
				float defaultDepth = OverlayModes.Mode.GetDefaultDepth(saveLoadRoot);
				Vector3 position = saveLoadRoot.transform.GetPosition();
				position.z = defaultDepth;
				saveLoadRoot.transform.SetPosition(position);
				KBatchedAnimController[] componentsInChildren = saveLoadRoot.GetComponentsInChildren<KBatchedAnimController>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					this.TriggerResorting(componentsInChildren[i]);
				}
			}
			OverlayModes.Mode.ResetDisplayValues<SaveLoadRoot>(this.layerTargets);
			Camera.main.cullingMask &= ~this.cameraLayerMask;
			SelectTool.Instance.ClearLayerMask();
			base.UnregisterSaveLoadListeners();
			this.partition.Clear();
			this.layerTargets.Clear();
			GridCompositor.Instance.ToggleMinor(false);
			base.Disable();
		}

		public override void Update()
		{
			Vector2I vector2I;
			Vector2I vector2I2;
			Grid.GetVisibleExtents(out vector2I, out vector2I2);
			OverlayModes.Mode.RemoveOffscreenTargets<SaveLoadRoot>(this.layerTargets, vector2I, vector2I2, delegate(SaveLoadRoot root)
			{
				if (root == null)
				{
					return;
				}
				float defaultDepth = OverlayModes.Mode.GetDefaultDepth(root);
				Vector3 position = root.transform.GetPosition();
				position.z = defaultDepth;
				root.transform.SetPosition(position);
				KBatchedAnimController[] componentsInChildren = root.GetComponentsInChildren<KBatchedAnimController>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					this.TriggerResorting(componentsInChildren[i]);
				}
			});
			foreach (object obj in this.partition.GetAllIntersecting(new Vector2((float)vector2I.x, (float)vector2I.y), new Vector2((float)vector2I2.x, (float)vector2I2.y)))
			{
				SaveLoadRoot saveLoadRoot = (SaveLoadRoot)obj;
				if (saveLoadRoot.GetComponent<Conduit>() != null)
				{
					base.AddTargetIfVisible<SaveLoadRoot>(saveLoadRoot, vector2I, vector2I2, this.layerTargets, this.conduitTargetLayer, null, null);
				}
				else
				{
					base.AddTargetIfVisible<SaveLoadRoot>(saveLoadRoot, vector2I, vector2I2, this.layerTargets, this.objectTargetLayer, delegate(SaveLoadRoot root)
					{
						Vector3 position2 = root.transform.GetPosition();
						float num2 = position2.z;
						KPrefabID component3 = root.GetComponent<KPrefabID>();
						if (component3 != null)
						{
							if (component3.HasTag(GameTags.OverlayInFrontOfConduits))
							{
								num2 = Grid.GetLayerZ((this.ViewMode() == OverlayModes.LiquidConduits.ID) ? Grid.SceneLayer.LiquidConduits : Grid.SceneLayer.GasConduits) - 0.2f;
							}
							else if (component3.HasTag(GameTags.OverlayBehindConduits))
							{
								num2 = Grid.GetLayerZ((this.ViewMode() == OverlayModes.LiquidConduits.ID) ? Grid.SceneLayer.LiquidConduits : Grid.SceneLayer.GasConduits) + 0.2f;
							}
						}
						position2.z = num2;
						root.transform.SetPosition(position2);
						KBatchedAnimController[] componentsInChildren2 = root.GetComponentsInChildren<KBatchedAnimController>();
						for (int j = 0; j < componentsInChildren2.Length; j++)
						{
							this.TriggerResorting(componentsInChildren2[j]);
						}
					}, null);
				}
			}
			GameObject gameObject = null;
			if (SelectTool.Instance != null && SelectTool.Instance.hover != null)
			{
				gameObject = SelectTool.Instance.hover.gameObject;
			}
			this.connectedNetworks.Clear();
			float num = 1f;
			if (gameObject != null)
			{
				IBridgedNetworkItem component = gameObject.GetComponent<IBridgedNetworkItem>();
				if (component != null)
				{
					int networkCell = component.GetNetworkCell();
					UtilityNetworkManager<FlowUtilityNetwork, Vent> utilityNetworkManager = ((this.ViewMode() == OverlayModes.LiquidConduits.ID) ? Game.Instance.liquidConduitSystem : Game.Instance.gasConduitSystem);
					this.visited.Clear();
					this.FindConnectedNetworks(networkCell, utilityNetworkManager, this.connectedNetworks, this.visited);
					this.visited.Clear();
					num = OverlayModes.ModeUtil.GetHighlightScale();
				}
			}
			Game.ConduitVisInfo conduitVisInfo = ((this.ViewMode() == OverlayModes.LiquidConduits.ID) ? Game.Instance.liquidConduitVisInfo : Game.Instance.gasConduitVisInfo);
			foreach (SaveLoadRoot saveLoadRoot2 in this.layerTargets)
			{
				if (!(saveLoadRoot2 == null) && saveLoadRoot2.GetComponent<IBridgedNetworkItem>() != null)
				{
					BuildingDef def = saveLoadRoot2.GetComponent<Building>().Def;
					Color32 color;
					if (def.ThermalConductivity == 1f)
					{
						color = GlobalAssets.Instance.colorSet.GetColorByName(conduitVisInfo.overlayTintName);
					}
					else if (def.ThermalConductivity < 1f)
					{
						color = GlobalAssets.Instance.colorSet.GetColorByName(conduitVisInfo.overlayInsulatedTintName);
					}
					else
					{
						color = GlobalAssets.Instance.colorSet.GetColorByName(conduitVisInfo.overlayRadiantTintName);
					}
					if (this.connectedNetworks.Count > 0)
					{
						IBridgedNetworkItem component2 = saveLoadRoot2.GetComponent<IBridgedNetworkItem>();
						if (component2 != null && component2.IsConnectedToNetworks(this.connectedNetworks))
						{
							color.r = (byte)((float)color.r * num);
							color.g = (byte)((float)color.g * num);
							color.b = (byte)((float)color.b * num);
						}
					}
					saveLoadRoot2.GetComponent<KBatchedAnimController>().TintColour = color;
				}
			}
		}

		private void TriggerResorting(KBatchedAnimController kbac)
		{
			if (kbac.enabled)
			{
				kbac.enabled = false;
				kbac.enabled = true;
			}
		}

		private void FindConnectedNetworks(int cell, IUtilityNetworkMgr mgr, ICollection<UtilityNetwork> networks, List<int> visited)
		{
			if (visited.Contains(cell))
			{
				return;
			}
			visited.Add(cell);
			UtilityNetwork networkForCell = mgr.GetNetworkForCell(cell);
			if (networkForCell != null)
			{
				networks.Add(networkForCell);
				UtilityConnections connections = mgr.GetConnections(cell, false);
				if ((connections & UtilityConnections.Right) != (UtilityConnections)0)
				{
					this.FindConnectedNetworks(Grid.CellRight(cell), mgr, networks, visited);
				}
				if ((connections & UtilityConnections.Left) != (UtilityConnections)0)
				{
					this.FindConnectedNetworks(Grid.CellLeft(cell), mgr, networks, visited);
				}
				if ((connections & UtilityConnections.Up) != (UtilityConnections)0)
				{
					this.FindConnectedNetworks(Grid.CellAbove(cell), mgr, networks, visited);
				}
				if ((connections & UtilityConnections.Down) != (UtilityConnections)0)
				{
					this.FindConnectedNetworks(Grid.CellBelow(cell), mgr, networks, visited);
				}
				object endpoint = mgr.GetEndpoint(cell);
				if (endpoint != null)
				{
					FlowUtilityNetwork.NetworkItem networkItem = endpoint as FlowUtilityNetwork.NetworkItem;
					if (networkItem != null)
					{
						IBridgedNetworkItem component = networkItem.GameObject.GetComponent<IBridgedNetworkItem>();
						if (component != null)
						{
							component.AddNetworks(networks);
						}
					}
				}
			}
		}

		private UniformGrid<SaveLoadRoot> partition;

		private HashSet<SaveLoadRoot> layerTargets = new HashSet<SaveLoadRoot>();

		private HashSet<UtilityNetwork> connectedNetworks = new HashSet<UtilityNetwork>();

		private List<int> visited = new List<int>();

		private ICollection<Tag> targetIDs;

		private int objectTargetLayer;

		private int conduitTargetLayer;

		private int cameraLayerMask;

		private int selectionMask;
	}

	public class Crop : OverlayModes.BasePlantMode
	{
		public override HashedString ViewMode()
		{
			return OverlayModes.Crop.ID;
		}

		public override string GetSoundName()
		{
			return "Harvest";
		}

		public Crop(Canvas ui_root, GameObject harvestable_notification_prefab)
		{
			OverlayModes.ColorHighlightCondition[] array = new OverlayModes.ColorHighlightCondition[3];
			array[0] = new OverlayModes.ColorHighlightCondition((KMonoBehaviour h) => GlobalAssets.Instance.colorSet.cropHalted, delegate(KMonoBehaviour h)
			{
				WiltCondition component = h.GetComponent<WiltCondition>();
				return component != null && component.IsWilting();
			});
			array[1] = new OverlayModes.ColorHighlightCondition((KMonoBehaviour h) => GlobalAssets.Instance.colorSet.cropGrowing, (KMonoBehaviour h) => !(h as HarvestDesignatable).CanBeHarvested());
			array[2] = new OverlayModes.ColorHighlightCondition((KMonoBehaviour h) => GlobalAssets.Instance.colorSet.cropGrown, (KMonoBehaviour h) => (h as HarvestDesignatable).CanBeHarvested());
			this.highlightConditions = array;
			base..ctor(OverlayScreen.HarvestableIDs);
			this.uiRoot = ui_root;
			this.harvestableNotificationPrefab = harvestable_notification_prefab;
		}

		public override List<LegendEntry> GetCustomLegendData()
		{
			return new List<LegendEntry>
			{
				new LegendEntry(UI.OVERLAYS.CROP.FULLY_GROWN, UI.OVERLAYS.CROP.TOOLTIPS.FULLY_GROWN, GlobalAssets.Instance.colorSet.cropGrown, null, null, true),
				new LegendEntry(UI.OVERLAYS.CROP.GROWING, UI.OVERLAYS.CROP.TOOLTIPS.GROWING, GlobalAssets.Instance.colorSet.cropGrowing, null, null, true),
				new LegendEntry(UI.OVERLAYS.CROP.GROWTH_HALTED, UI.OVERLAYS.CROP.TOOLTIPS.GROWTH_HALTED, GlobalAssets.Instance.colorSet.cropHalted, null, null, true)
			};
		}

		public override void Update()
		{
			this.updateCropInfo.Clear();
			this.freeHarvestableNotificationIdx = 0;
			Vector2I vector2I;
			Vector2I vector2I2;
			Grid.GetVisibleExtents(out vector2I, out vector2I2);
			OverlayModes.Mode.RemoveOffscreenTargets<HarvestDesignatable>(this.layerTargets, vector2I, vector2I2, null);
			foreach (object obj in this.partition.GetAllIntersecting(new Vector2((float)vector2I.x, (float)vector2I.y), new Vector2((float)vector2I2.x, (float)vector2I2.y)))
			{
				HarvestDesignatable harvestDesignatable = (HarvestDesignatable)obj;
				base.AddTargetIfVisible<HarvestDesignatable>(harvestDesignatable, vector2I, vector2I2, this.layerTargets, this.targetLayer, null, null);
			}
			foreach (HarvestDesignatable harvestDesignatable2 in this.layerTargets)
			{
				Vector2I vector2I3 = Grid.PosToXY(harvestDesignatable2.transform.GetPosition());
				if (vector2I <= vector2I3 && vector2I3 <= vector2I2)
				{
					this.AddCropUI(harvestDesignatable2);
				}
			}
			foreach (OverlayModes.Crop.UpdateCropInfo updateCropInfo in this.updateCropInfo)
			{
				updateCropInfo.harvestableUI.GetComponent<HarvestableOverlayWidget>().Refresh(updateCropInfo.harvestable);
			}
			for (int i = this.freeHarvestableNotificationIdx; i < this.harvestableNotificationList.Count; i++)
			{
				if (this.harvestableNotificationList[i].activeSelf)
				{
					this.harvestableNotificationList[i].SetActive(false);
				}
			}
			base.UpdateHighlightTypeOverlay<HarvestDesignatable>(vector2I, vector2I2, this.layerTargets, this.targetIDs, this.highlightConditions, OverlayModes.BringToFrontLayerSetting.Constant, this.targetLayer);
			base.Update();
		}

		public override void Disable()
		{
			this.DisableHarvestableUINotifications();
			base.Disable();
		}

		private void DisableHarvestableUINotifications()
		{
			this.freeHarvestableNotificationIdx = 0;
			foreach (GameObject gameObject in this.harvestableNotificationList)
			{
				gameObject.SetActive(false);
			}
			this.updateCropInfo.Clear();
		}

		public GameObject GetFreeCropUI()
		{
			GameObject gameObject;
			if (this.freeHarvestableNotificationIdx < this.harvestableNotificationList.Count)
			{
				gameObject = this.harvestableNotificationList[this.freeHarvestableNotificationIdx];
				if (!gameObject.gameObject.activeSelf)
				{
					gameObject.gameObject.SetActive(true);
				}
				this.freeHarvestableNotificationIdx++;
			}
			else
			{
				gameObject = global::Util.KInstantiateUI(this.harvestableNotificationPrefab.gameObject, this.uiRoot.transform.gameObject, false);
				this.harvestableNotificationList.Add(gameObject);
				this.freeHarvestableNotificationIdx++;
			}
			return gameObject;
		}

		private void AddCropUI(HarvestDesignatable harvestable)
		{
			GameObject freeCropUI = this.GetFreeCropUI();
			OverlayModes.Crop.UpdateCropInfo updateCropInfo = new OverlayModes.Crop.UpdateCropInfo(harvestable, freeCropUI);
			Vector3 vector = Grid.CellToPos(Grid.PosToCell(harvestable), 0.5f, -1.25f, 0f);
			freeCropUI.GetComponent<RectTransform>().SetPosition(Vector3.up + vector);
			this.updateCropInfo.Add(updateCropInfo);
		}

		public static readonly HashedString ID = "Crop";

		private Canvas uiRoot;

		private List<OverlayModes.Crop.UpdateCropInfo> updateCropInfo = new List<OverlayModes.Crop.UpdateCropInfo>();

		private int freeHarvestableNotificationIdx;

		private List<GameObject> harvestableNotificationList = new List<GameObject>();

		private GameObject harvestableNotificationPrefab;

		private OverlayModes.ColorHighlightCondition[] highlightConditions;

		private struct UpdateCropInfo
		{
			public UpdateCropInfo(HarvestDesignatable harvestable, GameObject harvestableUI)
			{
				this.harvestable = harvestable;
				this.harvestableUI = harvestableUI;
			}

			public HarvestDesignatable harvestable;

			public GameObject harvestableUI;
		}
	}

	public class Harvest : OverlayModes.BasePlantMode
	{
		public override HashedString ViewMode()
		{
			return OverlayModes.Harvest.ID;
		}

		public override string GetSoundName()
		{
			return "Harvest";
		}

		public Harvest()
		{
			OverlayModes.ColorHighlightCondition[] array = new OverlayModes.ColorHighlightCondition[1];
			array[0] = new OverlayModes.ColorHighlightCondition((KMonoBehaviour harvestable) => new Color(0.65f, 0.65f, 0.65f, 0.65f), (KMonoBehaviour harvestable) => true);
			this.highlightConditions = array;
			base..ctor(OverlayScreen.HarvestableIDs);
		}

		public override void Update()
		{
			Vector2I vector2I;
			Vector2I vector2I2;
			Grid.GetVisibleExtents(out vector2I, out vector2I2);
			OverlayModes.Mode.RemoveOffscreenTargets<HarvestDesignatable>(this.layerTargets, vector2I, vector2I2, null);
			foreach (object obj in this.partition.GetAllIntersecting(new Vector2((float)vector2I.x, (float)vector2I.y), new Vector2((float)vector2I2.x, (float)vector2I2.y)))
			{
				HarvestDesignatable harvestDesignatable = (HarvestDesignatable)obj;
				base.AddTargetIfVisible<HarvestDesignatable>(harvestDesignatable, vector2I, vector2I2, this.layerTargets, this.targetLayer, null, null);
			}
			base.UpdateHighlightTypeOverlay<HarvestDesignatable>(vector2I, vector2I2, this.layerTargets, this.targetIDs, this.highlightConditions, OverlayModes.BringToFrontLayerSetting.Constant, this.targetLayer);
			base.Update();
		}

		public static readonly HashedString ID = "HarvestWhenReady";

		private OverlayModes.ColorHighlightCondition[] highlightConditions;
	}

	public abstract class BasePlantMode : OverlayModes.Mode
	{
		public BasePlantMode(ICollection<Tag> ids)
		{
			this.targetLayer = LayerMask.NameToLayer("MaskedOverlay");
			this.cameraLayerMask = LayerMask.GetMask(new string[] { "MaskedOverlay", "MaskedOverlayBG" });
			this.selectionMask = LayerMask.GetMask(new string[] { "MaskedOverlay" });
			this.targetIDs = ids;
		}

		public override void Enable()
		{
			base.RegisterSaveLoadListeners();
			this.partition = OverlayModes.Mode.PopulatePartition<HarvestDesignatable>(this.targetIDs);
			Camera.main.cullingMask |= this.cameraLayerMask;
			SelectTool.Instance.SetLayerMask(this.selectionMask);
		}

		protected override void OnSaveLoadRootRegistered(SaveLoadRoot item)
		{
			Tag saveLoadTag = item.GetComponent<KPrefabID>().GetSaveLoadTag();
			if (!this.targetIDs.Contains(saveLoadTag))
			{
				return;
			}
			HarvestDesignatable component = item.GetComponent<HarvestDesignatable>();
			if (component == null)
			{
				return;
			}
			this.partition.Add(component);
		}

		protected override void OnSaveLoadRootUnregistered(SaveLoadRoot item)
		{
			if (item == null || item.gameObject == null)
			{
				return;
			}
			HarvestDesignatable component = item.GetComponent<HarvestDesignatable>();
			if (component == null)
			{
				return;
			}
			if (this.layerTargets.Contains(component))
			{
				this.layerTargets.Remove(component);
			}
			this.partition.Remove(component);
		}

		public override void Disable()
		{
			base.UnregisterSaveLoadListeners();
			base.DisableHighlightTypeOverlay<HarvestDesignatable>(this.layerTargets);
			Camera.main.cullingMask &= ~this.cameraLayerMask;
			this.partition.Clear();
			this.layerTargets.Clear();
			SelectTool.Instance.ClearLayerMask();
		}

		protected UniformGrid<HarvestDesignatable> partition;

		protected HashSet<HarvestDesignatable> layerTargets = new HashSet<HarvestDesignatable>();

		protected ICollection<Tag> targetIDs;

		protected int targetLayer;

		private int cameraLayerMask;

		private int selectionMask;
	}

	public class Decor : OverlayModes.Mode
	{
		public override HashedString ViewMode()
		{
			return OverlayModes.Decor.ID;
		}

		public override string GetSoundName()
		{
			return "Decor";
		}

		public override List<LegendEntry> GetCustomLegendData()
		{
			return new List<LegendEntry>
			{
				new LegendEntry(UI.OVERLAYS.DECOR.HIGHDECOR, UI.OVERLAYS.DECOR.TOOLTIPS.HIGHDECOR, GlobalAssets.Instance.colorSet.decorPositive, null, null, true),
				new LegendEntry(UI.OVERLAYS.DECOR.LOWDECOR, UI.OVERLAYS.DECOR.TOOLTIPS.LOWDECOR, GlobalAssets.Instance.colorSet.decorNegative, null, null, true)
			};
		}

		public Decor()
		{
			OverlayModes.ColorHighlightCondition[] array = new OverlayModes.ColorHighlightCondition[1];
			array[0] = new OverlayModes.ColorHighlightCondition(delegate(KMonoBehaviour dp)
			{
				Color black = Color.black;
				Color color = Color.black;
				if (dp != null)
				{
					int num = Grid.PosToCell(CameraController.Instance.baseCamera.ScreenToWorldPoint(KInputManager.GetMousePos()));
					float num2 = (dp as DecorProvider).GetDecorForCell(num);
					if (num2 > 0f)
					{
						color = GlobalAssets.Instance.colorSet.decorHighlightPositive;
					}
					else if (num2 < 0f)
					{
						color = GlobalAssets.Instance.colorSet.decorHighlightNegative;
					}
					else if (dp.GetComponent<MonumentPart>() != null && dp.GetComponent<MonumentPart>().IsMonumentCompleted())
					{
						foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(dp.GetComponent<AttachableBuilding>()))
						{
							num2 = gameObject.GetComponent<DecorProvider>().GetDecorForCell(num);
							if (num2 > 0f)
							{
								color = GlobalAssets.Instance.colorSet.decorHighlightPositive;
								break;
							}
							if (num2 < 0f)
							{
								color = GlobalAssets.Instance.colorSet.decorHighlightNegative;
								break;
							}
						}
					}
				}
				return Color.Lerp(black, color, 0.85f);
			}, (KMonoBehaviour dp) => SelectToolHoverTextCard.highlightedObjects.Contains(dp.gameObject));
			this.highlightConditions = array;
			base..ctor();
			this.targetLayer = LayerMask.NameToLayer("MaskedOverlay");
			this.cameraLayerMask = LayerMask.GetMask(new string[] { "MaskedOverlay", "MaskedOverlayBG" });
		}

		public override void Enable()
		{
			base.RegisterSaveLoadListeners();
			List<Tag> prefabTagsWithComponent = Assets.GetPrefabTagsWithComponent<DecorProvider>();
			this.targetIDs.UnionWith(prefabTagsWithComponent);
			foreach (Tag tag in new Tag[]
			{
				new Tag("Tile"),
				new Tag("SnowTile"),
				new Tag("WoodTile"),
				new Tag("MeshTile"),
				new Tag("InsulationTile"),
				new Tag("GasPermeableMembrane"),
				new Tag("CarpetTile")
			})
			{
				this.targetIDs.Remove(tag);
			}
			foreach (Tag tag2 in OverlayScreen.GasVentIDs)
			{
				this.targetIDs.Remove(tag2);
			}
			foreach (Tag tag3 in OverlayScreen.LiquidVentIDs)
			{
				this.targetIDs.Remove(tag3);
			}
			this.partition = OverlayModes.Mode.PopulatePartition<DecorProvider>(this.targetIDs);
			Camera.main.cullingMask |= this.cameraLayerMask;
		}

		public override void Update()
		{
			Vector2I vector2I;
			Vector2I vector2I2;
			Grid.GetVisibleExtents(out vector2I, out vector2I2);
			OverlayModes.Mode.RemoveOffscreenTargets<DecorProvider>(this.layerTargets, vector2I, vector2I2, null);
			this.partition.GetAllIntersecting(new Vector2((float)vector2I.x, (float)vector2I.y), new Vector2((float)vector2I2.x, (float)vector2I2.y), this.workingTargets);
			for (int i = 0; i < this.workingTargets.Count; i++)
			{
				DecorProvider decorProvider = this.workingTargets[i];
				base.AddTargetIfVisible<DecorProvider>(decorProvider, vector2I, vector2I2, this.layerTargets, this.targetLayer, null, null);
			}
			base.UpdateHighlightTypeOverlay<DecorProvider>(vector2I, vector2I2, this.layerTargets, this.targetIDs, this.highlightConditions, OverlayModes.BringToFrontLayerSetting.Conditional, this.targetLayer);
			this.workingTargets.Clear();
		}

		protected override void OnSaveLoadRootRegistered(SaveLoadRoot item)
		{
			Tag saveLoadTag = item.GetComponent<KPrefabID>().GetSaveLoadTag();
			if (this.targetIDs.Contains(saveLoadTag))
			{
				DecorProvider component = item.GetComponent<DecorProvider>();
				if (component != null)
				{
					this.partition.Add(component);
				}
			}
		}

		protected override void OnSaveLoadRootUnregistered(SaveLoadRoot item)
		{
			if (item == null || item.gameObject == null)
			{
				return;
			}
			DecorProvider component = item.GetComponent<DecorProvider>();
			if (component != null)
			{
				if (this.layerTargets.Contains(component))
				{
					this.layerTargets.Remove(component);
				}
				this.partition.Remove(component);
			}
		}

		public override void Disable()
		{
			base.DisableHighlightTypeOverlay<DecorProvider>(this.layerTargets);
			Camera.main.cullingMask &= ~this.cameraLayerMask;
			base.UnregisterSaveLoadListeners();
			this.partition.Clear();
			this.layerTargets.Clear();
		}

		public static readonly HashedString ID = "Decor";

		private UniformGrid<DecorProvider> partition;

		private HashSet<DecorProvider> layerTargets = new HashSet<DecorProvider>();

		private List<DecorProvider> workingTargets = new List<DecorProvider>();

		private HashSet<Tag> targetIDs = new HashSet<Tag>();

		private int targetLayer;

		private int cameraLayerMask;

		private OverlayModes.ColorHighlightCondition[] highlightConditions;
	}

	public class Disease : OverlayModes.Mode
	{
		private static float CalculateHUE(Color32 colour)
		{
			byte b = Math.Max(colour.r, Math.Max(colour.g, colour.b));
			byte b2 = Math.Min(colour.r, Math.Min(colour.g, colour.b));
			float num = 0f;
			int num2 = (int)(b - b2);
			if (num2 == 0)
			{
				num = 0f;
			}
			else if (b == colour.r)
			{
				num = (float)(colour.g - colour.b) / (float)num2 % 6f;
			}
			else if (b == colour.g)
			{
				num = (float)(colour.b - colour.r) / (float)num2 + 2f;
			}
			else if (b == colour.b)
			{
				num = (float)(colour.r - colour.g) / (float)num2 + 4f;
			}
			return num;
		}

		public override HashedString ViewMode()
		{
			return OverlayModes.Disease.ID;
		}

		public override string GetSoundName()
		{
			return "Disease";
		}

		public Disease(Canvas diseaseUIParent, GameObject diseaseOverlayPrefab)
		{
			this.diseaseUIParent = diseaseUIParent;
			this.diseaseOverlayPrefab = diseaseOverlayPrefab;
			this.legendFilters = this.CreateDefaultFilters();
			this.cameraLayerMask = LayerMask.GetMask(new string[] { "MaskedOverlay", "MaskedOverlayBG" });
		}

		public override void Enable()
		{
			Infrared.Instance.SetMode(Infrared.Mode.Disease);
			CameraController.Instance.ToggleColouredOverlayView(true);
			Camera.main.cullingMask |= this.cameraLayerMask;
			base.RegisterSaveLoadListeners();
			foreach (DiseaseSourceVisualizer diseaseSourceVisualizer in Components.DiseaseSourceVisualizers.Items)
			{
				if (!(diseaseSourceVisualizer == null))
				{
					diseaseSourceVisualizer.Show(this.ViewMode());
				}
			}
		}

		public override Dictionary<string, ToolParameterMenu.ToggleState> CreateDefaultFilters()
		{
			return new Dictionary<string, ToolParameterMenu.ToggleState>
			{
				{
					ToolParameterMenu.FILTERLAYERS.ALL,
					ToolParameterMenu.ToggleState.On
				},
				{
					ToolParameterMenu.FILTERLAYERS.LIQUIDCONDUIT,
					ToolParameterMenu.ToggleState.Off
				},
				{
					ToolParameterMenu.FILTERLAYERS.GASCONDUIT,
					ToolParameterMenu.ToggleState.Off
				}
			};
		}

		public override void OnFiltersChanged()
		{
			Game.Instance.showGasConduitDisease = base.InFilter(ToolParameterMenu.FILTERLAYERS.GASCONDUIT, this.legendFilters);
			Game.Instance.showLiquidConduitDisease = base.InFilter(ToolParameterMenu.FILTERLAYERS.LIQUIDCONDUIT, this.legendFilters);
		}

		protected override void OnSaveLoadRootRegistered(SaveLoadRoot item)
		{
			if (item == null)
			{
				return;
			}
			KBatchedAnimController component = item.GetComponent<KBatchedAnimController>();
			if (component == null)
			{
				return;
			}
			InfraredVisualizerComponents.ClearOverlayColour(component);
		}

		protected override void OnSaveLoadRootUnregistered(SaveLoadRoot item)
		{
		}

		public override void Disable()
		{
			foreach (DiseaseSourceVisualizer diseaseSourceVisualizer in Components.DiseaseSourceVisualizers.Items)
			{
				if (!(diseaseSourceVisualizer == null))
				{
					diseaseSourceVisualizer.Show(OverlayModes.None.ID);
				}
			}
			base.UnregisterSaveLoadListeners();
			Camera.main.cullingMask &= ~this.cameraLayerMask;
			foreach (KMonoBehaviour kmonoBehaviour in this.layerTargets)
			{
				if (!(kmonoBehaviour == null))
				{
					float defaultDepth = OverlayModes.Mode.GetDefaultDepth(kmonoBehaviour);
					Vector3 position = kmonoBehaviour.transform.GetPosition();
					position.z = defaultDepth;
					kmonoBehaviour.transform.SetPosition(position);
					KBatchedAnimController component = kmonoBehaviour.GetComponent<KBatchedAnimController>();
					component.enabled = false;
					component.enabled = true;
				}
			}
			CameraController.Instance.ToggleColouredOverlayView(false);
			Infrared.Instance.SetMode(Infrared.Mode.Disabled);
			Game.Instance.showGasConduitDisease = false;
			Game.Instance.showLiquidConduitDisease = false;
			this.freeDiseaseUI = 0;
			foreach (OverlayModes.Disease.UpdateDiseaseInfo updateDiseaseInfo in this.updateDiseaseInfo)
			{
				updateDiseaseInfo.ui.gameObject.SetActive(false);
			}
			this.updateDiseaseInfo.Clear();
			this.privateTargets.Clear();
			this.layerTargets.Clear();
		}

		public override List<LegendEntry> GetCustomLegendData()
		{
			List<LegendEntry> list = new List<LegendEntry>();
			List<OverlayModes.Disease.DiseaseSortInfo> list2 = new List<OverlayModes.Disease.DiseaseSortInfo>();
			foreach (Klei.AI.Disease disease in Db.Get().Diseases.resources)
			{
				list2.Add(new OverlayModes.Disease.DiseaseSortInfo(disease));
			}
			list2.Sort((OverlayModes.Disease.DiseaseSortInfo a, OverlayModes.Disease.DiseaseSortInfo b) => a.sortkey.CompareTo(b.sortkey));
			foreach (OverlayModes.Disease.DiseaseSortInfo diseaseSortInfo in list2)
			{
				list.Add(new LegendEntry(diseaseSortInfo.disease.Name, diseaseSortInfo.disease.overlayLegendHovertext.ToString(), GlobalAssets.Instance.colorSet.GetColorByName(diseaseSortInfo.disease.overlayColourName), null, null, true));
			}
			return list;
		}

		public GameObject GetFreeDiseaseUI()
		{
			GameObject gameObject;
			if (this.freeDiseaseUI < this.diseaseUIList.Count)
			{
				gameObject = this.diseaseUIList[this.freeDiseaseUI];
				gameObject.gameObject.SetActive(true);
				this.freeDiseaseUI++;
			}
			else
			{
				gameObject = global::Util.KInstantiateUI(this.diseaseOverlayPrefab, this.diseaseUIParent.transform.gameObject, false);
				this.diseaseUIList.Add(gameObject);
				this.freeDiseaseUI++;
			}
			return gameObject;
		}

		private void AddDiseaseUI(MinionIdentity target)
		{
			GameObject gameObject = this.GetFreeDiseaseUI();
			DiseaseOverlayWidget component = gameObject.GetComponent<DiseaseOverlayWidget>();
			AmountInstance amountInstance = target.GetComponent<Modifiers>().amounts.Get(Db.Get().Amounts.ImmuneLevel);
			OverlayModes.Disease.UpdateDiseaseInfo updateDiseaseInfo = new OverlayModes.Disease.UpdateDiseaseInfo(amountInstance, component);
			KAnimControllerBase component2 = target.GetComponent<KAnimControllerBase>();
			Vector3 vector = ((component2 != null) ? component2.GetWorldPivot() : (target.transform.GetPosition() + Vector3.down));
			gameObject.GetComponent<RectTransform>().SetPosition(vector);
			this.updateDiseaseInfo.Add(updateDiseaseInfo);
		}

		public override void Update()
		{
			Vector2I vector2I;
			Vector2I vector2I2;
			Grid.GetVisibleExtents(out vector2I, out vector2I2);
			using (new KProfiler.Region("UpdateDiseaseCarriers", null))
			{
				this.queuedAdds.Clear();
				foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities.Items)
				{
					if (!(minionIdentity == null))
					{
						Vector2I vector2I3 = Grid.PosToXY(minionIdentity.transform.GetPosition());
						if (vector2I <= vector2I3 && vector2I3 <= vector2I2 && !this.privateTargets.Contains(minionIdentity))
						{
							this.AddDiseaseUI(minionIdentity);
							this.queuedAdds.Add(minionIdentity);
						}
					}
				}
				foreach (KMonoBehaviour kmonoBehaviour in this.queuedAdds)
				{
					this.privateTargets.Add(kmonoBehaviour);
				}
				this.queuedAdds.Clear();
			}
			foreach (OverlayModes.Disease.UpdateDiseaseInfo updateDiseaseInfo in this.updateDiseaseInfo)
			{
				updateDiseaseInfo.ui.Refresh(updateDiseaseInfo.valueSrc);
			}
			bool flag = false;
			if (Game.Instance.showLiquidConduitDisease)
			{
				using (HashSet<Tag>.Enumerator enumerator4 = OverlayScreen.LiquidVentIDs.GetEnumerator())
				{
					while (enumerator4.MoveNext())
					{
						Tag tag = enumerator4.Current;
						if (!OverlayScreen.DiseaseIDs.Contains(tag))
						{
							OverlayScreen.DiseaseIDs.Add(tag);
							flag = true;
						}
					}
					goto IL_01F1;
				}
			}
			foreach (Tag tag2 in OverlayScreen.LiquidVentIDs)
			{
				if (OverlayScreen.DiseaseIDs.Contains(tag2))
				{
					OverlayScreen.DiseaseIDs.Remove(tag2);
					flag = true;
				}
			}
			IL_01F1:
			if (Game.Instance.showGasConduitDisease)
			{
				using (HashSet<Tag>.Enumerator enumerator4 = OverlayScreen.GasVentIDs.GetEnumerator())
				{
					while (enumerator4.MoveNext())
					{
						Tag tag3 = enumerator4.Current;
						if (!OverlayScreen.DiseaseIDs.Contains(tag3))
						{
							OverlayScreen.DiseaseIDs.Add(tag3);
							flag = true;
						}
					}
					goto IL_0297;
				}
			}
			foreach (Tag tag4 in OverlayScreen.GasVentIDs)
			{
				if (OverlayScreen.DiseaseIDs.Contains(tag4))
				{
					OverlayScreen.DiseaseIDs.Remove(tag4);
					flag = true;
				}
			}
			IL_0297:
			if (flag)
			{
				this.SetLayerZ(-50f);
			}
		}

		private void SetLayerZ(float offset_z)
		{
			Vector2I vector2I;
			Vector2I vector2I2;
			Grid.GetVisibleExtents(out vector2I, out vector2I2);
			OverlayModes.Mode.ClearOutsideViewObjects<KMonoBehaviour>(this.layerTargets, vector2I, vector2I2, OverlayScreen.DiseaseIDs, delegate(KMonoBehaviour go)
			{
				if (go != null)
				{
					float defaultDepth2 = OverlayModes.Mode.GetDefaultDepth(go);
					Vector3 position2 = go.transform.GetPosition();
					position2.z = defaultDepth2;
					go.transform.SetPosition(position2);
					KBatchedAnimController component2 = go.GetComponent<KBatchedAnimController>();
					component2.enabled = false;
					component2.enabled = true;
				}
			});
			Dictionary<Tag, List<SaveLoadRoot>> lists = SaveLoader.Instance.saveManager.GetLists();
			foreach (Tag tag in OverlayScreen.DiseaseIDs)
			{
				List<SaveLoadRoot> list;
				if (lists.TryGetValue(tag, out list))
				{
					foreach (KMonoBehaviour kmonoBehaviour in list)
					{
						if (!(kmonoBehaviour == null) && !this.layerTargets.Contains(kmonoBehaviour))
						{
							Vector3 position = kmonoBehaviour.transform.GetPosition();
							if (Grid.IsVisible(Grid.PosToCell(position)) && vector2I <= position && position <= vector2I2)
							{
								float defaultDepth = OverlayModes.Mode.GetDefaultDepth(kmonoBehaviour);
								position.z = defaultDepth + offset_z;
								kmonoBehaviour.transform.SetPosition(position);
								KBatchedAnimController component = kmonoBehaviour.GetComponent<KBatchedAnimController>();
								component.enabled = false;
								component.enabled = true;
								this.layerTargets.Add(kmonoBehaviour);
							}
						}
					}
				}
			}
		}

		public static readonly HashedString ID = "Disease";

		private int cameraLayerMask;

		private int freeDiseaseUI;

		private List<GameObject> diseaseUIList = new List<GameObject>();

		private List<OverlayModes.Disease.UpdateDiseaseInfo> updateDiseaseInfo = new List<OverlayModes.Disease.UpdateDiseaseInfo>();

		private HashSet<KMonoBehaviour> layerTargets = new HashSet<KMonoBehaviour>();

		private HashSet<KMonoBehaviour> privateTargets = new HashSet<KMonoBehaviour>();

		private List<KMonoBehaviour> queuedAdds = new List<KMonoBehaviour>();

		private Canvas diseaseUIParent;

		private GameObject diseaseOverlayPrefab;

		private struct DiseaseSortInfo
		{
			public DiseaseSortInfo(Klei.AI.Disease d)
			{
				this.disease = d;
				this.sortkey = OverlayModes.Disease.CalculateHUE(GlobalAssets.Instance.colorSet.GetColorByName(d.overlayColourName));
			}

			public float sortkey;

			public Klei.AI.Disease disease;
		}

		private struct UpdateDiseaseInfo
		{
			public UpdateDiseaseInfo(AmountInstance amount_inst, DiseaseOverlayWidget ui)
			{
				this.ui = ui;
				this.valueSrc = amount_inst;
			}

			public DiseaseOverlayWidget ui;

			public AmountInstance valueSrc;
		}
	}

	public class Logic : OverlayModes.Mode
	{
		public override HashedString ViewMode()
		{
			return OverlayModes.Logic.ID;
		}

		public override string GetSoundName()
		{
			return "Logic";
		}

		public override List<LegendEntry> GetCustomLegendData()
		{
			return new List<LegendEntry>
			{
				new LegendEntry(UI.OVERLAYS.LOGIC.INPUT, UI.OVERLAYS.LOGIC.TOOLTIPS.INPUT, Color.white, null, Assets.GetSprite("logicInput"), true),
				new LegendEntry(UI.OVERLAYS.LOGIC.OUTPUT, UI.OVERLAYS.LOGIC.TOOLTIPS.OUTPUT, Color.white, null, Assets.GetSprite("logicOutput"), true),
				new LegendEntry(UI.OVERLAYS.LOGIC.RIBBON_INPUT, UI.OVERLAYS.LOGIC.TOOLTIPS.RIBBON_INPUT, Color.white, null, Assets.GetSprite("logic_ribbon_all_in"), true),
				new LegendEntry(UI.OVERLAYS.LOGIC.RIBBON_OUTPUT, UI.OVERLAYS.LOGIC.TOOLTIPS.RIBBON_OUTPUT, Color.white, null, Assets.GetSprite("logic_ribbon_all_out"), true),
				new LegendEntry(UI.OVERLAYS.LOGIC.RESET_UPDATE, UI.OVERLAYS.LOGIC.TOOLTIPS.RESET_UPDATE, Color.white, null, Assets.GetSprite("logicResetUpdate"), true),
				new LegendEntry(UI.OVERLAYS.LOGIC.CONTROL_INPUT, UI.OVERLAYS.LOGIC.TOOLTIPS.CONTROL_INPUT, Color.white, null, Assets.GetSprite("control_input_frame_legend"), true),
				new LegendEntry(UI.OVERLAYS.LOGIC.CIRCUIT_STATUS_HEADER, null, Color.white, null, null, false),
				new LegendEntry(UI.OVERLAYS.LOGIC.ONE, null, GlobalAssets.Instance.colorSet.logicOnText, null, null, true),
				new LegendEntry(UI.OVERLAYS.LOGIC.ZERO, null, GlobalAssets.Instance.colorSet.logicOffText, null, null, true),
				new LegendEntry(UI.OVERLAYS.LOGIC.DISCONNECTED, UI.OVERLAYS.LOGIC.TOOLTIPS.DISCONNECTED, GlobalAssets.Instance.colorSet.logicDisconnected, null, null, true)
			};
		}

		public Logic(LogicModeUI ui_asset)
		{
			this.conduitTargetLayer = LayerMask.NameToLayer("MaskedOverlay");
			this.objectTargetLayer = LayerMask.NameToLayer("MaskedOverlayBG");
			this.cameraLayerMask = LayerMask.GetMask(new string[] { "MaskedOverlay", "MaskedOverlayBG" });
			this.selectionMask = this.cameraLayerMask;
			this.uiAsset = ui_asset;
		}

		public override void Enable()
		{
			Camera.main.cullingMask |= this.cameraLayerMask;
			SelectTool.Instance.SetLayerMask(this.selectionMask);
			base.RegisterSaveLoadListeners();
			this.gameObjPartition = OverlayModes.Mode.PopulatePartition<SaveLoadRoot>(OverlayModes.Logic.HighlightItemIDs);
			this.ioPartition = this.CreateLogicUIPartition();
			GridCompositor.Instance.ToggleMinor(true);
			LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
			logicCircuitManager.onElemAdded = (Action<ILogicUIElement>)Delegate.Combine(logicCircuitManager.onElemAdded, new Action<ILogicUIElement>(this.OnUIElemAdded));
			LogicCircuitManager logicCircuitManager2 = Game.Instance.logicCircuitManager;
			logicCircuitManager2.onElemRemoved = (Action<ILogicUIElement>)Delegate.Combine(logicCircuitManager2.onElemRemoved, new Action<ILogicUIElement>(this.OnUIElemRemoved));
			AudioMixer.instance.Start(AudioMixerSnapshots.Get().TechFilterLogicOn);
		}

		public override void Disable()
		{
			LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
			logicCircuitManager.onElemAdded = (Action<ILogicUIElement>)Delegate.Remove(logicCircuitManager.onElemAdded, new Action<ILogicUIElement>(this.OnUIElemAdded));
			LogicCircuitManager logicCircuitManager2 = Game.Instance.logicCircuitManager;
			logicCircuitManager2.onElemRemoved = (Action<ILogicUIElement>)Delegate.Remove(logicCircuitManager2.onElemRemoved, new Action<ILogicUIElement>(this.OnUIElemRemoved));
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().TechFilterLogicOn, STOP_MODE.ALLOWFADEOUT);
			foreach (SaveLoadRoot saveLoadRoot in this.gameObjTargets)
			{
				float defaultDepth = OverlayModes.Mode.GetDefaultDepth(saveLoadRoot);
				Vector3 position = saveLoadRoot.transform.GetPosition();
				position.z = defaultDepth;
				saveLoadRoot.transform.SetPosition(position);
				saveLoadRoot.GetComponent<KBatchedAnimController>().enabled = false;
				saveLoadRoot.GetComponent<KBatchedAnimController>().enabled = true;
			}
			OverlayModes.Mode.ResetDisplayValues<SaveLoadRoot>(this.gameObjTargets);
			OverlayModes.Mode.ResetDisplayValues<KBatchedAnimController>(this.wireControllers);
			OverlayModes.Mode.ResetDisplayValues<KBatchedAnimController>(this.ribbonControllers);
			this.ResetRibbonSymbolTints<KBatchedAnimController>(this.ribbonControllers);
			foreach (OverlayModes.Logic.BridgeInfo bridgeInfo in this.bridgeControllers)
			{
				if (bridgeInfo.controller != null)
				{
					OverlayModes.Mode.ResetDisplayValues(bridgeInfo.controller);
				}
			}
			foreach (OverlayModes.Logic.BridgeInfo bridgeInfo2 in this.ribbonBridgeControllers)
			{
				if (bridgeInfo2.controller != null)
				{
					this.ResetRibbonTint(bridgeInfo2.controller);
				}
			}
			Camera.main.cullingMask &= ~this.cameraLayerMask;
			SelectTool.Instance.ClearLayerMask();
			base.UnregisterSaveLoadListeners();
			foreach (OverlayModes.Logic.UIInfo uiinfo in this.uiInfo.GetDataList())
			{
				uiinfo.Release();
			}
			this.uiInfo.Clear();
			this.uiNodes.Clear();
			this.ioPartition.Clear();
			this.ioTargets.Clear();
			this.gameObjPartition.Clear();
			this.gameObjTargets.Clear();
			this.wireControllers.Clear();
			this.ribbonControllers.Clear();
			this.bridgeControllers.Clear();
			this.ribbonBridgeControllers.Clear();
			GridCompositor.Instance.ToggleMinor(false);
		}

		protected override void OnSaveLoadRootRegistered(SaveLoadRoot item)
		{
			Tag saveLoadTag = item.GetComponent<KPrefabID>().GetSaveLoadTag();
			if (OverlayModes.Logic.HighlightItemIDs.Contains(saveLoadTag))
			{
				this.gameObjPartition.Add(item);
			}
		}

		protected override void OnSaveLoadRootUnregistered(SaveLoadRoot item)
		{
			if (item == null || item.gameObject == null)
			{
				return;
			}
			if (this.gameObjTargets.Contains(item))
			{
				this.gameObjTargets.Remove(item);
			}
			this.gameObjPartition.Remove(item);
		}

		private void OnUIElemAdded(ILogicUIElement elem)
		{
			this.ioPartition.Add(elem);
		}

		private void OnUIElemRemoved(ILogicUIElement elem)
		{
			this.ioPartition.Remove(elem);
			if (this.ioTargets.Contains(elem))
			{
				this.ioTargets.Remove(elem);
				this.FreeUI(elem);
			}
		}

		public override void Update()
		{
			Vector2I vector2I;
			Vector2I vector2I2;
			Grid.GetVisibleExtents(out vector2I, out vector2I2);
			Tag wire_id = TagManager.Create("LogicWire");
			Tag ribbon_id = TagManager.Create("LogicRibbon");
			Tag bridge_id = TagManager.Create("LogicWireBridge");
			Tag ribbon_bridge_id = TagManager.Create("LogicRibbonBridge");
			OverlayModes.Mode.RemoveOffscreenTargets<SaveLoadRoot>(this.gameObjTargets, vector2I, vector2I2, delegate(SaveLoadRoot root)
			{
				if (root == null)
				{
					return;
				}
				KPrefabID component7 = root.GetComponent<KPrefabID>();
				if (component7 != null)
				{
					Tag prefabTag = component7.PrefabTag;
					if (prefabTag == wire_id)
					{
						this.wireControllers.Remove(root.GetComponent<KBatchedAnimController>());
						return;
					}
					if (prefabTag == ribbon_id)
					{
						this.ResetRibbonTint(root.GetComponent<KBatchedAnimController>());
						this.ribbonControllers.Remove(root.GetComponent<KBatchedAnimController>());
						return;
					}
					if (prefabTag == bridge_id)
					{
						KBatchedAnimController controller2 = root.GetComponent<KBatchedAnimController>();
						this.bridgeControllers.RemoveWhere((OverlayModes.Logic.BridgeInfo x) => x.controller == controller2);
						return;
					}
					if (prefabTag == ribbon_bridge_id)
					{
						KBatchedAnimController controller = root.GetComponent<KBatchedAnimController>();
						this.ResetRibbonTint(controller);
						this.ribbonBridgeControllers.RemoveWhere((OverlayModes.Logic.BridgeInfo x) => x.controller == controller);
						return;
					}
					float defaultDepth = OverlayModes.Mode.GetDefaultDepth(root);
					Vector3 position = root.transform.GetPosition();
					position.z = defaultDepth;
					root.transform.SetPosition(position);
					root.GetComponent<KBatchedAnimController>().enabled = false;
					root.GetComponent<KBatchedAnimController>().enabled = true;
				}
			});
			OverlayModes.Mode.RemoveOffscreenTargets<ILogicUIElement>(this.ioTargets, this.workingIOTargets, vector2I, vector2I2, new Action<ILogicUIElement>(this.FreeUI), null);
			using (new KProfiler.Region("UpdateLogicOverlay", null))
			{
				Action<SaveLoadRoot> <>9__3;
				foreach (object obj in this.gameObjPartition.GetAllIntersecting(new Vector2((float)vector2I.x, (float)vector2I.y), new Vector2((float)vector2I2.x, (float)vector2I2.y)))
				{
					SaveLoadRoot saveLoadRoot = (SaveLoadRoot)obj;
					if (saveLoadRoot != null)
					{
						KPrefabID component = saveLoadRoot.GetComponent<KPrefabID>();
						if (component.PrefabTag == wire_id || component.PrefabTag == bridge_id || component.PrefabTag == ribbon_id || component.PrefabTag == ribbon_bridge_id)
						{
							SaveLoadRoot saveLoadRoot2 = saveLoadRoot;
							Vector2I vector2I3 = vector2I;
							Vector2I vector2I4 = vector2I2;
							ICollection<SaveLoadRoot> collection = this.gameObjTargets;
							int num = this.conduitTargetLayer;
							Action<SaveLoadRoot> action;
							if ((action = <>9__3) == null)
							{
								action = (<>9__3 = delegate(SaveLoadRoot root)
								{
									if (root == null)
									{
										return;
									}
									KPrefabID component8 = root.GetComponent<KPrefabID>();
									if (OverlayModes.Logic.HighlightItemIDs.Contains(component8.PrefabTag))
									{
										if (component8.PrefabTag == wire_id)
										{
											this.wireControllers.Add(root.GetComponent<KBatchedAnimController>());
											return;
										}
										if (component8.PrefabTag == ribbon_id)
										{
											this.ribbonControllers.Add(root.GetComponent<KBatchedAnimController>());
											return;
										}
										if (component8.PrefabTag == bridge_id)
										{
											KBatchedAnimController component9 = root.GetComponent<KBatchedAnimController>();
											int networkCell2 = root.GetComponent<LogicUtilityNetworkLink>().GetNetworkCell();
											this.bridgeControllers.Add(new OverlayModes.Logic.BridgeInfo
											{
												cell = networkCell2,
												controller = component9
											});
											return;
										}
										if (component8.PrefabTag == ribbon_bridge_id)
										{
											KBatchedAnimController component10 = root.GetComponent<KBatchedAnimController>();
											int networkCell3 = root.GetComponent<LogicUtilityNetworkLink>().GetNetworkCell();
											this.ribbonBridgeControllers.Add(new OverlayModes.Logic.BridgeInfo
											{
												cell = networkCell3,
												controller = component10
											});
										}
									}
								});
							}
							base.AddTargetIfVisible<SaveLoadRoot>(saveLoadRoot2, vector2I3, vector2I4, collection, num, action, null);
						}
						else
						{
							base.AddTargetIfVisible<SaveLoadRoot>(saveLoadRoot, vector2I, vector2I2, this.gameObjTargets, this.objectTargetLayer, delegate(SaveLoadRoot root)
							{
								Vector3 position2 = root.transform.GetPosition();
								float num3 = position2.z;
								KPrefabID component11 = root.GetComponent<KPrefabID>();
								if (component11 != null)
								{
									if (component11.HasTag(GameTags.OverlayInFrontOfConduits))
									{
										num3 = Grid.GetLayerZ(Grid.SceneLayer.LogicWires) - 0.2f;
									}
									else if (component11.HasTag(GameTags.OverlayBehindConduits))
									{
										num3 = Grid.GetLayerZ(Grid.SceneLayer.LogicWires) + 0.2f;
									}
								}
								position2.z = num3;
								root.transform.SetPosition(position2);
								KBatchedAnimController component12 = root.GetComponent<KBatchedAnimController>();
								component12.enabled = false;
								component12.enabled = true;
							}, null);
						}
					}
				}
				foreach (object obj2 in this.ioPartition.GetAllIntersecting(new Vector2((float)vector2I.x, (float)vector2I.y), new Vector2((float)vector2I2.x, (float)vector2I2.y)))
				{
					ILogicUIElement logicUIElement = (ILogicUIElement)obj2;
					if (logicUIElement != null)
					{
						base.AddTargetIfVisible<ILogicUIElement>(logicUIElement, vector2I, vector2I2, this.ioTargets, this.objectTargetLayer, new Action<ILogicUIElement>(this.AddUI), (KMonoBehaviour kcmp) => kcmp != null && OverlayModes.Logic.HighlightItemIDs.Contains(kcmp.GetComponent<KPrefabID>().PrefabTag));
					}
				}
				this.connectedNetworks.Clear();
				float num2 = 1f;
				GameObject gameObject = null;
				if (SelectTool.Instance != null && SelectTool.Instance.hover != null)
				{
					gameObject = SelectTool.Instance.hover.gameObject;
				}
				if (gameObject != null)
				{
					IBridgedNetworkItem component2 = gameObject.GetComponent<IBridgedNetworkItem>();
					if (component2 != null)
					{
						int networkCell = component2.GetNetworkCell();
						this.visited.Clear();
						this.FindConnectedNetworks(networkCell, Game.Instance.logicCircuitSystem, this.connectedNetworks, this.visited);
						this.visited.Clear();
						num2 = OverlayModes.ModeUtil.GetHighlightScale();
					}
				}
				LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
				Color32 logicOn = GlobalAssets.Instance.colorSet.logicOn;
				Color32 logicOff = GlobalAssets.Instance.colorSet.logicOff;
				logicOff.a = (logicOn.a = 0);
				foreach (KBatchedAnimController kbatchedAnimController in this.wireControllers)
				{
					if (!(kbatchedAnimController == null))
					{
						Color32 color = logicOff;
						LogicCircuitNetwork networkForCell = logicCircuitManager.GetNetworkForCell(Grid.PosToCell(kbatchedAnimController.transform.GetPosition()));
						if (networkForCell != null)
						{
							color = (networkForCell.IsBitActive(0) ? logicOn : logicOff);
						}
						if (this.connectedNetworks.Count > 0)
						{
							IBridgedNetworkItem component3 = kbatchedAnimController.GetComponent<IBridgedNetworkItem>();
							if (component3 != null && component3.IsConnectedToNetworks(this.connectedNetworks))
							{
								color.r = (byte)((float)color.r * num2);
								color.g = (byte)((float)color.g * num2);
								color.b = (byte)((float)color.b * num2);
							}
						}
						kbatchedAnimController.TintColour = color;
					}
				}
				foreach (KBatchedAnimController kbatchedAnimController2 in this.ribbonControllers)
				{
					if (!(kbatchedAnimController2 == null))
					{
						Color32 color2 = logicOff;
						Color32 color3 = logicOff;
						Color32 color4 = logicOff;
						Color32 color5 = logicOff;
						LogicCircuitNetwork networkForCell2 = logicCircuitManager.GetNetworkForCell(Grid.PosToCell(kbatchedAnimController2.transform.GetPosition()));
						if (networkForCell2 != null)
						{
							color2 = (networkForCell2.IsBitActive(0) ? logicOn : logicOff);
							color3 = (networkForCell2.IsBitActive(1) ? logicOn : logicOff);
							color4 = (networkForCell2.IsBitActive(2) ? logicOn : logicOff);
							color5 = (networkForCell2.IsBitActive(3) ? logicOn : logicOff);
						}
						if (this.connectedNetworks.Count > 0)
						{
							IBridgedNetworkItem component4 = kbatchedAnimController2.GetComponent<IBridgedNetworkItem>();
							if (component4 != null && component4.IsConnectedToNetworks(this.connectedNetworks))
							{
								color2.r = (byte)((float)color2.r * num2);
								color2.g = (byte)((float)color2.g * num2);
								color2.b = (byte)((float)color2.b * num2);
								color3.r = (byte)((float)color3.r * num2);
								color3.g = (byte)((float)color3.g * num2);
								color3.b = (byte)((float)color3.b * num2);
								color4.r = (byte)((float)color4.r * num2);
								color4.g = (byte)((float)color4.g * num2);
								color4.b = (byte)((float)color4.b * num2);
								color5.r = (byte)((float)color5.r * num2);
								color5.g = (byte)((float)color5.g * num2);
								color5.b = (byte)((float)color5.b * num2);
							}
						}
						kbatchedAnimController2.SetSymbolTint(OverlayModes.Logic.RIBBON_WIRE_1_SYMBOL_NAME, color2);
						kbatchedAnimController2.SetSymbolTint(OverlayModes.Logic.RIBBON_WIRE_2_SYMBOL_NAME, color3);
						kbatchedAnimController2.SetSymbolTint(OverlayModes.Logic.RIBBON_WIRE_3_SYMBOL_NAME, color4);
						kbatchedAnimController2.SetSymbolTint(OverlayModes.Logic.RIBBON_WIRE_4_SYMBOL_NAME, color5);
					}
				}
				foreach (OverlayModes.Logic.BridgeInfo bridgeInfo in this.bridgeControllers)
				{
					if (!(bridgeInfo.controller == null))
					{
						Color32 color6 = logicOff;
						LogicCircuitNetwork networkForCell3 = logicCircuitManager.GetNetworkForCell(bridgeInfo.cell);
						if (networkForCell3 != null)
						{
							color6 = (networkForCell3.IsBitActive(0) ? logicOn : logicOff);
						}
						if (this.connectedNetworks.Count > 0)
						{
							IBridgedNetworkItem component5 = bridgeInfo.controller.GetComponent<IBridgedNetworkItem>();
							if (component5 != null && component5.IsConnectedToNetworks(this.connectedNetworks))
							{
								color6.r = (byte)((float)color6.r * num2);
								color6.g = (byte)((float)color6.g * num2);
								color6.b = (byte)((float)color6.b * num2);
							}
						}
						bridgeInfo.controller.TintColour = color6;
					}
				}
				foreach (OverlayModes.Logic.BridgeInfo bridgeInfo2 in this.ribbonBridgeControllers)
				{
					if (!(bridgeInfo2.controller == null))
					{
						Color32 color7 = logicOff;
						Color32 color8 = logicOff;
						Color32 color9 = logicOff;
						Color32 color10 = logicOff;
						LogicCircuitNetwork networkForCell4 = logicCircuitManager.GetNetworkForCell(bridgeInfo2.cell);
						if (networkForCell4 != null)
						{
							color7 = (networkForCell4.IsBitActive(0) ? logicOn : logicOff);
							color8 = (networkForCell4.IsBitActive(1) ? logicOn : logicOff);
							color9 = (networkForCell4.IsBitActive(2) ? logicOn : logicOff);
							color10 = (networkForCell4.IsBitActive(3) ? logicOn : logicOff);
						}
						if (this.connectedNetworks.Count > 0)
						{
							IBridgedNetworkItem component6 = bridgeInfo2.controller.GetComponent<IBridgedNetworkItem>();
							if (component6 != null && component6.IsConnectedToNetworks(this.connectedNetworks))
							{
								color7.r = (byte)((float)color7.r * num2);
								color7.g = (byte)((float)color7.g * num2);
								color7.b = (byte)((float)color7.b * num2);
								color8.r = (byte)((float)color8.r * num2);
								color8.g = (byte)((float)color8.g * num2);
								color8.b = (byte)((float)color8.b * num2);
								color9.r = (byte)((float)color9.r * num2);
								color9.g = (byte)((float)color9.g * num2);
								color9.b = (byte)((float)color9.b * num2);
								color10.r = (byte)((float)color10.r * num2);
								color10.g = (byte)((float)color10.g * num2);
								color10.b = (byte)((float)color10.b * num2);
							}
						}
						bridgeInfo2.controller.SetSymbolTint(OverlayModes.Logic.RIBBON_WIRE_1_SYMBOL_NAME, color7);
						bridgeInfo2.controller.SetSymbolTint(OverlayModes.Logic.RIBBON_WIRE_2_SYMBOL_NAME, color8);
						bridgeInfo2.controller.SetSymbolTint(OverlayModes.Logic.RIBBON_WIRE_3_SYMBOL_NAME, color9);
						bridgeInfo2.controller.SetSymbolTint(OverlayModes.Logic.RIBBON_WIRE_4_SYMBOL_NAME, color10);
					}
				}
			}
			this.UpdateUI();
		}

		private void UpdateUI()
		{
			Color32 logicOn = GlobalAssets.Instance.colorSet.logicOn;
			Color32 logicOff = GlobalAssets.Instance.colorSet.logicOff;
			Color32 logicDisconnected = GlobalAssets.Instance.colorSet.logicDisconnected;
			logicOff.a = (logicOn.a = byte.MaxValue);
			foreach (OverlayModes.Logic.UIInfo uiinfo in this.uiInfo.GetDataList())
			{
				LogicCircuitNetwork networkForCell = Game.Instance.logicCircuitManager.GetNetworkForCell(uiinfo.cell);
				Color32 color = logicDisconnected;
				LogicControlInputUI component = uiinfo.instance.GetComponent<LogicControlInputUI>();
				if (component != null)
				{
					component.SetContent(networkForCell);
				}
				else if (uiinfo.bitDepth == 4)
				{
					LogicRibbonDisplayUI component2 = uiinfo.instance.GetComponent<LogicRibbonDisplayUI>();
					if (component2 != null)
					{
						component2.SetContent(networkForCell);
					}
				}
				else if (uiinfo.bitDepth == 1)
				{
					if (networkForCell != null)
					{
						color = (networkForCell.IsBitActive(0) ? logicOn : logicOff);
					}
					if (uiinfo.image.color != color)
					{
						uiinfo.image.color = color;
					}
				}
			}
		}

		private void AddUI(ILogicUIElement ui_elem)
		{
			if (this.uiNodes.ContainsKey(ui_elem))
			{
				return;
			}
			HandleVector<int>.Handle handle = this.uiInfo.Allocate(new OverlayModes.Logic.UIInfo(ui_elem, this.uiAsset));
			this.uiNodes.Add(ui_elem, new OverlayModes.Logic.EventInfo
			{
				uiHandle = handle
			});
		}

		private void FreeUI(ILogicUIElement item)
		{
			if (item == null)
			{
				return;
			}
			OverlayModes.Logic.EventInfo eventInfo;
			if (this.uiNodes.TryGetValue(item, out eventInfo))
			{
				this.uiInfo.GetData(eventInfo.uiHandle).Release();
				this.uiInfo.Free(eventInfo.uiHandle);
				this.uiNodes.Remove(item);
			}
		}

		protected UniformGrid<ILogicUIElement> CreateLogicUIPartition()
		{
			UniformGrid<ILogicUIElement> uniformGrid = new UniformGrid<ILogicUIElement>(Grid.WidthInCells, Grid.HeightInCells, 8, 8);
			foreach (ILogicUIElement logicUIElement in Game.Instance.logicCircuitManager.GetVisElements())
			{
				if (logicUIElement != null)
				{
					uniformGrid.Add(logicUIElement);
				}
			}
			return uniformGrid;
		}

		private bool IsBitActive(int value, int bit)
		{
			return (value & (1 << bit)) > 0;
		}

		private void FindConnectedNetworks(int cell, IUtilityNetworkMgr mgr, ICollection<UtilityNetwork> networks, List<int> visited)
		{
			if (visited.Contains(cell))
			{
				return;
			}
			visited.Add(cell);
			UtilityNetwork networkForCell = mgr.GetNetworkForCell(cell);
			if (networkForCell != null)
			{
				networks.Add(networkForCell);
				UtilityConnections connections = mgr.GetConnections(cell, false);
				if ((connections & UtilityConnections.Right) != (UtilityConnections)0)
				{
					this.FindConnectedNetworks(Grid.CellRight(cell), mgr, networks, visited);
				}
				if ((connections & UtilityConnections.Left) != (UtilityConnections)0)
				{
					this.FindConnectedNetworks(Grid.CellLeft(cell), mgr, networks, visited);
				}
				if ((connections & UtilityConnections.Up) != (UtilityConnections)0)
				{
					this.FindConnectedNetworks(Grid.CellAbove(cell), mgr, networks, visited);
				}
				if ((connections & UtilityConnections.Down) != (UtilityConnections)0)
				{
					this.FindConnectedNetworks(Grid.CellBelow(cell), mgr, networks, visited);
				}
			}
		}

		private void ResetRibbonSymbolTints<T>(ICollection<T> targets) where T : MonoBehaviour
		{
			foreach (T t in targets)
			{
				if (!(t == null))
				{
					KBatchedAnimController component = t.GetComponent<KBatchedAnimController>();
					this.ResetRibbonTint(component);
				}
			}
		}

		private void ResetRibbonTint(KBatchedAnimController kbac)
		{
			if (kbac != null)
			{
				kbac.SetSymbolTint(OverlayModes.Logic.RIBBON_WIRE_1_SYMBOL_NAME, Color.white);
				kbac.SetSymbolTint(OverlayModes.Logic.RIBBON_WIRE_2_SYMBOL_NAME, Color.white);
				kbac.SetSymbolTint(OverlayModes.Logic.RIBBON_WIRE_3_SYMBOL_NAME, Color.white);
				kbac.SetSymbolTint(OverlayModes.Logic.RIBBON_WIRE_4_SYMBOL_NAME, Color.white);
			}
		}

		public static readonly HashedString ID = "Logic";

		public static HashSet<Tag> HighlightItemIDs = new HashSet<Tag>();

		public static KAnimHashedString RIBBON_WIRE_1_SYMBOL_NAME = "wire1";

		public static KAnimHashedString RIBBON_WIRE_2_SYMBOL_NAME = "wire2";

		public static KAnimHashedString RIBBON_WIRE_3_SYMBOL_NAME = "wire3";

		public static KAnimHashedString RIBBON_WIRE_4_SYMBOL_NAME = "wire4";

		private int conduitTargetLayer;

		private int objectTargetLayer;

		private int cameraLayerMask;

		private int selectionMask;

		private UniformGrid<ILogicUIElement> ioPartition;

		private HashSet<ILogicUIElement> ioTargets = new HashSet<ILogicUIElement>();

		private HashSet<ILogicUIElement> workingIOTargets = new HashSet<ILogicUIElement>();

		private HashSet<KBatchedAnimController> wireControllers = new HashSet<KBatchedAnimController>();

		private HashSet<KBatchedAnimController> ribbonControllers = new HashSet<KBatchedAnimController>();

		private HashSet<UtilityNetwork> connectedNetworks = new HashSet<UtilityNetwork>();

		private List<int> visited = new List<int>();

		private HashSet<OverlayModes.Logic.BridgeInfo> bridgeControllers = new HashSet<OverlayModes.Logic.BridgeInfo>();

		private HashSet<OverlayModes.Logic.BridgeInfo> ribbonBridgeControllers = new HashSet<OverlayModes.Logic.BridgeInfo>();

		private UniformGrid<SaveLoadRoot> gameObjPartition;

		private HashSet<SaveLoadRoot> gameObjTargets = new HashSet<SaveLoadRoot>();

		private LogicModeUI uiAsset;

		private Dictionary<ILogicUIElement, OverlayModes.Logic.EventInfo> uiNodes = new Dictionary<ILogicUIElement, OverlayModes.Logic.EventInfo>();

		private KCompactedVector<OverlayModes.Logic.UIInfo> uiInfo = new KCompactedVector<OverlayModes.Logic.UIInfo>(0);

		private struct BridgeInfo
		{
			public int cell;

			public KBatchedAnimController controller;
		}

		private struct EventInfo
		{
			public HandleVector<int>.Handle uiHandle;
		}

		private struct UIInfo
		{
			public UIInfo(ILogicUIElement ui_elem, LogicModeUI ui_data)
			{
				this.cell = ui_elem.GetLogicUICell();
				GameObject gameObject = null;
				Sprite sprite = null;
				this.bitDepth = 1;
				switch (ui_elem.GetLogicPortSpriteType())
				{
				case LogicPortSpriteType.Input:
					gameObject = ui_data.prefab;
					sprite = ui_data.inputSprite;
					break;
				case LogicPortSpriteType.Output:
					gameObject = ui_data.prefab;
					sprite = ui_data.outputSprite;
					break;
				case LogicPortSpriteType.ResetUpdate:
					gameObject = ui_data.prefab;
					sprite = ui_data.resetSprite;
					break;
				case LogicPortSpriteType.ControlInput:
					gameObject = ui_data.controlInputPrefab;
					break;
				case LogicPortSpriteType.RibbonInput:
					gameObject = ui_data.ribbonInputPrefab;
					this.bitDepth = 4;
					break;
				case LogicPortSpriteType.RibbonOutput:
					gameObject = ui_data.ribbonOutputPrefab;
					this.bitDepth = 4;
					break;
				}
				this.instance = global::Util.KInstantiate(gameObject, Grid.CellToPosCCC(this.cell, Grid.SceneLayer.Front), Quaternion.identity, GameScreenManager.Instance.worldSpaceCanvas, null, true, 0);
				this.instance.SetActive(true);
				this.image = this.instance.GetComponent<Image>();
				if (this.image != null)
				{
					this.image.raycastTarget = false;
					this.image.sprite = sprite;
				}
			}

			public void Release()
			{
				global::Util.KDestroyGameObject(this.instance);
			}

			public GameObject instance;

			public Image image;

			public int cell;

			public int bitDepth;
		}
	}

	public enum BringToFrontLayerSetting
	{
		None,
		Constant,
		Conditional
	}

	public class ColorHighlightCondition
	{
		public ColorHighlightCondition(Func<KMonoBehaviour, Color> highlight_color, Func<KMonoBehaviour, bool> highlight_condition)
		{
			this.highlight_color = highlight_color;
			this.highlight_condition = highlight_condition;
		}

		public Func<KMonoBehaviour, Color> highlight_color;

		public Func<KMonoBehaviour, bool> highlight_condition;
	}

	public class None : OverlayModes.Mode
	{
		public override HashedString ViewMode()
		{
			return OverlayModes.None.ID;
		}

		public override string GetSoundName()
		{
			return "Off";
		}

		public static readonly HashedString ID = HashedString.Invalid;
	}

	public class PathProber : OverlayModes.Mode
	{
		public override HashedString ViewMode()
		{
			return OverlayModes.PathProber.ID;
		}

		public override string GetSoundName()
		{
			return "Off";
		}

		public static readonly HashedString ID = "PathProber";
	}

	public class Oxygen : OverlayModes.Mode
	{
		public override HashedString ViewMode()
		{
			return OverlayModes.Oxygen.ID;
		}

		public override string GetSoundName()
		{
			return "Oxygen";
		}

		public override void Enable()
		{
			base.Enable();
			int defaultLayerMask = SelectTool.Instance.GetDefaultLayerMask();
			int mask = LayerMask.GetMask(new string[] { "MaskedOverlay" });
			SelectTool.Instance.SetLayerMask(defaultLayerMask | mask);
		}

		public override void Disable()
		{
			base.Disable();
			SelectTool.Instance.ClearLayerMask();
		}

		public static readonly HashedString ID = "Oxygen";
	}

	public class Light : OverlayModes.Mode
	{
		public override HashedString ViewMode()
		{
			return OverlayModes.Light.ID;
		}

		public override string GetSoundName()
		{
			return "Lights";
		}

		public static readonly HashedString ID = "Light";
	}

	public class Priorities : OverlayModes.Mode
	{
		public override HashedString ViewMode()
		{
			return OverlayModes.Priorities.ID;
		}

		public override string GetSoundName()
		{
			return "Priorities";
		}

		public static readonly HashedString ID = "Priorities";
	}

	public class ThermalConductivity : OverlayModes.Mode
	{
		public override HashedString ViewMode()
		{
			return OverlayModes.ThermalConductivity.ID;
		}

		public override string GetSoundName()
		{
			return "HeatFlow";
		}

		public static readonly HashedString ID = "ThermalConductivity";
	}

	public class HeatFlow : OverlayModes.Mode
	{
		public override HashedString ViewMode()
		{
			return OverlayModes.HeatFlow.ID;
		}

		public override string GetSoundName()
		{
			return "HeatFlow";
		}

		public static readonly HashedString ID = "HeatFlow";
	}

	public class Rooms : OverlayModes.Mode
	{
		public override HashedString ViewMode()
		{
			return OverlayModes.Rooms.ID;
		}

		public override string GetSoundName()
		{
			return "Rooms";
		}

		public override List<LegendEntry> GetCustomLegendData()
		{
			List<LegendEntry> list = new List<LegendEntry>();
			List<RoomType> list2 = new List<RoomType>(Db.Get().RoomTypes.resources);
			list2.Sort((RoomType a, RoomType b) => a.sortKey.CompareTo(b.sortKey));
			foreach (RoomType roomType in list2)
			{
				string text = roomType.GetCriteriaString();
				if (roomType.effects != null && roomType.effects.Length != 0)
				{
					text = text + "\n\n" + roomType.GetRoomEffectsString();
				}
				list.Add(new LegendEntry(roomType.Name + "\n" + roomType.effect, text, GlobalAssets.Instance.colorSet.GetColorByName(roomType.category.colorName), null, null, true));
			}
			return list;
		}

		public static readonly HashedString ID = "Rooms";
	}

	public abstract class Mode
	{
		public static void Clear()
		{
			OverlayModes.Mode.workingTargets.Clear();
		}

		public abstract HashedString ViewMode();

		public virtual void Enable()
		{
		}

		public virtual void Update()
		{
		}

		public virtual void Disable()
		{
		}

		public virtual List<LegendEntry> GetCustomLegendData()
		{
			return null;
		}

		public virtual Dictionary<string, ToolParameterMenu.ToggleState> CreateDefaultFilters()
		{
			return null;
		}

		public virtual void OnFiltersChanged()
		{
		}

		public virtual void DisableOverlay()
		{
		}

		public virtual void OnRenderImage(RenderTexture src, RenderTexture dest)
		{
		}

		public abstract string GetSoundName();

		protected bool InFilter(string layer, Dictionary<string, ToolParameterMenu.ToggleState> filter)
		{
			return (filter.ContainsKey(ToolParameterMenu.FILTERLAYERS.ALL) && filter[ToolParameterMenu.FILTERLAYERS.ALL] == ToolParameterMenu.ToggleState.On) || (filter.ContainsKey(layer) && filter[layer] == ToolParameterMenu.ToggleState.On);
		}

		public void RegisterSaveLoadListeners()
		{
			SaveManager saveManager = SaveLoader.Instance.saveManager;
			saveManager.onRegister += this.OnSaveLoadRootRegistered;
			saveManager.onUnregister += this.OnSaveLoadRootUnregistered;
		}

		public void UnregisterSaveLoadListeners()
		{
			SaveManager saveManager = SaveLoader.Instance.saveManager;
			saveManager.onRegister -= this.OnSaveLoadRootRegistered;
			saveManager.onUnregister -= this.OnSaveLoadRootUnregistered;
		}

		protected virtual void OnSaveLoadRootRegistered(SaveLoadRoot root)
		{
		}

		protected virtual void OnSaveLoadRootUnregistered(SaveLoadRoot root)
		{
		}

		protected void ProcessExistingSaveLoadRoots()
		{
			foreach (KeyValuePair<Tag, List<SaveLoadRoot>> keyValuePair in SaveLoader.Instance.saveManager.GetLists())
			{
				foreach (SaveLoadRoot saveLoadRoot in keyValuePair.Value)
				{
					this.OnSaveLoadRootRegistered(saveLoadRoot);
				}
			}
		}

		protected static UniformGrid<T> PopulatePartition<T>(ICollection<Tag> tags) where T : IUniformGridObject
		{
			Dictionary<Tag, List<SaveLoadRoot>> lists = SaveLoader.Instance.saveManager.GetLists();
			UniformGrid<T> uniformGrid = new UniformGrid<T>(Grid.WidthInCells, Grid.HeightInCells, 8, 8);
			foreach (Tag tag in tags)
			{
				List<SaveLoadRoot> list = null;
				if (lists.TryGetValue(tag, out list))
				{
					foreach (SaveLoadRoot saveLoadRoot in list)
					{
						T component = saveLoadRoot.GetComponent<T>();
						if (component != null)
						{
							uniformGrid.Add(component);
						}
					}
				}
			}
			return uniformGrid;
		}

		protected static void ResetDisplayValues<T>(ICollection<T> targets) where T : MonoBehaviour
		{
			foreach (T t in targets)
			{
				if (!(t == null))
				{
					KBatchedAnimController component = t.GetComponent<KBatchedAnimController>();
					if (component != null)
					{
						OverlayModes.Mode.ResetDisplayValues(component);
					}
				}
			}
		}

		protected static void ResetDisplayValues(KBatchedAnimController controller)
		{
			controller.SetLayer(0);
			controller.HighlightColour = Color.clear;
			controller.TintColour = Color.white;
			controller.SetLayer(controller.GetComponent<KPrefabID>().defaultLayer);
		}

		protected static void RemoveOffscreenTargets<T>(ICollection<T> targets, Vector2I min, Vector2I max, Action<T> on_removed = null) where T : KMonoBehaviour
		{
			OverlayModes.Mode.ClearOutsideViewObjects<T>(targets, min, max, null, delegate(T cmp)
			{
				if (cmp != null)
				{
					KBatchedAnimController component = cmp.GetComponent<KBatchedAnimController>();
					if (component != null)
					{
						OverlayModes.Mode.ResetDisplayValues(component);
					}
					if (on_removed != null)
					{
						on_removed(cmp);
					}
				}
			});
			OverlayModes.Mode.workingTargets.Clear();
		}

		protected static void ClearOutsideViewObjects<T>(ICollection<T> targets, Vector2I vis_min, Vector2I vis_max, ICollection<Tag> item_ids, Action<T> on_remove) where T : KMonoBehaviour
		{
			OverlayModes.Mode.workingTargets.Clear();
			foreach (T t in targets)
			{
				if (!(t == null))
				{
					Vector2I vector2I = Grid.PosToXY(t.transform.GetPosition());
					if (!(vis_min <= vector2I) || !(vector2I <= vis_max) || t.gameObject.GetMyWorldId() != ClusterManager.Instance.activeWorldId)
					{
						OverlayModes.Mode.workingTargets.Add(t);
					}
					else
					{
						KPrefabID component = t.GetComponent<KPrefabID>();
						if (item_ids != null && !item_ids.Contains(component.PrefabTag) && t.gameObject.GetMyWorldId() != ClusterManager.Instance.activeWorldId)
						{
							OverlayModes.Mode.workingTargets.Add(t);
						}
					}
				}
			}
			foreach (KMonoBehaviour kmonoBehaviour in OverlayModes.Mode.workingTargets)
			{
				T t2 = (T)((object)kmonoBehaviour);
				if (!(t2 == null))
				{
					if (on_remove != null)
					{
						on_remove(t2);
					}
					targets.Remove(t2);
				}
			}
			OverlayModes.Mode.workingTargets.Clear();
		}

		protected static void RemoveOffscreenTargets<T>(ICollection<T> targets, ICollection<T> working_targets, Vector2I vis_min, Vector2I vis_max, Action<T> on_removed = null, Func<T, bool> special_clear_condition = null) where T : IUniformGridObject
		{
			OverlayModes.Mode.ClearOutsideViewObjects<T>(targets, working_targets, vis_min, vis_max, delegate(T cmp)
			{
				if (cmp != null && on_removed != null)
				{
					on_removed(cmp);
				}
			});
			if (special_clear_condition != null)
			{
				working_targets.Clear();
				foreach (T t in targets)
				{
					if (special_clear_condition(t))
					{
						working_targets.Add(t);
					}
				}
				foreach (T t2 in working_targets)
				{
					if (t2 != null)
					{
						if (on_removed != null)
						{
							on_removed(t2);
						}
						targets.Remove(t2);
					}
				}
				working_targets.Clear();
			}
		}

		protected static void ClearOutsideViewObjects<T>(ICollection<T> targets, ICollection<T> working_targets, Vector2I vis_min, Vector2I vis_max, Action<T> on_removed = null) where T : IUniformGridObject
		{
			working_targets.Clear();
			foreach (T t in targets)
			{
				if (t != null)
				{
					Vector2 vector = t.PosMin();
					Vector2 vector2 = t.PosMin();
					if (vector2.x < (float)vis_min.x || vector2.y < (float)vis_min.y || (float)vis_max.x < vector.x || (float)vis_max.y < vector.y)
					{
						working_targets.Add(t);
					}
				}
			}
			foreach (T t2 in working_targets)
			{
				if (t2 != null)
				{
					if (on_removed != null)
					{
						on_removed(t2);
					}
					targets.Remove(t2);
				}
			}
			working_targets.Clear();
		}

		protected static float GetDefaultDepth(KMonoBehaviour cmp)
		{
			BuildingComplete component = cmp.GetComponent<BuildingComplete>();
			float num;
			if (component != null)
			{
				num = Grid.GetLayerZ(component.Def.SceneLayer);
			}
			else
			{
				num = Grid.GetLayerZ(Grid.SceneLayer.Creatures);
			}
			return num;
		}

		protected void UpdateHighlightTypeOverlay<T>(Vector2I min, Vector2I max, ICollection<T> targets, ICollection<Tag> item_ids, OverlayModes.ColorHighlightCondition[] highlights, OverlayModes.BringToFrontLayerSetting bringToFrontSetting, int layer) where T : KMonoBehaviour
		{
			foreach (T t in targets)
			{
				if (!(t == null))
				{
					Vector3 position = t.transform.GetPosition();
					int num = Grid.PosToCell(position);
					if (Grid.IsValidCell(num) && Grid.IsVisible(num) && min <= position && position <= max)
					{
						KBatchedAnimController component = t.GetComponent<KBatchedAnimController>();
						if (!(component == null))
						{
							int num2 = 0;
							Color32 color = Color.clear;
							if (highlights != null)
							{
								foreach (OverlayModes.ColorHighlightCondition colorHighlightCondition in highlights)
								{
									if (colorHighlightCondition.highlight_condition(t))
									{
										color = colorHighlightCondition.highlight_color(t);
										num2 = layer;
										break;
									}
								}
							}
							if (bringToFrontSetting != OverlayModes.BringToFrontLayerSetting.Constant)
							{
								if (bringToFrontSetting == OverlayModes.BringToFrontLayerSetting.Conditional)
								{
									component.SetLayer(num2);
								}
							}
							else
							{
								component.SetLayer(layer);
							}
							component.HighlightColour = color;
						}
					}
				}
			}
		}

		protected void DisableHighlightTypeOverlay<T>(ICollection<T> targets) where T : KMonoBehaviour
		{
			Color32 color = Color.clear;
			foreach (T t in targets)
			{
				if (!(t == null))
				{
					KBatchedAnimController component = t.GetComponent<KBatchedAnimController>();
					if (component != null)
					{
						component.HighlightColour = color;
						component.SetLayer(0);
					}
				}
			}
			targets.Clear();
		}

		protected void AddTargetIfVisible<T>(T instance, Vector2I vis_min, Vector2I vis_max, ICollection<T> targets, int layer, Action<T> on_added = null, Func<KMonoBehaviour, bool> should_add = null) where T : IUniformGridObject
		{
			if (instance.Equals(null))
			{
				return;
			}
			Vector2 vector = instance.PosMin();
			Vector2 vector2 = instance.PosMax();
			if (vector2.x < (float)vis_min.x || vector2.y < (float)vis_min.y || vector.x > (float)vis_max.x || vector.y > (float)vis_max.y)
			{
				return;
			}
			if (targets.Contains(instance))
			{
				return;
			}
			bool flag = false;
			int num = (int)vector.y;
			while ((float)num <= vector2.y)
			{
				int num2 = (int)vector.x;
				while ((float)num2 <= vector2.x)
				{
					int num3 = Grid.XYToCell(num2, num);
					if ((Grid.IsValidCell(num3) && Grid.Visible[num3] > 20 && (int)Grid.WorldIdx[num3] == ClusterManager.Instance.activeWorldId) || !PropertyTextures.IsFogOfWarEnabled)
					{
						flag = true;
						break;
					}
					num2++;
				}
				num++;
			}
			if (flag)
			{
				bool flag2 = true;
				KMonoBehaviour kmonoBehaviour = instance as KMonoBehaviour;
				if (kmonoBehaviour != null && should_add != null)
				{
					flag2 = should_add(kmonoBehaviour);
				}
				if (flag2)
				{
					if (kmonoBehaviour != null)
					{
						KBatchedAnimController component = kmonoBehaviour.GetComponent<KBatchedAnimController>();
						if (component != null)
						{
							component.SetLayer(layer);
						}
					}
					targets.Add(instance);
					if (on_added != null)
					{
						on_added(instance);
					}
				}
			}
		}

		public Dictionary<string, ToolParameterMenu.ToggleState> legendFilters;

		private static List<KMonoBehaviour> workingTargets = new List<KMonoBehaviour>();
	}

	public class ModeUtil
	{
		public static float GetHighlightScale()
		{
			return Mathf.SmoothStep(0.5f, 1f, Mathf.Abs(Mathf.Sin(Time.unscaledTime * 4f)));
		}
	}

	public class Power : OverlayModes.Mode
	{
		public override HashedString ViewMode()
		{
			return OverlayModes.Power.ID;
		}

		public override string GetSoundName()
		{
			return "Power";
		}

		public Power(Canvas powerLabelParent, LocText powerLabelPrefab, BatteryUI batteryUIPrefab, Vector3 powerLabelOffset, Vector3 batteryUIOffset, Vector3 batteryUITransformerOffset, Vector3 batteryUISmallTransformerOffset)
		{
			this.powerLabelParent = powerLabelParent;
			this.powerLabelPrefab = powerLabelPrefab;
			this.batteryUIPrefab = batteryUIPrefab;
			this.powerLabelOffset = powerLabelOffset;
			this.batteryUIOffset = batteryUIOffset;
			this.batteryUITransformerOffset = batteryUITransformerOffset;
			this.batteryUISmallTransformerOffset = batteryUISmallTransformerOffset;
			this.targetLayer = LayerMask.NameToLayer("MaskedOverlay");
			this.cameraLayerMask = LayerMask.GetMask(new string[] { "MaskedOverlay", "MaskedOverlayBG" });
			this.selectionMask = this.cameraLayerMask;
		}

		public override void Enable()
		{
			Camera.main.cullingMask |= this.cameraLayerMask;
			SelectTool.Instance.SetLayerMask(this.selectionMask);
			base.RegisterSaveLoadListeners();
			this.partition = OverlayModes.Mode.PopulatePartition<SaveLoadRoot>(OverlayScreen.WireIDs);
			GridCompositor.Instance.ToggleMinor(true);
		}

		public override void Disable()
		{
			OverlayModes.Mode.ResetDisplayValues<SaveLoadRoot>(this.layerTargets);
			Camera.main.cullingMask &= ~this.cameraLayerMask;
			SelectTool.Instance.ClearLayerMask();
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
			OverlayModes.Mode.RemoveOffscreenTargets<SaveLoadRoot>(this.layerTargets, vector2I, vector2I2, null);
			using (new KProfiler.Region("UpdatePowerOverlay", null))
			{
				foreach (object obj in this.partition.GetAllIntersecting(new Vector2((float)vector2I.x, (float)vector2I.y), new Vector2((float)vector2I2.x, (float)vector2I2.y)))
				{
					SaveLoadRoot saveLoadRoot = (SaveLoadRoot)obj;
					base.AddTargetIfVisible<SaveLoadRoot>(saveLoadRoot, vector2I, vector2I2, this.layerTargets, this.targetLayer, null, null);
				}
				this.connectedNetworks.Clear();
				float num = 1f;
				GameObject gameObject = null;
				if (SelectTool.Instance != null && SelectTool.Instance.hover != null)
				{
					gameObject = SelectTool.Instance.hover.gameObject;
				}
				if (gameObject != null)
				{
					IBridgedNetworkItem component = gameObject.GetComponent<IBridgedNetworkItem>();
					if (component != null)
					{
						int networkCell = component.GetNetworkCell();
						this.visited.Clear();
						this.FindConnectedNetworks(networkCell, Game.Instance.electricalConduitSystem, this.connectedNetworks, this.visited);
						this.visited.Clear();
						num = OverlayModes.ModeUtil.GetHighlightScale();
					}
				}
				CircuitManager circuitManager = Game.Instance.circuitManager;
				foreach (SaveLoadRoot saveLoadRoot2 in this.layerTargets)
				{
					if (!(saveLoadRoot2 == null))
					{
						IBridgedNetworkItem component2 = saveLoadRoot2.GetComponent<IBridgedNetworkItem>();
						if (component2 != null)
						{
							KAnimControllerBase component3 = (component2 as KMonoBehaviour).GetComponent<KBatchedAnimController>();
							int networkCell2 = component2.GetNetworkCell();
							UtilityNetwork networkForCell = Game.Instance.electricalConduitSystem.GetNetworkForCell(networkCell2);
							ushort num2 = ((networkForCell != null) ? ((ushort)networkForCell.id) : ushort.MaxValue);
							float wattsUsedByCircuit = circuitManager.GetWattsUsedByCircuit(num2);
							float num3 = circuitManager.GetMaxSafeWattageForCircuit(num2);
							num3 += POWER.FLOAT_FUDGE_FACTOR;
							float wattsNeededWhenActive = circuitManager.GetWattsNeededWhenActive(num2);
							Color32 color;
							if (wattsUsedByCircuit <= 0f)
							{
								color = GlobalAssets.Instance.colorSet.powerCircuitUnpowered;
							}
							else if (wattsUsedByCircuit > num3)
							{
								color = GlobalAssets.Instance.colorSet.powerCircuitOverloading;
							}
							else if (wattsNeededWhenActive > num3 && num3 > 0f && wattsUsedByCircuit / num3 >= 0.75f)
							{
								color = GlobalAssets.Instance.colorSet.powerCircuitStraining;
							}
							else
							{
								color = GlobalAssets.Instance.colorSet.powerCircuitSafe;
							}
							if (this.connectedNetworks.Count > 0 && component2.IsConnectedToNetworks(this.connectedNetworks))
							{
								color.r = (byte)((float)color.r * num);
								color.g = (byte)((float)color.g * num);
								color.b = (byte)((float)color.b * num);
							}
							component3.TintColour = color;
						}
					}
				}
			}
			this.queuedAdds.Clear();
			using (new KProfiler.Region("BatteryUI", null))
			{
				foreach (Battery battery in Components.Batteries.Items)
				{
					Vector2I vector2I3 = Grid.PosToXY(battery.transform.GetPosition());
					if (vector2I <= vector2I3 && vector2I3 <= vector2I2 && battery.GetMyWorldId() == ClusterManager.Instance.activeWorldId)
					{
						SaveLoadRoot component4 = battery.GetComponent<SaveLoadRoot>();
						if (!this.privateTargets.Contains(component4))
						{
							this.AddBatteryUI(battery);
							this.queuedAdds.Add(component4);
						}
					}
				}
				foreach (Generator generator in Components.Generators.Items)
				{
					Vector2I vector2I4 = Grid.PosToXY(generator.transform.GetPosition());
					if (vector2I <= vector2I4 && vector2I4 <= vector2I2 && generator.GetMyWorldId() == ClusterManager.Instance.activeWorldId)
					{
						SaveLoadRoot component5 = generator.GetComponent<SaveLoadRoot>();
						if (!this.privateTargets.Contains(component5))
						{
							this.privateTargets.Add(component5);
							if (generator.GetComponent<PowerTransformer>() == null)
							{
								this.AddPowerLabels(generator);
							}
						}
					}
				}
				foreach (EnergyConsumer energyConsumer in Components.EnergyConsumers.Items)
				{
					Vector2I vector2I5 = Grid.PosToXY(energyConsumer.transform.GetPosition());
					if (vector2I <= vector2I5 && vector2I5 <= vector2I2 && energyConsumer.GetMyWorldId() == ClusterManager.Instance.activeWorldId)
					{
						SaveLoadRoot component6 = energyConsumer.GetComponent<SaveLoadRoot>();
						if (!this.privateTargets.Contains(component6))
						{
							this.privateTargets.Add(component6);
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
				locText = global::Util.KInstantiateUI<LocText>(this.powerLabelPrefab.gameObject, this.powerLabelParent.transform.gameObject, false);
				this.powerLabels.Add(locText);
				this.freePowerLabelIdx++;
			}
			return locText;
		}

		private void UpdatePowerLabels()
		{
			foreach (OverlayModes.Power.UpdatePowerInfo updatePowerInfo in this.updatePowerInfo)
			{
				KMonoBehaviour item = updatePowerInfo.item;
				LocText powerLabel = updatePowerInfo.powerLabel;
				LocText unitLabel = updatePowerInfo.unitLabel;
				Generator generator = updatePowerInfo.generator;
				IEnergyConsumer consumer = updatePowerInfo.consumer;
				if (updatePowerInfo.item == null || updatePowerInfo.item.gameObject.GetMyWorldId() != ClusterManager.Instance.activeWorldId)
				{
					powerLabel.gameObject.SetActive(false);
				}
				else
				{
					powerLabel.gameObject.SetActive(true);
					if (generator != null && consumer == null)
					{
						int num;
						if (generator.GetComponent<ManualGenerator>() == null)
						{
							generator.GetComponent<Operational>();
							num = Mathf.Max(0, Mathf.RoundToInt(generator.WattageRating));
						}
						else
						{
							num = Mathf.Max(0, Mathf.RoundToInt(generator.WattageRating));
						}
						powerLabel.text = ((num != 0) ? ("+" + num.ToString()) : num.ToString());
						BuildingEnabledButton component = item.GetComponent<BuildingEnabledButton>();
						Color color = ((component != null && !component.IsEnabled) ? GlobalAssets.Instance.colorSet.powerBuildingDisabled : GlobalAssets.Instance.colorSet.powerGenerator);
						powerLabel.color = color;
						unitLabel.color = color;
						BuildingCellVisualizer component2 = generator.GetComponent<BuildingCellVisualizer>();
						if (component2 != null)
						{
							Image powerOutputIcon = component2.GetPowerOutputIcon();
							if (powerOutputIcon != null)
							{
								powerOutputIcon.color = color;
							}
						}
					}
					if (consumer != null)
					{
						BuildingEnabledButton component3 = item.GetComponent<BuildingEnabledButton>();
						Color color2 = ((component3 != null && !component3.IsEnabled) ? GlobalAssets.Instance.colorSet.powerBuildingDisabled : GlobalAssets.Instance.colorSet.powerConsumer);
						int num2 = Mathf.Max(0, Mathf.RoundToInt(consumer.WattsNeededWhenActive));
						string text = num2.ToString();
						powerLabel.text = ((num2 != 0) ? ("-" + text) : text);
						powerLabel.color = color2;
						unitLabel.color = color2;
						Image powerInputIcon = item.GetComponentInChildren<BuildingCellVisualizer>().GetPowerInputIcon();
						if (powerInputIcon != null)
						{
							powerInputIcon.color = color2;
						}
					}
				}
			}
			foreach (OverlayModes.Power.UpdateBatteryInfo updateBatteryInfo in this.updateBatteryInfo)
			{
				updateBatteryInfo.ui.SetContent(updateBatteryInfo.battery);
			}
		}

		private void AddPowerLabels(KMonoBehaviour item)
		{
			if (item.gameObject.GetMyWorldId() == ClusterManager.Instance.activeWorldId)
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
						freePowerLabel.rectTransform.SetPosition(vector + this.powerLabelOffset + Vector3.up * (num * 0.02f));
						if (componentInChildren != null && componentInChildren.PowerCell == componentInChildren2.PowerCell)
						{
							num -= 15f;
						}
						this.SetToolTip(freePowerLabel, UI.OVERLAYS.POWER.WATTS_GENERATED);
						this.updatePowerInfo.Add(new OverlayModes.Power.UpdatePowerInfo(item, freePowerLabel, component, componentInChildren2, null));
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
						freePowerLabel2.rectTransform.SetPosition(vector2 + this.powerLabelOffset + Vector3.up * (num * 0.02f));
						this.SetToolTip(freePowerLabel2, UI.OVERLAYS.POWER.WATTS_CONSUMED);
						this.updatePowerInfo.Add(new OverlayModes.Power.UpdatePowerInfo(item, freePowerLabel2, component2, null, componentInChildren));
					}
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
			bool flag = bat.powerTransformer != null;
			float num = 1f;
			Rotatable component = bat.GetComponent<Rotatable>();
			if (component != null && component.GetVisualizerFlipX())
			{
				num = -1f;
			}
			Vector3 vector2 = this.batteryUIOffset;
			if (flag)
			{
				vector2 = ((bat.GetComponent<Building>().Def.WidthInCells == 2) ? this.batteryUISmallTransformerOffset : this.batteryUITransformerOffset);
			}
			vector2.x *= num;
			freeBatteryUI.GetComponent<RectTransform>().SetPosition(Vector3.up + vector + vector2);
			this.updateBatteryInfo.Add(new OverlayModes.Power.UpdateBatteryInfo(bat, freeBatteryUI));
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
				batteryUI = global::Util.KInstantiateUI<BatteryUI>(this.batteryUIPrefab.gameObject, this.powerLabelParent.transform.gameObject, false);
				this.batteryUIList.Add(batteryUI);
				this.freeBatteryUIIdx++;
			}
			return batteryUI;
		}

		private void FindConnectedNetworks(int cell, IUtilityNetworkMgr mgr, ICollection<UtilityNetwork> networks, List<int> visited)
		{
			if (visited.Contains(cell))
			{
				return;
			}
			visited.Add(cell);
			UtilityNetwork networkForCell = mgr.GetNetworkForCell(cell);
			if (networkForCell != null)
			{
				networks.Add(networkForCell);
				UtilityConnections connections = mgr.GetConnections(cell, false);
				if ((connections & UtilityConnections.Right) != (UtilityConnections)0)
				{
					this.FindConnectedNetworks(Grid.CellRight(cell), mgr, networks, visited);
				}
				if ((connections & UtilityConnections.Left) != (UtilityConnections)0)
				{
					this.FindConnectedNetworks(Grid.CellLeft(cell), mgr, networks, visited);
				}
				if ((connections & UtilityConnections.Up) != (UtilityConnections)0)
				{
					this.FindConnectedNetworks(Grid.CellAbove(cell), mgr, networks, visited);
				}
				if ((connections & UtilityConnections.Down) != (UtilityConnections)0)
				{
					this.FindConnectedNetworks(Grid.CellBelow(cell), mgr, networks, visited);
				}
			}
		}

		public static readonly HashedString ID = "Power";

		private int targetLayer;

		private int cameraLayerMask;

		private int selectionMask;

		private List<OverlayModes.Power.UpdatePowerInfo> updatePowerInfo = new List<OverlayModes.Power.UpdatePowerInfo>();

		private List<OverlayModes.Power.UpdateBatteryInfo> updateBatteryInfo = new List<OverlayModes.Power.UpdateBatteryInfo>();

		private Canvas powerLabelParent;

		private LocText powerLabelPrefab;

		private Vector3 powerLabelOffset;

		private BatteryUI batteryUIPrefab;

		private Vector3 batteryUIOffset;

		private Vector3 batteryUITransformerOffset;

		private Vector3 batteryUISmallTransformerOffset;

		private int freePowerLabelIdx;

		private int freeBatteryUIIdx;

		private List<LocText> powerLabels = new List<LocText>();

		private List<BatteryUI> batteryUIList = new List<BatteryUI>();

		private UniformGrid<SaveLoadRoot> partition;

		private List<SaveLoadRoot> queuedAdds = new List<SaveLoadRoot>();

		private HashSet<SaveLoadRoot> layerTargets = new HashSet<SaveLoadRoot>();

		private HashSet<SaveLoadRoot> privateTargets = new HashSet<SaveLoadRoot>();

		private HashSet<UtilityNetwork> connectedNetworks = new HashSet<UtilityNetwork>();

		private List<int> visited = new List<int>();

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

	public class Radiation : OverlayModes.Mode
	{
		public override HashedString ViewMode()
		{
			return OverlayModes.Radiation.ID;
		}

		public override string GetSoundName()
		{
			return "Radiation";
		}

		public override void Enable()
		{
			AudioMixer.instance.Start(AudioMixerSnapshots.Get().TechFilterRadiationOn);
		}

		public override void Disable()
		{
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().TechFilterRadiationOn, STOP_MODE.ALLOWFADEOUT);
		}

		public static readonly HashedString ID = "Radiation";
	}

	public class SolidConveyor : OverlayModes.Mode
	{
		public override HashedString ViewMode()
		{
			return OverlayModes.SolidConveyor.ID;
		}

		public override string GetSoundName()
		{
			return "LiquidVent";
		}

		public SolidConveyor()
		{
			this.targetLayer = LayerMask.NameToLayer("MaskedOverlay");
			this.cameraLayerMask = LayerMask.GetMask(new string[] { "MaskedOverlay", "MaskedOverlayBG" });
			this.selectionMask = this.cameraLayerMask;
		}

		public override void Enable()
		{
			base.RegisterSaveLoadListeners();
			this.partition = OverlayModes.Mode.PopulatePartition<SaveLoadRoot>(this.targetIDs);
			Camera.main.cullingMask |= this.cameraLayerMask;
			SelectTool.Instance.SetLayerMask(this.selectionMask);
			GridCompositor.Instance.ToggleMinor(false);
			base.Enable();
		}

		protected override void OnSaveLoadRootRegistered(SaveLoadRoot item)
		{
			Tag saveLoadTag = item.GetComponent<KPrefabID>().GetSaveLoadTag();
			if (this.targetIDs.Contains(saveLoadTag))
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

		public override void Disable()
		{
			OverlayModes.Mode.ResetDisplayValues<SaveLoadRoot>(this.layerTargets);
			Camera.main.cullingMask &= ~this.cameraLayerMask;
			SelectTool.Instance.ClearLayerMask();
			base.UnregisterSaveLoadListeners();
			this.partition.Clear();
			this.layerTargets.Clear();
			GridCompositor.Instance.ToggleMinor(false);
			base.Disable();
		}

		public override void Update()
		{
			Vector2I vector2I;
			Vector2I vector2I2;
			Grid.GetVisibleExtents(out vector2I, out vector2I2);
			OverlayModes.Mode.RemoveOffscreenTargets<SaveLoadRoot>(this.layerTargets, vector2I, vector2I2, null);
			foreach (object obj in this.partition.GetAllIntersecting(new Vector2((float)vector2I.x, (float)vector2I.y), new Vector2((float)vector2I2.x, (float)vector2I2.y)))
			{
				SaveLoadRoot saveLoadRoot = (SaveLoadRoot)obj;
				base.AddTargetIfVisible<SaveLoadRoot>(saveLoadRoot, vector2I, vector2I2, this.layerTargets, this.targetLayer, null, null);
			}
			GameObject gameObject = null;
			if (SelectTool.Instance != null && SelectTool.Instance.hover != null)
			{
				gameObject = SelectTool.Instance.hover.gameObject;
			}
			this.connectedNetworks.Clear();
			float num = 1f;
			if (gameObject != null)
			{
				SolidConduit component = gameObject.GetComponent<SolidConduit>();
				if (component != null)
				{
					int num2 = Grid.PosToCell(component);
					UtilityNetworkManager<FlowUtilityNetwork, SolidConduit> solidConduitSystem = Game.Instance.solidConduitSystem;
					this.visited.Clear();
					this.FindConnectedNetworks(num2, solidConduitSystem, this.connectedNetworks, this.visited);
					this.visited.Clear();
					num = OverlayModes.ModeUtil.GetHighlightScale();
				}
			}
			foreach (SaveLoadRoot saveLoadRoot2 in this.layerTargets)
			{
				if (!(saveLoadRoot2 == null))
				{
					Color32 color = this.tint_color;
					SolidConduit component2 = saveLoadRoot2.GetComponent<SolidConduit>();
					if (component2 != null)
					{
						if (this.connectedNetworks.Count > 0 && this.IsConnectedToNetworks(component2, this.connectedNetworks))
						{
							color.r = (byte)((float)color.r * num);
							color.g = (byte)((float)color.g * num);
							color.b = (byte)((float)color.b * num);
						}
						saveLoadRoot2.GetComponent<KBatchedAnimController>().TintColour = color;
					}
				}
			}
		}

		public bool IsConnectedToNetworks(SolidConduit conduit, ICollection<UtilityNetwork> networks)
		{
			UtilityNetwork network = conduit.GetNetwork();
			return networks.Contains(network);
		}

		private void FindConnectedNetworks(int cell, IUtilityNetworkMgr mgr, ICollection<UtilityNetwork> networks, List<int> visited)
		{
			if (visited.Contains(cell))
			{
				return;
			}
			visited.Add(cell);
			UtilityNetwork networkForCell = mgr.GetNetworkForCell(cell);
			if (networkForCell != null)
			{
				networks.Add(networkForCell);
				UtilityConnections connections = mgr.GetConnections(cell, false);
				if ((connections & UtilityConnections.Right) != (UtilityConnections)0)
				{
					this.FindConnectedNetworks(Grid.CellRight(cell), mgr, networks, visited);
				}
				if ((connections & UtilityConnections.Left) != (UtilityConnections)0)
				{
					this.FindConnectedNetworks(Grid.CellLeft(cell), mgr, networks, visited);
				}
				if ((connections & UtilityConnections.Up) != (UtilityConnections)0)
				{
					this.FindConnectedNetworks(Grid.CellAbove(cell), mgr, networks, visited);
				}
				if ((connections & UtilityConnections.Down) != (UtilityConnections)0)
				{
					this.FindConnectedNetworks(Grid.CellBelow(cell), mgr, networks, visited);
				}
				object endpoint = mgr.GetEndpoint(cell);
				if (endpoint != null)
				{
					FlowUtilityNetwork.NetworkItem networkItem = endpoint as FlowUtilityNetwork.NetworkItem;
					if (networkItem != null)
					{
						GameObject gameObject = networkItem.GameObject;
						if (gameObject != null)
						{
							IBridgedNetworkItem component = gameObject.GetComponent<IBridgedNetworkItem>();
							if (component != null)
							{
								component.AddNetworks(networks);
							}
						}
					}
				}
			}
		}

		public static readonly HashedString ID = "SolidConveyor";

		private UniformGrid<SaveLoadRoot> partition;

		private HashSet<SaveLoadRoot> layerTargets = new HashSet<SaveLoadRoot>();

		private ICollection<Tag> targetIDs = OverlayScreen.SolidConveyorIDs;

		private Color32 tint_color = new Color32(201, 201, 201, 0);

		private HashSet<UtilityNetwork> connectedNetworks = new HashSet<UtilityNetwork>();

		private List<int> visited = new List<int>();

		private int targetLayer;

		private int cameraLayerMask;

		private int selectionMask;
	}

	public class Sound : OverlayModes.Mode
	{
		public override HashedString ViewMode()
		{
			return OverlayModes.Sound.ID;
		}

		public override string GetSoundName()
		{
			return "Sound";
		}

		public Sound()
		{
			OverlayModes.ColorHighlightCondition[] array = new OverlayModes.ColorHighlightCondition[1];
			array[0] = new OverlayModes.ColorHighlightCondition(delegate(KMonoBehaviour np)
			{
				Color black = Color.black;
				Color black2 = Color.black;
				float num = 0.8f;
				if (np != null)
				{
					int num2 = Grid.PosToCell(CameraController.Instance.baseCamera.ScreenToWorldPoint(KInputManager.GetMousePos()));
					if ((np as NoisePolluter).GetNoiseForCell(num2) < 36f)
					{
						num = 1f;
						black2 = new Color(0.4f, 0.4f, 0.4f);
					}
				}
				return Color.Lerp(black, black2, num);
			}, delegate(KMonoBehaviour np)
			{
				List<GameObject> highlightedObjects = SelectToolHoverTextCard.highlightedObjects;
				bool flag = false;
				for (int i = 0; i < highlightedObjects.Count; i++)
				{
					if (highlightedObjects[i] != null && highlightedObjects[i] == np.gameObject)
					{
						flag = true;
						break;
					}
				}
				return flag;
			});
			this.highlightConditions = array;
			base..ctor();
			this.targetLayer = LayerMask.NameToLayer("MaskedOverlay");
			this.cameraLayerMask = LayerMask.GetMask(new string[] { "MaskedOverlay", "MaskedOverlayBG" });
			List<Tag> prefabTagsWithComponent = Assets.GetPrefabTagsWithComponent<NoisePolluter>();
			this.targetIDs.UnionWith(prefabTagsWithComponent);
		}

		public override void Enable()
		{
			base.RegisterSaveLoadListeners();
			List<Tag> prefabTagsWithComponent = Assets.GetPrefabTagsWithComponent<NoisePolluter>();
			this.targetIDs.UnionWith(prefabTagsWithComponent);
			this.partition = OverlayModes.Mode.PopulatePartition<NoisePolluter>(this.targetIDs);
			Camera.main.cullingMask |= this.cameraLayerMask;
		}

		public override void Update()
		{
			Vector2I vector2I;
			Vector2I vector2I2;
			Grid.GetVisibleExtents(out vector2I, out vector2I2);
			OverlayModes.Mode.RemoveOffscreenTargets<NoisePolluter>(this.layerTargets, vector2I, vector2I2, null);
			foreach (object obj in this.partition.GetAllIntersecting(new Vector2((float)vector2I.x, (float)vector2I.y), new Vector2((float)vector2I2.x, (float)vector2I2.y)))
			{
				NoisePolluter noisePolluter = (NoisePolluter)obj;
				base.AddTargetIfVisible<NoisePolluter>(noisePolluter, vector2I, vector2I2, this.layerTargets, this.targetLayer, null, null);
			}
			base.UpdateHighlightTypeOverlay<NoisePolluter>(vector2I, vector2I2, this.layerTargets, this.targetIDs, this.highlightConditions, OverlayModes.BringToFrontLayerSetting.Conditional, this.targetLayer);
		}

		protected override void OnSaveLoadRootRegistered(SaveLoadRoot item)
		{
			Tag saveLoadTag = item.GetComponent<KPrefabID>().GetSaveLoadTag();
			if (this.targetIDs.Contains(saveLoadTag))
			{
				NoisePolluter component = item.GetComponent<NoisePolluter>();
				this.partition.Add(component);
			}
		}

		protected override void OnSaveLoadRootUnregistered(SaveLoadRoot item)
		{
			if (item == null || item.gameObject == null)
			{
				return;
			}
			NoisePolluter component = item.GetComponent<NoisePolluter>();
			if (this.layerTargets.Contains(component))
			{
				this.layerTargets.Remove(component);
			}
			this.partition.Remove(component);
		}

		public override void Disable()
		{
			base.DisableHighlightTypeOverlay<NoisePolluter>(this.layerTargets);
			Camera.main.cullingMask &= ~this.cameraLayerMask;
			base.UnregisterSaveLoadListeners();
			this.partition.Clear();
			this.layerTargets.Clear();
		}

		public static readonly HashedString ID = "Sound";

		private UniformGrid<NoisePolluter> partition;

		private HashSet<NoisePolluter> layerTargets = new HashSet<NoisePolluter>();

		private HashSet<Tag> targetIDs = new HashSet<Tag>();

		private int targetLayer;

		private int cameraLayerMask;

		private OverlayModes.ColorHighlightCondition[] highlightConditions;
	}

	public class Suit : OverlayModes.Mode
	{
		public override HashedString ViewMode()
		{
			return OverlayModes.Suit.ID;
		}

		public override string GetSoundName()
		{
			return "SuitRequired";
		}

		public Suit(Canvas ui_parent, GameObject overlay_prefab)
		{
			this.targetLayer = LayerMask.NameToLayer("MaskedOverlay");
			this.cameraLayerMask = LayerMask.GetMask(new string[] { "MaskedOverlay", "MaskedOverlayBG" });
			this.selectionMask = this.cameraLayerMask;
			this.targetIDs = OverlayScreen.SuitIDs;
			this.uiParent = ui_parent;
			this.overlayPrefab = overlay_prefab;
		}

		public override void Enable()
		{
			this.partition = new UniformGrid<SaveLoadRoot>(Grid.WidthInCells, Grid.HeightInCells, 8, 8);
			base.ProcessExistingSaveLoadRoots();
			base.RegisterSaveLoadListeners();
			Camera.main.cullingMask |= this.cameraLayerMask;
			SelectTool.Instance.SetLayerMask(this.selectionMask);
			GridCompositor.Instance.ToggleMinor(false);
			base.Enable();
		}

		public override void Disable()
		{
			base.UnregisterSaveLoadListeners();
			OverlayModes.Mode.ResetDisplayValues<SaveLoadRoot>(this.layerTargets);
			Camera.main.cullingMask &= ~this.cameraLayerMask;
			SelectTool.Instance.ClearLayerMask();
			this.partition.Clear();
			this.partition = null;
			this.layerTargets.Clear();
			for (int i = 0; i < this.uiList.Count; i++)
			{
				this.uiList[i].SetActive(false);
			}
			GridCompositor.Instance.ToggleMinor(false);
			base.Disable();
		}

		protected override void OnSaveLoadRootRegistered(SaveLoadRoot item)
		{
			Tag saveLoadTag = item.GetComponent<KPrefabID>().GetSaveLoadTag();
			if (this.targetIDs.Contains(saveLoadTag))
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

		private GameObject GetFreeUI()
		{
			GameObject gameObject;
			if (this.freeUiIdx >= this.uiList.Count)
			{
				gameObject = global::Util.KInstantiateUI(this.overlayPrefab, this.uiParent.transform.gameObject, false);
				this.uiList.Add(gameObject);
			}
			else
			{
				List<GameObject> list = this.uiList;
				int num = this.freeUiIdx;
				this.freeUiIdx = num + 1;
				gameObject = list[num];
			}
			if (!gameObject.activeSelf)
			{
				gameObject.SetActive(true);
			}
			return gameObject;
		}

		public override void Update()
		{
			this.freeUiIdx = 0;
			Vector2I vector2I;
			Vector2I vector2I2;
			Grid.GetVisibleExtents(out vector2I, out vector2I2);
			OverlayModes.Mode.RemoveOffscreenTargets<SaveLoadRoot>(this.layerTargets, vector2I, vector2I2, null);
			foreach (object obj in this.partition.GetAllIntersecting(new Vector2((float)vector2I.x, (float)vector2I.y), new Vector2((float)vector2I2.x, (float)vector2I2.y)))
			{
				SaveLoadRoot saveLoadRoot = (SaveLoadRoot)obj;
				base.AddTargetIfVisible<SaveLoadRoot>(saveLoadRoot, vector2I, vector2I2, this.layerTargets, this.targetLayer, null, null);
			}
			foreach (SaveLoadRoot saveLoadRoot2 in this.layerTargets)
			{
				if (!(saveLoadRoot2 == null))
				{
					saveLoadRoot2.GetComponent<KBatchedAnimController>().TintColour = Color.white;
					bool flag = false;
					if (saveLoadRoot2.GetComponent<KPrefabID>().HasTag(GameTags.Suit))
					{
						flag = true;
					}
					else
					{
						SuitLocker component = saveLoadRoot2.GetComponent<SuitLocker>();
						if (component != null)
						{
							flag = component.GetStoredOutfit() != null;
						}
					}
					if (flag)
					{
						this.GetFreeUI().GetComponent<RectTransform>().SetPosition(saveLoadRoot2.transform.GetPosition());
					}
				}
			}
			for (int i = this.freeUiIdx; i < this.uiList.Count; i++)
			{
				if (this.uiList[i].activeSelf)
				{
					this.uiList[i].SetActive(false);
				}
			}
		}

		public static readonly HashedString ID = "Suit";

		private UniformGrid<SaveLoadRoot> partition;

		private HashSet<SaveLoadRoot> layerTargets = new HashSet<SaveLoadRoot>();

		private ICollection<Tag> targetIDs;

		private List<GameObject> uiList = new List<GameObject>();

		private int freeUiIdx;

		private int targetLayer;

		private int cameraLayerMask;

		private int selectionMask;

		private Canvas uiParent;

		private GameObject overlayPrefab;
	}

	public class Temperature : OverlayModes.Mode
	{
		public override HashedString ViewMode()
		{
			return OverlayModes.Temperature.ID;
		}

		public override string GetSoundName()
		{
			return "Temperature";
		}

		public Temperature()
		{
			this.legendFilters = this.CreateDefaultFilters();
		}

		public override void Update()
		{
			base.Update();
			if (this.previousUserSetting != SimDebugView.Instance.user_temperatureThresholds)
			{
				this.RefreshLegendValues();
				this.previousUserSetting = SimDebugView.Instance.user_temperatureThresholds;
			}
		}

		public override void Enable()
		{
			base.Enable();
			this.previousUserSetting = SimDebugView.Instance.user_temperatureThresholds;
			this.RefreshLegendValues();
		}

		public void RefreshLegendValues()
		{
			int num = SimDebugView.Instance.temperatureThresholds.Length - 1;
			for (int i = 0; i < num; i++)
			{
				this.temperatureLegend[i].colour = GlobalAssets.Instance.colorSet.GetColorByName(SimDebugView.Instance.temperatureThresholds[num - i].colorName);
				this.temperatureLegend[i].desc_arg = GameUtil.GetFormattedTemperature(SimDebugView.Instance.temperatureThresholds[num - i].value, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false);
			}
		}

		public override Dictionary<string, ToolParameterMenu.ToggleState> CreateDefaultFilters()
		{
			return new Dictionary<string, ToolParameterMenu.ToggleState>
			{
				{
					ToolParameterMenu.FILTERLAYERS.ABSOLUTETEMPERATURE,
					ToolParameterMenu.ToggleState.On
				},
				{
					ToolParameterMenu.FILTERLAYERS.RELATIVETEMPERATURE,
					ToolParameterMenu.ToggleState.Off
				},
				{
					ToolParameterMenu.FILTERLAYERS.HEATFLOW,
					ToolParameterMenu.ToggleState.Off
				},
				{
					ToolParameterMenu.FILTERLAYERS.STATECHANGE,
					ToolParameterMenu.ToggleState.Off
				}
			};
		}

		public override void OnRenderImage(RenderTexture src, RenderTexture dest)
		{
			if (Game.IsQuitting())
			{
				return;
			}
			KAnimBatchManager.Instance().RenderKAnimTemperaturePostProcessingEffects();
		}

		public override List<LegendEntry> GetCustomLegendData()
		{
			switch (Game.Instance.temperatureOverlayMode)
			{
			case Game.TemperatureOverlayModes.AbsoluteTemperature:
				return this.temperatureLegend;
			case Game.TemperatureOverlayModes.AdaptiveTemperature:
				return this.expandedTemperatureLegend;
			case Game.TemperatureOverlayModes.HeatFlow:
				return this.heatFlowLegend;
			case Game.TemperatureOverlayModes.StateChange:
				return this.stateChangeLegend;
			case Game.TemperatureOverlayModes.RelativeTemperature:
				return new List<LegendEntry>();
			default:
				return this.temperatureLegend;
			}
		}

		public override void OnFiltersChanged()
		{
			if (base.InFilter(ToolParameterMenu.FILTERLAYERS.HEATFLOW, this.legendFilters))
			{
				Game.Instance.temperatureOverlayMode = Game.TemperatureOverlayModes.HeatFlow;
			}
			if (base.InFilter(ToolParameterMenu.FILTERLAYERS.ABSOLUTETEMPERATURE, this.legendFilters))
			{
				Game.Instance.temperatureOverlayMode = Game.TemperatureOverlayModes.AbsoluteTemperature;
			}
			if (base.InFilter(ToolParameterMenu.FILTERLAYERS.RELATIVETEMPERATURE, this.legendFilters))
			{
				Game.Instance.temperatureOverlayMode = Game.TemperatureOverlayModes.RelativeTemperature;
			}
			if (base.InFilter(ToolParameterMenu.FILTERLAYERS.ADAPTIVETEMPERATURE, this.legendFilters))
			{
				Game.Instance.temperatureOverlayMode = Game.TemperatureOverlayModes.AdaptiveTemperature;
			}
			if (base.InFilter(ToolParameterMenu.FILTERLAYERS.STATECHANGE, this.legendFilters))
			{
				Game.Instance.temperatureOverlayMode = Game.TemperatureOverlayModes.StateChange;
			}
			switch (Game.Instance.temperatureOverlayMode)
			{
			case Game.TemperatureOverlayModes.AbsoluteTemperature:
				Infrared.Instance.SetMode(Infrared.Mode.Infrared);
				CameraController.Instance.ToggleColouredOverlayView(true);
				return;
			case Game.TemperatureOverlayModes.AdaptiveTemperature:
				Infrared.Instance.SetMode(Infrared.Mode.Infrared);
				CameraController.Instance.ToggleColouredOverlayView(true);
				return;
			case Game.TemperatureOverlayModes.HeatFlow:
				Infrared.Instance.SetMode(Infrared.Mode.Disabled);
				CameraController.Instance.ToggleColouredOverlayView(false);
				return;
			case Game.TemperatureOverlayModes.StateChange:
				Infrared.Instance.SetMode(Infrared.Mode.Disabled);
				CameraController.Instance.ToggleColouredOverlayView(false);
				return;
			case Game.TemperatureOverlayModes.RelativeTemperature:
				Infrared.Instance.SetMode(Infrared.Mode.Infrared);
				CameraController.Instance.ToggleColouredOverlayView(true);
				return;
			default:
				return;
			}
		}

		public override void Disable()
		{
			Infrared.Instance.SetMode(Infrared.Mode.Disabled);
			CameraController.Instance.ToggleColouredOverlayView(false);
			base.Disable();
		}

		public static readonly HashedString ID = "Temperature";

		private Vector2 previousUserSetting;

		public List<LegendEntry> temperatureLegend = new List<LegendEntry>
		{
			new LegendEntry(UI.OVERLAYS.TEMPERATURE.MAXHOT, UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(0.8901961f, 0.13725491f, 0.12941177f), null, null, true),
			new LegendEntry(UI.OVERLAYS.TEMPERATURE.EXTREMEHOT, UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(0.9843137f, 0.3254902f, 0.3137255f), null, null, true),
			new LegendEntry(UI.OVERLAYS.TEMPERATURE.VERYHOT, UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(1f, 0.6627451f, 0.14117648f), null, null, true),
			new LegendEntry(UI.OVERLAYS.TEMPERATURE.HOT, UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(0.9372549f, 1f, 0f), null, null, true),
			new LegendEntry(UI.OVERLAYS.TEMPERATURE.TEMPERATE, UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(0.23137255f, 0.99607843f, 0.2901961f), null, null, true),
			new LegendEntry(UI.OVERLAYS.TEMPERATURE.COLD, UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(0.12156863f, 0.6313726f, 1f), null, null, true),
			new LegendEntry(UI.OVERLAYS.TEMPERATURE.VERYCOLD, UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(0.16862746f, 0.79607844f, 1f), null, null, true),
			new LegendEntry(UI.OVERLAYS.TEMPERATURE.EXTREMECOLD, UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(0.5019608f, 0.99607843f, 0.9411765f), null, null, true),
			new LegendEntry(UI.OVERLAYS.TEMPERATURE.HEATSOURCES, UI.OVERLAYS.TEMPERATURE.TOOLTIPS.HEATSOURCES, Color.white, null, Assets.GetSprite("heat_source"), true),
			new LegendEntry(UI.OVERLAYS.TEMPERATURE.HEATSINK, UI.OVERLAYS.TEMPERATURE.TOOLTIPS.HEATSINK, Color.white, null, Assets.GetSprite("heat_sink"), true)
		};

		public List<LegendEntry> heatFlowLegend = new List<LegendEntry>
		{
			new LegendEntry(UI.OVERLAYS.HEATFLOW.HEATING, UI.OVERLAYS.HEATFLOW.TOOLTIPS.HEATING, new Color(0.9098039f, 0.25882354f, 0.14901961f), null, null, true),
			new LegendEntry(UI.OVERLAYS.HEATFLOW.NEUTRAL, UI.OVERLAYS.HEATFLOW.TOOLTIPS.NEUTRAL, new Color(0.30980393f, 0.30980393f, 0.30980393f), null, null, true),
			new LegendEntry(UI.OVERLAYS.HEATFLOW.COOLING, UI.OVERLAYS.HEATFLOW.TOOLTIPS.COOLING, new Color(0.2509804f, 0.6313726f, 0.90588236f), null, null, true),
			new LegendEntry(UI.OVERLAYS.TEMPERATURE.HEATSOURCES, UI.OVERLAYS.TEMPERATURE.TOOLTIPS.HEATSOURCES, Color.white, null, Assets.GetSprite("heat_source"), true),
			new LegendEntry(UI.OVERLAYS.TEMPERATURE.HEATSINK, UI.OVERLAYS.TEMPERATURE.TOOLTIPS.HEATSINK, Color.white, null, Assets.GetSprite("heat_sink"), true)
		};

		public List<LegendEntry> expandedTemperatureLegend = new List<LegendEntry>
		{
			new LegendEntry(UI.OVERLAYS.TEMPERATURE.MAXHOT, UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(0.8901961f, 0.13725491f, 0.12941177f), null, null, true),
			new LegendEntry(UI.OVERLAYS.TEMPERATURE.EXTREMEHOT, UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(0.9843137f, 0.3254902f, 0.3137255f), null, null, true),
			new LegendEntry(UI.OVERLAYS.TEMPERATURE.VERYHOT, UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(1f, 0.6627451f, 0.14117648f), null, null, true),
			new LegendEntry(UI.OVERLAYS.TEMPERATURE.HOT, UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(0.9372549f, 1f, 0f), null, null, true),
			new LegendEntry(UI.OVERLAYS.TEMPERATURE.TEMPERATE, UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(0.23137255f, 0.99607843f, 0.2901961f), null, null, true),
			new LegendEntry(UI.OVERLAYS.TEMPERATURE.COLD, UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(0.12156863f, 0.6313726f, 1f), null, null, true),
			new LegendEntry(UI.OVERLAYS.TEMPERATURE.VERYCOLD, UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(0.16862746f, 0.79607844f, 1f), null, null, true),
			new LegendEntry(UI.OVERLAYS.TEMPERATURE.EXTREMECOLD, UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(0.5019608f, 0.99607843f, 0.9411765f), null, null, true),
			new LegendEntry(UI.OVERLAYS.TEMPERATURE.HEATSOURCES, UI.OVERLAYS.TEMPERATURE.TOOLTIPS.HEATSOURCES, Color.white, null, Assets.GetSprite("heat_source"), true),
			new LegendEntry(UI.OVERLAYS.TEMPERATURE.HEATSINK, UI.OVERLAYS.TEMPERATURE.TOOLTIPS.HEATSINK, Color.white, null, Assets.GetSprite("heat_sink"), true)
		};

		public List<LegendEntry> stateChangeLegend = new List<LegendEntry>
		{
			new LegendEntry(UI.OVERLAYS.STATECHANGE.HIGHPOINT, UI.OVERLAYS.STATECHANGE.TOOLTIPS.HIGHPOINT, new Color(0.8901961f, 0.13725491f, 0.12941177f), null, null, true),
			new LegendEntry(UI.OVERLAYS.STATECHANGE.STABLE, UI.OVERLAYS.STATECHANGE.TOOLTIPS.STABLE, new Color(0.23137255f, 0.99607843f, 0.2901961f), null, null, true),
			new LegendEntry(UI.OVERLAYS.STATECHANGE.LOWPOINT, UI.OVERLAYS.STATECHANGE.TOOLTIPS.LOWPOINT, new Color(0.5019608f, 0.99607843f, 0.9411765f), null, null, true),
			new LegendEntry(UI.OVERLAYS.TEMPERATURE.HEATSOURCES, UI.OVERLAYS.TEMPERATURE.TOOLTIPS.HEATSOURCES, Color.white, null, Assets.GetSprite("heat_source"), true),
			new LegendEntry(UI.OVERLAYS.TEMPERATURE.HEATSINK, UI.OVERLAYS.TEMPERATURE.TOOLTIPS.HEATSINK, Color.white, null, Assets.GetSprite("heat_sink"), true)
		};
	}

	public class TileMode : OverlayModes.Mode
	{
		public override HashedString ViewMode()
		{
			return OverlayModes.TileMode.ID;
		}

		public override string GetSoundName()
		{
			return "SuitRequired";
		}

		public TileMode()
		{
			OverlayModes.ColorHighlightCondition[] array = new OverlayModes.ColorHighlightCondition[1];
			array[0] = new OverlayModes.ColorHighlightCondition(delegate(KMonoBehaviour primary_element)
			{
				Color color = Color.black;
				if (primary_element != null)
				{
					color = (primary_element as PrimaryElement).Element.substance.uiColour;
				}
				return color;
			}, (KMonoBehaviour primary_element) => primary_element.gameObject.GetComponent<KBatchedAnimController>().IsVisible());
			this.highlightConditions = array;
			base..ctor();
			this.targetLayer = LayerMask.NameToLayer("MaskedOverlay");
			this.cameraLayerMask = LayerMask.GetMask(new string[] { "MaskedOverlay", "MaskedOverlayBG" });
			this.legendFilters = this.CreateDefaultFilters();
		}

		public override void Enable()
		{
			base.Enable();
			List<Tag> prefabTagsWithComponent = Assets.GetPrefabTagsWithComponent<PrimaryElement>();
			this.targetIDs.UnionWith(prefabTagsWithComponent);
			Camera.main.cullingMask |= this.cameraLayerMask;
			int defaultLayerMask = SelectTool.Instance.GetDefaultLayerMask();
			int mask = LayerMask.GetMask(new string[] { "MaskedOverlay" });
			SelectTool.Instance.SetLayerMask(defaultLayerMask | mask);
		}

		public override void Update()
		{
			Vector2I vector2I;
			Vector2I vector2I2;
			Grid.GetVisibleExtents(out vector2I, out vector2I2);
			OverlayModes.Mode.RemoveOffscreenTargets<PrimaryElement>(this.layerTargets, vector2I, vector2I2, null);
			int num = vector2I2.y - vector2I.y;
			int num2 = vector2I2.x - vector2I.x;
			Extents extents = new Extents(vector2I.x, vector2I.y, num2, num);
			List<ScenePartitionerEntry> list = new List<ScenePartitionerEntry>();
			GameScenePartitioner.Instance.GatherEntries(extents, GameScenePartitioner.Instance.pickupablesLayer, list);
			foreach (ScenePartitionerEntry scenePartitionerEntry in list)
			{
				PrimaryElement component = ((Pickupable)scenePartitionerEntry.obj).gameObject.GetComponent<PrimaryElement>();
				if (component != null)
				{
					this.TryAddObject(component, vector2I, vector2I2);
				}
			}
			list.Clear();
			GameScenePartitioner.Instance.GatherEntries(extents, GameScenePartitioner.Instance.completeBuildings, list);
			foreach (ScenePartitionerEntry scenePartitionerEntry2 in list)
			{
				BuildingComplete buildingComplete = (BuildingComplete)scenePartitionerEntry2.obj;
				PrimaryElement component2 = buildingComplete.gameObject.GetComponent<PrimaryElement>();
				if (component2 != null && buildingComplete.gameObject.layer == 0)
				{
					this.TryAddObject(component2, vector2I, vector2I2);
				}
			}
			base.UpdateHighlightTypeOverlay<PrimaryElement>(vector2I, vector2I2, this.layerTargets, this.targetIDs, this.highlightConditions, OverlayModes.BringToFrontLayerSetting.Conditional, this.targetLayer);
		}

		private void TryAddObject(PrimaryElement pe, Vector2I min, Vector2I max)
		{
			Element element = pe.Element;
			foreach (Tag tag in Game.Instance.tileOverlayFilters)
			{
				if (element.HasTag(tag))
				{
					base.AddTargetIfVisible<PrimaryElement>(pe, min, max, this.layerTargets, this.targetLayer, null, null);
					break;
				}
			}
		}

		public override void Disable()
		{
			base.Disable();
			base.DisableHighlightTypeOverlay<PrimaryElement>(this.layerTargets);
			Camera.main.cullingMask &= ~this.cameraLayerMask;
			this.layerTargets.Clear();
			SelectTool.Instance.ClearLayerMask();
		}

		public override Dictionary<string, ToolParameterMenu.ToggleState> CreateDefaultFilters()
		{
			return new Dictionary<string, ToolParameterMenu.ToggleState>
			{
				{
					ToolParameterMenu.FILTERLAYERS.ALL,
					ToolParameterMenu.ToggleState.On
				},
				{
					ToolParameterMenu.FILTERLAYERS.METAL,
					ToolParameterMenu.ToggleState.Off
				},
				{
					ToolParameterMenu.FILTERLAYERS.BUILDABLE,
					ToolParameterMenu.ToggleState.Off
				},
				{
					ToolParameterMenu.FILTERLAYERS.FILTER,
					ToolParameterMenu.ToggleState.Off
				},
				{
					ToolParameterMenu.FILTERLAYERS.CONSUMABLEORE,
					ToolParameterMenu.ToggleState.Off
				},
				{
					ToolParameterMenu.FILTERLAYERS.ORGANICS,
					ToolParameterMenu.ToggleState.Off
				},
				{
					ToolParameterMenu.FILTERLAYERS.FARMABLE,
					ToolParameterMenu.ToggleState.Off
				},
				{
					ToolParameterMenu.FILTERLAYERS.LIQUIFIABLE,
					ToolParameterMenu.ToggleState.Off
				},
				{
					ToolParameterMenu.FILTERLAYERS.GAS,
					ToolParameterMenu.ToggleState.Off
				},
				{
					ToolParameterMenu.FILTERLAYERS.LIQUID,
					ToolParameterMenu.ToggleState.Off
				},
				{
					ToolParameterMenu.FILTERLAYERS.MISC,
					ToolParameterMenu.ToggleState.Off
				}
			};
		}

		public override void OnFiltersChanged()
		{
			Game.Instance.tileOverlayFilters.Clear();
			if (base.InFilter(ToolParameterMenu.FILTERLAYERS.METAL, this.legendFilters))
			{
				Game.Instance.tileOverlayFilters.Add(GameTags.Metal);
				Game.Instance.tileOverlayFilters.Add(GameTags.RefinedMetal);
			}
			if (base.InFilter(ToolParameterMenu.FILTERLAYERS.BUILDABLE, this.legendFilters))
			{
				Game.Instance.tileOverlayFilters.Add(GameTags.BuildableRaw);
				Game.Instance.tileOverlayFilters.Add(GameTags.BuildableProcessed);
			}
			if (base.InFilter(ToolParameterMenu.FILTERLAYERS.FILTER, this.legendFilters))
			{
				Game.Instance.tileOverlayFilters.Add(GameTags.Filter);
			}
			if (base.InFilter(ToolParameterMenu.FILTERLAYERS.LIQUIFIABLE, this.legendFilters))
			{
				Game.Instance.tileOverlayFilters.Add(GameTags.Liquifiable);
			}
			if (base.InFilter(ToolParameterMenu.FILTERLAYERS.LIQUID, this.legendFilters))
			{
				Game.Instance.tileOverlayFilters.Add(GameTags.Liquid);
			}
			if (base.InFilter(ToolParameterMenu.FILTERLAYERS.CONSUMABLEORE, this.legendFilters))
			{
				Game.Instance.tileOverlayFilters.Add(GameTags.ConsumableOre);
			}
			if (base.InFilter(ToolParameterMenu.FILTERLAYERS.ORGANICS, this.legendFilters))
			{
				Game.Instance.tileOverlayFilters.Add(GameTags.Organics);
			}
			if (base.InFilter(ToolParameterMenu.FILTERLAYERS.FARMABLE, this.legendFilters))
			{
				Game.Instance.tileOverlayFilters.Add(GameTags.Farmable);
				Game.Instance.tileOverlayFilters.Add(GameTags.Agriculture);
			}
			if (base.InFilter(ToolParameterMenu.FILTERLAYERS.GAS, this.legendFilters))
			{
				Game.Instance.tileOverlayFilters.Add(GameTags.Breathable);
				Game.Instance.tileOverlayFilters.Add(GameTags.Unbreathable);
			}
			if (base.InFilter(ToolParameterMenu.FILTERLAYERS.MISC, this.legendFilters))
			{
				Game.Instance.tileOverlayFilters.Add(GameTags.Other);
			}
			base.DisableHighlightTypeOverlay<PrimaryElement>(this.layerTargets);
			this.layerTargets.Clear();
			Game.Instance.ForceOverlayUpdate(false);
		}

		public static readonly HashedString ID = "TileMode";

		private HashSet<PrimaryElement> layerTargets = new HashSet<PrimaryElement>();

		private HashSet<Tag> targetIDs = new HashSet<Tag>();

		private int targetLayer;

		private int cameraLayerMask;

		private OverlayModes.ColorHighlightCondition[] highlightConditions;
	}
}
