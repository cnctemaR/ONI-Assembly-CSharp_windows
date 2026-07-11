using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.UI;

namespace OverlayModes
{
	public class Logic : Mode
	{
		public Logic(LogicModeUI ui_asset)
		{
			this.conduitTargetLayer = LayerMask.NameToLayer("MaskedOverlay");
			this.objectTargetLayer = LayerMask.NameToLayer("MaskedOverlayBG");
			this.cameraLayerMask = LayerMask.GetMask(new string[] { "MaskedOverlay", "MaskedOverlayBG" });
			this.selectionMask = this.cameraLayerMask;
			this.uiAsset = ui_asset;
		}

		public override SimViewMode ViewMode()
		{
			return SimViewMode.Logic;
		}

		public override string GetSoundName()
		{
			return "Logic";
		}

		public override void Enable()
		{
			Camera.main.cullingMask |= this.cameraLayerMask;
			SelectTool.Instance.SetLayerMask(this.selectionMask);
			DragTool.SetLayerMask(this.selectionMask);
			base.RegisterSaveLoadListeners();
			this.gameObjPartition = Mode.PopulatePartition<SaveLoadRoot>(Logic.HighlightItemIDs);
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
				float defaultDepth = Mode.GetDefaultDepth(saveLoadRoot);
				Vector3 position = saveLoadRoot.transform.GetPosition();
				position.z = defaultDepth;
				saveLoadRoot.transform.SetPosition(position);
			}
			Mode.ResetDisplayValues<SaveLoadRoot>(this.gameObjTargets);
			Mode.ResetDisplayValues<KBatchedAnimController>(this.wireControllers);
			foreach (Logic.BridgeInfo bridgeInfo in this.bridgeControllers)
			{
				if (bridgeInfo.controller != null)
				{
					Mode.ResetDisplayValues(bridgeInfo.controller);
				}
			}
			Camera.main.cullingMask &= ~this.cameraLayerMask;
			SelectTool.Instance.ClearLayerMask();
			DragTool.ClearLayerMask();
			base.UnregisterSaveLoadListeners();
			foreach (Logic.UIInfo uiinfo in this.uiInfo.GetDataList())
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
			this.bridgeControllers.Clear();
			GridCompositor.Instance.ToggleMinor(false);
		}

