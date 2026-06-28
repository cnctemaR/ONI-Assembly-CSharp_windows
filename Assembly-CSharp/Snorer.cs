using System;
using System.Collections;
using System.Collections.Generic;
using Klei.AI;
using UnityEngine;

[SkipSerialization]
public class Snorer : StateMachineComponent<Snorer.StatesInstance>
{
	protected override void OnPrefabInit()
	{
		this.Subscribe(1623392196, new EventSystem.EventHandler(this.OnDeath));
		this.Subscribe(-1117766961, new EventSystem.EventHandler(this.OnRevived));
	}

	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}

	private void Emit(object data)
	{
		GameObject gameObject = (GameObject)data;
		Components.Cmps<MinionIdentity> liveMinionIdentities = Components.LiveMinionIdentities;
		Vector2 vector = gameObject.transform.position;
		int num = Grid.PosToCell(vector);
		Snorer.cellsInRange.Clear();
		int num2 = Mathf.CeilToInt(3f);
		Snorer.DetermineCellsInRadius(num, 0, num2, Snorer.cellsInRange);
		this.DrawSnoreEffect(num, Snorer.cellsInRange);
		for (int i = 0; i < liveMinionIdentities.Count; i++)
		{
			MinionIdentity minionIdentity = liveMinionIdentities[i];
			if (minionIdentity.gameObject != gameObject.gameObject)
			{
				Vector2 vector2 = minionIdentity.transform.position;
				float num3 = Vector2.SqrMagnitude(vector - vector2);
				if (num3 <= 9f)
				{
					int num4 = Grid.PosToCell(vector2);
					if (Snorer.cellsInRange.Contains(num4) && minionIdentity.GetSMI<StaminaMonitor.Instance>().IsSleeping())
					{
						minionIdentity.Trigger(1338475637, this);
					}
				}
			}
		}
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
			if (!Grid.Solid[num2] || !Grid.Solid[num4])
			{
				Snorer.DetermineCellsInRadius(num2 - 1, num, max_depth, Snorer.cellsInRange);
			}
			Snorer.DetermineCellsInRadius(num2, num, max_depth, Snorer.cellsInRange);
			if (!Grid.Solid[num2] || !Grid.Solid[num5])
			{
				Snorer.DetermineCellsInRadius(num2 + 1, num, max_depth, Snorer.cellsInRange);
			}
			Snorer.DetermineCellsInRadius(Grid.CellLeft(cell), num, max_depth, Snorer.cellsInRange);
			Snorer.DetermineCellsInRadius(Grid.CellRight(cell), num, max_depth, Snorer.cellsInRange);
			if (!Grid.Solid[num3] || !Grid.Solid[num4])
			{
				Snorer.DetermineCellsInRadius(num3 - 1, num, max_depth, Snorer.cellsInRange);
			}
			Snorer.DetermineCellsInRadius(num3, num, max_depth, Snorer.cellsInRange);
			if (!Grid.Solid[num3] || !Grid.Solid[num5])
			{
				Snorer.DetermineCellsInRadius(num3 + 1, num, max_depth, Snorer.cellsInRange);
			}
		}
	}

	private void OnDeath(object data)
	{
		base.enabled = false;
	}

	private void OnRevived(object data)
	{
		base.enabled = true;
	}

	private void DrawSnoreEffect(int center_cell, HashSet<int> cells)
	{
		foreach (int num in cells)
		{
			int gridDistance = Snorer.GetGridDistance(num, center_cell);
			GameScheduler.Instance.Schedule("radialgrid_pre", Snorer.distanceDelay * (float)gridDistance, new Action<object>(Snorer.SpawnEffect), num, null);
		}
	}

	private static void SpawnEffect(object data)
	{
		int num = (int)data;
		KBatchedAnimController kbatchedAnimController = FXHelpers.CreateEffect("radialgrid", Grid.CellToPosCCC(num, Grid.SceneLayer.Background), SceneOrganizer.Instance.GetFolder(Folder.FX).transform, false, Grid.SceneLayer.Background);
		kbatchedAnimController.destroyOnAnimComplete = false;
		kbatchedAnimController.Play(Snorer.PreAnims, KAnim.PlayMode.Loop);
		GameScheduler.Instance.Schedule("radialgrid_loop", Snorer.duration, new Action<object>(Snorer.DestroyEffect), kbatchedAnimController, null);
	}

	private static void DestroyEffect(object data)
	{
		KBatchedAnimController kbatchedAnimController = (KBatchedAnimController)data;
		kbatchedAnimController.destroyOnAnimComplete = true;
		kbatchedAnimController.Play(Snorer.PostAnim, KAnim.PlayMode.Once, 1f, 0f);
	}

	private static int GetGridDistance(int cell, int center_cell)
	{
		Vector2I vector2I = Grid.CellToXY(cell);
		Vector2I vector2I2 = Grid.CellToXY(center_cell);
		Vector2I vector2I3 = vector2I - vector2I2;
		return Math.Abs(vector2I3.x) + Math.Abs(vector2I3.y);
	}

	private IEnumerator ScaleDestroy(GameObject fx_root, float start_time, Vector2 min_scale, Vector2 max_scale, float scale_time, bool destroy)
	{
		while (!(fx_root == null))
		{
			float dt = Time.time - start_time;
			Vector2 scale = Vector2.Lerp(min_scale, max_scale, Mathf.Clamp01(dt / scale_time));
			fx_root.transform.localScale = scale;
			yield return null;
			if (dt >= scale_time)
			{
				IL_00B1:
				if (destroy)
				{
					Util.KDestroyGameObject(fx_root);
				}
				yield break;
			}
		}
		goto IL_00B1;
	}

	public void ModifyTrait(Trait t)
	{
	}

	private const float EmissionRadius = 3f;

	private const float MaxDistanceSq = 9f;

	private static readonly HashedString HeadHash = KCompBuilder.snapTo_mouth;

	private static HashSet<int> cellsInRange = new HashSet<int>();

	private static readonly string[] PreAnims = new string[] { "grid_pre", "grid_loop" };

	private static readonly string PostAnim = "grid_pst";

	private static float distanceDelay = 0.25f;

	private static float duration = 3f;

	public class StatesInstance : GameStateMachine<Snorer.States, Snorer.StatesInstance, Snorer>.GameInstance
	{
		public StatesInstance(Snorer master)
			: base(master)
		{
		}

		public bool IsSleeping()
		{
			StaminaMonitor.Instance smi = base.master.GetSMI<StaminaMonitor.Instance>();
			return smi != null && smi.IsSleeping();
		}

		public void StartSmallSnore()
		{
			KBatchedAnimController component = base.smi.master.GetComponent<KBatchedAnimController>();
			bool flag;
			Matrix4x4 symbolTransform = component.GetSymbolTransform(Snorer.HeadHash, out flag);
			if (flag)
			{
				Vector4 column = symbolTransform.GetColumn(3);
				Vector3 vector = column;
				vector.z = Grid.GetLayerZ(Grid.SceneLayer.FXFront);
				this.snoreEffect = FXHelpers.CreateEffect("snore_fx", vector, SceneOrganizer.Instance.GetFolder(Folder.FX).transform, false, Grid.SceneLayer.Front);
				this.snoreEffect.destroyOnAnimComplete = true;
				this.snoreEffect.Play("snore", KAnim.PlayMode.Loop, 1f, 0f);
			}
		}

		public void StopSmallSnore()
		{
			if (this.snoreEffect != null)
			{
				this.snoreEffect.PlayMode = KAnim.PlayMode.Once;
			}
		}

		private KBatchedAnimController snoreEffect;
	}

	public class States : GameStateMachine<Snorer.States, Snorer.StatesInstance, Snorer>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.idle;
			this.idle.Transition(this.sleeping, (Snorer.StatesInstance smi) => smi.IsSleeping());
			this.sleeping.DefaultState(this.sleeping.quiet).Enter(delegate(Snorer.StatesInstance smi)
			{
				GameScheduler.Instance.Schedule("snorelines", 2f, delegate(object data)
				{
					smi.StartSmallSnore();
				}, null, null);
			}).Exit(delegate(Snorer.StatesInstance smi)
			{
				smi.StopSmallSnore();
			})
				.Transition(this.idle, (Snorer.StatesInstance smi) => !smi.master.GetSMI<StaminaMonitor.Instance>().IsSleeping());
			this.sleeping.quiet.Enter("ScheduleNextSnore", delegate(Snorer.StatesInstance smi)
			{
				smi.ScheduleGoTo(this.GetNewInterval(), this.sleeping.snoring);
			});
			this.sleeping.snoring.Enter("Snore", delegate(Snorer.StatesInstance smi)
			{
				smi.master.Emit(smi.master.gameObject);
			}).ToggleExpression(Db.Get().Expressions.Relief, null).ScheduleGoTo(3f, this.sleeping.quiet);
		}

		private float GetNewInterval()
		{
			float num = Util.GaussianRandom(5f, 1f);
			num = Mathf.Max(num, 3f);
			return Mathf.Min(num, 10f);
		}

		public GameStateMachine<Snorer.States, Snorer.StatesInstance, Snorer>.State idle;

		public Snorer.States.SleepStates sleeping;

		public class SleepStates : GameStateMachine<Snorer.States, Snorer.StatesInstance, Snorer>.State
		{
			public GameStateMachine<Snorer.States, Snorer.StatesInstance, Snorer>.State quiet;

			public GameStateMachine<Snorer.States, Snorer.StatesInstance, Snorer>.State snoring;
		}
	}

	private struct CellInfo
	{
		public override int GetHashCode()
		{
			return this.cell;
		}

		public override bool Equals(object obj)
		{
			Snorer.CellInfo cellInfo = (Snorer.CellInfo)obj;
			return this.cell == cellInfo.cell;
		}

		public int cell;

		public int depth;
	}
}
