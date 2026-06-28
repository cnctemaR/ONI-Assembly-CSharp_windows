using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

public class MysteryEgg : StateMachineComponent<MysteryEgg.StatesInstance>
{
	protected override void OnSpawn()
	{
		this.HatchPossibilities.Add("Hatch", 2);
		this.HatchPossibilities.Add("Glom", 1);
		this.HatchPossibilities.Add("Puft", 4);
		base.OnSpawn();
		base.GetComponent<KPrefabID>().AddTag(GameTags.Egg);
		if (!this.initialized)
		{
			HandleVector<Game.CallbackInfo>.Handle handle = Game.Instance.callbackManager.Add(new Game.CallbackInfo(delegate
			{
				base.smi.StartSM();
				base.smi.master.initialized = true;
			}, false));
			int num = Grid.PosToCell(base.gameObject);
			SimHashes simHashes = SimHashes.Dirt;
			CellElementEvent objectSetSimOnSpawn = CellEventLogger.Instance.ObjectSetSimOnSpawn;
			float num2 = global::UnityEngine.Random.Range(1000f, 3000f);
			float num3 = -1f;
			int index = handle.index;
			SimMessages.ReplaceElement(num, simHashes, objectSetSimOnSpawn, num2, num3, byte.MaxValue, 0, index);
			handle.index = -1;
		}
		else
		{
			base.smi.StartSM();
		}
	}

