using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.Rendering;

namespace UnityEngine.AMD
{
	public class GraphicsDevice
	{
		private GraphicsDevice()
		{
		}

		private bool Initialize()
		{
			return GraphicsDevice.AMDUP_InitApi();
		}

		private void Shutdown()
		{
			GraphicsDevice.AMDUP_ShutdownApi();
		}

		~GraphicsDevice()
		{
			this.Shutdown();
		}

		private void InsertEventCall(CommandBuffer cmd, PluginEvent pluginEvent, IntPtr ptr)
		{
			cmd.IssuePluginEventAndData(GraphicsDevice.AMDUP_GetRenderEventCallback(), (int)(pluginEvent + GraphicsDevice.AMDUP_GetBaseEventId()), ptr);
		}

		private static GraphicsDevice InternalCreate()
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
				GraphicsDevice graphicsDevice2 = new GraphicsDevice();
				bool flag2 = graphicsDevice2.Initialize();
				if (flag2)
				{
					GraphicsDevice.sGraphicsDeviceInstance = graphicsDevice2;
					graphicsDevice = graphicsDevice2;
				}
				else
				{
					Debug.LogWarning("Unity has an invalid api for dvice. Init failed[");
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

		private void SetTexture(CommandBuffer cmd, FSR2Context fsr2Context, FSR2CommandExecutionData.Textures textureSlot, Texture texture, bool clearTextureTable = false)
		{
			bool flag = texture == null;
			if (!flag)
			{
				uint num = (uint)GraphicsDevice.CreateSetTextureUserData((int)fsr2Context.featureSlot, (int)textureSlot, clearTextureTable);
				cmd.IssuePluginCustomTextureUpdateV2(GraphicsDevice.AMDUP_GetSetTextureEventCallback(), texture, num);
			}
		}

		public static GraphicsDevice CreateGraphicsDevice()
		{
			return GraphicsDevice.InternalCreate();
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
				return GraphicsDevice.AMDUP_GetDeviceVersion();
			}
		}

		public FSR2Context CreateFeature(CommandBuffer cmd, in FSR2CommandInitializationData initSettings)
		{
			bool flag = this.s_ContextObjectPool.Count == 0;
			FSR2Context fsr2Context;
			if (flag)
			{
				fsr2Context = new FSR2Context();
			}
			else
			{
				fsr2Context = this.s_ContextObjectPool.Pop();
			}
			fsr2Context.Init(initSettings, GraphicsDevice.AMDUP_CreateFeatureSlot());
			this.InsertEventCall(cmd, PluginEvent.FSR2Init, fsr2Context.GetInitCmdPtr());
			return fsr2Context;
		}

		public bool GetRenderResolutionFromQualityMode(FSR2Quality qualityMode, uint displayWidth, uint displayHeight, out uint renderWidth, out uint renderHeight)
		{
			return GraphicsDevice.AMDUP_GetRenderResolutionFromQualityMode(qualityMode, displayWidth, displayHeight, out renderWidth, out renderHeight);
		}

		public float GetUpscaleRatioFromQualityMode(FSR2Quality qualityMode)
		{
			return GraphicsDevice.AMDUP_GetUpscaleRatioFromQualityMode(qualityMode);
		}

		public void DestroyFeature(CommandBuffer cmd, FSR2Context fsrContext)
		{
			this.InsertEventCall(cmd, PluginEvent.DestroyFeature, new IntPtr((long)((ulong)fsrContext.featureSlot)));
			fsrContext.Reset();
			this.s_ContextObjectPool.Push(fsrContext);
		}

		public void ExecuteFSR2(CommandBuffer cmd, FSR2Context fsr2Context, in FSR2TextureTable textures)
		{
			this.SetTexture(cmd, fsr2Context, FSR2CommandExecutionData.Textures.ColorInput, textures.colorInput, true);
			this.SetTexture(cmd, fsr2Context, FSR2CommandExecutionData.Textures.ColorOutput, textures.colorOutput, false);
			this.SetTexture(cmd, fsr2Context, FSR2CommandExecutionData.Textures.Depth, textures.depth, false);
			this.SetTexture(cmd, fsr2Context, FSR2CommandExecutionData.Textures.MotionVectors, textures.motionVectors, false);
			this.SetTexture(cmd, fsr2Context, FSR2CommandExecutionData.Textures.TransparencyMask, textures.transparencyMask, false);
			this.SetTexture(cmd, fsr2Context, FSR2CommandExecutionData.Textures.ExposureTexture, textures.exposureTexture, false);
			this.SetTexture(cmd, fsr2Context, FSR2CommandExecutionData.Textures.BiasColorMask, textures.biasColorMask, false);
			this.InsertEventCall(cmd, PluginEvent.FSR2Execute, fsr2Context.GetExecuteCmdPtr());
			this.InsertEventCall(cmd, PluginEvent.FSR2PostExecute, fsr2Context.GetExecuteCmdPtr());
		}

		[DllImport("AMDUnityPlugin", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
		private static extern bool AMDUP_InitApi();

		[DllImport("AMDUnityPlugin", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
		private static extern void AMDUP_ShutdownApi();

		[DllImport("AMDUnityPlugin", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
		private static extern uint AMDUP_GetDeviceVersion();

		[DllImport("AMDUnityPlugin", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
		private static extern IntPtr AMDUP_GetRenderEventCallback();

		[DllImport("AMDUnityPlugin", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
		private static extern IntPtr AMDUP_GetSetTextureEventCallback();

		[DllImport("AMDUnityPlugin", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
		private static extern uint AMDUP_CreateFeatureSlot();

		[DllImport("AMDUnityPlugin", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
		private static extern bool AMDUP_GetRenderResolutionFromQualityMode(FSR2Quality qualityMode, uint displayWidth, uint displayHeight, out uint renderWidth, out uint renderHeight);

		[DllImport("AMDUnityPlugin", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
		private static extern float AMDUP_GetUpscaleRatioFromQualityMode(FSR2Quality qualityMode);

		[DllImport("AMDUnityPlugin", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
		private static extern int AMDUP_GetBaseEventId();

		private static GraphicsDevice sGraphicsDeviceInstance;

		private Stack<FSR2Context> s_ContextObjectPool = new Stack<FSR2Context>();
	}
}
