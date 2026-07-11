using System;

namespace UnityEngine.Windows.Speech
{
	/// <summary>
	///   <para>Represents an error in a speech recognition system.</para>
	/// </summary>
	public enum SpeechError
	{
		/// <summary>
		///   <para>No error occurred.</para>
		/// </summary>
		NoError,
		/// <summary>
		///   <para>Supplied grammar file language is not supported.</para>
		/// </summary>
		TopicLanguageNotSupported,
		GrammarLanguageMismatch,
		/// <summary>
		///   <para>Speech recognition engine failed to compiled specified grammar.</para>
		/// </summary>
		GrammarCompilationFailure,
		/// <summary>
		///   <para>Speech recognition engine failed because the audio quality was too low.</para>
		/// </summary>
		AudioQualityFailure,
		PauseLimitExceeded,
		/// <summary>
		///   <para>A speech recognition system has timed out.</para>
		/// </summary>
		TimeoutExceeded,
		/// <summary>
		///   <para>Speech error occurred due to a network failure.</para>
		/// </summary>
		NetworkFailure,
		/// <summary>
		///   <para>Speech error occurred because a microphone was not available.</para>
		/// </summary>
		MicrophoneUnavailable,
		/// <summary>
		///   <para>A speech recognition system has encountered an unknown error.</para>
		/// </summary>
		UnknownError
	}
}