	private void HatchCreature()
	{
		int num = 0;
		int num2 = 0;
		foreach (KeyValuePair<string, int> keyValuePair in this.HatchPossibilities)
		{
			num += keyValuePair.Value;
			num2++;
		}
		float num3 = (float)global::UnityEngine.Random.Range(0, num);
		string text = "";
		float num4 = 0f;
		foreach (KeyValuePair<string, int> keyValuePair2 in this.HatchPossibilities)
		{
			if (num4 + (float)keyValuePair2.Value >= num3)
			{
				text = keyValuePair2.Key;
				break;
			}
			num4 += (float)keyValuePair2.Value;
		}
		int num5 = Grid.PosToCell(base.transform.position);
		GameObject gameObject = Scenario.SpawnPrefab(num5, 0, 1, text, Grid.SceneLayer.Ore, Folder.Entities);
		PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, gameObject.GetProperName(), gameObject.transform, 1.5f, false);
		gameObject.SetActive(true);
		EggIncubator component = base.transform.parent.GetComponent<EggIncubator>();
		if (component)
		{
			component.RemoveHatchedEgg(base.gameObject);
		}
	}

	[MyCmpAdd]
	private KBatchedAnimController anim;

	[MyCmpAdd]
	private CircleCollider2D mCollider;

	public bool alive = true;

	private float maturity = 0f;

	private float matureRate = 1f;

	private EggIncubator incubator;

	[Serialize]
	private bool initialized = false;

	private Dictionary<string, int> HatchPossibilities = new Dictionary<string, int>();

	public class StatesInstance : GameStateMachine<MysteryEgg.States, MysteryEgg.StatesInstance, MysteryEgg, object>.GameInstance
	{
		public StatesInstance(MysteryEgg smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<MysteryEgg.States, MysteryEgg.StatesInstance, MysteryEgg>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.grounded.idle;
			base.serializable = true;
			this.grounded.EventTransition(GameHashes.TooHotFatal, this.dead, (MysteryEgg.StatesInstance smi) => smi.master.alive && smi.timeinstate > 0f).EventTransition(GameHashes.TooColdFatal, this.dead, (MysteryEgg.StatesInstance smi) => smi.master.alive && smi.timeinstate > 0f).EventTransition(GameHashes.OnStore, this.incubating.idle, (MysteryEgg.StatesInstance smi) => smi.master.transform.parent.GetComponent<EggIncubator>() != null)
				.Update(delegate(MysteryEgg.StatesInstance smi)
				{
					int num = Grid.PosToCell(smi.transform.position + Vector3.down);
					if (Grid.IsValidCell(num) && !Grid.Solid[num])
					{
						smi.GoTo(this.fall);
					}
				});
			this.grounded.idle.PlayAnim("idle").Enter(delegate(MysteryEgg.StatesInstance smi)
			{
				int num2 = Grid.PosToCell(smi.transform.position + Vector3.down);
				if (Grid.IsValidCell(num2) && !Grid.Solid[num2])
				{
					smi.GoTo(this.fall);
				}
			});
			this.incubating.EventTransition(GameHashes.OnStorageChange, this.grounded.idle, (MysteryEgg.StatesInstance smi) => smi.master.transform.parent == null || smi.master.transform.parent.GetComponent<EggIncubator>() == null);
			this.incubating.idle_alt.Enter(delegate(MysteryEgg.StatesInstance smi)
			{
				smi.GoTo(this.grounded.idle);
			});
			this.incubating.wiggle_small.PlayAnim("wiggle_small").OnAnimQueueComplete(this.grounded.idle);
			this.incubating.wiggle_large.PlayAnim("wiggle_large").OnAnimQueueComplete(this.grounded.idle);
			this.incubating.wiggle_hatch.PlayAnim("hatch").OnAnimQueueComplete(this.hatch_pst);
			this.incubating.idle.PlayAnim("idle").Enter(delegate(MysteryEgg.StatesInstance smi)
			{
				smi.master.incubator = smi.master.transform.parent.GetComponent<EggIncubator>();
				if (smi.master.maturity > 1200f)
				{
					smi.ScheduleGoTo(3f, this.incubating.wiggle_hatch);
				}
				else if (smi.master.maturity > 1020f)
				{
					smi.ScheduleGoTo(3f, this.incubating.wiggle_large);
				}
				else if (smi.master.maturity > 150f)
				{
					smi.ScheduleGoTo(3f, this.incubating.wiggle_small);
				}
				else
				{
					smi.ScheduleGoTo(3f, this.incubating.idle_alt);
				}
			}).Update(delegate(MysteryEgg.StatesInstance smi)
			{
				if (smi.master.incubator != null && smi.master.incubator.operational.IsOperational)
				{
					smi.master.maturity += smi.deltatime * smi.master.matureRate;
				}
			});
			this.hatch_pst.PlayAnim("hatch_pst").Enter(delegate(MysteryEgg.StatesInstance smi)
			{
				smi.master.HatchCreature();
				smi.Schedule(2f, delegate(object d)
				{
					Util.KDestroyGameObject(smi.gameObject);
				}, null);
			});
			this.dead.PlayAnim("dead").Enter(delegate(MysteryEgg.StatesInstance smi)
			{
				smi.Schedule(5f, delegate(object d)
				{
					Util.KDestroyGameObject(smi.gameObject);
				}, null);
			});
			this.fall.ToggleGravity(this.grounded.idle).PlayAnim("idle", KAnim.PlayMode.Loop);
		}

		public MysteryEgg.States.GroundedState grounded;

		public MysteryEgg.States.IncubatingState incubating;

		public GameStateMachine<MysteryEgg.States, MysteryEgg.StatesInstance, MysteryEgg, object>.State hatch_pst;

		public GameStateMachine<MysteryEgg.States, MysteryEgg.StatesInstance, MysteryEgg, object>.State dead;

		public GameStateMachine<MysteryEgg.States, MysteryEgg.StatesInstance, MysteryEgg, object>.State fall;

		public class GroundedState : GameStateMachine<MysteryEgg.States, MysteryEgg.StatesInstance, MysteryEgg, object>.State
		{
			public GameStateMachine<MysteryEgg.States, MysteryEgg.StatesInstance, MysteryEgg, object>.State idle;
		}

		public class IncubatingState : GameStateMachine<MysteryEgg.States, MysteryEgg.StatesInstance, MysteryEgg, object>.State
		{
			public GameStateMachine<MysteryEgg.States, MysteryEgg.StatesInstance, MysteryEgg, object>.State idle;

			public GameStateMachine<MysteryEgg.States, MysteryEgg.StatesInstance, MysteryEgg, object>.State idle_alt;

			public GameStateMachine<MysteryEgg.States, MysteryEgg.StatesInstance, MysteryEgg, object>.State wiggle_small;

			public GameStateMachine<MysteryEgg.States, MysteryEgg.StatesInstance, MysteryEgg, object>.State wiggle_large;

			public GameStateMachine<MysteryEgg.States, MysteryEgg.StatesInstance, MysteryEgg, object>.State wiggle_hatch;
		}
	}
}
