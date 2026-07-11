using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	public struct WebCamDevice
	{
		public string name
		{
			get
			{
				return this.m_Name;
			}
		}

		public bool isFrontFacing
		{
			get
			{
				return (this.m_Flags & 1) != 0;
			}
		}

		public WebCamKind kind
		{
			get
			{
				return this.m_Kind;
			}
		}

		public string depthCameraName
		{
			get
			{
				return (!(this.m_DepthCameraName == "")) ? this.m_DepthCameraName : null;
			}
		}

		public bool isAutoFocusPointSupported
		{
			get
			{
				return (this.m_Flags & 2) != 0;
			}
		}

		public Resolution[] availableResolutions
		{
			get
			{
				return this.m_Resolutions;
			}
		}

		internal string m_Name;

		internal string m_DepthCameraName;

		internal int m_Flags;

		internal WebCamKind m_Kind;

		internal Resolution[] m_Resolutions;
	}
}
