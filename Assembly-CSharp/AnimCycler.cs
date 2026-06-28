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

	public KAnimFileData file;

	public KBatchedAnimController controller;

	public int index;
}
