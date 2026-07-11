using System;
using System.Collections;

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
				return this.activity;
			}
			set
			{
				this.activity = value;
			}
		}

		public Stack LogicalOperationStack
		{
			get
			{
				return this.op_stack;
			}
		}

		public void StartLogicalOperation()
		{
			this.StartLogicalOperation(Guid.NewGuid());
		}

		public void StartLogicalOperation(object operationId)
		{
			this.op_stack.Push(operationId);
		}

		public void StopLogicalOperation()
		{
			this.op_stack.Pop();
		}

		private Guid activity;

		private Stack op_stack = new Stack();
	}
}
