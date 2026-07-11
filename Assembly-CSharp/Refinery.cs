using System;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class Refinery : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	[Serializable]
	public struct OrderSaveData
	{
		public OrderSaveData(string id, bool infinite)
		{
			this.id = id;
			this.infinite = infinite;
		}

		public string id;

		public bool infinite;
	}
}
