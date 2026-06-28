using System;
using Klei.AI;
using KSerialization;
using UnityEngine;
using UnityEngine.UI;

[SerializationConfig(MemberSerialization.OptIn)]
public class Workable : KMonoBehaviour, ISaveLoadable, IApproachable
{
	public Workable()
	{
		this.preferPrimaryCell = true;
	}

	public Worker worker { get; protected set; }

	public float WorkTimeRemaining
	{
		get
		{
			return this.workTimeRemaining;
		}
		set
		{
			this.workTimeRemaining = value;
		}
	}

	public bool preferPrimaryCell { get; set; }

	public bool preferUnreservedCell { get; set; }

	public virtual float GetWorkTime()
	{
		return this.workTime;
	}

	public Worker GetWorker()
	{
		return this.worker;
	}

	public virtual float GetPercentComplete()
	{
		return (this.workTimeRemaining > this.workTime) ? (-1f) : (1f - this.workTimeRemaining / this.workTime);
	}

	public virtual Workable.AnimInfo GetAnim(Worker worker)
	{
		Workable.AnimInfo animInfo = default(Workable.AnimInfo);
		if (this.overrideAnims != null && this.overrideAnims.Length > 0)
		{
			animInfo.overrideAnims = this.overrideAnims;
		}
		animInfo.forcePlayPst = this.forcePlayPst;
		return animInfo;
	}

	public virtual HashedString[] GetWorkAnims(Worker worker)
	{
		return Workable.DefaultWorkAnims;
	}

	public virtual Vector3 GetWorkOffset()
	{
		return Vector3.zero;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.workerStatusItem = Db.Get().MiscStatusItems.Using;
		this.workingStatusItem = Db.Get().MiscStatusItems.Operating;
		this.statusItemData = this;
		this.workTime = this.GetWorkTime();
		this.workTimeRemaining = Mathf.Min(this.workTimeRemaining, this.workTime);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Components.Workables.Add(this);
	}

	public int GetCell()
	{
		return Grid.PosToCell(this);
	}

	public bool ShouldPreferPrimaryCell()
	{
		return this.preferPrimaryCell;
	}

	public bool ShouldPreferUnreservedCell()
	{
		return this.preferUnreservedCell;
	}

	public void StartWork(Worker workerToStart)
	{
		if (this.selectable != null && this.workingStatusItem != null)
		{
			this.selectable.AddStatusItem(this.workingStatusItem, this.statusItemData);
		}
		this.worker = workerToStart;
		this.ShowProgressBar(true);
		this.OnStartWork(this.worker);
	}

	public bool WorkTick(Worker worker, float dt)
	{
		bool flag = false;
		if (dt > 0f)
		{
			this.workTimeRemaining -= dt;
			flag = this.OnWorkTick(worker, dt);
		}
		return flag || this.workTimeRemaining < 0f;
	}

	public virtual float GetEfficiencyMultiplier(Worker worker)
	{
		if (this.attributeConverter != null)
		{
			AttributeConverterInstance converter = worker.GetComponent<AttributeConverters>().GetConverter(this.attributeConverter.Id);
			return Mathf.Max(1f + converter.Evaluate(), 0.1f);
		}
		return 1f;
	}

	public virtual Klei.AI.Attribute GetWorkAttribute()
	{
		if (this.attributeConverter != null)
		{
			return this.attributeConverter.attribute;
		}
		return null;
	}

	public virtual float GetExperienceMultiplier()
	{
		return 1f;
	}

	protected virtual bool OnWorkTick(Worker worker, float dt)
	{
		return false;
	}

	public void StopWork(Worker workerToStop)
	{
		if (this.selectable != null && this.workingStatusItem != null)
		{
			this.selectable.RemoveStatusItem(this.workingStatusItem, false);
		}
		if (this.worker != null)
		{
			this.OnAbortWork(this.worker);
		}
		this.OnStopWork(this.worker);
		this.worker = null;
	}

	public virtual StatusItem GetWorkerStatusItem()
	{
		return this.workerStatusItem;
	}

	public void CompleteWork(Worker worker)
	{
		if (this.selectable != null && this.workingStatusItem != null)
		{
			this.selectable.RemoveStatusItem(this.workingStatusItem, false);
		}
		this.OnCompleteWork(worker);
		this.OnStopWork(worker);
		this.workTimeRemaining = this.GetWorkTime();
		this.ShowProgressBar(false);
		this.worker = null;
	}

	protected virtual void OnStartWork(Worker worker)
	{
	}

	protected virtual void OnStopWork(Worker worker)
	{
	}

	protected virtual void OnCompleteWork(Worker worker)
	{
	}

	protected virtual void OnAbortWork(Worker worker)
	{
	}

