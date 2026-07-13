using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class RadiationEmitter : SimComponent
{
	protected override void OnSpawn()
	{
		this.cellChangedHandlerID = Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(base.transform, RadiationEmitter.RefreshDispatcher, this, "RadiationEmitter.OnSpawn");
		base.OnSpawn();
	}

	protected override void OnCleanUp()
	{
		Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(ref this.cellChangedHandlerID);
		base.OnCleanUp();
	}

	public void SetEmitting(bool emitting)
	{
		base.SetSimActive(emitting);
	}

	public int GetEmissionCell()
	{
		return Grid.PosToCell(base.transform.GetPosition() + this.emissionOffset);
	}

	public void Refresh()
	{
		int emissionCell = this.GetEmissionCell();
		if (this.radiusProportionalToRads)
		{
			this.SetRadiusProportionalToRads();
		}
		SimMessages.ModifyRadiationEmitter(this.simHandle, emissionCell, this.emitRadiusX, this.emitRadiusY, this.emitRads, this.emitRate, this.emitSpeed, this.emitDirection, this.emitAngle, this.emitType);
	}

	private void SetRadiusProportionalToRads()
	{
		this.emitRadiusX = (short)Mathf.Clamp(Mathf.RoundToInt(this.emitRads * 1f), 1, 128);
		this.emitRadiusY = (short)Mathf.Clamp(Mathf.RoundToInt(this.emitRads * 1f), 1, 128);
	}

	protected override void OnSimActivate()
	{
		int emissionCell = this.GetEmissionCell();
		if (this.radiusProportionalToRads)
		{
			this.SetRadiusProportionalToRads();
		}
		SimMessages.ModifyRadiationEmitter(this.simHandle, emissionCell, this.emitRadiusX, this.emitRadiusY, this.emitRads, this.emitRate, this.emitSpeed, this.emitDirection, this.emitAngle, this.emitType);
	}

	protected override void OnSimDeactivate()
	{
		int emissionCell = this.GetEmissionCell();
		SimMessages.ModifyRadiationEmitter(this.simHandle, emissionCell, 0, 0, 0f, 0f, 0f, 0f, 0f, this.emitType);
	}

	protected override void OnSimRegister(HandleVector<Game.ComplexCallbackInfo<int>>.Handle cb_handle)
	{
		Game.Instance.simComponentCallbackManager.GetItem(cb_handle);
		int emissionCell = this.GetEmissionCell();
		SimMessages.AddRadiationEmitter(cb_handle.index, emissionCell, 0, 0, 0f, 0f, 0f, 0f, 0f, this.emitType);
	}

	protected override void OnSimUnregister()
	{
		RadiationEmitter.StaticUnregister(this.simHandle);
	}

	private static void StaticUnregister(int sim_handle)
	{
		global::Debug.Assert(Sim.IsValidHandle(sim_handle));
		SimMessages.RemoveRadiationEmitter(-1, sim_handle);
	}

	private void OnDrawGizmosSelected()
	{
		int emissionCell = this.GetEmissionCell();
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(Grid.CellToPos(emissionCell) + Vector3.right / 2f + Vector3.up / 2f, 0.2f);
	}

	protected override Action<int> GetStaticUnregister()
	{
		return new Action<int>(RadiationEmitter.StaticUnregister);
	}

	public bool radiusProportionalToRads;

	[SerializeField]
	public short emitRadiusX = 4;

	[SerializeField]
	public short emitRadiusY = 4;

	[SerializeField]
	public float emitRads = 10f;

	[SerializeField]
	public float emitRate = 1f;

	[SerializeField]
	public float emitSpeed = 1f;

	[SerializeField]
	public float emitDirection;

	[SerializeField]
	public float emitAngle = 360f;

	[SerializeField]
	public RadiationEmitter.RadiationEmitterType emitType;

	[SerializeField]
	public Vector3 emissionOffset = Vector3.zero;

	private ulong cellChangedHandlerID;

	private static readonly Action<object> RefreshDispatcher = delegate(object obj)
	{
		Unsafe.As<RadiationEmitter>(obj).Refresh();
	};

	public enum RadiationEmitterType
	{
		Constant,
		Pulsing,
		PulsingAveraged,
		SimplePulse,
		RadialBeams,
		Attractor
	}
}
