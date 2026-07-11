using System;

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

	public void LoadData(ElementsAudio.ElementAudioConfig[] elements_audio_configs)
	{
		this.elementAudioConfigs = elements_audio_configs;
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

	public class ElementAudioConfig : Resource
	{
		public SimHashes elementID;

		public AmbienceType ambienceType = AmbienceType.None;

		public SolidAmbienceType solidAmbienceType = SolidAmbienceType.None;

		public string miningSound = "";

		public string miningBreakSound = "";

		public string oreBumpSound = "";

		public string floorEventAudioCategory = "";

		public string creatureChewSound = "";
	}
}
