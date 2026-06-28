using System;
using System.Collections;
using UnityEngine;

namespace OverlayModes
{
	public class Harvest : BasePlantMode
	{
		public Harvest()
		{
			ColorHighlightCondition[] array = new ColorHighlightCondition[1];
			array[0] = new ColorHighlightCondition((KMonoBehaviour harvestable) => new Color(0.65f, 0.65f, 0.65f, 0.65f), (KMonoBehaviour harvestable) => true);
			this.highlightConditions = array;
			base..ctor(OverlayScreen.HarvestableIDs);
		}

		public override SimViewMode ViewMode()
		{
			return SimViewMode.HarvestWhenReady;
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
			Mode.RemoveOffscreenTargets<Harvestable>(this.layerTargets, vector2I, vector2I2);
			IEnumerable allIntersecting = this.partition.GetAllIntersecting(new Vector2((float)vector2I.x, (float)vector2I.y), new Vector2((float)vector2I2.x, (float)vector2I2.y));
			foreach (object obj in allIntersecting)
			{
				Harvestable harvestable = (Harvestable)obj;
				base.AddTargetIfVisible<Harvestable>(harvestable, vector2I, vector2I2, this.layerTargets, this.targetLayer);
			}
			base.UpdateHighlightTypeOverlay<Harvestable>(vector2I, vector2I2, this.layerTargets, this.targetIDs, this.highlightConditions, BringToFrontLayerSetting.Constant, this.targetLayer);
			base.Update();
		}

		private ColorHighlightCondition[] highlightConditions;
	}
}
