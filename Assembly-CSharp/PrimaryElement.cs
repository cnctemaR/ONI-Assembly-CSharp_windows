using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using Klei;
using Klei.AI;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
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
			this._units = value;
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
			Sim.DiseaseCell diseaseCell = Grid.Disease[num];
			if (diseaseCell.diseaseIdx != 255)
			{
				this.diseaseID = Db.Get().Diseases[(int)diseaseCell.diseaseIdx].id;
				this.diseaseCount = diseaseCell.elementCount;
			}
		}
		else if (this.diseaseHandle.IsValid())
		{
			DiseaseContainer data = GameComps.DiseaseContainers.GetData(this.diseaseHandle);
			if (data.diseaseIdx != 255)
			{
				this.diseaseID = Db.Get().Diseases[(int)data.diseaseIdx].id;
				this.diseaseCount = data.diseaseCount;
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
		this.Temperature = this._Temperature;
		if (float.IsNaN(this.Temperature))
		{
			DeserializeWarnings.Instance.PrimaryElementTemperatureIsNan.Warn(base.name + " temperature is NaN. Resetting temperature.", null);
			this._Temperature = this.Element.defaultValues.temperature;
			this.Temperature = this.Element.defaultValues.temperature;
		}
		if (this.Temperature <= 0f)
		{
			DeserializeWarnings.Instance.PrimaryElementTemperatureIsNan.Warn(base.name + " temperature is zero. Resetting temperature because I don't believe it", null);
			this._Temperature = this.Element.defaultValues.temperature;
			this.Temperature = this.Element.defaultValues.temperature;
		}
		if (this.Element == null)
		{
			DeserializeWarnings.Instance.PrimaryElementHasNoElement.Warn(base.name + "Primary element has no element.", null);
		}
		if (this.Mass > 100000f)
		{
			Output.LogWarningWithObj(base.gameObject, new object[] { "deserialized very large ore mass... error?" });
		}
		if (this.Mass < 0f)
		{
			Output.LogErrorWithObj(base.gameObject, new object[] { "deserialized ore with less than 0 mass. Error! Destroying" });
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
			}
		}
		else if (this.diseaseHandle.IsValid())
		{
			DiseaseContainer data = GameComps.DiseaseContainers.GetData(this.diseaseHandle);
			data.diseaseIdx = index;
			data.diseaseCount = this.diseaseCount;
			GameComps.DiseaseContainers.SetData(this.diseaseHandle, data);
		}
		else
		{
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
			KCrashReporter.Assert(false, base.gameObject.name + " is attempting serializing a temperature of <= 0K. Resetting to default.");
			this._Temperature = this.Element.defaultValues.temperature;
		}
		if (this.Mass > 100000f)
		{
			KCrashReporter.Assert(false, base.gameObject.name + " is attempting to serialize a very large mass. Resetting to default.");
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
		this.Units = mass / this.MassPerUnit;
		if (this.Units <= 0f && !this.KeepZeroMassObject)
		{
			Util.KDestroyGameObject(base.gameObject);
		}
		else if (!this.KeepZeroMassObject && this.Units <= 0f)
		{
			throw new ArgumentException("Invalid mass");
		}
		if (this.Units > 100000f)
		{
			Output.LogWarningWithObj(base.gameObject, new object[] { "unexpectedly large mass" });
		}
	}

	private void SetTemperature(float temperature)
	{
		if (float.IsNaN(temperature) || float.IsInfinity(temperature))
		{
			Output.LogErrorWithObj(base.gameObject, new object[] { "Invalid temperature [" + temperature + "]" });
			return;
		}
		if (temperature <= 0f)
		{
			StackTrace stackTrace = new StackTrace(0, true);
			KCrashReporter.Assert(false, "Tried to set PrimaryElement.Temperature to a value <= 0\n\n" + stackTrace.ToString());
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
			byte b = byte.MaxValue;
			if (this.useSimDiseaseInfo)
			{
				int num = Grid.PosToCell(base.transform.GetPosition());
				b = Grid.Disease[num].diseaseIdx;
			}
			else if (this.diseaseHandle.IsValid())
			{
				b = GameComps.DiseaseContainers.GetData(this.diseaseHandle).diseaseIdx;
			}
			return b;
		}
	}

	public int DiseaseCount
	{
		get
		{
			int num = 0;
			if (this.useSimDiseaseInfo)
			{
				int num2 = Grid.PosToCell(base.transform.GetPosition());
				num = Grid.Disease[num2].elementCount;
			}
			else if (this.diseaseHandle.IsValid())
			{
				num = GameComps.DiseaseContainers.GetData(this.diseaseHandle).diseaseCount;
			}
			return num;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		GameComps.InfraredVisualizers.Add(base.gameObject);
		base.Subscribe(1335436905, new Action<object>(this.OnSplitFromChunk));
		base.Subscribe(-2064133523, new Action<object>(this.OnAbsorb));
	}

	protected override void OnSpawn()
	{
		Attributes attributes = this.GetAttributes();
		if (attributes != null)
		{
			Element element = this.Element;
			foreach (AttributeModifier attributeModifier in element.attributeModifiers)
			{
				attributes.Add(element.name, attributeModifier);
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

	public void SetElement(SimHashes element_id)
	{
		this.ElementID = element_id;
		this.UpdateTags();
	}

	public void UpdateTags()
	{
		if (this.ElementID == (SimHashes)0)
		{
			Output.LogWithObj(base.gameObject, new object[] { "UpdateTags() Primary element 0" });
			return;
		}
		KPrefabID component = base.GetComponent<KPrefabID>();
		if (component != null)
		{
			List<Tag> list = new List<Tag>();
			Element element = this.Element;
			list.Add(GameTagExtensions.Create(element.id));
			foreach (Tag tag in element.oreTags)
			{
				list.Add(tag);
			}
			if (component.HasAnyTags(PrimaryElement.metalTags))
			{
				list.Add(GameTags.StoredMetal);
			}
			component.AddPrefabTags(list);
		}
	}

	public void ModifyDiseaseCount(int delta, string reason)
	{
		if (this.ModifyDiseaseCountHandler != null)
		{
			this.ModifyDiseaseCountHandler(delta, reason);
			return;
		}
		if (this.useSimDiseaseInfo)
		{
			int num = Grid.PosToCell(this);
			SimMessages.ModifyDiseaseOnCell(num, byte.MaxValue, delta);
		}
		else if (delta != 0 && this.diseaseHandle.IsValid())
		{
			int num2 = GameComps.DiseaseContainers.ModifyDiseaseCount(this.diseaseHandle, delta);
			if (num2 <= 0 && !this.forcePermanentDiseaseContainer)
			{
				base.Trigger(-1689370368, false);
				GameComps.DiseaseContainers.Remove(base.gameObject);
				this.diseaseHandle.Clear();
			}
		}
	}

	public void AddDisease(byte disease_idx, int delta, string reason)
	{
		if (delta == 0)
		{
			return;
		}
		if (this.AddDiseaseHandler != null)
		{
			this.AddDiseaseHandler(disease_idx, delta, reason);
			return;
		}
		if (this.useSimDiseaseInfo)
		{
			int num = Grid.PosToCell(this);
			SimMessages.ModifyDiseaseOnCell(num, disease_idx, delta);
		}
		else if (this.diseaseHandle.IsValid())
		{
			int num2 = GameComps.DiseaseContainers.AddDisease(this.diseaseHandle, disease_idx, delta);
			if (num2 <= 0)
			{
				GameComps.DiseaseContainers.Remove(base.gameObject);
				this.diseaseHandle.Clear();
			}
		}
		else if (delta > 0)
		{
			this.diseaseHandle = GameComps.DiseaseContainers.Add(base.gameObject, disease_idx, delta);
			base.Trigger(-1689370368, true);
			base.Trigger(-283306403, null);
		}
	}

	private static float OnGetTemperature(PrimaryElement primary_element)
	{
		return primary_element._Temperature;
	}

	private static void OnSetTemperature(PrimaryElement primary_element, float temperature)
	{
		if (temperature <= 0f)
		{
			Output.LogErrorWithObj(primary_element.gameObject, new object[] { primary_element.gameObject.name + " has a temperature of zero which has always been an error in my experience." });
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

	public void SetDiseaseVisualProvider(GameObject visualizer)
	{
		HandleVector<int>.Handle handle = GameComps.DiseaseContainers.GetHandle(base.gameObject);
		if (handle != HandleVector<int>.InvalidHandle)
		{
			DiseaseContainer data = GameComps.DiseaseContainers.GetData(handle);
			data.visualDiseaseProvider = visualizer;
			GameComps.DiseaseContainers.SetData(handle, data);
		}
	}

	public PrimaryElement.GetTemperatureCallback getTemperatureCallback = new PrimaryElement.GetTemperatureCallback(PrimaryElement.OnGetTemperature);

	public PrimaryElement.SetTemperatureCallback setTemperatureCallback = new PrimaryElement.SetTemperatureCallback(PrimaryElement.OnSetTemperature);

	public Action<int, string> ModifyDiseaseCountHandler;

	public Action<byte, int, string> AddDiseaseHandler;

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

	public delegate float GetTemperatureCallback(PrimaryElement primary_element);

	public delegate void SetTemperatureCallback(PrimaryElement primary_element, float temperature);
}
