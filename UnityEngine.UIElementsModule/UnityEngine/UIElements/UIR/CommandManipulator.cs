using System;

namespace UnityEngine.UIElements.UIR
{
	internal static class CommandManipulator
	{
		public static void ReplaceHeadCommands(RenderTreeManager renderTreeManager, RenderData renderData, EntryProcessor processor)
		{
			bool flag = false;
			RenderChainCommand renderChainCommand = null;
			RenderChainCommand renderChainCommand2 = null;
			bool flag2 = renderData.firstHeadCommand != null;
			if (flag2)
			{
				renderChainCommand = renderData.firstHeadCommand.prev;
				renderChainCommand2 = renderData.lastHeadCommand.next;
				CommandManipulator.RemoveChain(renderData.renderTree, renderData.firstHeadCommand, renderData.lastHeadCommand);
				flag = true;
			}
			bool flag3 = processor.firstHeadCommand != null;
			if (flag3)
			{
				bool flag4 = !flag;
				if (flag4)
				{
					CommandManipulator.FindHeadCommandInsertionPoint(renderData, out renderChainCommand, out renderChainCommand2);
				}
				bool flag5 = renderChainCommand != null;
				if (flag5)
				{
					processor.firstHeadCommand.prev = renderChainCommand;
					renderChainCommand.next = processor.firstHeadCommand;
				}
				bool flag6 = renderChainCommand2 != null;
				if (flag6)
				{
					processor.lastHeadCommand.next = renderChainCommand2;
					renderChainCommand2.prev = processor.lastHeadCommand;
				}
				renderData.renderTree.OnRenderCommandAdded(processor.firstHeadCommand);
			}
			renderData.firstHeadCommand = processor.firstHeadCommand;
			renderData.lastHeadCommand = processor.lastHeadCommand;
		}

		public static void ReplaceTailCommands(RenderTreeManager renderTreeManager, RenderData renderData, EntryProcessor processor)
		{
			bool flag = false;
			RenderChainCommand renderChainCommand = null;
			RenderChainCommand renderChainCommand2 = null;
			bool flag2 = renderData.firstTailCommand != null;
			if (flag2)
			{
				renderChainCommand = renderData.firstTailCommand.prev;
				renderChainCommand2 = renderData.lastTailCommand.next;
				CommandManipulator.RemoveChain(renderData.renderTree, renderData.firstTailCommand, renderData.lastTailCommand);
				flag = true;
			}
			bool flag3 = processor.firstTailCommand != null;
			if (flag3)
			{
				bool flag4 = !flag;
				if (flag4)
				{
					CommandManipulator.FindTailCommandInsertionPoint(renderData, out renderChainCommand, out renderChainCommand2);
				}
				bool flag5 = renderChainCommand != null;
				if (flag5)
				{
					processor.firstTailCommand.prev = renderChainCommand;
					renderChainCommand.next = processor.firstTailCommand;
				}
				bool flag6 = renderChainCommand2 != null;
				if (flag6)
				{
					processor.lastTailCommand.next = renderChainCommand2;
					renderChainCommand2.prev = processor.lastTailCommand;
				}
				renderData.renderTree.OnRenderCommandAdded(processor.firstTailCommand);
			}
			renderData.firstTailCommand = processor.firstTailCommand;
			renderData.lastTailCommand = processor.lastTailCommand;
		}

		private static RenderChainCommand FindPrevCommand(RenderData candidate, bool searchFromHead)
		{
			for (;;)
			{
				bool flag = !searchFromHead;
				if (flag)
				{
					bool flag2 = candidate.lastTailCommand != null;
					if (flag2)
					{
						break;
					}
					bool flag3 = candidate.lastChild != null;
					if (flag3)
					{
						candidate = candidate.lastChild;
						continue;
					}
				}
				searchFromHead = false;
				bool flag4 = candidate.lastHeadCommand != null;
				if (flag4)
				{
					goto Block_4;
				}
				bool flag5 = candidate.prevSibling != null;
				if (flag5)
				{
					candidate = candidate.prevSibling;
				}
				else
				{
					bool flag6 = candidate.parent == null;
					if (flag6)
					{
						goto Block_6;
					}
					candidate = candidate.parent;
					searchFromHead = true;
				}
			}
			return candidate.lastTailCommand;
			Block_4:
			return candidate.lastHeadCommand;
			Block_6:
			return null;
		}

