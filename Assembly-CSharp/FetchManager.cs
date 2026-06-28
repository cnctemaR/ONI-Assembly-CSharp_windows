using System;
using System.Collections.Generic;

public class FetchManager : KMonoBehaviour
{
	public static FetchManager Instance { get; private set; }

	public void Add(Pickupable pickupable)
	{
		this.pickupables.Add(pickupable);
	}

	public void Remove(Pickupable pickupable)
	{
		this.pickupables.Remove(pickupable);
	}

	protected override void OnPrefabInit()
	{
		FetchManager.Instance = this;
	}

	public void FindFetchTarget(Worker worker, Storage destination, TagBits tag_bits, TagBits required_tags, TagBits forbid_tags, float required_amount, ref Pickupable target)
	{
		FetchManagerUpdater.FindFetchTarget(worker, destination, this.pickupables, tag_bits, required_tags, forbid_tags, required_amount, ref target);
	}

	protected override void OnLoadLevel()
	{
		base.OnLoadLevel();
		FetchManagerUpdater.FreeResources();
		FetchManager.Instance = null;
	}

	public List<Pickupable> pickupables = new List<Pickupable>();
}
