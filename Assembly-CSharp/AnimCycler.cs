using System;
using UnityEngine;

public class AnimCycler : Cycler
{
	private void Start()
	{
		if (this.file == null)
		{
			global::UnityEngine.Object.DestroyObject(base.gameObject);
		}
		else
		{
			this.Next();
			KBoxCollider2D kboxCollider2D = base.gameObject.AddComponent<KBoxCollider2D>();
			kboxCollider2D.size = Vector2.one * 3f;
			kboxCollider2D.offset = Vector2.up * 1.5f;
			BatchAnimCamera.bounds.Encapsulate(kboxCollider2D.bounds);
			base.gameObject.AddComponent<LoopingSounds>();
		}
	}

	protected override void Next()
	{
		if (this.file != null && this.file.animCount > 0)
		{
			this.controller.Play(this.file.GetAnim(this.index++).name, KAnim.PlayMode.Loop, 1f, 0f);
			if (this.file.buildIndex != -1)
			{
				this.controller.AddBuildOverride(this.file, true, false);
			}
			this.controller.UpdateSymbolLookups();
			this.index %= this.file.animCount;
		}
	}

	private void OnMouseDown()
	{
	}

	public KAnimFileData file;

	public KBatchedAnimController controller;

	public int index;
}
