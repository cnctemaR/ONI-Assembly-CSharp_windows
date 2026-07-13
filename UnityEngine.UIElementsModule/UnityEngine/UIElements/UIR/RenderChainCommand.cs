using System;
using Unity.Profiling;

namespace UnityEngine.UIElements.UIR
{
	internal class RenderChainCommand : LinkedPoolItem<RenderChainCommand>
	{
		public RenderChainCommand()
		{
			this.Reset();
		}

		public void Reset()
		{
			this.owner = null;
			this.prev = (this.next = null);
			this.type = CommandType.Draw;
			this.flags = CommandFlags.None;
			this.material = null;
			this.userProps = null;
			this.texture = TextureId.invalid;
			this.stencilRef = 0;
			this.sdfScale = 0f;
			this.sharpness = 0f;
			this.mesh = null;
			this.indexOffset = (this.indexCount = 0);
			this.callback = null;
		}

		public void ExecuteNonDrawMesh(DrawParams drawParams, float pixelsPerPoint, ref Exception immediateException)
		{
			switch (this.type)
			{
			case CommandType.ImmediateCull:
			{
				bool flag = !RenderChainCommand.RectPointsToPixelsAndFlipYAxis(this.owner.owner.worldBound, pixelsPerPoint).Overlaps(Utility.GetActiveViewport());
				if (flag)
				{
					return;
				}
				break;
			}
			case CommandType.Immediate:
				break;
			case CommandType.PushView:
			{
				Matrix4x4 matrix4x;
				UIRUtility.ComputeMatrixRelativeToRenderTree(this.owner, out matrix4x);
				drawParams.view.Push(matrix4x);
				GL.modelview = matrix4x;
				RenderData parent = this.owner.parent;
				bool flag2 = parent != null;
				Rect rect;
				if (flag2)
				{
					rect = parent.clippingRect;
				}
				else
				{
					rect = DrawParams.k_FullNormalizedRect;
				}
				RenderChainCommand.PushScissor(drawParams, rect, pixelsPerPoint);
				return;
			}
			case CommandType.PopView:
				drawParams.view.Pop();
				GL.modelview = drawParams.view.Peek();
				RenderChainCommand.PopScissor(drawParams, pixelsPerPoint);
				return;
			case CommandType.PushScissor:
				RenderChainCommand.PushScissor(drawParams, this.owner.clippingRect, pixelsPerPoint);
				return;
			case CommandType.PopScissor:
				RenderChainCommand.PopScissor(drawParams, pixelsPerPoint);
				return;
			case CommandType.PushDefaultMaterial:
			case CommandType.PopDefaultMaterial:
				return;
			default:
				return;
			}
			bool flag3 = immediateException != null;
			if (!flag3)
			{
				bool flag4 = this.owner.compositeOpacity < 0.001f;
				if (!flag4)
				{
					Matrix4x4 unityProjectionMatrix = Utility.GetUnityProjectionMatrix();
					Camera current = Camera.current;
					RenderTexture active = RenderTexture.active;
					Matrix4x4 matrix4x2;
					UIRUtility.ComputeMatrixRelativeToRenderTree(this.owner, out matrix4x2);
					GL.modelview = matrix4x2;
					RenderChainCommand.PushScissor(drawParams, this.owner.clippingRect, pixelsPerPoint);
					try
					{
						this.callback();
					}
					catch (Exception ex)
					{
						immediateException = ex;
					}
					RenderChainCommand.PopScissor(drawParams, pixelsPerPoint);
					Camera.SetupCurrent(current);
					RenderTexture.active = active;
					GL.modelview = drawParams.view.Peek();
					GL.LoadProjectionMatrix(unityProjectionMatrix);
				}
			}
		}

		public static void PushScissor(DrawParams drawParams, Rect scissor, float pixelsPerPoint)
		{
			Rect rect = RenderChainCommand.CombineScissorRects(scissor, drawParams.scissor.Peek());
			drawParams.scissor.Push(rect);
			Utility.SetScissorRect(RenderChainCommand.RectPointsToPixelsAndFlipYAxis(rect, pixelsPerPoint));
		}

		public static void PopScissor(DrawParams drawParams, float pixelsPerPoint)
		{
			drawParams.scissor.Pop();
			Rect rect = drawParams.scissor.Peek();
			bool flag = rect.x == DrawParams.k_UnlimitedRect.x;
			if (flag)
			{
				Utility.DisableScissor();
			}
			else
			{
				Utility.SetScissorRect(RenderChainCommand.RectPointsToPixelsAndFlipYAxis(rect, pixelsPerPoint));
			}
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

		private static RectInt RectPointsToPixelsAndFlipYAxis(Rect rect, float pixelsPerPoint)
		{
			float num = (float)Utility.GetActiveViewport().height;
			return new RectInt(0, 0, 0, 0)
			{
				x = Mathf.RoundToInt(rect.x * pixelsPerPoint),
				y = Mathf.RoundToInt(num - rect.yMax * pixelsPerPoint),
				width = Mathf.RoundToInt(rect.width * pixelsPerPoint),
				height = Mathf.RoundToInt(rect.height * pixelsPerPoint)
			};
		}

		public RenderData owner;

		public RenderChainCommand prev;

		public RenderChainCommand next;

		public CommandType type;

		public CommandFlags flags;

		public Material material;

		public MaterialPropertyBlock userProps;

		public TextureId texture;

		public int stencilRef;

		public float sdfScale;

		public float sharpness;

		public MeshHandle mesh;

		public int indexOffset;

		public int indexCount;

		public Action callback;

		private static ProfilerMarker s_ImmediateOverheadMarker = new ProfilerMarker("UIR.ImmediateOverhead");
	}
}
