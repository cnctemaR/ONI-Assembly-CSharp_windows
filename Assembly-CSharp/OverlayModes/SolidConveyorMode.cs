using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OverlayModes
{
	public class SolidConveyorMode : Mode
	{
		public SolidConveyorMode()
		{
			this.targetLayer = LayerMask.NameToLayer("MaskedOverlay");
			this.cameraLayerMask = LayerMask.GetMask(new string[] { "MaskedOverlay", "MaskedOverlayBG" });
			this.selectionMask = this.cameraLayerMask;
		}

		public override SimViewMode ViewMode()
		{
			return SimViewMode.SolidConveyorMap;
		}

		public override string GetSoundName()
		{
			return "LiquidVent";
		}

		public override void Enable()
		{
			base.RegisterSaveLoadListeners();
			this.partition = Mode.PopulatePartition<SaveLoadRoot>(this.targetIDs);
			Camera.main.cullingMask |= this.cameraLayerMask;
			SelectTool.Instance.SetLayerMask(this.selectionMask);
			DragTool.SetLayerMask(this.selectionMask);
			GridCompositor.Instance.ToggleMinor(false);
			base.Enable();
		}

		protected override void OnSaveLoadRootRegistered(SaveLoadRoot item)
		{
			KPrefabID component = item.GetComponent<KPrefabID>();
			Tag saveLoadTag = component.GetSaveLoadTag();
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
			Mode.ResetDisplayValues<SaveLoadRoot>(this.layerTargets);
			Camera.main.cullingMask &= ~this.cameraLayerMask;
			SelectTool.Instance.ClearLayerMask();
			DragTool.ClearLayerMask();
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
			Mode.RemoveOffscreenTargets<SaveLoadRoot>(this.layerTargets, vector2I, vector2I2, null);
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
			Color32 color = Color.white;
			foreach (SaveLoadRoot saveLoadRoot2 in this.layerTargets)
			{
				if (!(saveLoadRoot2 == null))
				{
					KBatchedAnimController component = saveLoadRoot2.GetComponent<KBatchedAnimController>();
					component.TintColour = color;
				}
			}
		}

		private UniformGrid<SaveLoadRoot> partition;

		private HashSet<SaveLoadRoot> layerTargets = new HashSet<SaveLoadRoot>();

		private ICollection<Tag> targetIDs = OverlayScreen.SolidConveyorIDs;

		private int targetLayer;

		private int cameraLayerMask;

		private int selectionMask;
	}
}
