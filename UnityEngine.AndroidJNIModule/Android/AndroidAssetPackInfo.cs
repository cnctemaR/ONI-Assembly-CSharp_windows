using System;

namespace UnityEngine.Android
{
	public class AndroidAssetPackInfo
	{
		internal AndroidAssetPackInfo(string name, AndroidAssetPackStatus status, ulong size, ulong bytesDownloaded, float transferProgress, AndroidAssetPackError error)
		{
			this.name = name;
			this.status = status;
			this.size = size;
			this.bytesDownloaded = bytesDownloaded;
			this.transferProgress = transferProgress;
			this.error = error;
		}

		public string name { get; }

		public AndroidAssetPackStatus status { get; }

		public ulong size { get; }

		public ulong bytesDownloaded { get; }

		public float transferProgress { get; }

		public AndroidAssetPackError error { get; }

		internal bool downloadInProgress
		{
			get
			{
				return AndroidAssetPackInfo.DownloadInProgress(this.status);
			}
		}

		internal static bool DownloadInProgress(AndroidAssetPackStatus status)
		{
			return status != AndroidAssetPackStatus.Canceled && status != AndroidAssetPackStatus.Completed && status != AndroidAssetPackStatus.Failed && status > AndroidAssetPackStatus.Unknown;
		}
	}
}
