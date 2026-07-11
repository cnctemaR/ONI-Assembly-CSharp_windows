using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Data structure for downloading AssetBundles to a customized cache path. See Also:UnityWebRequestAssetBundle.GetAssetBundle for more information.</para>
	/// </summary>
	[UsedByNativeCode]
	public struct CachedAssetBundle
	{
		public CachedAssetBundle(string name, Hash128 hash)
		{
			this.m_Name = name;
			this.m_Hash = hash;
		}

		/// <summary>
		///   <para>AssetBundle name which is used as the customized cache path.</para>
		/// </summary>
		public string name
		{
			get
			{
				return this.m_Name;
			}
			set
			{
				this.m_Name = value;
			}
		}

		/// <summary>
		///   <para>Hash128 which is used as the version of the AssetBundle.</para>
		/// </summary>
		public Hash128 hash
		{
			get
			{
				return this.m_Hash;
			}
			set
			{
				this.m_Hash = value;
			}
		}

		private string m_Name;

		private Hash128 m_Hash;
	}
}
