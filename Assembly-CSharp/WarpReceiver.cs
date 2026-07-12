using System;
using System.Linq;
using KSerialization;

public class WarpReceiver : Workable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.warpReceiverSMI = new WarpReceiver.WarpReceiverSM.Instance(this);
		this.warpReceiverSMI.StartSM();
		Components.WarpReceivers.Add(this);
	}

	public void ReceiveWarpedDuplicant(Worker dupe)
	{
		dupe.transform.SetPosition(Grid.CellToPos(Grid.PosToCell(this), CellAlignment.Bottom, Grid.SceneLayer.Move));
		Debug.Assert(this.chore == null);
		KAnimFile anim = Assets.GetAnim("anim_interacts_warp_portal_receiver_kanim");
		ChoreType migrate = Db.Get().ChoreTypes.Migrate;
		KAnimFile kanimFile = anim;
		this.chore = new WorkChore<Workable>(migrate, this, dupe.GetComponent<ChoreProvider>(), true, delegate(Chore o)
		{
			this.CompleteChore();
		}, null, null, true, null, true, true, kanimFile, false, true, false, PriorityScreen.PriorityClass.compulsory, 5, false, true);
		Workable component = base.GetComponent<Workable>();
		component.workLayer = Grid.SceneLayer.Building;
		component.workAnims = new HashedString[] { "printing_pre", "printing_loop" };
		component.workingPstComplete = new HashedString[] { "printing_pst" };
		component.workingPstFailed = new HashedString[] { "printing_pst" };
		component.synchronizeAnims = true;
		float num = 0f;
		KAnimFileData data = anim.GetData();
		for (int i = 0; i < data.animCount; i++)
		{
			KAnim.Anim anim2 = data.GetAnim(i);
			if (component.workAnims.Contains(anim2.hash))
			{
				num += anim2.totalTime;
			}
		}
		component.SetWorkTime(num);
		this.Used = true;
	}

	private void CompleteChore()
	{
		this.chore.Cleanup();
		this.chore = null;
		this.warpReceiverSMI.GoTo(this.warpReceiverSMI.sm.idle);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.WarpReceivers.Remove(this);
	}

	[MyCmpAdd]
	public Notifier notifier;

	private WarpReceiver.WarpReceiverSM.Instance warpReceiverSMI;

	private Notification notification;

	[Serialize]
	public bool IsConsumed;

	private Chore chore;

	[Serialize]
	public bool Used;

	public class WarpReceiverSM : GameStateMachine<WarpReceiver.WarpReceiverSM, WarpReceiver.WarpReceiverSM.Instance, WarpReceiver>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.idle;
			this.idle.PlayAnim("idle");
		}

		public GameStateMachine<WarpReceiver.WarpReceiverSM, WarpReceiver.WarpReceiverSM.Instance, WarpReceiver, object>.State idle;

		public new class Instance : GameStateMachine<WarpReceiver.WarpReceiverSM, WarpReceiver.WarpReceiverSM.Instance, WarpReceiver, object>.GameInstance
		{
			public Instance(WarpReceiver master)
				: base(master)
			{
			}
		}
	}
}
