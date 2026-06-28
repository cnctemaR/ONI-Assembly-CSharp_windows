using System;
using FileHelpers;
using UnityEngine;

public class ElementInteractions
{
	public ElementInteractions(TextAsset asset)
	{
		this.asset = asset;
		this.Reload();
	}

	private void Reload()
	{
		FileHelperEngine fileHelperEngine = new FileHelperEngine(typeof(ElementInteractions.InteractionDef));
		ElementInteractions.InteractionDef[] array = (ElementInteractions.InteractionDef[])fileHelperEngine.ReadString(this.asset.text);
		this.interactions = new SimMessages.ElementInteraction[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			this.interactions[i] = array[i].Transform();
		}
		SimMessages.CreateElementInteractions(this.interactions);
	}

	public SimMessages.ElementInteraction[] interactions;

	private TextAsset asset;

	[DelimitedRecord(",")]
	[IgnoreEmptyLines]
	[IgnoreCommentedLines("//")]
	[IgnoreFirst(1)]
	private class InteractionDef
	{
		public SimMessages.ElementInteraction Transform()
		{
			return new SimMessages.ElementInteraction
			{
				interactionType = (uint)this.interactionType,
				minMass = this.minMass,
				interactionProbability = this.interactionProbability,
				elemIdx1 = (byte)ElementLoader.GetElementIndex(this.elemHash1),
				elem1MassDestructionPercent = this.elem1MassDestructionPercent,
				elemIdx2 = (byte)ElementLoader.GetElementIndex(this.elemHash2),
				elem2MassRequiredMultiplier = this.elem2MassRequiredMultiplier,
				elemResultIdx = (byte)ElementLoader.GetElementIndex(this.elemResultHash),
				elemResultMassCreationMultiplier = this.elemResultMassCreationMultiplier
			};
		}

		public ElementInteractionHashes interactionType;

		[FieldNullValue(1f)]
		public float interactionProbability = 1f;

		[FieldNullValue(0f)]
		public float minMass;

		public SimHashes elemHash1;

		[FieldNullValue(1f)]
		public float elem1MassDestructionPercent = 1f;

		public SimHashes elemHash2;

		[FieldNullValue(1f)]
		public float elem2MassRequiredMultiplier = 1f;

		public SimHashes elemResultHash;

		[FieldNullValue(1f)]
		public float elemResultMassCreationMultiplier = 1f;
	}
}
