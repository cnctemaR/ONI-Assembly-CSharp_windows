using System;
using System.Collections.Generic;
using UnityEngine;

namespace OverlayModes
{
	public abstract class BasePlantMode : Mode
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
			this.partition = Mode.PopulatePartition<Harvestable>(this.targetIDs);
			Camera.main.cullingMask |= this.cameraLayerMask;
			SelectTool.Instance.SetLayerMask(this.selectionMask);
			DragTool.SetLayerMask(this.selectionMask);
		}

		protected override void OnSaveLoadRootRegistered(SaveLoadRoot item)
		{
			Tag saveLoadTag = item.GetComponent<KPrefabID>().GetSaveLoadTag();
			if (this.targetIDs.Contains(saveLoadTag))
			{
				Harvestable component = item.GetComponent<Harvestable>();
				if (!(component == null))
				{
					this.partition.Add(component);
				}
			}
		}

		protected override void OnSaveLoadRootUnregistered(SaveLoadRoot item)
		{
			if (!(item == null) && !(item.gameObject == null))
			{
				Harvestable component = item.GetComponent<Harvestable>();
				if (!(component == null))
				{
					if (this.layerTargets.Contains(component))
					{
						this.layerTargets.Remove(component);
					}
					this.partition.Remove(component);
				}
			}
		}

		public override void Disable()
		{
			base.UnregisterSaveLoadListeners();
			base.DisableHighlightTypeOverlay<Harvestable>(this.layerTargets);
			Camera.main.cullingMask &= ~this.cameraLayerMask;
			this.partition.Clear();
			this.layerTargets.Clear();
			SelectTool.Instance.ClearLayerMask();
			DragTool.ClearLayerMask();
		}

		protected UniformGrid<Harvestable> partition;

		protected HashSet<Harvestable> layerTargets = new HashSet<Harvestable>();

		protected ICollection<Tag> targetIDs;

		protected int targetLayer;

		private int cameraLayerMask;

		private int selectionMask;
	}
}
