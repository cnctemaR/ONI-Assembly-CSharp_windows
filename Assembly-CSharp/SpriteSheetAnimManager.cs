using System;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSheetAnimManager : KMonoBehaviour, IRenderEveryTick
{
	protected override void OnPrefabInit()
	{
		SpriteSheetAnimManager.instance = this;
	}

	protected override void OnSpawn()
	{
		for (int i = 0; i < this.sheets.Length; i++)
		{
			int num = Hash.SDBMLower(this.sheets[i].name);
			this.nameIndexMap[num] = new SpriteSheetAnimator(this.sheets[i]);
		}
	}

	public void Play(string name, Vector3 pos, Vector2 size, Color32 colour)
	{
		int num = Hash.SDBMLower(name);
		this.Play(num, pos, Quaternion.identity, size, colour);
	}

	public void Play(string name, Vector3 pos, Quaternion rotation, Vector2 size, Color32 colour)
	{
		int num = Hash.SDBMLower(name);
		this.Play(num, pos, rotation, size, colour);
	}

	public void Play(int name_hash, Vector3 pos, Quaternion rotation, Vector2 size, Color32 colour)
	{
		SpriteSheetAnimator spriteSheetAnimator = this.nameIndexMap[name_hash];
		spriteSheetAnimator.Play(pos, rotation, size, colour);
	}

	public void RenderEveryTick(float dt)
	{
		this.UpdateAnims(dt);
		this.Render();
	}

	public void UpdateAnims(float dt)
	{
		foreach (SpriteSheetAnimator spriteSheetAnimator in this.nameIndexMap.Values)
		{
			spriteSheetAnimator.UpdateAnims(dt);
		}
	}

	public void Render()
	{
		Vector3 zero = Vector3.zero;
		foreach (SpriteSheetAnimator spriteSheetAnimator in this.nameIndexMap.Values)
		{
			spriteSheetAnimator.Render();
		}
	}

	public SpriteSheetAnimator GetSpriteSheetAnimator(HashedString name)
	{
		return this.nameIndexMap[name.HashValue];
	}

	public const float SECONDS_PER_FRAME = 0.033333335f;

	[SerializeField]
	private SpriteSheet[] sheets;

	private Dictionary<int, SpriteSheetAnimator> nameIndexMap = new Dictionary<int, SpriteSheetAnimator>();

	public static SpriteSheetAnimManager instance;
}
