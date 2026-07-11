using System;

namespace System
{
	internal enum ParsingError
	{
		None,
		BadFormat,
		BadScheme,
		BadAuthority,
		EmptyUriString,
		LastRelativeUriOkErrIndex = 4,
		SchemeLimit,
		SizeLimit,
		MustRootedPath,
		BadHostName,
		NonEmptyHost,
		BadPort,
		BadAuthorityTerminator,
		CannotCreateRelative
	}
}
