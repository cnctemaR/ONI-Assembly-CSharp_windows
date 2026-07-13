using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Klei;
using Klei.AI;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/PrimaryElement")]
public class PrimaryElement : KMonoBehaviour, ISaveLoadable
{
	public void SetUseSimDiseaseInfo(bool use)
	{
		this.useSimDiseaseInfo = use;
	}

	[Serialize]
	public float Units
	{
		get
		{
			return this._units;
		}
		set
		{
			if (float.IsInfinity(value) || float.IsNaN(value))
			{
				DebugUtil.DevLogError("Invalid units value for element, setting Units to 0");
				this._units = 0f;
			}
			else
			{
				this._units = value;
			}
			if (this.onDataChanged != null)
			{
				this.onDataChanged(this);
			}
		}
	}

	public float Temperature
	{
		get
		{
			return this.getTemperatureCallback(this);
		}
		set
		{
			this.SetTemperature(value);
		}
	}

	public float InternalTemperature
	{
		get
		{
			return this._Temperature;
		}
		set
		{
			this._Temperature = value;
		}
	}

	[OnSerializing]
	private void OnSerializing()
	{
		this._Temperature = this.Temperature;
		this.SanitizeMassAndTemperature();
		this.diseaseID.HashValue = 0;
		this.diseaseCount = 0;
		if (this.useSimDiseaseInfo)
		{
			int num = Grid.PosToCell(base.transform.GetPosition());
			if (Grid.DiseaseIdx[num] != 255)
			{
				this.diseaseID = Db.Get().Diseases[(int)Grid.DiseaseIdx[num]].id;
				this.diseaseCount = Grid.DiseaseCount[num];
				return;
			}
		}
		else if (this.diseaseHandle.IsValid())
		{
			DiseaseHeader header = GameComps.DiseaseContainers.GetHeader(this.diseaseHandle);
			if (header.diseaseIdx != 255)
			{
				this.diseaseID = Db.Get().Diseases[(int)header.diseaseIdx].id;
				this.diseaseCount = header.diseaseCount;
			}
		}
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		if (this.ElementID == (SimHashes)351109216)
		{
			this.ElementID = SimHashes.Creature;
		}
		this.SanitizeMassAndTemperature();
		float num = this._Temperature;
		if (float.IsNaN(num) || float.IsInfinity(num) || num < 0f || 10000f < num)
		{
			DeserializeWarnings.Instance.PrimaryElementTemperatureIsNan.Warn(string.Format("{0} has invalid temperature of {1}. Resetting temperature.", base.name, this.Temperature), null);
			num = this.Element.defaultValues.temperature;
		}
		this._Temperature = num;
		this.Temperature = num;
		if (this.Element == null)
		{
			DeserializeWarnings.Instance.PrimaryElementHasNoElement.Warn(base.name + "Primary element has no element.", null);
		}
		if (this.Mass < 0f)
		{
			DebugUtil.DevLogError(base.gameObject, "deserialized ore with less than 0 mass. Error! Destroying");
			Util.KDestroyGameObject(base.gameObject);
			return;
		}
		if (this.Mass == 0f && !this.KeepZeroMassObject)
		{
			DebugUtil.DevLogError(base.gameObject, "deserialized element with 0 mass. Destroying");
			Util.KDestroyGameObject(base.gameObject);
			return;
		}
		if (this.onDataChanged != null)
		{
			this.onDataChanged(this);
		}
		byte index = Db.Get().Diseases.GetIndex(this.diseaseID);
		if (index == 255 || this.diseaseCount <= 0)
		{
			if (this.diseaseHandle.IsValid())
			{
				GameComps.DiseaseContainers.Remove(base.gameObject);
				this.diseaseHandle.Clear();
				return;
			}
		}
		else
		{
			if (this.diseaseHandle.IsValid())
			{
				DiseaseHeader header = GameComps.DiseaseContainers.GetHeader(this.diseaseHandle);
				header.diseaseIdx = index;
				header.diseaseCount = this.diseaseCount;
				GameComps.DiseaseContainers.SetHeader(this.diseaseHandle, header);
				return;
			}
			this.diseaseHandle = GameComps.DiseaseContainers.Add(base.gameObject, index, this.diseaseCount);
		}
	}

