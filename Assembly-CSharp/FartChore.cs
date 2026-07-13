using System;
using UnityEngine;

public class FartChore : Chore<FartChore.StatesInstance>
{
	public FartChore(IStateMachineTarget target, ChoreType chore_type, float mass, SimHashes element_id, byte disease_idx, int disease_count, float overpressureThreshold)
		: base(chore_type, target, target.GetComponent<ChoreProvider>(), false, null, null, null, PriorityScreen.PriorityClass.compulsory, 5, false, true, 0, false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new FartChore.StatesInstance(this, target.gameObject);
		this.mass = mass;
		this.element_id = element_id;
		this.disease_idx = disease_idx;
		this.disease_count = disease_count;
		this.overpressureThreshold = overpressureThreshold;
	}

	private bool CheckIsOverpressure(int cell)
	{
		return Grid.Mass[cell] > this.overpressureThreshold;
	}

	public static void CreateEmission(FartChore.StatesInstance smi)
	{
		smi.master.DoFart();
	}

	public void DoFart()
	{
		if (this.mass <= 0f)
		{
			return;
		}
		Element element = ElementLoader.FindElementByHash(this.element_id);
		float temperature = base.smi.master.GetComponent<PrimaryElement>().Temperature;
		if (element.IsGas || element.IsLiquid)
		{
			int num = Grid.PosToCell(base.transform.GetPosition());
			if (this.CheckIsOverpressure(num))
			{
				return;
			}
			SimMessages.AddRemoveSubstance(num, this.element_id, CellEventLogger.Instance.ElementConsumerSimUpdate, this.mass, temperature, this.disease_idx, this.disease_count, true, -1);
		}
		else if (element.IsSolid)
		{
			element.substance.SpawnResource(base.transform.GetPosition() + new Vector3(0f, 0.5f, 0f), this.mass, temperature, this.disease_idx, this.disease_count, false, true, false);
		}
		PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, element.name, this.gameObject.transform, 1.5f, false);
	}

	private float mass;

	private SimHashes element_id;

	private byte disease_idx;

	private int disease_count;

	private float overpressureThreshold;

	public class StatesInstance : GameStateMachine<FartChore.States, FartChore.StatesInstance, FartChore, object>.GameInstance
	{
		public StatesInstance(FartChore master, GameObject farter)
			: base(master)
		{
			base.sm.farter.Set(farter, base.smi, false);
		}
	}

	public class States : GameStateMachine<FartChore.States, FartChore.StatesInstance, FartChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.root;
			base.Target(this.farter);
			this.root.PlayAnim("fart").ScheduleGoTo(10f, this.finish).OnAnimQueueComplete(this.finish);
			this.finish.Enter(new StateMachine<FartChore.States, FartChore.StatesInstance, FartChore, object>.State.Callback(FartChore.CreateEmission)).ReturnSuccess();
		}

		public StateMachine<FartChore.States, FartChore.StatesInstance, FartChore, object>.TargetParameter farter;

		public GameStateMachine<FartChore.States, FartChore.StatesInstance, FartChore, object>.State finish;
	}
}
