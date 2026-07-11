using System;

namespace UnityEngine.Windows.Speech
{
	/// <summary>
	///   <para>Represents the current status of the speech recognition system or a dictation recognizer.</para>
	/// </summary>
	public enum SpeechSystemStatus
	{
		/// <summary>
		///   <para>Speech recognition system is stopped.</para>
		/// </summary>
		Stopped,
		/// <summary>
		///   <para>Speech recognition system is running.</para>
		/// </summary>
		Running,
		/// <summary>
		///   <para>Speech recognition system has encountered an error and is in an indeterminate state.</para>
		/// </summary>
		Failed
	}
}
