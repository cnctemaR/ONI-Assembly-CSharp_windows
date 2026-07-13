using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/HarvestablePOIConfigurator")]
public class HarvestablePOIConfigurator : KMonoBehaviour
{
	public static HarvestablePOIConfigurator.HarvestablePOIType FindType(HashedString typeId)
	{
		HarvestablePOIConfigurator.HarvestablePOIType harvestablePOIType = null;
		if (typeId != HashedString.Invalid)
		{
			harvestablePOIType = HarvestablePOIConfigurator._poiTypes.Find((HarvestablePOIConfigurator.HarvestablePOIType t) => t.id == typeId);
		}
		if (harvestablePOIType == null)
		{
			global::Debug.LogError(string.Format("Tried finding a harvestable poi with id {0} but it doesn't exist!", typeId.ToString()));
		}
		return harvestablePOIType;
	}

	public HarvestablePOIConfigurator.HarvestablePOIInstanceConfiguration MakeConfiguration()
	{
		return this.CreateRandomInstance(this.presetType, this.presetMin, this.presetMax);
	}

	private HarvestablePOIConfigurator.HarvestablePOIInstanceConfiguration CreateRandomInstance(HashedString typeId, float min, float max)
	{
		int globalWorldSeed = SaveLoader.Instance.clusterDetailSave.globalWorldSeed;
		ClusterGridEntity component = base.GetComponent<ClusterGridEntity>();
		Vector3 position = ClusterGrid.Instance.GetPosition(component);
		KRandom krandom = new KRandom(globalWorldSeed + (int)position.x + (int)position.y);
		return new HarvestablePOIConfigurator.HarvestablePOIInstanceConfiguration
		{
			typeId = typeId,
			capacityRoll = this.Roll(krandom, min, max),
			rechargeRoll = this.Roll(krandom, min, max)
		};
	}

	private float Roll(KRandom randomSource, float min, float max)
	{
		return (float)(randomSource.NextDouble() * (double)(max - min)) + min;
	}

	private static List<HarvestablePOIConfigurator.HarvestablePOIType> _poiTypes;

	public HashedString presetType;

	public float presetMin;

	public float presetMax = 1f;

	public class HarvestablePOIType : IHasDlcRestrictions
	{
		public string[] GetRequiredDlcIds()
		{
			return this.requiredDlcIds;
		}

		public string[] GetForbiddenDlcIds()
		{
			return this.forbiddenDlcIds;
		}

		public HarvestablePOIType(string id, Dictionary<SimHashes, float> harvestableElements, float poiCapacityMin = 54000f, float poiCapacityMax = 81000f, float poiRechargeMin = 30000f, float poiRechargeMax = 60000f, bool canProvideArtifacts = true, List<string> orbitalObject = null, int maxNumOrbitingObjects = 20, string[] requiredDlcIds = null, string[] forbiddenDlcIds = null)
			: this(id, harvestableElements, 0, null, poiCapacityMin, poiCapacityMax, poiRechargeMin, poiRechargeMax, canProvideArtifacts, orbitalObject, maxNumOrbitingObjects, requiredDlcIds, forbiddenDlcIds)
		{
		}

		public HarvestablePOIType(string id, Dictionary<SimHashes, float> harvestableElements, int initialDatabanks, Dictionary<SimHashes, float> initialLiberatedResources = null, float poiCapacityMin = 54000f, float poiCapacityMax = 81000f, float poiRechargeMin = 30000f, float poiRechargeMax = 60000f, bool canProvideArtifacts = true, List<string> orbitalObject = null, int maxNumOrbitingObjects = 20, string[] requiredDlcIds = null, string[] forbiddenDlcIds = null)
		{
			this.id = id;
			this.idHash = id;
			this.harvestableElements = harvestableElements;
			this.initialDataBanks = initialDatabanks;
			this.initialLiberatedResources = initialLiberatedResources;
			this.poiCapacityMin = poiCapacityMin;
			this.poiCapacityMax = poiCapacityMax;
			this.poiRechargeMin = poiRechargeMin;
			this.poiRechargeMax = poiRechargeMax;
			this.canProvideArtifacts = canProvideArtifacts;
			this.orbitalObject = orbitalObject;
			this.maxNumOrbitingObjects = maxNumOrbitingObjects;
			this.requiredDlcIds = requiredDlcIds;
			this.forbiddenDlcIds = forbiddenDlcIds;
			if (HarvestablePOIConfigurator._poiTypes == null)
			{
				HarvestablePOIConfigurator._poiTypes = new List<HarvestablePOIConfigurator.HarvestablePOIType>();
			}
			HarvestablePOIConfigurator._poiTypes.Add(this);
		}

		[Obsolete]
		public HarvestablePOIType(string id, Dictionary<SimHashes, float> harvestableElements, float poiCapacityMin = 54000f, float poiCapacityMax = 81000f, float poiRechargeMin = 30000f, float poiRechargeMax = 60000f, bool canProvideArtifacts = true, List<string> orbitalObject = null, int maxNumOrbitingObjects = 20, string dlcID = "EXPANSION1_ID")
			: this(id, harvestableElements, poiCapacityMin, poiCapacityMax, poiRechargeMin, poiRechargeMax, canProvideArtifacts, orbitalObject, maxNumOrbitingObjects, null, null)
		{
			this.requiredDlcIds = DlcManager.EXPANSION1;
		}

		public string id;

		public HashedString idHash;

		public Dictionary<SimHashes, float> harvestableElements;

		public float poiCapacityMin;

		public float poiCapacityMax;

		public float poiRechargeMin;

		public float poiRechargeMax;

		public int initialDataBanks;

		public Dictionary<SimHashes, float> initialLiberatedResources;

		public bool canProvideArtifacts;

		[Obsolete]
		public string dlcID;

		public string[] requiredDlcIds;

		public string[] forbiddenDlcIds;

		public List<string> orbitalObject;

		public int maxNumOrbitingObjects;
	}

	[Serializable]
	public class HarvestablePOIInstanceConfiguration
	{
		private void Init()
		{
			if (this.didInit)
			{
				return;
			}
			this.didInit = true;
			this.poiTotalCapacity = MathUtil.ReRange(this.capacityRoll, 0f, 1f, this.poiType.poiCapacityMin, this.poiType.poiCapacityMax);
			this.poiRecharge = MathUtil.ReRange(this.rechargeRoll, 0f, 1f, this.poiType.poiRechargeMin, this.poiType.poiRechargeMax);
		}

		public HarvestablePOIConfigurator.HarvestablePOIType poiType
		{
			get
			{
				return HarvestablePOIConfigurator.FindType(this.typeId);
			}
		}

		public Dictionary<SimHashes, float> GetElementsWithWeights()
		{
			this.Init();
			return this.poiType.harvestableElements;
		}

		public bool CanProvideArtifacts()
		{
			this.Init();
			return this.poiType.canProvideArtifacts;
		}

		public float GetMaxCapacity()
		{
			this.Init();
			return this.poiTotalCapacity;
		}

		public float GetRechargeTime()
		{
			this.Init();
			return this.poiRecharge;
		}

		public HashedString typeId;

		private bool didInit;

		public float capacityRoll;

		public float rechargeRoll;

		private float poiTotalCapacity;

		private float poiRecharge;
	}
}
