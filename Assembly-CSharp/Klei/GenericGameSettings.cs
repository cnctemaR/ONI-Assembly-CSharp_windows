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

		public int demoTime { get; private set; }

		public bool showDemoTimer { get; private set; }

		private static GenericGameSettings _instance;
	}
}
