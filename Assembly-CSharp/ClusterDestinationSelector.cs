using System;
using KSerialization;

public class ClusterDestinationSelector : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.Subscribe<ClusterDestinationSelector>(-1298331547, this.OnClusterLocationChangedDelegate);
	}

	protected virtual void OnClusterLocationChanged(object data)
	{
		if (((ClusterLocationChangedEvent)data).newLocation == this.m_destination)
		{
			base.Trigger(1796608350, data);
		}
	}

	public int GetDestinationWorld()
	{
		return ClusterUtil.GetAsteroidWorldIdAtLocation(this.m_destination);
	}

	public AxialI GetDestination()
	{
		return this.m_destination;
	}

	public virtual void SetDestination(AxialI location)
	{
		if (this.requireAsteroidDestination)
		{
			Debug.Assert(ClusterUtil.GetAsteroidWorldIdAtLocation(location) != -1, string.Format("Cannot SetDestination to {0} as there is no world there", location));
		}
		this.m_destination = location;
		base.Trigger(543433792, location);
	}

	public bool HasAsteroidDestination()
	{
		return ClusterUtil.GetAsteroidWorldIdAtLocation(this.m_destination) != -1;
	}

	public virtual bool IsAtDestination()
	{
		return this.GetMyWorldLocation() == this.m_destination;
	}

	[Serialize]
	protected AxialI m_destination;

	public bool assignable;

	public bool requireAsteroidDestination;

	[Serialize]
	public bool canNavigateFogOfWar;

	public bool dodgesHiddenAsteroids;

	public bool requireLaunchPadOnAsteroidDestination;

	public bool shouldPointTowardsPath;

	private EventSystem.IntraObjectHandler<ClusterDestinationSelector> OnClusterLocationChangedDelegate = new EventSystem.IntraObjectHandler<ClusterDestinationSelector>(delegate(ClusterDestinationSelector cmp, object data)
	{
		cmp.OnClusterLocationChanged(data);
	});
}
