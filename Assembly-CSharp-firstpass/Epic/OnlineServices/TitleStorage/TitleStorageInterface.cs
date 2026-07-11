using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.TitleStorage
{
	public sealed class TitleStorageInterface : Handle
	{
		public TitleStorageInterface(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public void QueryFile(QueryFileOptions options, object clientData, OnQueryFileCompleteCallback completionCallback)
		{
			QueryFileOptionsInternal queryFileOptionsInternal = Helper.CopyProperties<QueryFileOptionsInternal>(options);
			OnQueryFileCompleteCallbackInternal onQueryFileCompleteCallbackInternal = new OnQueryFileCompleteCallbackInternal(TitleStorageInterface.OnQueryFileComplete);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionCallback, onQueryFileCompleteCallbackInternal, Array.Empty<Delegate>());
			TitleStorageInterface.EOS_TitleStorage_QueryFile(base.InnerHandle, ref queryFileOptionsInternal, zero, onQueryFileCompleteCallbackInternal);
			Helper.TryMarshalDispose<QueryFileOptionsInternal>(ref queryFileOptionsInternal);
		}

		public void QueryFileList(QueryFileListOptions options, object clientData, OnQueryFileListCompleteCallback completionCallback)
		{
			QueryFileListOptionsInternal queryFileListOptionsInternal = Helper.CopyProperties<QueryFileListOptionsInternal>(options);
			OnQueryFileListCompleteCallbackInternal onQueryFileListCompleteCallbackInternal = new OnQueryFileListCompleteCallbackInternal(TitleStorageInterface.OnQueryFileListComplete);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionCallback, onQueryFileListCompleteCallbackInternal, Array.Empty<Delegate>());
			TitleStorageInterface.EOS_TitleStorage_QueryFileList(base.InnerHandle, ref queryFileListOptionsInternal, zero, onQueryFileListCompleteCallbackInternal);
			Helper.TryMarshalDispose<QueryFileListOptionsInternal>(ref queryFileListOptionsInternal);
		}

		public Result CopyFileMetadataByFilename(CopyFileMetadataByFilenameOptions options, out FileMetadata outMetadata)
		{
			CopyFileMetadataByFilenameOptionsInternal copyFileMetadataByFilenameOptionsInternal = Helper.CopyProperties<CopyFileMetadataByFilenameOptionsInternal>(options);
			outMetadata = Helper.GetDefault<FileMetadata>();
			IntPtr zero = IntPtr.Zero;
			Result result = TitleStorageInterface.EOS_TitleStorage_CopyFileMetadataByFilename(base.InnerHandle, ref copyFileMetadataByFilenameOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CopyFileMetadataByFilenameOptionsInternal>(ref copyFileMetadataByFilenameOptionsInternal);
			if (Helper.TryMarshalGet<FileMetadataInternal, FileMetadata>(zero, out outMetadata))
			{
				TitleStorageInterface.EOS_TitleStorage_FileMetadata_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public uint GetFileMetadataCount(GetFileMetadataCountOptions options)
		{
			GetFileMetadataCountOptionsInternal getFileMetadataCountOptionsInternal = Helper.CopyProperties<GetFileMetadataCountOptionsInternal>(options);
			uint num = TitleStorageInterface.EOS_TitleStorage_GetFileMetadataCount(base.InnerHandle, ref getFileMetadataCountOptionsInternal);
			Helper.TryMarshalDispose<GetFileMetadataCountOptionsInternal>(ref getFileMetadataCountOptionsInternal);
			uint @default = Helper.GetDefault<uint>();
			Helper.TryMarshalGet<uint>(num, out @default);
			return @default;
		}

		public Result CopyFileMetadataAtIndex(CopyFileMetadataAtIndexOptions options, out FileMetadata outMetadata)
		{
			CopyFileMetadataAtIndexOptionsInternal copyFileMetadataAtIndexOptionsInternal = Helper.CopyProperties<CopyFileMetadataAtIndexOptionsInternal>(options);
			outMetadata = Helper.GetDefault<FileMetadata>();
			IntPtr zero = IntPtr.Zero;
			Result result = TitleStorageInterface.EOS_TitleStorage_CopyFileMetadataAtIndex(base.InnerHandle, ref copyFileMetadataAtIndexOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CopyFileMetadataAtIndexOptionsInternal>(ref copyFileMetadataAtIndexOptionsInternal);
			if (Helper.TryMarshalGet<FileMetadataInternal, FileMetadata>(zero, out outMetadata))
			{
				TitleStorageInterface.EOS_TitleStorage_FileMetadata_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public TitleStorageFileTransferRequest ReadFile(ReadFileOptions options, object clientData, OnReadFileCompleteCallback completionCallback)
		{
			ReadFileOptionsInternal readFileOptionsInternal = Helper.CopyProperties<ReadFileOptionsInternal>(options);
			OnReadFileCompleteCallbackInternal onReadFileCompleteCallbackInternal = new OnReadFileCompleteCallbackInternal(TitleStorageInterface.OnReadFileComplete);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionCallback, onReadFileCompleteCallbackInternal, new Delegate[] { options.ReadFileDataCallback, readFileOptionsInternal.ReadFileDataCallback, options.FileTransferProgressCallback, readFileOptionsInternal.FileTransferProgressCallback });
			IntPtr intPtr = TitleStorageInterface.EOS_TitleStorage_ReadFile(base.InnerHandle, ref readFileOptionsInternal, zero, onReadFileCompleteCallbackInternal);
			Helper.TryMarshalDispose<ReadFileOptionsInternal>(ref readFileOptionsInternal);
			TitleStorageFileTransferRequest @default = Helper.GetDefault<TitleStorageFileTransferRequest>();
			Helper.TryMarshalGet<TitleStorageFileTransferRequest>(intPtr, out @default);
			return @default;
		}

		public Result DeleteCache(DeleteCacheOptions options, object clientData, OnDeleteCacheCompleteCallback completionCallback)
		{
			DeleteCacheOptionsInternal deleteCacheOptionsInternal = Helper.CopyProperties<DeleteCacheOptionsInternal>(options);
			OnDeleteCacheCompleteCallbackInternal onDeleteCacheCompleteCallbackInternal = new OnDeleteCacheCompleteCallbackInternal(TitleStorageInterface.OnDeleteCacheComplete);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionCallback, onDeleteCacheCompleteCallbackInternal, Array.Empty<Delegate>());
			Result result = TitleStorageInterface.EOS_TitleStorage_DeleteCache(base.InnerHandle, ref deleteCacheOptionsInternal, zero, onDeleteCacheCompleteCallbackInternal);
			Helper.TryMarshalDispose<DeleteCacheOptionsInternal>(ref deleteCacheOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		[MonoPInvokeCallback]
		internal static void OnFileTransferProgress(IntPtr callbackInfoAddress)
		{
			OnFileTransferProgressCallback onFileTransferProgressCallback = null;
			FileTransferProgressCallbackInfo fileTransferProgressCallbackInfo = null;
			if (Helper.TryGetAdditionalCallback<OnFileTransferProgressCallback, FileTransferProgressCallbackInfoInternal, FileTransferProgressCallbackInfo>(callbackInfoAddress, out onFileTransferProgressCallback, out fileTransferProgressCallbackInfo))
			{
				onFileTransferProgressCallback(fileTransferProgressCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static ReadResult OnReadFileData(IntPtr callbackInfoAddress)
		{
			OnReadFileDataCallback onReadFileDataCallback = null;
			ReadFileDataCallbackInfo readFileDataCallbackInfo = null;
			if (Helper.TryGetAdditionalCallback<OnReadFileDataCallback, ReadFileDataCallbackInfoInternal, ReadFileDataCallbackInfo>(callbackInfoAddress, out onReadFileDataCallback, out readFileDataCallbackInfo))
			{
				return onReadFileDataCallback(readFileDataCallbackInfo);
			}
			return Helper.GetDefault<ReadResult>();
		}

		[MonoPInvokeCallback]
		internal static void OnDeleteCacheComplete(IntPtr address)
		{
			OnDeleteCacheCompleteCallback onDeleteCacheCompleteCallback = null;
			DeleteCacheCallbackInfo deleteCacheCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnDeleteCacheCompleteCallback, DeleteCacheCallbackInfoInternal, DeleteCacheCallbackInfo>(address, out onDeleteCacheCompleteCallback, out deleteCacheCallbackInfo))
			{
				onDeleteCacheCompleteCallback(deleteCacheCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnReadFileComplete(IntPtr address)
		{
			OnReadFileCompleteCallback onReadFileCompleteCallback = null;
			ReadFileCallbackInfo readFileCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnReadFileCompleteCallback, ReadFileCallbackInfoInternal, ReadFileCallbackInfo>(address, out onReadFileCompleteCallback, out readFileCallbackInfo))
			{
				onReadFileCompleteCallback(readFileCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnQueryFileListComplete(IntPtr address)
		{
			OnQueryFileListCompleteCallback onQueryFileListCompleteCallback = null;
			QueryFileListCallbackInfo queryFileListCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnQueryFileListCompleteCallback, QueryFileListCallbackInfoInternal, QueryFileListCallbackInfo>(address, out onQueryFileListCompleteCallback, out queryFileListCallbackInfo))
			{
				onQueryFileListCompleteCallback(queryFileListCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnQueryFileComplete(IntPtr address)
		{
			OnQueryFileCompleteCallback onQueryFileCompleteCallback = null;
			QueryFileCallbackInfo queryFileCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnQueryFileCompleteCallback, QueryFileCallbackInfoInternal, QueryFileCallbackInfo>(address, out onQueryFileCompleteCallback, out queryFileCallbackInfo))
			{
				onQueryFileCompleteCallback(queryFileCallbackInfo);
			}
		}

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_TitleStorage_FileMetadata_Release(IntPtr fileMetadata);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_TitleStorage_DeleteCache(IntPtr handle, ref DeleteCacheOptionsInternal options, IntPtr clientData, OnDeleteCacheCompleteCallbackInternal completionCallback);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern IntPtr EOS_TitleStorage_ReadFile(IntPtr handle, ref ReadFileOptionsInternal options, IntPtr clientData, OnReadFileCompleteCallbackInternal completionCallback);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_TitleStorage_CopyFileMetadataAtIndex(IntPtr handle, ref CopyFileMetadataAtIndexOptionsInternal options, ref IntPtr outMetadata);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern uint EOS_TitleStorage_GetFileMetadataCount(IntPtr handle, ref GetFileMetadataCountOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_TitleStorage_CopyFileMetadataByFilename(IntPtr handle, ref CopyFileMetadataByFilenameOptionsInternal options, ref IntPtr outMetadata);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_TitleStorage_QueryFileList(IntPtr handle, ref QueryFileListOptionsInternal options, IntPtr clientData, OnQueryFileListCompleteCallbackInternal completionCallback);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_TitleStorage_QueryFile(IntPtr handle, ref QueryFileOptionsInternal options, IntPtr clientData, OnQueryFileCompleteCallbackInternal completionCallback);

		public const int DeletecacheoptionsApiLatest = 1;

		public const int ReadfileoptionsApiLatest = 1;

		public const int CopyfilemetadatabyfilenameoptionsApiLatest = 1;

		public const int CopyfilemetadataatindexoptionsApiLatest = 1;

		public const int GetfilemetadatacountoptionsApiLatest = 1;

		public const int QueryfilelistoptionsApiLatest = 1;

		public const int QueryfileoptionsApiLatest = 1;

		public const int FilemetadataApiLatest = 1;

		public const int FilenameMaxLengthBytes = 64;
	}
}
