using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.PlayerDataStorage
{
	public sealed class PlayerDataStorageInterface : Handle
	{
		public PlayerDataStorageInterface(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public void QueryFile(QueryFileOptions queryFileOptions, object clientData, OnQueryFileCompleteCallback completionCallback)
		{
			QueryFileOptionsInternal queryFileOptionsInternal = Helper.CopyProperties<QueryFileOptionsInternal>(queryFileOptions);
			OnQueryFileCompleteCallbackInternal onQueryFileCompleteCallbackInternal = new OnQueryFileCompleteCallbackInternal(PlayerDataStorageInterface.OnQueryFileComplete);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionCallback, onQueryFileCompleteCallbackInternal, Array.Empty<Delegate>());
			PlayerDataStorageInterface.EOS_PlayerDataStorage_QueryFile(base.InnerHandle, ref queryFileOptionsInternal, zero, onQueryFileCompleteCallbackInternal);
			Helper.TryMarshalDispose<QueryFileOptionsInternal>(ref queryFileOptionsInternal);
		}

		public void QueryFileList(QueryFileListOptions queryFileListOptions, object clientData, OnQueryFileListCompleteCallback completionCallback)
		{
			QueryFileListOptionsInternal queryFileListOptionsInternal = Helper.CopyProperties<QueryFileListOptionsInternal>(queryFileListOptions);
			OnQueryFileListCompleteCallbackInternal onQueryFileListCompleteCallbackInternal = new OnQueryFileListCompleteCallbackInternal(PlayerDataStorageInterface.OnQueryFileListComplete);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionCallback, onQueryFileListCompleteCallbackInternal, Array.Empty<Delegate>());
			PlayerDataStorageInterface.EOS_PlayerDataStorage_QueryFileList(base.InnerHandle, ref queryFileListOptionsInternal, zero, onQueryFileListCompleteCallbackInternal);
			Helper.TryMarshalDispose<QueryFileListOptionsInternal>(ref queryFileListOptionsInternal);
		}

		public Result CopyFileMetadataByFilename(CopyFileMetadataByFilenameOptions copyFileMetadataOptions, out FileMetadata outMetadata)
		{
			CopyFileMetadataByFilenameOptionsInternal copyFileMetadataByFilenameOptionsInternal = Helper.CopyProperties<CopyFileMetadataByFilenameOptionsInternal>(copyFileMetadataOptions);
			outMetadata = Helper.GetDefault<FileMetadata>();
			IntPtr zero = IntPtr.Zero;
			Result result = PlayerDataStorageInterface.EOS_PlayerDataStorage_CopyFileMetadataByFilename(base.InnerHandle, ref copyFileMetadataByFilenameOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CopyFileMetadataByFilenameOptionsInternal>(ref copyFileMetadataByFilenameOptionsInternal);
			if (Helper.TryMarshalGet<FileMetadataInternal, FileMetadata>(zero, out outMetadata))
			{
				PlayerDataStorageInterface.EOS_PlayerDataStorage_FileMetadata_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result GetFileMetadataCount(GetFileMetadataCountOptions getFileMetadataCountOptions, out int outFileMetadataCount)
		{
			GetFileMetadataCountOptionsInternal getFileMetadataCountOptionsInternal = Helper.CopyProperties<GetFileMetadataCountOptionsInternal>(getFileMetadataCountOptions);
			outFileMetadataCount = Helper.GetDefault<int>();
			Result result = PlayerDataStorageInterface.EOS_PlayerDataStorage_GetFileMetadataCount(base.InnerHandle, ref getFileMetadataCountOptionsInternal, ref outFileMetadataCount);
			Helper.TryMarshalDispose<GetFileMetadataCountOptionsInternal>(ref getFileMetadataCountOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result CopyFileMetadataAtIndex(CopyFileMetadataAtIndexOptions copyFileMetadataOptions, out FileMetadata outMetadata)
		{
			CopyFileMetadataAtIndexOptionsInternal copyFileMetadataAtIndexOptionsInternal = Helper.CopyProperties<CopyFileMetadataAtIndexOptionsInternal>(copyFileMetadataOptions);
			outMetadata = Helper.GetDefault<FileMetadata>();
			IntPtr zero = IntPtr.Zero;
			Result result = PlayerDataStorageInterface.EOS_PlayerDataStorage_CopyFileMetadataAtIndex(base.InnerHandle, ref copyFileMetadataAtIndexOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CopyFileMetadataAtIndexOptionsInternal>(ref copyFileMetadataAtIndexOptionsInternal);
			if (Helper.TryMarshalGet<FileMetadataInternal, FileMetadata>(zero, out outMetadata))
			{
				PlayerDataStorageInterface.EOS_PlayerDataStorage_FileMetadata_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public void DuplicateFile(DuplicateFileOptions duplicateOptions, object clientData, OnDuplicateFileCompleteCallback completionCallback)
		{
			DuplicateFileOptionsInternal duplicateFileOptionsInternal = Helper.CopyProperties<DuplicateFileOptionsInternal>(duplicateOptions);
			OnDuplicateFileCompleteCallbackInternal onDuplicateFileCompleteCallbackInternal = new OnDuplicateFileCompleteCallbackInternal(PlayerDataStorageInterface.OnDuplicateFileComplete);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionCallback, onDuplicateFileCompleteCallbackInternal, Array.Empty<Delegate>());
			PlayerDataStorageInterface.EOS_PlayerDataStorage_DuplicateFile(base.InnerHandle, ref duplicateFileOptionsInternal, zero, onDuplicateFileCompleteCallbackInternal);
			Helper.TryMarshalDispose<DuplicateFileOptionsInternal>(ref duplicateFileOptionsInternal);
		}

		public void DeleteFile(DeleteFileOptions deleteOptions, object clientData, OnDeleteFileCompleteCallback completionCallback)
		{
			DeleteFileOptionsInternal deleteFileOptionsInternal = Helper.CopyProperties<DeleteFileOptionsInternal>(deleteOptions);
			OnDeleteFileCompleteCallbackInternal onDeleteFileCompleteCallbackInternal = new OnDeleteFileCompleteCallbackInternal(PlayerDataStorageInterface.OnDeleteFileComplete);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionCallback, onDeleteFileCompleteCallbackInternal, Array.Empty<Delegate>());
			PlayerDataStorageInterface.EOS_PlayerDataStorage_DeleteFile(base.InnerHandle, ref deleteFileOptionsInternal, zero, onDeleteFileCompleteCallbackInternal);
			Helper.TryMarshalDispose<DeleteFileOptionsInternal>(ref deleteFileOptionsInternal);
		}

		public PlayerDataStorageFileTransferRequest ReadFile(ReadFileOptions readOptions, object clientData, OnReadFileCompleteCallback completionCallback)
		{
			ReadFileOptionsInternal readFileOptionsInternal = Helper.CopyProperties<ReadFileOptionsInternal>(readOptions);
			OnReadFileCompleteCallbackInternal onReadFileCompleteCallbackInternal = new OnReadFileCompleteCallbackInternal(PlayerDataStorageInterface.OnReadFileComplete);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionCallback, onReadFileCompleteCallbackInternal, new Delegate[] { readOptions.ReadFileDataCallback, readFileOptionsInternal.ReadFileDataCallback, readOptions.FileTransferProgressCallback, readFileOptionsInternal.FileTransferProgressCallback });
			IntPtr intPtr = PlayerDataStorageInterface.EOS_PlayerDataStorage_ReadFile(base.InnerHandle, ref readFileOptionsInternal, zero, onReadFileCompleteCallbackInternal);
			Helper.TryMarshalDispose<ReadFileOptionsInternal>(ref readFileOptionsInternal);
			PlayerDataStorageFileTransferRequest @default = Helper.GetDefault<PlayerDataStorageFileTransferRequest>();
			Helper.TryMarshalGet<PlayerDataStorageFileTransferRequest>(intPtr, out @default);
			return @default;
		}

		public PlayerDataStorageFileTransferRequest WriteFile(WriteFileOptions writeOptions, object clientData, OnWriteFileCompleteCallback completionCallback)
		{
			WriteFileOptionsInternal writeFileOptionsInternal = Helper.CopyProperties<WriteFileOptionsInternal>(writeOptions);
			OnWriteFileCompleteCallbackInternal onWriteFileCompleteCallbackInternal = new OnWriteFileCompleteCallbackInternal(PlayerDataStorageInterface.OnWriteFileComplete);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionCallback, onWriteFileCompleteCallbackInternal, new Delegate[] { writeOptions.WriteFileDataCallback, writeFileOptionsInternal.WriteFileDataCallback, writeOptions.FileTransferProgressCallback, writeFileOptionsInternal.FileTransferProgressCallback });
			IntPtr intPtr = PlayerDataStorageInterface.EOS_PlayerDataStorage_WriteFile(base.InnerHandle, ref writeFileOptionsInternal, zero, onWriteFileCompleteCallbackInternal);
			Helper.TryMarshalDispose<WriteFileOptionsInternal>(ref writeFileOptionsInternal);
			PlayerDataStorageFileTransferRequest @default = Helper.GetDefault<PlayerDataStorageFileTransferRequest>();
			Helper.TryMarshalGet<PlayerDataStorageFileTransferRequest>(intPtr, out @default);
			return @default;
		}

		[MonoPInvokeCallback]
		internal static WriteResult OnWriteFileData(IntPtr callbackInfoAddress, IntPtr outDataBuffer, ref uint outDataWritten)
		{
			OnWriteFileDataCallback onWriteFileDataCallback = null;
			WriteFileDataCallbackInfo writeFileDataCallbackInfo = null;
			if (Helper.TryGetAdditionalCallback<OnWriteFileDataCallback, WriteFileDataCallbackInfoInternal, WriteFileDataCallbackInfo>(callbackInfoAddress, out onWriteFileDataCallback, out writeFileDataCallbackInfo))
			{
				byte[] array = null;
				WriteResult writeResult = onWriteFileDataCallback(writeFileDataCallbackInfo, out array, out outDataWritten);
				Marshal.Copy(array, 0, outDataBuffer, (int)outDataWritten);
				return writeResult;
			}
			return Helper.GetDefault<WriteResult>();
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
		internal static void OnWriteFileComplete(IntPtr address)
		{
			OnWriteFileCompleteCallback onWriteFileCompleteCallback = null;
			WriteFileCallbackInfo writeFileCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnWriteFileCompleteCallback, WriteFileCallbackInfoInternal, WriteFileCallbackInfo>(address, out onWriteFileCompleteCallback, out writeFileCallbackInfo))
			{
				onWriteFileCompleteCallback(writeFileCallbackInfo);
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
		internal static void OnDeleteFileComplete(IntPtr address)
		{
			OnDeleteFileCompleteCallback onDeleteFileCompleteCallback = null;
			DeleteFileCallbackInfo deleteFileCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnDeleteFileCompleteCallback, DeleteFileCallbackInfoInternal, DeleteFileCallbackInfo>(address, out onDeleteFileCompleteCallback, out deleteFileCallbackInfo))
			{
				onDeleteFileCompleteCallback(deleteFileCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnDuplicateFileComplete(IntPtr address)
		{
			OnDuplicateFileCompleteCallback onDuplicateFileCompleteCallback = null;
			DuplicateFileCallbackInfo duplicateFileCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnDuplicateFileCompleteCallback, DuplicateFileCallbackInfoInternal, DuplicateFileCallbackInfo>(address, out onDuplicateFileCompleteCallback, out duplicateFileCallbackInfo))
			{
				onDuplicateFileCompleteCallback(duplicateFileCallbackInfo);
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
		private static extern void EOS_PlayerDataStorage_FileMetadata_Release(IntPtr fileMetadata);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern IntPtr EOS_PlayerDataStorage_WriteFile(IntPtr handle, ref WriteFileOptionsInternal writeOptions, IntPtr clientData, OnWriteFileCompleteCallbackInternal completionCallback);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern IntPtr EOS_PlayerDataStorage_ReadFile(IntPtr handle, ref ReadFileOptionsInternal readOptions, IntPtr clientData, OnReadFileCompleteCallbackInternal completionCallback);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_PlayerDataStorage_DeleteFile(IntPtr handle, ref DeleteFileOptionsInternal deleteOptions, IntPtr clientData, OnDeleteFileCompleteCallbackInternal completionCallback);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_PlayerDataStorage_DuplicateFile(IntPtr handle, ref DuplicateFileOptionsInternal duplicateOptions, IntPtr clientData, OnDuplicateFileCompleteCallbackInternal completionCallback);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_PlayerDataStorage_CopyFileMetadataAtIndex(IntPtr handle, ref CopyFileMetadataAtIndexOptionsInternal copyFileMetadataOptions, ref IntPtr outMetadata);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_PlayerDataStorage_GetFileMetadataCount(IntPtr handle, ref GetFileMetadataCountOptionsInternal getFileMetadataCountOptions, ref int outFileMetadataCount);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_PlayerDataStorage_CopyFileMetadataByFilename(IntPtr handle, ref CopyFileMetadataByFilenameOptionsInternal copyFileMetadataOptions, ref IntPtr outMetadata);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_PlayerDataStorage_QueryFileList(IntPtr handle, ref QueryFileListOptionsInternal queryFileListOptions, IntPtr clientData, OnQueryFileListCompleteCallbackInternal completionCallback);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_PlayerDataStorage_QueryFile(IntPtr handle, ref QueryFileOptionsInternal queryFileOptions, IntPtr clientData, OnQueryFileCompleteCallbackInternal completionCallback);

		public const int WritefileoptionsApiLatest = 1;

		public const int ReadfileoptionsApiLatest = 1;

		public const int DeletefileoptionsApiLatest = 1;

		public const int DuplicatefileoptionsApiLatest = 1;

		public const int CopyfilemetadatabyfilenameoptionsApiLatest = 1;

		public const int CopyfilemetadataatindexoptionsApiLatest = 1;

		public const int GetfilemetadatacountoptionsApiLatest = 1;

		public const int QueryfilelistoptionsApiLatest = 1;

		public const int QueryfileoptionsApiLatest = 1;

		public const int FilemetadataApiLatest = 1;

		public const int FileMaxSizeBytes = 67108864;

		public const int FilenameMaxLengthBytes = 64;
	}
}
