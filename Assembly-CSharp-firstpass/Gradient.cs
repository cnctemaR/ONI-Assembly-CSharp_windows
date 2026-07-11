using System;

[Serializable]
public class Gradient<T>
{
	public Gradient(T content, float bandSize)
	{
		this.bandSize = bandSize;
		this.content = content;
	}

	public T content { get; protected set; }

	public float bandSize { get; protected set; }

	public float maxValue { get; set; }
}
