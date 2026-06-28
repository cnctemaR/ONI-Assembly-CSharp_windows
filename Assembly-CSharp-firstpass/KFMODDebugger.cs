using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class KFMODDebugger : KMonoBehaviour
{
	public static KFMODDebugger Get()
	{
		return KFMODDebugger.instance;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		KFMODDebugger.instance = this;
		IEnumerator enumerator = Enum.GetValues(typeof(KFMODDebugger.DebugSoundType)).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				KFMODDebugger.DebugSoundType debugSoundType = (KFMODDebugger.DebugSoundType)obj;
				this.allDebugSoundTypes.Add(debugSoundType, false);
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = enumerator as IDisposable) != null)
			{
				disposable.Dispose();
			}
		}
	}

	protected override void OnCleanUp()
	{
		KFMODDebugger.instance = null;
	}

	[Conditional("UNITY_EDITOR")]
	public void Log(string s)
	{
	}

	private KFMODDebugger.DebugSoundType GetDebugSoundType(string s)
	{
		KFMODDebugger.DebugSoundType debugSoundType;
		if (s.Contains("Buildings"))
		{
			debugSoundType = KFMODDebugger.DebugSoundType.Buildings;
		}
		else if (s.Contains("Notifications"))
		{
			debugSoundType = KFMODDebugger.DebugSoundType.Notifications;
		}
		else if (s.Contains("UI"))
		{
			debugSoundType = KFMODDebugger.DebugSoundType.UI;
		}
		else if (s.Contains("Creatures"))
		{
			debugSoundType = KFMODDebugger.DebugSoundType.Creatures;
		}
		else if (s.Contains("Duplicant_voices"))
		{
			debugSoundType = KFMODDebugger.DebugSoundType.DupeVoices;
		}
		else if (s.Contains("Ambience"))
		{
			debugSoundType = KFMODDebugger.DebugSoundType.Ambience;
		}
		else if (s.Contains("Environment"))
		{
			debugSoundType = KFMODDebugger.DebugSoundType.Environment;
		}
		else if (s.Contains("FX"))
		{
			debugSoundType = KFMODDebugger.DebugSoundType.FX;
		}
		else if (s.Contains("Duplicant_actions/LowImportance/Movement"))
		{
			debugSoundType = KFMODDebugger.DebugSoundType.DupeMovement;
		}
		else if (s.Contains("Duplicant_actions"))
		{
			debugSoundType = KFMODDebugger.DebugSoundType.DupeActions;
		}
		else if (s.Contains("Plants"))
		{
			debugSoundType = KFMODDebugger.DebugSoundType.Plants;
		}
		else if (s.Contains("Music"))
		{
			debugSoundType = KFMODDebugger.DebugSoundType.Music;
		}
		else
		{
			debugSoundType = KFMODDebugger.DebugSoundType.Uncategorized;
		}
		return debugSoundType;
	}

	public static KFMODDebugger instance;

	public List<KFMODDebugger.AudioDebugEntry> AudioDebugLog = new List<KFMODDebugger.AudioDebugEntry>();

	public Dictionary<KFMODDebugger.DebugSoundType, bool> allDebugSoundTypes = new Dictionary<KFMODDebugger.DebugSoundType, bool>();

	public bool debugEnabled = false;

	public struct AudioDebugEntry
	{
		public string log;

		public KFMODDebugger.DebugSoundType soundType;

		public float callTime;
	}

	public enum DebugSoundType
	{
		Uncategorized = -1,
		UI,
		Notifications,
		Buildings,
		DupeVoices,
		DupeMovement,
		DupeActions,
		Creatures,
		Plants,
		Ambience,
		Environment,
		FX,
		Music
	}
}
