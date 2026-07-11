using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/Refinery")]
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
