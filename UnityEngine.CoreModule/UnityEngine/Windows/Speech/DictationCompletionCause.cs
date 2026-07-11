using System;

namespace UnityEngine.Windows.Speech
{
	/// <summary>
	///   <para>Represents the reason why dictation session has completed.</para>
	/// </summary>
	public enum DictationCompletionCause
	{
		/// <summary>
		///   <para>Dictation session has completed successfully.</para>
		/// </summary>
		Complete,
		/// <summary>
		///   <para>Dictation session completion was caused by bad audio quality.</para>
		/// </summary>
		AudioQualityFailure,
		/// <summary>
		///   <para>Dictation session was either cancelled, or the application was paused while dictation session was in progress.</para>
		/// </summary>
		Canceled,
		/// <summary>
		///   <para>Dictation session has reached its timeout.</para>
		/// </summary>
		TimeoutExceeded,
		PauseLimitExceeded,
		/// <summary>
		///   <para>Dictation session has finished because network connection was not available.</para>
		/// </summary>
		NetworkFailure,
		/// <summary>
		///   <para>Dictation session has finished because a microphone was not available.</para>
		/// </summary>
		MicrophoneUnavailable,
		/// <summary>
		///   <para>Dictation session has completed due to an unknown error.</para>
		/// </summary>
		UnknownError
	}
}
