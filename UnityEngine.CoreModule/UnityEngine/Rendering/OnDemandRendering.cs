using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	[RequiredByNativeCode]
	public class OnDemandRendering
	{
		public static bool willCurrentFrameRender
		{
			get
			{
				return Time.frameCount % OnDemandRendering.renderFrameInterval == 0;
			}
		}

		public static int renderFrameInterval
		{
			get
			{
				return OnDemandRendering.m_RenderFrameInterval;
			}
			set
			{
				OnDemandRendering.m_RenderFrameInterval = Math.Max(1, value);
			}
		}

		[RequiredByNativeCode]
		internal static void GetRenderFrameInterval(out int frameInterval)
		{
			frameInterval = OnDemandRendering.renderFrameInterval;
		}

		public static int effectiveRenderFrameRate
		{
			get
			{
				bool flag = QualitySettings.vSyncCount > 0;
				int num;
				if (flag)
				{
					num = Screen.currentResolution.refreshRate / QualitySettings.vSyncCount / OnDemandRendering.renderFrameInterval;
				}
				else
				{
					bool flag2 = Application.targetFrameRate <= 0;
					if (flag2)
					{
						num = Application.targetFrameRate;
					}
					else
					{
						num = Application.targetFrameRate / OnDemandRendering.renderFrameInterval;
					}
				}
				return num;
			}
		}

		private static int m_RenderFrameInterval = 1;
	}
}
