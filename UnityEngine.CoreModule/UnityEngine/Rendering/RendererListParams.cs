using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.Rendering
{
	public struct RendererListParams : IEquatable<RendererListParams>
	{
		public RendererListParams(CullingResults cullingResults, DrawingSettings drawSettings, FilteringSettings filteringSettings)
		{
			this.cullingResults = cullingResults;
			this.drawSettings = drawSettings;
			this.filteringSettings = filteringSettings;
			this.tagName = ShaderTagId.none;
			this.isPassTagName = false;
			this.tagValues = null;
			this.stateBlocks = null;
		}

		internal int numStateBlocks
		{
			get
			{
				bool flag = this.tagValues != null;
				int num;
				if (flag)
				{
					num = this.tagValues.Value.Length;
				}
				else
				{
					num = 0;
				}
				return num;
			}
		}

		internal IntPtr stateBlocksPtr
		{
			get
			{
				bool flag = this.stateBlocks == null;
				IntPtr intPtr;
				if (flag)
				{
					intPtr = IntPtr.Zero;
				}
				else
				{
					intPtr = (IntPtr)this.stateBlocks.Value.GetUnsafeReadOnlyPtr<RenderStateBlock>();
				}
				return intPtr;
			}
		}

		internal IntPtr tagsValuePtr
		{
			get
			{
				bool flag = this.tagValues == null;
				IntPtr intPtr;
				if (flag)
				{
					intPtr = IntPtr.Zero;
				}
				else
				{
					intPtr = (IntPtr)this.tagValues.Value.GetUnsafeReadOnlyPtr<ShaderTagId>();
				}
				return intPtr;
			}
		}

		internal void Dispose()
		{
			bool flag = this.stateBlocks != null;
			if (flag)
			{
				this.stateBlocks.Value.Dispose();
				this.stateBlocks = null;
			}
			bool flag2 = this.tagValues != null;
			if (flag2)
			{
				this.tagValues.Value.Dispose();
				this.tagValues = null;
			}
		}

		internal void Validate()
		{
			bool flag = this.tagValues != null && this.stateBlocks != null;
			if (flag)
			{
				bool flag2 = this.tagValues.Value.Length != this.stateBlocks.Value.Length;
				if (flag2)
				{
					throw new ArgumentException(string.Format("Arrays {0} and {1} should have same length, but {2} had length {3} while {4} had length {5}.", new object[]
					{
						"tagValues",
						"stateBlocks",
						"tagValues",
						this.tagValues.Value.Length,
						"stateBlocks",
						this.stateBlocks.Value.Length
					}));
				}
			}
			else
			{
				bool flag3 = (this.tagValues != null && this.stateBlocks == null) || (this.tagValues == null && this.stateBlocks != null);
				if (flag3)
				{
					throw new ArgumentException(string.Format("Arrays {0} and {1} should have same length, but one of them is null ({2} : {3}, {4} : {5}).", new object[]
					{
						"tagValues",
						"stateBlocks",
						"tagValues",
						this.tagValues != null,
						"stateBlocks",
						this.stateBlocks != null
					}));
				}
			}
		}

		public bool Equals(RendererListParams other)
		{
			return this.cullingResults == other.cullingResults && this.drawSettings == other.drawSettings && this.filteringSettings == other.filteringSettings && this.tagName == other.tagName && this.isPassTagName == other.isPassTagName && this.tagValues == other.tagValues && this.stateBlocks == other.stateBlocks;
		}

		public override bool Equals(object obj)
		{
			bool flag = obj == null;
			return !flag && obj is RendererListParams && this.Equals((RendererListParams)obj);
		}

		public override int GetHashCode()
		{
			int num = this.cullingResults.GetHashCode();
			num = (num * 397) ^ this.drawSettings.GetHashCode();
			num = (num * 397) ^ this.filteringSettings.GetHashCode();
			num = (num * 397) ^ this.tagName.GetHashCode();
			num = (num * 397) ^ (this.isPassTagName ? 0 : 1);
			num = (num * 397) ^ this.tagValues.GetHashCode();
			return (num * 397) ^ this.stateBlocks.GetHashCode();
		}

		public static bool operator ==(RendererListParams left, RendererListParams right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(RendererListParams left, RendererListParams right)
		{
			return !left.Equals(right);
		}

		public static readonly RendererListParams Invalid = default(RendererListParams);

		public CullingResults cullingResults;

		public DrawingSettings drawSettings;

		public FilteringSettings filteringSettings;

		public ShaderTagId tagName;

		public bool isPassTagName;

		public NativeArray<ShaderTagId>? tagValues;

		public NativeArray<RenderStateBlock>? stateBlocks;
	}
}
