using System;
using System.Collections.Generic;
using UnityEngine;

namespace OverlayModes
{
	public class Decor : Mode
	{
		public Decor()
		{
			ColorHighlightCondition[] array = new ColorHighlightCondition[1];
			array[0] = new ColorHighlightCondition(delegate(KMonoBehaviour dp)
			{
				Color black = Color.black;
				Color black2 = Color.black;
				if (dp != null)
				{
					int num = Grid.PosToCell(CameraController.Instance.baseCamera.ScreenToWorldPoint(Input.mousePosition));
					float decorForCell = (dp as DecorProvider).GetDecorForCell(num);
					if (decorForCell > 0f)
					{
						black2 = new Color(0f, 0.8f, 0f, 0.8f);
					}
					else if (decorForCell < 0f)
					{
						black2 = new Color(1f, 0f, 0f, 0.4f);
					}
				}
				return Color.Lerp(black, black2, 0.85f);
			}, (KMonoBehaviour dp) => SelectToolHoverTextCard.highlightedObjects.Contains(dp.gameObject));
			this.highlightConditions = array;
			base..ctor();
			this.targetLayer = LayerMask.NameToLayer("MaskedOverlay");
			this.cameraLayerMask = LayerMask.GetMask(new string[] { "MaskedOverlay", "MaskedOverlayBG" });
		}

		public override SimViewMode ViewMode()
		{
			return SimViewMode.Decor;
		}

		public override string GetSoundName()
		{
			return "Decor";
		}

		public override void Enable()
		{
			base.RegisterSaveLoadListeners();
			List<Tag> prefabTagsWithComponent = Assets.GetPrefabTagsWithComponent<DecorProvider>();
			this.targetIDs.UnionWith(prefabTagsWithComponent);
			Tag[] array = new Tag[]
			{
				new Tag("Tile"),
				new Tag("MeshTile"),
				new Tag("InsulationTile"),
				new Tag("GasPermeableMembrane")
			};
			foreach (Tag tag in array)
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
			this.partition = Mode.PopulatePartition<DecorProvider>(this.targetIDs);
			Camera.main.cullingMask |= this.cameraLayerMask;
		}

		public override void Update()
		{
			Vector2I vector2I;
			Vector2I vector2I2;
			Grid.GetVisibleExtents(out vector2I, out vector2I2);
			Mode.RemoveOffscreenTargets<DecorProvider>(this.layerTargets, vector2I, vector2I2);
			this.partition.GetAllIntersecting(new Vector2((float)vector2I.x, (float)vector2I.y), new Vector2((float)vector2I2.x, (float)vector2I2.y), this.workingTargets);
			for (int i = 0; i < this.workingTargets.Count; i++)
			{
				DecorProvider decorProvider = this.workingTargets[i];
				base.AddTargetIfVisible<DecorProvider>(decorProvider, vector2I, vector2I2, this.layerTargets, this.targetLayer);
			}
			base.UpdateHighlightTypeOverlay<DecorProvider>(vector2I, vector2I2, this.layerTargets, this.targetIDs, this.highlightConditions, BringToFrontLayerSetting.Conditional, this.targetLayer);
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

		private UniformGrid<DecorProvider> partition;

		private HashSet<DecorProvider> layerTargets = new HashSet<DecorProvider>();

		private List<DecorProvider> workingTargets = new List<DecorProvider>();

		private HashSet<Tag> targetIDs = new HashSet<Tag>();

		private int targetLayer;

		private int cameraLayerMask;

		private ColorHighlightCondition[] highlightConditions;
	}
}
