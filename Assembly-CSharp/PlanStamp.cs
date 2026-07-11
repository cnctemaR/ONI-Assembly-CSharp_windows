using System;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/PlanStamp")]
public class PlanStamp : KMonoBehaviour
{
	public void SetStamp(Sprite sprite, string Text)
	{
		this.StampImage.sprite = sprite;
		this.StampText.text = Text.ToUpper();
	}

	public PlanStamp.StampArt stampSprites;

	[SerializeField]
	private Image StampImage;

	[SerializeField]
	private Text StampText;

	[Serializable]
	public struct StampArt
	{
		public Sprite UnderConstruction;

		public Sprite NeedsResearch;

		public Sprite SelectResource;

		public Sprite NeedsRepair;

		public Sprite NeedsPower;

		public Sprite NeedsResource;

		public Sprite NeedsGasPipe;

		public Sprite NeedsLiquidPipe;
	}
}
