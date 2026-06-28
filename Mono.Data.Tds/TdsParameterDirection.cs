using System;

namespace Mono.Data.Tds
{
	[Serializable]
	public enum TdsParameterDirection
	{
		Input,
		Output,
		InputOutput,
		ReturnValue
	}
}
