using System;
using KSerialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Baggable")]
public class Baggable : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.minionAnimOverride = Assets.GetAnim("anim_restrain_creature_kanim");
		Pickupable pickupable = base.gameObject.AddOrGet<Pickupable>();
		pickupable.workAnims = new HashedString[]
		{
			new HashedString("capture"),
			new HashedString("pickup")
		};
		pickupable.workAnimPlayMode = KAnim.PlayMode.Once;
		pickupable.workingPstComplete = null;
		pickupable.workingPstFailed = null;
		pickupable.overrideAnims = new KAnimFile[] { this.minionAnimOverride };
		pickupable.trackOnPickup = false;
		pickupable.useGunforPickup = this.useGunForPickup;
		pickupable.synchronizeAnims = false;
		pickupable.SetWorkTime(3f);
		if (this.mustStandOntopOfTrapForPickup)
		{
			pickupable.SetOffsets(new CellOffset[]
			{
				default(CellOffset),
				new CellOffset(0, -1)
			});
		}
		base.Subscribe<Baggable>(856640610, Baggable.OnStoreDelegate);
		if (base.transform.parent != null)
		{
			if (base.transform.parent.GetComponent<Trap>() != null)
			{
				base.GetComponent<KBatchedAnimController>().enabled = true;
			}
			if (base.transform.parent.GetComponent<EggIncubator>() != null)
			{
				this.wrangled = true;
			}
		}
		if (this.wrangled)
		{
			this.SetWrangled();
		}
	}

	private void OnStore(object data)
	{
		Storage storage = data as Storage;
		if (storage != null || (data != null && (bool)data))
		{
			base.gameObject.AddTag(GameTags.Creatures.Bagged);
			if (storage && storage.IsPrefabID(GameTags.Minion))
			{
				this.SetVisible(false);
				return;
			}
		}
		else
		{
			this.Free();
		}
	}

	private void SetVisible(bool visible)
	{
		KAnimControllerBase component = base.gameObject.GetComponent<KAnimControllerBase>();
		if (component != null && component.enabled != visible)
		{
			component.enabled = visible;
		}
		KSelectable component2 = base.gameObject.GetComponent<KSelectable>();
		if (component2 != null && component2.enabled != visible)
		{
			component2.enabled = visible;
		}
	}

	public void SetWrangled()
	{
		this.wrangled = true;
		Navigator component = base.GetComponent<Navigator>();
		if (component && component.IsValidNavType(NavType.Floor))
		{
			component.SetCurrentNavType(NavType.Floor);
		}
		base.gameObject.AddTag(GameTags.Creatures.Bagged);
		base.GetComponent<KAnimControllerBase>().Play("trussed", KAnim.PlayMode.Loop, 1f, 0f);
	}

	public void Free()
	{
		base.gameObject.RemoveTag(GameTags.Creatures.Bagged);
		this.wrangled = false;
		this.SetVisible(true);
	}

	[SerializeField]
	private KAnimFile minionAnimOverride;

	public bool mustStandOntopOfTrapForPickup;

	[Serialize]
	public bool wrangled;

	public bool useGunForPickup;

	private static readonly EventSystem.IntraObjectHandler<Baggable> OnStoreDelegate = new EventSystem.IntraObjectHandler<Baggable>(delegate(Baggable component, object data)
	{
		component.OnStore(data);
	});
}
