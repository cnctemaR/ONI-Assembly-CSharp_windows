using System;
using System.Globalization;

namespace System.Numerics
{
	[Serializable]
	public struct Complex : IEquatable<Complex>, IFormattable
	{
		public Complex(double real, double imaginary)
		{
			this.m_real = real;
			this.m_imaginary = imaginary;
		}

		public double Real
		{
			get
			{
				return this.m_real;
			}
		}

		public double Imaginary
		{
			get
			{
				return this.m_imaginary;
			}
		}

		public double Magnitude
		{
			get
			{
				return Complex.Abs(this);
			}
		}

		public double Phase
		{
			get
			{
				return Math.Atan2(this.m_imaginary, this.m_real);
			}
		}

		public static Complex FromPolarCoordinates(double magnitude, double phase)
		{
			return new Complex(magnitude * Math.Cos(phase), magnitude * Math.Sin(phase));
		}

		public static Complex Negate(Complex value)
		{
			return -value;
		}

		public static Complex Add(Complex left, Complex right)
		{
			return left + right;
		}

		public static Complex Subtract(Complex left, Complex right)
		{
			return left - right;
		}

		public static Complex Multiply(Complex left, Complex right)
		{
			return left * right;
		}

		public static Complex Divide(Complex dividend, Complex divisor)
		{
			return dividend / divisor;
		}

		public static Complex operator -(Complex value)
		{
			return new Complex(-value.m_real, -value.m_imaginary);
		}

		public static Complex operator +(Complex left, Complex right)
		{
			return new Complex(left.m_real + right.m_real, left.m_imaginary + right.m_imaginary);
		}

		public static Complex operator -(Complex left, Complex right)
		{
			return new Complex(left.m_real - right.m_real, left.m_imaginary - right.m_imaginary);
		}

		public static Complex operator *(Complex left, Complex right)
		{
			double num = left.m_real * right.m_real - left.m_imaginary * right.m_imaginary;
			double num2 = left.m_imaginary * right.m_real + left.m_real * right.m_imaginary;
			return new Complex(num, num2);
		}

		public static Complex operator /(Complex left, Complex right)
		{
			double real = left.m_real;
			double imaginary = left.m_imaginary;
			double real2 = right.m_real;
			double imaginary2 = right.m_imaginary;
			if (Math.Abs(imaginary2) < Math.Abs(real2))
			{
				double num = imaginary2 / real2;
				return new Complex((real + imaginary * num) / (real2 + imaginary2 * num), (imaginary - real * num) / (real2 + imaginary2 * num));
			}
			double num2 = real2 / imaginary2;
			return new Complex((imaginary + real * num2) / (imaginary2 + real2 * num2), (-real + imaginary * num2) / (imaginary2 + real2 * num2));
		}

		public static double Abs(Complex value)
		{
			return Complex.Hypot(value.m_real, value.m_imaginary);
		}

		private static double Hypot(double a, double b)
		{
			a = Math.Abs(a);
			b = Math.Abs(b);
			double num;
			double num2;
			if (a < b)
			{
				num = a;
				num2 = b;
			}
			else
			{
				num = b;
				num2 = a;
			}
			if (num == 0.0)
			{
				return num2;
			}
			if (double.IsPositiveInfinity(num2) && !double.IsNaN(num))
			{
				return double.PositiveInfinity;
			}
			double num3 = num / num2;
			return num2 * Math.Sqrt(1.0 + num3 * num3);
		}

		private static double Log1P(double x)
		{
			double num = 1.0 + x;
			if (num == 1.0)
			{
				return x;
			}
			if (x < 0.75)
			{
				return x * Math.Log(num) / (num - 1.0);
			}
			return Math.Log(num);
		}

		public static Complex Conjugate(Complex value)
		{
			return new Complex(value.m_real, -value.m_imaginary);
		}

		public static Complex Reciprocal(Complex value)
		{
			if (value.m_real == 0.0 && value.m_imaginary == 0.0)
			{
				return Complex.Zero;
			}
			return Complex.One / value;
		}

