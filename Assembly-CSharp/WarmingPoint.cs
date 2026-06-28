using System;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class WarmingPoint : BuildingWorkable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.Subscribe(-1643076535, new Action<object>(this.OnRotated));
		this.actualOffsets = new CellOffset[this.offsets.Length];
		this.UpdateOffsets();
		this.workTime = float.PositiveInfinity;
		this.workTimeRemaining = float.PositiveInfinity;
		this.workerStatusItem = Db.Get().DuplicantStatusItems.WarmingUp;
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_use_machine_kanim") };
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.CreateTask();
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		return !worker.GetComponent<ChoreConsumer>().HasUrge(Db.Get().Urges.WarmUp);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		this.CreateTask();
	}

	private void CreateTask()
	{
		this.chore = new WorkChore<WarmingPoint>(Db.Get().ChoreTypes.Warmup, this, null, true, null, null, null, true, null, true, default(Tag), null, false, true);
	}

	public override CellOffset[] GetOffsets()
	{
		return this.actualOffsets;
	}

	private void OnRotated(object data)
	{
		this.UpdateOffsets();
	}

	private void UpdateOffsets()
	{
		Rotatable component = base.GetComponent<Rotatable>();
		for (int i = 0; i < this.offsets.Length; i++)
		{
			if (component != null)
			{
				this.actualOffsets[i] = component.GetRotatedCellOffset(this.offsets[i]);
			}
			else
			{
				this.actualOffsets[i] = this.offsets[i];
			}
		}
	}

	protected override void OnCleanUp()
	{
		this.chore.Cancel("Building destroyed");
		this.chore = null;
	}

	public CellOffset[] offsets;

	private CellOffset[] actualOffsets;

	private Chore chore;
}
