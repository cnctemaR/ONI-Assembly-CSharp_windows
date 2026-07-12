using System;
using System.Collections.Generic;

namespace rail
{
	public class IRailUserSpaceHelperImpl : RailObject, IRailUserSpaceHelper
	{
		internal IRailUserSpaceHelperImpl(IntPtr cPtr)
		{
			this.swigCPtr_ = cPtr;
		}

		~IRailUserSpaceHelperImpl()
		{
		}

		public virtual RailResult AsyncGetMySubscribedWorks(uint offset, uint max_works, EnumRailSpaceWorkType type, RailQueryWorkFileOptions options, string user_data)
		{
			IntPtr intPtr = ((options == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailQueryWorkFileOptions__SWIG_0());
			if (options != null)
			{
				RailConverter.Csharp2Cpp(options, intPtr);
			}
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailUserSpaceHelper_AsyncGetMySubscribedWorks__SWIG_0(this.swigCPtr_, offset, max_works, (int)type, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailQueryWorkFileOptions(intPtr);
			}
			return railResult;
		}

		public virtual RailResult AsyncGetMySubscribedWorks(uint offset, uint max_works, EnumRailSpaceWorkType type, RailQueryWorkFileOptions options)
		{
			IntPtr intPtr = ((options == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailQueryWorkFileOptions__SWIG_0());
			if (options != null)
			{
				RailConverter.Csharp2Cpp(options, intPtr);
			}
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailUserSpaceHelper_AsyncGetMySubscribedWorks__SWIG_1(this.swigCPtr_, offset, max_works, (int)type, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailQueryWorkFileOptions(intPtr);
			}
			return railResult;
		}

		public virtual RailResult AsyncGetMySubscribedWorks(uint offset, uint max_works, EnumRailSpaceWorkType type)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailUserSpaceHelper_AsyncGetMySubscribedWorks__SWIG_2(this.swigCPtr_, offset, max_works, (int)type);
		}

		public virtual RailResult AsyncGetMyFavoritesWorks(uint offset, uint max_works, EnumRailSpaceWorkType type, RailQueryWorkFileOptions options, string user_data)
		{
			IntPtr intPtr = ((options == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailQueryWorkFileOptions__SWIG_0());
			if (options != null)
			{
				RailConverter.Csharp2Cpp(options, intPtr);
			}
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailUserSpaceHelper_AsyncGetMyFavoritesWorks__SWIG_0(this.swigCPtr_, offset, max_works, (int)type, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailQueryWorkFileOptions(intPtr);
			}
			return railResult;
		}

		public virtual RailResult AsyncGetMyFavoritesWorks(uint offset, uint max_works, EnumRailSpaceWorkType type, RailQueryWorkFileOptions options)
		{
			IntPtr intPtr = ((options == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailQueryWorkFileOptions__SWIG_0());
			if (options != null)
			{
				RailConverter.Csharp2Cpp(options, intPtr);
			}
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailUserSpaceHelper_AsyncGetMyFavoritesWorks__SWIG_1(this.swigCPtr_, offset, max_works, (int)type, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailQueryWorkFileOptions(intPtr);
			}
			return railResult;
		}

		public virtual RailResult AsyncGetMyFavoritesWorks(uint offset, uint max_works, EnumRailSpaceWorkType type)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailUserSpaceHelper_AsyncGetMyFavoritesWorks__SWIG_2(this.swigCPtr_, offset, max_works, (int)type);
		}

		public virtual RailResult AsyncQuerySpaceWorks(RailSpaceWorkFilter filter, uint offset, uint max_works, EnumRailSpaceWorkOrderBy order_by, RailQueryWorkFileOptions options, string user_data)
		{
			IntPtr intPtr = ((filter == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailSpaceWorkFilter__SWIG_0());
			if (filter != null)
			{
				RailConverter.Csharp2Cpp(filter, intPtr);
			}
			IntPtr intPtr2 = ((options == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailQueryWorkFileOptions__SWIG_0());
			if (options != null)
			{
				RailConverter.Csharp2Cpp(options, intPtr2);
			}
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailUserSpaceHelper_AsyncQuerySpaceWorks__SWIG_0(this.swigCPtr_, intPtr, offset, max_works, (int)order_by, intPtr2, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailSpaceWorkFilter(intPtr);
				RAIL_API_PINVOKE.delete_RailQueryWorkFileOptions(intPtr2);
			}
			return railResult;
		}

		public virtual RailResult AsyncQuerySpaceWorks(RailSpaceWorkFilter filter, uint offset, uint max_works, EnumRailSpaceWorkOrderBy order_by, RailQueryWorkFileOptions options)
		{
			IntPtr intPtr = ((filter == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailSpaceWorkFilter__SWIG_0());
			if (filter != null)
			{
				RailConverter.Csharp2Cpp(filter, intPtr);
			}
			IntPtr intPtr2 = ((options == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailQueryWorkFileOptions__SWIG_0());
			if (options != null)
			{
				RailConverter.Csharp2Cpp(options, intPtr2);
			}
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailUserSpaceHelper_AsyncQuerySpaceWorks__SWIG_1(this.swigCPtr_, intPtr, offset, max_works, (int)order_by, intPtr2);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailSpaceWorkFilter(intPtr);
				RAIL_API_PINVOKE.delete_RailQueryWorkFileOptions(intPtr2);
			}
			return railResult;
		}

		public virtual RailResult AsyncQuerySpaceWorks(RailSpaceWorkFilter filter, uint offset, uint max_works, EnumRailSpaceWorkOrderBy order_by)
		{
			IntPtr intPtr = ((filter == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailSpaceWorkFilter__SWIG_0());
			if (filter != null)
			{
				RailConverter.Csharp2Cpp(filter, intPtr);
			}
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailUserSpaceHelper_AsyncQuerySpaceWorks__SWIG_2(this.swigCPtr_, intPtr, offset, max_works, (int)order_by);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailSpaceWorkFilter(intPtr);
			}
			return railResult;
		}

		public virtual RailResult AsyncQuerySpaceWorks(RailSpaceWorkFilter filter, uint offset, uint max_works)
		{
			IntPtr intPtr = ((filter == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailSpaceWorkFilter__SWIG_0());
			if (filter != null)
			{
				RailConverter.Csharp2Cpp(filter, intPtr);
			}
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailUserSpaceHelper_AsyncQuerySpaceWorks__SWIG_3(this.swigCPtr_, intPtr, offset, max_works);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailSpaceWorkFilter(intPtr);
			}
			return railResult;
		}

		public virtual RailResult AsyncSubscribeSpaceWorks(List<SpaceWorkID> ids, bool subscribe, string user_data)
		{
			IntPtr intPtr = ((ids == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArraySpaceWorkID__SWIG_0());
			if (ids != null)
			{
				RailConverter.Csharp2Cpp(ids, intPtr);
			}
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailUserSpaceHelper_AsyncSubscribeSpaceWorks(this.swigCPtr_, intPtr, subscribe, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArraySpaceWorkID(intPtr);
			}
			return railResult;
		}

		public virtual IRailSpaceWork OpenSpaceWork(SpaceWorkID id)
		{
			IntPtr intPtr = ((id == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_SpaceWorkID__SWIG_0());
			if (id != null)
			{
				RailConverter.Csharp2Cpp(id, intPtr);
			}
			IRailSpaceWork railSpaceWork;
			try
			{
				IntPtr intPtr2 = RAIL_API_PINVOKE.IRailUserSpaceHelper_OpenSpaceWork(this.swigCPtr_, intPtr);
				railSpaceWork = ((intPtr2 == IntPtr.Zero) ? null : new IRailSpaceWorkImpl(intPtr2));
			}
			finally
			{
				RAIL_API_PINVOKE.delete_SpaceWorkID(intPtr);
			}
			return railSpaceWork;
		}

		public virtual IRailSpaceWork CreateSpaceWork(EnumRailSpaceWorkType type)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailUserSpaceHelper_CreateSpaceWork(this.swigCPtr_, (int)type);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailSpaceWorkImpl(intPtr);
			}
			return null;
		}

		public virtual RailResult GetMySubscribedWorks(uint offset, uint max_works, EnumRailSpaceWorkType type, QueryMySubscribedSpaceWorksResult result)
		{
			IntPtr intPtr = ((result == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_QueryMySubscribedSpaceWorksResult__SWIG_0());
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailUserSpaceHelper_GetMySubscribedWorks(this.swigCPtr_, offset, max_works, (int)type, intPtr);
			}
			finally
			{
				if (result != null)
				{
					RailConverter.Cpp2Csharp(intPtr, result);
				}
				RAIL_API_PINVOKE.delete_QueryMySubscribedSpaceWorksResult(intPtr);
			}
			return railResult;
		}

		public virtual uint GetMySubscribedWorksCount(EnumRailSpaceWorkType type, out RailResult result)
		{
			return RAIL_API_PINVOKE.IRailUserSpaceHelper_GetMySubscribedWorksCount(this.swigCPtr_, (int)type, out result);
		}

		public virtual RailResult AsyncRemoveSpaceWork(SpaceWorkID id, string user_data)
		{
			IntPtr intPtr = ((id == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_SpaceWorkID__SWIG_0());
			if (id != null)
			{
				RailConverter.Csharp2Cpp(id, intPtr);
			}
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailUserSpaceHelper_AsyncRemoveSpaceWork(this.swigCPtr_, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_SpaceWorkID(intPtr);
			}
			return railResult;
		}

		public virtual RailResult AsyncModifyFavoritesWorks(List<SpaceWorkID> ids, EnumRailModifyFavoritesSpaceWorkType modify_flag, string user_data)
		{
			IntPtr intPtr = ((ids == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArraySpaceWorkID__SWIG_0());
			if (ids != null)
			{
				RailConverter.Csharp2Cpp(ids, intPtr);
			}
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailUserSpaceHelper_AsyncModifyFavoritesWorks(this.swigCPtr_, intPtr, (int)modify_flag, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArraySpaceWorkID(intPtr);
			}
			return railResult;
		}

		public virtual RailResult AsyncVoteSpaceWork(SpaceWorkID id, EnumRailSpaceWorkVoteValue vote, string user_data)
		{
			IntPtr intPtr = ((id == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_SpaceWorkID__SWIG_0());
			if (id != null)
			{
				RailConverter.Csharp2Cpp(id, intPtr);
			}
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailUserSpaceHelper_AsyncVoteSpaceWork(this.swigCPtr_, intPtr, (int)vote, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_SpaceWorkID(intPtr);
			}
			return railResult;
		}

		public virtual RailResult AsyncSearchSpaceWork(RailSpaceWorkSearchFilter filter, RailQueryWorkFileOptions options, List<EnumRailSpaceWorkType> types, uint offset, uint max_works, EnumRailSpaceWorkOrderBy order_by, string user_data)
		{
			IntPtr intPtr = ((filter == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailSpaceWorkSearchFilter__SWIG_0());
			if (filter != null)
			{
				RailConverter.Csharp2Cpp(filter, intPtr);
			}
			IntPtr intPtr2 = ((options == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailQueryWorkFileOptions__SWIG_0());
			if (options != null)
			{
				RailConverter.Csharp2Cpp(options, intPtr2);
			}
			IntPtr intPtr3 = ((types == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayEnumRailSpaceWorkType__SWIG_0());
			if (types != null)
			{
				RailConverter.Csharp2Cpp(types, intPtr3);
			}
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailUserSpaceHelper_AsyncSearchSpaceWork(this.swigCPtr_, intPtr, intPtr2, intPtr3, offset, max_works, (int)order_by, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailSpaceWorkSearchFilter(intPtr);
				RAIL_API_PINVOKE.delete_RailQueryWorkFileOptions(intPtr2);
				RAIL_API_PINVOKE.delete_RailArrayEnumRailSpaceWorkType(intPtr3);
			}
			return railResult;
		}

		public virtual RailResult AsyncRateSpaceWork(SpaceWorkID id, EnumRailSpaceWorkRateValue mark, string user_data)
		{
			IntPtr intPtr = ((id == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_SpaceWorkID__SWIG_0());
			if (id != null)
			{
				RailConverter.Csharp2Cpp(id, intPtr);
			}
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailUserSpaceHelper_AsyncRateSpaceWork(this.swigCPtr_, intPtr, (int)mark, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_SpaceWorkID(intPtr);
			}
			return railResult;
		}

		public virtual RailResult AsyncQuerySpaceWorksInfo(List<SpaceWorkID> ids, string user_data)
		{
			IntPtr intPtr = ((ids == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArraySpaceWorkID__SWIG_0());
			if (ids != null)
			{
				RailConverter.Csharp2Cpp(ids, intPtr);
			}
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailUserSpaceHelper_AsyncQuerySpaceWorksInfo(this.swigCPtr_, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArraySpaceWorkID(intPtr);
			}
			return railResult;
		}
	}
}
