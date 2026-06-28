using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class EggIncubator : KMonoBehaviour, ISaveLoadable
{
	public Pickupable egg
	{
		get
		{
			return this.eggRef.Get();
		}
		set
		{
			this.eggRef.Set(value);
		}
	}

	protected override void OnSpawn()
	{
		this.CreateFetchChore();
	}

	private void CreateFetchChore()
	{
		if (this.fetchChore == null && this.egg == null)
		{
			Action<Chore> action = new Action<Chore>(this.OnFetchComplete);
			this.fetchChore = new FetchChore(this.storage, 1f, new Tag[] { GameTags.Egg }, null, null, true, action, null, null, FetchOrder2.OperationalRequirement.Operational, 0);
		}
	}

	private void OnFetchComplete(Chore chore)
	{
		this.egg = this.fetchChore.fetchTarget;
		this.egg.gameObject.layer = 1;
		this.egg.transform.SetLocalPosition(this.eggPositionPoint);
		this.fetchChore = null;
		this.SetOperation();
		this.Subscribe(-592767678, delegate
		{
			this.SetOperation();
		});
	}

	private void SetOperation()
	{
		Operational component = base.GetComponent<Operational>();
		if (component.IsOperational && this.egg != null)
		{
			component.SetActive(true, false);
		}
		else
		{
			component.SetActive(false, false);
		}
	}

	public void RemoveHatchedEgg(GameObject eggObject)
	{
		this.storage.Remove(eggObject);
		this.fetchChore = null;
		this.egg.gameObject.layer = 3;
		KBatchedAnimController component = this.egg.GetComponent<KBatchedAnimController>();
		component.enabled = false;
		component.enabled = true;
		this.egg = null;
		this.CreateFetchChore();
		this.SetOperation();
	}

	[MyCmpReq]
	private Storage storage;

	[MyCmpReq]
	public Operational operational;

	[Serialize]
	private Ref<Pickupable> eggRef = new Ref<Pickupable>();

	private FetchChore fetchChore;

	private Vector3 eggPositionPoint = new Vector3(-0.05f, 0.32f, 1f);
}
