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

	public string Get(KAnimHashedString hash)
	{
		return this.Get(hash.HashValue);
	}

	public void Add(int hash, string text)
	{
		this.hashes[hash] = text;
	}

	private Dictionary<int, string> hashes = new Dictionary<int, string>();

	private static HashCache instance;
}
