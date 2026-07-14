using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShelfDisplay : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.shelfItems = new KBatchedAnimController[4];
		base.Subscribe(-1697596308, new Action<object>(this.OnStorageChange));
		this.OnStorageChange(null);
	}

	private KBatchedAnimController CreateShelfItem(int index, KAnimFile animFile, bool show)
	{
		if (animFile == null)
		{
			global::Debug.LogWarning("Mini-fridge: shelf item anim file is null");
			return null;
		}
		KBatchedAnimController kbatchedAnimController = this.shelfItems[index];
		if (kbatchedAnimController != null)
		{
			kbatchedAnimController.enabled = show;
			if (show)
			{
				kbatchedAnimController.SwapAnims(new KAnimFile[] { animFile });
			}
			return kbatchedAnimController;
		}
		Vector3 position = base.transform.position;
		GameObject gameObject = new GameObject("Fridge shelf display");
		gameObject.SetActive(false);
		KBatchedAnimController kbatchedAnimController2 = gameObject.AddComponent<KBatchedAnimController>();
		kbatchedAnimController2.AnimFiles = new KAnimFile[] { animFile };
		kbatchedAnimController2.initialAnim = "ui";
		kbatchedAnimController2.sceneLayer = Grid.SceneLayer.Building;
		kbatchedAnimController2.animScale *= 0.4f;
		kbatchedAnimController2.enabled = show;
		gameObject.transform.parent = base.gameObject.transform;
		gameObject.transform.position = position + ShelfDisplay.offsets[index];
		gameObject.SetActive(true);
		this.shelfItems[index] = kbatchedAnimController2;
		return kbatchedAnimController2;
	}

	private void OnStorageChange(object obj)
	{
		List<GameObject> items = this.storage.GetItems();
		items.OrderBy<GameObject, float>((GameObject i) => i.GetComponent<PrimaryElement>().Mass);
		for (int j = 0; j < 4; j++)
		{
			if (items.Count > j)
			{
				GameObject gameObject = items[j];
				this.ShowItem(j, gameObject);
			}
			else
			{
				this.HideItem(j);
			}
		}
	}

	private void HideItem(int index)
	{
		if (this.shelfItems[index] != null)
		{
			this.shelfItems[index].enabled = false;
		}
	}

	private void ShowItem(int index, GameObject item)
	{
		KBatchedAnimController kbatchedAnimController;
		if (item.TryGetComponent<KBatchedAnimController>(out kbatchedAnimController))
		{
			if (kbatchedAnimController.AnimFiles == null || kbatchedAnimController.AnimFiles.Length == 0)
			{
				global::Debug.LogWarning(string.Format("ShelfDisplay: anim files null or empty {0}", item.PrefabID()));
				this.HideItem(index);
				return;
			}
			this.CreateShelfItem(index, kbatchedAnimController.AnimFiles[0], true);
		}
	}

	[MyCmpReq]
	private SymbolOverrideController symbolOverrideController;

	[MyCmpReq]
	private KBatchedAnimController kbac;

	[MyCmpReq]
	private Storage storage;

	private KBatchedAnimController[] shelfItems;

	public static Vector3[] offsets = new Vector3[]
	{
		new Vector3(-0.1f, 0.1f, 0.1f),
		new Vector3(-0.1f, 0.4f, 0.2f),
		new Vector3(0.2f, 0.1f, 0.05f),
		new Vector3(0.2f, 0.4f, 0.15f)
	};
}
