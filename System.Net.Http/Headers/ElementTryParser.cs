using System;

namespace System.Net.Http.Headers
{
	internal delegate bool ElementTryParser<T>(Lexer lexer, out T parsedValue, out Token token);
}