		protected override void OnSaveLoadRootRegistered(SaveLoadRoot item)
		{
			Tag saveLoadTag = item.GetComponent<KPrefabID>().GetSaveLoadTag();
			if (Logic.HighlightItemIDs.Contains(saveLoadTag))
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
			Tag bridge_id = TagManager.Create("LogicWireBridge");
			Mode.RemoveOffscreenTargets<SaveLoadRoot>(this.gameObjTargets, vector2I, vector2I2, delegate(SaveLoadRoot root)
			{
				if (root == null)
				{
					return;
				}
				KPrefabID component5 = root.GetComponent<KPrefabID>();
				if (component5 != null)
				{
					Tag prefabTag = component5.PrefabTag;
					if (prefabTag == wire_id)
					{
						this.wireControllers.Remove(root.GetComponent<KBatchedAnimController>());
					}
					else if (prefabTag == bridge_id)
					{
						KBatchedAnimController controller = root.GetComponent<KBatchedAnimController>();
						this.bridgeControllers.RemoveWhere((Logic.BridgeInfo x) => x.controller == controller);
					}
				}
			});
			Mode.RemoveOffscreenTargets<ILogicUIElement>(this.ioTargets, this.workingIOTargets, vector2I, vector2I2, new Action<ILogicUIElement>(this.FreeUI), null);
			using (new KProfiler.Region("UpdateLogicOverlay", null))
			{
				IEnumerable allIntersecting = this.gameObjPartition.GetAllIntersecting(new Vector2((float)vector2I.x, (float)vector2I.y), new Vector2((float)vector2I2.x, (float)vector2I2.y));
				IEnumerator enumerator = allIntersecting.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						SaveLoadRoot saveLoadRoot = (SaveLoadRoot)obj;
						if (saveLoadRoot != null)
						{
							KPrefabID component = saveLoadRoot.GetComponent<KPrefabID>();
							if (component.PrefabTag == wire_id || component.PrefabTag == bridge_id)
							{
								base.AddTargetIfVisible<SaveLoadRoot>(saveLoadRoot, vector2I, vector2I2, this.gameObjTargets, this.conduitTargetLayer, delegate(SaveLoadRoot root)
								{
									if (root == null)
									{
										return;
									}
									KPrefabID component6 = root.GetComponent<KPrefabID>();
									if (Logic.HighlightItemIDs.Contains(component6.PrefabTag))
									{
										if (component6.PrefabTag == wire_id)
										{
											this.wireControllers.Add(root.GetComponent<KBatchedAnimController>());
										}
										else if (component6.PrefabTag == bridge_id)
										{
											KBatchedAnimController component7 = root.GetComponent<KBatchedAnimController>();
											LogicUtilityNetworkLink component8 = root.GetComponent<LogicUtilityNetworkLink>();
											int num2;
											int num3;
											component8.GetCells(out num2, out num3);
											this.bridgeControllers.Add(new Logic.BridgeInfo
											{
												cell = num2,
												controller = component7
											});
										}
									}
								}, null);
							}
							else
							{
								base.AddTargetIfVisible<SaveLoadRoot>(saveLoadRoot, vector2I, vector2I2, this.gameObjTargets, this.objectTargetLayer, delegate(SaveLoadRoot root)
								{
									Vector3 position = root.transform.GetPosition();
									position.z += 2f;
									root.transform.SetPosition(position);
									KBatchedAnimController component9 = root.GetComponent<KBatchedAnimController>();
									component9.enabled = false;
									component9.enabled = true;
								}, null);
							}
						}
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
				IEnumerable allIntersecting2 = this.ioPartition.GetAllIntersecting(new Vector2((float)vector2I.x, (float)vector2I.y), new Vector2((float)vector2I2.x, (float)vector2I2.y));
				IEnumerator enumerator2 = allIntersecting2.GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						object obj2 = enumerator2.Current;
						ILogicUIElement logicUIElement = (ILogicUIElement)obj2;
						if (logicUIElement != null)
						{
							base.AddTargetIfVisible<ILogicUIElement>(logicUIElement, vector2I, vector2I2, this.ioTargets, this.objectTargetLayer, new Action<ILogicUIElement>(this.AddUI), (KMonoBehaviour kcmp) => kcmp != null && Logic.HighlightItemIDs.Contains(kcmp.GetComponent<KPrefabID>().PrefabTag));
						}
					}
				}
				finally
				{
					IDisposable disposable2;
					if ((disposable2 = enumerator2 as IDisposable) != null)
					{
						disposable2.Dispose();
					}
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
					IBridgedNetworkItem component2 = gameObject.GetComponent<IBridgedNetworkItem>();
					if (component2 != null)
					{
						int networkCell = component2.GetNetworkCell();
						this.visited.Clear();
						this.FindConnectedNetworks(networkCell, Game.Instance.logicCircuitSystem, this.connectedNetworks, this.visited);
						this.visited.Clear();
						num = ModeUtil.GetHighlightScale();
					}
				}
				LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
				Color32 colourOn = this.uiAsset.colourOn;
				Color32 colourOff = this.uiAsset.colourOff;
				colourOff.a = (colourOn.a = 0);
				foreach (KBatchedAnimController kbatchedAnimController in this.wireControllers)
				{
					if (!(kbatchedAnimController == null))
					{
						Color32 color = colourOff;
						LogicCircuitNetwork networkForCell = logicCircuitManager.GetNetworkForCell(Grid.PosToCell(kbatchedAnimController.transform.GetPosition()));
						if (networkForCell != null)
						{
							color = ((networkForCell.OutputValue <= 0) ? colourOff : colourOn);
						}
						if (this.connectedNetworks.Count > 0)
						{
							IBridgedNetworkItem component3 = kbatchedAnimController.GetComponent<IBridgedNetworkItem>();
							if (component3 != null && component3.IsConnectedToNetworks(this.connectedNetworks))
							{
								color.r = (byte)((float)color.r * num);
								color.g = (byte)((float)color.g * num);
								color.b = (byte)((float)color.b * num);
							}
						}
						kbatchedAnimController.TintColour = color;
					}
				}
				foreach (Logic.BridgeInfo bridgeInfo in this.bridgeControllers)
				{
					if (!(bridgeInfo.controller == null))
					{
						Color32 color2 = colourOff;
						LogicCircuitNetwork networkForCell2 = logicCircuitManager.GetNetworkForCell(bridgeInfo.cell);
						if (networkForCell2 != null)
						{
							color2 = ((networkForCell2.OutputValue <= 0) ? colourOff : colourOn);
						}
						if (this.connectedNetworks.Count > 0)
						{
							IBridgedNetworkItem component4 = bridgeInfo.controller.GetComponent<IBridgedNetworkItem>();
							if (component4 != null && component4.IsConnectedToNetworks(this.connectedNetworks))
							{
								color2.r = (byte)((float)color2.r * num);
								color2.g = (byte)((float)color2.g * num);
								color2.b = (byte)((float)color2.b * num);
							}
						}
						bridgeInfo.controller.TintColour = color2;
					}
				}
			}
			this.UpdateUI();
		}

		private void UpdateUI()
		{
			Color32 colourOn = this.uiAsset.colourOn;
			Color32 colourOff = this.uiAsset.colourOff;
			Color32 colourDisconnected = this.uiAsset.colourDisconnected;
			colourOff.a = (colourOn.a = byte.MaxValue);
			foreach (Logic.UIInfo uiinfo in this.uiInfo.GetDataList())
			{
				LogicCircuitNetwork networkForCell = Game.Instance.logicCircuitManager.GetNetworkForCell(uiinfo.cell);
				Color32 color = colourDisconnected;
				if (networkForCell != null)
				{
					bool flag = networkForCell.OutputValue > 0;
					color = ((!flag) ? colourOff : colourOn);
				}
				if (uiinfo.image.color != color)
				{
					uiinfo.image.color = color;
				}
			}
		}

		private void AddUI(ILogicUIElement ui_elem)
		{
			if (this.uiNodes.ContainsKey(ui_elem))
			{
				return;
			}
			HandleVector<int>.Handle handle = this.uiInfo.Allocate(new Logic.UIInfo(ui_elem, this.uiAsset));
			this.uiNodes.Add(ui_elem, new Logic.EventInfo
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
			Logic.EventInfo eventInfo;
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
			LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
			ReadOnlyCollection<ILogicUIElement> visElements = logicCircuitManager.GetVisElements();
			foreach (ILogicUIElement logicUIElement in visElements)
			{
				if (logicUIElement != null)
				{
					uniformGrid.Add(logicUIElement);
				}
			}
			return uniformGrid;
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

		public static HashSet<Tag> HighlightItemIDs = new HashSet<Tag>();

		private int conduitTargetLayer;

		private int objectTargetLayer;

		private int cameraLayerMask;

		private int selectionMask;

		private UniformGrid<ILogicUIElement> ioPartition;

		private HashSet<ILogicUIElement> ioTargets = new HashSet<ILogicUIElement>();

		private HashSet<ILogicUIElement> workingIOTargets = new HashSet<ILogicUIElement>();

		private HashSet<KBatchedAnimController> wireControllers = new HashSet<KBatchedAnimController>();

		private HashSet<UtilityNetwork> connectedNetworks = new HashSet<UtilityNetwork>();

		private List<int> visited = new List<int>();

		private HashSet<Logic.BridgeInfo> bridgeControllers = new HashSet<Logic.BridgeInfo>();

		private UniformGrid<SaveLoadRoot> gameObjPartition;

		private HashSet<SaveLoadRoot> gameObjTargets = new HashSet<SaveLoadRoot>();

		private LogicModeUI uiAsset;

		private Dictionary<ILogicUIElement, Logic.EventInfo> uiNodes = new Dictionary<ILogicUIElement, Logic.EventInfo>();

		private KCompactedVector<Logic.UIInfo> uiInfo = new KCompactedVector<Logic.UIInfo>(0);

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
				this.instance = global::Util.KInstantiate(ui_data.prefab, Grid.CellToPosCCC(this.cell, Grid.SceneLayer.Front), Quaternion.identity, GameScreenManager.Instance.worldSpaceCanvas, null, true, 0);
				this.instance.SetActive(true);
				this.image = this.instance.GetComponent<Image>();
				this.image.raycastTarget = false;
				LogicPortSpriteType logicPortSpriteType = ui_elem.GetLogicPortSpriteType();
				if (logicPortSpriteType != LogicPortSpriteType.Input)
				{
					if (logicPortSpriteType != LogicPortSpriteType.Output)
					{
						if (logicPortSpriteType == LogicPortSpriteType.ResetUpdate)
						{
							this.image.sprite = ui_data.resetSprite;
						}
					}
					else
					{
						this.image.sprite = ui_data.outputSprite;
					}
				}
				else
				{
					this.image.sprite = ui_data.inputSprite;
				}
			}

			public void Release()
			{
				global::Util.KDestroyGameObject(this.instance);
			}

			public GameObject instance;

			public Image image;

			public int cell;
		}
	}
}
