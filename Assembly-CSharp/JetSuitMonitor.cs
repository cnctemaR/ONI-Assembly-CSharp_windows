using System;
using UnityEngine;

public class JetSuitMonitor : GameStateMachine<JetSuitMonitor, JetSuitMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.off;
		base.Target(this.owner);
		this.off.EventTransition(GameHashes.PathAdvanced, this.flying, new StateMachine<JetSuitMonitor, JetSuitMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(JetSuitMonitor.ShouldStartFlying));
		this.flying.Enter(new StateMachine<JetSuitMonitor, JetSuitMonitor.Instance, IStateMachineTarget, object>.State.Callback(JetSuitMonitor.StartFlying)).Exit(new StateMachine<JetSuitMonitor, JetSuitMonitor.Instance, IStateMachineTarget, object>.State.Callback(JetSuitMonitor.StopFlying)).EventTransition(GameHashes.PathAdvanced, this.off, new StateMachine<JetSuitMonitor, JetSuitMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(JetSuitMonitor.ShouldStopFlying))
			.Update(new Action<JetSuitMonitor.Instance, float>(JetSuitMonitor.Emit), UpdateRate.SIM_200ms, false);
	}

	public static bool ShouldStartFlying(JetSuitMonitor.Instance smi)
	{
		return smi.navigator && smi.navigator.CurrentNavType == NavType.Hover;
	}

	public static bool ShouldStopFlying(JetSuitMonitor.Instance smi)
	{
		return !smi.navigator || smi.navigator.CurrentNavType != NavType.Hover;
	}

	public static void StartFlying(JetSuitMonitor.Instance smi)
	{
	}

	public static void StopFlying(JetSuitMonitor.Instance smi)
	{
	}

	public static void Emit(JetSuitMonitor.Instance smi, float dt)
	{
		if (!smi.navigator)
		{
			return;
		}
		GameObject gameObject = smi.sm.owner.Get(smi);
		if (!gameObject)
		{
			return;
		}
		Grid.PosToCell(gameObject.transform.GetPosition());
		float num = 0.2f * dt;
		num = Mathf.Min(num, smi.jet_suit_tank.amount);
		smi.jet_suit_tank.amount -= num;
		float num2 = num * 0.25f;
		if (num2 > 1E-45f)
		{
			Vector3 vector3;
			Vector3 vector2;
			Vector3 vector = (vector2 = (vector3 = gameObject.transform.position));
			Vector3 down = Vector3.down;
			if (smi.helmetController.jet_anim != null)
			{
				KBatchedAnimController jet_anim = smi.helmetController.jet_anim;
				bool flag;
				Matrix4x4 symbolTransform = jet_anim.GetSymbolTransform("left_fire", out flag);
				Matrix4x4 symbolTransform2 = jet_anim.GetSymbolTransform("right_fire", out flag);
				vector3 = symbolTransform.GetColumn(3);
				vector2 = symbolTransform2.GetColumn(3);
				float num3 = Quaternion.LookRotation(symbolTransform.GetColumn(2), symbolTransform.GetColumn(1)).eulerAngles.z * 0.017453292f;
				down = new Vector3(-Mathf.Sin(num3), Mathf.Cos(num3));
				vector3 += down.normalized * 0.6f;
				vector2 += down.normalized * 0.6f;
			}
			float num4 = num2 / 2f;
			float num5 = 0.5f;
			int num6 = Grid.PosToCell(vector);
			CO2Manager.instance.SpawnExhaust(vector3, down.normalized * num5, num6, num4, 373.15f);
			CO2Manager.instance.SpawnExhaust(vector2, down.normalized * num5, num6, num4, 373.15f);
		}
		if (smi.jet_suit_tank.amount == 0f)
		{
			smi.navigator.AddTag(GameTags.JetSuitOutOfFuel);
			smi.navigator.SetCurrentNavType(NavType.Floor);
		}
	}

	public GameStateMachine<JetSuitMonitor, JetSuitMonitor.Instance, IStateMachineTarget, object>.State off;

	public GameStateMachine<JetSuitMonitor, JetSuitMonitor.Instance, IStateMachineTarget, object>.State flying;

	public StateMachine<JetSuitMonitor, JetSuitMonitor.Instance, IStateMachineTarget, object>.TargetParameter owner;

	public new class Instance : GameStateMachine<JetSuitMonitor, JetSuitMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master, GameObject owner)
			: base(master)
		{
			base.sm.owner.Set(owner, base.smi, false);
			this.helmetController = master.GetComponent<HelmetController>();
			this.navigator = owner.GetComponent<Navigator>();
			this.jet_suit_tank = master.GetComponent<JetSuitTank>();
		}

		public HelmetController helmetController;

		public Navigator navigator;

		public JetSuitTank jet_suit_tank;
	}
}
