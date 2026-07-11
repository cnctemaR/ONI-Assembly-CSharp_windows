using System;

namespace System.Security.Authentication
{
	public enum CipherAlgorithmType
	{
		None,
		Null = 24576,
		Aes = 26129,
		Aes128 = 26126,
		Aes192,
		Aes256,
		Des = 26113,
		Rc2,
		Rc4 = 26625,
		TripleDes = 26115
	}
}
