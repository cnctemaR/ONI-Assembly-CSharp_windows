using System;
using System.IO;
using UnityEngine;

namespace Klei
{
	public class GenericGameSettings : YamlIO<GenericGameSettings>
	{
		public GenericGameSettings()
		{
			this.demoMode = false;
			this.demoTime = 300;
			this.showDemoTimer = true;
			this.sleepWhenOutOfFocus = true;
			this.debugEnable = false;
			this.developerDebugEnable = false;
			GenericGameSettings._instance = this;
		}

		public static GenericGameSettings instance
		{
			get
			{
				if (GenericGameSettings._instance == null)
				{
					try
					{
						YamlIO<GenericGameSettings>.LoadFile(Path.GetDirectoryName(Application.dataPath) + "/settings.yml");
					}
					catch
					{
						GenericGameSettings._instance = new GenericGameSettings();
					}
				}
				return GenericGameSettings._instance;
			}
		}

		public bool demoMode { get; private set; }

		public bool sleepWhenOutOfFocus { get; private set; }

		public int demoTime { get; private set; }

		public bool showDemoTimer { get; private set; }

		public bool debugEnable { get; private set; }

		public bool developerDebugEnable { get; private set; }

		public bool disableGameOver { get; private set; }

		public bool disablePopFx { get; private set; }

		private static GenericGameSettings _instance;
	}
}
