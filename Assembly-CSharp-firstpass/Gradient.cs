using System;

[Serializable]
public class Gradient<T>
{
	public Gradient(T content, float bandSize)
	{
		this.bandSize = bandSize;
		this.content = content;
	}

	public T content { get; set; }

	public float bandSize { get; set; }

	public float maxValue { get; set; }
}
