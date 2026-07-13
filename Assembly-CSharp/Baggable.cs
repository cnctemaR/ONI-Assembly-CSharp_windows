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
			if (base.transform.parent.GetComponent<Trap>() != null || base.transform.parent.GetSMI<ReusableTrap.Instance>() != null)
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
		if (storage != null || (data != null && ((Boxed<bool>)data).value))
		{
			base.gameObject.AddTag(GameTags.Creatures.Bagged);
			if (storage && storage.HasTag(GameTags.BaseMinion))
			{
				this.SetVisible(false);
				return;
			}
		}
		else
		{
			if (!this.keepWrangledNextTimeRemovedFromStorage)
			{
				this.Free();
			}
			this.keepWrangledNextTimeRemovedFromStorage = false;
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

	public static string GetBaggedAnimName(GameObject baggableObject)
	{
		string text = "trussed";
		Pickupable pickupable = baggableObject.AddOrGet<Pickupable>();
		if (pickupable != null && pickupable.storage != null)
		{
			IBaggedStateAnimationInstructions component = pickupable.storage.GetComponent<IBaggedStateAnimationInstructions>();
			if (component != null)
			{
				string baggedAnimationName = component.GetBaggedAnimationName();
				if (baggedAnimationName != null)
				{
					text = baggedAnimationName;
				}
			}
		}
		return text;
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
		base.GetComponent<KAnimControllerBase>().Play(Baggable.GetBaggedAnimName(base.gameObject), KAnim.PlayMode.Loop, 1f, 0f);
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

	[Serialize]
	public bool keepWrangledNextTimeRemovedFromStorage;

	public bool useGunForPickup;

	private static readonly EventSystem.IntraObjectHandler<Baggable> OnStoreDelegate = new EventSystem.IntraObjectHandler<Baggable>(delegate(Baggable component, object data)
	{
		component.OnStore(data);
	});

	public const string DEFAULT_BAGGED_ANIM_NAME = "trussed";
}
