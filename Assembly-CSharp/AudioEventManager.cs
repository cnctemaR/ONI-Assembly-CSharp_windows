using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioEventManager : KMonoBehaviour
{
	public static AudioEventManager Get()
	{
		if (AudioEventManager.instance == null)
		{
			if (App.IsExiting)
			{
				return null;
			}
			GameObject gameObject = GameObject.Find("/AudioEventManager");
			if (gameObject == null)
			{
				gameObject = new GameObject();
				gameObject.name = "AudioEventManager";
			}
			AudioEventManager.instance = gameObject.GetComponent<AudioEventManager>();
			if (AudioEventManager.instance == null)
			{
				AudioEventManager.instance = gameObject.AddComponent<AudioEventManager>();
			}
		}
		return AudioEventManager.instance;
	}

	public float GetNoisePollutionAtCell(int cell)
	{
		this.polluterCount = 0;
		float num = 0f;
		int num2 = 0;
		if (NoisePolluter.IsNoiseableCell(cell) && Grid.NoisePollution[cell] != -1)
		{
			if (this.noiseCells.ContainsKey(cell))
			{
				for (int i = 0; i < this.noiseCells[cell].cellContents.Count; i++)
				{
					NoiseSplat noiseSplat = default(NoiseSplat);
					if (this.splats.TryGetValue(this.noiseCells[cell].cellContents[i].first, out noiseSplat))
					{
						int num3 = this.noiseCells[cell].cellContents[i].second;
						if (noiseSplat.deathTime > 0f)
						{
							float num4 = Mathf.Max(noiseSplat.deathTime - Time.time, 0f);
							float num5 = 0.25f;
							if (num4 < num5)
							{
								num3 = (int)((float)num3 * (num4 / num5));
							}
						}
						AudioEventManager.polluterArray[this.polluterCount++] = num3;
						if (this.polluterCount == AudioEventManager.polluterArray.Length)
						{
							break;
						}
					}
				}
			}
			Array.Sort<int>(AudioEventManager.polluterArray, 0, this.polluterCount);
			for (int j = this.polluterCount - 1; j >= 0; j--)
			{
				if (num == 0f)
				{
					num2 = AudioEventManager.polluterArray[j];
					num = (float)AudioEventManager.polluterArray[j];
				}
				else
				{
					int num6 = num2 - AudioEventManager.polluterArray[j];
					if (num6 < AudioEventManager.dbToAdd.Length)
					{
						num += AudioEventManager.dbToAdd[num6];
					}
					num2 = Mathf.CeilToInt(num);
				}
			}
		}
		return (float)Mathf.CeilToInt(num);
	}

	public static string GetLoudestNoisePollutorAtCell(int cell)
	{
		string text = null;
		List<AudioEventManager.PolluterDisplay> pollutorsForCell = AudioEventManager.Get().GetPollutorsForCell(cell);
		if (pollutorsForCell.Count > 0)
		{
			pollutorsForCell.Sort((AudioEventManager.PolluterDisplay x, AudioEventManager.PolluterDisplay y) => y.value.CompareTo(x.value));
			text = pollutorsForCell[0].name;
		}
		return text;
	}

	public void ClearNoiseSplat(long id)
	{
		if (this.splats.ContainsKey(id))
		{
			this.splats[id].Clear();
		}
	}

	public long UpdateConstantNoiseSplat(NoisePolluter np)
	{
		long num = np.noiseSplatID;
		if (num == -1L)
		{
			long num2;
			this.maxID = (num2 = this.maxID) + 1L;
			num = num2;
		}
		this.splats[num] = new NoiseSplat(np, num, 0f);
		return num;
	}

	public long UpdateNoiseSplat(Vector2 pos, int dB, int radius, string name, long id = -1L)
	{
		if (id == -1L)
		{
			long num;
			this.maxID = (num = this.maxID) + 1L;
			id = num;
		}
		Polluter polluter = this.GetPolluter(radius);
		polluter.SetAttributes(pos, dB, name);
		this.splats[id] = new NoiseSplat(polluter, id, 0f);
		return id;
	}

	public void AddPollution(long id, int cell, int value)
	{
		if (!this.noiseCells.ContainsKey(cell))
		{
			this.noiseCells.Add(cell, new AudioEventManager.EffectedCell());
		}
		if (!this.noiseCells[cell].dirty)
		{
			this.dirtyNoiseCells.Add(cell);
		}
		this.noiseCells[cell].AddEffector(id, value);
	}

	public void RemovePollution(long id, int cell)
	{
		if (!this.noiseCells[cell].dirty)
		{
			this.dirtyNoiseCells.Add(cell);
		}
		this.noiseCells[cell].RemoveEffector(id);
	}

	public List<AudioEventManager.PolluterDisplay> GetPollutorsForCell(int cell)
	{
		List<AudioEventManager.PolluterDisplay> list = new List<AudioEventManager.PolluterDisplay>();
		if (this.noiseCells.ContainsKey(cell))
		{
			for (int i = 0; i < this.noiseCells[cell].cellContents.Count; i++)
			{
				list.Add(new AudioEventManager.PolluterDisplay
				{
					name = this.splats[this.noiseCells[cell].cellContents[i].first].GetName(),
					value = this.noiseCells[cell].cellContents[i].second,
					affecting = (i <= this.noiseCells[cell].affectingIndex)
				});
			}
		}
		return list;
	}

	private void RemoveExpiredSplats()
	{
		if (this.removeTime.Count > 1)
		{
			this.removeTime.Sort((Pair<float, long> a, Pair<float, long> b) => a.first.CompareTo(b.first));
		}
		int num = -1;
		for (int i = 0; i < this.removeTime.Count; i++)
		{
			if (this.removeTime[i].first > Time.time)
			{
				break;
			}
			if (this.splats.ContainsKey(this.removeTime[i].second))
			{
				IPolluter provider = this.splats[this.removeTime[i].second].GetProvider();
				this.splats[this.removeTime[i].second].Clear();
				this.splats.Remove(this.removeTime[i].second);
				this.FreePolluter(provider as Polluter);
			}
			num = i;
		}
		for (int j = num; j >= 0; j--)
		{
			this.removeTime.RemoveAt(j);
		}
	}

	private void UpdateDirtyCells()
	{
		int num = -1;
		int num2 = 0;
		while (num2 < this.dirtyNoiseCells.Count && num2 < 500)
		{
			this.noiseCells[this.dirtyNoiseCells[num2]].Update();
			Grid.NoisePollution[this.dirtyNoiseCells[num2]] = this.noiseCells[this.dirtyNoiseCells[num2]].dB;
			num = num2;
			num2++;
		}
		for (int i = num; i >= 0; i--)
		{
			this.dirtyNoiseCells.RemoveAt(i);
		}
	}

	private void Update()
	{
		this.RemoveExpiredSplats();
		this.UpdateDirtyCells();
	}

	private Polluter GetPolluter(int radius)
	{
		if (!this.freePool.ContainsKey(radius))
		{
			this.freePool.Add(radius, new List<Polluter>());
		}
		Polluter polluter;
		if (this.freePool[radius].Count > 0)
		{
			polluter = this.freePool[radius][0];
			this.freePool[radius].RemoveAt(0);
		}
		else
		{
			polluter = new Polluter(radius);
		}
		if (!this.inusePool.ContainsKey(radius))
		{
			this.inusePool.Add(radius, new List<Polluter>());
		}
		this.inusePool[radius].Add(polluter);
		return polluter;
	}

	private void FreePolluter(Polluter pol)
	{
		if (pol != null)
		{
			this.inusePool[pol.radius].Remove(pol);
			this.freePool[pol.radius].Add(pol);
		}
	}

	public void PlayTimedOnceOff(Vector2 pos, int dB, int radius, string name, float time = 1f)
	{
		if (dB > 0 && radius > 0 && time > 0f)
		{
			Polluter polluter = this.GetPolluter(radius);
			polluter.SetAttributes(pos, dB, name);
			this.AddTimedInstance(polluter, time);
		}
	}

	private void AddTimedInstance(Polluter p, float time)
	{
		long num;
		this.maxID = (num = this.maxID) + 1L;
		long num2 = num;
		this.splats.Add(num2, new NoiseSplat(p, num2, time + Time.time));
		this.removeTime.Add(new Pair<float, long>(time + Time.time, num2));
	}

	private static void SoundLog(long itemId, string message)
	{
		global::Debug.Log(string.Concat(new object[] { " [", itemId, "] \t", message }), null);
	}

	public const int NO_NOISE_EFFECTORS = -1;

	private static AudioEventManager instance;

	private static float[] dbToAdd = new float[]
	{
		3f, 2.5f, 2f, 2f, 1.5f, 1f, 1f, 1f, 0.5f, 0.5f,
		0.5f
	};

	private List<int> dirtyNoiseCells = new List<int>();

	private Dictionary<int, AudioEventManager.EffectedCell> noiseCells = new Dictionary<int, AudioEventManager.EffectedCell>();

	private List<Pair<float, long>> removeTime = new List<Pair<float, long>>();

	private Dictionary<int, List<Polluter>> freePool = new Dictionary<int, List<Polluter>>();

	private Dictionary<int, List<Polluter>> inusePool = new Dictionary<int, List<Polluter>>();

	private Dictionary<long, NoiseSplat> splats = new Dictionary<long, NoiseSplat>();

	private long maxID;

	private static int[] polluterArray = new int[50];

	private int polluterCount;

	public enum NoiseEffect
	{
		Peaceful,
		Quiet = 36,
		TossAndTurn = 45,
		WakeUp = 60,
		Passive = 80,
		Active = 106,
		Extreme = 125
	}

	public struct PolluterDisplay
	{
		public string name;

		public int value;

		public bool affecting;
	}

	private class EffectedCell
	{
		public EffectedCell()
		{
			this.cellContents = new List<Pair<long, int>>();
		}

		public int dB { get; private set; }

		public bool dirty { get; private set; }

		public List<Pair<long, int>> cellContents { get; private set; }

		public void AddEffector(long id, int value)
		{
			this.cellContents.Add(new Pair<long, int>(id, value));
			this.dirty = true;
		}

		public void RemoveEffector(long id)
		{
			this.cellContents.RemoveAll((Pair<long, int> p) => p.first == id);
			this.dirty = true;
		}

		public void Update()
		{
			this.dirty = false;
			if (this.cellContents.Count == 0)
			{
				this.dB = 0;
				return;
			}
			if (this.cellContents.Count == 1)
			{
				this.dB = this.cellContents[0].second;
				return;
			}
			this.cellContents.Sort((Pair<long, int> a, Pair<long, int> b) => b.second.CompareTo(a.second));
			this.dB = this.cellContents[0].second;
			this.affectingIndex = 0;
			float num = (float)this.dB;
			int num2 = 1;
			while (num2 < this.cellContents.Count && this.dB - this.cellContents[num2].second <= 10)
			{
				int num3 = this.dB - this.cellContents[num2].second;
				num += AudioEventManager.dbToAdd[num3];
				this.affectingIndex = num2;
				this.dB = Mathf.CeilToInt(num);
				num2++;
			}
			this.dB = Mathf.CeilToInt(num);
		}

		public int affectingIndex = -1;
	}
}
