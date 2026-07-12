using System;
using System.Collections.Generic;
using STRINGS;

public class RailgunPayloadClusterGridEntity : ClusterGridEntity
{
	public override string Name
	{
		get
		{
			return ITEMS.RAILGUNPAYLOAD.NAME;
		}
	}

	public override EntityLayer Layer
	{
		get
		{
			return EntityLayer.Payload;
		}
	}

	public override List<ClusterGridEntity.AnimConfig> AnimConfigs
	{
		get
		{
			return new List<ClusterGridEntity.AnimConfig>
			{
				new ClusterGridEntity.AnimConfig
				{
					animFile = Assets.GetAnim("payload01_kanim"),
					initialAnim = "idle_loop"
				}
			};
		}
	}

	public override bool IsVisible
	{
		get
		{
			return this.m_clusterTraveler.IsTraveling();
		}
	}

	public override ClusterRevealLevel IsVisibleInFOW
	{
		get
		{
			return ClusterRevealLevel.Hidden;
		}
	}

	public override bool SpaceOutInSameHex()
	{
		return true;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.m_clusterTraveler.getSpeedCB = new Func<float>(this.GetSpeed);
		this.m_clusterTraveler.getCanTravelCB = new Func<bool, bool>(this.CanTravel);
		this.m_clusterTraveler.onTravelCB = null;
	}

	private float GetSpeed()
	{
		return 10f;
	}

	private bool CanTravel(bool tryingToLand)
	{
		return this.HasTag(GameTags.EntityInSpace);
	}

	public void Configure(AxialI source, AxialI destination)
	{
		this.m_location = source;
		this.m_destionationSelector.SetDestination(destination);
	}

	public override bool ShowPath()
	{
		return this.m_selectable.IsSelected;
	}

	public override bool ShowProgressBar()
	{
		return this.m_selectable.IsSelected && this.m_clusterTraveler.IsTraveling();
	}

	public override float GetProgress()
	{
		return this.m_clusterTraveler.GetMoveProgress();
	}

	[MyCmpReq]
	private ClusterDestinationSelector m_destionationSelector;

	[MyCmpReq]
	private KSelectable m_selectable;

	[MyCmpReq]
	private ClusterTraveler m_clusterTraveler;

	public bool NoWaitInOrbit;
}
