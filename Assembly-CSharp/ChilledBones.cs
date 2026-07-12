using System;
using Klei.AI;

public class ChilledBones : GameStateMachine<ChilledBones, ChilledBones.Instance, IStateMachineTarget, ChilledBones.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.normal;
		this.normal.UpdateTransition(this.chilled, new Func<ChilledBones.Instance, float, bool>(this.IsChilling), UpdateRate.SIM_200ms, false);
		this.chilled.ToggleEffect("ChilledBones").UpdateTransition(this.normal, new Func<ChilledBones.Instance, float, bool>(this.IsNotChilling), UpdateRate.SIM_200ms, false);
	}

	public bool IsNotChilling(ChilledBones.Instance smi, float dt)
	{
		return !this.IsChilling(smi, dt);
	}

	public bool IsChilling(ChilledBones.Instance smi, float dt)
	{
		return smi.IsChilled;
	}

	public const string EFFECT_NAME = "ChilledBones";

	public GameStateMachine<ChilledBones, ChilledBones.Instance, IStateMachineTarget, ChilledBones.Def>.State normal;

	public GameStateMachine<ChilledBones, ChilledBones.Instance, IStateMachineTarget, ChilledBones.Def>.State chilled;

	public class Def : StateMachine.BaseDef
	{
		public float THRESHOLD = -1f;
	}

	public new class Instance : GameStateMachine<ChilledBones, ChilledBones.Instance, IStateMachineTarget, ChilledBones.Def>.GameInstance
	{
		public float TemperatureTransferAttribute
		{
			get
			{
				return this.minionModifiers.GetAttributes().GetValue(this.bodyTemperatureTransferAttribute.Id) * 600f;
			}
		}

		public bool IsChilled
		{
			get
			{
				return this.TemperatureTransferAttribute < base.def.THRESHOLD;
			}
		}

		public Instance(IStateMachineTarget master, ChilledBones.Def def)
			: base(master, def)
		{
			this.bodyTemperatureTransferAttribute = Db.Get().Attributes.TryGet("TemperatureDelta");
		}

		[MyCmpGet]
		public MinionModifiers minionModifiers;

		public Klei.AI.Attribute bodyTemperatureTransferAttribute;
	}
}
