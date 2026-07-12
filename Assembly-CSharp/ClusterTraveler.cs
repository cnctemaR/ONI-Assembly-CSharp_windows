using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

public class ClusterTraveler : KMonoBehaviour, ISim200ms
{
	public List<AxialI> CurrentPath
	{
		get
		{
			if (this.m_cachedPath == null || this.m_destinationSelector.GetDestination() != this.m_cachedPathDestination)
			{
				this.m_cachedPathDestination = this.m_destinationSelector.GetDestination();
				this.m_cachedPath = ClusterGrid.Instance.GetPath(this.m_clusterGridEntity.Location, this.m_cachedPathDestination, this.m_destinationSelector);
			}
			return this.m_cachedPath;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Components.ClusterTravelers.Add(this);
	}

	protected override void OnCleanUp()
	{
		Components.ClusterTravelers.Remove(this);
		Game.Instance.Unsubscribe(-1991583975, new Action<object>(this.OnClusterFogOfWarRevealed));
		base.OnCleanUp();
	}

	private void ForceRevealLocation(AxialI location)
	{
		if (!ClusterGrid.Instance.IsCellVisible(location))
		{
			SaveGame.Instance.GetSMI<ClusterFogOfWarManager.Instance>().RevealLocation(location, 0);
		}
	}

	protected override void OnSpawn()
	{
		base.Subscribe<ClusterTraveler>(543433792, ClusterTraveler.ClusterDestinationChangedHandler);
		Game.Instance.Subscribe(-1991583975, new Action<object>(this.OnClusterFogOfWarRevealed));
		this.UpdateAnimationTags();
		this.MarkPathDirty();
		this.RevalidatePath(false);
		this.ForceRevealLocation(this.m_clusterGridEntity.Location);
	}

	private void MarkPathDirty()
	{
		this.m_isPathDirty = true;
	}

	private void OnClusterFogOfWarRevealed(object data)
	{
		this.MarkPathDirty();
	}

	private void OnClusterDestinationChanged(object data)
	{
		if (this.m_destinationSelector.IsAtDestination())
		{
			this.m_movePotential = 0f;
			if (this.CurrentPath != null)
			{
				this.CurrentPath.Clear();
			}
		}
		this.MarkPathDirty();
	}

	public int GetDestinationWorldID()
	{
		return this.m_destinationSelector.GetDestinationWorld();
	}

	public float TravelETA()
	{
		if (!this.IsTraveling() || this.getSpeedCB == null)
		{
			return 0f;
		}
		return this.RemainingTravelDistance() / this.getSpeedCB();
	}

	public float RemainingTravelDistance()
	{
		int num = this.RemainingTravelNodes();
		if (this.GetDestinationWorldID() >= 0)
		{
			num--;
			num = Mathf.Max(num, 0);
		}
		return (float)num * 600f - this.m_movePotential;
	}

	public int RemainingTravelNodes()
	{
		int count = this.CurrentPath.Count;
		return Mathf.Max(0, count);
	}

	public float GetMoveProgress()
	{
		return this.m_movePotential / 600f;
	}

	public bool IsTraveling()
	{
		return !this.m_destinationSelector.IsAtDestination();
	}

	public void Sim200ms(float dt)
	{
		if (!this.IsTraveling())
		{
			return;
		}
		bool flag = this.CurrentPath != null && this.CurrentPath.Count > 0;
		bool flag2 = this.m_destinationSelector.HasAsteroidDestination();
		bool flag3 = flag2 && flag && this.CurrentPath.Count == 1;
		if (this.getCanTravelCB != null && !this.getCanTravelCB(flag3))
		{
			return;
		}
		AxialI location = this.m_clusterGridEntity.Location;
		if (flag)
		{
			if (flag2)
			{
				bool requireLaunchPadOnAsteroidDestination = this.m_destinationSelector.requireLaunchPadOnAsteroidDestination;
			}
			if (!flag2 || this.CurrentPath.Count > 1)
			{
				float num = dt * this.getSpeedCB();
				this.m_movePotential += num;
				if (this.m_movePotential >= 600f)
				{
					this.m_movePotential = 600f;
					if (this.AdvancePathOneStep())
					{
						global::Debug.Assert(ClusterGrid.Instance.GetVisibleEntityOfLayerAtCell(this.m_clusterGridEntity.Location, EntityLayer.Asteroid) == null, string.Format("Somehow this clustercraft pathed through an asteroid at {0}", this.m_clusterGridEntity.Location));
						this.m_movePotential -= 600f;
						if (this.onTravelCB != null)
						{
							this.onTravelCB();
						}
					}
				}
			}
			else
			{
				this.AdvancePathOneStep();
			}
		}
		this.RevalidatePath(true);
	}

	public bool AdvancePathOneStep()
	{
		if (this.validateTravelCB != null && !this.validateTravelCB(this.CurrentPath[0]))
		{
			return false;
		}
		AxialI axialI = this.CurrentPath[0];
		this.CurrentPath.RemoveAt(0);
		this.ForceRevealLocation(axialI);
		this.m_clusterGridEntity.Location = axialI;
		this.UpdateAnimationTags();
		return true;
	}

	private void UpdateAnimationTags()
	{
		if (this.CurrentPath == null)
		{
			this.m_clusterGridEntity.RemoveTag(GameTags.BallisticEntityLaunching);
			this.m_clusterGridEntity.RemoveTag(GameTags.BallisticEntityLanding);
			this.m_clusterGridEntity.RemoveTag(GameTags.BallisticEntityMoving);
			return;
		}
		if (!(ClusterGrid.Instance.GetAsteroidAtCell(this.m_clusterGridEntity.Location) != null))
		{
			this.m_clusterGridEntity.AddTag(GameTags.BallisticEntityMoving);
			this.m_clusterGridEntity.RemoveTag(GameTags.BallisticEntityLanding);
			this.m_clusterGridEntity.RemoveTag(GameTags.BallisticEntityLaunching);
			return;
		}
		if (this.CurrentPath.Count == 0 || this.m_clusterGridEntity.Location == this.CurrentPath[this.CurrentPath.Count - 1])
		{
			this.m_clusterGridEntity.AddTag(GameTags.BallisticEntityLanding);
			this.m_clusterGridEntity.RemoveTag(GameTags.BallisticEntityLaunching);
			this.m_clusterGridEntity.RemoveTag(GameTags.BallisticEntityMoving);
			return;
		}
		this.m_clusterGridEntity.AddTag(GameTags.BallisticEntityLaunching);
		this.m_clusterGridEntity.RemoveTag(GameTags.BallisticEntityLanding);
		this.m_clusterGridEntity.RemoveTag(GameTags.BallisticEntityMoving);
	}

	public void RevalidatePath(bool react_to_change = true)
	{
		string reason;
		List<AxialI> list;
		if (this.HasCurrentPathChanged(out reason, out list))
		{
			if (this.stopAndNotifyWhenPathChanges && react_to_change)
			{
				this.m_destinationSelector.SetDestination(this.m_destinationSelector.GetMyWorldLocation());
				string message = MISC.NOTIFICATIONS.BADROCKETPATH.TOOLTIP;
				Notification notification = new Notification(MISC.NOTIFICATIONS.BADROCKETPATH.NAME, NotificationType.BadMinor, (List<Notification> notificationList, object data) => message + notificationList.ReduceMessages(false) + "\n\n" + reason, null, true, 0f, null, null, null, true);
				base.GetComponent<Notifier>().Add(notification, "");
				return;
			}
			this.m_cachedPath = list;
		}
	}

	private bool HasCurrentPathChanged(out string reason, out List<AxialI> updatedPath)
	{
		if (!this.m_isPathDirty)
		{
			reason = null;
			updatedPath = null;
			return false;
		}
		this.m_isPathDirty = false;
		updatedPath = ClusterGrid.Instance.GetPath(this.m_clusterGridEntity.Location, this.m_cachedPathDestination, this.m_destinationSelector, out reason);
		if (updatedPath == null)
		{
			return true;
		}
		if (updatedPath.Count != this.m_cachedPath.Count)
		{
			return true;
		}
		for (int i = 0; i < this.m_cachedPath.Count; i++)
		{
			if (this.m_cachedPath[i] != updatedPath[i])
			{
				return true;
			}
		}
		return false;
	}

	[ContextMenu("Fill Move Potential")]
	public void FillMovePotential()
	{
		this.m_movePotential = 600f;
	}

	[MyCmpReq]
	private ClusterDestinationSelector m_destinationSelector;

	[MyCmpReq]
	private ClusterGridEntity m_clusterGridEntity;

	[Serialize]
	private float m_movePotential;

	public Func<float> getSpeedCB;

	public Func<bool, bool> getCanTravelCB;

	public Func<AxialI, bool> validateTravelCB;

	public global::System.Action onTravelCB;

	private AxialI m_cachedPathDestination;

	private List<AxialI> m_cachedPath;

	private bool m_isPathDirty;

	public bool stopAndNotifyWhenPathChanges;

	private static EventSystem.IntraObjectHandler<ClusterTraveler> ClusterDestinationChangedHandler = new EventSystem.IntraObjectHandler<ClusterTraveler>(delegate(ClusterTraveler cmp, object data)
	{
		cmp.OnClusterDestinationChanged(data);
	});
}
