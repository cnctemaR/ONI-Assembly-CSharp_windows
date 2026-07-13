using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;
using UnityEngine.UIElements.UIR;

namespace UnityEngine.UIElements
{
	[NativeType(Header = "Modules/UIElements/Core/Native/Renderer/UIRenderer.h")]
	public sealed class UIRenderer : Renderer
	{
		internal void AddDrawCallData(int safeFrameIndex, int cmdListIndex, Material mat, uint textureSlotCount, uint forceRenderType)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<UIRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			UIRenderer.AddDrawCallData_Injected(intPtr, safeFrameIndex, cmdListIndex, Object.MarshalledUnityObject.Marshal<Material>(mat), textureSlotCount, forceRenderType);
		}

		internal void ResetDrawCallData()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<UIRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			UIRenderer.ResetDrawCallData_Injected(intPtr);
		}

		internal int GetDrawCallDataCount()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<UIRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return UIRenderer.GetDrawCallDataCount_Injected(intPtr);
		}

		[RequiredByNativeCode]
		private static void OnRenderNodeExecute(UIRenderer renderer, int safeFrameIndex, int cmdListIndex)
		{
			bool flag = renderer.skipRendering;
			if (!flag)
			{
				List<CommandList>[] array = renderer.commandLists;
				List<CommandList> list = ((array != null) ? array[safeFrameIndex] : null);
				bool flag2 = list != null && cmdListIndex < list.Count;
				if (flag2)
				{
					CommandList commandList = list[cmdListIndex];
					if (commandList != null)
					{
						commandList.Execute();
					}
				}
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AddDrawCallData_Injected(IntPtr _unity_self, int safeFrameIndex, int cmdListIndex, IntPtr mat, uint textureSlotCount, uint forceRenderType);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ResetDrawCallData_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetDrawCallDataCount_Injected(IntPtr _unity_self);

		internal volatile List<CommandList>[] commandLists;

		internal volatile bool skipRendering;
	}
}
