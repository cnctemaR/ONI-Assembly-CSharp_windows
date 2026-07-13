using System;
using KSerialization;
using UnityEngine;

public class EntityClusterDestinationSelector : ClusterDestinationSelector
{
	private ClusterGridEntity DestinationEntity
	{
		get
		{
			if (this.m_DestinationEntity == null)
			{
				return null;
			}
			return this.m_DestinationEntity.Get();
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		global::Debug.Assert(this.requiredEntityLayer != EntityLayer.None, "EnityClusterDestinationSelector must specify an EntityLayer");
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.Subscribe(-905833192, new Action<object>(this.OnCopySettings));
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		if (gameObject != null)
		{
			EntityClusterDestinationSelector component = gameObject.GetComponent<EntityClusterDestinationSelector>();
			if (component != null && component.DestinationEntity != null)
			{
				this.m_DestinationEntity = new Ref<ClusterGridEntity>(component.DestinationEntity);
				this.SetDestination(this.m_DestinationEntity.Get().Location);
			}
		}
	}

	public override ClusterGridEntity GetClusterEntityTarget()
	{
		return this.DestinationEntity;
	}

	public override AxialI GetDestination()
	{
		if (this.DestinationEntity != null)
		{
			return this.DestinationEntity.Location;
		}
		return base.GetDestination();
	}

	public override void SetDestination(AxialI location)
	{
		ClusterGridEntity visibleEntityOfLayerAtCell = ClusterGrid.Instance.GetVisibleEntityOfLayerAtCell(location, this.requiredEntityLayer);
		this.m_DestinationEntity.Set(visibleEntityOfLayerAtCell);
		base.SetDestination(location);
	}

	[Serialize]
	protected Ref<ClusterGridEntity> m_DestinationEntity = new Ref<ClusterGridEntity>();
}
