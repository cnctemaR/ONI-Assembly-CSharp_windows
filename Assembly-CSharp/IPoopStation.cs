using System;
using UnityEngine;

public interface IPoopStation
{
	bool IsPoopStationOperational();

	GameObject GetPoopStationObject();

	GameObject GetCurrentPoopStationUser();

	float GetAvailablePoopCapacity();

	string[] GetPoopingAnimNames();

	bool AttemptToReservePoopStation(GameObject user);

	void ClearPoopStationUser(GameObject user);

	void RegisterPoopStation();

	void UnregisterPoopStation();

	PoopData GetPoopData();

	bool IsUserCompatibleWithPoopStation(KPrefabID user);

	void PlayPoopStationAnim(string animName, KAnim.PlayMode playMode);
}
