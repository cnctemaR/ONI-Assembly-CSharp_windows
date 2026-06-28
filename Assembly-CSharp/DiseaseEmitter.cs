using System;
using UnityEngine;

public class DiseaseEmitter : SimComponent
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.diseaseIdx = Db.Get().Diseases.GetIndex(this.diseaseID);
	}

	protected override void OnSimRegister(HandleVector<Game.ComplexCallbackInfo>.Handle cb_handle)
	{
		SimMessages.AddDiseaseEmitter(cb_handle.index);
	}

	protected override void OnSimUnregister()
	{
		DiseaseEmitter.StaticUnregister(this.simHandle);
	}

	private static void StaticUnregister(int sim_handle)
	{
		if (Sim.IsValidHandle(sim_handle))
		{
			SimMessages.RemoveDiseaseEmitter(-1, sim_handle);
		}
	}

	protected override Action<int> GetStaticUnregister()
	{
		return new Action<int>(DiseaseEmitter.StaticUnregister);
	}

	public void SetEmitting(bool emitting)
	{
		base.SetSimActive(emitting);
	}

	protected override void OnSimActivate()
	{
		if (this.diseaseIdx != 255 && this.emitCount > 0 && this.emitInterval > 0f)
		{
			int num = Grid.PosToCell(base.transform.GetPosition());
			SimMessages.ModifyDiseaseEmitter(this.simHandle, num, this.emitRange, this.diseaseIdx, this.emitInterval, this.emitCount);
		}
	}

	protected override void OnSimDeactivate()
	{
		SimMessages.ModifyDiseaseEmitter(this.simHandle, 0, 0, byte.MaxValue, 0f, 0);
	}

	[SerializeField]
	public HashedString diseaseID;

	[SerializeField]
	public byte emitRange = 1;

	[SerializeField]
	public float emitInterval = 1f;

	[SerializeField]
	public int emitCount = 1;

	private byte diseaseIdx;
}
