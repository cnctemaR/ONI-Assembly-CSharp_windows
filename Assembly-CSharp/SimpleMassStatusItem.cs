using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SimpleMassStatusItem")]
public class SimpleMassStatusItem : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.GetComponent<KSelectable>().AddStatusItem(Db.Get().MiscStatusItems.OreMass, base.gameObject);
	}

	public string symbolPrefix = "";
}
