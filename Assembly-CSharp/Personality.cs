using System;
using System.Collections.Generic;
using Klei.AI;

public class Personality : Resource
{
	public Personality(string name, string Gender, string StressTrait, string CongenitalTrait, int headShape, int mouth, int neck, int eyes, int hair, int body, string description)
		: base(name, name)
	{
		this.gender = Gender;
		this.stresstrait = StressTrait;
		this.congenitaltrait = CongenitalTrait;
		this.unformattedDescription = description;
		this.headShape = headShape;
		this.mouth = mouth;
		this.neck = neck;
		this.eyes = eyes;
		this.hair = hair;
		this.body = body;
	}

	public string description
	{
		get
		{
			return this.GetDescription();
		}
	}

	public string GetDescription()
	{
		if (this.unformattedDescription.Contains("{0}"))
		{
			return string.Format(this.unformattedDescription, this.Name);
		}
		return this.unformattedDescription;
	}

	public void SetAttribute(Klei.AI.Attribute attribute, int value)
	{
		Personality.StartingAttribute startingAttribute = new Personality.StartingAttribute(attribute, value);
		this.attributes.Add(startingAttribute);
	}

	public void AddTrait(Trait trait)
	{
		this.traits.Add(trait);
	}

	public List<Personality.StartingAttribute> attributes = new List<Personality.StartingAttribute>();

	public List<Trait> traits = new List<Trait>();

	public int headShape;

	public int mouth;

	public int neck;

	public int eyes;

	public int hair;

	public int body;

	public string gender;

	public string stresstrait;

	public string congenitaltrait;

	public string unformattedDescription;

	public class StartingAttribute
	{
		public StartingAttribute(Klei.AI.Attribute attribute, int value)
		{
			this.attribute = attribute;
			this.value = value;
		}

		public Klei.AI.Attribute attribute;

		public int value;
	}
}
