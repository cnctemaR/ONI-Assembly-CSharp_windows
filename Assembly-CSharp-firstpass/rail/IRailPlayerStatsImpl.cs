using System;

namespace rail
{
	public class IRailPlayerStatsImpl : RailObject, IRailPlayerStats, IRailComponent
	{
		internal IRailPlayerStatsImpl(IntPtr cPtr)
		{
			this.swigCPtr_ = cPtr;
		}

		~IRailPlayerStatsImpl()
		{
		}

		public virtual RailID GetRailID()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailPlayerStats_GetRailID(this.swigCPtr_);
			RailID railID = new RailID();
			RailConverter.Cpp2Csharp(intPtr, railID);
			return railID;
		}

		public virtual RailResult AsyncRequestStats(string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailPlayerStats_AsyncRequestStats(this.swigCPtr_, user_data);
		}

		public virtual RailResult GetStatValue(string name, out int data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailPlayerStats_GetStatValue__SWIG_0(this.swigCPtr_, name, out data);
		}

		public virtual RailResult GetStatValue(string name, out double data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailPlayerStats_GetStatValue__SWIG_1(this.swigCPtr_, name, out data);
		}

		public virtual RailResult SetStatValue(string name, int data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailPlayerStats_SetStatValue__SWIG_0(this.swigCPtr_, name, data);
		}

		public virtual RailResult SetStatValue(string name, double data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailPlayerStats_SetStatValue__SWIG_1(this.swigCPtr_, name, data);
		}

		public virtual RailResult UpdateAverageStatValue(string name, double data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailPlayerStats_UpdateAverageStatValue(this.swigCPtr_, name, data);
		}

		public virtual RailResult AsyncStoreStats(string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailPlayerStats_AsyncStoreStats(this.swigCPtr_, user_data);
		}

		public virtual RailResult ResetAllStats()
		{
			return (RailResult)RAIL_API_PINVOKE.IRailPlayerStats_ResetAllStats(this.swigCPtr_);
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
