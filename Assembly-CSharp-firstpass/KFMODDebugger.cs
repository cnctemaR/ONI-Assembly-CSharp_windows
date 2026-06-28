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
		if (s.Contains("Buildings"))
		{
			return KFMODDebugger.DebugSoundType.Buildings;
		}
		if (s.Contains("Notifications"))
		{
			return KFMODDebugger.DebugSoundType.Notifications;
		}
		if (s.Contains("UI"))
		{
			return KFMODDebugger.DebugSoundType.UI;
		}
		if (s.Contains("Creatures"))
		{
			return KFMODDebugger.DebugSoundType.Creatures;
		}
		if (s.Contains("Duplicant_voices"))
		{
			return KFMODDebugger.DebugSoundType.DupeVoices;
		}
		if (s.Contains("Ambience"))
		{
			return KFMODDebugger.DebugSoundType.Ambience;
		}
		if (s.Contains("Environment"))
		{
			return KFMODDebugger.DebugSoundType.Environment;
		}
		if (s.Contains("FX"))
		{
			return KFMODDebugger.DebugSoundType.FX;
		}
		if (s.Contains("Duplicant_actions/LowImportance/Movement"))
		{
			return KFMODDebugger.DebugSoundType.DupeMovement;
		}
		if (s.Contains("Duplicant_actions"))
		{
			return KFMODDebugger.DebugSoundType.DupeActions;
		}
		if (s.Contains("Plants"))
		{
			return KFMODDebugger.DebugSoundType.Plants;
		}
		if (s.Contains("Music"))
		{
			return KFMODDebugger.DebugSoundType.Music;
		}
		return KFMODDebugger.DebugSoundType.Uncategorized;
	}

	public static KFMODDebugger instance;

	public List<KFMODDebugger.AudioDebugEntry> AudioDebugLog = new List<KFMODDebugger.AudioDebugEntry>();

	public Dictionary<KFMODDebugger.DebugSoundType, bool> allDebugSoundTypes = new Dictionary<KFMODDebugger.DebugSoundType, bool>();

	public bool debugEnabled;

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
