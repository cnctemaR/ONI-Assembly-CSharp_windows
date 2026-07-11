using System;
using UnityEngine;

public class SocialChoreTracker
{
	public SocialChoreTracker(GameObject owner, CellOffset[] chore_offsets)
	{
		this.owner = owner;
		this.choreOffsets = chore_offsets;
		this.chores = new Chore[this.choreOffsets.Length];
		Extents extents = new Extents(Grid.PosToCell(owner), this.choreOffsets);
		this.validNavCellChangedPartitionerEntry = GameScenePartitioner.Instance.Add("PrintingPodSocialize", owner, extents, GameScenePartitioner.Instance.validNavCellChangedLayer, new Action<object>(this.OnCellChanged));
	}

	public void Update(bool update = true)
	{
		int num = 0;
		for (int i = 0; i < this.choreOffsets.Length; i++)
		{
			CellOffset cellOffset = this.choreOffsets[i];
			Chore chore = this.chores[i];
			bool flag = update && num < this.choreCount && this.IsOffsetValid(cellOffset);
			if (flag)
			{
				num++;
				if (chore == null || chore.isComplete)
				{
					this.chores[i] = ((this.CreateChoreCB == null) ? null : this.CreateChoreCB(i));
				}
			}
			else if (chore != null)
			{
				chore.Cancel("locator invalidated");
				this.chores[i] = null;
			}
		}
	}

	private void OnCellChanged(object data)
	{
		if (this.owner.HasTag(GameTags.Operational))
		{
			this.Update(true);
		}
	}

	public void Clear()
	{
		if (this.validNavCellChangedPartitionerEntry != null)
		{
			this.validNavCellChangedPartitionerEntry.Release();
			this.validNavCellChangedPartitionerEntry = null;
		}
		this.Update(false);
	}

	private bool IsOffsetValid(CellOffset offset)
	{
		int num = Grid.PosToCell(this.owner);
		int num2 = Grid.OffsetCell(num, offset);
		int num3 = Grid.CellBelow(num2);
		return GameNavGrids.FloorValidator.IsWalkableCell(num2, num3, Grid.BitFields, false, false);
	}

	public Func<int, Chore> CreateChoreCB;

	public int choreCount;

	private GameObject owner;

	private CellOffset[] choreOffsets;

	private Chore[] chores;

	private GameScenePartitionerEntry validNavCellChangedPartitionerEntry;
}
