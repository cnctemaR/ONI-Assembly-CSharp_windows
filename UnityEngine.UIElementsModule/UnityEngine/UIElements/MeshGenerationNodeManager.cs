using System;
using System.Collections.Generic;
using UnityEngine.UIElements.UIR;

namespace UnityEngine.UIElements
{
	internal class MeshGenerationNodeManager : IDisposable
	{
		public MeshGenerationNodeManager(EntryRecorder entryRecorder)
		{
			this.m_EntryRecorder = entryRecorder;
		}

		public void CreateNode(Entry parentEntry, out MeshGenerationNode node)
		{
			MeshGenerationNodeImpl meshGenerationNodeImpl = this.CreateImpl(parentEntry, true);
			meshGenerationNodeImpl.GetNode(out node);
		}

		public void CreateUnsafeNode(Entry parentEntry, out UnsafeMeshGenerationNode node)
		{
			MeshGenerationNodeImpl meshGenerationNodeImpl = this.CreateImpl(parentEntry, false);
			meshGenerationNodeImpl.GetUnsafeNode(out node);
		}

		private MeshGenerationNodeImpl CreateImpl(Entry parentEntry, bool safe)
		{
			bool disposed = this.disposed;
			MeshGenerationNodeImpl meshGenerationNodeImpl;
			if (disposed)
			{
				DisposeHelper.NotifyDisposedUsed(this);
				meshGenerationNodeImpl = null;
			}
			else
			{
				bool flag = this.m_Nodes.Count == this.m_UsedCounter;
				if (flag)
				{
					for (int i = 0; i < 200; i++)
					{
						this.m_Nodes.Add(new MeshGenerationNodeImpl());
					}
				}
				List<MeshGenerationNodeImpl> nodes = this.m_Nodes;
				int usedCounter = this.m_UsedCounter;
				this.m_UsedCounter = usedCounter + 1;
				MeshGenerationNodeImpl meshGenerationNodeImpl2 = nodes[usedCounter];
				meshGenerationNodeImpl2.Init(parentEntry, this.m_EntryRecorder, safe);
				meshGenerationNodeImpl = meshGenerationNodeImpl2;
			}
			return meshGenerationNodeImpl;
		}

		public void ResetAll()
		{
			for (int i = 0; i < this.m_UsedCounter; i++)
			{
				this.m_Nodes[i].Reset();
			}
			this.m_UsedCounter = 0;
		}

		private protected bool disposed { protected get; private set; }

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing)
		{
			bool disposed = this.disposed;
			if (!disposed)
			{
				if (disposing)
				{
					int i = 0;
					int count = this.m_Nodes.Count;
					while (i < count)
					{
						this.m_Nodes[i].Dispose();
						i++;
					}
					this.m_Nodes.Clear();
				}
				this.disposed = true;
			}
		}

		private List<MeshGenerationNodeImpl> m_Nodes = new List<MeshGenerationNodeImpl>(8);

		private int m_UsedCounter;

		private EntryRecorder m_EntryRecorder;
	}
}
