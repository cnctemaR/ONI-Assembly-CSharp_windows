using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements.UIR
{
	internal struct DepthOrderedDirtyTracking
	{
		public void EnsureFits(int maxDepth)
		{
			while (this.heads.Count <= maxDepth)
			{
				this.heads.Add(null);
				this.tails.Add(null);
			}
		}

		public void RegisterDirty(RenderData renderData, RenderDataDirtyTypes dirtyTypes, RenderDataDirtyTypeClasses dirtyTypeClass)
		{
			Debug.Assert(renderData.renderTree == this.owner);
			Debug.Assert(dirtyTypes > RenderDataDirtyTypes.None);
			int depthInRenderTree = renderData.depthInRenderTree;
			this.minDepths[(int)dirtyTypeClass] = ((depthInRenderTree < this.minDepths[(int)dirtyTypeClass]) ? depthInRenderTree : this.minDepths[(int)dirtyTypeClass]);
			this.maxDepths[(int)dirtyTypeClass] = ((depthInRenderTree > this.maxDepths[(int)dirtyTypeClass]) ? depthInRenderTree : this.maxDepths[(int)dirtyTypeClass]);
			bool flag = renderData.dirtiedValues > RenderDataDirtyTypes.None;
			if (flag)
			{
				renderData.dirtiedValues |= dirtyTypes;
			}
			else
			{
				renderData.dirtiedValues = dirtyTypes;
				bool flag2 = this.tails[depthInRenderTree] != null;
				if (flag2)
				{
					this.tails[depthInRenderTree].nextDirty = renderData;
					renderData.prevDirty = this.tails[depthInRenderTree];
					this.tails[depthInRenderTree] = renderData;
				}
				else
				{
					List<RenderData> list = this.heads;
					int num = depthInRenderTree;
					this.tails[depthInRenderTree] = renderData;
					list[num] = renderData;
				}
			}
		}

		public void ClearDirty(RenderData renderData, RenderDataDirtyTypes dirtyTypesInverse)
		{
			Debug.Assert(renderData.dirtiedValues > RenderDataDirtyTypes.None);
			renderData.dirtiedValues &= dirtyTypesInverse;
			bool flag = renderData.dirtiedValues == RenderDataDirtyTypes.None;
			if (flag)
			{
				bool flag2 = renderData.prevDirty != null;
				if (flag2)
				{
					renderData.prevDirty.nextDirty = renderData.nextDirty;
				}
				bool flag3 = renderData.nextDirty != null;
				if (flag3)
				{
					renderData.nextDirty.prevDirty = renderData.prevDirty;
				}
				bool flag4 = this.tails[renderData.depthInRenderTree] == renderData;
				if (flag4)
				{
					Debug.Assert(renderData.nextDirty == null);
					this.tails[renderData.depthInRenderTree] = renderData.prevDirty;
				}
				bool flag5 = this.heads[renderData.depthInRenderTree] == renderData;
				if (flag5)
				{
					Debug.Assert(renderData.prevDirty == null);
					this.heads[renderData.depthInRenderTree] = renderData.nextDirty;
				}
				renderData.prevDirty = (renderData.nextDirty = null);
			}
		}

		public void Reset()
		{
			for (int i = 0; i < this.minDepths.Length; i++)
			{
				this.minDepths[i] = int.MaxValue;
				this.maxDepths[i] = int.MinValue;
			}
		}

		public RenderTree owner;

		public List<RenderData> heads;

		public List<RenderData> tails;

		public int[] minDepths;

		public int[] maxDepths;

		public uint dirtyID;
	}
}
