using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/StationaryChoreRangeVisualizer")]
public class StationaryChoreRangeVisualizer : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.Subscribe<StationaryChoreRangeVisualizer>(-1503271301, StationaryChoreRangeVisualizer.OnSelectDelegate);
		if (this.movable)
		{
			Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(base.transform, new global::System.Action(this.OnCellChange), "StationaryChoreRangeVisualizer.OnSpawn");
			base.Subscribe<StationaryChoreRangeVisualizer>(-1643076535, StationaryChoreRangeVisualizer.OnRotatedDelegate);
		}
	}

	protected override void OnCleanUp()
	{
		Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(base.transform, new global::System.Action(this.OnCellChange));
		base.Unsubscribe<StationaryChoreRangeVisualizer>(-1503271301, StationaryChoreRangeVisualizer.OnSelectDelegate, false);
		base.Unsubscribe<StationaryChoreRangeVisualizer>(-1643076535, StationaryChoreRangeVisualizer.OnRotatedDelegate, false);
		this.ClearVisualizers();
		base.OnCleanUp();
	}

	private void OnSelect(object data)
	{
		if ((bool)data)
		{
			SoundEvent.PlayOneShot(GlobalAssets.GetSound("RadialGrid_form", false), base.transform.position, 1f);
			this.UpdateVisualizers();
			return;
		}
		SoundEvent.PlayOneShot(GlobalAssets.GetSound("RadialGrid_disappear", false), base.transform.position, 1f);
		this.ClearVisualizers();
	}

	private void OnRotated(object data)
	{
		this.UpdateVisualizers();
	}

	private void OnCellChange()
	{
		this.UpdateVisualizers();
	}

	private void UpdateVisualizers()
	{
		this.newCells.Clear();
		CellOffset rotatedCellOffset = this.vision_offset;
		if (this.rotatable)
		{
			rotatedCellOffset = this.rotatable.GetRotatedCellOffset(this.vision_offset);
		}
		int num = Grid.PosToCell(base.transform.gameObject);
		int num2;
		int num3;
		Grid.CellToXY(Grid.OffsetCell(num, rotatedCellOffset), out num2, out num3);
		for (int i = 0; i < this.height; i++)
		{
			for (int j = 0; j < this.width; j++)
			{
				CellOffset rotatedCellOffset2 = new CellOffset(this.x + j, this.y + i);
				if (this.rotatable)
				{
					rotatedCellOffset2 = this.rotatable.GetRotatedCellOffset(rotatedCellOffset2);
				}
				int num4 = Grid.OffsetCell(num, rotatedCellOffset2);
				if (Grid.IsValidCell(num4))
				{
					int num5;
					int num6;
					Grid.CellToXY(num4, out num5, out num6);
					if (Grid.TestLineOfSight(num2, num3, num5, num6, this.blocking_cb, this.blocking_tile_visible))
					{
						this.newCells.Add(num4);
					}
				}
			}
		}
		for (int k = this.visualizers.Count - 1; k >= 0; k--)
		{
			if (this.newCells.Contains(this.visualizers[k].cell))
			{
				this.newCells.Remove(this.visualizers[k].cell);
			}
			else
			{
				this.DestroyEffect(this.visualizers[k].controller);
				this.visualizers.RemoveAt(k);
			}
		}
		for (int l = 0; l < this.newCells.Count; l++)
		{
			KBatchedAnimController kbatchedAnimController = this.CreateEffect(this.newCells[l]);
			this.visualizers.Add(new StationaryChoreRangeVisualizer.VisData
			{
				cell = this.newCells[l],
				controller = kbatchedAnimController
			});
		}
	}

	private void ClearVisualizers()
	{
		for (int i = 0; i < this.visualizers.Count; i++)
		{
			this.DestroyEffect(this.visualizers[i].controller);
		}
		this.visualizers.Clear();
	}

	private KBatchedAnimController CreateEffect(int cell)
	{
		KBatchedAnimController kbatchedAnimController = FXHelpers.CreateEffect(StationaryChoreRangeVisualizer.AnimName, Grid.CellToPosCCC(cell, this.sceneLayer), null, false, this.sceneLayer, true);
		kbatchedAnimController.destroyOnAnimComplete = false;
		kbatchedAnimController.visibilityType = KAnimControllerBase.VisibilityType.Always;
		kbatchedAnimController.gameObject.SetActive(true);
		kbatchedAnimController.Play(StationaryChoreRangeVisualizer.PreAnims, KAnim.PlayMode.Loop);
		return kbatchedAnimController;
	}

	private void DestroyEffect(KBatchedAnimController controller)
	{
		controller.destroyOnAnimComplete = true;
		controller.Play(StationaryChoreRangeVisualizer.PostAnim, KAnim.PlayMode.Once, 1f, 0f);
	}

	[MyCmpReq]
	private KSelectable selectable;

	[MyCmpGet]
	private Rotatable rotatable;

	public int x;

	public int y;

	public int width;

	public int height;

	public bool movable;

	public Grid.SceneLayer sceneLayer = Grid.SceneLayer.FXFront;

	public CellOffset vision_offset;

	public Func<int, bool> blocking_cb = new Func<int, bool>(Grid.PhysicalBlockingCB);

	public bool blocking_tile_visible = true;

	private static readonly string AnimName = "transferarmgrid_kanim";

	private static readonly HashedString[] PreAnims = new HashedString[] { "grid_pre", "grid_loop" };

	private static readonly HashedString PostAnim = "grid_pst";

	private List<StationaryChoreRangeVisualizer.VisData> visualizers = new List<StationaryChoreRangeVisualizer.VisData>();

	private List<int> newCells = new List<int>();

	private static readonly EventSystem.IntraObjectHandler<StationaryChoreRangeVisualizer> OnSelectDelegate = new EventSystem.IntraObjectHandler<StationaryChoreRangeVisualizer>(delegate(StationaryChoreRangeVisualizer component, object data)
	{
		component.OnSelect(data);
	});

	private static readonly EventSystem.IntraObjectHandler<StationaryChoreRangeVisualizer> OnRotatedDelegate = new EventSystem.IntraObjectHandler<StationaryChoreRangeVisualizer>(delegate(StationaryChoreRangeVisualizer component, object data)
	{
		component.OnRotated(data);
	});

	private struct VisData
	{
		public int cell;

		public KBatchedAnimController controller;
	}
}
