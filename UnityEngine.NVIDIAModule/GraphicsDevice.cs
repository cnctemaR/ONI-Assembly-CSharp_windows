using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.Rendering;

namespace UnityEngine.NVIDIA
{
	public class GraphicsDevice
	{
		private GraphicsDevice(string projectId, string engineVersion, string appDir)
		{
			this.m_InitDeviceContext = new InitDeviceContext(projectId, engineVersion, appDir);
		}

		private bool Initialize()
		{
			return GraphicsDevice.NVUP_InitApi(this.m_InitDeviceContext.GetInitCmdPtr());
		}

		private void Shutdown()
		{
			GraphicsDevice.NVUP_ShutdownApi();
		}

		~GraphicsDevice()
		{
			this.Shutdown();
		}

		private void InsertEventCall(CommandBuffer cmd, PluginEvent pluginEvent, IntPtr ptr)
		{
			cmd.IssuePluginEventAndData(GraphicsDevice.NVUP_GetRenderEventCallback(), (int)(pluginEvent + GraphicsDevice.NVUP_GetBaseEventId()), ptr);
		}

		private static GraphicsDevice InternalCreate(string appIdOrProjectId, string engineVersion, string appDir)
		{
			bool flag = GraphicsDevice.sGraphicsDeviceInstance != null;
			GraphicsDevice graphicsDevice;
			if (flag)
			{
				GraphicsDevice.sGraphicsDeviceInstance.Shutdown();
				GraphicsDevice.sGraphicsDeviceInstance.Initialize();
				graphicsDevice = GraphicsDevice.sGraphicsDeviceInstance;
			}
			else
			{
				GraphicsDevice graphicsDevice2 = new GraphicsDevice(appIdOrProjectId, engineVersion, appDir);
				bool flag2 = graphicsDevice2.Initialize();
				if (flag2)
				{
					GraphicsDevice.sGraphicsDeviceInstance = graphicsDevice2;
					graphicsDevice = graphicsDevice2;
				}
				else
				{
					graphicsDevice = null;
				}
			}
			return graphicsDevice;
		}

		private static int CreateSetTextureUserData(int featureId, int textureSlot, bool clearTextureTable)
		{
			int num = featureId & 65535;
			int num2 = textureSlot & 32767;
			int num3 = (clearTextureTable ? 1 : 0);
			return (num << 16) | (num2 << 1) | num3;
		}

		private void SetTexture(CommandBuffer cmd, DLSSContext dlssContext, DLSSCommandExecutionData.Textures textureSlot, Texture texture, bool clearTextureTable = false)
		{
			bool flag = texture == null;
			if (!flag)
			{
				uint num = (uint)GraphicsDevice.CreateSetTextureUserData((int)dlssContext.featureSlot, (int)textureSlot, clearTextureTable);
				cmd.IssuePluginCustomTextureUpdateV2(GraphicsDevice.NVUP_GetSetTextureEventCallback(), texture, num);
			}
		}

		internal GraphicsDeviceDebugInfo GetDebugInfo(uint debugViewId)
		{
			GraphicsDeviceDebugInfo graphicsDeviceDebugInfo = default(GraphicsDeviceDebugInfo);
			GraphicsDevice.NVUP_GetGraphicsDeviceDebugInfo(debugViewId, out graphicsDeviceDebugInfo);
			return graphicsDeviceDebugInfo;
		}

		internal uint CreateDebugViewId()
		{
			return GraphicsDevice.NVUP_CreateDebugView();
		}

		internal void DeleteDebugViewId(uint debugViewId)
		{
			GraphicsDevice.NVUP_DeleteDebugView(debugViewId);
		}

		public static GraphicsDevice CreateGraphicsDevice()
		{
			return GraphicsDevice.InternalCreate(GraphicsDevice.s_DefaultProjectID, Application.unityVersion, GraphicsDevice.s_DefaultAppDir);
		}

		public static GraphicsDevice CreateGraphicsDevice(string projectID)
		{
			return GraphicsDevice.InternalCreate(projectID, Application.unityVersion, GraphicsDevice.s_DefaultAppDir);
		}

		public static GraphicsDevice CreateGraphicsDevice(string projectID, string appDir)
		{
			return GraphicsDevice.InternalCreate(projectID, Application.unityVersion, appDir);
		}

		public static GraphicsDevice device
		{
			get
			{
				return GraphicsDevice.sGraphicsDeviceInstance;
			}
		}

		public static uint version
		{
			get
			{
				return GraphicsDevice.NVUP_GetDeviceVersion();
			}
		}

		public bool IsFeatureAvailable(GraphicsDeviceFeature featureID)
		{
			return GraphicsDevice.NVUP_IsFeatureAvailable(featureID);
		}

		public DLSSContext CreateFeature(CommandBuffer cmd, in DLSSCommandInitializationData initSettings)
		{
			bool flag = !this.IsFeatureAvailable(GraphicsDeviceFeature.DLSS);
			DLSSContext dlsscontext;
			if (flag)
			{
				dlsscontext = null;
			}
			else
			{
				bool flag2 = this.s_ContextObjectPool.Count == 0;
				DLSSContext dlsscontext2;
				if (flag2)
				{
					dlsscontext2 = new DLSSContext();
				}
				else
				{
					dlsscontext2 = this.s_ContextObjectPool.Pop();
				}
				dlsscontext2.Init(initSettings, GraphicsDevice.NVUP_CreateFeatureSlot());
				this.InsertEventCall(cmd, PluginEvent.DLSSInit, dlsscontext2.GetInitCmdPtr());
				dlsscontext = dlsscontext2;
			}
			return dlsscontext;
		}

