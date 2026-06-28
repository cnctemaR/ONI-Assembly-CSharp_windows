using System;
using System.Collections.Generic;

public class HashCache
{
	public static HashCache Get()
	{
		if (HashCache.instance == null)
		{
			HashCache.instance = new HashCache();
		}
		return HashCache.instance;
	}

	public string Get(int hash)
	{
		string empty = string.Empty;
		this.hashes.TryGetValue(hash, out empty);
		return empty;
	}

	public string Get(HashedString hash)
	{
		return this.Get(hash.HashValue);
	}

	public string Get(KAnimHashedString hash)
	{
		return this.Get(hash.HashValue);
	}

	public HashedString Add(string text)
	{
		HashedString hashedString = new HashedString(text);
		this.Add(hashedString.HashValue, text);
		return hashedString;
	}

	public void Add(int hash, string text)
	{
		string text2 = null;
		if (!this.hashes.TryGetValue(hash, out text2))
		{
			this.hashes[hash] = text.ToLower();
		}
	}

	private Dictionary<int, string> hashes = new Dictionary<int, string>();

	private static HashCache instance;
}
