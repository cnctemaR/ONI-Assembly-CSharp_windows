using System;
using Klei.AI;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ParkSign")]
public class ParkSign : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe<ParkSign>(-832141045, ParkSign.TriggerRoomEffectsDelegate);
	}

	private void TriggerRoomEffects(object data)
	{
		GameObject gameObject = (GameObject)data;
		Room roomOfGameObject = Game.Instance.roomProber.GetRoomOfGameObject(base.gameObject);
		if (roomOfGameObject != null)
		{
			roomOfGameObject.roomType.TriggerRoomEffects(base.gameObject.GetComponent<KPrefabID>(), gameObject.GetComponent<Effects>());
		}
	}

	private static readonly EventSystem.IntraObjectHandler<ParkSign> TriggerRoomEffectsDelegate = new EventSystem.IntraObjectHandler<ParkSign>(delegate(ParkSign component, object data)
	{
		component.TriggerRoomEffects(data);
	});
}
