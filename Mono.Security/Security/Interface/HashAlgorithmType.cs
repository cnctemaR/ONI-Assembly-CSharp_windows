using System;

namespace Mono.Security.Interface
{
	public enum HashAlgorithmType
	{
		None,
		Md5,
		Sha1,
		Sha224,
		Sha256,
		Sha384,
		Sha512,
		Unknown = 255,
		Md5Sha1 = 254
	}
}