	protected override void OnLoadLevel()
	{
		base.OnLoadLevel();
	}

	private void SanitizeMassAndTemperature()
	{
		if (this._Temperature <= 0f)
		{
			DebugUtil.DevLogError(base.gameObject.name + " is attempting to serialize a temperature of <= 0K. Resetting to default. world=" + base.gameObject.DebugGetMyWorldName());
			this._Temperature = this.Element.defaultValues.temperature;
		}
		if (this.Mass > PrimaryElement.MAX_MASS)
		{
			DebugUtil.DevLogError(string.Format("{0} is attempting to serialize very large mass {1}. Resetting to default. world={2}", base.gameObject.name, this.Mass, base.gameObject.DebugGetMyWorldName()));
			this.Mass = this.Element.defaultValues.mass;
		}
	}

	public float Mass
	{
		get
		{
			return this.Units * this.MassPerUnit;
		}
		set
		{
			this.SetMass(value);
			if (this.onDataChanged != null)
			{
				this.onDataChanged(this);
			}
		}
	}

	private void SetMass(float mass)
	{
		if ((mass > PrimaryElement.MAX_MASS || mass < 0f) && this.ElementID != SimHashes.Regolith)
		{
			DebugUtil.DevLogErrorFormat(base.gameObject, "{0} is getting an abnormal mass set {1}.", new object[]
			{
				base.gameObject.name,
				mass
			});
		}
		mass = Mathf.Clamp(mass, 0f, PrimaryElement.MAX_MASS);
		this.Units = mass / this.MassPerUnit;
		if (this.Units <= 0f && !this.KeepZeroMassObject)
		{
			Util.KDestroyGameObject(base.gameObject);
		}
	}

	private void SetTemperature(float temperature)
	{
		if (float.IsNaN(temperature) || float.IsInfinity(temperature))
		{
			DebugUtil.LogErrorArgs(base.gameObject, new object[] { "Invalid temperature [" + temperature.ToString() + "]" });
			return;
		}
		if (temperature <= 0f)
		{
			KCrashReporter.Assert(false, "Tried to set PrimaryElement.Temperature to a value <= 0", null);
		}
		this.setTemperatureCallback(this, temperature);
	}

	public void SetMassTemperature(float mass, float temperature)
	{
		this.SetMass(mass);
		this.SetTemperature(temperature);
	}

	public Element Element
	{
		get
		{
			if (this._Element == null)
			{
				this._Element = ElementLoader.FindElementByHash(this.ElementID);
			}
			return this._Element;
		}
	}

	public byte DiseaseIdx
	{
		get
		{
			if (this.diseaseRedirectTarget)
			{
				return this.diseaseRedirectTarget.DiseaseIdx;
			}
			byte b = byte.MaxValue;
			if (this.useSimDiseaseInfo)
			{
				int num = Grid.PosToCell(base.transform.GetPosition());
				b = Grid.DiseaseIdx[num];
			}
			else if (this.diseaseHandle.IsValid())
			{
				b = GameComps.DiseaseContainers.GetHeader(this.diseaseHandle).diseaseIdx;
			}
			return b;
		}
	}

