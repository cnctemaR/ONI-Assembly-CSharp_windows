using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class MysteryEgg : StateMachineComponent<MysteryEgg.StatesInstance>, ISaveLoadableJson
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
			HandleVector<global::System.Action>.Handle handle = Game.Instance.callbackManager.Add(delegate
			{
				base.smi.StartSM();
				base.smi.master.initialized = true;
			}, "MysteryEgg");
			SimMessages.ReplaceElement(Grid.PosToCell(base.gameObject), SimHashes.Dirt, CellEventLogger.Instance.ObjectSetSimOnSpawn, global::UnityEngine.Random.Range(1000f, 3000f), -1f, handle.index);
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
		string text = string.Empty;
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
		int num5 = Grid.PosToCell(this.transform.position);
		GameObject gameObject = Scenario.SpawnPrefab(num5, 0, 1, text, Grid.SceneLayer.Use, Folder.Entities);
		PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, gameObject.GetProperName(), gameObject.transform, 1.5f, false);
		gameObject.SetActive(true);
		EggIncubator component = this.transform.parent.GetComponent<EggIncubator>();
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

	private float maturity;

	private float matureRate = 1f;

	private EggIncubator incubator;

	[Serialize]
	private bool initialized;

	private Dictionary<string, int> HatchPossibilities = new Dictionary<string, int>();

	public class StatesInstance : GameStateMachine<MysteryEgg.States, MysteryEgg.StatesInstance, MysteryEgg>.GameInstance
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
			this.grounded.idle.PlayAnim("idle", KAnim.PlayMode.Once, null).Enter(delegate(MysteryEgg.StatesInstance smi)
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
			this.incubating.wiggle_small.PlayAnim("wiggle_small", KAnim.PlayMode.Once, null).OnAnimQueueComplete(this.grounded.idle);
			this.incubating.wiggle_large.PlayAnim("wiggle_large", KAnim.PlayMode.Once, null).OnAnimQueueComplete(this.grounded.idle);
			this.incubating.wiggle_hatch.PlayAnim("hatch", KAnim.PlayMode.Once, null).OnAnimQueueComplete(this.hatch_pst);
			this.incubating.idle.PlayAnim("idle", KAnim.PlayMode.Once, null).Enter(delegate(MysteryEgg.StatesInstance smi)
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
			this.hatch_pst.PlayAnim("hatch_pst", KAnim.PlayMode.Once, null).Enter(delegate(MysteryEgg.StatesInstance smi)
			{
				smi.master.HatchCreature();
				smi.Schedule(2f, delegate(object d)
				{
					Util.KDestroyGameObject(smi.gameObject);
				}, null);
			});
			this.dead.PlayAnim("dead", KAnim.PlayMode.Once, null).Enter(delegate(MysteryEgg.StatesInstance smi)
			{
				smi.Schedule(5f, delegate(object d)
				{
					Util.KDestroyGameObject(smi.gameObject);
				}, null);
			});
			this.fall.ToggleGravity(this.grounded.idle).PlayAnim("idle", KAnim.PlayMode.Loop, null);
		}

		public MysteryEgg.States.GroundedState grounded;

		public MysteryEgg.States.IncubatingState incubating;

		public GameStateMachine<MysteryEgg.States, MysteryEgg.StatesInstance, MysteryEgg>.State hatch_pst;

		public GameStateMachine<MysteryEgg.States, MysteryEgg.StatesInstance, MysteryEgg>.State dead;

		public GameStateMachine<MysteryEgg.States, MysteryEgg.StatesInstance, MysteryEgg>.State fall;

		public class GroundedState : GameStateMachine<MysteryEgg.States, MysteryEgg.StatesInstance, MysteryEgg>.State
		{
			public GameStateMachine<MysteryEgg.States, MysteryEgg.StatesInstance, MysteryEgg>.State idle;
		}

		public class IncubatingState : GameStateMachine<MysteryEgg.States, MysteryEgg.StatesInstance, MysteryEgg>.State
		{
			public GameStateMachine<MysteryEgg.States, MysteryEgg.StatesInstance, MysteryEgg>.State idle;

			public GameStateMachine<MysteryEgg.States, MysteryEgg.StatesInstance, MysteryEgg>.State idle_alt;

			public GameStateMachine<MysteryEgg.States, MysteryEgg.StatesInstance, MysteryEgg>.State wiggle_small;

			public GameStateMachine<MysteryEgg.States, MysteryEgg.StatesInstance, MysteryEgg>.State wiggle_large;

			public GameStateMachine<MysteryEgg.States, MysteryEgg.StatesInstance, MysteryEgg>.State wiggle_hatch;
		}
	}
}
