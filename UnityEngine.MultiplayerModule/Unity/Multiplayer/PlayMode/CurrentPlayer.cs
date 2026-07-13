using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Unity.Multiplayer.PlayMode
{
	[MovedFrom(true, "Unity.Multiplayer.Playmode", "Unity.Multiplayer.Playmode", null)]
	public static class CurrentPlayer
	{
		private static void EnsureInitialized()
		{
			bool flag = CurrentPlayer.s_CurrentPlayerApi != null;
			if (!flag)
			{
				CurrentPlayer.s_CurrentPlayerApi = new CurrentPlayerApi();
			}
		}

		public static bool IsMainEditor
		{
			get
			{
				CurrentPlayer.EnsureInitialized();
				return CurrentPlayer.s_CurrentPlayerApi.IsMainEditor;
			}
		}

		public static IReadOnlyList<string> Tags
		{
			get
			{
				CurrentPlayer.EnsureInitialized();
				return CurrentPlayer.s_CurrentPlayerApi.ReadOnlyTags();
			}
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void ReloadLatestTagsOnEnterPlaymode()
		{
			CurrentPlayer.s_CurrentPlayerApi = null;
		}

		internal static void ReportResult(bool condition, string message = "", [CallerFilePath] string callingFilePath = "", [CallerLineNumber] int lineNumber = 0)
		{
			CurrentPlayer.EnsureInitialized();
			CurrentPlayer.s_CurrentPlayerApi.ReportResult(condition, message, callingFilePath, lineNumber);
		}

		[Obsolete("ReadOnlyTags has been deprecated. Use CurrentPlayer.Tags which has better performance properties.", false)]
		public static string[] ReadOnlyTags()
		{
			IReadOnlyList<string> tags = CurrentPlayer.Tags;
			string[] array = new string[tags.Count];
			for (int i = 0; i < tags.Count; i++)
			{
				array[i] = tags[i];
			}
			return array;
		}

		private static CurrentPlayerApi s_CurrentPlayerApi;
	}
}
