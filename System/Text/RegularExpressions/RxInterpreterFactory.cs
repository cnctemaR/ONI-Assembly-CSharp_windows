using System;
using System.Collections;

namespace System.Text.RegularExpressions
{
	internal class RxInterpreterFactory : IMachineFactory
	{
		public RxInterpreterFactory(byte[] program, EvalDelegate eval_del)
		{
			this.program = program;
			this.eval_del = eval_del;
		}

		public IMachine NewInstance()
		{
			return new RxInterpreter(this.program, this.eval_del);
		}

		public int GroupCount
		{
			get
			{
				return (int)this.program[1] | ((int)this.program[2] << 8);
			}
		}

		public int Gap
		{
			get
			{
				return this.gap;
			}
			set
			{
				this.gap = value;
			}
		}

		public IDictionary Mapping
		{
			get
			{
				return this.mapping;
			}
			set
			{
				this.mapping = value;
			}
		}

		public string[] NamesMapping
		{
			get
			{
				return this.namesMapping;
			}
			set
			{
				this.namesMapping = value;
			}
		}

		private IDictionary mapping;

		private byte[] program;

		private EvalDelegate eval_del;

		private string[] namesMapping;

		private int gap;
	}
}