		public static bool operator ==(Complex left, Complex right)
		{
			return left.m_real == right.m_real && left.m_imaginary == right.m_imaginary;
		}

		public static bool operator !=(Complex left, Complex right)
		{
			return left.m_real != right.m_real || left.m_imaginary != right.m_imaginary;
		}

		public override bool Equals(object obj)
		{
			return obj is Complex && this.Equals((Complex)obj);
		}

		public bool Equals(Complex value)
		{
			return this.m_real.Equals(value.m_real) && this.m_imaginary.Equals(value.m_imaginary);
		}

		public override int GetHashCode()
		{
			int num = 99999997;
			int num2 = this.m_real.GetHashCode() % num;
			int hashCode = this.m_imaginary.GetHashCode();
			return num2 ^ hashCode;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, "({0}, {1})", this.m_real, this.m_imaginary);
		}

		public string ToString(string format)
		{
			return string.Format(CultureInfo.CurrentCulture, "({0}, {1})", this.m_real.ToString(format, CultureInfo.CurrentCulture), this.m_imaginary.ToString(format, CultureInfo.CurrentCulture));
		}

		public string ToString(IFormatProvider provider)
		{
			return string.Format(provider, "({0}, {1})", this.m_real, this.m_imaginary);
		}

		public string ToString(string format, IFormatProvider provider)
		{
			return string.Format(provider, "({0}, {1})", this.m_real.ToString(format, provider), this.m_imaginary.ToString(format, provider));
		}

		public static Complex Sin(Complex value)
		{
			double num = Math.Exp(value.m_imaginary);
			double num2 = 1.0 / num;
			double num3 = (num - num2) * 0.5;
			double num4 = (num + num2) * 0.5;
			return new Complex(Math.Sin(value.m_real) * num4, Math.Cos(value.m_real) * num3);
		}

		public static Complex Sinh(Complex value)
		{
			Complex complex = Complex.Sin(new Complex(-value.m_imaginary, value.m_real));
			return new Complex(complex.m_imaginary, -complex.m_real);
		}

		public static Complex Asin(Complex value)
		{
			double num;
			double num2;
			double num3;
			Complex.Asin_Internal(Math.Abs(value.Real), Math.Abs(value.Imaginary), out num, out num2, out num3);
			double num4;
			if (num2 < 0.0)
			{
				num4 = Math.Asin(num);
			}
			else
			{
				num4 = Math.Atan(num2);
			}
			if (value.Real < 0.0)
			{
				num4 = -num4;
			}
			if (value.Imaginary < 0.0)
			{
				num3 = -num3;
			}
			return new Complex(num4, num3);
		}

		public static Complex Cos(Complex value)
		{
			double num = Math.Exp(value.m_imaginary);
			double num2 = 1.0 / num;
			double num3 = (num - num2) * 0.5;
			double num4 = (num + num2) * 0.5;
			return new Complex(Math.Cos(value.m_real) * num4, -Math.Sin(value.m_real) * num3);
		}

		public static Complex Cosh(Complex value)
		{
			return Complex.Cos(new Complex(-value.m_imaginary, value.m_real));
		}

		public static Complex Acos(Complex value)
		{
			double num;
			double num2;
			double num3;
			Complex.Asin_Internal(Math.Abs(value.Real), Math.Abs(value.Imaginary), out num, out num2, out num3);
			double num4;
			if (num2 < 0.0)
			{
				num4 = Math.Acos(num);
			}
			else
			{
				num4 = Math.Atan(1.0 / num2);
			}
			if (value.Real < 0.0)
			{
				num4 = 3.141592653589793 - num4;
			}
			if (value.Imaginary > 0.0)
			{
				num3 = -num3;
			}
			return new Complex(num4, num3);
		}

		public static Complex Tan(Complex value)
		{
			double num = 2.0 * value.m_real;
			double num2 = 2.0 * value.m_imaginary;
			double num3 = Math.Exp(num2);
			double num4 = 1.0 / num3;
			double num5 = (num3 + num4) * 0.5;
			if (Math.Abs(value.m_imaginary) <= 4.0)
			{
				double num6 = (num3 - num4) * 0.5;
				double num7 = Math.Cos(num) + num5;
				return new Complex(Math.Sin(num) / num7, num6 / num7);
			}
			double num8 = 1.0 + Math.Cos(num) / num5;
			return new Complex(Math.Sin(num) / num5 / num8, Math.Tanh(num2) / num8);
		}

