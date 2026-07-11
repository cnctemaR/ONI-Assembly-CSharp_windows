using System;
using System.Collections.Generic;
using UnityEngine;

public class AcousticDisturbance
{
	public static void Emit(object data, int EmissionRadius)
	{
		GameObject gameObject = (GameObject)data;
		Components.Cmps<MinionIdentity> liveMinionIdentities = Components.LiveMinionIdentities;
		Vector2 vector = gameObject.transform.GetPosition();
		int num = Grid.PosToCell(vector);
		int num2 = EmissionRadius * EmissionRadius;
		int num3 = Mathf.CeilToInt((float)EmissionRadius);
		AcousticDisturbance.DetermineCellsInRadius(num, 0, num3, AcousticDisturbance.cellsInRange);
		AcousticDisturbance.DrawVisualEffect(num, AcousticDisturbance.cellsInRange);
		for (int i = 0; i < liveMinionIdentities.Count; i++)
		{
			MinionIdentity minionIdentity = liveMinionIdentities[i];
			if (minionIdentity.gameObject != gameObject.gameObject)
			{
				Vector2 vector2 = minionIdentity.transform.GetPosition();
				if (Vector2.SqrMagnitude(vector - vector2) <= (float)num2)
				{
					int num4 = Grid.PosToCell(vector2);
					if (AcousticDisturbance.cellsInRange.Contains(num4) && minionIdentity.GetSMI<StaminaMonitor.Instance>().IsSleeping())
					{
						minionIdentity.Trigger(-527751701, data);
						minionIdentity.Trigger(1621815900, data);
					}
				}
			}
		}
		AcousticDisturbance.cellsInRange.Clear();
	}

	private static void DrawVisualEffect(int center_cell, HashSet<int> cells)
	{
		SoundEvent.PlayOneShot(GlobalResources.Instance().AcousticDisturbanceSound, Grid.CellToPos(center_cell), 1f);
		foreach (int num in cells)
		{
			int gridDistance = AcousticDisturbance.GetGridDistance(num, center_cell);
			GameScheduler.Instance.Schedule("radialgrid_pre", AcousticDisturbance.distanceDelay * (float)gridDistance, new Action<object>(AcousticDisturbance.SpawnEffect), num, null);
		}
	}

	private static void SpawnEffect(object data)
	{
		Grid.SceneLayer sceneLayer = Grid.SceneLayer.InteriorWall;
		int num = (int)data;
		KBatchedAnimController kbatchedAnimController = FXHelpers.CreateEffect("radialgrid_kanim", Grid.CellToPosCCC(num, sceneLayer), null, false, sceneLayer, false);
		kbatchedAnimController.destroyOnAnimComplete = false;
		kbatchedAnimController.Play(AcousticDisturbance.PreAnims, KAnim.PlayMode.Loop);
		GameScheduler.Instance.Schedule("radialgrid_loop", AcousticDisturbance.duration, new Action<object>(AcousticDisturbance.DestroyEffect), kbatchedAnimController, null);
	}

	private static void DestroyEffect(object data)
	{
		KBatchedAnimController kbatchedAnimController = (KBatchedAnimController)data;
		kbatchedAnimController.destroyOnAnimComplete = true;
		kbatchedAnimController.Play(AcousticDisturbance.PostAnim, KAnim.PlayMode.Once, 1f, 0f);
	}

	private static int GetGridDistance(int cell, int center_cell)
	{
		Vector2I vector2I = Grid.CellToXY(cell);
		Vector2I vector2I2 = Grid.CellToXY(center_cell);
		Vector2I vector2I3 = vector2I - vector2I2;
		return Math.Abs(vector2I3.x) + Math.Abs(vector2I3.y);
	}

	private static void DetermineCellsInRadius(int cell, int depth, int max_depth, HashSet<int> cells_in_range)
	{
		if (!Grid.IsValidCell(cell))
		{
			return;
		}
		if (Grid.Solid[cell])
		{
			return;
		}
		cells_in_range.Add(cell);
		if (depth < max_depth)
		{
			int num = depth + 1;
			int num2 = Grid.CellBelow(cell);
			int num3 = Grid.CellAbove(cell);
			int num4 = cell - 1;
			int num5 = cell + 1;
			object obj = Grid.IsValidCell(num2) && !Grid.Solid[num2];
			bool flag = Grid.IsValidCell(num3) && !Grid.Solid[num3];
			bool flag2 = Grid.IsValidCell(num4) && !Grid.Solid[num4];
			bool flag3 = Grid.IsValidCell(num5) && !Grid.Solid[num5];
			object obj2 = obj;
			if ((obj2 | flag2) != null)
			{
				AcousticDisturbance.DetermineCellsInRadius(num2 - 1, num, max_depth, cells_in_range);
			}
			AcousticDisturbance.DetermineCellsInRadius(num2, num, max_depth, cells_in_range);
			if ((obj2 | flag3) != null)
			{
				AcousticDisturbance.DetermineCellsInRadius(num2 + 1, num, max_depth, cells_in_range);
			}
			AcousticDisturbance.DetermineCellsInRadius(num4, num, max_depth, cells_in_range);
			AcousticDisturbance.DetermineCellsInRadius(num5, num, max_depth, cells_in_range);
			if (flag || flag2)
			{
				AcousticDisturbance.DetermineCellsInRadius(num3 - 1, num, max_depth, cells_in_range);
			}
			AcousticDisturbance.DetermineCellsInRadius(num3, num, max_depth, AcousticDisturbance.cellsInRange);
			if (flag || flag3)
			{
				AcousticDisturbance.DetermineCellsInRadius(num3 + 1, num, max_depth, cells_in_range);
			}
		}
	}

	private static readonly HashedString[] PreAnims = new HashedString[] { "grid_pre", "grid_loop" };

	private static readonly HashedString PostAnim = "grid_pst";

	private static float distanceDelay = 0.25f;

	private static float duration = 3f;

	private static HashSet<int> cellsInRange = new HashSet<int>();
}
