using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UnityEngine.UIElements.UIR
{
	internal class EntryPreProcessor
	{
		public int childrenIndex
		{
			get
			{
				return this.m_ChildrenIndex;
			}
		}

		public List<EntryPreProcessor.AllocSize> headAllocs
		{
			get
			{
				return this.m_HeadAllocs;
			}
		}

		public List<EntryPreProcessor.AllocSize> tailAllocs
		{
			get
			{
				return this.m_TailAllocs;
			}
		}

		public List<Entry> flattenedEntries
		{
			get
			{
				return this.m_FlattenedEntries;
			}
		}

		public void PreProcess(Entry root)
		{
			this.m_ChildrenIndex = -1;
			this.m_FlattenedEntries.Clear();
			this.m_HeadAllocs.Clear();
			this.m_TailAllocs.Clear();
			this.m_Allocs = this.m_HeadAllocs;
			this.DoEvaluate(root);
			this.Flush();
			Debug.Assert(!this.m_IsPushingMask);
			Debug.Assert(this.m_Mask.Count == 0);
			Debug.Assert(this.m_ChildrenIndex >= 0);
		}

		public void ClearReferences()
		{
			this.m_FlattenedEntries.Clear();
		}

		private void DoEvaluate(Entry entry)
		{
			while (entry != null)
			{
				bool flag = entry.type != EntryType.DedicatedPlaceholder;
				if (flag)
				{
					this.m_FlattenedEntries.Add(entry);
				}
				switch (entry.type)
				{
				case EntryType.DrawSolidMesh:
				case EntryType.DrawTexturedMesh:
				case EntryType.DrawTexturedMeshSkipAtlas:
				case EntryType.DrawDynamicTexturedMesh:
				case EntryType.DrawTextMesh:
				case EntryType.DrawGradients:
					Debug.Assert((long)entry.vertices.Length <= (long)((ulong)UIRenderDevice.maxVerticesPerPage));
					this.Add(entry.vertices.Length, entry.indices.Length);
					break;
				case EntryType.DrawImmediate:
				case EntryType.DrawImmediateCull:
				case EntryType.PushClippingRect:
				case EntryType.PopClippingRect:
				case EntryType.PushScissors:
				case EntryType.PopScissors:
				case EntryType.PushGroupMatrix:
				case EntryType.PopGroupMatrix:
				case EntryType.PushDefaultMaterial:
				case EntryType.PopDefaultMaterial:
				case EntryType.CutRenderChain:
				case EntryType.DedicatedPlaceholder:
					break;
				case EntryType.DrawChildren:
					Debug.Assert(!this.m_IsPushingMask);
					Debug.Assert(this.m_ChildrenIndex == -1);
					this.Flush();
					this.m_ChildrenIndex = this.m_FlattenedEntries.Count - 1;
					this.m_Allocs = this.tailAllocs;
					break;
				case EntryType.BeginStencilMask:
					Debug.Assert(!this.m_IsPushingMask);
					this.m_IsPushingMask = true;
					break;
				case EntryType.EndStencilMask:
					Debug.Assert(this.m_IsPushingMask);
					this.m_IsPushingMask = false;
					break;
				case EntryType.PopStencilMask:
					for (;;)
					{
						EntryPreProcessor.AllocSize allocSize;
						bool flag2 = this.m_Mask.TryPop(out allocSize);
						if (!flag2)
						{
							break;
						}
						this.Add(allocSize.vertexCount, allocSize.indexCount);
					}
					break;
				default:
					throw new NotImplementedException();
				}
				bool flag3 = entry.firstChild != null;
				if (flag3)
				{
					this.DoEvaluate(entry.firstChild);
				}
				entry = entry.nextSibling;
			}
		}

		private void Add(int vertexCount, int indexCount)
		{
			bool flag = vertexCount == 0 || indexCount == 0;
			if (!flag)
			{
				int num = this.m_Pending.vertexCount + vertexCount;
				bool flag2 = (long)num <= (long)((ulong)UIRenderDevice.maxVerticesPerPage);
				if (flag2)
				{
					this.m_Pending.vertexCount = num;
					this.m_Pending.indexCount = this.m_Pending.indexCount + indexCount;
				}
				else
				{
					this.Flush();
					this.m_Pending.vertexCount = vertexCount;
					this.m_Pending.indexCount = indexCount;
				}
				bool isPushingMask = this.m_IsPushingMask;
				if (isPushingMask)
				{
					this.m_Mask.Push(new EntryPreProcessor.AllocSize
					{
						vertexCount = vertexCount,
						indexCount = indexCount
					});
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Flush()
		{
			bool flag = this.m_Pending.vertexCount > 0;
			if (flag)
			{
				this.m_Allocs.Add(this.m_Pending);
				this.m_Pending = default(EntryPreProcessor.AllocSize);
			}
		}

		private int m_ChildrenIndex;

		private List<EntryPreProcessor.AllocSize> m_Allocs;

		private List<EntryPreProcessor.AllocSize> m_HeadAllocs = new List<EntryPreProcessor.AllocSize>(1);

		private List<EntryPreProcessor.AllocSize> m_TailAllocs = new List<EntryPreProcessor.AllocSize>(1);

		private List<Entry> m_FlattenedEntries = new List<Entry>(8);

		private EntryPreProcessor.AllocSize m_Pending;

		private Stack<EntryPreProcessor.AllocSize> m_Mask = new Stack<EntryPreProcessor.AllocSize>(1);

		private bool m_IsPushingMask;

		public struct AllocSize
		{
			public int vertexCount;

			public int indexCount;
		}
	}
}
