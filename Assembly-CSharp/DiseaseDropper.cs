using System;

public class DiseaseDropper : GameStateMachine<DiseaseDropper, DiseaseDropper.Instance, IStateMachineTarget, DiseaseDropper.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.stopped;
		this.root.EventHandler(GameHashes.BurstEmitDisease, delegate(DiseaseDropper.Instance smi)
		{
			smi.DropSingleEmit();
		});
		this.working.TagTransition(GameTags.PreventEmittingDisease, this.stopped, false).Update(delegate(DiseaseDropper.Instance smi, float dt)
		{
			smi.DropPeriodic(dt);
		}, UpdateRate.SIM_200ms, false);
		this.stopped.TagTransition(GameTags.PreventEmittingDisease, this.working, true);
	}

	public GameStateMachine<DiseaseDropper, DiseaseDropper.Instance, IStateMachineTarget, DiseaseDropper.Def>.State working;

	public GameStateMachine<DiseaseDropper, DiseaseDropper.Instance, IStateMachineTarget, DiseaseDropper.Def>.State stopped;

	public StateMachine<DiseaseDropper, DiseaseDropper.Instance, IStateMachineTarget, DiseaseDropper.Def>.Signal cellChangedSignal;

	public class Def : StateMachine.BaseDef
	{
		public byte diseaseIdx = byte.MaxValue;

		public int singleEmitQuantity;

		public int averageEmitPerSecond;

		public float emitFrequency = 1f;
	}

	public new class Instance : GameStateMachine<DiseaseDropper, DiseaseDropper.Instance, IStateMachineTarget, DiseaseDropper.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, DiseaseDropper.Def def)
			: base(master, def)
		{
		}

		public bool ShouldDropDisease()
		{
			return true;
		}

		public void DropSingleEmit()
		{
			this.DropDisease(base.def.diseaseIdx, base.def.singleEmitQuantity);
		}

		public void DropPeriodic(float dt)
		{
			this.timeSinceLastDrop += dt;
			if (base.def.averageEmitPerSecond > 0 && base.def.emitFrequency > 0f)
			{
				while (this.timeSinceLastDrop > base.def.emitFrequency)
				{
					this.DropDisease(base.def.diseaseIdx, (int)((float)base.def.averageEmitPerSecond * base.def.emitFrequency));
					this.timeSinceLastDrop -= base.def.emitFrequency;
				}
			}
		}

		public void DropDisease(byte disease_idx, int disease_count)
		{
			if (disease_count <= 0 || disease_idx == 255)
			{
				return;
			}
			int num = Grid.PosToCell(base.transform.GetPosition());
			if (!Grid.IsValidCell(num))
			{
				return;
			}
			SimMessages.ModifyDiseaseOnCell(num, disease_idx, disease_count);
		}

		public bool IsValidDropCell()
		{
			int num = Grid.PosToCell(base.transform.GetPosition());
			return Grid.IsValidCell(num) && Grid.IsGas(num) && Grid.Mass[num] <= 1f;
		}

		private float timeSinceLastDrop;
	}
}
