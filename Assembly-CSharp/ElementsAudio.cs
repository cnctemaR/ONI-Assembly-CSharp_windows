using System;
using FileHelpers;
using UnityEngine;

public class ElementsAudio
{
	public static ElementsAudio Instance
	{
		get
		{
			if (ElementsAudio._instance == null)
			{
				ElementsAudio._instance = new ElementsAudio();
			}
			return ElementsAudio._instance;
		}
	}

	public void LoadData(string csvData)
	{
		Debug.Assert(csvData != null && csvData != string.Empty, "Empty element audio config file... ");
		FileHelperEngine fileHelperEngine = new FileHelperEngine(typeof(ElementsAudio.ElementAudioConfig));
		this.elementAudioConfigs = fileHelperEngine.ReadString(csvData) as ElementsAudio.ElementAudioConfig[];
		Debug.Assert(this.elementAudioConfigs != null, "Failed to load audio configs for elements");
	}

	public ElementsAudio.ElementAudioConfig GetConfigForElement(SimHashes id)
	{
		if (this.elementAudioConfigs != null)
		{
			for (int i = 0; i < this.elementAudioConfigs.Length; i++)
			{
				if (this.elementAudioConfigs[i].elementID == id)
				{
					return this.elementAudioConfigs[i];
				}
			}
		}
		return null;
	}

	private static ElementsAudio _instance;

	private ElementsAudio.ElementAudioConfig[] elementAudioConfigs;

	[IgnoreFirst(1)]
	[DelimitedRecord(",")]
	[IgnoreEmptyLines]
	public class ElementAudioConfig
	{
		[FieldOrder(1)]
		public SimHashes elementID;

		[FieldOptional]
		[FieldOrder(2)]
		[FieldNullValue(AmbienceType.None)]
		public AmbienceType ambienceType;

		[FieldOrder(3)]
		[FieldNullValue(SolidAmbienceType.None)]
		[FieldOptional]
		public SolidAmbienceType solidAmbienceType;

		[FieldOrder(4)]
		[FieldNullValue("")]
		[FieldOptional]
		public string miningSound;

		[FieldOptional]
		[FieldNullValue("")]
		[FieldOrder(5)]
		public string miningBreakSound;

		[FieldOrder(6)]
		[FieldNullValue("")]
		[FieldOptional]
		public string oreBumpSound;

		[FieldNullValue("")]
		[FieldOptional]
		[FieldOrder(7)]
		public string floorEventAudioCategory;
	}
}
