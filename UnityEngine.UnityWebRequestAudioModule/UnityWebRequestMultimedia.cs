using System;

namespace UnityEngine.Networking
{
	/// <summary>
	///   <para>Helpers for downloading multimedia files using UnityWebRequest.</para>
	/// </summary>
	public static class UnityWebRequestMultimedia
	{
		/// <summary>
		///   <para>Create a UnityWebRequest to download an audio clip via HTTP GET and create an AudioClip based on the retrieved data.</para>
		/// </summary>
		/// <param name="uri">The URI of the audio clip to download.</param>
		/// <param name="audioType">The type of audio encoding for the downloaded audio clip. See AudioType.</param>
		/// <returns>
		///   <para>A UnityWebRequest properly configured to download an audio clip and convert it to an AudioClip.</para>
		/// </returns>
		public static UnityWebRequest GetAudioClip(string uri, AudioType audioType)
		{
			return new UnityWebRequest(uri, "GET", new DownloadHandlerAudioClip(uri, audioType), null);
		}

		public static UnityWebRequest GetAudioClip(Uri uri, AudioType audioType)
		{
			return new UnityWebRequest(uri, "GET", new DownloadHandlerAudioClip(uri, audioType), null);
		}

		/// <summary>
		///   <para>Create a UnityWebRequest intended to download a movie clip via HTTP GET and create an MovieTexture based on the retrieved data.</para>
		/// </summary>
		/// <param name="uri">The URI of the movie clip to download.</param>
		/// <returns>
		///   <para>A UnityWebRequest properly configured to download a movie clip and convert it to a MovieTexture.</para>
		/// </returns>
		[Obsolete("MovieTexture is deprecated. Use VideoPlayer instead.", false)]
		public static UnityWebRequest GetMovieTexture(string uri)
		{
			return new UnityWebRequest(uri, "GET", new DownloadHandlerMovieTexture(), null);
		}

		[Obsolete("MovieTexture is deprecated. Use VideoPlayer instead.", false)]
		public static UnityWebRequest GetMovieTexture(Uri uri)
		{
			return new UnityWebRequest(uri, "GET", new DownloadHandlerMovieTexture(), null);
		}
	}
}
