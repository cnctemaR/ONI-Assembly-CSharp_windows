using System;
using UnityEngine;

public class Sensor
{
	public Sensor(Sensors sensors)
	{
		this.sensors = sensors;
	}

	public ComponentType GetComponent<ComponentType>()
	{
		return this.sensors.GetComponent<ComponentType>();
	}

	public GameObject gameObject
	{
		get
		{
			return this.sensors.gameObject;
		}
	}

	public Transform transform
	{
		get
		{
			return this.gameObject.transform;
		}
	}

	public void Trigger(int hash, object data = null)
	{
		this.sensors.Trigger(hash, data);
	}

	public virtual void Update()
	{
	}

	public virtual void ShowEditor()
	{
	}

	protected Sensors sensors;
}
