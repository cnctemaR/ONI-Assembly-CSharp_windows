using System;
using System.Collections.Generic;

namespace rail
{
	public class IRailZoneServerImpl : RailObject, IRailZoneServer, IRailComponent
	{
		internal IRailZoneServerImpl(IntPtr cPtr)
		{
			this.swigCPtr_ = cPtr;
		}

		~IRailZoneServerImpl()
		{
		}

		public virtual RailZoneID GetZoneID()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailZoneServer_GetZoneID(this.swigCPtr_);
			RailZoneID railZoneID = new RailZoneID();
			RailConverter.Cpp2Csharp(intPtr, railZoneID);
			return railZoneID;
		}

		public virtual RailResult GetZoneNameLanguages(List<string> languages)
		{
			IntPtr intPtr = ((languages == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailString__SWIG_0());
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailZoneServer_GetZoneNameLanguages(this.swigCPtr_, intPtr);
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

		public virtual RailResult GetZoneName(string language_filter, out string zone_name)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailZoneServer_GetZoneName(this.swigCPtr_, language_filter, intPtr);
			}
			finally
			{
				zone_name = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
			return railResult;
		}

		public virtual RailResult GetZoneDescriptionLanguages(List<string> languages)
		{
			IntPtr intPtr = ((languages == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailString__SWIG_0());
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailZoneServer_GetZoneDescriptionLanguages(this.swigCPtr_, intPtr);
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

		public virtual RailResult GetZoneDescription(string language_filter, out string zone_description)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailZoneServer_GetZoneDescription(this.swigCPtr_, language_filter, intPtr);
			}
			finally
			{
				zone_description = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
			return railResult;
		}

		public virtual RailResult GetGameServerAddresses(List<string> server_addresses)
		{
			IntPtr intPtr = ((server_addresses == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailString__SWIG_0());
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailZoneServer_GetGameServerAddresses(this.swigCPtr_, intPtr);
			}
			finally
			{
				if (server_addresses != null)
				{
					RailConverter.Cpp2Csharp(intPtr, server_addresses);
				}
				RAIL_API_PINVOKE.delete_RailArrayRailString(intPtr);
			}
			return railResult;
		}

		public virtual RailResult GetZoneMetadatas(List<RailKeyValue> key_values)
		{
			IntPtr intPtr = ((key_values == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailKeyValue__SWIG_0());
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailZoneServer_GetZoneMetadatas(this.swigCPtr_, intPtr);
			}
			finally
			{
				if (key_values != null)
				{
					RailConverter.Cpp2Csharp(intPtr, key_values);
				}
				RAIL_API_PINVOKE.delete_RailArrayRailKeyValue(intPtr);
			}
			return railResult;
		}

		public virtual RailResult GetChildrenZoneIDs(List<RailZoneID> zone_ids)
		{
			IntPtr intPtr = ((zone_ids == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailZoneID__SWIG_0());
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailZoneServer_GetChildrenZoneIDs(this.swigCPtr_, intPtr);
			}
			finally
			{
				if (zone_ids != null)
				{
					RailConverter.Cpp2Csharp(intPtr, zone_ids);
				}
				RAIL_API_PINVOKE.delete_RailArrayRailZoneID(intPtr);
			}
			return railResult;
		}

		public virtual bool IsZoneVisiable()
		{
			return RAIL_API_PINVOKE.IRailZoneServer_IsZoneVisiable(this.swigCPtr_);
		}

		public virtual bool IsZoneJoinable()
		{
			return RAIL_API_PINVOKE.IRailZoneServer_IsZoneJoinable(this.swigCPtr_);
		}

		public virtual uint GetZoneEnableStartTime()
		{
			return RAIL_API_PINVOKE.IRailZoneServer_GetZoneEnableStartTime(this.swigCPtr_);
		}

		public virtual uint GetZoneEnableEndTime()
		{
			return RAIL_API_PINVOKE.IRailZoneServer_GetZoneEnableEndTime(this.swigCPtr_);
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