		private static void FindHeadCommandInsertionPoint(RenderData renderData, out RenderChainCommand prev, out RenderChainCommand next)
		{
			Debug.Assert(renderData.firstHeadCommand == null);
			prev = CommandManipulator.FindPrevCommand(renderData, true);
			bool flag = prev == null;
			if (flag)
			{
				next = renderData.renderTree.firstCommand;
			}
			else
			{
				next = prev.next;
			}
		}

		private static void FindTailCommandInsertionPoint(RenderData renderData, out RenderChainCommand prev, out RenderChainCommand next)
		{
			Debug.Assert(renderData.firstTailCommand == null);
			prev = CommandManipulator.FindPrevCommand(renderData, false);
			Debug.Assert(prev != null);
			next = prev.next;
		}

		private static void RemoveChain(RenderTree renderTree, RenderChainCommand first, RenderChainCommand last)
		{
			Debug.Assert(first != null);
			Debug.Assert(last != null);
			renderTree.OnRenderCommandsRemoved(first, last);
			bool flag = first.prev != null;
			if (flag)
			{
				first.prev.next = last.next;
			}
			bool flag2 = last.next != null;
			if (flag2)
			{
				last.next.prev = first.prev;
			}
			RenderChainCommand renderChainCommand = first;
			RenderChainCommand renderChainCommand2;
			do
			{
				RenderChainCommand next = renderChainCommand.next;
				renderTree.renderTreeManager.FreeCommand(renderChainCommand);
				renderChainCommand2 = renderChainCommand;
				renderChainCommand = next;
			}
			while (renderChainCommand2 != last);
		}

		public static void ResetCommands(RenderTreeManager renderTreeManager, RenderData renderData)
		{
			bool flag = renderData.firstHeadCommand != null;
			if (flag)
			{
				renderData.renderTree.OnRenderCommandsRemoved(renderData.firstHeadCommand, renderData.lastHeadCommand);
			}
			RenderChainCommand renderChainCommand = ((renderData.firstHeadCommand != null) ? renderData.firstHeadCommand.prev : null);
			RenderChainCommand renderChainCommand2 = ((renderData.lastHeadCommand != null) ? renderData.lastHeadCommand.next : null);
			Debug.Assert(renderChainCommand == null || renderChainCommand.owner != renderData);
			Debug.Assert(renderChainCommand2 == null || renderChainCommand2 == renderData.firstTailCommand || renderChainCommand2.owner != renderData);
			bool flag2 = renderChainCommand != null;
			if (flag2)
			{
				renderChainCommand.next = renderChainCommand2;
			}
			bool flag3 = renderChainCommand2 != null;
			if (flag3)
			{
				renderChainCommand2.prev = renderChainCommand;
			}
			bool flag4 = renderData.firstHeadCommand != null;
			if (flag4)
			{
				RenderChainCommand renderChainCommand3;
				RenderChainCommand next;
				for (renderChainCommand3 = renderData.firstHeadCommand; renderChainCommand3 != renderData.lastHeadCommand; renderChainCommand3 = next)
				{
					next = renderChainCommand3.next;
					renderTreeManager.FreeCommand(renderChainCommand3);
				}
				renderTreeManager.FreeCommand(renderChainCommand3);
			}
			renderData.firstHeadCommand = (renderData.lastHeadCommand = null);
			renderChainCommand = ((renderData.firstTailCommand != null) ? renderData.firstTailCommand.prev : null);
			renderChainCommand2 = ((renderData.lastTailCommand != null) ? renderData.lastTailCommand.next : null);
			Debug.Assert(renderChainCommand == null || renderChainCommand.owner != renderData);
			Debug.Assert(renderChainCommand2 == null || renderChainCommand2.owner != renderData);
			bool flag5 = renderChainCommand != null;
			if (flag5)
			{
				renderChainCommand.next = renderChainCommand2;
			}
			bool flag6 = renderChainCommand2 != null;
			if (flag6)
			{
				renderChainCommand2.prev = renderChainCommand;
			}
			bool flag7 = renderData.firstTailCommand != null;
			if (flag7)
			{
				renderData.renderTree.OnRenderCommandsRemoved(renderData.firstTailCommand, renderData.lastTailCommand);
				RenderChainCommand renderChainCommand4;
				RenderChainCommand next2;
				for (renderChainCommand4 = renderData.firstTailCommand; renderChainCommand4 != renderData.lastTailCommand; renderChainCommand4 = next2)
				{
					next2 = renderChainCommand4.next;
					renderTreeManager.FreeCommand(renderChainCommand4);
				}
				renderTreeManager.FreeCommand(renderChainCommand4);
			}
			renderData.firstTailCommand = (renderData.lastTailCommand = null);
		}

