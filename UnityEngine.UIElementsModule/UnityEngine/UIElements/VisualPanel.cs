using System;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.UIElements
{
	internal readonly struct VisualPanel
	{
		public static VisualPanel Null
		{
			get
			{
				return new VisualPanel(null, VisualPanelHandle.Null);
			}
		}

		public VisualPanelHandle Handle
		{
			get
			{
				return this.m_Handle;
			}
		}

		public bool IsCreated
		{
			get
			{
				return !this.m_Handle.Equals(VisualPanelHandle.Null) && this.m_Manager.ContainsPanel(in this.m_Handle);
			}
		}

		internal ref VisualPanelData Data
		{
			get
			{
				return UnsafeUtility.AsRef<VisualPanelData>(this.m_Manager.GetPanelDataPtr(in this.m_Handle));
			}
		}

		public ref VisualNodeHandle RootContainer
		{
			get
			{
				return ref this.Data.RootContainer;
			}
		}

		public ref bool DuringLayoutPhase
		{
			get
			{
				return ref this.Data.DuringLayoutPhase;
			}
		}

		public VisualNode GetRootContainer()
		{
			return new VisualNode(this.m_Manager, this.Data.RootContainer);
		}

		public void SetRootContainer(VisualNode node)
		{
			this.Data.RootContainer = node.Handle;
		}

		internal VisualPanel(VisualManager manager, VisualPanelHandle handle)
		{
			this.m_Manager = manager;
			this.m_Handle = handle;
		}

		public void Destroy()
		{
			this.m_Manager.RemovePanel(in this.m_Handle);
		}

		public BaseVisualElementPanel GetOwner()
		{
			return this.m_Manager.GetOwner(in this.m_Handle);
		}

		public void SetOwner(BaseVisualElementPanel owner)
		{
			this.m_Manager.SetOwner(in this.m_Handle, owner);
		}

		private readonly VisualManager m_Manager;

		private readonly VisualPanelHandle m_Handle;
	}
}
