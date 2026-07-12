using System;
using UnityEngine;

public class Chore<StateMachineInstanceType> : Chore, IStateMachineTarget where StateMachineInstanceType : StateMachine.Instance
{
	public StateMachineInstanceType smi { get; protected set; }

	protected override StateMachine.Instance GetSMI()
	{
		return this.smi;
	}

	public int Subscribe(int hash, Action<object> handler)
	{
		return this.GetComponent<KPrefabID>().Subscribe(hash, handler);
	}

	public void Unsubscribe(int hash, Action<object> handler)
	{
		this.GetComponent<KPrefabID>().Unsubscribe(hash, handler);
	}

	public void Unsubscribe(int id)
	{
		this.GetComponent<KPrefabID>().Unsubscribe(id);
	}

	public void Trigger(int hash, object data = null)
	{
		this.GetComponent<KPrefabID>().Trigger(hash, data);
	}

	public ComponentType GetComponent<ComponentType>()
	{
		return base.target.GetComponent<ComponentType>();
	}

	public override GameObject gameObject
	{
		get
		{
			return base.target.gameObject;
		}
	}

	public Transform transform
	{
		get
		{
			return base.target.gameObject.transform;
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
			return base.target.isNull;
		}
	}

	public Chore(ChoreType chore_type, IStateMachineTarget target, ChoreProvider chore_provider, bool run_until_complete = true, Action<Chore> on_complete = null, Action<Chore> on_begin = null, Action<Chore> on_end = null, PriorityScreen.PriorityClass master_priority_class = PriorityScreen.PriorityClass.basic, int master_priority_value = 5, bool is_preemptable = false, bool allow_in_context_menu = true, int priority_mod = 0, bool add_to_daily_report = false, ReportManager.ReportType report_type = ReportManager.ReportType.WorkTime)
		: base(chore_type, target, chore_provider, run_until_complete, on_complete, on_begin, on_end, master_priority_class, master_priority_value, is_preemptable, allow_in_context_menu, priority_mod, add_to_daily_report, report_type)
	{
		target.Subscribe(1969584890, new Action<object>(this.OnTargetDestroyed));
		this.reportType = report_type;
		this.addToDailyReport = add_to_daily_report;
		if (this.addToDailyReport)
		{
			ReportManager.Instance.ReportValue(ReportManager.ReportType.ChoreStatus, 1f, chore_type.Name, GameUtil.GetChoreName(this, null));
		}
	}

	public override string ResolveString(string str)
	{
		if (!base.target.isNull)
		{
			str = str.Replace("{Target}", base.target.gameObject.GetProperName());
		}
		return base.ResolveString(str);
	}

	public override void Cleanup()
	{
		base.Cleanup();
		if (base.target != null)
		{
			base.target.Unsubscribe(1969584890, new Action<object>(this.OnTargetDestroyed));
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

	public override bool CanPreempt(Chore.Precondition.Context context)
	{
		return base.CanPreempt(context);
	}
}
