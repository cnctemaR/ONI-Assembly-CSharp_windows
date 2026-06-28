using System;
using UnityEngine;

public class Chore<StateMachineInstanceType> : Chore, IStateMachineTarget where StateMachineInstanceType : StateMachine.Instance
{
	public Chore(ChoreType chore_type, IStateMachineTarget target, ChoreProvider chore_provider, bool run_until_complete = true, Action<Chore> on_complete = null, Action<Chore> on_begin = null, Action<Chore> on_end = null, int master_priority = 2147483647, bool is_preemptable = false, bool allow_in_context_menu = true)
		: base(chore_type, chore_provider, run_until_complete, on_complete, on_begin, on_end, master_priority, is_preemptable, allow_in_context_menu)
	{
		this.target = target;
		target.Subscribe(1969584890, new EventSystem.EventHandler(this.OnTargetDestroyed));
	}

	public StateMachine.Instance sm
	{
		get
		{
			return this.smi;
		}
	}

	protected override StateMachine.Instance GetSMI()
	{
		return this.smi;
	}

	public void Subscribe(int hash, EventSystem.EventHandler handler)
	{
		this.GetComponent<KPrefabID>().Subscribe(hash, handler);
	}

	public void Unsubscribe(int hash, EventSystem.EventHandler handler)
	{
		this.GetComponent<KPrefabID>().Unsubscribe(hash, handler);
	}

	public void Trigger(int hash, object data = null)
	{
		this.GetComponent<KPrefabID>().Trigger(hash, data);
	}

	public ComponentType GetComponent<ComponentType>()
	{
		return this.target.GetComponent<ComponentType>();
	}

	public override GameObject gameObject
	{
		get
		{
			return this.target.gameObject;
		}
	}

	public Transform transform
	{
		get
		{
			return this.target.gameObject.transform;
		}
	}

	public string name
	{
		get
		{
			return this.gameObject.name;
		}
	}

	public override bool isNull
	{
		get
		{
			return this.target.isNull;
		}
	}

	public override string ResolveString(string str)
	{
		if (!this.target.isNull)
		{
			str = str.Replace("{Target}", this.target.gameObject.GetProperName());
		}
		return base.ResolveString(str);
	}

	public override void Cleanup()
	{
		base.Cleanup();
		if (this.target != null)
		{
			this.target.Unsubscribe(1969584890, new EventSystem.EventHandler(this.OnTargetDestroyed));
		}
		if (this.onCleanup != null)
		{
			this.onCleanup(this);
		}
	}

	private void OnTargetDestroyed(object data)
	{
		base.Cancel("Target Destroyed");
	}

	protected IStateMachineTarget target;

	protected StateMachineInstanceType smi;
}
