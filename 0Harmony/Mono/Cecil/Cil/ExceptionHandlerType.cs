using System;

namespace Mono.Cecil.Cil
{
	internal enum ExceptionHandlerType
	{
		Catch,
		Filter,
		Finally,
		Fault = 4
	}
}
