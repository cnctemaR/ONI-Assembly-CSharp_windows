using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEditor.Analytics
{
	[ExcludeFromDocs]
	[RequiredByNativeCode(GenerateProxy = true)]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class AssetDatabaseRefreshAnalytic : AnalyticsEventBase
	{
		public AssetDatabaseRefreshAnalytic()
			: base("assetDatabaseInitRefresh", 1, SendEventOptions.kAppendNone, "")
		{
		}

		[RequiredByNativeCode]
		internal static AssetDatabaseRefreshAnalytic CreateAssetDatabaseRefreshAnalytic()
		{
			return new AssetDatabaseRefreshAnalytic();
		}

		[SerializeField]
		public bool isV2;

		[SerializeField]
		public long Imports_Imported;

		[SerializeField]
		public long Imports_ImportedInProcess;

		[SerializeField]
		public long Imports_ImportedOutOfProcess;

		[SerializeField]
		public long Imports_Refresh;

		[SerializeField]
		public long Imports_DomainReload;

		[SerializeField]
		public long CacheServer_MetadataRequested;

		[SerializeField]
		public long CacheServer_MetadataDownloaded;

		[SerializeField]
		public long CacheServer_MetadataFailedToDownload;

		[SerializeField]
		public long CacheServer_MetadataUploaded;

		[SerializeField]
		public long CacheServer_ArtifactsFailedToUpload;

		[SerializeField]
		public long CacheServer_MetadataVersionsDownloaded;

		[SerializeField]
		public long CacheServer_MetadataMatched;

		[SerializeField]
		public long CacheServer_ArtifactsDownloaded;

		[SerializeField]
		public long CacheServer_ArtifactFilesDownloaded;

		[SerializeField]
		public long CacheServer_ArtifactFilesFailedToDownload;

		[SerializeField]
		public long CacheServer_ArtifactsUploaded;

		[SerializeField]
		public long CacheServer_ArtifactFilesUploaded;

		[SerializeField]
		public long CacheServer_ArtifactFilesFailedToUpload;

		[SerializeField]
		public long CacheServer_Connects;

		[SerializeField]
		public long CacheServer_Disconnects;
	}
}
