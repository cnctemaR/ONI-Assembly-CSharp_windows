using System;
using UnityEngine;

public class HelmetController : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe<HelmetController>(-1617557748, HelmetController.OnEquippedDelegate);
		base.Subscribe<HelmetController>(-170173755, HelmetController.OnUnequippedDelegate);
	}

	private KBatchedAnimController GetAssigneeController()
	{
		KBatchedAnimController kbatchedAnimController = null;
		Equippable component = base.GetComponent<Equippable>();
		if (component.assignee != null)
		{
			Transform transform = component.assignee.GetSoleOwner().transform;
			kbatchedAnimController = transform.GetComponent<KBatchedAnimController>();
		}
		return kbatchedAnimController;
	}

	private void OnEquipped(object data)
	{
		Equippable component = base.GetComponent<Equippable>();
		this.ShowHelmet();
		Ownables soleOwner = component.assignee.GetSoleOwner();
		soleOwner.Subscribe(961737054, new Action<object>(this.OnBeginRecoverBreath));
		soleOwner.Subscribe(-2037519664, new Action<object>(this.OnEndRecoverBreath));
		soleOwner.Subscribe(1347184327, new Action<object>(this.OnPathAdvanced));
		this.in_tube = false;
		this.is_flying = false;
		this.owner_navigator = soleOwner.GetComponent<Navigator>();
	}

	private void OnUnequipped(object data)
	{
		this.owner_navigator = null;
		Equippable component = base.GetComponent<Equippable>();
		if (component != null)
		{
			this.HideHelmet();
			IAssignableIdentity assignee = component.assignee;
			if (assignee != null)
			{
				Ownables soleOwner = assignee.GetSoleOwner();
				soleOwner.Unsubscribe(961737054, new Action<object>(this.OnBeginRecoverBreath));
				soleOwner.Unsubscribe(-2037519664, new Action<object>(this.OnEndRecoverBreath));
				soleOwner.Unsubscribe(1347184327, new Action<object>(this.OnPathAdvanced));
			}
		}
	}

	private void ShowHelmet()
	{
		KBatchedAnimController assigneeController = this.GetAssigneeController();
		if (assigneeController == null)
		{
			return;
		}
		KAnimFile anim = Assets.GetAnim(this.anim_file);
		KAnimHashedString kanimHashedString = new KAnimHashedString("snapTo_neck");
		assigneeController.GetComponent<SymbolOverrideController>().AddSymbolOverride(kanimHashedString, anim.GetData().build.GetSymbol(kanimHashedString), 6);
		assigneeController.SetSymbolVisiblity(kanimHashedString, true);
		this.is_shown = true;
		this.UpdateJets();
	}

	private void HideHelmet()
	{
		this.is_shown = false;
		KBatchedAnimController assigneeController = this.GetAssigneeController();
		if (assigneeController == null)
		{
			return;
		}
		KAnimHashedString kanimHashedString = "snapTo_neck";
		SymbolOverrideController component = assigneeController.GetComponent<SymbolOverrideController>();
		if (component == null)
		{
			return;
		}
		component.RemoveSymbolOverride(kanimHashedString, 6);
		assigneeController.SetSymbolVisiblity(kanimHashedString, false);
		this.UpdateJets();
	}

	private void UpdateJets()
	{
		if (this.is_shown && this.is_flying)
		{
			this.EnableJets();
		}
		else
		{
			this.DisableJets();
		}
	}

	private void EnableJets()
	{
		if (!this.has_jets)
		{
			return;
		}
		if (this.jet_go)
		{
			return;
		}
		this.jet_go = this.AddTrackedAnim("jet", Assets.GetAnim("jetsuit_thruster_fx_kanim"), "loop", Grid.SceneLayer.Creatures, "snapTo_neck");
		this.glow_go = this.AddTrackedAnim("glow", Assets.GetAnim("jetsuit_thruster_glow_fx_kanim"), "loop", Grid.SceneLayer.Front, "snapTo_neck");
	}

	private void DisableJets()
	{
		if (!this.has_jets)
		{
			return;
		}
		global::UnityEngine.Object.Destroy(this.jet_go);
		this.jet_go = null;
		global::UnityEngine.Object.Destroy(this.glow_go);
		this.glow_go = null;
	}

	private GameObject AddTrackedAnim(string name, KAnimFile anim_file, string anim_clip, Grid.SceneLayer layer, string symbol_name)
	{
		KBatchedAnimController assigneeController = this.GetAssigneeController();
		if (assigneeController == null)
		{
			return null;
		}
		string text = assigneeController.name + "." + name;
		GameObject gameObject = new GameObject(text);
		gameObject.SetActive(false);
		gameObject.transform.parent = assigneeController.transform;
		KPrefabID kprefabID = gameObject.AddComponent<KPrefabID>();
		kprefabID.PrefabTag = new Tag(text);
		KBatchedAnimController kbatchedAnimController = gameObject.AddComponent<KBatchedAnimController>();
		kbatchedAnimController.AnimFiles = new KAnimFile[] { anim_file };
		kbatchedAnimController.initialAnim = anim_clip;
		kbatchedAnimController.isMovable = true;
		kbatchedAnimController.sceneLayer = layer;
		KBatchedAnimTracker kbatchedAnimTracker = gameObject.AddComponent<KBatchedAnimTracker>();
		kbatchedAnimTracker.symbol = symbol_name;
		bool flag;
		Vector4 column = assigneeController.GetSymbolTransform(symbol_name, out flag).GetColumn(3);
		Vector3 vector = column;
		vector.z = Grid.GetLayerZ(layer);
		gameObject.transform.SetPosition(vector);
		gameObject.SetActive(true);
		kbatchedAnimController.Play(anim_clip, KAnim.PlayMode.Loop, 1f, 0f);
		return gameObject;
	}

	private void OnBeginRecoverBreath(object data)
	{
		this.HideHelmet();
	}

	private void OnEndRecoverBreath(object data)
	{
		this.ShowHelmet();
	}

	private void OnPathAdvanced(object data)
	{
		if (this.owner_navigator == null)
		{
			return;
		}
		bool flag = this.owner_navigator.CurrentNavType == NavType.Hover;
		bool flag2 = this.owner_navigator.CurrentNavType == NavType.Tube;
		if (flag2 != this.in_tube)
		{
			this.in_tube = flag2;
			if (this.in_tube)
			{
				this.HideHelmet();
			}
			else
			{
				this.ShowHelmet();
			}
		}
		if (flag != this.is_flying)
		{
			this.is_flying = flag;
			this.UpdateJets();
		}
	}

	public string anim_file = "helm_oxygen_kanim";

	public bool has_jets;

	private bool is_shown;

	private bool in_tube;

	private bool is_flying;

	private Navigator owner_navigator;

	private GameObject jet_go;

	private GameObject glow_go;

	private static readonly EventSystem.IntraObjectHandler<HelmetController> OnEquippedDelegate = new EventSystem.IntraObjectHandler<HelmetController>(delegate(HelmetController component, object data)
	{
		component.OnEquipped(data);
	});

	private static readonly EventSystem.IntraObjectHandler<HelmetController> OnUnequippedDelegate = new EventSystem.IntraObjectHandler<HelmetController>(delegate(HelmetController component, object data)
	{
		component.OnUnequipped(data);
	});
}
