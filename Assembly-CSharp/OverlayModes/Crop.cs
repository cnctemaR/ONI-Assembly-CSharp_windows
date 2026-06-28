using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OverlayModes
{
	public class Crop : BasePlantMode
	{
		public Crop(Canvas ui_root, GameObject harvestable_notification_prefab)
		{
			ColorHighlightCondition[] array = new ColorHighlightCondition[3];
			array[0] = new ColorHighlightCondition((KMonoBehaviour h) => new Color(0.95686275f, 0.2509804f, 0.2784314f, 0.75f), delegate(KMonoBehaviour h)
			{
				WiltCondition component = h.GetComponent<WiltCondition>();
				return component != null && component.IsWilting();
			});
			array[1] = new ColorHighlightCondition((KMonoBehaviour h) => new Color(0.9843137f, 0.6901961f, 0.23137255f, 0.75f), (KMonoBehaviour h) => !(h as Harvestable).CanBeHavested);
			array[2] = new ColorHighlightCondition((KMonoBehaviour h) => new Color(0.41960785f, 0.827451f, 0.5176471f, 0.75f), (KMonoBehaviour h) => (h as Harvestable).CanBeHavested);
			this.highlightConditions = array;
			base..ctor(OverlayScreen.HarvestableIDs);
			this.uiRoot = ui_root;
			this.harvestableNotificationPrefab = harvestable_notification_prefab;
		}

		public override SimViewMode ViewMode()
		{
			return SimViewMode.Crop;
		}

		public override string GetSoundName()
		{
			return "Harvest";
		}

		public override void Update()
		{
			Vector2I vector2I;
			Vector2I vector2I2;
			Grid.GetVisibleExtents(out vector2I, out vector2I2);
			Mode.RemoveOffscreenTargets<Harvestable>(this.layerTargets, vector2I, vector2I2, null);
			IEnumerable allIntersecting = this.partition.GetAllIntersecting(new Vector2((float)vector2I.x, (float)vector2I.y), new Vector2((float)vector2I2.x, (float)vector2I2.y));
			IEnumerator enumerator = allIntersecting.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					Harvestable harvestable = (Harvestable)obj;
					base.AddTargetIfVisible<Harvestable>(harvestable, vector2I, vector2I2, this.layerTargets, this.targetLayer, null, null);
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
			foreach (Harvestable harvestable2 in Components.Harvestables)
			{
				Vector2I vector2I3 = Grid.PosToXY(harvestable2.transform.position);
				if (vector2I <= vector2I3 && vector2I3 <= vector2I2 && !this.privateTargets.Contains(harvestable2))
				{
					this.AddCropUI(harvestable2);
					this.queuedAdds.Add(harvestable2);
				}
			}
			foreach (Harvestable harvestable3 in this.queuedAdds)
			{
				this.privateTargets.Add(harvestable3);
			}
			this.queuedAdds.Clear();
			foreach (Crop.UpdateCropInfo updateCropInfo in this.updateCropInfo)
			{
				updateCropInfo.harvestableUI.GetComponent<HarvestableOverlayWidget>().Refresh(updateCropInfo.harvestable);
			}
			base.UpdateHighlightTypeOverlay<Harvestable>(vector2I, vector2I2, this.layerTargets, this.targetIDs, this.highlightConditions, BringToFrontLayerSetting.Constant, this.targetLayer);
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
				gameObject.gameObject.SetActive(true);
				this.freeHarvestableNotificationIdx++;
			}
			else
			{
				gameObject = Util.KInstantiateUI(this.harvestableNotificationPrefab.gameObject, this.uiRoot.transform.gameObject, false);
				this.harvestableNotificationList.Add(gameObject);
				this.freeHarvestableNotificationIdx++;
			}
			return gameObject;
		}

		private void AddCropUI(Harvestable harvestable)
		{
			GameObject freeCropUI = this.GetFreeCropUI();
			Crop.UpdateCropInfo updateCropInfo = new Crop.UpdateCropInfo(harvestable, freeCropUI);
			Vector3 vector = Grid.CellToPos(Grid.PosToCell(harvestable), 0.5f, -1.25f, 0f);
			freeCropUI.GetComponent<RectTransform>().position = Vector3.up + vector;
			this.updateCropInfo.Add(updateCropInfo);
		}

		private List<Harvestable> queuedAdds = new List<Harvestable>();

		private HashSet<Harvestable> privateTargets = new HashSet<Harvestable>();

		private Canvas uiRoot;

		private List<Crop.UpdateCropInfo> updateCropInfo = new List<Crop.UpdateCropInfo>();

		private int freeHarvestableNotificationIdx;

		private List<GameObject> harvestableNotificationList = new List<GameObject>();

		private GameObject harvestableNotificationPrefab;

		private ColorHighlightCondition[] highlightConditions;

		private struct UpdateCropInfo
		{
			public UpdateCropInfo(Harvestable harvestable, GameObject harvestableUI)
			{
				this.harvestable = harvestable;
				this.harvestableUI = harvestableUI;
			}

			public Harvestable harvestable;

			public GameObject harvestableUI;
		}
	}
}