		private static void InjectCommandInBetween(RenderChainCommand cmd, bool isHeadCommand, RenderChainCommand prev, RenderChainCommand next)
		{
			bool flag = prev != null;
			if (flag)
			{
				cmd.prev = prev;
				prev.next = cmd;
			}
			bool flag2 = next != null;
			if (flag2)
			{
				cmd.next = next;
				next.prev = cmd;
			}
			RenderData owner = cmd.owner;
			if (isHeadCommand)
			{
				bool flag3 = owner.firstHeadCommand == null || owner.firstHeadCommand == next;
				if (flag3)
				{
					owner.firstHeadCommand = cmd;
				}
				bool flag4 = owner.lastHeadCommand == null || owner.lastHeadCommand == prev;
				if (flag4)
				{
					owner.lastHeadCommand = cmd;
				}
			}
			else
			{
				bool flag5 = owner.firstTailCommand == null || owner.firstTailCommand == next;
				if (flag5)
				{
					owner.firstTailCommand = cmd;
				}
				bool flag6 = owner.lastTailCommand == null || owner.lastTailCommand == prev;
				if (flag6)
				{
					owner.lastTailCommand = cmd;
				}
			}
			owner.renderTree.OnRenderCommandAdded(cmd);
		}

		public static void DisableElementRendering(RenderTreeManager renderTreeManager, VisualElement ve, bool renderingDisabled)
		{
			RenderData renderData = ve.renderData;
			bool flag = renderData == null;
			if (!flag)
			{
				if (renderingDisabled)
				{
					bool flag2 = renderData.firstHeadCommand == null || renderData.firstHeadCommand.type != CommandType.BeginDisable;
					if (flag2)
					{
						RenderChainCommand renderChainCommand = renderTreeManager.AllocCommand();
						renderChainCommand.type = CommandType.BeginDisable;
						renderChainCommand.owner = renderData;
						bool flag3 = renderData.firstHeadCommand == null;
						if (flag3)
						{
							RenderChainCommand renderChainCommand2;
							RenderChainCommand renderChainCommand3;
							CommandManipulator.FindHeadCommandInsertionPoint(renderData, out renderChainCommand2, out renderChainCommand3);
							CommandManipulator.InjectCommandInBetween(renderChainCommand, true, renderChainCommand2, renderChainCommand3);
						}
						else
						{
							RenderChainCommand prev = renderData.firstHeadCommand.prev;
							RenderChainCommand firstHeadCommand = renderData.firstHeadCommand;
							RenderChainCommand lastHeadCommand = renderData.lastHeadCommand;
							Debug.Assert(lastHeadCommand != null);
							renderData.firstHeadCommand = null;
							CommandManipulator.InjectCommandInBetween(renderChainCommand, true, prev, firstHeadCommand);
							renderData.lastHeadCommand = lastHeadCommand;
						}
					}
					bool flag4 = renderData.lastTailCommand == null || renderData.lastTailCommand.type != CommandType.EndDisable;
					if (flag4)
					{
						RenderChainCommand renderChainCommand4 = renderTreeManager.AllocCommand();
						renderChainCommand4.type = CommandType.EndDisable;
						renderChainCommand4.owner = renderData;
						bool flag5 = renderData.lastTailCommand == null;
						if (flag5)
						{
							RenderChainCommand renderChainCommand5;
							RenderChainCommand renderChainCommand6;
							CommandManipulator.FindTailCommandInsertionPoint(renderData, out renderChainCommand5, out renderChainCommand6);
							CommandManipulator.InjectCommandInBetween(renderChainCommand4, false, renderChainCommand5, renderChainCommand6);
						}
						else
						{
							RenderChainCommand lastTailCommand = renderData.lastTailCommand;
							RenderChainCommand next = renderData.lastTailCommand.next;
							Debug.Assert(renderData.firstTailCommand != null);
							CommandManipulator.InjectCommandInBetween(renderChainCommand4, false, lastTailCommand, next);
						}
					}
				}
				else
				{
					bool flag6 = renderData.firstHeadCommand != null && renderData.firstHeadCommand.type == CommandType.BeginDisable;
					if (flag6)
					{
						CommandManipulator.RemoveSingleCommand(renderTreeManager, renderData, renderData.firstHeadCommand);
					}
					bool flag7 = renderData.lastTailCommand != null && renderData.lastTailCommand.type == CommandType.EndDisable;
					if (flag7)
					{
						CommandManipulator.RemoveSingleCommand(renderTreeManager, renderData, renderData.lastTailCommand);
					}
				}
			}
		}

