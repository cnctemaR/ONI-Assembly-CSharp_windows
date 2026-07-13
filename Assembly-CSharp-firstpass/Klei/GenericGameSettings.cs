using System;
using System.IO;
using UnityEngine;

namespace Klei
{
	public class GenericGameSettings
	{
		public static GenericGameSettings instance
		{
			get
			{
				if (GenericGameSettings._instance == null)
				{
					if (FileSystem.FileExists(GenericGameSettings.Path))
					{
						GenericGameSettings._instance = YamlIO.LoadFile<GenericGameSettings>(GenericGameSettings.Path, null, null);
						global::Debug.Assert(GenericGameSettings._instance != null, "Loading " + GenericGameSettings.Path + " returned null, the file may be corrupted");
					}
					else
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

		public bool autoResumeGame { get; private set; }

		public bool disableFogOfWar { get; private set; }

		public bool acceleratedLifecycle { get; private set; }

		public bool enableEditorCrashReporting { get; private set; }

		public bool allowInsufficientMaterialBuild { get; private set; }

		public bool keepAllAutosaves { get; private set; }

		public bool takeSaveScreenshots { get; private set; }

		public bool disableAutosave { get; private set; }

		public bool quickDevTools { get; private set; }

		public bool devAutoWorldGen { get; set; }

		public int devWorldGenSeed { get; set; }

		public string devWorldGenCluster { get; set; }

		public string[] devWorldGenSkip { get; set; }

		public string[] devStoryTraits { get; set; }

		public string[] devSubworldMixing { get; set; }

		public string[] devWorldMixing { get; set; }

		public bool devBootSmoke { get; set; }

		public bool devBootModReport { get; set; }

		public bool devQuitAfterLoadingSave { get; set; }

		public bool enableAudioLogging { get; set; }

		public GenericGameSettings.ScriptedProfile scriptedProfile { get; set; }

		private static string Path
		{
			get
			{
				return global::System.IO.Path.GetDirectoryName(Application.dataPath) + "/settings.yml";
			}
		}

		public GenericGameSettings()
		{
			this.demoMode = false;
			this.demoTime = 300;
			this.showDemoTimer = true;
			this.sleepWhenOutOfFocus = true;
			this.debugEnable = false;
			this.developerDebugEnable = false;
			this.scriptedProfile = new GenericGameSettings.ScriptedProfile();
			GenericGameSettings._instance = this;
		}

		public void SaveSettings()
		{
			try
			{
				YamlIO.Save<GenericGameSettings>(this, GenericGameSettings.Path, null);
			}
			catch (Exception ex)
			{
				global::Debug.LogWarning("Failed to save settings.yml: " + ex.ToString());
			}
		}

		private static GenericGameSettings _instance;

		public bool devAutoWorldGenActive;

		public class ScriptedProfile
		{
			public string saveGame { get; set; }

			public bool disableGC { get; set; }

			public float startWaitTime { get; set; }

			public int frameCount { get; set; }

			public string eventFilename { get; set; }
		}
	}
}