		public static Complex Tanh(Complex value)
		{
			Complex complex = Complex.Tan(new Complex(-value.m_imaginary, value.m_real));
			return new Complex(complex.m_imaginary, -complex.m_real);
		}

		public static Complex Atan(Complex value)
		{
			Complex complex = new Complex(2.0, 0.0);
			return Complex.ImaginaryOne / complex * (Complex.Log(Complex.One - Complex.ImaginaryOne * value) - Complex.Log(Complex.One + Complex.ImaginaryOne * value));
		}

		private static void Asin_Internal(double x, double y, out double b, out double bPrime, out double v)
		{
			if (x > Complex.s_asinOverflowThreshold || y > Complex.s_asinOverflowThreshold)
			{
				b = -1.0;
				bPrime = x / y;
				double num;
				double num2;
				if (x < y)
				{
					num = x;
					num2 = y;
				}
				else
				{
					num = y;
					num2 = x;
				}
				double num3 = num / num2;
				v = Complex.s_log2 + Math.Log(num2) + 0.5 * Complex.Log1P(num3 * num3);
				return;
			}
			double num4 = Complex.Hypot(x + 1.0, y);
			double num5 = Complex.Hypot(x - 1.0, y);
			double num6 = (num4 + num5) * 0.5;
			b = x / num6;
			if (b > 0.75)
			{
				if (x <= 1.0)
				{
					double num7 = (y * y / (num4 + (x + 1.0)) + (num5 + (1.0 - x))) * 0.5;
					bPrime = x / Math.Sqrt((num6 + x) * num7);
				}
				else
				{
					double num8 = (1.0 / (num4 + (x + 1.0)) + 1.0 / (num5 + (x - 1.0))) * 0.5;
					bPrime = x / y / Math.Sqrt((num6 + x) * num8);
				}
			}
			else
			{
				bPrime = -1.0;
			}
			if (num6 >= 1.5)
			{
				v = Math.Log(num6 + Math.Sqrt((num6 - 1.0) * (num6 + 1.0)));
				return;
			}
			if (x < 1.0)
			{
				double num9 = (1.0 / (num4 + (x + 1.0)) + 1.0 / (num5 + (1.0 - x))) * 0.5;
				double num10 = y * y * num9;
				v = Complex.Log1P(num10 + y * Math.Sqrt(num9 * (num6 + 1.0)));
				return;
			}
			double num11 = (y * y / (num4 + (x + 1.0)) + (num5 + (x - 1.0))) * 0.5;
			v = Complex.Log1P(num11 + Math.Sqrt(num11 * (num6 + 1.0)));
		}

		public static Complex Log(Complex value)
		{
			return new Complex(Math.Log(Complex.Abs(value)), Math.Atan2(value.m_imaginary, value.m_real));
		}

		public static Complex Log(Complex value, double baseValue)
		{
			return Complex.Log(value) / Complex.Log(baseValue);
		}

		public static Complex Log10(Complex value)
		{
			return Complex.Scale(Complex.Log(value), 0.43429448190325);
		}

		public static Complex Exp(Complex value)
		{
			double num = Math.Exp(value.m_real);
			double num2 = num * Math.Cos(value.m_imaginary);
			double num3 = num * Math.Sin(value.m_imaginary);
			return new Complex(num2, num3);
		}

