using System;
using System.Collections.Generic;
using KSerialization;
using TUNING;
using UnityEngine;

public class ClusterFogOfWarManager : GameStateMachine<ClusterFogOfWarManager, ClusterFogOfWarManager.Instance, IStateMachineTarget, ClusterFogOfWarManager.Def>
{
	public override void InitializeStates(out StateMachine.BaseState defaultState)
	{
		defaultState = this.root;
		this.root.Enter(delegate(ClusterFogOfWarManager.Instance smi)
		{
			smi.Initialize();
		}).EventHandler(GameHashes.DiscoveredWorldsChanged, (ClusterFogOfWarManager.Instance smi) => Game.Instance, delegate(ClusterFogOfWarManager.Instance smi)
		{
			smi.UpdateRevealedCellsFromDiscoveredWorlds();
		});
	}

	public const int AUTOMATIC_PEEK_RADIUS = 2;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<ClusterFogOfWarManager, ClusterFogOfWarManager.Instance, IStateMachineTarget, ClusterFogOfWarManager.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, ClusterFogOfWarManager.Def def)
			: base(master, def)
		{
		}

		public void Initialize()
		{
			this.UpdateRevealedCellsFromDiscoveredWorlds();
			this.EnsureRevealedTilesHavePeek();
		}

		public ClusterRevealLevel GetCellRevealLevel(AxialI location)
		{
			if (this.GetRevealCompleteFraction(location) >= 1f)
			{
				return ClusterRevealLevel.Visible;
			}
			if (this.GetRevealCompleteFraction(location) > 0f)
			{
				return ClusterRevealLevel.Peeked;
			}
			return ClusterRevealLevel.Hidden;
		}

		public void DEBUG_REVEAL_ENTIRE_MAP()
		{
			this.RevealLocation(AxialI.ZERO, 100, 2);
		}

		public bool IsLocationRevealed(AxialI location)
		{
			return ClusterGrid.Instance.IsValidCell(location) && this.GetRevealCompleteFraction(location) >= 1f;
		}

		private void EnsureRevealedTilesHavePeek()
		{
			foreach (KeyValuePair<AxialI, List<ClusterGridEntity>> keyValuePair in ClusterGrid.Instance.cellContents)
			{
				if (this.IsLocationRevealed(keyValuePair.Key))
				{
					this.PeekLocation(keyValuePair.Key, 2);
				}
			}
		}

		public void PeekLocation(AxialI location, int radius)
		{
			foreach (AxialI axialI in AxialUtil.GetAllPointsWithinRadius(location, radius))
			{
				if (this.m_revealPointsByCell.ContainsKey(axialI))
				{
					this.m_revealPointsByCell[axialI] = Mathf.Max(this.m_revealPointsByCell[axialI], 0.01f);
				}
				else
				{
					this.m_revealPointsByCell[axialI] = 0.01f;
				}
			}
		}

		public void RevealLocation(AxialI location, int radius = 0, int peekRadius = 2)
		{
			if (ClusterGrid.Instance.GetHiddenEntitiesOfLayerAtCell(location, EntityLayer.Asteroid).Count > 0 || ClusterGrid.Instance.GetVisibleEntityOfLayerAtCell(location, EntityLayer.Asteroid) != null)
			{
				radius = Mathf.Max(radius, 1);
			}
			bool flag = false;
			foreach (AxialI axialI in AxialUtil.GetAllPointsWithinRadius(location, radius))
			{
				flag |= this.RevealCellIfValid(axialI, peekRadius);
			}
			if (flag)
			{
				Game.Instance.BoxingTrigger<AxialI>(-1991583975, location);
			}
		}

		public bool EarnRevealPointsForLocation(AxialI location, float points)
		{
			global::Debug.Assert(ClusterGrid.Instance.IsValidCell(location), string.Format("EarnRevealPointsForLocation called with invalid location: {0}", location));
			if (this.IsLocationRevealed(location))
			{
				return false;
			}
			if (this.m_revealPointsByCell.ContainsKey(location))
			{
				Dictionary<AxialI, float> revealPointsByCell = this.m_revealPointsByCell;
				revealPointsByCell[location] += points;
			}
			else
			{
				this.m_revealPointsByCell[location] = points;
				Game.Instance.BoxingTrigger<AxialI>(-1554423969, location);
			}
			if (this.IsLocationRevealed(location))
			{
				this.RevealLocation(location, 0, 2);
				this.PeekLocation(location, 2);
				Game.Instance.BoxingTrigger<AxialI>(-1991583975, location);
				return true;
			}
			return false;
		}

		public float GetRevealCompleteFraction(AxialI location)
		{
			if (!ClusterGrid.Instance.IsValidCell(location))
			{
				global::Debug.LogError(string.Format("GetRevealCompleteFraction called with invalid location: {0}, {1}", location.r, location.q));
			}
			if (DebugHandler.RevealFogOfWar)
			{
				return 1f;
			}
			float num;
			if (this.m_revealPointsByCell.TryGetValue(location, out num))
			{
				return Mathf.Min(num / ROCKETRY.CLUSTER_FOW.POINTS_TO_REVEAL, 1f);
			}
			return 0f;
		}

		private bool RevealCellIfValid(AxialI cell, int peekRadius = 2)
		{
			if (!ClusterGrid.Instance.IsValidCell(cell))
			{
				return false;
			}
			if (this.IsLocationRevealed(cell))
			{
				return false;
			}
			this.m_revealPointsByCell[cell] = ROCKETRY.CLUSTER_FOW.POINTS_TO_REVEAL;
			this.PeekLocation(cell, peekRadius);
			return true;
		}

		public bool GetUnrevealedLocationWithinRadius(AxialI center, int radius, out AxialI result)
		{
			for (int i = 0; i <= radius; i++)
			{
				foreach (AxialI axialI in AxialUtil.GetRing(center, i))
				{
					if (ClusterGrid.Instance.IsValidCell(axialI) && !this.IsLocationRevealed(axialI))
					{
						result = axialI;
						return true;
					}
				}
			}
			result = AxialI.ZERO;
			return false;
		}

		public void UpdateRevealedCellsFromDiscoveredWorlds()
		{
			int num = (DlcManager.IsExpansion1Active() ? 0 : 2);
			foreach (WorldContainer worldContainer in ClusterManager.Instance.WorldContainers)
			{
				if (worldContainer.IsDiscovered && !DebugHandler.RevealFogOfWar)
				{
					this.RevealLocation(worldContainer.GetComponent<ClusterGridEntity>().Location, num, 2);
				}
			}
		}

		[Serialize]
		private Dictionary<AxialI, float> m_revealPointsByCell = new Dictionary<AxialI, float>();
	}
}
