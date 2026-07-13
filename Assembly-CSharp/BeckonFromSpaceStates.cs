using System;
using UnityEngine;

internal class BeckonFromSpaceStates : GameStateMachine<BeckonFromSpaceStates, BeckonFromSpaceStates.Instance, IStateMachineTarget, BeckonFromSpaceStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.beckoning;
		this.beckoning.ToggleMainStatusItem(Db.Get().CreatureStatusItems.Beckoning, null).Enter(new StateMachine<BeckonFromSpaceStates, BeckonFromSpaceStates.Instance, IStateMachineTarget, BeckonFromSpaceStates.Def>.State.Callback(BeckonFromSpaceStates.ChooseSong)).DefaultState(this.beckoning.pre);
		this.beckoning.pre.PlayAnim(new Func<BeckonFromSpaceStates.Instance, string>(BeckonFromSpaceStates.GetSongAnimPre), KAnim.PlayMode.Once).OnAnimQueueComplete(this.beckoning.loop);
		this.beckoning.loop.PlayAnim(new Func<BeckonFromSpaceStates.Instance, string>(BeckonFromSpaceStates.GetSongAnimLoop), KAnim.PlayMode.Once).Enter(new StateMachine<BeckonFromSpaceStates, BeckonFromSpaceStates.Instance, IStateMachineTarget, BeckonFromSpaceStates.Def>.State.Callback(BeckonFromSpaceStates.MooEchoFX)).OnAnimQueueComplete(this.beckoning.pst);
		this.beckoning.pst.PlayAnim(new Func<BeckonFromSpaceStates.Instance, string>(BeckonFromSpaceStates.GetSongAnimPst), KAnim.PlayMode.Once).OnAnimQueueComplete(this.behaviourcomplete);
		this.behaviourcomplete.PlayAnim("idle_loop", KAnim.PlayMode.Loop).Enter(new StateMachine<BeckonFromSpaceStates, BeckonFromSpaceStates.Instance, IStateMachineTarget, BeckonFromSpaceStates.Def>.State.Callback(BeckonFromSpaceStates.DoBeckon)).Enter(new StateMachine<BeckonFromSpaceStates, BeckonFromSpaceStates.Instance, IStateMachineTarget, BeckonFromSpaceStates.Def>.State.Callback(BeckonFromSpaceStates.MooCheer))
			.BehaviourComplete(GameTags.Creatures.WantsToBeckon, false);
	}

	public static string GetSongAnimPre(BeckonFromSpaceStates.Instance smi)
	{
		return smi.ChosenSong.singAnimPre;
	}

	public static string GetSongAnimLoop(BeckonFromSpaceStates.Instance smi)
	{
		return smi.ChosenSong.singAnimLoop;
	}

	public static string GetSongAnimPst(BeckonFromSpaceStates.Instance smi)
	{
		return smi.ChosenSong.singAnimPst;
	}

	private static void ChooseSong(BeckonFromSpaceStates.Instance smi)
	{
		smi.ChooseSong();
	}

	private static void MooEchoFX(BeckonFromSpaceStates.Instance smi)
	{
		KBatchedAnimController kbatchedAnimController = FXHelpers.CreateEffect("moo_call_fx_kanim", smi.master.transform.position, null, false, Grid.SceneLayer.Front, false);
		kbatchedAnimController.destroyOnAnimComplete = true;
		kbatchedAnimController.Play("moo_call", KAnim.PlayMode.Once, 1f, 0f);
	}

	private static Util.IterationInstruction mooCheerVisitor(object obj, BeckonFromSpaceStates.Instance smi)
	{
		KPrefabID kprefabID = (obj as Pickupable).KPrefabID;
		if (kprefabID.gameObject == smi.gameObject)
		{
			return Util.IterationInstruction.Continue;
		}
		if (kprefabID.HasTag("Moo") && kprefabID.GetSMI<AnimInterruptMonitor.Instance>() != null)
		{
			kprefabID.GetSMI<AnimInterruptMonitor.Instance>().PlayAnimSequence(smi.def.choirAnims);
		}
		return Util.IterationInstruction.Continue;
	}

	private static void MooCheer(BeckonFromSpaceStates.Instance smi)
	{
		Vector3 position = smi.transform.GetPosition();
		Extents extents = new Extents((int)position.x, (int)position.y, 15);
		GameScenePartitioner.Instance.VisitEntries<BeckonFromSpaceStates.Instance>(extents.x, extents.y, extents.width, extents.height, GameScenePartitioner.Instance.pickupablesLayer, new Func<object, BeckonFromSpaceStates.Instance, Util.IterationInstruction>(BeckonFromSpaceStates.mooCheerVisitor), smi);
	}

	private static void DoBeckon(BeckonFromSpaceStates.Instance smi)
	{
		Db.Get().Amounts.Beckoning.Lookup(smi.gameObject).value = 0f;
		WorldContainer myWorld = smi.GetMyWorld();
		Vector3 position = smi.transform.position;
		float num = (float)(myWorld.Height + myWorld.WorldOffset.y - 1);
		float layerZ = Grid.GetLayerZ(smi.def.sceneLayer);
		float num2 = (num - position.y) * Mathf.Tan(0.2617994f);
		float num3 = position.x + (float)global::UnityEngine.Random.Range(-5, 5);
		float num4 = num3 - num2;
		float num5 = num3 + num2;
		float num6 = position.x;
		bool flag = false;
		if (num4 > (float)myWorld.WorldOffset.x && num4 < (float)(myWorld.WorldOffset.x + myWorld.Width))
		{
			num6 = num4;
			flag = false;
		}
		else if (num4 > (float)myWorld.WorldOffset.x && num4 < (float)(myWorld.WorldOffset.x + myWorld.Width))
		{
			num6 = num5;
			flag = true;
		}
		DebugUtil.DevAssert(myWorld.ContainsPoint(new Vector2(num6, num)), "Gassy Moo spawned outside world bounds", null);
		Vector3 vector = new Vector3(num6, num, layerZ);
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(smi.ChosenSong.meteorID), vector, Quaternion.identity, null, null, true, 0);
		GassyMooComet component = gameObject.GetComponent<GassyMooComet>();
		if (component != null)
		{
			component.spawnWithOffset = true;
			if (num6 != position.x)
			{
				component.SetCustomInitialFlip(flag);
			}
		}
		gameObject.SetActive(true);
	}

	public BeckonFromSpaceStates.BeckoningState beckoning;

	public GameStateMachine<BeckonFromSpaceStates, BeckonFromSpaceStates.Instance, IStateMachineTarget, BeckonFromSpaceStates.Def>.State behaviourcomplete;

	public class Def : StateMachine.BaseDef
	{
		public Grid.SceneLayer sceneLayer;

		public HashedString[] choirAnims = new HashedString[] { "reply_loop" };
	}

	public new class Instance : GameStateMachine<BeckonFromSpaceStates, BeckonFromSpaceStates.Instance, IStateMachineTarget, BeckonFromSpaceStates.Def>.GameInstance
	{
		public Instance(Chore<BeckonFromSpaceStates.Instance> chore, BeckonFromSpaceStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.WantsToBeckon);
			this.monitor = base.gameObject.GetSMI<BeckoningMonitor.Instance>();
		}

		public void ChooseSong()
		{
			float num = global::UnityEngine.Random.value;
			BeckoningMonitor.SongChance songChance = null;
			foreach (BeckoningMonitor.SongChance songChance2 in this.monitor.songChances)
			{
				num -= songChance2.weight;
				if (num <= 0f)
				{
					songChance = songChance2;
					break;
				}
			}
			this.ChosenSong = songChance;
		}

		public BeckoningMonitor.SongChance ChosenSong;

		private BeckoningMonitor.Instance monitor;
	}

	public class BeckoningState : GameStateMachine<BeckonFromSpaceStates, BeckonFromSpaceStates.Instance, IStateMachineTarget, BeckonFromSpaceStates.Def>.State
	{
		public GameStateMachine<BeckonFromSpaceStates, BeckonFromSpaceStates.Instance, IStateMachineTarget, BeckonFromSpaceStates.Def>.State pre;

		public GameStateMachine<BeckonFromSpaceStates, BeckonFromSpaceStates.Instance, IStateMachineTarget, BeckonFromSpaceStates.Def>.State loop;

		public GameStateMachine<BeckonFromSpaceStates, BeckonFromSpaceStates.Instance, IStateMachineTarget, BeckonFromSpaceStates.Def>.State pst;
	}
}
