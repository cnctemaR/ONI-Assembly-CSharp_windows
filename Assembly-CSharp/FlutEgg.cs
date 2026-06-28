using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class FlutEgg : StateMachineComponent<FlutEgg.StatesInstance>, ISaveLoadableJson
{
	protected override void OnSpawn()
	{
		this.HatchPossibilities.Add("Flut", 1);
		base.OnSpawn();
		base.GetComponent<KPrefabID>().AddTag(GameTags.Egg);
		base.smi.StartSM();
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
		GameObject gameObject = Scenario.SpawnPrefab(num5, 0, 0, text, Grid.SceneLayer.Use, Folder.Entities);
		PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, gameObject.GetProperName(), gameObject.transform, 1.5f, false);
		gameObject.SetActive(true);
		if (this.transform.parent != null)
		{
			EggIncubator component = this.transform.parent.GetComponent<EggIncubator>();
			if (component)
			{
				component.RemoveHatchedEgg(base.gameObject);
			}
		}
	}

	[MyCmpAdd]
	private KBatchedAnimController anim;

	[MyCmpAdd]
	private BoxCollider2D mCollider;

	public bool alive = true;

	private float maturity;

	private Dictionary<string, int> HatchPossibilities = new Dictionary<string, int>();

	public class StatesInstance : GameStateMachine<FlutEgg.States, FlutEgg.StatesInstance, FlutEgg>.GameInstance
	{
		public StatesInstance(FlutEgg smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<FlutEgg.States, FlutEgg.StatesInstance, FlutEgg>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.grounded.idle;
			base.serializable = true;
			this.lay.PlayAnim("lay", KAnim.PlayMode.Once, null).OnAnimQueueComplete(this.grounded.idle);
			this.grounded.ToggleMainStatusItem(Db.Get().CreatureStatusItems.Incubating).Update(delegate(FlutEgg.StatesInstance smi)
			{
				if (!Grid.Solid[Grid.PosToCell(smi.transform.position)])
				{
					smi.master.maturity += smi.deltatime;
				}
				int num = Grid.PosToCell(smi.transform.position + Vector3.down);
				if (Grid.IsValidCell(num) && !Grid.Solid[num])
				{
					smi.GoTo(this.fall);
				}
			}).EventTransition(GameHashes.TooHotFatal, this.dead, (FlutEgg.StatesInstance smi) => smi.master.alive && smi.timeinstate > 0f)
				.EventTransition(GameHashes.TooColdFatal, this.dead, (FlutEgg.StatesInstance smi) => smi.master.alive && smi.timeinstate > 0f);
			this.grounded.idle.PlayAnim("idle", KAnim.PlayMode.Once, null).Enter(delegate(FlutEgg.StatesInstance smi)
			{
				if (smi.master.maturity > 100f)
				{
					smi.ScheduleGoTo(3f, this.grounded.wiggle_hatch);
				}
				else
				{
					smi.ScheduleGoTo(3f, this.grounded.idle_alt);
				}
				int num2 = Grid.PosToCell(smi.transform.position + Vector3.down);
				if (Grid.IsValidCell(num2) && !Grid.Solid[num2])
				{
					smi.GoTo(this.fall);
				}
			});
			this.grounded.idle_alt.Enter(delegate(FlutEgg.StatesInstance smi)
			{
				smi.GoTo(this.grounded.idle);
			});
			this.grounded.wiggle_hatch.PlayAnim("hatch", KAnim.PlayMode.Once, null).Enter(delegate(FlutEgg.StatesInstance smi)
			{
				smi.Schedule(3.4f, delegate(object d)
				{
					smi.master.HatchCreature();
				}, null);
			}).EventHandler(GameHashes.AnimQueueComplete, delegate(FlutEgg.StatesInstance smi)
			{
				Util.KDestroyGameObject(smi.gameObject);
			});
			this.dead.ToggleMainStatusItem(Db.Get().CreatureStatusItems.Dead).PlayAnim("dead", KAnim.PlayMode.Once, null).Enter(delegate(FlutEgg.StatesInstance smi)
			{
				smi.Schedule(5f, delegate(object d)
				{
					Util.KDestroyGameObject(smi.gameObject);
				}, null);
			});
			this.fall.PlayAnim("idle", KAnim.PlayMode.Loop, null).ToggleGravity(this.grounded.idle);
		}

		public FlutEgg.States.GroundedState grounded;

		public GameStateMachine<FlutEgg.States, FlutEgg.StatesInstance, FlutEgg>.State lay;

		public GameStateMachine<FlutEgg.States, FlutEgg.StatesInstance, FlutEgg>.State dead;

		public GameStateMachine<FlutEgg.States, FlutEgg.StatesInstance, FlutEgg>.State fall;

		public class GroundedState : GameStateMachine<FlutEgg.States, FlutEgg.StatesInstance, FlutEgg>.State
		{
			public GameStateMachine<FlutEgg.States, FlutEgg.StatesInstance, FlutEgg>.State idle;

			public GameStateMachine<FlutEgg.States, FlutEgg.StatesInstance, FlutEgg>.State idle_alt;

			public GameStateMachine<FlutEgg.States, FlutEgg.StatesInstance, FlutEgg>.State wiggle_hatch;
		}
	}
}
