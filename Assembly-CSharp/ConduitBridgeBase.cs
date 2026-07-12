using System;

public class ConduitBridgeBase : KMonoBehaviour
{
	protected void SendEmptyOnMassTransfer()
	{
		if (this.OnMassTransfer != null)
		{
			this.OnMassTransfer(SimHashes.Void, 0f, 0f, 0, 0, null);
		}
	}

	public ConduitBridgeBase.DesiredMassTransfer desiredMassTransfer;

	public ConduitBridgeBase.ConduitBridgeEvent OnMassTransfer;

	public delegate float DesiredMassTransfer(float dt, SimHashes element, float mass, float temperature, byte disease_idx, int disease_count, Pickupable pickupable);

	public delegate void ConduitBridgeEvent(SimHashes element, float mass, float temperature, byte disease_idx, int disease_count, Pickupable pickupable);
}
