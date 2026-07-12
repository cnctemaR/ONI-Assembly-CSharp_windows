using System;
using KSerialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/BudUprootedMonitor")]
public class BudUprootedMonitor : KMonoBehaviour
{
	public bool IsUprooted
	{
		get
		{
			return this.uprooted || base.GetComponent<KPrefabID>().HasTag(GameTags.Uprooted);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.Subscribe<BudUprootedMonitor>(-216549700, BudUprootedMonitor.OnUprootedDelegate);
	}

	public void SetParentObject(KPrefabID id)
	{
		this.parentObject = new Ref<KPrefabID>(id);
		base.Subscribe(id.gameObject, 1969584890, new Action<object>(this.OnLoseParent));
	}

	private void OnLoseParent(object obj)
	{
		if (!this.uprooted && !base.isNull)
		{
			base.GetComponent<KPrefabID>().AddTag(GameTags.Uprooted, false);
			this.uprooted = true;
			base.Trigger(-216549700, null);
			if (this.destroyOnParentLost)
			{
				Util.KDestroyGameObject(base.gameObject);
			}
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}

	public static bool IsObjectUprooted(GameObject plant)
	{
		BudUprootedMonitor component = plant.GetComponent<BudUprootedMonitor>();
		return !(component == null) && component.IsUprooted;
	}

	[Serialize]
	public bool canBeUprooted = true;

	[Serialize]
	private bool uprooted;

	public bool destroyOnParentLost;

	public Ref<KPrefabID> parentObject = new Ref<KPrefabID>();

	private HandleVector<int>.Handle partitionerEntry;

	private static readonly EventSystem.IntraObjectHandler<BudUprootedMonitor> OnUprootedDelegate = new EventSystem.IntraObjectHandler<BudUprootedMonitor>(delegate(BudUprootedMonitor component, object data)
	{
		if (!component.uprooted)
		{
			component.GetComponent<KPrefabID>().AddTag(GameTags.Uprooted, false);
			component.uprooted = true;
			component.Trigger(-216549700, null);
		}
	});
}
