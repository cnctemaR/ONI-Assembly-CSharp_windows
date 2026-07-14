using System;
using UnityEngine;

public class UnderwaterShearingStaion : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.storage = base.GetComponent<Storage>();
		this.SetupShearableSymbol();
	}

	public void UpdateShearableSymbol(Tag item_tag)
	{
		this.symbolController.gameObject.SetActive(true);
		GameObject prefab = Assets.GetPrefab(item_tag);
		this.symbolController.SwapAnims(prefab.GetComponent<KBatchedAnimController>().AnimFiles);
		this.symbolController.Play("idle1", KAnim.PlayMode.Loop, 1f, 0f);
	}

	public void HideShearableSymbol()
	{
		this.symbolController.gameObject.SetActive(false);
	}

	public void SetupShearableSymbol()
	{
		KBatchedAnimController component = base.gameObject.GetComponent<KBatchedAnimController>();
		KBatchedAnimController[] componentsInChildren = base.gameObject.GetComponentsInChildren<KBatchedAnimController>(true);
		GameObject gameObject = Util.NewGameObject(base.gameObject, base.gameObject.name + ".ore_symbol");
		gameObject.SetActive(false);
		bool flag;
		Vector3 vector = component.GetSymbolTransform(UnderwaterShearingStaion.SYMBOL_HASH, out flag).GetColumn(3);
		vector.z = component.transform.GetPosition().z - 0.05f;
		gameObject.transform.SetPosition(vector);
		this.symbolController = gameObject.AddComponent<KBatchedAnimController>();
		this.symbolController.AnimFiles = new KAnimFile[] { Assets.GetAnim("hematite_kanim") };
		this.symbolController.initialAnim = "idle1";
		component.SetSymbolVisiblity(UnderwaterShearingStaion.SYMBOL_HASH, false);
		KBatchedAnimController[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetSymbolVisiblity(UnderwaterShearingStaion.SYMBOL_HASH, false);
		}
		KBatchedAnimTracker kbatchedAnimTracker = gameObject.AddComponent<KBatchedAnimTracker>();
		kbatchedAnimTracker.symbol = UnderwaterShearingStaion.SYMBOL_HASH;
		kbatchedAnimTracker.offset = Vector3.zero;
	}

	private KBatchedAnimController symbolController;

	private static HashedString SYMBOL_HASH = "object";

	private Storage storage;
}