	public void SetOffsets(CellOffset[] offsets)
	{
		this.offsetTracker.Clear();
		this.offsetTracker = new StandardOffsetTracker(offsets);
	}

	public void SetOffsetTable(CellOffset[][] offset_table)
	{
		if (this.offsetTracker != null)
		{
			this.offsetTracker.Clear();
		}
		this.offsetTracker = new OffsetTableTracker(offset_table, this);
	}

	public virtual CellOffset[] GetOffsets()
	{
		if (this.offsetTracker == null)
		{
			this.offsetTracker = new StandardOffsetTracker(Grid.DefaultOffset);
		}
		return this.offsetTracker.GetOffsets(Grid.PosToCell(this));
	}

	public void SetWorkTime(float work_time)
	{
		this.workTime = work_time;
		this.workTimeRemaining = work_time;
	}

	public bool ShouldFaceTargetWhenWorking()
	{
		return this.faceTargetWhenWorking;
	}

	protected virtual void CreateProgressBar()
	{
		if (this.progressBar != null)
		{
			return;
		}
		if (!this.showProgressBar)
		{
			return;
		}
		this.progressBar = Util.KInstantiateUI<ProgressBar>(ProgressBarsConfig.Instance.progressBarPrefab, null, false);
		this.progressBar.SetUpdateFunc(new Func<float>(this.GetPercentComplete));
		this.progressBar.transform.SetParent(GameScreenManager.Instance.worldSpaceCanvas.transform);
		this.progressBar.name = base.name + "." + base.GetType().Name + " ProgressBar";
		this.progressBar.transform.FindChild("Bar").GetComponent<Image>().color = ProgressBarsConfig.Instance.GetBarColor("ProgressBar");
		this.progressBar.Update();
		Building component = base.GetComponent<Building>();
		Vector3 vector = base.gameObject.transform.position + Vector3.down * this.progressbar_y_offset;
		if (component != null)
		{
			vector -= Vector3.right * 0.5f * (float)(component.Def.WidthInCells % 2);
		}
		else
		{
			vector -= Vector3.right * 0.5f;
		}
		this.progressBar.transform.SetPosition(vector);
	}

	public void ShowProgressBar(bool show)
	{
		if (show)
		{
			this.CreateProgressBar();
		}
		else if (this.progressBar != null)
		{
			this.progressBar.gameObject.DeleteObject();
			this.progressBar = null;
		}
	}

	protected override void OnCleanUp()
	{
		this.ShowProgressBar(false);
		Components.Workables.Remove(this);
		base.OnCleanUp();
	}

	public virtual Vector3 GetTargetPoint()
	{
		Vector3 vector = this.transform.position;
		float num = vector.y + 0.65f;
		BoxCollider2D component = base.GetComponent<BoxCollider2D>();
		if (component != null)
		{
			vector = component.bounds.center;
		}
		vector.y = num;
		vector.z = 0f;
		return vector;
	}

	public int GetNavigationCost(Navigator navigator)
	{
		int num = PathProber.InvalidCost;
		int num2 = Grid.PosToCell(this);
		foreach (CellOffset cellOffset in this.GetOffsets())
		{
			int num3 = Grid.OffsetCell(num2, cellOffset);
			if (Grid.IsValidCell(num3))
			{
				int navigationCost = navigator.GetNavigationCost(num3);
				if (navigationCost != PathProber.InvalidCost && (num == PathProber.InvalidCost || navigationCost < num))
				{
					num = navigationCost;
				}
			}
		}
		return num;
	}

	[ContextMenu("Refresh Reachability")]
	public void RefreshReachability()
	{
		if (this.offsetTracker != null)
		{
			this.offsetTracker.ForceRefresh();
		}
	}

	public float workTime;

	protected bool showProgressBar = true;

	protected float progressbar_y_offset = 0.45f;

	protected StatusItem workerStatusItem;

	protected StatusItem workingStatusItem;

	protected object statusItemData;

	protected OffsetTracker offsetTracker;

	protected AttributeConverter attributeConverter;

	protected bool forcePlayPst;

	[Serialize]
	[SerializeField]
	protected float workTimeRemaining = float.PositiveInfinity;

	[SerializeField]
	public KAnimFile[] overrideAnims;

	[MyCmpGet]
	protected KSelectable selectable;

	public int masterPriority = int.MaxValue;

	protected bool faceTargetWhenWorking;

	public global::System.Action onPriorityChanged;

	protected static readonly HashedString[] DefaultWorkAnims = new HashedString[] { "working_pre", "working_loop" };

	protected ProgressBar progressBar;

	public struct AnimInfo
	{
		public KAnimFile[] overrideAnims;

		public StateMachine.Instance smi;

		public bool forcePlayPst;
	}
}
