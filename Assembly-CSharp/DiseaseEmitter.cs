using System;
using System.Collections.Generic;
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

	private void OnCellChanged()
	{
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
					SimMessages.ModifyDiseaseEmitter(this.simHandles[i], num, this.emitRange, this.emitDiseases[i], this.emitRate, this.emitCount);
				}
			}
		}
	}

	private void SimRegister()
	{
		if (this.simHandles == null || !this.enableEmitter)
		{
			return;
		}
		Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(base.transform, new global::System.Action(this.OnCellChanged), "DiseaseEmitter.Modify");
		for (int i = 0; i < this.simHandles.Length; i++)
		{
			if (this.simHandles[i] == -1)
			{
				this.simHandles[i] = -2;
				SimMessages.AddDiseaseEmitter(Game.Instance.simComponentCallbackManager.Add(new Action<int, object>(DiseaseEmitter.OnSimRegisteredCallback), this, "DiseaseEmitter").index);
			}
		}
	}

	private void SimUnregister()
	{
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
		Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(base.transform, new global::System.Action(this.OnCellChanged));
	}

	private static void OnSimRegisteredCallback(int handle, object data)
	{
		((DiseaseEmitter)data).OnSimRegistered(handle);
	}

	private void OnSimRegistered(int handle)
	{
		bool flag = false;
		if (this != null)
		{
			for (int i = 0; i < this.simHandles.Length; i++)
			{
				if (this.simHandles[i] == -2)
				{
					this.simHandles[i] = handle;
					flag = true;
					break;
				}
			}
			this.OnCellChanged();
		}
		if (!flag)
		{
			SimMessages.RemoveDiseaseEmitter(-1, handle);
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
	private bool enableEmitter;
}
