using System;
using UnityEngine;

public class AnimCycler : MonoBehaviour
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

	private void Update()
	{
		this.timer += Time.deltaTime;
		if (this.timer > this.maxPerAnim)
		{
			this.timer = 0f;
			this.Next();
		}
	}

	private void Next()
	{
		if (this.file != null && this.file.anims != null && this.file.anims.Length > 0)
		{
			this.controller.Play(this.file.anims[this.index++].name, KAnim.PlayMode.Loop, 1f, 0f);
			if (this.file.build != null)
			{
				this.controller.AddBuildOverride(this.file, true);
			}
			this.index %= this.file.anims.Length;
		}
	}

	public KAnimFileData file;

	public KBatchedAnimController controller;

	private float timer;

	public float maxPerAnim = 5f;

	public int index;
}
