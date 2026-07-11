using System;

namespace System.Data
{
	internal sealed class OperatorInfo
	{
		internal OperatorInfo(Nodes type, int op, int pri)
		{
			this._type = type;
			this._op = op;
			this._priority = pri;
		}

		internal Nodes _type;

		internal int _op;

		internal int _priority;
	}
}
