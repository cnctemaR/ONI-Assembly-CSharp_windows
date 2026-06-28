using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	public sealed class WWW : IDisposable
	{
		public WWW(string url)
		{
			this.InitWWW(url, null, null);
		}

		public WWW(string url, WWWForm form)
		{
			string[] array = WWW.FlattenedHeadersFrom(form.headers);
			this.InitWWW(url, form.data, array);
		}

		public WWW(string url, byte[] postData)
		{
			this.InitWWW(url, postData, null);
		}

		[Obsolete("This overload is deprecated. Use UnityEngine.WWW.WWW(string, byte[], System.Collections.Generic.Dictionary<string, string>) instead.", true)]
		public WWW(string url, byte[] postData, Hashtable headers)
		{
			Debug.LogError("This overload is deprecated. Use UnityEngine.WWW.WWW(string, byte[], System.Collections.Generic.Dictionary<string, string>) instead");
		}

		public WWW(string url, byte[] postData, Dictionary<string, string> headers)
		{
			string[] array = WWW.FlattenedHeadersFrom(headers);
			this.InitWWW(url, postData, array);
		}

		internal WWW(string url, Hash128 hash, uint crc)
		{
			WWW.INTERNAL_CALL_WWW(this, url, ref hash, crc);
		}

		public void Dispose()
		{
			this.DestroyWWW(true);
		}

		~WWW()
		{
			this.DestroyWWW(false);
		}

		[ThreadAndSerializationSafe]
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void DestroyWWW(bool cancel);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitWWW(string url, byte[] postData, string[] iHeaders);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool enforceWebSecurityRestrictions();

		[ExcludeFromDocs]
		public static string EscapeURL(string s)
		{
			Encoding utf = Encoding.UTF8;
			return WWW.EscapeURL(s, utf);
		}

		public static string EscapeURL(string s, [DefaultValue("System.Text.Encoding.UTF8")] Encoding e)
		{
			string text;
			if (s == null)
			{
				text = null;
			}
			else if (s == "")
			{
				text = "";
			}
			else if (e == null)
			{
				text = null;
			}
			else
			{
				text = WWWTranscoder.URLEncode(s, e);
			}
			return text;
		}

		[ExcludeFromDocs]
		public static string UnEscapeURL(string s)
		{
			Encoding utf = Encoding.UTF8;
			return WWW.UnEscapeURL(s, utf);
		}

		public static string UnEscapeURL(string s, [DefaultValue("System.Text.Encoding.UTF8")] Encoding e)
		{
			string text;
			if (s == null)
			{
				text = null;
			}
			else if (s.IndexOf('%') == -1 && s.IndexOf('+') == -1)
			{
				text = s;
			}
			else
			{
				text = WWWTranscoder.URLDecode(s, e);
			}
			return text;
		}

		public Dictionary<string, string> responseHeaders
		{
			get
			{
				if (!this.isDone)
				{
					throw new UnityException("WWW is not finished downloading yet");
				}
				return WWW.ParseHTTPHeaderString(this.responseHeadersString);
			}
		}

		private extern string responseHeadersString
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public string text
		{
			get
			{
				if (!this.isDone)
				{
					throw new UnityException("WWW is not ready downloading yet");
				}
				byte[] bytes = this.bytes;
				return this.GetTextEncoder().GetString(bytes, 0, bytes.Length);
			}
		}

		internal static Encoding DefaultEncoding
		{
			get
			{
				return Encoding.ASCII;
			}
		}

		private Encoding GetTextEncoder()
		{
			string text = null;
			if (this.responseHeaders.TryGetValue("CONTENT-TYPE", out text))
			{
				int num = text.IndexOf("charset", StringComparison.OrdinalIgnoreCase);
				if (num > -1)
				{
					int num2 = text.IndexOf('=', num);
					if (num2 > -1)
					{
						string text2 = text.Substring(num2 + 1).Trim().Trim(new char[] { '\'', '"' })
							.Trim();
						int num3 = text2.IndexOf(';');
						if (num3 > -1)
						{
							text2 = text2.Substring(0, num3);
						}
						try
						{
							return Encoding.GetEncoding(text2);
						}
						catch (Exception)
						{
							Debug.Log("Unsupported encoding: '" + text2 + "'");
						}
					}
				}
			}
			return Encoding.UTF8;
		}

		[Obsolete("Please use WWW.text instead")]
		public string data
		{
			get
			{
				return this.text;
			}
		}

		public extern byte[] bytes
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int size
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern string error
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Texture2D GetTexture(bool markNonReadable);

		public Texture2D texture
		{
			get
			{
				return this.GetTexture(false);
			}
		}

		public Texture2D textureNonReadable
		{
			get
			{
				return this.GetTexture(true);
			}
		}

		[Obsolete("Obsolete msg (UnityUpgradable) -> * UnityEngine.WWWAudioExtensions.GetAudioClip(UnityEngine.WWW)", true)]
		public Object audioClip
		{
			get
			{
				return null;
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern Object GetAudioClipInternal(bool threeD, bool stream, bool compressed, AudioType audioType);

		[Obsolete("Obsolete msg (UnityUpgradable) -> * UnityEngine.WWWAudioExtensions.GetMovieTexture(UnityEngine.WWW)", true)]
		public Object movie
		{
			get
			{
				return null;
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern Object GetMovieTextureInternal();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void LoadImageIntoTexture(Texture2D tex);

		public extern bool isDone
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("All blocking WWW functions have been deprecated, please use one of the asynchronous functions instead.", true)]
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetURL(string url);

		[Obsolete("All blocking WWW functions have been deprecated, please use one of the asynchronous functions instead.", true)]
		public static Texture2D GetTextureFromURL(string url)
		{
			return new WWW(url).texture;
		}

		public extern float progress
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern float uploadProgress
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int bytesDownloaded
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public Object oggVorbis
		{
			get
			{
				return null;
			}
		}

		[Obsolete("LoadUnityWeb is no longer supported. Please use javascript to reload the web player on a different url instead", true)]
		public void LoadUnityWeb()
		{
		}

		public extern string url
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern AssetBundle assetBundle
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern ThreadPriority threadPriority
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_WWW(WWW self, string url, ref Hash128 hash, uint crc);

		[ExcludeFromDocs]
		public static WWW LoadFromCacheOrDownload(string url, int version)
		{
			uint num = 0U;
			return WWW.LoadFromCacheOrDownload(url, version, num);
		}

		public static WWW LoadFromCacheOrDownload(string url, int version, [DefaultValue("0")] uint crc)
		{
			Hash128 hash = new Hash128(0U, 0U, 0U, (uint)version);
			return WWW.LoadFromCacheOrDownload(url, hash, crc);
		}

		[ExcludeFromDocs]
		public static WWW LoadFromCacheOrDownload(string url, Hash128 hash)
		{
			uint num = 0U;
			return WWW.LoadFromCacheOrDownload(url, hash, num);
		}

		public static WWW LoadFromCacheOrDownload(string url, Hash128 hash, [DefaultValue("0")] uint crc)
		{
			return new WWW(url, hash, crc);
		}

		private static string[] FlattenedHeadersFrom(Dictionary<string, string> headers)
		{
			string[] array;
			if (headers == null)
			{
				array = null;
			}
			else
			{
				string[] array2 = new string[headers.Count * 2];
				int num = 0;
				foreach (KeyValuePair<string, string> keyValuePair in headers)
				{
					array2[num++] = keyValuePair.Key.ToString();
					array2[num++] = keyValuePair.Value.ToString();
				}
				array = array2;
			}
			return array;
		}

		internal static Dictionary<string, string> ParseHTTPHeaderString(string input)
		{
			if (input == null)
			{
				throw new ArgumentException("input was null to ParseHTTPHeaderString");
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			StringReader stringReader = new StringReader(input);
			int num = 0;
			for (;;)
			{
				string text = stringReader.ReadLine();
				if (text == null)
				{
					break;
				}
				if (num++ == 0 && text.StartsWith("HTTP"))
				{
					dictionary["STATUS"] = text;
				}
				else
				{
					int num2 = text.IndexOf(": ");
					if (num2 != -1)
					{
						string text2 = text.Substring(0, num2).ToUpper();
						string text3 = text.Substring(num2 + 2);
						string text4;
						if (dictionary.TryGetValue(text2, out text4))
						{
							text3 = text4 + "," + text3;
						}
						dictionary[text2] = text3;
					}
				}
			}
			return dictionary;
		}

		[RequiredByNativeCode]
		internal IntPtr m_Ptr;
	}
}
