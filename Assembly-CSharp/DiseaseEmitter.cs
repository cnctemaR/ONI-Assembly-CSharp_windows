using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Klei.AI;
using KSerialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/DiseaseEmitter")]
public class DiseaseEmitter : KMonoBehaviour
{
	public float EmitRate
	{
		get
		{
			return this.emitRate;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.emitDiseases != null)
		{
			this.simHandles = new int[this.emitDiseases.Length];
			for (int i = 0; i < this.simHandles.Length; i++)
			{
				this.simHandles[i] = -1;
			}
		}
		this.SimRegister();
	}

	protected override void OnCleanUp()
	{
		this.SimUnregister();
		base.OnCleanUp();
	}

	public void SetEnable(bool enable)
	{
		if (this.enableEmitter == enable)
		{
			return;
		}
		this.enableEmitter = enable;
		if (this.enableEmitter)
		{
			this.SimRegister();
			return;
		}
		this.SimUnregister();
	}

	private void SimModifyDiseaseEmitter(int emitterIndex, int cell)
	{
		SimMessages.ModifyDiseaseEmitter(this.simHandles[emitterIndex], cell, this.emitRange, this.emitDiseases[emitterIndex], this.emitRate, this.emitCount);
	}

	protected void OnCellChanged()
	{
		DebugUtil.DevAssert(this.simHandles != null, "DiseaseEmitter received cell change notification but has not been Spawned?!", null);
		if (this.simHandles == null || !this.enableEmitter)
		{
			return;
		}
		int num = Grid.PosToCell(this);
		if (Grid.IsValidCell(num))
		{
			for (int i = 0; i < this.emitDiseases.Length; i++)
			{
				if (Sim.IsValidHandle(this.simHandles[i]))
				{
					this.SimModifyDiseaseEmitter(i, num);
				}
			}
		}
	}

	private void SimRegister()
	{
		DebugUtil.DevAssert(this.simHandles != null, "DiseaseEmitter.SimRegister invoked but has not been Spawned?!", null);
		if (this.simHandles == null || !this.enableEmitter)
		{
			return;
		}
		if (this.cellChangedHandlerID != 0UL)
		{
			this.cellChangedHandlerID = Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(base.transform, DiseaseEmitter.OnCellChangedDispatcher, this, null);
		}
		for (int i = 0; i < this.simHandles.Length; i++)
		{
			if (this.simHandles[i] == -1)
			{
				this.simHandles[i] = -2;
				SimMessages.AddDiseaseEmitter(Game.Instance.simComponentCallbackManager.Add(new Action<int, object>(DiseaseEmitter.OnSimRegisteredCallback), new DiseaseEmitter.EmitterRegistration
				{
					emitter = this,
					emitterIndex = i
				}, "DiseaseEmitter").index);
			}
		}
	}

	private void SimUnregister()
	{
		DebugUtil.DevAssert(this.simHandles != null, "DiseaseEmitter.SimUnregister invoked but has not been Spawned?!", null);
		if (this.simHandles == null)
		{
			return;
		}
		for (int i = 0; i < this.simHandles.Length; i++)
		{
			if (Sim.IsValidHandle(this.simHandles[i]))
			{
				SimMessages.RemoveDiseaseEmitter(-1, this.simHandles[i]);
			}
			this.simHandles[i] = -1;
		}
		Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(ref this.cellChangedHandlerID);
	}

	private static void OnSimRegisteredCallback(int handle, object data)
	{
		DiseaseEmitter.EmitterRegistration emitterRegistration = (DiseaseEmitter.EmitterRegistration)data;
		emitterRegistration.emitter.OnSimRegistered(handle, emitterRegistration.emitterIndex);
	}

	private void OnSimRegistered(int handle, int emitterIndex)
	{
		if (this.IsNullOrDestroyed())
		{
			SimMessages.RemoveDiseaseEmitter(-1, handle);
			return;
		}
		this.simHandles[emitterIndex] = handle;
		int num = Grid.PosToCell(this);
		DebugUtil.DevAssert(Grid.IsValidCell(num), "Failed to initialize DiseaseEmitter because it is on an invalid cell", null);
		if (Grid.IsValidCell(num))
		{
			this.SimModifyDiseaseEmitter(emitterIndex, num);
		}
	}

	public void SetDiseases(List<Disease> diseases)
	{
		this.emitDiseases = new byte[diseases.Count];
		for (int i = 0; i < diseases.Count; i++)
		{
			this.emitDiseases[i] = Db.Get().Diseases.GetIndex(diseases[i].id);
		}
	}

	[Serialize]
	public float emitRate = 1f;

	[Serialize]
	public byte emitRange;

	[Serialize]
	public int emitCount;

	[Serialize]
	public byte[] emitDiseases;

	public int[] simHandles;

	[Serialize]
	protected bool enableEmitter = true;

	private ulong cellChangedHandlerID;

	private static readonly Action<object> OnCellChangedDispatcher = delegate(object obj)
	{
		Unsafe.As<DiseaseEmitter>(obj).OnCellChanged();
	};

	private struct EmitterRegistration
	{
		public DiseaseEmitter emitter;

		public int emitterIndex;
	}
}
