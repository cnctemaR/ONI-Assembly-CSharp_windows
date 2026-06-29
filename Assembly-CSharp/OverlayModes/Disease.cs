using System;
using System.Collections.Generic;
using Klei.AI;
using UnityEngine;

namespace OverlayModes
{
	public class Disease : Mode
	{
		public Disease(Canvas diseaseUIParent, GameObject diseaseOverlayPrefab)
		{
			this.diseaseUIParent = diseaseUIParent;
			this.diseaseOverlayPrefab = diseaseOverlayPrefab;
			this.cameraLayerMask = LayerMask.GetMask(new string[] { "MaskedOverlay", "MaskedOverlayBG" });
		}

		public override SimViewMode ViewMode()
		{
			return SimViewMode.Disease;
		}

		public override string GetSoundName()
		{
			return "Disease";
		}

		public override void Enable()
		{
			Infrared.Instance.SetMode(Infrared.Mode.Disease);
			CameraController.Instance.ToggleColouredOverlayView(true);
			Camera.main.cullingMask |= this.cameraLayerMask;
			base.RegisterSaveLoadListeners();
			foreach (DiseaseSourceVisualizer diseaseSourceVisualizer in Components.DiseaseSourceVisualizers)
			{
				if (!(diseaseSourceVisualizer == null))
				{
					diseaseSourceVisualizer.Show(this.ViewMode());
				}
			}
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
			foreach (DiseaseSourceVisualizer diseaseSourceVisualizer in Components.DiseaseSourceVisualizers)
			{
				if (!(diseaseSourceVisualizer == null))
				{
					diseaseSourceVisualizer.Show(SimViewMode.None);
				}
			}
			base.UnregisterSaveLoadListeners();
			Camera.main.cullingMask &= ~this.cameraLayerMask;
			foreach (KMonoBehaviour kmonoBehaviour in this.layerTargets)
			{
				if (!(kmonoBehaviour == null))
				{
					float defaultDepth = Mode.GetDefaultDepth(kmonoBehaviour);
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
			OverlayLegend.Instance.DisableDiseaseOverlay();
			Game.Instance.showGasConduitDisease = false;
			Game.Instance.showLiquidConduitDisease = false;
			this.freeDiseaseUI = 0;
			foreach (Disease.UpdateDiseaseInfo updateDiseaseInfo in this.updateDiseaseInfo)
			{
				updateDiseaseInfo.ui.gameObject.SetActive(false);
			}
			this.updateDiseaseInfo.Clear();
			this.privateTargets.Clear();
			this.layerTargets.Clear();
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
				gameObject = Util.KInstantiateUI(this.diseaseOverlayPrefab, this.diseaseUIParent.transform.gameObject, false);
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
			Disease.UpdateDiseaseInfo updateDiseaseInfo = new Disease.UpdateDiseaseInfo(amountInstance, component);
			KAnimControllerBase component2 = target.GetComponent<KAnimControllerBase>();
			Vector3 vector = ((!(component2 != null)) ? (target.transform.GetPosition() + Vector3.down) : component2.GetWorldPivot());
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
				foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities)
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
			foreach (Disease.UpdateDiseaseInfo updateDiseaseInfo in this.updateDiseaseInfo)
			{
				updateDiseaseInfo.ui.Refresh(updateDiseaseInfo.valueSrc);
			}
			bool flag = false;
			if (Game.Instance.showLiquidConduitDisease)
			{
				foreach (Tag tag in OverlayScreen.LiquidVentIDs)
				{
					if (!OverlayScreen.DiseaseIDs.Contains(tag))
					{
						OverlayScreen.DiseaseIDs.Add(tag);
						flag = true;
					}
				}
			}
			else
			{
				foreach (Tag tag2 in OverlayScreen.LiquidVentIDs)
				{
					if (OverlayScreen.DiseaseIDs.Contains(tag2))
					{
						OverlayScreen.DiseaseIDs.Remove(tag2);
						flag = true;
					}
				}
			}
			if (Game.Instance.showGasConduitDisease)
			{
				foreach (Tag tag3 in OverlayScreen.GasVentIDs)
				{
					if (!OverlayScreen.DiseaseIDs.Contains(tag3))
					{
						OverlayScreen.DiseaseIDs.Add(tag3);
						flag = true;
					}
				}
			}
			else
			{
				foreach (Tag tag4 in OverlayScreen.GasVentIDs)
				{
					if (OverlayScreen.DiseaseIDs.Contains(tag4))
					{
						OverlayScreen.DiseaseIDs.Remove(tag4);
						flag = true;
					}
				}
			}
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
			Mode.ClearOutsideViewObjects<KMonoBehaviour>(this.layerTargets, vector2I, vector2I2, OverlayScreen.DiseaseIDs, delegate(KMonoBehaviour go)
			{
				if (go != null)
				{
					float defaultDepth2 = Mode.GetDefaultDepth(go);
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
						if (!(kmonoBehaviour == null))
						{
							if (!this.layerTargets.Contains(kmonoBehaviour))
							{
								Vector3 position = kmonoBehaviour.transform.GetPosition();
								if (Grid.IsVisible(Grid.PosToCell(position)))
								{
									if (vector2I <= position && position <= vector2I2)
									{
										float defaultDepth = Mode.GetDefaultDepth(kmonoBehaviour);
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
			}
		}

		private int cameraLayerMask;

		private int freeDiseaseUI;

		private List<GameObject> diseaseUIList = new List<GameObject>();

		private List<Disease.UpdateDiseaseInfo> updateDiseaseInfo = new List<Disease.UpdateDiseaseInfo>();

		private HashSet<KMonoBehaviour> layerTargets = new HashSet<KMonoBehaviour>();

		private HashSet<KMonoBehaviour> privateTargets = new HashSet<KMonoBehaviour>();

		private List<KMonoBehaviour> queuedAdds = new List<KMonoBehaviour>();

		private Canvas diseaseUIParent;

		private GameObject diseaseOverlayPrefab;

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
}
