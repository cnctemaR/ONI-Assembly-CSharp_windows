using System;
using Klei.AI;
using KSerialization;

public class HygieneMonitor : GameStateMachine<HygieneMonitor, HygieneMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.clean;
		base.serializable = true;
		this.root.EventHandler(GameHashes.NewDay, (HygieneMonitor.Instance smi) => GameClock.Instance, delegate(HygieneMonitor.Instance smi)
		{
			smi.AddUncleanEffect();
		}).Update(delegate(HygieneMonitor.Instance smi, float dt)
		{
			smi.UpdateDirtiness();
		});
		this.clean.EventTransition(GameHashes.EffectAdded, this.needsshower_pre, (HygieneMonitor.Instance smi) => smi.NeedsShower());
		this.needsshower_pre.Enter(delegate(HygieneMonitor.Instance smi)
		{
			smi.SetDirtiness(1f);
			smi.GoTo(this.needsshower);
		});
		this.needsshower.EventTransition(GameHashes.EffectRemoved, this.clean, (HygieneMonitor.Instance smi) => !smi.NeedsShower()).ToggleUrge(Db.Get().Urges.Shower).Exit(delegate(HygieneMonitor.Instance smi)
		{
			smi.SetDirtiness(0f);
		});
	}

	public GameStateMachine<HygieneMonitor, HygieneMonitor.Instance, IStateMachineTarget, object>.State clean;

	public GameStateMachine<HygieneMonitor, HygieneMonitor.Instance, IStateMachineTarget, object>.State needsshower_pre;

	public GameStateMachine<HygieneMonitor, HygieneMonitor.Instance, IStateMachineTarget, object>.State needsshower;

	public new class Instance : GameStateMachine<HygieneMonitor, HygieneMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			this.effects = master.GetComponent<Effects>();
		}

		public void AddUncleanEffect()
		{
			this.effects.Add("Unclean", true);
		}

		public float GetDirtiness()
		{
			return this.dirtiness;
		}

		public void SetDirtiness(float dirtiness)
		{
			this.dirtiness = dirtiness;
		}

		public bool NeedsShower()
		{
			bool flag = false;
			for (int i = 0; i < HygieneMonitor.Instance.NeedsShowerEffectsIDs.Length; i++)
			{
				string text = HygieneMonitor.Instance.NeedsShowerEffectsIDs[i];
				if (this.effects.HasEffect(text))
				{
					flag = true;
					break;
				}
			}
			return flag;
		}

		public void UpdateDirtiness()
		{
			int num = Grid.PosToCell(base.master.transform.GetPosition());
			int num2 = Grid.CellAbove(num);
			Element element = Grid.Element[num];
			Element element2 = Grid.Element[num2];
			if ((element.IsLiquid && element.id != SimHashes.Water) || (element2.IsLiquid && element2.id != SimHashes.Water))
			{
				base.master.GetComponent<Effects>().Add("Unclean", true);
			}
		}

		private Effects effects;

		[Serialize]
		private float dirtiness;

		private static readonly string[] NeedsShowerEffectsIDs = new string[] { "Unclean" };
	}
}
