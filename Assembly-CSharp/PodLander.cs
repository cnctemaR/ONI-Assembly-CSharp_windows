using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class PodLander : StateMachineComponent<PodLander.StatesInstance>, IGameObjectEffectDescriptor
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	public void ReleaseAstronaut()
	{
		if (this.releasingAstronaut)
		{
			return;
		}
		this.releasingAstronaut = true;
		MinionStorage component = base.GetComponent<MinionStorage>();
		List<MinionStorage.Info> storedMinionInfo = component.GetStoredMinionInfo();
		for (int i = storedMinionInfo.Count - 1; i >= 0; i--)
		{
			MinionStorage.Info info = storedMinionInfo[i];
			component.DeserializeMinion(info.id, Grid.CellToPos(Grid.PosToCell(base.smi.master.transform.GetPosition())));
		}
		this.releasingAstronaut = false;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return null;
	}

	[Serialize]
	private int landOffLocation;

	[Serialize]
	private float flightAnimOffset;

	private float rocketSpeed;

	public float exhaustEmitRate = 2f;

	public float exhaustTemperature = 1000f;

	public SimHashes exhaustElement = SimHashes.CarbonDioxide;

	private GameObject soundSpeakerObject;

	private bool releasingAstronaut;

	public class StatesInstance : GameStateMachine<PodLander.States, PodLander.StatesInstance, PodLander, object>.GameInstance
	{
		public StatesInstance(PodLander master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<PodLander.States, PodLander.StatesInstance, PodLander>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.landing;
			base.serializable = StateMachine.SerializeType.Both_DEPRECATED;
			this.landing.PlayAnim("launch_loop", KAnim.PlayMode.Loop).Enter(delegate(PodLander.StatesInstance smi)
			{
				smi.master.flightAnimOffset = 50f;
			}).Update(delegate(PodLander.StatesInstance smi, float dt)
			{
				float num = 10f;
				smi.master.rocketSpeed = num - Mathf.Clamp(Mathf.Pow(smi.timeinstate / 3.5f, 4f), 0f, num - 2f);
				smi.master.flightAnimOffset -= dt * smi.master.rocketSpeed;
				KBatchedAnimController component = smi.master.GetComponent<KBatchedAnimController>();
				component.Offset = Vector3.up * smi.master.flightAnimOffset;
				Vector3 positionIncludingOffset = component.PositionIncludingOffset;
				int num2 = Grid.PosToCell(smi.master.gameObject.transform.GetPosition() + smi.master.GetComponent<KBatchedAnimController>().Offset);
				if (Grid.IsValidCell(num2))
				{
					SimMessages.EmitMass(num2, ElementLoader.GetElementIndex(smi.master.exhaustElement), dt * smi.master.exhaustEmitRate, smi.master.exhaustTemperature, 0, 0, -1);
				}
				if (component.Offset.y <= 0f)
				{
					smi.GoTo(this.crashed);
				}
			}, UpdateRate.SIM_33ms, false);
			this.crashed.PlayAnim("grounded").Enter(delegate(PodLander.StatesInstance smi)
			{
				smi.master.GetComponent<KBatchedAnimController>().Offset = Vector3.zero;
				smi.master.rocketSpeed = 0f;
				smi.master.ReleaseAstronaut();
			});
		}

		public GameStateMachine<PodLander.States, PodLander.StatesInstance, PodLander, object>.State landing;

		public GameStateMachine<PodLander.States, PodLander.StatesInstance, PodLander, object>.State crashed;
	}
}
