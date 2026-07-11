using System;

namespace Mono.Net.Dns
{
	internal enum ResolverError
	{
		NoError,
		FormatError,
		ServerFailure,
		NameError,
		NotImplemented,
		Refused,
		ResponseHeaderError,
		ResponseFormatError,
		Timeout
	}
}