		private static void RemoveSingleCommand(RenderTreeManager renderTreeManager, RenderData renderData, RenderChainCommand cmd)
		{
			Debug.Assert(cmd != null);
			Debug.Assert(cmd.owner == renderData);
			renderData.renderTree.OnRenderCommandsRemoved(cmd, cmd);
			RenderChainCommand prev = cmd.prev;
			RenderChainCommand next = cmd.next;
			bool flag = prev != null;
			if (flag)
			{
				prev.next = next;
			}
			bool flag2 = next != null;
			if (flag2)
			{
				next.prev = prev;
			}
			bool flag3 = renderData.firstHeadCommand == cmd;
			if (flag3)
			{
				bool flag4 = renderData.firstHeadCommand == renderData.lastHeadCommand;
				if (flag4)
				{
					RenderChainCommand prev2 = cmd.prev;
					Debug.Assert(((prev2 != null) ? prev2.owner : null) != renderData, "When removing the first head command, the command before this one in the queue should belong to an other parent");
					RenderChainCommand next2 = cmd.next;
					Debug.Assert(((next2 != null) ? next2.owner : null) != renderData || cmd.next == renderData.firstTailCommand);
					renderData.firstHeadCommand = null;
					renderData.lastHeadCommand = null;
				}
				else
				{
					Debug.Assert(cmd.next.owner == renderData);
					Debug.Assert(renderData.lastHeadCommand != null);
					renderData.firstHeadCommand = cmd.next;
				}
			}
			else
			{
				bool flag5 = renderData.lastHeadCommand == cmd;
				if (flag5)
				{
					Debug.Assert(cmd.prev.owner == renderData);
					Debug.Assert(renderData.firstHeadCommand != null);
					renderData.lastHeadCommand = cmd.prev;
				}
			}
			bool flag6 = renderData.firstTailCommand == cmd;
			if (flag6)
			{
				bool flag7 = renderData.firstTailCommand == renderData.lastTailCommand;
				if (flag7)
				{
					RenderChainCommand prev3 = cmd.prev;
					Debug.Assert(((prev3 != null) ? prev3.owner : null) != renderData || cmd.prev == renderData.lastHeadCommand);
					RenderChainCommand next3 = cmd.next;
					Debug.Assert(((next3 != null) ? next3.owner : null) != renderData);
					renderData.firstTailCommand = null;
					renderData.lastTailCommand = null;
				}
				else
				{
					Debug.Assert(cmd.next.owner == renderData);
					Debug.Assert(renderData.lastTailCommand != null);
					renderData.firstTailCommand = cmd.next;
				}
			}
			else
			{
				bool flag8 = renderData.lastTailCommand == cmd;
				if (flag8)
				{
					Debug.Assert(cmd.prev.owner == renderData);
					Debug.Assert(renderData.firstTailCommand != null);
					renderData.lastTailCommand = cmd.prev;
				}
			}
			renderTreeManager.FreeCommand(cmd);
		}
	}
}
