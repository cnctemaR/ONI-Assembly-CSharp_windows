using System;
using System.Collections.Generic;
using UnityEngine;

public class StationaryChoreRangeVisualizer : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.Subscribe(-1503271301, new Action<object>(this.OnSelect));
		if (this.movable)
		{
			CellChangeMonitor.Instance.RegisterCellChangedHandler(base.transform, new global::System.Action(this.OnCellChange), "StationaryChoreRangeVisualizer.OnSpawn");
		}
	}

	protected override void OnCleanUp()
	{
		CellChangeMonitor.Instance.UnregisterCellChangedHandler(base.transform, new global::System.Action(this.OnCellChange));
		base.Unsubscribe(-1503271301, new Action<object>(this.OnSelect));
		this.ClearVisualizers();
		base.OnCleanUp();
	}

	private void OnSelect(object data)
	{
		bool flag = (bool)data;
		if (flag)
		{
			this.UpdateVisualizers();
		}
		else
		{
			this.ClearVisualizers();
		}
	}

	private void OnCellChange()
	{
		this.UpdateVisualizers();
	}

	private void UpdateVisualizers()
	{
		this.newCells.Clear();
		Vector3 position = base.transform.GetPosition();
		int num = Mathf.FloorToInt(position.x);
		int num2 = Mathf.FloorToInt(position.y);
		for (int i = -this.range; i <= this.range; i++)
		{
			for (int j = -this.range; j <= this.range; j++)
			{
				int num3 = Grid.XYToCell(num + j, num2 + i);
				if (Grid.IsValidCell(num3) && Grid.VisibilityTest(num, num2, num + j, num2 + i, true, true))
				{
					this.newCells.Add(num3);
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
		KBatchedAnimController kbatchedAnimController = FXHelpers.CreateEffect(StationaryChoreRangeVisualizer.AnimName, Grid.CellToPosCCC(cell, Grid.SceneLayer.Background), SceneOrganizer.Instance.GetFolder(Folder.FX).transform, false, Grid.SceneLayer.Background, true);
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

	public bool movable;

	public int range;

	private static readonly string AnimName = "transferarmgrid_kanim";

	private static readonly HashedString[] PreAnims = new HashedString[] { "grid_pre", "grid_loop" };

	private static readonly HashedString PostAnim = "grid_pst";

	private List<StationaryChoreRangeVisualizer.VisData> visualizers = new List<StationaryChoreRangeVisualizer.VisData>();

	private List<int> newCells = new List<int>();

	private struct VisData
	{
		public int cell;

		public KBatchedAnimController controller;
	}
}
