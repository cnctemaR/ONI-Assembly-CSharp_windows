using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.Android
{
	public class DownloadAssetPackAsyncOperation : CustomYieldInstruction
	{
		public override bool keepWaiting
		{
			get
			{
				Dictionary<string, AndroidAssetPackInfo> assetPackInfos = this.m_AssetPackInfos;
				bool flag3;
				lock (assetPackInfos)
				{
					foreach (AndroidAssetPackInfo androidAssetPackInfo in this.m_AssetPackInfos.Values)
					{
						bool flag2 = androidAssetPackInfo == null;
						if (flag2)
						{
							return true;
						}
						bool downloadInProgress = androidAssetPackInfo.downloadInProgress;
						if (downloadInProgress)
						{
							return true;
						}
					}
					flag3 = false;
				}
				return flag3;
			}
		}

		public bool isDone
		{
			get
			{
				return !this.keepWaiting;
			}
		}

		public float progress
		{
			get
			{
				Dictionary<string, AndroidAssetPackInfo> assetPackInfos = this.m_AssetPackInfos;
				float num4;
				lock (assetPackInfos)
				{
					float num = 0f;
					float num2 = 0f;
					foreach (AndroidAssetPackInfo androidAssetPackInfo in this.m_AssetPackInfos.Values)
					{
						bool flag2 = androidAssetPackInfo == null;
						if (!flag2)
						{
							bool flag3 = !androidAssetPackInfo.downloadInProgress;
							if (flag3)
							{
								num += 1f;
								num2 += 1f;
							}
							else
							{
								double num3 = androidAssetPackInfo.bytesDownloaded / androidAssetPackInfo.size;
								num += (float)num3;
								num2 += androidAssetPackInfo.transferProgress;
							}
						}
					}
					num4 = Mathf.Clamp((num * 0.8f + num2 * 0.2f) / (float)this.m_AssetPackInfos.Count, 0f, 1f);
				}
				return num4;
			}
		}

		public string[] downloadedAssetPacks
		{
			get
			{
				Dictionary<string, AndroidAssetPackInfo> assetPackInfos = this.m_AssetPackInfos;
				string[] array;
				lock (assetPackInfos)
				{
					List<string> list = new List<string>();
					foreach (AndroidAssetPackInfo androidAssetPackInfo in this.m_AssetPackInfos.Values)
					{
						bool flag2 = androidAssetPackInfo == null;
						if (!flag2)
						{
							bool flag3 = androidAssetPackInfo.status == AndroidAssetPackStatus.Completed;
							if (flag3)
							{
								list.Add(androidAssetPackInfo.name);
							}
						}
					}
					array = list.ToArray();
				}
				return array;
			}
		}

		public string[] downloadFailedAssetPacks
		{
			get
			{
				Dictionary<string, AndroidAssetPackInfo> assetPackInfos = this.m_AssetPackInfos;
				string[] array;
				lock (assetPackInfos)
				{
					List<string> list = new List<string>();
					foreach (KeyValuePair<string, AndroidAssetPackInfo> keyValuePair in this.m_AssetPackInfos)
					{
						AndroidAssetPackInfo value = keyValuePair.Value;
						bool flag2 = value == null;
						if (flag2)
						{
							list.Add(keyValuePair.Key);
						}
						else
						{
							bool flag3 = value.status == AndroidAssetPackStatus.Canceled || value.status == AndroidAssetPackStatus.Failed || value.status == AndroidAssetPackStatus.Unknown;
							if (flag3)
							{
								list.Add(value.name);
							}
						}
					}
					array = list.ToArray();
				}
				return array;
			}
		}

		internal DownloadAssetPackAsyncOperation(string[] assetPackNames)
		{
			this.m_AssetPackInfos = assetPackNames.ToDictionary<string, string, AndroidAssetPackInfo>((string name) => name, (string name) => null);
		}

		internal void OnUpdate(AndroidAssetPackInfo info)
		{
			Dictionary<string, AndroidAssetPackInfo> assetPackInfos = this.m_AssetPackInfos;
			lock (assetPackInfos)
			{
				this.m_AssetPackInfos[info.name] = info;
			}
		}

		private Dictionary<string, AndroidAssetPackInfo> m_AssetPackInfos;
	}
}
