using System;
using UnityEngine;

public class CellSelectionInstantiator : MonoBehaviour
{
	private void Awake()
	{
		GameObject gameObject = Util.KInstantiate(this.CellSelectionPrefab, null, "WorldSelectionCollider");
		GameObject gameObject2 = Util.KInstantiate(this.CellSelectionPrefab, null, "WorldSelectionCollider");
		CellSelectionObject component = gameObject.GetComponent<CellSelectionObject>();
		CellSelectionObject component2 = gameObject2.GetComponent<CellSelectionObject>();
		component.alternateSelectionObject = component2;
		component2.alternateSelectionObject = component;
		CellSelectionInstantiator.CreateBackwallSelectionProxy();
	}

	private static void CreateBackwallSelectionProxy()
	{
		GameObject gameObject = new GameObject("BackwallSelectionCollider");
		gameObject.SetActive(false);
		gameObject.AddComponent<BackwallSelectionObject>();
		gameObject.AddComponent<KSelectable>().DisableSelectMarker = true;
		gameObject.AddComponent<KBoxCollider2D>();
		gameObject.SetActive(true);
	}

	public GameObject CellSelectionPrefab;
}
