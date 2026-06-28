using System;
using UnityEngine;

public class HelmetController : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe(-1617557748, new Action<object>(this.OnEquipped));
		base.Subscribe(-170173755, new Action<object>(this.OnUnequipped));
	}

	private void OnEquipped(object data)
	{
		string text = "helmet_name";
		Equippable component = base.GetComponent<Equippable>();
		Grid.SceneLayer sceneLayer = Grid.SceneLayer.Move;
		this.helmet = new GameObject(text);
		this.helmet.SetActive(false);
		KPrefabID kprefabID = this.helmet.AddComponent<KPrefabID>();
		PrimaryElement primaryElement = this.helmet.AddComponent<PrimaryElement>();
		primaryElement.ElementID = component.GetComponent<PrimaryElement>().ElementID;
		primaryElement.Temperature = component.GetComponent<PrimaryElement>().Temperature;
		kprefabID.PrefabTag = GameTags.Helmet;
		HashedString hashedString = new HashedString("snapto_neck");
		this.helmet.transform.parent = component.assignee.GetSoleOwner().transform;
		this.helmet.transform.localPosition = new Vector3(0f, 0f, Grid.GetLayerZ(sceneLayer));
		KBatchedAnimController kbatchedAnimController = this.helmet.AddComponent<KBatchedAnimController>();
		kbatchedAnimController.SetAnims(new KAnimFile[] { Assets.GetAnim("body_comp_default_kanim") }, true);
		KAnimFile anim = Assets.GetAnim("helm_oxygen_kanim");
		KAnim.Build.Symbol symbol = anim.GetData().build.symbols[0];
		foreach (KAnim.Build.Symbol symbol2 in anim.GetData().build.symbols)
		{
			if (symbol2.hash.HashValue == hashedString.HashValue)
			{
				symbol = symbol2;
				break;
			}
		}
		kbatchedAnimController.AddSymbolOverride(hashedString, anim.batchTag, symbol, false);
		kbatchedAnimController.ShowSymbol(hashedString);
		kbatchedAnimController.isMovable = true;
		kbatchedAnimController.sceneLayer = sceneLayer;
		kbatchedAnimController.Play("ah", KAnim.PlayMode.Once, 1f, 0f);
		primaryElement.ForcePermanentDiseaseContainer(true);
		primaryElement.SetDiseaseVisualProvider(component.gameObject);
		KBatchedAnimTracker kbatchedAnimTracker = this.helmet.AddComponent<KBatchedAnimTracker>();
		kbatchedAnimTracker.symbol = new HashedString("snapTo_headshape");
		kbatchedAnimTracker.offset = new Vector3(0f, 0f, 0f);
		this.helmet.SetActive(true);
		component.assignee.GetSoleOwner().transform.GetComponent<KMonoBehaviour>().Subscribe(961737054, new Action<object>(this.OnBeginRecoverBreath));
		component.assignee.GetSoleOwner().transform.GetComponent<KMonoBehaviour>().Subscribe(-2037519664, new Action<object>(this.OnEndRecoverBreath));
	}

	private void OnUnequipped(object data)
	{
		Equippable component = base.GetComponent<Equippable>();
		if (this.helmet != null)
		{
			this.helmet.DeleteObject();
		}
		if (component != null)
		{
			component.assignee.GetSoleOwner().transform.GetComponent<KMonoBehaviour>().Unsubscribe(961737054, new Action<object>(this.OnBeginRecoverBreath));
			component.assignee.GetSoleOwner().transform.GetComponent<KMonoBehaviour>().Unsubscribe(-2037519664, new Action<object>(this.OnEndRecoverBreath));
		}
	}

	private void OnBeginRecoverBreath(object data)
	{
		if (this.helmet != null)
		{
			this.helmet.SetActive(false);
		}
	}

	private void OnEndRecoverBreath(object data)
	{
		if (this.helmet != null)
		{
			this.helmet.SetActive(true);
		}
	}

	public GameObject helmet;
}
