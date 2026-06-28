using System;
using System.Runtime.Serialization;
using KSerialization;

[SerializationConfig(MemberSerialization.OptOut)]
public class Chunk
{
	public Chunk()
	{
		this.state = Chunk.State.Unprocessed;
		this.data = null;
		this.overrides = null;
		this.density = null;
		this.heatOffset = null;
		this.defaultTemp = null;
	}

	public Chunk(int x, int y, int width, int height)
	{
		this.offset = new Vector2I(x, y);
		this.size = new Vector2I(width, height);
	}

	[OnDeserializing]
	internal void OnDeserializingMethod()
	{
		int x = this.size.x;
		int y = this.size.y;
		this.data = new float[x * y];
		this.overrides = new float[x * y];
		this.density = new float[x * y];
		this.heatOffset = new float[x * y];
		this.defaultTemp = new float[x * y];
		this.state = Chunk.State.Loaded;
	}

	public Chunk.State state;

	public Vector2I offset;

	public Vector2I size;

	public float[] data;

	public float[] overrides;

	public float[] density;

	public float[] heatOffset;

	public float[] defaultTemp;

	public enum State
	{
		Unprocessed,
		GeneratedNoise,
		Processed,
		Loaded
	}
}
