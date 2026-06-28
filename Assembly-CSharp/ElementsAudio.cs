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
	[IgnoreEmptyLines]
	[IgnoreFirst(1)]
	public class ElementAudioConfig
	{
		[FieldOrder(1)]
		public SimHashes elementID;

		[FieldOptional]
		[FieldNullValue(AmbienceType.None)]
		[FieldOrder(2)]
		public AmbienceType ambienceType;

		[FieldOrder(3)]
		[FieldOptional]
		[FieldNullValue(SolidAmbienceType.None)]
		public SolidAmbienceType solidAmbienceType;

		[FieldOptional]
		[FieldNullValue("")]
		[FieldOrder(4)]
		public string miningSound;

		[FieldNullValue("")]
		[FieldOptional]
		[FieldOrder(5)]
		public string miningBreakSound;

		[FieldNullValue("")]
		[FieldOrder(6)]
		[FieldOptional]
		public string oreBumpSound;

		[FieldOrder(7)]
		[FieldOptional]
		[FieldNullValue("")]
		public string floorEventAudioCategory;
	}
}
