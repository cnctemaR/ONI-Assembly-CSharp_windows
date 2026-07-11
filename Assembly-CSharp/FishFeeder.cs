using System;
using UnityEngine;

public class FishFeeder : GameStateMachine<FishFeeder, FishFeeder.Instance, IStateMachineTarget, FishFeeder.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.notoperational;
		this.root.Enter(new StateMachine<FishFeeder, FishFeeder.Instance, IStateMachineTarget, FishFeeder.Def>.State.Callback(FishFeeder.SetupFishFeederTopAndBot)).Exit(new StateMachine<FishFeeder, FishFeeder.Instance, IStateMachineTarget, FishFeeder.Def>.State.Callback(FishFeeder.CleanupFishFeederTopAndBot)).EventHandler(GameHashes.OnStorageChange, new GameStateMachine<FishFeeder, FishFeeder.Instance, IStateMachineTarget, FishFeeder.Def>.GameEvent.Callback(FishFeeder.OnStorageChange));
		this.notoperational.TagTransition(GameTags.Operational, this.operational, false);
		this.operational.DefaultState(this.operational.on).TagTransition(GameTags.Operational, this.notoperational, true);
		this.operational.on.DoNothing();
		int num = 19;
		FishFeeder.ballSymbols = new HashedString[num];
		for (int i = 0; i < num; i++)
		{
			FishFeeder.ballSymbols[i] = "ball" + i.ToString();
		}
	}

	private static void SetupFishFeederTopAndBot(FishFeeder.Instance smi)
	{
		Storage storage = smi.Get<Storage>();
		smi.fishFeederTop = new FishFeeder.FishFeederTop(smi, FishFeeder.ballSymbols, storage.Capacity());
		smi.fishFeederTop.RefreshStorage();
		smi.fishFeederBot = new FishFeeder.FishFeederBot(smi, 10f, FishFeeder.ballSymbols);
		smi.fishFeederBot.RefreshStorage();
	}

	private static void CleanupFishFeederTopAndBot(FishFeeder.Instance smi)
	{
		smi.fishFeederTop.Cleanup();
		smi.fishFeederBot.Cleanup();
	}

	private static void MoveStoredContentsToConsumeOffset(FishFeeder.Instance smi)
	{
		foreach (GameObject gameObject in smi.GetComponent<Storage>().items)
		{
			if (!(gameObject == null))
			{
				FishFeeder.OnStorageChange(smi, gameObject);
			}
		}
	}

	private static void OnStorageChange(FishFeeder.Instance smi, object data)
	{
		GameObject gameObject = (GameObject)data;
		if (gameObject == null)
		{
			return;
		}
		smi.fishFeederTop.RefreshStorage();
		smi.fishFeederBot.RefreshStorage();
	}

	public GameStateMachine<FishFeeder, FishFeeder.Instance, IStateMachineTarget, FishFeeder.Def>.State notoperational;

	public FishFeeder.OperationalState operational;

	public static HashedString[] ballSymbols;

	public class Def : StateMachine.BaseDef
	{
	}

	public class OperationalState : GameStateMachine<FishFeeder, FishFeeder.Instance, IStateMachineTarget, FishFeeder.Def>.State
	{
		public GameStateMachine<FishFeeder, FishFeeder.Instance, IStateMachineTarget, FishFeeder.Def>.State on;
	}

	public new class Instance : GameStateMachine<FishFeeder, FishFeeder.Instance, IStateMachineTarget, FishFeeder.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, FishFeeder.Def def)
			: base(master, def)
		{
		}

		public FishFeeder.FishFeederTop fishFeederTop;

		public FishFeeder.FishFeederBot fishFeederBot;
	}

	public class FishFeederTop : IRenderEveryTick
	{
		public FishFeederTop(FishFeeder.Instance smi, HashedString[] ball_symbols, float capacity)
		{
			this.smi = smi;
			this.ballSymbols = ball_symbols;
			this.massPerBall = capacity / (float)ball_symbols.Length;
			this.FillFeeder(this.mass);
			SimAndRenderScheduler.instance.Add(this, false);
		}

		private void FillFeeder(float mass)
		{
			KBatchedAnimController component = this.smi.GetComponent<KBatchedAnimController>();
			SymbolOverrideController component2 = this.smi.GetComponent<SymbolOverrideController>();
			KAnim.Build.Symbol symbol = null;
			Storage component3 = this.smi.GetComponent<Storage>();
			if (component3.items.Count > 0 && component3.items[0] != null)
			{
				symbol = this.smi.GetComponent<Storage>().items[0].GetComponent<KBatchedAnimController>().AnimFiles[0].GetData().build.GetSymbol("algae");
			}
			for (int i = 0; i < this.ballSymbols.Length; i++)
			{
				bool flag = mass > (float)(i + 1) * this.massPerBall;
				component.SetSymbolVisiblity(this.ballSymbols[i], flag);
				if (symbol != null)
				{
					component2.AddSymbolOverride(this.ballSymbols[i], symbol, 0);
				}
			}
		}

		public void RefreshStorage()
		{
			float num = 0f;
			foreach (GameObject gameObject in this.smi.GetComponent<Storage>().items)
			{
				if (!(gameObject == null))
				{
					num += gameObject.GetComponent<PrimaryElement>().Mass;
				}
			}
			this.targetMass = num;
		}

		public void RenderEveryTick(float dt)
		{
			this.timeSinceLastBallAppeared += dt;
			if (this.targetMass > this.mass && this.timeSinceLastBallAppeared > 0.025f)
			{
				float num = Mathf.Min(this.massPerBall, this.targetMass - this.mass);
				this.mass += num;
				this.FillFeeder(this.mass);
				this.timeSinceLastBallAppeared = 0f;
			}
		}

		public void Cleanup()
		{
			SimAndRenderScheduler.instance.Remove(this);
		}

		private FishFeeder.Instance smi;

		private float mass;

		private float targetMass;

		private HashedString[] ballSymbols;

		private float massPerBall;

		private float timeSinceLastBallAppeared;
	}

	public class FishFeederBot
	{
		public FishFeederBot(FishFeeder.Instance smi, float mass_per_ball, HashedString[] ball_symbols)
		{
			this.smi = smi;
			this.massPerBall = mass_per_ball;
			this.anim = GameUtil.KInstantiate(Assets.GetPrefab("FishFeederBot"), smi.transform.GetPosition(), Grid.SceneLayer.Front, Folder.Buildings, null, 0).GetComponent<KBatchedAnimController>();
			this.anim.transform.SetParent(smi.transform);
			this.anim.gameObject.SetActive(true);
			this.anim.SetSceneLayer(Grid.SceneLayer.Building);
			this.anim.Play("ball", KAnim.PlayMode.Once, 1f, 0f);
			foreach (HashedString hashedString in ball_symbols)
			{
				this.anim.SetSymbolVisiblity(hashedString, false);
			}
			Storage[] components = smi.gameObject.GetComponents<Storage>();
			this.topStorage = components[0];
			this.botStorage = components[1];
		}

		public void RefreshStorage()
		{
			if (this.refreshingStorage)
			{
				return;
			}
			this.refreshingStorage = true;
			float num = 0f;
			foreach (GameObject gameObject in this.botStorage.items)
			{
				if (!(gameObject == null))
				{
					num += gameObject.GetComponent<PrimaryElement>().Mass;
					int num2 = Grid.PosToCell(this.smi.transform.GetPosition());
					int num3 = Grid.CellBelow(Grid.CellBelow(num2));
					gameObject.transform.SetPosition(Grid.CellToPosCBC(num3, Grid.SceneLayer.BuildingBack));
				}
			}
			if (num == 0f)
			{
				float num4 = 0f;
				foreach (GameObject gameObject2 in this.topStorage.items)
				{
					if (!(gameObject2 == null))
					{
						num4 += gameObject2.GetComponent<PrimaryElement>().Mass;
					}
				}
				if (num4 > 0f)
				{
					this.anim.SetSymbolVisiblity(FishFeeder.FishFeederBot.HASH_FEEDBALL, true);
					this.anim.Play("ball", KAnim.PlayMode.Once, 1f, 0f);
					Pickupable pickupable = this.topStorage.items[0].GetComponent<Pickupable>().Take(this.massPerBall);
					KAnim.Build.Symbol symbol = pickupable.GetComponent<KBatchedAnimController>().AnimFiles[0].GetData().build.GetSymbol("algae");
					if (symbol != null)
					{
						this.anim.GetComponent<SymbolOverrideController>().AddSymbolOverride(FishFeeder.FishFeederBot.HASH_FEEDBALL, symbol, 0);
					}
					this.botStorage.Store(pickupable.gameObject, false, false, true, false);
					int num5 = Grid.PosToCell(this.smi.transform.GetPosition());
					int num6 = Grid.CellBelow(Grid.CellBelow(num5));
					pickupable.transform.SetPosition(Grid.CellToPosCBC(num6, Grid.SceneLayer.BuildingUse));
				}
				else
				{
					this.anim.SetSymbolVisiblity(FishFeeder.FishFeederBot.HASH_FEEDBALL, false);
				}
			}
			this.refreshingStorage = false;
		}

		public void Cleanup()
		{
		}

		private KBatchedAnimController anim;

		private Storage topStorage;

		private Storage botStorage;

		private bool refreshingStorage;

		private FishFeeder.Instance smi;

		private float massPerBall;

		private static readonly HashedString HASH_FEEDBALL = "feedball";
	}
}
