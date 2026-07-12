using System;

namespace UnityEngine.Android
{
	public class GetAssetPackStateAsyncOperation : CustomYieldInstruction
	{
		public override bool keepWaiting
		{
			get
			{
				object operationLock = this.m_OperationLock;
				bool flag;
				lock (operationLock)
				{
					flag = this.m_States == null;
				}
				return flag;
			}
		}

		public bool isDone
		{
			get
			{
				return !this.keepWaiting;
			}
		}

		public ulong size
		{
			get
			{
				object operationLock = this.m_OperationLock;
				ulong size;
				lock (operationLock)
				{
					size = this.m_Size;
				}
				return size;
			}
		}

		public AndroidAssetPackState[] states
		{
			get
			{
				object operationLock = this.m_OperationLock;
				AndroidAssetPackState[] states;
				lock (operationLock)
				{
					states = this.m_States;
				}
				return states;
			}
		}

		internal GetAssetPackStateAsyncOperation()
		{
			this.m_OperationLock = new object();
		}

		internal void OnResult(ulong size, AndroidAssetPackState[] states)
		{
			object operationLock = this.m_OperationLock;
			lock (operationLock)
			{
				this.m_Size = size;
				this.m_States = states;
			}
		}

		private ulong m_Size;

		private AndroidAssetPackState[] m_States;

		private readonly object m_OperationLock;
	}
}
