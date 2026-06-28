using System;
using UnityEngine;

public class CellSelectionInstantiator : MonoBehaviour
{
	private void Awake()
	{
		GameObject gameObject = Util.KInstantiate(this.CellSelectionPrefab, SceneOrganizer.Instance.GetFolder(Folder.Misc), "WorldSelectionCollider");
		GameObject gameObject2 = Util.KInstantiate(this.CellSelectionPrefab, SceneOrganizer.Instance.GetFolder(Folder.Misc), "WorldSelectionCollider");
		CellSelectionObject component = gameObject.GetComponent<CellSelectionObject>();
		CellSelectionObject component2 = gameObject2.GetComponent<CellSelectionObject>();
		component.alternateSelectionObject = component2;
		component2.alternateSelectionObject = component;
	}

	public GameObject CellSelectionPrefab;
}
