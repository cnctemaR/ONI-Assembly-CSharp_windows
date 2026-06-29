using System;
using System.Collections.Generic;
using UnityEngine;

public class BubbleManager : KMonoBehaviour, ISim33ms, IRenderEveryTick
{
	protected override void OnPrefabInit()
	{
		BubbleManager.instance = this;
	}

	public void SpawnBubble(Vector2 position, Vector2 velocity, SimHashes element, float mass, float temperature)
	{
		BubbleManager.Bubble bubble = new BubbleManager.Bubble
		{
			position = position,
			velocity = velocity,
			element = element,
			temperature = temperature,
			mass = mass
		};
		this.bubbles.Add(bubble);
	}

	public void Sim33ms(float dt)
	{
		List<BubbleManager.Bubble> list = ListPool<BubbleManager.Bubble, BubbleManager>.Allocate();
		List<BubbleManager.Bubble> list2 = ListPool<BubbleManager.Bubble, BubbleManager>.Allocate();
		foreach (BubbleManager.Bubble bubble in this.bubbles)
		{
			BubbleManager.Bubble bubble2 = bubble;
			bubble2.position += bubble2.velocity * dt;
			bubble2.elapsedTime += dt;
			int num = Grid.PosToCell(bubble2.position);
			if (!Grid.IsVisiblyInLiquid(bubble2.position) || Grid.Element[num].id == bubble2.element)
			{
				list2.Add(bubble2);
			}
			else
			{
				list.Add(bubble2);
			}
		}
		foreach (BubbleManager.Bubble bubble3 in list2)
		{
			int num2 = Grid.PosToCell(bubble3.position);
			SimMessages.AddRemoveSubstance(num2, bubble3.element, CellEventLogger.Instance.FallingWaterAddToSim, bubble3.mass, bubble3.temperature, byte.MaxValue, 0, -1);
		}
		this.bubbles.Clear();
		this.bubbles.AddRange(list);
		ListPool<BubbleManager.Bubble, BubbleManager>.Free(list2);
		ListPool<BubbleManager.Bubble, BubbleManager>.Free(list);
	}

	public void RenderEveryTick(float dt)
	{
		List<SpriteSheetAnimator.AnimInfo> list = ListPool<SpriteSheetAnimator.AnimInfo, BubbleManager>.Allocate();
		SpriteSheetAnimator spriteSheetAnimator = SpriteSheetAnimManager.instance.GetSpriteSheetAnimator("liquid_splash1");
		foreach (BubbleManager.Bubble bubble in this.bubbles)
		{
			BubbleManager.Bubble bubble2 = bubble;
			SpriteSheetAnimator.AnimInfo animInfo = new SpriteSheetAnimator.AnimInfo
			{
				frame = spriteSheetAnimator.GetFrameFromElapsedTimeLooping(bubble2.elapsedTime),
				elapsedTime = bubble2.elapsedTime,
				pos = new Vector3(bubble2.position.x, bubble2.position.y, 0f),
				rotation = Quaternion.identity,
				size = Vector2.one,
				colour = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue)
			};
			list.Add(animInfo);
		}
		ListPool<SpriteSheetAnimator.AnimInfo, BubbleManager>.Free(list);
	}

	public static BubbleManager instance;

	private List<BubbleManager.Bubble> bubbles = new List<BubbleManager.Bubble>();

	private struct Bubble
	{
		public Vector2 position;

		public Vector2 velocity;

		public float elapsedTime;

		public int frame;

		public SimHashes element;

		public float temperature;

		public float mass;
	}
}
