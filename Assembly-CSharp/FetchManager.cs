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

	public PathFinderFlags FindFetchTarget(Worker worker, Storage destination, Tag[] tags, Tag[] required_tags, Tag[] forbid_tags, float required_amount, ref Pickupable target)
	{
		return FetchManagerUpdater.FindFetchTarget(worker, destination, this.pickupables, tags, required_tags, forbid_tags, required_amount, ref target);
	}

	public void Clear()
	{
		this.pickupables.Clear();
	}

	public List<Pickupable> pickupables = new List<Pickupable>();
}
