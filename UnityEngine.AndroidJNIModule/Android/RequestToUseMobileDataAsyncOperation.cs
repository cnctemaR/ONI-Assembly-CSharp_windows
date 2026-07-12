using System;

namespace UnityEngine.Android
{
	public class RequestToUseMobileDataAsyncOperation : CustomYieldInstruction
	{
		public override bool keepWaiting
		{
			get
			{
				object operationLock = this.m_OperationLock;
				bool flag;
				lock (operationLock)
				{
					flag = this.m_RequestResult == null;
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

		public AndroidAssetPackUseMobileDataRequestResult result
		{
			get
			{
				object operationLock = this.m_OperationLock;
				AndroidAssetPackUseMobileDataRequestResult requestResult;
				lock (operationLock)
				{
					requestResult = this.m_RequestResult;
				}
				return requestResult;
			}
		}

		internal RequestToUseMobileDataAsyncOperation()
		{
			this.m_OperationLock = new object();
		}

		internal void OnResult(AndroidAssetPackUseMobileDataRequestResult result)
		{
			object operationLock = this.m_OperationLock;
			lock (operationLock)
			{
				this.m_RequestResult = result;
			}
		}

		private AndroidAssetPackUseMobileDataRequestResult m_RequestResult;

		private readonly object m_OperationLock;
	}
}
