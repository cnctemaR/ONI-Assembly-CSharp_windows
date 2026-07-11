using System;
using Klei.AI;
using STRINGS;

public class LightMonitor : GameStateMachine<LightMonitor, LightMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.unburnt;
		this.root.EventTransition(GameHashes.SicknessAdded, this.burnt, (LightMonitor.Instance smi) => smi.gameObject.GetSicknesses().Has(Db.Get().Sicknesses.Sunburn)).Update(new Action<LightMonitor.Instance, float>(LightMonitor.CheckLightLevel), UpdateRate.SIM_1000ms, false);
		this.unburnt.DefaultState(this.unburnt.safe).ParamTransition<float>(this.burnResistance, this.get_burnt, GameStateMachine<LightMonitor, LightMonitor.Instance, IStateMachineTarget, object>.IsLTEZero);
		this.unburnt.safe.DefaultState(this.unburnt.safe.unlit).Update(delegate(LightMonitor.Instance smi, float dt)
		{
			smi.sm.burnResistance.DeltaClamp(dt * 0.25f, 0f, 120f, smi);
		}, UpdateRate.SIM_200ms, false);
		this.unburnt.safe.unlit.ParamTransition<float>(this.lightLevel, this.unburnt.safe.normal_light, GameStateMachine<LightMonitor, LightMonitor.Instance, IStateMachineTarget, object>.IsGTZero);
		this.unburnt.safe.normal_light.ParamTransition<float>(this.lightLevel, this.unburnt.safe.unlit, GameStateMachine<LightMonitor, LightMonitor.Instance, IStateMachineTarget, object>.IsLTEZero).ParamTransition<float>(this.lightLevel, this.unburnt.safe.sunlight, (LightMonitor.Instance smi, float p) => p >= 40000f);
		this.unburnt.safe.sunlight.ParamTransition<float>(this.lightLevel, this.unburnt.safe.normal_light, (LightMonitor.Instance smi, float p) => p < 40000f).ParamTransition<float>(this.lightLevel, this.unburnt.burning, (LightMonitor.Instance smi, float p) => p >= 72000f).ToggleEffect("Sunlight_Pleasant");
		this.unburnt.burning.ParamTransition<float>(this.lightLevel, this.unburnt.safe.sunlight, (LightMonitor.Instance smi, float p) => p < 72000f).Update(delegate(LightMonitor.Instance smi, float dt)
		{
			smi.sm.burnResistance.DeltaClamp(-dt, 0f, 120f, smi);
		}, UpdateRate.SIM_200ms, false).ToggleEffect("Sunlight_Burning");
		this.get_burnt.Enter(delegate(LightMonitor.Instance smi)
		{
			smi.gameObject.GetSicknesses().Infect(new SicknessExposureInfo(Db.Get().Sicknesses.Sunburn.Id, DUPLICANTS.DISEASES.SUNBURNSICKNESS.SUNEXPOSURE));
		}).GoTo(this.burnt);
		this.burnt.EventTransition(GameHashes.SicknessCured, this.unburnt, (LightMonitor.Instance smi) => !smi.gameObject.GetSicknesses().Has(Db.Get().Sicknesses.Sunburn)).Exit(delegate(LightMonitor.Instance smi)
		{
			smi.sm.burnResistance.Set(120f, smi);
		});
	}

	private static void CheckLightLevel(LightMonitor.Instance smi, float dt)
	{
		KPrefabID component = smi.GetComponent<KPrefabID>();
		if (component != null && component.HasTag(GameTags.Shaded))
		{
			smi.sm.lightLevel.Set(0f, smi);
			return;
		}
		int num = Grid.PosToCell(smi.gameObject);
		if (Grid.IsValidCell(num))
		{
			smi.sm.lightLevel.Set((float)Grid.LightIntensity[num], smi);
		}
	}

	public const float BURN_RESIST_RECOVERY_FACTOR = 0.25f;

	public StateMachine<LightMonitor, LightMonitor.Instance, IStateMachineTarget, object>.FloatParameter lightLevel;

	public StateMachine<LightMonitor, LightMonitor.Instance, IStateMachineTarget, object>.FloatParameter burnResistance = new StateMachine<LightMonitor, LightMonitor.Instance, IStateMachineTarget, object>.FloatParameter(120f);

	public LightMonitor.UnburntStates unburnt;

	public GameStateMachine<LightMonitor, LightMonitor.Instance, IStateMachineTarget, object>.State get_burnt;

	public GameStateMachine<LightMonitor, LightMonitor.Instance, IStateMachineTarget, object>.State burnt;

	public class UnburntStates : GameStateMachine<LightMonitor, LightMonitor.Instance, IStateMachineTarget, object>.State
	{
		public LightMonitor.SafeStates safe;

		public GameStateMachine<LightMonitor, LightMonitor.Instance, IStateMachineTarget, object>.State burning;
	}

	public class SafeStates : GameStateMachine<LightMonitor, LightMonitor.Instance, IStateMachineTarget, object>.State
	{
		public GameStateMachine<LightMonitor, LightMonitor.Instance, IStateMachineTarget, object>.State unlit;

		public GameStateMachine<LightMonitor, LightMonitor.Instance, IStateMachineTarget, object>.State normal_light;

		public GameStateMachine<LightMonitor, LightMonitor.Instance, IStateMachineTarget, object>.State sunlight;
	}

	public new class Instance : GameStateMachine<LightMonitor, LightMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			this.effects = base.GetComponent<Effects>();
		}

		public Effects effects;
	}
}