	public int DiseaseCount
	{
		get
		{
			if (this.diseaseRedirectTarget)
			{
				return this.diseaseRedirectTarget.DiseaseCount;
			}
			int num = 0;
			if (this.useSimDiseaseInfo)
			{
				int num2 = Grid.PosToCell(base.transform.GetPosition());
				num = Grid.DiseaseCount[num2];
			}
			else if (this.diseaseHandle.IsValid())
			{
				num = GameComps.DiseaseContainers.GetHeader(this.diseaseHandle).diseaseCount;
			}
			return num;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		GameComps.InfraredVisualizers.Add(base.gameObject);
		base.Subscribe<PrimaryElement>(1335436905, PrimaryElement.OnSplitFromChunkDelegate);
		base.Subscribe<PrimaryElement>(-2064133523, PrimaryElement.OnAbsorbDelegate);
	}

	protected override void OnSpawn()
	{
		Attributes attributes = this.GetAttributes();
		if (attributes != null)
		{
			foreach (AttributeModifier attributeModifier in this.Element.attributeModifiers)
			{
				attributes.Add(attributeModifier);
			}
		}
	}

	public void ForcePermanentDiseaseContainer(bool force_on)
	{
		if (force_on)
		{
			if (!this.diseaseHandle.IsValid())
			{
				this.diseaseHandle = GameComps.DiseaseContainers.Add(base.gameObject, byte.MaxValue, 0);
			}
		}
		else if (this.diseaseHandle.IsValid() && this.DiseaseIdx == 255)
		{
			GameComps.DiseaseContainers.Remove(base.gameObject);
			this.diseaseHandle.Clear();
		}
		this.forcePermanentDiseaseContainer = force_on;
	}

	protected override void OnCleanUp()
	{
		GameComps.InfraredVisualizers.Remove(base.gameObject);
		if (this.diseaseHandle.IsValid())
		{
			GameComps.DiseaseContainers.Remove(base.gameObject);
			this.diseaseHandle.Clear();
		}
		base.OnCleanUp();
	}

	public void SetElement(SimHashes element_id, bool addTags = true)
	{
		this.ElementID = element_id;
		if (addTags)
		{
			this.UpdateTags();
		}
	}

	public void UpdateTags()
	{
		if (this.ElementID == (SimHashes)0)
		{
			global::Debug.Log("UpdateTags() Primary element 0", base.gameObject);
			return;
		}
		KPrefabID component = base.GetComponent<KPrefabID>();
		if (component != null)
		{
			List<Tag> list = new List<Tag>();
			foreach (Tag tag in this.Element.oreTags)
			{
				list.Add(tag);
			}
			if (component.HasAnyTags(PrimaryElement.metalTags))
			{
				list.Add(GameTags.StoredMetal);
			}
			foreach (Tag tag2 in list)
			{
				component.AddTag(tag2, false);
			}
		}
	}

	public void ModifyDiseaseCount(int delta, string reason)
	{
		if (this.diseaseRedirectTarget)
		{
			this.diseaseRedirectTarget.ModifyDiseaseCount(delta, reason);
			return;
		}
		if (this.useSimDiseaseInfo)
		{
			SimMessages.ModifyDiseaseOnCell(Grid.PosToCell(this), byte.MaxValue, delta);
			return;
		}
		if (delta != 0 && this.diseaseHandle.IsValid() && GameComps.DiseaseContainers.ModifyDiseaseCount(this.diseaseHandle, delta) <= 0 && !this.forcePermanentDiseaseContainer)
		{
			base.Trigger(-1689370368, BoxedBools.False);
			GameComps.DiseaseContainers.Remove(base.gameObject);
			this.diseaseHandle.Clear();
		}
	}

	public void AddDisease(byte disease_idx, int delta, string reason)
	{
		if (delta == 0)
		{
			return;
		}
		if (this.diseaseRedirectTarget)
		{
			this.diseaseRedirectTarget.AddDisease(disease_idx, delta, reason);
			return;
		}
		if (this.useSimDiseaseInfo)
		{
			SimMessages.ModifyDiseaseOnCell(Grid.PosToCell(this), disease_idx, delta);
			return;
		}
		if (this.diseaseHandle.IsValid())
		{
			if (GameComps.DiseaseContainers.AddDisease(this.diseaseHandle, disease_idx, delta) <= 0)
			{
				GameComps.DiseaseContainers.Remove(base.gameObject);
				this.diseaseHandle.Clear();
				return;
			}
		}
		else if (delta > 0)
		{
			this.diseaseHandle = GameComps.DiseaseContainers.Add(base.gameObject, disease_idx, delta);
			base.Trigger(-1689370368, BoxedBools.True);
			base.Trigger(-283306403, null);
		}
	}

	private static float OnGetTemperature(PrimaryElement primary_element)
	{
		return primary_element._Temperature;
	}

	private static void OnSetTemperature(PrimaryElement primary_element, float temperature)
	{
		global::Debug.Assert(!float.IsNaN(temperature));
		if (temperature <= 0f)
		{
			DebugUtil.LogErrorArgs(primary_element.gameObject, new object[] { primary_element.gameObject.name + " has a temperature of zero which has always been an error in my experience." });
		}
		primary_element._Temperature = temperature;
	}

	private void OnSplitFromChunk(object data)
	{
		Pickupable pickupable = (Pickupable)data;
		if (pickupable == null)
		{
			return;
		}
		float num = this.Units / (this.Units + pickupable.PrimaryElement.Units);
		SimUtil.DiseaseInfo percentOfDisease = SimUtil.GetPercentOfDisease(pickupable.PrimaryElement, num);
		this.AddDisease(percentOfDisease.idx, percentOfDisease.count, "PrimaryElement.SplitFromChunk");
		pickupable.PrimaryElement.ModifyDiseaseCount(-percentOfDisease.count, "PrimaryElement.SplitFromChunk");
	}

	private void OnAbsorb(object data)
	{
		Pickupable pickupable = (Pickupable)data;
		if (pickupable == null)
		{
			return;
		}
		this.AddDisease(pickupable.PrimaryElement.DiseaseIdx, pickupable.PrimaryElement.DiseaseCount, "PrimaryElement.OnAbsorb");
	}

	private void SetDiseaseVisualProvider(GameObject visualizer)
	{
		HandleVector<int>.Handle handle = GameComps.DiseaseContainers.GetHandle(base.gameObject);
		if (handle != HandleVector<int>.InvalidHandle)
		{
			DiseaseContainer payload = GameComps.DiseaseContainers.GetPayload(handle);
			payload.visualDiseaseProvider = visualizer;
			GameComps.DiseaseContainers.SetPayload(handle, ref payload);
		}
	}

	public void RedirectDisease(GameObject target)
	{
		this.SetDiseaseVisualProvider(target);
		this.diseaseRedirectTarget = (target ? target.GetComponent<PrimaryElement>() : null);
		global::Debug.Assert(this.diseaseRedirectTarget != this, "Disease redirect target set to myself");
	}

	public static float MAX_MASS = 100000f;

	public SimTemperatureTransfer sttOptimizationHook;

	public PrimaryElement.GetTemperatureCallback getTemperatureCallback = new PrimaryElement.GetTemperatureCallback(PrimaryElement.OnGetTemperature);

	public PrimaryElement.SetTemperatureCallback setTemperatureCallback = new PrimaryElement.SetTemperatureCallback(PrimaryElement.OnSetTemperature);

	private PrimaryElement diseaseRedirectTarget;

	private bool useSimDiseaseInfo;

	public const float DefaultChunkMass = 400f;

	private static readonly Tag[] metalTags = new Tag[]
	{
		GameTags.Metal,
		GameTags.RefinedMetal
	};

	[Serialize]
	[HashedEnum]
	public SimHashes ElementID;

	private float _units = 1f;

	[Serialize]
	[SerializeField]
	private float _Temperature;

	[Serialize]
	[NonSerialized]
	public bool KeepZeroMassObject;

	[Serialize]
	private HashedString diseaseID;

	[Serialize]
	private int diseaseCount;

	private HandleVector<int>.Handle diseaseHandle = HandleVector<int>.InvalidHandle;

	public float MassPerUnit = 1f;

	[NonSerialized]
	private Element _Element;

	[NonSerialized]
	public Action<PrimaryElement> onDataChanged;

	[NonSerialized]
	private bool forcePermanentDiseaseContainer;

	private static readonly EventSystem.IntraObjectHandler<PrimaryElement> OnSplitFromChunkDelegate = new EventSystem.IntraObjectHandler<PrimaryElement>(delegate(PrimaryElement component, object data)
	{
		component.OnSplitFromChunk(data);
	});

	private static readonly EventSystem.IntraObjectHandler<PrimaryElement> OnAbsorbDelegate = new EventSystem.IntraObjectHandler<PrimaryElement>(delegate(PrimaryElement component, object data)
	{
		component.OnAbsorb(data);
	});

	public delegate float GetTemperatureCallback(PrimaryElement primary_element);

	public delegate void SetTemperatureCallback(PrimaryElement primary_element, float temperature);
}
