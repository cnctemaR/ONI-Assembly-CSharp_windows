using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Klei.AI;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class PrimaryElement : KMonoBehaviour, ISaveLoadable
{
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
			if (float.IsNaN(value) || float.IsInfinity(value))
			{
				Output.LogErrorWithObj(base.gameObject, new object[] { "Invalid temperature [" + value + "]" });
				return;
			}
			this.setTemperatureCallback(this, value);
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
			this.Units = value / this.MassPerUnit;
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
			if (this.onDataChanged != null)
			{
				this.onDataChanged(this);
			}
		}
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

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		GameComps.InfraredVisualizers.Add(base.gameObject);
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

	protected override void OnCleanUp()
	{
		GameComps.InfraredVisualizers.Remove(base.gameObject);
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
			list.Add(TagManager.Create(element.id));
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

	public const float DefaultChunkMass = 400f;

	public PrimaryElement.GetTemperatureCallback getTemperatureCallback = new PrimaryElement.GetTemperatureCallback(PrimaryElement.OnGetTemperature);

	public PrimaryElement.SetTemperatureCallback setTemperatureCallback = new PrimaryElement.SetTemperatureCallback(PrimaryElement.OnSetTemperature);

	private static readonly Tag[] metalTags = new Tag[]
	{
		GameTags.Metal,
		GameTags.RefinedMetal
	};

	[HashedEnum]
	[Serialize]
	public SimHashes ElementID;

	private float _units = 1f;

	[Serialize]
	[SerializeField]
	private float _Temperature;

	[Serialize]
	[NonSerialized]
	public bool KeepZeroMassObject;

	public bool CountableUnits;

	public float MassPerUnit = 1f;

	[NonSerialized]
	private Element _Element;

	[NonSerialized]
	public Action<PrimaryElement> onDataChanged;

	public delegate float GetTemperatureCallback(PrimaryElement primary_element);

	public delegate void SetTemperatureCallback(PrimaryElement primary_element, float temperature);
}
