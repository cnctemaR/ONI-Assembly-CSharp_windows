using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SpawnScreen")]
public class SpawnScreen : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		Util.KInstantiateUI(this.Screen, base.gameObject, false);
	}

	public GameObject Screen;
}
