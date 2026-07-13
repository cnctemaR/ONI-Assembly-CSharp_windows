using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements.UIR
{
	internal class CommandListManager : IDisposable
	{
		public CommandListManager(IntPtr vertexDecl, IntPtr defaultStencilState)
		{
			this.m_VertexDecl = vertexDecl;
			this.m_DefaultStencilState = defaultStencilState;
			this.m_CommandListsArray = new List<CommandList>[4];
			int num = 0;
			while ((long)num < 4L)
			{
				this.m_CommandListsArray[num] = new List<CommandList>();
				num++;
			}
		}

		public CommandList defaultCommandList
		{
			get
			{
				return this.m_DefaultCommandList;
			}
		}

		public CommandList GetOrCreateCommandList(VisualElement owner, Material material, CommandFlags commandFlags)
		{
			bool flag = this.m_CommandListPool.Count > 0;
			CommandList commandList;
			if (flag)
			{
				commandList = this.m_CommandListPool.Pop();
			}
			else
			{
				commandList = new CommandList(this.m_VertexDecl, this.m_DefaultStencilState);
			}
			commandList.Init(owner, material, commandFlags);
			this.m_CurrentFrameCommandLists.Add(commandList);
			return commandList;
		}

		public void AdvanceFrame()
		{
			this.m_CurrentIndex += 1U;
			bool flag = this.m_CurrentIndex == 4U;
			if (flag)
			{
				this.m_CurrentIndex = 0U;
			}
			this.m_CurrentFrameCommandLists = this.m_CommandListsArray[(int)this.m_CurrentIndex];
			for (int i = 0; i < this.m_CurrentFrameCommandLists.Count; i++)
			{
				CommandList commandList = this.m_CurrentFrameCommandLists[i];
				commandList.Reset();
				this.m_CommandListPool.Push(commandList);
			}
			this.m_CurrentFrameCommandLists.Clear();
			this.ResetUIRendererDrawCallData();
		}

		public void BeginSerialize(TextureSlotCount textureSlotCount)
		{
			this.m_TextureSlotCount = textureSlotCount;
			this.m_DefaultCommandList.Init(null, null, CommandFlags.None);
		}

		public void EndSerialize()
		{
			for (int i = 0; i < this.m_CurrentFrameCommandLists.Count; i++)
			{
				CommandList commandList = this.m_CurrentFrameCommandLists[i];
				UIRenderer renderer = commandList.m_Renderer;
				bool flag = renderer != null;
				if (flag)
				{
					renderer.commandLists = this.m_CommandListsArray;
					bool flag2 = (commandList.flags & CommandFlags.ForceSingleTextureSlot) > CommandFlags.None;
					uint num = (uint)((commandList.flags & CommandFlags.ForceRenderTypeBits) >> 1);
					renderer.AddDrawCallData((int)this.m_CurrentIndex, i, commandList.m_Material, (uint)(flag2 ? TextureSlotCount.One : this.m_TextureSlotCount), num);
					bool flag3 = this.m_UIRenderersWithDrawCallData.Count == 0 || this.m_UIRenderersWithDrawCallData[this.m_UIRenderersWithDrawCallData.Count - 1] != renderer;
					if (flag3)
					{
						this.m_UIRenderersWithDrawCallData.Add(renderer);
					}
				}
			}
			this.m_DefaultCommandList.Reset();
		}

		private protected bool disposed { protected get; private set; }

		public void Dispose()
		{
			this.Dispose(true);
		}

		public void ResetUIRendererDrawCallData()
		{
			foreach (UIRenderer uirenderer in this.m_UIRenderersWithDrawCallData)
			{
				bool flag = uirenderer != null;
				if (flag)
				{
					uirenderer.ResetDrawCallData();
				}
			}
			this.m_UIRenderersWithDrawCallData.Clear();
		}

		protected void Dispose(bool disposing)
		{
			bool disposed = this.disposed;
			if (!disposed)
			{
				if (disposing)
				{
					this.m_DefaultCommandList.Dispose();
					this.m_DefaultCommandList = null;
					for (int i = 0; i < this.m_CommandListsArray.Length; i++)
					{
						List<CommandList> list = this.m_CommandListsArray[i];
						for (int j = 0; j < list.Count; j++)
						{
							list[j].Dispose();
						}
						list.Clear();
					}
					this.m_CommandListsArray = null;
				}
				this.disposed = true;
			}
		}

		private readonly IntPtr m_VertexDecl;

		private readonly IntPtr m_DefaultStencilState;

		private uint m_CurrentIndex = 3U;

		private Stack<CommandList> m_CommandListPool = new Stack<CommandList>();

		private CommandList m_DefaultCommandList = new CommandList(IntPtr.Zero, IntPtr.Zero);

		private List<CommandList>[] m_CommandListsArray;

		private List<CommandList> m_CurrentFrameCommandLists;

		private List<UIRenderer> m_UIRenderersWithDrawCallData = new List<UIRenderer>();

		private TextureSlotCount m_TextureSlotCount;

		public static class Testing
		{
			public static List<CommandList> GetCurrentFrameCommandLists(CommandListManager instance)
			{
				return instance.m_CurrentFrameCommandLists;
			}
		}
	}
}
