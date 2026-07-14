using System;
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
		AcousticDisturbance.cellsInRange.Clear();
		FloodFill.DepthTraverse<FloodFill.PredicateCondition, FloodFill.HashSetVisitTracker, FloodFill.MaxDepth, AcousticDisturbance.CellCollector>(num, new FloodFill.PredicateCondition(AcousticDisturbance.notSolid), FloodFill.HashSetVisitTracker.Default(), new FloodFill.MaxDepth(EmissionRadius), default(AcousticDisturbance.CellCollector));
		AcousticDisturbance.DrawVisualEffect(num, AcousticDisturbance.cellsInRange);
		for (int i = 0; i < liveMinionIdentities.Count; i++)
		{
			MinionIdentity minionIdentity = liveMinionIdentities[i];
			if (!(minionIdentity.gameObject == gameObject.gameObject))
			{
				Vector2 vector2 = minionIdentity.transform.GetPosition();
				if (Vector2.SqrMagnitude(vector - vector2) <= (float)num2)
				{
					int num3 = Grid.PosToCell(vector2);
					if (AcousticDisturbance.cellsInRange.Contains(num3))
					{
						StaminaMonitor.Instance smi = minionIdentity.GetSMI<StaminaMonitor.Instance>();
						if (smi != null && smi.IsSleeping())
						{
							minionIdentity.Trigger(-527751701, data);
							minionIdentity.Trigger(1621815900, data);
						}
					}
				}
			}
		}
	}

	private static void DrawVisualEffect(int center_cell, HybridListHashSet<int> cells)
	{
		SoundEvent.PlayOneShot(GlobalResources.Instance().AcousticDisturbanceSound, Grid.CellToPos(center_cell), 1f);
		for (int num = 0; num != cells.Count; num++)
		{
			int num2 = cells[num];
			int gridDistance = AcousticDisturbance.GetGridDistance(num2, center_cell);
			GameScheduler.Instance.Schedule("radialgrid_pre", AcousticDisturbance.distanceDelay * (float)gridDistance, new Action<object>(AcousticDisturbance.SpawnEffect), num2, null);
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

	private static readonly HashedString[] PreAnims = new HashedString[] { "grid_pre", "grid_loop" };

	private static readonly HashedString PostAnim = "grid_pst";

	private static float distanceDelay = 0.25f;

	private static float duration = 3f;

	private static readonly Func<int, FloodFill.BoundaryCheckResult> notSolid = delegate(int cell)
	{
		if (!Grid.Solid[cell])
		{
			return FloodFill.BoundaryCheckResult.Continue;
		}
		return FloodFill.BoundaryCheckResult.Halt;
	};

	private static readonly HybridListHashSet<int> cellsInRange = new HybridListHashSet<int>();

	private readonly struct CellCollector : FloodFill.IVisitor
	{
		public bool EarlyOut
		{
			get
			{
				return false;
			}
		}

		public void VisitCell(int cell)
		{
			AcousticDisturbance.cellsInRange.Add(cell);
		}

		public void VisitBoundary(int cell)
		{
		}
	}
}
