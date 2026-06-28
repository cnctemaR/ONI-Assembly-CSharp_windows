using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

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
		CellChangeMonitor.Instance.RegisterCellChangedHandler(base.transform, new global::System.Action(this.OnCellChange));
		this.registered = true;
	}

	private void Unregister()
	{
		if (!this.registered)
		{
			return;
		}
		CellChangeMonitor.Instance.UnregisterCellChangedHandler(base.transform, new global::System.Action(this.OnCellChange));
		this.registered = false;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return new List<Descriptor>
		{
			new Descriptor(UI.BUILDINGEFFECTS.CAPTURE_METHOD_TRAP, UI.BUILDINGEFFECTS.TOOLTIPS.CAPTURE_METHOD_TRAP, Descriptor.DescriptorType.Effect, false)
		};
	}

	private bool registered;
}
