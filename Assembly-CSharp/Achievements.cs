using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Achievements")]
public class Achievements : KMonoBehaviour
{
	public void Unlock(string id)
	{
		if (SteamAchievementService.Instance)
		{
			SteamAchievementService.Instance.Unlock(id);
		}
	}
}
