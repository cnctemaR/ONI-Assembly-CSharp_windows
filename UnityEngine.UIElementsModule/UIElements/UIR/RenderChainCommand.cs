using System;

namespace UnityEngine.UIElements.UIR
{
	internal class RenderChainCommand : PoolItem
	{
		internal void Reset()
		{
			this.owner = null;
			this.prev = (this.next = null);
			this.closing = false;
			this.type = CommandType.Draw;
			this.state = default(State);
			this.mesh = null;
			this.indexOffset = (this.indexCount = 0);
			this.callback = null;
		}

		internal void ExecuteNonDrawMesh(DrawParams drawParams, bool straightY, float pixelsPerPoint, ref Exception immediateException)
		{
			switch (this.type)
			{
			case CommandType.Immediate:
			{
				bool flag = immediateException != null;
				if (!flag)
				{
					bool flag2 = drawParams.scissor.Count > 1;
					bool flag3 = flag2;
					if (flag3)
					{
						Utility.DisableScissor();
					}
					Utility.ProfileImmediateRendererBegin();
					try
					{
						using (new GUIClip.ParentClipScope(this.owner.worldTransform, this.owner.worldClip))
						{
							this.callback();
						}
					}
					catch (Exception ex)
					{
						immediateException = ex;
					}
					GL.modelview = drawParams.view.Peek().transform;
					GL.LoadProjectionMatrix(drawParams.projection);
					Utility.ProfileImmediateRendererEnd();
					bool flag4 = flag2;
					if (flag4)
					{
						Utility.SetScissorRect(RenderChainCommand.RectPointsToPixelsAndFlipYAxis(drawParams.scissor.Peek(), drawParams.viewport, pixelsPerPoint));
					}
				}
				break;
			}
			case CommandType.PushView:
			{
				ViewTransform viewTransform = new ViewTransform
				{
					transform = this.owner.worldTransform,
					clipRect = RenderChainCommand.RectToScreenSpace(this.owner.worldClip, drawParams.projection, straightY)
				};
				drawParams.view.Push(viewTransform);
				GL.modelview = viewTransform.transform;
				break;
			}
			case CommandType.PopView:
				drawParams.view.Pop();
				GL.modelview = drawParams.view.Peek().transform;
				break;
			case CommandType.PushScissor:
			{
				Rect rect = RenderChainCommand.CombineScissorRects(this.owner.worldClip, drawParams.scissor.Peek());
				drawParams.scissor.Push(rect);
				Utility.SetScissorRect(RenderChainCommand.RectPointsToPixelsAndFlipYAxis(rect, drawParams.viewport, pixelsPerPoint));
				break;
			}
			case CommandType.PopScissor:
			{
				drawParams.scissor.Pop();
				Rect rect2 = drawParams.scissor.Peek();
				bool flag5 = rect2.x == DrawParams.k_UnlimitedRect.x;
				if (flag5)
				{
					Utility.DisableScissor();
				}
				else
				{
					Utility.SetScissorRect(RenderChainCommand.RectPointsToPixelsAndFlipYAxis(rect2, drawParams.viewport, pixelsPerPoint));
				}
				break;
			}
			}
		}

		private static Vector4 RectToScreenSpace(Rect rc, Matrix4x4 projection, bool straightY)
		{
			RectInt activeViewport = Utility.GetActiveViewport();
			Vector3 vector = projection.MultiplyPoint(new Vector3(rc.xMin, rc.yMin, 0.995f));
			Vector3 vector2 = projection.MultiplyPoint(new Vector3(rc.xMax, rc.yMax, 0.995f));
			float num = (straightY ? 0.5f : (-0.5f));
			float num2 = (vector.x * 0.5f + 0.5f) * (float)activeViewport.width;
			float num3 = (vector2.x * 0.5f + 0.5f) * (float)activeViewport.width;
			float num4 = (vector.y * num + 0.5f) * (float)activeViewport.height;
			float num5 = (vector2.y * num + 0.5f) * (float)activeViewport.height;
			return new Vector4(Mathf.Min(num2, num3), Mathf.Min(num4, num5), Mathf.Max(num2, num3), Mathf.Max(num4, num5));
		}

		private static Rect CombineScissorRects(Rect r0, Rect r1)
		{
			Rect rect = new Rect(0f, 0f, 0f, 0f);
			rect.x = Math.Max(r0.x, r1.x);
			rect.y = Math.Max(r0.y, r1.y);
			rect.xMax = Math.Max(rect.x, Math.Min(r0.xMax, r1.xMax));
			rect.yMax = Math.Max(rect.y, Math.Min(r0.yMax, r1.yMax));
			return rect;
		}

		private static RectInt RectPointsToPixelsAndFlipYAxis(Rect rect, Rect viewport, float pixelsPerPoint)
		{
			return new RectInt(0, 0, 0, 0)
			{
				x = Mathf.RoundToInt(rect.x * pixelsPerPoint),
				y = Mathf.RoundToInt((viewport.height - rect.yMax) * pixelsPerPoint),
				width = Mathf.RoundToInt(rect.width * pixelsPerPoint),
				height = Mathf.RoundToInt(rect.height * pixelsPerPoint)
			};
		}

		internal VisualElement owner;

		internal RenderChainCommand prev;

		internal RenderChainCommand next;

		internal bool closing;

		internal CommandType type;

		internal State state;

		internal MeshHandle mesh;

		internal int indexOffset;

		internal int indexCount;

		internal Action callback;
	}
}
