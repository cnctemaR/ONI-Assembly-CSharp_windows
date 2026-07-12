using System;
using System.Collections.Generic;

namespace rail
{
	public class IRailSpaceWorkImpl : RailObject, IRailSpaceWork, IRailComponent
	{
		internal IRailSpaceWorkImpl(IntPtr cPtr)
		{
			this.swigCPtr_ = cPtr;
		}

		~IRailSpaceWorkImpl()
		{
		}

		public virtual void Close()
		{
			RAIL_API_PINVOKE.IRailSpaceWork_Close(this.swigCPtr_);
		}

		public virtual SpaceWorkID GetSpaceWorkID()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailSpaceWork_GetSpaceWorkID(this.swigCPtr_);
			SpaceWorkID spaceWorkID = new SpaceWorkID();
			RailConverter.Cpp2Csharp(intPtr, spaceWorkID);
			return spaceWorkID;
		}

		public virtual bool Editable()
		{
			return RAIL_API_PINVOKE.IRailSpaceWork_Editable(this.swigCPtr_);
		}

		public virtual RailResult StartSync(string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_StartSync(this.swigCPtr_, user_data);
		}

		public virtual RailResult GetSyncProgress(RailSpaceWorkSyncProgress progress)
		{
			IntPtr intPtr = ((progress == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailSpaceWorkSyncProgress__SWIG_0());
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_GetSyncProgress(this.swigCPtr_, intPtr);
			}
			finally
			{
				if (progress != null)
				{
					RailConverter.Cpp2Csharp(intPtr, progress);
				}
				RAIL_API_PINVOKE.delete_RailSpaceWorkSyncProgress(intPtr);
			}
			return railResult;
		}

		public virtual RailResult CancelSync()
		{
			return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_CancelSync(this.swigCPtr_);
		}

