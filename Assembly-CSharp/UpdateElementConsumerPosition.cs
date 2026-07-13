using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/UpdateElementConsumerPosition")]
public class UpdateElementConsumerPosition : KMonoBehaviour, ISim200ms
{
	protected override void OnSpawn()
	{
		this.consumer = base.GetComponent<ElementConsumer>();
	}

	public void Sim200ms(float dt)
	{
		this.consumer.GetComponent<ElementConsumer>().RefreshConsumptionRate();
	}

	private ElementConsumer consumer;
}
