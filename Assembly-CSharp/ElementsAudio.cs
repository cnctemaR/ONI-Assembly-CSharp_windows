using System;
using FileHelpers;

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
		FileHelperEngine fileHelperEngine = new FileHelperEngine(typeof(ElementsAudio.ElementAudioConfig));
		this.elementAudioConfigs = fileHelperEngine.ReadString(csvData) as ElementsAudio.ElementAudioConfig[];
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

	[DelimitedRecord(",")]
	[IgnoreFirst(1)]
	[IgnoreEmptyLines]
	public class ElementAudioConfig
	{
		[FieldOrder(1)]
		public SimHashes elementID;

		[FieldOrder(2)]
		[FieldOptional]
		[FieldNullValue(AmbienceType.None)]
		public AmbienceType ambienceType;

		[FieldOrder(3)]
		[FieldOptional]
		[FieldNullValue(SolidAmbienceType.None)]
		public SolidAmbienceType solidAmbienceType;

		[FieldOrder(4)]
		[FieldOptional]
		[FieldNullValue("")]
		public string miningSound;

		[FieldOrder(5)]
		[FieldOptional]
		[FieldNullValue("")]
		public string miningBreakSound;

		[FieldOrder(6)]
		[FieldOptional]
		[FieldNullValue("")]
		public string oreBumpSound;

		[FieldOrder(7)]
		[FieldOptional]
		[FieldNullValue("")]
		public string floorEventAudioCategory;

		[FieldOrder(8)]
		[FieldOptional]
		[FieldNullValue("")]
		public string creatureChewSound;
	}
}