		public static Complex Sqrt(Complex value)
		{
			if (value.m_imaginary != 0.0)
			{
				bool flag = false;
				if (Math.Abs(value.m_real) >= Complex.s_sqrtRescaleThreshold || Math.Abs(value.m_imaginary) >= Complex.s_sqrtRescaleThreshold)
				{
					if (double.IsInfinity(value.m_imaginary) && !double.IsNaN(value.m_real))
					{
						return new Complex(double.PositiveInfinity, value.m_imaginary);
					}
					value.m_real *= 0.25;
					value.m_imaginary *= 0.25;
					flag = true;
				}
				double num;
				double num2;
				if (value.m_real >= 0.0)
				{
					num = Math.Sqrt((Complex.Hypot(value.m_real, value.m_imaginary) + value.m_real) * 0.5);
					num2 = value.m_imaginary / (2.0 * num);
				}
				else
				{
					num2 = Math.Sqrt((Complex.Hypot(value.m_real, value.m_imaginary) - value.m_real) * 0.5);
					if (value.m_imaginary < 0.0)
					{
						num2 = -num2;
					}
					num = value.m_imaginary / (2.0 * num2);
				}
				if (flag)
				{
					num *= 2.0;
					num2 *= 2.0;
				}
				return new Complex(num, num2);
			}
			if (value.m_real < 0.0)
			{
				return new Complex(0.0, Math.Sqrt(-value.m_real));
			}
			return new Complex(Math.Sqrt(value.m_real), 0.0);
		}

		public static Complex Pow(Complex value, Complex power)
		{
			if (power == Complex.Zero)
			{
				return Complex.One;
			}
			if (value == Complex.Zero)
			{
				return Complex.Zero;
			}
			double real = value.m_real;
			double imaginary = value.m_imaginary;
			double real2 = power.m_real;
			double imaginary2 = power.m_imaginary;
			double num = Complex.Abs(value);
			double num2 = Math.Atan2(imaginary, real);
			double num3 = real2 * num2 + imaginary2 * Math.Log(num);
			double num4 = Math.Pow(num, real2) * Math.Pow(2.718281828459045, -imaginary2 * num2);
			return new Complex(num4 * Math.Cos(num3), num4 * Math.Sin(num3));
		}

		public static Complex Pow(Complex value, double power)
		{
			return Complex.Pow(value, new Complex(power, 0.0));
		}

		private static Complex Scale(Complex value, double factor)
		{
			double num = factor * value.m_real;
			double num2 = factor * value.m_imaginary;
			return new Complex(num, num2);
		}

		public static implicit operator Complex(short value)
		{
			return new Complex((double)value, 0.0);
		}

		public static implicit operator Complex(int value)
		{
			return new Complex((double)value, 0.0);
		}

		public static implicit operator Complex(long value)
		{
			return new Complex((double)value, 0.0);
		}

		[CLSCompliant(false)]
		public static implicit operator Complex(ushort value)
		{
			return new Complex((double)value, 0.0);
		}

		[CLSCompliant(false)]
		public static implicit operator Complex(uint value)
		{
			return new Complex(value, 0.0);
		}

		[CLSCompliant(false)]
		public static implicit operator Complex(ulong value)
		{
			return new Complex(value, 0.0);
		}

		[CLSCompliant(false)]
		public static implicit operator Complex(sbyte value)
		{
			return new Complex((double)value, 0.0);
		}

		public static implicit operator Complex(byte value)
		{
			return new Complex((double)value, 0.0);
		}

		public static implicit operator Complex(float value)
		{
			return new Complex((double)value, 0.0);
		}

		public static implicit operator Complex(double value)
		{
			return new Complex(value, 0.0);
		}

		public static explicit operator Complex(BigInteger value)
		{
			return new Complex((double)value, 0.0);
		}

		public static explicit operator Complex(decimal value)
		{
			return new Complex((double)value, 0.0);
		}

		public static readonly Complex Zero = new Complex(0.0, 0.0);

		public static readonly Complex One = new Complex(1.0, 0.0);

		public static readonly Complex ImaginaryOne = new Complex(0.0, 1.0);

		private const double InverseOfLog10 = 0.43429448190325;

		private static readonly double s_sqrtRescaleThreshold = double.MaxValue / (Math.Sqrt(2.0) + 1.0);

		private static readonly double s_asinOverflowThreshold = Math.Sqrt(double.MaxValue) / 2.0;

		private static readonly double s_log2 = Math.Log(2.0);

		private double m_real;

		private double m_imaginary;
	}
}