		public virtual RailResult GetWorkLocalFolder(out string path)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_GetWorkLocalFolder(this.swigCPtr_, intPtr);
			}
			finally
			{
				path = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
			return railResult;
		}

		public virtual RailResult AsyncUpdateMetadata(string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_AsyncUpdateMetadata(this.swigCPtr_, user_data);
		}

		public virtual RailResult GetName(out string name)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_GetName(this.swigCPtr_, intPtr);
			}
			finally
			{
				name = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
			return railResult;
		}

		public virtual RailResult GetDescription(out string description)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_GetDescription(this.swigCPtr_, intPtr);
			}
			finally
			{
				description = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
			return railResult;
		}

		public virtual RailResult GetUrl(out string url)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_GetUrl(this.swigCPtr_, intPtr);
			}
			finally
			{
				url = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
			return railResult;
		}

		public virtual uint GetCreateTime()
		{
			return RAIL_API_PINVOKE.IRailSpaceWork_GetCreateTime(this.swigCPtr_);
		}

		public virtual uint GetLastUpdateTime()
		{
			return RAIL_API_PINVOKE.IRailSpaceWork_GetLastUpdateTime(this.swigCPtr_);
		}

		public virtual ulong GetWorkFileSize()
		{
			return RAIL_API_PINVOKE.IRailSpaceWork_GetWorkFileSize(this.swigCPtr_);
		}

		public virtual RailResult GetTags(List<string> tags)
		{
			IntPtr intPtr = ((tags == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailString__SWIG_0());
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_GetTags(this.swigCPtr_, intPtr);
			}
			finally
			{
				if (tags != null)
				{
					RailConverter.Cpp2Csharp(intPtr, tags);
				}
				RAIL_API_PINVOKE.delete_RailArrayRailString(intPtr);
			}
			return railResult;
		}

		public virtual RailResult GetPreviewImage(out string path)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_GetPreviewImage(this.swigCPtr_, intPtr);
			}
			finally
			{
				path = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
			return railResult;
		}

		public virtual RailResult GetVersion(out string version)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_GetVersion(this.swigCPtr_, intPtr);
			}
			finally
			{
				version = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
			return railResult;
		}

		public virtual ulong GetDownloadCount()
		{
			return RAIL_API_PINVOKE.IRailSpaceWork_GetDownloadCount(this.swigCPtr_);
		}

		public virtual ulong GetSubscribedCount()
		{
			return RAIL_API_PINVOKE.IRailSpaceWork_GetSubscribedCount(this.swigCPtr_);
		}

		public virtual EnumRailSpaceWorkShareLevel GetShareLevel()
		{
			return (EnumRailSpaceWorkShareLevel)RAIL_API_PINVOKE.IRailSpaceWork_GetShareLevel(this.swigCPtr_);
		}

		public virtual ulong GetScore()
		{
			return RAIL_API_PINVOKE.IRailSpaceWork_GetScore(this.swigCPtr_);
		}

		public virtual RailResult GetMetadata(string key, out string value)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_GetMetadata(this.swigCPtr_, key, intPtr);
			}
			finally
			{
				value = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
			return railResult;
		}

		public virtual EnumRailSpaceWorkRateValue GetMyVote()
		{
			return (EnumRailSpaceWorkRateValue)RAIL_API_PINVOKE.IRailSpaceWork_GetMyVote(this.swigCPtr_);
		}

		public virtual bool IsFavorite()
		{
			return RAIL_API_PINVOKE.IRailSpaceWork_IsFavorite(this.swigCPtr_);
		}

		public virtual bool IsSubscribed()
		{
			return RAIL_API_PINVOKE.IRailSpaceWork_IsSubscribed(this.swigCPtr_);
		}

		public virtual RailResult SetName(string name)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_SetName(this.swigCPtr_, name);
		}

		public virtual RailResult SetDescription(string description)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_SetDescription(this.swigCPtr_, description);
		}

		public virtual RailResult SetTags(List<string> tags)
		{
			IntPtr intPtr = ((tags == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailString__SWIG_0());
			if (tags != null)
			{
				RailConverter.Csharp2Cpp(tags, intPtr);
			}
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_SetTags(this.swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayRailString(intPtr);
			}
			return railResult;
		}

		public virtual RailResult SetPreviewImage(string path_filename)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_SetPreviewImage(this.swigCPtr_, path_filename);
		}

		public virtual RailResult SetVersion(string version)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_SetVersion(this.swigCPtr_, version);
		}

		public virtual RailResult SetShareLevel(EnumRailSpaceWorkShareLevel level)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_SetShareLevel__SWIG_0(this.swigCPtr_, (int)level);
		}

		public virtual RailResult SetShareLevel()
		{
			return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_SetShareLevel__SWIG_1(this.swigCPtr_);
		}

		public virtual RailResult SetMetadata(string key, string value)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_SetMetadata(this.swigCPtr_, key, value);
		}

		public virtual RailResult SetContentFromFolder(string path)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_SetContentFromFolder(this.swigCPtr_, path);
		}

		public virtual RailResult GetAllMetadata(List<RailKeyValue> metadata)
		{
			IntPtr intPtr = ((metadata == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailKeyValue__SWIG_0());
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_GetAllMetadata(this.swigCPtr_, intPtr);
			}
			finally
			{
				if (metadata != null)
				{
					RailConverter.Cpp2Csharp(intPtr, metadata);
				}
				RAIL_API_PINVOKE.delete_RailArrayRailKeyValue(intPtr);
			}
			return railResult;
		}

		public virtual RailResult GetAdditionalPreviewUrls(List<string> preview_urls)
		{
			IntPtr intPtr = ((preview_urls == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailString__SWIG_0());
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_GetAdditionalPreviewUrls(this.swigCPtr_, intPtr);
			}
			finally
			{
				if (preview_urls != null)
				{
					RailConverter.Cpp2Csharp(intPtr, preview_urls);
				}
				RAIL_API_PINVOKE.delete_RailArrayRailString(intPtr);
			}
			return railResult;
		}

		public virtual RailResult GetAssociatedSpaceWorks(List<SpaceWorkID> ids)
		{
			IntPtr intPtr = ((ids == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArraySpaceWorkID__SWIG_0());
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_GetAssociatedSpaceWorks(this.swigCPtr_, intPtr);
			}
			finally
			{
				if (ids != null)
				{
					RailConverter.Cpp2Csharp(intPtr, ids);
				}
				RAIL_API_PINVOKE.delete_RailArraySpaceWorkID(intPtr);
			}
			return railResult;
		}

		public virtual RailResult GetLanguages(List<string> languages)
		{
			IntPtr intPtr = ((languages == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailString__SWIG_0());
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_GetLanguages(this.swigCPtr_, intPtr);
			}
			finally
			{
				if (languages != null)
				{
					RailConverter.Cpp2Csharp(intPtr, languages);
				}
				RAIL_API_PINVOKE.delete_RailArrayRailString(intPtr);
			}
			return railResult;
		}

		public virtual RailResult RemoveMetadata(string key)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_RemoveMetadata(this.swigCPtr_, key);
		}

		public virtual RailResult SetAdditionalPreviews(List<string> local_paths)
		{
			IntPtr intPtr = ((local_paths == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailString__SWIG_0());
			if (local_paths != null)
			{
				RailConverter.Csharp2Cpp(local_paths, intPtr);
			}
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_SetAdditionalPreviews(this.swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayRailString(intPtr);
			}
			return railResult;
		}

		public virtual RailResult SetAssociatedSpaceWorks(List<SpaceWorkID> ids)
		{
			IntPtr intPtr = ((ids == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArraySpaceWorkID__SWIG_0());
			if (ids != null)
			{
				RailConverter.Csharp2Cpp(ids, intPtr);
			}
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_SetAssociatedSpaceWorks(this.swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArraySpaceWorkID(intPtr);
			}
			return railResult;
		}

		public virtual RailResult SetLanguages(List<string> languages)
		{
			IntPtr intPtr = ((languages == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailString__SWIG_0());
			if (languages != null)
			{
				RailConverter.Csharp2Cpp(languages, intPtr);
			}
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_SetLanguages(this.swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayRailString(intPtr);
			}
			return railResult;
		}

		public virtual RailResult GetPreviewUrl(out string url, uint scaling)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_GetPreviewUrl__SWIG_0(this.swigCPtr_, intPtr, scaling);
			}
			finally
			{
				url = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
			return railResult;
		}

		public virtual RailResult GetPreviewUrl(out string url)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_GetPreviewUrl__SWIG_1(this.swigCPtr_, intPtr);
			}
			finally
			{
				url = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
			return railResult;
		}

		public virtual RailResult GetVoteDetail(List<RailSpaceWorkVoteDetail> vote_details)
		{
			IntPtr intPtr = ((vote_details == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailSpaceWorkVoteDetail__SWIG_0());
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_GetVoteDetail(this.swigCPtr_, intPtr);
			}
			finally
			{
				if (vote_details != null)
				{
					RailConverter.Cpp2Csharp(intPtr, vote_details);
				}
				RAIL_API_PINVOKE.delete_RailArrayRailSpaceWorkVoteDetail(intPtr);
			}
			return railResult;
		}

		public virtual RailResult GetUploaderIDs(List<RailID> uploader_ids)
		{
			IntPtr intPtr = ((uploader_ids == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailID__SWIG_0());
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_GetUploaderIDs(this.swigCPtr_, intPtr);
			}
			finally
			{
				if (uploader_ids != null)
				{
					RailConverter.Cpp2Csharp(intPtr, uploader_ids);
				}
				RAIL_API_PINVOKE.delete_RailArrayRailID(intPtr);
			}
			return railResult;
		}

		public virtual RailResult SetUpdateOptions(RailSpaceWorkUpdateOptions options)
		{
			IntPtr intPtr = ((options == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailSpaceWorkUpdateOptions__SWIG_0());
			if (options != null)
			{
				RailConverter.Csharp2Cpp(options, intPtr);
			}
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_SetUpdateOptions(this.swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailSpaceWorkUpdateOptions(intPtr);
			}
			return railResult;
		}

		public virtual RailResult GetStatistic(EnumRailSpaceWorkStatistic stat_type, out ulong value)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_GetStatistic(this.swigCPtr_, (int)stat_type, out value);
		}

		public virtual RailResult RemovePreviewImage()
		{
			return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_RemovePreviewImage(this.swigCPtr_);
		}

		public virtual uint GetState()
		{
			return RAIL_API_PINVOKE.IRailSpaceWork_GetState(this.swigCPtr_);
		}

		public virtual RailResult AddAssociatedGameIDs(List<RailGameID> game_ids)
		{
			IntPtr intPtr = ((game_ids == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailGameID__SWIG_0());
			if (game_ids != null)
			{
				RailConverter.Csharp2Cpp(game_ids, intPtr);
			}
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_AddAssociatedGameIDs(this.swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayRailGameID(intPtr);
			}
			return railResult;
		}

		public virtual RailResult RemoveAssociatedGameIDs(List<RailGameID> game_ids)
		{
			IntPtr intPtr = ((game_ids == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailGameID__SWIG_0());
			if (game_ids != null)
			{
				RailConverter.Csharp2Cpp(game_ids, intPtr);
			}
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_RemoveAssociatedGameIDs(this.swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayRailGameID(intPtr);
			}
			return railResult;
		}

		public virtual RailResult GetAssociatedGameIDs(List<RailGameID> game_ids)
		{
			IntPtr intPtr = ((game_ids == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailGameID__SWIG_0());
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_GetAssociatedGameIDs(this.swigCPtr_, intPtr);
			}
			finally
			{
				if (game_ids != null)
				{
					RailConverter.Cpp2Csharp(intPtr, game_ids);
				}
				RAIL_API_PINVOKE.delete_RailArrayRailGameID(intPtr);
			}
			return railResult;
		}

		public virtual RailResult GetLocalVersion(out string version)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_GetLocalVersion(this.swigCPtr_, intPtr);
			}
			finally
			{
				version = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
			return railResult;
		}

		public virtual ulong GetComponentVersion()
		{
			return RAIL_API_PINVOKE.IRailComponent_GetComponentVersion(this.swigCPtr_);
		}

		public virtual void Release()
		{
			RAIL_API_PINVOKE.IRailComponent_Release(this.swigCPtr_);
		}
	}
}
