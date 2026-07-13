using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Trappable")]
public class Trappable : KMonoBehaviour, IGameObjectEffectDescriptor
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.Register();
		this.OnCellChange();
	}

	protected override void OnCleanUp()
	{
		this.Unregister();
		base.OnCleanUp();
	}

	private void OnCellChange()
	{
		int num = Grid.PosToCell(this);
		GameScenePartitioner.Instance.TriggerEvent(num, GameScenePartitioner.Instance.trapsLayer, this);
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		this.Register();
	}

	protected override void OnCmpDisable()
	{
		this.Unregister();
		base.OnCmpDisable();
	}

	private void Register()
	{
		if (this.registered)
		{
			return;
		}
		base.Subscribe<Trappable>(856640610, Trappable.OnStoreDelegate);
		this.cellChangedHandlerID = Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(base.transform, Trappable.OnCellChangedDispatcher, this, "Trappable.Register");
		this.registered = true;
	}

	private void Unregister()
	{
		if (!this.registered)
		{
			return;
		}
		base.Unsubscribe<Trappable>(856640610, Trappable.OnStoreDelegate, false);
		Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(ref this.cellChangedHandlerID);
		this.registered = false;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return new List<Descriptor>
		{
			new Descriptor(UI.BUILDINGEFFECTS.CAPTURE_METHOD_LAND_TRAP, UI.BUILDINGEFFECTS.TOOLTIPS.CAPTURE_METHOD_TRAP, Descriptor.DescriptorType.Effect, false)
		};
	}

	public void OnStore(object data)
	{
		Storage storage = data as Storage;
		if (storage && (storage.GetComponent<Trap>() != null || storage.GetSMI<ReusableTrap.Instance>() != null))
		{
			base.gameObject.AddTag(GameTags.Trapped);
			Navigator component = base.gameObject.GetComponent<Navigator>();
			if (component != null)
			{
				component.Stop(false, true);
			}
			Brain component2 = base.gameObject.GetComponent<Brain>();
			if (component2 != null)
			{
				Game.BrainScheduler.PrioritizeBrain(component2);
				return;
			}
		}
		else
		{
			base.gameObject.RemoveTag(GameTags.Trapped);
		}
	}

	private bool registered;

	private ulong cellChangedHandlerID;

	private static readonly Action<object> OnCellChangedDispatcher = delegate(object obj)
	{
		Unsafe.As<Trappable>(obj).OnCellChange();
	};

	private static readonly EventSystem.IntraObjectHandler<Trappable> OnStoreDelegate = new EventSystem.IntraObjectHandler<Trappable>(delegate(Trappable component, object data)
	{
		component.OnStore(data);
	});
}
