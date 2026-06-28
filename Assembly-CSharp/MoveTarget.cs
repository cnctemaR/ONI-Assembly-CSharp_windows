using System;
using UnityEngine;

public class MoveTarget : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.transform.parent = SceneOrganizer.Instance.GetFolder(Folder.MoveTarget).transform;
		base.gameObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector | HideFlags.DontSaveInEditor | HideFlags.NotEditable | HideFlags.DontSaveInBuild | HideFlags.DontUnloadUnusedAsset;
	}
}