		public void DestroyFeature(CommandBuffer cmd, DLSSContext dlssContext)
		{
			this.InsertEventCall(cmd, PluginEvent.DestroyFeature, new IntPtr((long)((ulong)dlssContext.featureSlot)));
			dlssContext.Reset();
			this.s_ContextObjectPool.Push(dlssContext);
		}

		public void ExecuteDLSS(CommandBuffer cmd, DLSSContext dlssContext, in DLSSTextureTable textures)
		{
			this.SetTexture(cmd, dlssContext, DLSSCommandExecutionData.Textures.ColorInput, textures.colorInput, true);
			this.SetTexture(cmd, dlssContext, DLSSCommandExecutionData.Textures.ColorOutput, textures.colorOutput, false);
			this.SetTexture(cmd, dlssContext, DLSSCommandExecutionData.Textures.Depth, textures.depth, false);
			this.SetTexture(cmd, dlssContext, DLSSCommandExecutionData.Textures.MotionVectors, textures.motionVectors, false);
			this.SetTexture(cmd, dlssContext, DLSSCommandExecutionData.Textures.TransparencyMask, textures.transparencyMask, false);
			this.SetTexture(cmd, dlssContext, DLSSCommandExecutionData.Textures.ExposureTexture, textures.exposureTexture, false);
			this.SetTexture(cmd, dlssContext, DLSSCommandExecutionData.Textures.BiasColorMask, textures.biasColorMask, false);
			this.InsertEventCall(cmd, PluginEvent.DLSSExecute, dlssContext.GetExecuteCmdPtr());
		}

		public bool GetOptimalSettings(uint targetWidth, uint targetHeight, DLSSQuality quality, out OptimalDLSSSettingsData optimalSettings)
		{
			return GraphicsDevice.NVUP_GetOptimalSettings(targetWidth, targetHeight, quality, out optimalSettings);
		}

		public GraphicsDeviceDebugView CreateDebugView()
		{
			return new GraphicsDeviceDebugView(this.CreateDebugViewId());
		}

		public unsafe void UpdateDebugView(GraphicsDeviceDebugView debugView)
		{
			bool flag = debugView == null;
			if (!flag)
			{
				GraphicsDeviceDebugInfo debugInfo = this.GetDebugInfo(debugView.m_ViewId);
				debugView.m_DeviceVersion = debugInfo.NVDeviceVersion;
				debugView.m_NgxVersion = debugInfo.NGXVersion;
				bool flag2 = debugView.m_DlssDebugFeatures == null || (ulong)debugInfo.dlssInfosCount != (ulong)((long)debugView.m_DlssDebugFeatures.Length);
				if (flag2)
				{
					debugView.m_DlssDebugFeatures = new DLSSDebugFeatureInfos[debugInfo.dlssInfosCount];
				}
				int num = 0;
				while ((long)num < (long)((ulong)debugInfo.dlssInfosCount))
				{
					debugView.m_DlssDebugFeatures[num] = debugInfo.dlssInfos[num];
					num++;
				}
			}
		}

		public void DeleteDebugView(GraphicsDeviceDebugView debugView)
		{
			bool flag = debugView == null;
			if (!flag)
			{
				this.DeleteDebugViewId(debugView.m_ViewId);
			}
		}

		[DllImport("NVUnityPlugin", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
		private static extern bool NVUP_InitApi(IntPtr initData);

		[DllImport("NVUnityPlugin", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
		private static extern void NVUP_ShutdownApi();

		[DllImport("NVUnityPlugin", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
		private static extern bool NVUP_IsFeatureAvailable(GraphicsDeviceFeature featureID);

		[DllImport("NVUnityPlugin", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
		private static extern bool NVUP_GetOptimalSettings(uint inTargetWidth, uint inTargetHeight, DLSSQuality inPerfVQuality, out OptimalDLSSSettingsData data);

		[DllImport("NVUnityPlugin", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
		private static extern IntPtr NVUP_GetRenderEventCallback();

		[DllImport("NVUnityPlugin", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
		private static extern IntPtr NVUP_GetSetTextureEventCallback();

		[DllImport("NVUnityPlugin", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
		private static extern uint NVUP_CreateFeatureSlot();

		[DllImport("NVUnityPlugin", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
		private static extern uint NVUP_GetDeviceVersion();

		[DllImport("NVUnityPlugin", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
		private static extern uint NVUP_CreateDebugView();

		[DllImport("NVUnityPlugin", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
		private static extern void NVUP_GetGraphicsDeviceDebugInfo(uint debugViewId, out GraphicsDeviceDebugInfo data);

		[DllImport("NVUnityPlugin", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
		private static extern void NVUP_DeleteDebugView(uint debugViewId);

		[DllImport("NVUnityPlugin", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
		private static extern int NVUP_GetBaseEventId();

		private static string s_DefaultProjectID = "231313132";

		private static string s_DefaultAppDir = ".\\";

		private static GraphicsDevice sGraphicsDeviceInstance = null;

		private InitDeviceContext m_InitDeviceContext = null;

		private Stack<DLSSContext> s_ContextObjectPool = new Stack<DLSSContext>();
	}
}
