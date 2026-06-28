using System;
using UnityEngine;

public class SpawnScreen : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		Util.KInstantiateUI(this.Screen, base.gameObject, false);
	}

	public GameObject Screen;
}
