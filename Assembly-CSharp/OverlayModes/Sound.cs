using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OverlayModes
{
	public class Sound : Mode
	{
		public Sound()
		{
			ColorHighlightCondition[] array = new ColorHighlightCondition[1];
			array[0] = new ColorHighlightCondition(delegate(KMonoBehaviour np)
			{
				Color black = Color.black;
				Color color = Color.black;
				float num = 0.8f;
				if (np != null)
				{
					int num2 = Grid.PosToCell(CameraController.Instance.baseCamera.ScreenToWorldPoint(Input.mousePosition));
					float noiseForCell = (np as NoisePolluter).GetNoiseForCell(num2);
					if (noiseForCell < 36f)
					{
						num = 1f;
						color = new Color(0.4f, 0.4f, 0.4f);
					}
					else
					{
						color = SimDebugView.Instance.GetNoisePollutionCategoryColourFromDecibels(noiseForCell);
					}
				}
				return Color.Lerp(black, color, num);
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

		public override SimViewMode ViewMode()
		{
			return SimViewMode.NoisePollution;
		}

		public override string GetSoundName()
		{
			return "Sound";
		}

		public override void Enable()
		{
			base.RegisterSaveLoadListeners();
			List<Tag> prefabTagsWithComponent = Assets.GetPrefabTagsWithComponent<NoisePolluter>();
			this.targetIDs.UnionWith(prefabTagsWithComponent);
			this.partition = Mode.PopulatePartition<NoisePolluter>(this.targetIDs);
			Camera.main.cullingMask |= this.cameraLayerMask;
		}

		public override void Update()
		{
			Vector2I vector2I;
			Vector2I vector2I2;
			Grid.GetVisibleExtents(out vector2I, out vector2I2);
			Mode.RemoveOffscreenTargets<NoisePolluter>(this.layerTargets, vector2I, vector2I2, null);
			IEnumerable allIntersecting = this.partition.GetAllIntersecting(new Vector2((float)vector2I.x, (float)vector2I.y), new Vector2((float)vector2I2.x, (float)vector2I2.y));
			IEnumerator enumerator = allIntersecting.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					NoisePolluter noisePolluter = (NoisePolluter)obj;
					base.AddTargetIfVisible<NoisePolluter>(noisePolluter, vector2I, vector2I2, this.layerTargets, this.targetLayer, null, null);
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
			base.UpdateHighlightTypeOverlay<NoisePolluter>(vector2I, vector2I2, this.layerTargets, this.targetIDs, this.highlightConditions, BringToFrontLayerSetting.Conditional, this.targetLayer);
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
			if (!(item == null) && !(item.gameObject == null))
			{
				NoisePolluter component = item.GetComponent<NoisePolluter>();
				if (this.layerTargets.Contains(component))
				{
					this.layerTargets.Remove(component);
				}
				this.partition.Remove(component);
			}
		}

		public override void Disable()
		{
			base.DisableHighlightTypeOverlay<NoisePolluter>(this.layerTargets);
			Camera.main.cullingMask &= ~this.cameraLayerMask;
			base.UnregisterSaveLoadListeners();
			this.partition.Clear();
			this.layerTargets.Clear();
		}

		private UniformGrid<NoisePolluter> partition;

		private HashSet<NoisePolluter> layerTargets = new HashSet<NoisePolluter>();

		private HashSet<Tag> targetIDs = new HashSet<Tag>();

		private int targetLayer;

		private int cameraLayerMask;

		private ColorHighlightCondition[] highlightConditions;
	}
}
