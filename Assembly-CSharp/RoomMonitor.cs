using System;

public class RoomMonitor : GameStateMachine<RoomMonitor, RoomMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.root;
		this.root.EventHandler(GameHashes.PathAdvanced, new StateMachine<RoomMonitor, RoomMonitor.Instance, IStateMachineTarget, object>.State.Callback(RoomMonitor.UpdateRoomType));
	}

	private static void UpdateRoomType(RoomMonitor.Instance smi)
	{
		Room roomOfGameObject = Game.Instance.roomProber.GetRoomOfGameObject(smi.master.gameObject);
		if (roomOfGameObject != smi.currentRoom)
		{
			smi.currentRoom = roomOfGameObject;
			if (roomOfGameObject != null)
			{
				roomOfGameObject.cavity.OnEnter(smi.master.gameObject);
			}
		}
	}

	public new class Instance : GameStateMachine<RoomMonitor, RoomMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public Room currentRoom;
	}
}
