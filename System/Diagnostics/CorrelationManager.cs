using System;
using System.Collections;
using System.Runtime.Remoting.Messaging;

namespace System.Diagnostics
{
	public class CorrelationManager
	{
		internal CorrelationManager()
		{
		}

		public Guid ActivityId
		{
			get
			{
				object obj = CallContext.LogicalGetData("E2ETrace.ActivityID");
				if (obj != null)
				{
					return (Guid)obj;
				}
				return Guid.Empty;
			}
			set
			{
				CallContext.LogicalSetData("E2ETrace.ActivityID", value);
			}
		}

		public Stack LogicalOperationStack
		{
			get
			{
				return this.GetLogicalOperationStack();
			}
		}

		public void StartLogicalOperation(object operationId)
		{
			if (operationId == null)
			{
				throw new ArgumentNullException("operationId");
			}
			this.GetLogicalOperationStack().Push(operationId);
		}

		public void StartLogicalOperation()
		{
			this.StartLogicalOperation(Guid.NewGuid());
		}

		public void StopLogicalOperation()
		{
			this.GetLogicalOperationStack().Pop();
		}

		private Stack GetLogicalOperationStack()
		{
			Stack stack = CallContext.LogicalGetData("System.Diagnostics.Trace.CorrelationManagerSlot") as Stack;
			if (stack == null)
			{
				stack = new Stack();
				CallContext.LogicalSetData("System.Diagnostics.Trace.CorrelationManagerSlot", stack);
			}
			return stack;
		}

		private const string transactionSlotName = "System.Diagnostics.Trace.CorrelationManagerSlot";

		private const string activityIdSlotName = "E2ETrace.ActivityID";
	}
}
