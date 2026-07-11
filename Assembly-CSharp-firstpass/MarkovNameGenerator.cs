using System;
using System.Collections.Generic;

public class MarkovNameGenerator
{
	public MarkovNameGenerator(IEnumerable<string> sampleNames, int order, int minLength)
	{
		if (order < 1)
		{
			order = 1;
		}
		if (minLength < 1)
		{
			minLength = 1;
		}
		this._order = order;
		this._minLength = minLength;
		foreach (string text in sampleNames)
		{
			string[] array = text.Split(new char[] { ',' });
			for (int i = 0; i < array.Length; i++)
			{
				string text2 = array[i].Trim().ToUpper();
				if (text2.Length >= order + 1)
				{
					this._samples.Add(text2);
				}
			}
		}
		foreach (string text3 in this._samples)
		{
			for (int j = 0; j < text3.Length - order; j++)
			{
				string text4 = text3.Substring(j, order);
				List<char> list;
				if (this._chains.ContainsKey(text4))
				{
					list = this._chains[text4];
				}
				else
				{
					list = new List<char>();
					this._chains[text4] = list;
				}
				list.Add(text3[j + order]);
			}
		}
	}

	public string NextName
	{
		get
		{
			string text;
			do
			{
				int num = this._rnd.Next(this._samples.Count);
				int length = this._samples[num].Length;
				text = this._samples[num].Substring(this._rnd.Next(0, this._samples[num].Length - this._order), this._order);
				while (text.Length < length)
				{
					string text2 = text.Substring(text.Length - this._order, this._order);
					if (this.GetLetter(text2) == '?')
					{
						break;
					}
					text += this.GetLetter(text2).ToString();
				}
				if (text.Contains(" "))
				{
					string[] array = text.Split(new char[] { ' ' });
					text = "";
					for (int i = 0; i < array.Length; i++)
					{
						if (!(array[i] == ""))
						{
							if (array[i].Length == 1)
							{
								array[i] = array[i].ToUpper();
							}
							else
							{
								array[i] = array[i].Substring(0, 1) + array[i].Substring(1).ToLower();
							}
							if (text != "")
							{
								text += " ";
							}
							text += array[i];
						}
					}
				}
				else
				{
					text = text.Substring(0, 1) + text.Substring(1).ToLower();
				}
			}
			while (this._used.Contains(text) || text.Length < this._minLength);
			this._used.Add(text);
			return text;
		}
	}

	public void Reset()
	{
		this._used.Clear();
	}

	private char GetLetter(string token)
	{
		if (!this._chains.ContainsKey(token))
		{
			return '?';
		}
		List<char> list = this._chains[token];
		int num = this._rnd.Next(list.Count);
		return list[num];
	}

	private Dictionary<string, List<char>> _chains = new Dictionary<string, List<char>>();

	private List<string> _samples = new List<string>();

	private List<string> _used = new List<string>();

	private Random _rnd = new Random();

	private int _order;

	private int _minLength;
}
