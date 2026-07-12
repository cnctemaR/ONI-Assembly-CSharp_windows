using System;

namespace rail
{
	public class IRailZoneServerHelperImpl : RailObject, IRailZoneServerHelper
	{
		internal IRailZoneServerHelperImpl(IntPtr cPtr)
		{
			this.swigCPtr_ = cPtr;
		}

		~IRailZoneServerHelperImpl()
		{
		}

		public virtual RailZoneID GetPlayerSelectedZoneID()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailZoneServerHelper_GetPlayerSelectedZoneID(this.swigCPtr_);
			RailZoneID railZoneID = new RailZoneID();
			RailConverter.Cpp2Csharp(intPtr, railZoneID);
			return railZoneID;
		}

		public virtual RailZoneID GetRootZoneID()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailZoneServerHelper_GetRootZoneID(this.swigCPtr_);
			RailZoneID railZoneID = new RailZoneID();
			RailConverter.Cpp2Csharp(intPtr, railZoneID);
			return railZoneID;
		}

		public virtual IRailZoneServer OpenZoneServer(RailZoneID zone_id, out RailResult result)
		{
			IntPtr intPtr = ((zone_id == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailZoneID__SWIG_0());
			if (zone_id != null)
			{
				RailConverter.Csharp2Cpp(zone_id, intPtr);
			}
			IRailZoneServer railZoneServer;
			try
			{
				IntPtr intPtr2 = RAIL_API_PINVOKE.IRailZoneServerHelper_OpenZoneServer(this.swigCPtr_, intPtr, out result);
				railZoneServer = ((intPtr2 == IntPtr.Zero) ? null : new IRailZoneServerImpl(intPtr2));
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailZoneID(intPtr);
			}
			return railZoneServer;
		}

		public virtual RailResult AsyncSwitchPlayerSelectedZone(RailZoneID zone_id)
		{
			IntPtr intPtr = ((zone_id == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailZoneID__SWIG_0());
			if (zone_id != null)
			{
				RailConverter.Csharp2Cpp(zone_id, intPtr);
			}
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailZoneServerHelper_AsyncSwitchPlayerSelectedZone(this.swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailZoneID(intPtr);
			}
			return railResult;
		}
	}
}
