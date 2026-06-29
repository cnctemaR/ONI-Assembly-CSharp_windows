using System;
using UnityEngine;

public class BubbleSpawner : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		this.emitMass += (global::UnityEngine.Random.value - 0.5f) * this.emitVariance * this.emitMass;
		base.OnSpawn();
		base.Subscribe(-1697596308, new Action<object>(this.OnStorageChanged));
	}

	private void OnStorageChanged(object data)
	{
		GameObject gameObject = this.storage.FindFirst(ElementLoader.FindElementByHash(this.element).tag);
		if (gameObject == null)
		{
			return;
		}
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		if (component.Mass >= this.emitMass)
		{
			gameObject.GetComponent<PrimaryElement>().Mass -= this.emitMass;
			BubbleManager.instance.SpawnBubble(base.transform.GetPosition(), this.initialVelocity, component.ElementID, this.emitMass, component.Temperature);
		}
	}

	public SimHashes element;

	public float emitMass;

	public float emitVariance;

	public Vector3 emitOffset = Vector3.zero;

	public Vector2 initialVelocity;

	[MyCmpGet]
	private Storage storage;
}
