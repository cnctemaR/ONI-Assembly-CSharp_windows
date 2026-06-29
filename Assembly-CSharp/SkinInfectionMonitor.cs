using System;
using Klei.AI;
using UnityEngine;

public class SkinInfectionMonitor : GameStateMachine<SkinInfectionMonitor, SkinInfectionMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.clean;
		base.serializable = false;
		this.clean.EventTransition(GameHashes.ExposeToDisease, this.dirty, (SkinInfectionMonitor.Instance smi) => smi.IsInfecting());
		this.dirty.Update(delegate(SkinInfectionMonitor.Instance smi, float dt)
		{
			smi.GetInfectedByContainedDisease(dt);
		}, UpdateRate.SIM_200ms, false).EventTransition(GameHashes.ExposeToDisease, this.clean, (SkinInfectionMonitor.Instance smi) => !smi.IsInfecting());
	}

	public GameStateMachine<SkinInfectionMonitor, SkinInfectionMonitor.Instance, IStateMachineTarget, object>.State clean;

	public GameStateMachine<SkinInfectionMonitor, SkinInfectionMonitor.Instance, IStateMachineTarget, object>.State dirty;

	public new class Instance : GameStateMachine<SkinInfectionMonitor, SkinInfectionMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			this.primaryElement = master.GetComponent<PrimaryElement>();
			this.diseaseConsumptionHandle = Game.Instance.complexCallbackManager.Add(new Game.ComplexCallbackInfo(new Action<object>(this.OnDiseaseConsumed), "SkinInfectionMonitor"));
		}

		public override void StartSM()
		{
			this.immuneSystemMonitor = this.controller.GetSMI<ImmuneSystemMonitor.Instance>();
			base.StartSM();
		}

		public override void StopSM(string reason)
		{
			if (this.diseaseConsumptionHandle.IsValid())
			{
				Game.Instance.complexCallbackManager.Release(this.diseaseConsumptionHandle);
				this.diseaseConsumptionHandle.Clear();
			}
			base.StopSM(reason);
		}

		public void GetInfectedByContainedDisease(float dt)
		{
			byte diseaseIdx = this.primaryElement.DiseaseIdx;
			if (diseaseIdx != 255)
			{
				Disease disease = Db.Get().Diseases[(int)diseaseIdx];
				if (disease.infectionVectors.Contains(Disease.InfectionVector.Contact))
				{
					int num = Mathf.CeilToInt(dt);
					this.primaryElement.ModifyDiseaseCount(-num, "SkinInfectionMonitor.GetInfectedByContainedDisease");
					this.immuneSystemMonitor.InjectDisease(disease, num, Tag.Invalid, Disease.InfectionVector.Contact);
				}
			}
			else
			{
				base.smi.GoTo(base.sm.clean);
			}
		}

		public void InteractWithWorld()
		{
			float time = Time.time;
			if (time - this.lastInteractTime > 1f)
			{
				int num = Grid.PosToCell(base.master.transform.GetPosition());
				int num2 = Grid.CellAbove(num);
				int num3 = Grid.CellBelow(num);
				SimMessages.ConsumeDisease(num, 0.016666668f, 250000, this.diseaseConsumptionHandle.index);
				if (Grid.IsValidCell(num2))
				{
					SimMessages.ConsumeDisease(num2, 0.016666668f, 250000, this.diseaseConsumptionHandle.index);
				}
				if (Grid.IsValidCell(num3))
				{
					SimMessages.ConsumeDisease(num3, 0.0016666667f, 25000, this.diseaseConsumptionHandle.index);
				}
				this.lastInteractTime = time;
			}
		}

		private void OnDiseaseConsumed(object data)
		{
			if (this.diseaseConsumptionHandle.IsValid())
			{
				Sim.DiseaseConsumptionCallback diseaseConsumptionCallback = (Sim.DiseaseConsumptionCallback)data;
				if (diseaseConsumptionCallback.diseaseIdx != 255)
				{
					this.primaryElement.AddDisease(diseaseConsumptionCallback.diseaseIdx, diseaseConsumptionCallback.diseaseCount, "SkinInfectionMonitor.OnDiseaseConsumed");
				}
			}
		}

		public bool IsInfecting()
		{
			byte diseaseIdx = this.primaryElement.DiseaseIdx;
			return diseaseIdx != byte.MaxValue;
		}

		private PrimaryElement primaryElement;

		private ImmuneSystemMonitor.Instance immuneSystemMonitor;

		private const float INTERACT_INTERVAL = 1f;

		private float lastInteractTime = float.NegativeInfinity;

		private HandleVector<Game.ComplexCallbackInfo>.Handle diseaseConsumptionHandle;
	}
}
