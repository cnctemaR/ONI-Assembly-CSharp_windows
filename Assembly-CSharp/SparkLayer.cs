using System;
using UnityEngine;
using UnityEngine.UI;

public class SparkLayer : LineLayer
{
	public void SetColor(ColonyDiagnostic.DiagnosticResult result)
	{
		switch (result.opinion)
		{
		case ColonyDiagnostic.DiagnosticResult.Opinion.DuplicantThreatening:
		case ColonyDiagnostic.DiagnosticResult.Opinion.Bad:
			this.SetColor(Constants.NEGATIVE_COLOR);
			return;
		case ColonyDiagnostic.DiagnosticResult.Opinion.Warning:
		case ColonyDiagnostic.DiagnosticResult.Opinion.Concern:
			this.SetColor(Constants.WARNING_COLOR);
			return;
		case ColonyDiagnostic.DiagnosticResult.Opinion.Suggestion:
		case ColonyDiagnostic.DiagnosticResult.Opinion.Tutorial:
		case ColonyDiagnostic.DiagnosticResult.Opinion.Normal:
			this.SetColor(Constants.NEUTRAL_COLOR);
			return;
		case ColonyDiagnostic.DiagnosticResult.Opinion.Good:
			this.SetColor(Constants.POSITIVE_COLOR);
			return;
		default:
			this.SetColor(Constants.NEUTRAL_COLOR);
			return;
		}
	}

	public void SetColor(Color color)
	{
		this.line_formatting[0].color = color;
	}

	public override GraphedLine NewLine(global::Tuple<float, float>[] points, string ID = "")
	{
		Color positive_COLOR = Constants.POSITIVE_COLOR;
		Color neutral_COLOR = Constants.NEUTRAL_COLOR;
		Color negative_COLOR = Constants.NEGATIVE_COLOR;
		if (this.colorRules.setOwnColor)
		{
			if (points.Length > 2)
			{
				if (this.colorRules.zeroIsBad && points[points.Length - 1].second == 0f)
				{
					this.line_formatting[0].color = negative_COLOR;
				}
				else if (points[points.Length - 1].second > points[points.Length - 2].second)
				{
					this.line_formatting[0].color = (this.colorRules.positiveIsGood ? positive_COLOR : negative_COLOR);
				}
				else if (points[points.Length - 1].second < points[points.Length - 2].second)
				{
					this.line_formatting[0].color = (this.colorRules.positiveIsGood ? negative_COLOR : positive_COLOR);
				}
				else
				{
					this.line_formatting[0].color = neutral_COLOR;
				}
			}
			else
			{
				this.line_formatting[0].color = neutral_COLOR;
			}
		}
		this.ScaleToData(points);
		if (this.subZeroAreaFill != null)
		{
			this.subZeroAreaFill.color = new Color(this.line_formatting[0].color.r, this.line_formatting[0].color.g, this.line_formatting[0].color.b, this.fillAlphaMin);
		}
		return base.NewLine(points, ID);
	}

	public override void RefreshLine(global::Tuple<float, float>[] points, string ID)
	{
		this.SetColor(points);
		this.ScaleToData(points);
		base.RefreshLine(points, ID);
	}

	private void SetColor(global::Tuple<float, float>[] points)
	{
		Color positive_COLOR = Constants.POSITIVE_COLOR;
		Color neutral_COLOR = Constants.NEUTRAL_COLOR;
		Color negative_COLOR = Constants.NEGATIVE_COLOR;
		if (this.colorRules.setOwnColor)
		{
			if (points.Length > 2)
			{
				if (this.colorRules.zeroIsBad && points[points.Length - 1].second == 0f)
				{
					this.line_formatting[0].color = negative_COLOR;
				}
				else if (points[points.Length - 1].second > points[points.Length - 2].second)
				{
					this.line_formatting[0].color = (this.colorRules.positiveIsGood ? positive_COLOR : negative_COLOR);
				}
				else if (points[points.Length - 1].second < points[points.Length - 2].second)
				{
					this.line_formatting[0].color = (this.colorRules.positiveIsGood ? negative_COLOR : positive_COLOR);
				}
				else
				{
					this.line_formatting[0].color = neutral_COLOR;
				}
			}
			else
			{
				this.line_formatting[0].color = neutral_COLOR;
			}
		}
		if (this.subZeroAreaFill != null)
		{
			this.subZeroAreaFill.color = new Color(this.line_formatting[0].color.r, this.line_formatting[0].color.g, this.line_formatting[0].color.b, this.fillAlphaMin);
		}
	}

	private void ScaleToData(global::Tuple<float, float>[] points)
	{
		if (this.scaleWidthToData || this.scaleHeightToData)
		{
			Vector2 vector = base.CalculateMin(points);
			Vector2 vector2 = base.CalculateMax(points);
			if (this.scaleHeightToData)
			{
				base.graph.ClearHorizontalGuides();
				base.graph.axis_y.max_value = vector2.y;
				base.graph.axis_y.min_value = vector.y;
				base.graph.RefreshHorizontalGuides();
			}
			if (this.scaleWidthToData)
			{
				base.graph.ClearVerticalGuides();
				base.graph.axis_x.max_value = vector2.x;
				base.graph.axis_x.min_value = vector.x;
				base.graph.RefreshVerticalGuides();
			}
		}
	}

	public Image subZeroAreaFill;

	public SparkLayer.ColorRules colorRules;

	public bool debugMark;

	public bool scaleHeightToData = true;

	public bool scaleWidthToData = true;

	[Serializable]
	public struct ColorRules
	{
		public bool setOwnColor;

		public bool positiveIsGood;

		public bool zeroIsBad;
	}
}
