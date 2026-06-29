using System;
using System.Diagnostics;
using UnityEngine;

public abstract class SimComponent : KMonoBehaviour, ISim200ms
{
	protected virtual void OnSimRegister(HandleVector<Game.ComplexCallbackInfo>.Handle cb_handle)
	{
	}

	protected virtual void OnSimRegistered()
	{
	}

	protected virtual void OnSimActivate()
	{
	}

	protected virtual void OnSimDeactivate()
	{
	}

	protected virtual void OnSimUnregister()
	{
	}

	protected abstract Action<int> GetStaticUnregister();

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.SimRegister();
	}

	protected override void OnCleanUp()
	{
		this.SimUnregister();
		base.OnCleanUp();
	}

	public void SetSimActive(bool active)
	{
		this.simActive = active;
		this.dirty = true;
	}

	public void Sim200ms(float dt)
	{
		if (!Sim.IsValidHandle(this.simHandle))
		{
			return;
		}
		this.UpdateSimState();
	}

	private void UpdateSimState()
	{
		if (!this.dirty)
		{
			return;
		}
		this.dirty = false;
		if (this.simActive)
		{
			this.OnSimActivate();
		}
		else
		{
			this.OnSimDeactivate();
		}
	}

	private void SimRegister()
	{
		if (base.isSpawned && this.simHandle == -1)
		{
			this.simHandle = -2;
			Action<int> static_unregister = this.GetStaticUnregister();
			HandleVector<Game.ComplexCallbackInfo>.Handle handle = Game.Instance.complexCallbackManager.Add(new Game.ComplexCallbackInfo(delegate(object data)
			{
				SimComponent.OnSimRegistered(this, data, static_unregister);
			}, "SimComponent.SimREgister"));
			this.OnSimRegister(handle);
		}
	}

	private void SimUnregister()
	{
		if (Sim.IsValidHandle(this.simHandle))
		{
			this.OnSimUnregister();
		}
		this.simHandle = -1;
	}

	private static void OnSimRegistered(SimComponent instance, object data, Action<int> static_unregister)
	{
		int num = (int)data;
		if (instance != null)
		{
			instance.simHandle = num;
			instance.OnSimRegistered();
		}
		else
		{
			static_unregister(num);
		}
	}

	[Conditional("ENABLE_LOGGER")]
	protected void Log(string msg)
	{
	}

	[SerializeField]
	protected int simHandle = -1;

	private bool simActive = true;

	private bool dirty = true;
}
