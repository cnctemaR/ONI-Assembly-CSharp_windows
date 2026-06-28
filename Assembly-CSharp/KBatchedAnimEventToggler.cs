using System;
using System.Collections.Generic;
using UnityEngine;

public class KBatchedAnimEventToggler : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		Vector3 position = this.eventSource.transform.GetPosition();
		position.z = Grid.GetLayerZ(Grid.SceneLayer.FXFront);
		int num = LayerMask.NameToLayer("Default");
		foreach (KBatchedAnimEventToggler.Entry entry in this.entries)
		{
			entry.controller.transform.SetPosition(position);
			entry.controller.SetLayer(num);
			entry.controller.gameObject.SetActive(false);
		}
		int num2 = Hash.SDBMLower(this.enableEvent);
		int num3 = Hash.SDBMLower(this.disableEvent);
		base.Subscribe(this.eventSource, num2, new Action<object>(this.Enable));
		base.Subscribe(this.eventSource, num3, new Action<object>(this.Disable));
	}

	protected override void OnSpawn()
	{
		this.animEventHandler = base.GetComponentInParent<AnimEventHandler>();
	}

	private void Enable(object data)
	{
		this.StopAll();
		HashedString context = this.animEventHandler.GetContext();
		if (!context.IsValid)
		{
			return;
		}
		foreach (KBatchedAnimEventToggler.Entry entry in this.entries)
		{
			if (entry.context == context)
			{
				entry.controller.gameObject.SetActive(true);
				entry.controller.Play(entry.anim, KAnim.PlayMode.Loop, 1f, 0f);
			}
		}
	}

	private void Disable(object data)
	{
		this.StopAll();
	}

	private void StopAll()
	{
		foreach (KBatchedAnimEventToggler.Entry entry in this.entries)
		{
			entry.controller.StopAndClear();
			entry.controller.gameObject.SetActive(false);
		}
	}

	[SerializeField]
	private GameObject eventSource;

	[SerializeField]
	private string enableEvent;

	[SerializeField]
	private string disableEvent;

	[SerializeField]
	private List<KBatchedAnimEventToggler.Entry> entries;

	private AnimEventHandler animEventHandler;

	[Serializable]
	public struct Entry
	{
		public string anim;

		public HashedString context;

		public KBatchedAnimController controller;
	}
}
