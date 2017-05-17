// Type: System.Decimal
// Assembly: mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\mscorlib.dll

using System.Globalization;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;

namespace System
{
  /// <summary>
  /// Represents a decimal number.
  /// </summary>
  /// <filterpriority>1</filterpriority>
  [ComVisible(true)]
  [__DynamicallyInvokable]
  [Serializable]
  public struct Decimal : IFormattable, IComparable, IConvertible, IDeserializationCallback, IComparable<Decimal>, IEquatable<Decimal>
  {
    private static uint[] Powers10 = new uint[10]
    {
      1U,
      10U,
      100U,
      1000U,
      10000U,
      100000U,
      1000000U,
      10000000U,
      100000000U,
      1000000000U
    };
    /// <summary>
    /// Represents the number zero (0).
    /// </summary>
    /// <filterpriority>1</filterpriority>
    [__DynamicallyInvokable]
    public const Decimal Zero = new Decimal(0);
    /// <summary>
    /// Represents the number one (1).
    /// </summary>
    /// <filterpriority>1</filterpriority>
    [__DynamicallyInvokable]
    public const Decimal One = new Decimal(1);
    /// <summary>
    /// Represents the number negative one (-1).
    /// </summary>
    /// <filterpriority>1</filterpriority>
    [__DynamicallyInvokable]
    public const Decimal MinusOne = new Decimal(-1);
    /// <summary>
    /// Represents the largest possible value of <see cref="T:System.Decimal"/>. This field is constant and read-only.
    /// </summary>
    /// <filterpriority>1</filterpriority>
    [__DynamicallyInvokable]
    public const Decimal MaxValue = new Decimal(-1, -1, -1, false, (byte) 0);
    /// <summary>
    /// Represents the smallest possible value of <see cref="T:System.Decimal"/>. This field is constant and read-only.
    /// </summary>
    /// <filterpriority>1</filterpriority>
    [__DynamicallyInvokable]
    public const Decimal MinValue = new Decimal(-1, -1, -1, true, (byte) 0);
    private const Decimal NearNegativeZero = new Decimal(1, 0, 0, true, (byte) 27);
    private const Decimal NearPositiveZero = new Decimal(1, 0, 0, false, (byte) 27);
    private int flags;
    private int hi;
    private int lo;
    private int mid;
    private const int SignMask = -2147483648;
    private const byte DECIMAL_NEG = (byte) 128;
    private const byte DECIMAL_ADD = (byte) 0;
    private const int ScaleMask = 16711680;
    private const int ScaleShift = 16;
    private const int MaxInt32Scale = 9;

    static Decimal()
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="T:System.Decimal"/> to the value of the specified 32-bit signed integer.
    /// </summary>
    /// <param name="value">The value to represent as a <see cref="T:System.Decimal"/>. </param>
    [__DynamicallyInvokable]
    public Decimal(int value)
    {
      int num = value;
      if (num >= 0)
      {
        this.flags = 0;
      }
      else
      {
        this.flags = int.MinValue;
        num = -num;
      }
      this.lo = num;
      this.mid = 0;
      this.hi = 0;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="T:System.Decimal"/> to the value of the specified 32-bit unsigned integer.
    /// </summary>
    /// <param name="value">The value to represent as a <see cref="T:System.Decimal"/>. </param>
    [CLSCompliant(false)]
    [__DynamicallyInvokable]
    public Decimal(uint value)
    {
      this.flags = 0;
      this.lo = (int) value;
      this.mid = 0;
      this.hi = 0;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="T:System.Decimal"/> to the value of the specified 64-bit signed integer.
    /// </summary>
    /// <param name="value">The value to represent as a <see cref="T:System.Decimal"/>. </param>
    [__DynamicallyInvokable]
    public Decimal(long value)
    {
      long num = value;
      if (num >= 0L)
      {
        this.flags = 0;
      }
      else
      {
        this.flags = int.MinValue;
        num = -num;
      }
      this.lo = (int) num;
      this.mid = (int) (num >> 32);
      this.hi = 0;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="T:System.Decimal"/> to the value of the specified 64-bit unsigned integer.
    /// </summary>
    /// <param name="value">The value to represent as a <see cref="T:System.Decimal"/>. </param>
    [CLSCompliant(false)]
    [__DynamicallyInvokable]
    public Decimal(ulong value)
    {
      this.flags = 0;
      this.lo = (int) value;
      this.mid = (int) (value >> 32);
      this.hi = 0;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="T:System.Decimal"/> to the value of the specified single-precision floating-point number.
    /// </summary>
    /// <param name="value">The value to represent as a <see cref="T:System.Decimal"/>. </param><exception cref="T:System.OverflowException"><paramref name="value"/> is greater than <see cref="F:System.Decimal.MaxValue"/> or less than <see cref="F:System.Decimal.MinValue"/>.-or- <paramref name="value"/> is <see cref="F:System.Single.NaN"/>, <see cref="F:System.Single.PositiveInfinity"/>, or <see cref="F:System.Single.NegativeInfinity"/>. </exception>
    [SecuritySafeCritical]
    [__DynamicallyInvokable]
    [MethodImpl(MethodImplOptions.InternalCall)]
    public Decimal(float value);

    /// <summary>
    /// Initializes a new instance of <see cref="T:System.Decimal"/> to the value of the specified double-precision floating-point number.
    /// </summary>
    /// <param name="value">The value to represent as a <see cref="T:System.Decimal"/>. </param><exception cref="T:System.OverflowException"><paramref name="value"/> is greater than <see cref="F:System.Decimal.MaxValue"/> or less than <see cref="F:System.Decimal.MinValue"/>.-or- <paramref name="value"/> is <see cref="F:System.Double.NaN"/>, <see cref="F:System.Double.PositiveInfinity"/>, or <see cref="F:System.Double.NegativeInfinity"/>. </exception>
    [SecuritySafeCritical]
    [__DynamicallyInvokable]
    [MethodImpl(MethodImplOptions.InternalCall)]
    public Decimal(double value);

    [ForceTokenStabilization]
    internal Decimal(Currency value)
    {
      Decimal num = Currency.ToDecimal(value);
      this.lo = num.lo;
      this.mid = num.mid;
      this.hi = num.hi;
      this.flags = num.flags;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="T:System.Decimal"/> to a decimal value represented in binary and contained in a specified array.
    /// </summary>
    /// <param name="bits">An array of 32-bit signed integers containing a representation of a decimal value. </param><exception cref="T:System.ArgumentNullException"><paramref name="bits"/> is null. </exception><exception cref="T:System.ArgumentException">The length of the <paramref name="bits"/> is not 4.-or- The representation of the decimal value in <paramref name="bits"/> is not valid. </exception>
    [__DynamicallyInvokable]
    public Decimal(int[] bits)
    {
      this.lo = 0;
      this.mid = 0;
      this.hi = 0;
      this.flags = 0;
      this.SetBits(bits);
    }

    /// <summary>
    /// Initializes a new instance of <see cref="T:System.Decimal"/> from parameters specifying the instance's constituent parts.
    /// </summary>
    /// <param name="lo">The low 32 bits of a 96-bit integer. </param><param name="mid">The middle 32 bits of a 96-bit integer. </param><param name="hi">The high 32 bits of a 96-bit integer. </param><param name="isNegative">true to indicate a negative number; false to indicate a positive number. </param><param name="scale">A power of 10 ranging from 0 to 28. </param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="scale"/> is greater than 28. </exception>
    [__DynamicallyInvokable]
    public Decimal(int lo, int mid, int hi, bool isNegative, byte scale)
    {
      if ((int) scale > 28)
        throw new ArgumentOutOfRangeException("scale", Environment.GetResourceString("ArgumentOutOfRange_DecimalScale"));
      this.lo = lo;
      this.mid = mid;
      this.hi = hi;
      this.flags = (int) scale << 16;
      if (!isNegative)
        return;
      this.flags |= int.MinValue;
    }

    private Decimal(int lo, int mid, int hi, int flags)
    {
      if ((flags & 2130771967) != 0 || (flags & 16711680) > 1835008)
        throw new ArgumentException(Environment.GetResourceString("Arg_DecBitCtor"));
      this.lo = lo;
      this.mid = mid;
      this.hi = hi;
      this.flags = flags;
    }

    /// <summary>
    /// Defines an explicit conversion of an 8-bit unsigned integer to a <see cref="T:System.Decimal"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The converted 8-bit unsigned integer.
    /// </returns>
    /// <param name="value">The 8-bit unsigned integer to convert. </param><filterpriority>3</filterpriority>
    [__DynamicallyInvokable]
    public static implicit operator Decimal(byte value)
    {
      return new Decimal((int) value);
    }

    /// <summary>
    /// Defines an explicit conversion of an 8-bit signed integer to a <see cref="T:System.Decimal"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The converted 8-bit signed integer.
    /// </returns>
    /// <param name="value">The 8-bit signed integer to convert. </param><filterpriority>3</filterpriority>
    [CLSCompliant(false)]
    [__DynamicallyInvokable]
    public static implicit operator Decimal(sbyte value)
    {
      return new Decimal((int) value);
    }

    /// <summary>
    /// Defines an explicit conversion of a 16-bit signed integer to a <see cref="T:System.Decimal"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The converted 16-bit signed integer.
    /// </returns>
    /// <param name="value">The16-bit signed integer to convert. </param><filterpriority>3</filterpriority>
    [__DynamicallyInvokable]
    public static implicit operator Decimal(short value)
    {
      return new Decimal((int) value);
    }

    /// <summary>
    /// Defines an explicit conversion of a 16-bit unsigned integer to a <see cref="T:System.Decimal"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The converted 16-bit unsigned integer.
    /// </returns>
    /// <param name="value">The 16-bit unsigned integer to convert. </param><filterpriority>3</filterpriority>
    [CLSCompliant(false)]
    [__DynamicallyInvokable]
    public static implicit operator Decimal(ushort value)
    {
      return new Decimal((int) value);
    }

    /// <summary>
    /// Defines an explicit conversion of a Unicode character to a <see cref="T:System.Decimal"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The converted Unicode character.
    /// </returns>
    /// <param name="value">The Unicode character to convert. </param><filterpriority>3</filterpriority>
    [__DynamicallyInvokable]
    public static implicit operator Decimal(char value)
    {
      return new Decimal((int) value);
    }

    /// <summary>
    /// Defines an explicit conversion of a 32-bit signed integer to a <see cref="T:System.Decimal"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The converted 32-bit signed integer.
    /// </returns>
    /// <param name="value">The 32-bit signed integer to convert. </param><filterpriority>3</filterpriority>
    [__DynamicallyInvokable]
    public static implicit operator Decimal(int value)
    {
      return new Decimal(value);
    }

    /// <summary>
    /// Defines an explicit conversion of a 32-bit unsigned integer to a <see cref="T:System.Decimal"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The converted 32-bit unsigned integer.
    /// </returns>
    /// <param name="value">The 32-bit unsigned integer to convert. </param><filterpriority>3</filterpriority>
    [CLSCompliant(false)]
    [__DynamicallyInvokable]
    public static implicit operator Decimal(uint value)
    {
      return new Decimal(value);
    }

    /// <summary>
    /// Defines an explicit conversion of a 64-bit signed integer to a <see cref="T:System.Decimal"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The converted 64-bit signed integer.
    /// </returns>
    /// <param name="value">The 64-bit signed integer to convert. </param><filterpriority>3</filterpriority>
    [__DynamicallyInvokable]
    public static implicit operator Decimal(long value)
    {
      return new Decimal(value);
    }

    /// <summary>
    /// Defines an explicit conversion of a 64-bit unsigned integer to a <see cref="T:System.Decimal"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The converted 64-bit unsigned integer.
    /// </returns>
    /// <param name="value">The 64-bit unsigned integer to convert. </param><filterpriority>3</filterpriority>
    [CLSCompliant(false)]
    [__DynamicallyInvokable]
    public static implicit operator Decimal(ulong value)
    {
      return new Decimal(value);
    }

    /// <summary>
    /// Defines an explicit conversion of a single-precision floating-point number to a <see cref="T:System.Decimal"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The converted single-precision floating point number.
    /// </returns>
    /// <param name="value">The single-precision floating-point number to convert. </param><exception cref="T:System.OverflowException"><paramref name="value"/> is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>.-or- <paramref name="value"/> is <see cref="F:System.Single.NaN"/>, <see cref="F:System.Single.PositiveInfinity"/>, or <see cref="F:System.Single.NegativeInfinity"/>. </exception><filterpriority>3</filterpriority>
    [__DynamicallyInvokable]
    public static explicit operator Decimal(float value)
    {
      return new Decimal(value);
    }

    /// <summary>
    /// Defines an explicit conversion of a double-precision floating-point number to a <see cref="T:System.Decimal"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The converted double-precision floating point number.
    /// </returns>
    /// <param name="value">The double-precision floating-point number to convert. </param><exception cref="T:System.OverflowException"><paramref name="value"/> is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>.-or- <paramref name="value"/> is <see cref="F:System.Double.NaN"/>, <see cref="F:System.Double.PositiveInfinity"/>, or <see cref="F:System.Double.NegativeInfinity"/>. </exception><filterpriority>3</filterpriority>
    [__DynamicallyInvokable]
    public static explicit operator Decimal(double value)
    {
      return new Decimal(value);
    }

    /// <summary>
    /// Defines an explicit conversion of a <see cref="T:System.Decimal"/> to an 8-bit unsigned integer.
    /// </summary>
    /// 
    /// <returns>
    /// An 8-bit unsigned integer that represents the converted <see cref="T:System.Decimal"/>.
    /// </returns>
    /// <param name="value">The value to convert. </param><exception cref="T:System.OverflowException"><paramref name="value"/> is less than <see cref="F:System.Byte.MinValue"/> or greater than <see cref="F:System.Byte.MaxValue"/>. </exception><filterpriority>3</filterpriority>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static explicit operator byte(Decimal value)
    {
      return Decimal.ToByte(value);
    }

    /// <summary>
    /// Defines an explicit conversion of a <see cref="T:System.Decimal"/> to an 8-bit signed integer.
    /// </summary>
    /// 
    /// <returns>
    /// An 8-bit signed integer that represents the converted <see cref="T:System.Decimal"/>.
    /// </returns>
    /// <param name="value">The value to convert. </param><exception cref="T:System.OverflowException"><paramref name="value"/> is less than <see cref="F:System.SByte.MinValue"/> or greater than <see cref="F:System.SByte.MaxValue"/>. </exception><filterpriority>3</filterpriority>
    [CLSCompliant(false)]
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static explicit operator sbyte(Decimal value)
    {
      return Decimal.ToSByte(value);
    }

    /// <summary>
    /// Defines an explicit conversion of a <see cref="T:System.Decimal"/> to a Unicode character.
    /// </summary>
    /// 
    /// <returns>
    /// A Unicode character that represents the converted <see cref="T:System.Decimal"/>.
    /// </returns>
    /// <param name="value">The value to convert. </param><exception cref="T:System.OverflowException"><paramref name="value"/> is less than <see cref="F:System.Char.MinValue"/> or greater than <see cref="F:System.Char.MaxValue"/>. </exception><filterpriority>3</filterpriority>
    [__DynamicallyInvokable]
    public static explicit operator char(Decimal value)
    {
      try
      {
        return (char) Decimal.ToUInt16(value);
      }
      catch (OverflowException ex)
      {
        throw new OverflowException(Environment.GetResourceString("Overflow_Char"), (Exception) ex);
      }
    }

    /// <summary>
    /// Defines an explicit conversion of a <see cref="T:System.Decimal"/> to a 16-bit signed integer.
    /// </summary>
    /// 
    /// <returns>
    /// A 16-bit signed integer that represents the converted <see cref="T:System.Decimal"/>.
    /// </returns>
    /// <param name="value">The value to convert. </param><exception cref="T:System.OverflowException"><paramref name="value"/> is less than <see cref="F:System.Int16.MinValue"/> or greater than <see cref="F:System.Int16.MaxValue"/>. </exception><filterpriority>3</filterpriority>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static explicit operator short(Decimal value)
    {
      return Decimal.ToInt16(value);
    }

    /// <summary>
    /// Defines an explicit conversion of a <see cref="T:System.Decimal"/> to a 16-bit unsigned integer.
    /// </summary>
    /// 
    /// <returns>
    /// A 16-bit unsigned integer that represents the converted <see cref="T:System.Decimal"/>.
    /// </returns>
    /// <param name="value">The value to convert. </param><exception cref="T:System.OverflowException"><paramref name="value"/> is greater than <see cref="F:System.UInt16.MaxValue"/> or less than <see cref="F:System.UInt16.MinValue"/>. </exception><filterpriority>3</filterpriority>
    [CLSCompliant(false)]
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static explicit operator ushort(Decimal value)
    {
      return Decimal.ToUInt16(value);
    }

    /// <summary>
    /// Defines an explicit conversion of a <see cref="T:System.Decimal"/> to a 32-bit signed integer.
    /// </summary>
    /// 
    /// <returns>
    /// A 32-bit signed integer that represents the converted <see cref="T:System.Decimal"/>.
    /// </returns>
    /// <param name="value">The value to convert. </param><exception cref="T:System.OverflowException"><paramref name="value"/> is less than <see cref="F:System.Int32.MinValue"/> or greater than <see cref="F:System.Int32.MaxValue"/>. </exception><filterpriority>3</filterpriority>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static explicit operator int(Decimal value)
    {
      return Decimal.ToInt32(value);
    }

    /// <summary>
    /// Defines an explicit conversion of a <see cref="T:System.Decimal"/> to a 32-bit unsigned integer.
    /// </summary>
    /// 
    /// <returns>
    /// A 32-bit unsigned integer that represents the converted <see cref="T:System.Decimal"/>.
    /// </returns>
    /// <param name="value">The value to convert. </param><exception cref="T:System.OverflowException"><paramref name="value"/> is negative or greater than <see cref="F:System.UInt32.MaxValue"/>. </exception><filterpriority>3</filterpriority>
    [CLSCompliant(false)]
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static explicit operator uint(Decimal value)
    {
      return Decimal.ToUInt32(value);
    }

    /// <summary>
    /// Defines an explicit conversion of a <see cref="T:System.Decimal"/> to a 64-bit signed integer.
    /// </summary>
    /// 
    /// <returns>
    /// A 64-bit signed integer that represents the converted <see cref="T:System.Decimal"/>.
    /// </returns>
    /// <param name="value">The value to convert. </param><exception cref="T:System.OverflowException"><paramref name="value"/> is less than <see cref="F:System.Int64.MinValue"/> or greater than <see cref="F:System.Int64.MaxValue"/>. </exception><filterpriority>3</filterpriority>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static explicit operator long(Decimal value)
    {
      return Decimal.ToInt64(value);
    }

    /// <summary>
    /// Defines an explicit conversion of a <see cref="T:System.Decimal"/> to a 64-bit unsigned integer.
    /// </summary>
    /// 
    /// <returns>
    /// A 64-bit unsigned integer that represents the converted <see cref="T:System.Decimal"/>.
    /// </returns>
    /// <param name="value">The value to convert. </param><exception cref="T:System.OverflowException"><paramref name="value"/> is negative or greater than <see cref="F:System.UInt64.MaxValue"/>. </exception><filterpriority>3</filterpriority>
    [CLSCompliant(false)]
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static explicit operator ulong(Decimal value)
    {
      return Decimal.ToUInt64(value);
    }

    /// <summary>
    /// Defines an explicit conversion of a <see cref="T:System.Decimal"/> to a single-precision floating-point number.
    /// </summary>
    /// 
    /// <returns>
    /// A single-precision floating-point number that represents the converted <see cref="T:System.Decimal"/>.
    /// </returns>
    /// <param name="value">The value to convert. </param><filterpriority>3</filterpriority>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static explicit operator float(Decimal value)
    {
      return Decimal.ToSingle(value);
    }

    /// <summary>
    /// Defines an explicit conversion of a <see cref="T:System.Decimal"/> to a double-precision floating-point number.
    /// </summary>
    /// 
    /// <returns>
    /// A double-precision floating-point number that represents the converted <see cref="T:System.Decimal"/>.
    /// </returns>
    /// <param name="value">The value to convert. </param><filterpriority>3</filterpriority>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static explicit operator double(Decimal value)
    {
      return Decimal.ToDouble(value);
    }

    /// <summary>
    /// Returns the value of the <see cref="T:System.Decimal"/> operand (the sign of the operand is unchanged).
    /// </summary>
    /// 
    /// <returns>
    /// The value of the operand, <paramref name="d"/>.
    /// </returns>
    /// <param name="d">The operand to return.</param><filterpriority>3</filterpriority>
    [__DynamicallyInvokable]
    public static Decimal operator +(Decimal d)
    {
      return d;
    }

    /// <summary>
    /// Negates the value of the specified <see cref="T:System.Decimal"/> operand.
    /// </summary>
    /// 
    /// <returns>
    /// The result of <paramref name="d"/> multiplied by negative one (-1).
    /// </returns>
    /// <param name="d">The value to negate. </param><filterpriority>3</filterpriority>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static Decimal operator -(Decimal d)
    {
      return Decimal.Negate(d);
    }

    /// <summary>
    /// Increments the <see cref="T:System.Decimal"/> operand by 1.
    /// </summary>
    /// 
    /// <returns>
    /// The value of <paramref name="d"/> incremented by 1.
    /// </returns>
    /// <param name="d">The value to increment. </param><exception cref="T:System.OverflowException">The return value is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>. </exception><filterpriority>3</filterpriority>
    [__DynamicallyInvokable]
    public static Decimal operator ++(Decimal d)
    {
      return Decimal.Add(d, new Decimal(1));
    }

    /// <summary>
    /// Decrements the <see cref="T:System.Decimal"/> operand by one.
    /// </summary>
    /// 
    /// <returns>
    /// The value of <paramref name="d"/> decremented by 1.
    /// </returns>
    /// <param name="d">The value to decrement. </param><exception cref="T:System.OverflowException">The return value is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>. </exception><filterpriority>3</filterpriority>
    [__DynamicallyInvokable]
    public static Decimal operator --(Decimal d)
    {
      return Decimal.Subtract(d, new Decimal(1));
    }

    /// <summary>
    /// Adds two specified <see cref="T:System.Decimal"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// The result of adding <paramref name="d1"/> and <paramref name="d2"/>.
    /// </returns>
    /// <param name="d1">The first value to add. </param><param name="d2">The second value to add. </param><exception cref="T:System.OverflowException">The return value is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>. </exception><filterpriority>3</filterpriority>
    [SecuritySafeCritical]
    [__DynamicallyInvokable]
    public static Decimal operator +(Decimal d1, Decimal d2)
    {
      Decimal.FCallAddSub(ref d1, ref d2, (byte) 0);
      return d1;
    }

    /// <summary>
    /// Subtracts two specified <see cref="T:System.Decimal"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// The result of subtracting <paramref name="d2"/> from <paramref name="d1"/>.
    /// </returns>
    /// <param name="d1">The minuend. </param><param name="d2">The subtrahend. </param><exception cref="T:System.OverflowException">The return value is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>. </exception><filterpriority>3</filterpriority>
    [SecuritySafeCritical]
    [__DynamicallyInvokable]
    public static Decimal operator -(Decimal d1, Decimal d2)
    {
      Decimal.FCallAddSub(ref d1, ref d2, (byte) sbyte.MinValue);
      return d1;
    }

    /// <summary>
    /// Multiplies two specified <see cref="T:System.Decimal"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// The result of multiplying <paramref name="d1"/> by <paramref name="d2"/>.
    /// </returns>
    /// <param name="d1">The first value to multiply. </param><param name="d2">The second value to multiply. </param><exception cref="T:System.OverflowException">The return value is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>. </exception><filterpriority>3</filterpriority>
    [SecuritySafeCritical]
    [__DynamicallyInvokable]
    public static Decimal operator *(Decimal d1, Decimal d2)
    {
      Decimal.FCallMultiply(ref d1, ref d2);
      return d1;
    }

    /// <summary>
    /// Divides two specified <see cref="T:System.Decimal"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// The result of dividing <paramref name="d1"/> by <paramref name="d2"/>.
    /// </returns>
    /// <param name="d1">The dividend. </param><param name="d2">The divisor. </param><exception cref="T:System.DivideByZeroException"><paramref name="d2"/> is zero. </exception><exception cref="T:System.OverflowException">The return value is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>. </exception><filterpriority>3</filterpriority>
    [SecuritySafeCritical]
    [__DynamicallyInvokable]
    public static Decimal operator /(Decimal d1, Decimal d2)
    {
      Decimal.FCallDivide(ref d1, ref d2);
      return d1;
    }

    /// <summary>
    /// Returns the remainder resulting from dividing two specified <see cref="T:System.Decimal"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// The remainder resulting from dividing <paramref name="d1"/> by <paramref name="d2"/>.
    /// </returns>
    /// <param name="d1">The dividend. </param><param name="d2">The divisor. </param><exception cref="T:System.DivideByZeroException"><paramref name="d2"/> is zero. </exception><exception cref="T:System.OverflowException">The return value is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>. </exception><filterpriority>3</filterpriority>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static Decimal operator %(Decimal d1, Decimal d2)
    {
      return Decimal.Remainder(d1, d2);
    }

    /// <summary>
    /// Returns a value that indicates whether two <see cref="T:System.Decimal"/> values are equal.
    /// </summary>
    /// 
    /// <returns>
    /// true if <paramref name="d1"/> and <paramref name="d2"/> are equal; otherwise, false.
    /// </returns>
    /// <param name="d1">The first value to compare. </param><param name="d2">The second value to compare. </param><filterpriority>3</filterpriority>
    [SecuritySafeCritical]
    [__DynamicallyInvokable]
    public static bool operator ==(Decimal d1, Decimal d2)
    {
      return Decimal.FCallCompare(ref d1, ref d2) == 0;
    }

    /// <summary>
    /// Returns a value that indicates whether two <see cref="T:System.Decimal"/> objects have different values.
    /// </summary>
    /// 
    /// <returns>
    /// true if <paramref name="d1"/> and <paramref name="d2"/> are not equal; otherwise, false.
    /// </returns>
    /// <param name="d1">The first value to compare. </param><param name="d2">The second value to compare. </param><filterpriority>3</filterpriority>
    [SecuritySafeCritical]
    [__DynamicallyInvokable]
    public static bool operator !=(Decimal d1, Decimal d2)
    {
      return Decimal.FCallCompare(ref d1, ref d2) != 0;
    }

    /// <summary>
    /// Returns a value indicating whether a specified <see cref="T:System.Decimal"/> is less than another specified <see cref="T:System.Decimal"/>.
    /// </summary>
    /// 
    /// <returns>
    /// true if <paramref name="d1"/> is less than <paramref name="d2"/>; otherwise, false.
    /// </returns>
    /// <param name="d1">The first value to compare. </param><param name="d2">The second value to compare. </param><filterpriority>3</filterpriority>
    [SecuritySafeCritical]
    [__DynamicallyInvokable]
    public static bool operator <(Decimal d1, Decimal d2)
    {
      return Decimal.FCallCompare(ref d1, ref d2) < 0;
    }

    /// <summary>
    /// Returns a value indicating whether a specified <see cref="T:System.Decimal"/> is less than or equal to another specified <see cref="T:System.Decimal"/>.
    /// </summary>
    /// 
    /// <returns>
    /// true if <paramref name="d1"/> is less than or equal to <paramref name="d2"/>; otherwise, false.
    /// </returns>
    /// <param name="d1">The first value to compare. </param><param name="d2">The second value to compare. </param><filterpriority>3</filterpriority>
    [SecuritySafeCritical]
    [__DynamicallyInvokable]
    public static bool operator <=(Decimal d1, Decimal d2)
    {
      return Decimal.FCallCompare(ref d1, ref d2) <= 0;
    }

    /// <summary>
    /// Returns a value indicating whether a specified <see cref="T:System.Decimal"/> is greater than another specified <see cref="T:System.Decimal"/>.
    /// </summary>
    /// 
    /// <returns>
    /// true if <paramref name="d1"/> is greater than <paramref name="d2"/>; otherwise, false.
    /// </returns>
    /// <param name="d1">The first value to compare. </param><param name="d2">The second value to compare. </param><filterpriority>3</filterpriority>
    [SecuritySafeCritical]
    [__DynamicallyInvokable]
    public static bool operator >(Decimal d1, Decimal d2)
    {
      return Decimal.FCallCompare(ref d1, ref d2) > 0;
    }

    /// <summary>
    /// Returns a value indicating whether a specified <see cref="T:System.Decimal"/> is greater than or equal to another specified <see cref="T:System.Decimal"/>.
    /// </summary>
    /// 
    /// <returns>
    /// true if <paramref name="d1"/> is greater than or equal to <paramref name="d2"/>; otherwise, false.
    /// </returns>
    /// <param name="d1">The first value to compare. </param><param name="d2">The second value to compare. </param><filterpriority>3</filterpriority>
    [SecuritySafeCritical]
    [__DynamicallyInvokable]
    public static bool operator >=(Decimal d1, Decimal d2)
    {
      return Decimal.FCallCompare(ref d1, ref d2) >= 0;
    }

    /// <summary>
    /// Converts the specified <see cref="T:System.Decimal"/> value to the equivalent OLE Automation Currency value, which is contained in a 64-bit signed integer.
    /// </summary>
    /// 
    /// <returns>
    /// A 64-bit signed integer that contains the OLE Automation equivalent of <paramref name="value"/>.
    /// </returns>
    /// <param name="value">The decimal number to convert. </param><filterpriority>2</filterpriority>
    [__DynamicallyInvokable]
    public static long ToOACurrency(Decimal value)
    {
      return new Currency(value).ToOACurrency();
    }

    /// <summary>
    /// Converts the specified 64-bit signed integer, which contains an OLE Automation Currency value, to the equivalent <see cref="T:System.Decimal"/> value.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Decimal"/> that contains the equivalent of <paramref name="cy"/>.
    /// </returns>
    /// <param name="cy">An OLE Automation Currency value. </param><filterpriority>1</filterpriority>
    [__DynamicallyInvokable]
    public static Decimal FromOACurrency(long cy)
    {
      return Currency.ToDecimal(Currency.FromOACurrency(cy));
    }

    void IDeserializationCallback.OnDeserialization(object sender)
    {
      try
      {
        this.SetBits(Decimal.GetBits(this));
      }
      catch (ArgumentException ex)
      {
        throw new SerializationException(Environment.GetResourceString("Overflow_Decimal"), (Exception) ex);
      }
    }

    internal static Decimal Abs(Decimal d)
    {
      return new Decimal(d.lo, d.mid, d.hi, d.flags & int.MaxValue);
    }

    /// <summary>
    /// Adds two specified <see cref="T:System.Decimal"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// The sum of <paramref name="d1"/> and <paramref name="d2"/>.
    /// </returns>
    /// <param name="d1">The first value to add. </param><param name="d2">The second value to add. </param><exception cref="T:System.OverflowException">The sum of <paramref name="d1"/> and <paramref name="d2"/> is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>. </exception><filterpriority>1</filterpriority>
    [SecuritySafeCritical]
    [__DynamicallyInvokable]
    public static Decimal Add(Decimal d1, Decimal d2)
    {
      Decimal.FCallAddSub(ref d1, ref d2, (byte) 0);
      return d1;
    }

    /// <summary>
    /// Returns the smallest integral value that is greater than or equal to the specified decimal number.
    /// </summary>
    /// 
    /// <returns>
    /// The smallest integral value that is greater than or equal to the <paramref name="d"/> parameter. Note that this method returns a <see cref="T:System.Decimal"/> instead of an integral type.
    /// </returns>
    /// <param name="d">A decimal number. </param><filterpriority>1</filterpriority>
    [__DynamicallyInvokable]
    public static Decimal Ceiling(Decimal d)
    {
      return -Decimal.Floor(-d);
    }

    /// <summary>
    /// Compares two specified <see cref="T:System.Decimal"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// A signed number indicating the relative values of <paramref name="d1"/> and <paramref name="d2"/>.Return value Meaning Less than zero <paramref name="d1"/> is less than <paramref name="d2"/>. Zero <paramref name="d1"/> and <paramref name="d2"/> are equal. Greater than zero <paramref name="d1"/> is greater than <paramref name="d2"/>.
    /// </returns>
    /// <param name="d1">The first value to compare. </param><param name="d2">The second value to compare. </param><filterpriority>1</filterpriority>
    [SecuritySafeCritical]
    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    [__DynamicallyInvokable]
    public static int Compare(Decimal d1, Decimal d2)
    {
      return Decimal.FCallCompare(ref d1, ref d2);
    }

    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    [SecurityCritical]
    [MethodImpl(MethodImplOptions.InternalCall)]
    private static int FCallCompare(ref Decimal d1, ref Decimal d2);

    /// <summary>
    /// Compares this instance to a specified object and returns a comparison of their relative values.
    /// </summary>
    /// 
    /// <returns>
    /// A signed number indicating the relative values of this instance and <paramref name="value"/>.Return value Meaning Less than zero This instance is less than <paramref name="value"/>. Zero This instance is equal to <paramref name="value"/>. Greater than zero This instance is greater than <paramref name="value"/>.-or- <paramref name="value"/> is null.
    /// </returns>
    /// <param name="value">The object to compare with this instance, or null. </param><exception cref="T:System.ArgumentException"><paramref name="value"/> is not a <see cref="T:System.Decimal"/>. </exception><filterpriority>2</filterpriority>
    [SecuritySafeCritical]
    public int CompareTo(object value)
    {
      if (value == null)
        return 1;
      if (!(value is Decimal))
        throw new ArgumentException(Environment.GetResourceString("Arg_MustBeDecimal"));
      Decimal d2 = (Decimal) value;
      return Decimal.FCallCompare(ref this, ref d2);
    }

    /// <summary>
    /// Compares this instance to a specified <see cref="T:System.Decimal"/> object and returns a comparison of their relative values.
    /// </summary>
    /// 
    /// <returns>
    /// A signed number indicating the relative values of this instance and <paramref name="value"/>.Return value Meaning Less than zero This instance is less than <paramref name="value"/>. Zero This instance is equal to <paramref name="value"/>. Greater than zero This instance is greater than <paramref name="value"/>.
    /// </returns>
    /// <param name="value">The object to compare with this instance.</param><filterpriority>2</filterpriority>
    [SecuritySafeCritical]
    [__DynamicallyInvokable]
    public int CompareTo(Decimal value)
    {
      return Decimal.FCallCompare(ref this, ref value);
    }

    /// <summary>
    /// Divides two specified <see cref="T:System.Decimal"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// The result of dividing <paramref name="d1"/> by <paramref name="d2"/>.
    /// </returns>
    /// <param name="d1">The dividend. </param><param name="d2">The divisor. </param><exception cref="T:System.DivideByZeroException"><paramref name="d2"/> is zero. </exception><exception cref="T:System.OverflowException">The return value (that is, the quotient) is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>. </exception><filterpriority>1</filterpriority>
    [SecuritySafeCritical]
    [__DynamicallyInvokable]
    public static Decimal Divide(Decimal d1, Decimal d2)
    {
      Decimal.FCallDivide(ref d1, ref d2);
      return d1;
    }

    /// <summary>
    /// Returns a value indicating whether this instance and a specified <see cref="T:System.Object"/> represent the same type and value.
    /// </summary>
    /// 
    /// <returns>
    /// true if <paramref name="value"/> is a <see cref="T:System.Decimal"/> and equal to this instance; otherwise, false.
    /// </returns>
    /// <param name="value">The object to compare with this instance. </param><filterpriority>2</filterpriority>
    [SecuritySafeCritical]
    [__DynamicallyInvokable]
    public override bool Equals(object value)
    {
      if (!(value is Decimal))
        return false;
      Decimal d2 = (Decimal) value;
      return Decimal.FCallCompare(ref this, ref d2) == 0;
    }

    /// <summary>
    /// Returns a value indicating whether this instance and a specified <see cref="T:System.Decimal"/> object represent the same value.
    /// </summary>
    /// 
    /// <returns>
    /// true if <paramref name="value"/> is equal to this instance; otherwise, false.
    /// </returns>
    /// <param name="value">An object to compare to this instance.</param><filterpriority>2</filterpriority>
    [SecuritySafeCritical]
    [__DynamicallyInvokable]
    public bool Equals(Decimal value)
    {
      return Decimal.FCallCompare(ref this, ref value) == 0;
    }

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// 
    /// <returns>
    /// A 32-bit signed integer hash code.
    /// </returns>
    /// <filterpriority>2</filterpriority>
    [SecuritySafeCritical]
    [__DynamicallyInvokable]
    [MethodImpl(MethodImplOptions.InternalCall)]
    public override int GetHashCode();

    /// <summary>
    /// Returns a value indicating whether two specified instances of <see cref="T:System.Decimal"/> represent the same value.
    /// </summary>
    /// 
    /// <returns>
    /// true if <paramref name="d1"/> and <paramref name="d2"/> are equal; otherwise, false.
    /// </returns>
    /// <param name="d1">The first value to compare. </param><param name="d2">The second value to compare. </param><filterpriority>1</filterpriority>
    [SecuritySafeCritical]
    [__DynamicallyInvokable]
    public static bool Equals(Decimal d1, Decimal d2)
    {
      return Decimal.FCallCompare(ref d1, ref d2) == 0;
    }

    /// <summary>
    /// Rounds a specified <see cref="T:System.Decimal"/> number to the closest integer toward negative infinity.
    /// </summary>
    /// 
    /// <returns>
    /// If <paramref name="d"/> has a fractional part, the next whole <see cref="T:System.Decimal"/> number toward negative infinity that is less than <paramref name="d"/>.-or- If <paramref name="d"/> doesn't have a fractional part, <paramref name="d"/> is returned unchanged. Note that the method returns an integral value of type <see cref="T:System.Decimal"/>.
    /// </returns>
    /// <param name="d">The value to round. </param><filterpriority>1</filterpriority>
    [SecuritySafeCritical]
    [__DynamicallyInvokable]
    public static Decimal Floor(Decimal d)
    {
      Decimal.FCallFloor(ref d);
      return d;
    }

    /// <summary>
    /// Converts the numeric value of this instance to its equivalent string representation.
    /// </summary>
    /// 
    /// <returns>
    /// A string that represents the value of this instance.
    /// </returns>
    /// <filterpriority>1</filterpriority>
    [SecuritySafeCritical]
    [__DynamicallyInvokable]
    public override string ToString()
    {
      return Number.FormatDecimal(this, (string) null, NumberFormatInfo.CurrentInfo);
    }

    /// <summary>
    /// Converts the numeric value of this instance to its equivalent string representation, using the specified format.
    /// </summary>
    /// 
    /// <returns>
    /// The string representation of the value of this instance as specified by <paramref name="format"/>.
    /// </returns>
    /// <param name="format">A standard or custom numeric format string (see Remarks).</param><exception cref="T:System.FormatException"><paramref name="format"/> is invalid. </exception><filterpriority>1</filterpriority>
    [SecuritySafeCritical]
    [__DynamicallyInvokable]
    public string ToString(string format)
    {
      return Number.FormatDecimal(this, format, NumberFormatInfo.CurrentInfo);
    }

    /// <summary>
    /// Converts the numeric value of this instance to its equivalent string representation using the specified culture-specific format information.
    /// </summary>
    /// 
    /// <returns>
    /// The string representation of the value of this instance as specified by <paramref name="provider"/>.
    /// </returns>
    /// <param name="provider">An object that supplies culture-specific formatting information. </param><filterpriority>1</filterpriority>
    [SecuritySafeCritical]
    [__DynamicallyInvokable]
    public string ToString(IFormatProvider provider)
    {
      return Number.FormatDecimal(this, (string) null, NumberFormatInfo.GetInstance(provider));
    }

    /// <summary>
    /// Converts the numeric value of this instance to its equivalent string representation using the specified format and culture-specific format information.
    /// </summary>
    /// 
    /// <returns>
    /// The string representation of the value of this instance as specified by <paramref name="format"/> and <paramref name="provider"/>.
    /// </returns>
    /// <param name="format">A numeric format string (see Remarks).</param><param name="provider">An object that supplies culture-specific formatting information. </param><exception cref="T:System.FormatException"><paramref name="format"/> is invalid. </exception><filterpriority>1</filterpriority>
    [SecuritySafeCritical]
    [__DynamicallyInvokable]
    public string ToString(string format, IFormatProvider provider)
    {
      return Number.FormatDecimal(this, format, NumberFormatInfo.GetInstance(provider));
    }

    /// <summary>
    /// Converts the string representation of a number to its <see cref="T:System.Decimal"/> equivalent.
    /// </summary>
    /// 
    /// <returns>
    /// The equivalent to the number contained in <paramref name="s"/>.
    /// </returns>
    /// <param name="s">The string representation of the number to convert.</param><exception cref="T:System.ArgumentNullException"><paramref name="s"/> is null. </exception><exception cref="T:System.FormatException"><paramref name="s"/> is not in the correct format. </exception><exception cref="T:System.OverflowException"><paramref name="s"/> represents a number less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>. </exception><filterpriority>1</filterpriority>
    [__DynamicallyInvokable]
    public static Decimal Parse(string s)
    {
      return Number.ParseDecimal(s, NumberStyles.Number, NumberFormatInfo.CurrentInfo);
    }

    /// <summary>
    /// Converts the string representation of a number in a specified style to its <see cref="T:System.Decimal"/> equivalent.
    /// </summary>
    /// 
    /// <returns>
    /// The <see cref="T:System.Decimal"/> number equivalent to the number contained in <paramref name="s"/> as specified by <paramref name="style"/>.
    /// </returns>
    /// <param name="s">The string representation of the number to convert. </param><param name="style">A bitwise combination of <see cref="T:System.Globalization.NumberStyles"/> values that indicates the style elements that can be present in <paramref name="s"/>. A typical value to specify is <see cref="F:System.Globalization.NumberStyles.Number"/>.</param><exception cref="T:System.ArgumentNullException"><paramref name="s"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="style"/> is not a <see cref="T:System.Globalization.NumberStyles"/> value. -or-<paramref name="style"/> is the <see cref="F:System.Globalization.NumberStyles.AllowHexSpecifier"/> value.</exception><exception cref="T:System.FormatException"><paramref name="s"/> is not in the correct format. </exception><exception cref="T:System.OverflowException"><paramref name="s"/> represents a number less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/></exception><filterpriority>1</filterpriority>
    [__DynamicallyInvokable]
    public static Decimal Parse(string s, NumberStyles style)
    {
      NumberFormatInfo.ValidateParseStyleFloatingPoint(style);
      return Number.ParseDecimal(s, style, NumberFormatInfo.CurrentInfo);
    }

    /// <summary>
    /// Converts the string representation of a number to its <see cref="T:System.Decimal"/> equivalent using the specified culture-specific format information.
    /// </summary>
    /// 
    /// <returns>
    /// The <see cref="T:System.Decimal"/> number equivalent to the number contained in <paramref name="s"/> as specified by <paramref name="provider"/>.
    /// </returns>
    /// <param name="s">The string representation of the number to convert. </param><param name="provider">An <see cref="T:System.IFormatProvider"/> that supplies culture-specific parsing information about <paramref name="s"/>. </param><exception cref="T:System.ArgumentNullException"><paramref name="s"/> is null. </exception><exception cref="T:System.FormatException"><paramref name="s"/> is not of the correct format </exception><exception cref="T:System.OverflowException"><paramref name="s"/> represents a number less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/></exception><filterpriority>1</filterpriority>
    [__DynamicallyInvokable]
    public static Decimal Parse(string s, IFormatProvider provider)
    {
      return Number.ParseDecimal(s, NumberStyles.Number, NumberFormatInfo.GetInstance(provider));
    }

    /// <summary>
    /// Converts the string representation of a number to its <see cref="T:System.Decimal"/> equivalent using the specified style and culture-specific format.
    /// </summary>
    /// 
    /// <returns>
    /// The <see cref="T:System.Decimal"/> number equivalent to the number contained in <paramref name="s"/> as specified by <paramref name="style"/> and <paramref name="provider"/>.
    /// </returns>
    /// <param name="s">The string representation of the number to convert. </param><param name="style">A bitwise combination of <see cref="T:System.Globalization.NumberStyles"/> values that indicates the style elements that can be present in <paramref name="s"/>. A typical value to specify is <see cref="F:System.Globalization.NumberStyles.Number"/>.</param><param name="provider">An <see cref="T:System.IFormatProvider"/> object that supplies culture-specific information about the format of <paramref name="s"/>. </param><exception cref="T:System.FormatException"><paramref name="s"/> is not in the correct format. </exception><exception cref="T:System.OverflowException"><paramref name="s"/> represents a number less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>. </exception><exception cref="T:System.ArgumentNullException"><paramref name="s"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="style"/> is not a <see cref="T:System.Globalization.NumberStyles"/> value. -or-<paramref name="style"/> is the <see cref="F:System.Globalization.NumberStyles.AllowHexSpecifier"/> value.</exception><filterpriority>1</filterpriority>
    [__DynamicallyInvokable]
    public static Decimal Parse(string s, NumberStyles style, IFormatProvider provider)
    {
      NumberFormatInfo.ValidateParseStyleFloatingPoint(style);
      return Number.ParseDecimal(s, style, NumberFormatInfo.GetInstance(provider));
    }

    /// <summary>
    /// Converts the string representation of a number to its <see cref="T:System.Decimal"/> equivalent. A return value indicates whether the conversion succeeded or failed.
    /// </summary>
    /// 
    /// <returns>
    /// true if <paramref name="s"/> was converted successfully; otherwise, false.
    /// </returns>
    /// <param name="s">The string representation of the number to convert.</param><param name="result">When this method returns, contains the <see cref="T:System.Decimal"/> number that is equivalent to the numeric value contained in <paramref name="s"/>, if the conversion succeeded, or is zero if the conversion failed. The conversion fails if the <paramref name="s"/> parameter is null, is not a number in a valid format, or represents a number less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>. This parameter is passed uninitialized. </param><filterpriority>1</filterpriority>
    [__DynamicallyInvokable]
    public static bool TryParse(string s, out Decimal result)
    {
      return Number.TryParseDecimal(s, NumberStyles.Number, NumberFormatInfo.CurrentInfo, out result);
    }

    /// <summary>
    /// Converts the string representation of a number to its <see cref="T:System.Decimal"/> equivalent using the specified style and culture-specific format. A return value indicates whether the conversion succeeded or failed.
    /// </summary>
    /// 
    /// <returns>
    /// true if <paramref name="s"/> was converted successfully; otherwise, false.
    /// </returns>
    /// <param name="s">The string representation of the number to convert.</param><param name="style">A bitwise combination of enumeration values that indicates the permitted format of <paramref name="s"/>. A typical value to specify is <see cref="F:System.Globalization.NumberStyles.Number"/>.</param><param name="provider">An object that supplies culture-specific parsing information about <paramref name="s"/>. </param><param name="result">When this method returns, contains the <see cref="T:System.Decimal"/> number that is equivalent to the numeric value contained in <paramref name="s"/>, if the conversion succeeded, or is zero if the conversion failed. The conversion fails if the <paramref name="s"/> parameter is null, is not in a format compliant with <paramref name="style"/>, or represents a number less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>. This parameter is passed uninitialized. </param><exception cref="T:System.ArgumentException"><paramref name="style"/> is not a <see cref="T:System.Globalization.NumberStyles"/> value. -or-<paramref name="style"/> is the <see cref="F:System.Globalization.NumberStyles.AllowHexSpecifier"/> value.</exception><filterpriority>1</filterpriority>
    [__DynamicallyInvokable]
    public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out Decimal result)
    {
      NumberFormatInfo.ValidateParseStyleFloatingPoint(style);
      return Number.TryParseDecimal(s, style, NumberFormatInfo.GetInstance(provider), out result);
    }

    /// <summary>
    /// Converts the value of a specified instance of <see cref="T:System.Decimal"/> to its equivalent binary representation.
    /// </summary>
    /// 
    /// <returns>
    /// A 32-bit signed integer array with four elements that contain the binary representation of <paramref name="d"/>.
    /// </returns>
    /// <param name="d">The value to convert. </param><filterpriority>1</filterpriority>
    [__DynamicallyInvokable]
    public static int[] GetBits(Decimal d)
    {
      return new int[4]
      {
        d.lo,
        d.mid,
        d.hi,
        d.flags
      };
    }

    internal static void GetBytes(Decimal d, byte[] buffer)
    {
      buffer[0] = (byte) d.lo;
      buffer[1] = (byte) (d.lo >> 8);
      buffer[2] = (byte) (d.lo >> 16);
      buffer[3] = (byte) (d.lo >> 24);
      buffer[4] = (byte) d.mid;
      buffer[5] = (byte) (d.mid >> 8);
      buffer[6] = (byte) (d.mid >> 16);
      buffer[7] = (byte) (d.mid >> 24);
      buffer[8] = (byte) d.hi;
      buffer[9] = (byte) (d.hi >> 8);
      buffer[10] = (byte) (d.hi >> 16);
      buffer[11] = (byte) (d.hi >> 24);
      buffer[12] = (byte) d.flags;
      buffer[13] = (byte) (d.flags >> 8);
      buffer[14] = (byte) (d.flags >> 16);
      buffer[15] = (byte) (d.flags >> 24);
    }

    internal static Decimal ToDecimal(byte[] buffer)
    {
      return new Decimal((int) buffer[0] | (int) buffer[1] << 8 | (int) buffer[2] << 16 | (int) buffer[3] << 24, (int) buffer[4] | (int) buffer[5] << 8 | (int) buffer[6] << 16 | (int) buffer[7] << 24, (int) buffer[8] | (int) buffer[9] << 8 | (int) buffer[10] << 16 | (int) buffer[11] << 24, (int) buffer[12] | (int) buffer[13] << 8 | (int) buffer[14] << 16 | (int) buffer[15] << 24);
    }

    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    [SecuritySafeCritical]
    internal static Decimal Max(Decimal d1, Decimal d2)
    {
      if (Decimal.FCallCompare(ref d1, ref d2) < 0)
        return d2;
      else
        return d1;
    }

    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    [SecuritySafeCritical]
    internal static Decimal Min(Decimal d1, Decimal d2)
    {
      if (Decimal.FCallCompare(ref d1, ref d2) >= 0)
        return d2;
      else
        return d1;
    }

    /// <summary>
    /// Computes the remainder after dividing two <see cref="T:System.Decimal"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// The remainder after dividing <paramref name="d1"/> by <paramref name="d2"/>.
    /// </returns>
    /// <param name="d1">The dividend. </param><param name="d2">The divisor. </param><exception cref="T:System.DivideByZeroException"><paramref name="d2"/> is zero. </exception><exception cref="T:System.OverflowException">The return value is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>. </exception><filterpriority>1</filterpriority>
    [__DynamicallyInvokable]
    public static Decimal Remainder(Decimal d1, Decimal d2)
    {
      d2.flags = d2.flags & int.MaxValue | d1.flags & int.MinValue;
      if (Decimal.Abs(d1) < Decimal.Abs(d2))
        return d1;
      d1 -= d2;
      if (d1 == new Decimal(0))
        d1.flags = d1.flags & int.MaxValue | d2.flags & int.MinValue;
      Decimal num1 = Decimal.Truncate(d1 / d2) * d2;
      Decimal num2 = d1 - num1;
      if ((d1.flags & int.MinValue) != (num2.flags & int.MinValue))
      {
        if (new Decimal(1, 0, 0, true, (byte) 27) <= num2 && num2 <= new Decimal(1, 0, 0, false, (byte) 27))
          num2.flags = num2.flags & int.MaxValue | d1.flags & int.MinValue;
        else
          num2 += d2;
      }
      return num2;
    }

    /// <summary>
    /// Multiplies two specified <see cref="T:System.Decimal"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// The result of multiplying <paramref name="d1"/> and <paramref name="d2"/>.
    /// </returns>
    /// <param name="d1">The multiplicand. </param><param name="d2">The multiplier. </param><exception cref="T:System.OverflowException">The return value is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>. </exception><filterpriority>1</filterpriority>
    [SecuritySafeCritical]
    [__DynamicallyInvokable]
    public static Decimal Multiply(Decimal d1, Decimal d2)
    {
      Decimal.FCallMultiply(ref d1, ref d2);
      return d1;
    }

    /// <summary>
    /// Returns the result of multiplying the specified <see cref="T:System.Decimal"/> value by negative one.
    /// </summary>
    /// 
    /// <returns>
    /// A decimal number with the value of <paramref name="d"/>, but the opposite sign.-or- Zero, if <paramref name="d"/> is zero.
    /// </returns>
    /// <param name="d">The value to negate. </param><filterpriority>1</filterpriority>
    [__DynamicallyInvokable]
    public static Decimal Negate(Decimal d)
    {
      return new Decimal(d.lo, d.mid, d.hi, d.flags ^ int.MinValue);
    }

    /// <summary>
    /// Rounds a decimal value to the nearest integer.
    /// </summary>
    /// 
    /// <returns>
    /// The integer that is nearest to the <paramref name="d"/> parameter. If <paramref name="d"/> is halfway between two integers, one of which is even and the other odd, the even number is returned.
    /// </returns>
    /// <param name="d">A decimal number to round. </param><exception cref="T:System.OverflowException">The result is outside the range of a <see cref="T:System.Decimal"/> object.</exception><filterpriority>1</filterpriority>
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static Decimal Round(Decimal d)
    {
      return Decimal.Round(d, 0);
    }

    /// <summary>
    /// Rounds a <see cref="T:System.Decimal"/> value to a specified number of decimal places.
    /// </summary>
    /// 
    /// <returns>
    /// The decimal number equivalent to <paramref name="d"/> rounded to <paramref name="decimals"/> number of decimal places.
    /// </returns>
    /// <param name="d">A decimal number to round. </param><param name="decimals">A value from 0 to 28 that specifies the number of decimal places to round to. </param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="decimals"/> is not a value from 0 to 28. </exception><filterpriority>1</filterpriority>
    [SecuritySafeCritical]
    [__DynamicallyInvokable]
    public static Decimal Round(Decimal d, int decimals)
    {
      Decimal.FCallRound(ref d, decimals);
      return d;
    }

    /// <summary>
    /// Rounds a decimal value to the nearest integer. A parameter specifies how to round the value if it is midway between two other numbers.
    /// </summary>
    /// 
    /// <returns>
    /// The integer that is nearest to the <paramref name="d"/> parameter. If <paramref name="d"/> is halfway between two numbers, one of which is even and the other odd, the <paramref name="mode"/> parameter determines which of the two numbers is returned.
    /// </returns>
    /// <param name="d">A decimal number to round. </param><param name="mode">A value that specifies how to round <paramref name="d"/> if it is midway between two other numbers.</param><exception cref="T:System.ArgumentException"><paramref name="mode"/> is not a <see cref="T:System.MidpointRounding"/> value.</exception><exception cref="T:System.OverflowException">The result is outside the range of a <see cref="T:System.Decimal"/> object.</exception><filterpriority>1</filterpriority>
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static Decimal Round(Decimal d, MidpointRounding mode)
    {
      return Decimal.Round(d, 0, mode);
    }

    /// <summary>
    /// Rounds a decimal value to a specified precision. A parameter specifies how to round the value if it is midway between two other numbers.
    /// </summary>
    /// 
    /// <returns>
    /// The number that is nearest to the <paramref name="d"/> parameter with a precision equal to the <paramref name="decimals"/> parameter. If <paramref name="d"/> is halfway between two numbers, one of which is even and the other odd, the <paramref name="mode"/> parameter determines which of the two numbers is returned. If the precision of <paramref name="d"/> is less than <paramref name="decimals"/>, <paramref name="d"/> is returned unchanged.
    /// </returns>
    /// <param name="d">A decimal number to round. </param><param name="decimals">The number of significant decimal places (precision) in the return value. </param><param name="mode">A value that specifies how to round <paramref name="d"/> if it is midway between two other numbers.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="decimals"/> is less than 0 or greater than 28. </exception><exception cref="T:System.ArgumentException"><paramref name="mode"/> is not a <see cref="T:System.MidpointRounding"/> value.</exception><exception cref="T:System.OverflowException">The result is outside the range of a <see cref="T:System.Decimal"/> object.</exception><filterpriority>1</filterpriority>
    [SecuritySafeCritical]
    public static Decimal Round(Decimal d, int decimals, MidpointRounding mode)
    {
      if (decimals < 0 || decimals > 28)
        throw new ArgumentOutOfRangeException("decimals", Environment.GetResourceString("ArgumentOutOfRange_DecimalRound"));
      if (mode < MidpointRounding.ToEven || mode > MidpointRounding.AwayFromZero)
      {
        throw new ArgumentException(Environment.GetResourceString("Argument_InvalidEnumValue", (object) mode, (object) "MidpointRounding"), "mode");
      }
      else
      {
        if (mode == MidpointRounding.ToEven)
          Decimal.FCallRound(ref d, decimals);
        else
          Decimal.InternalRoundFromZero(ref d, decimals);
        return d;
      }
    }

    /// <summary>
    /// Subtracts one specified <see cref="T:System.Decimal"/> value from another.
    /// </summary>
    /// 
    /// <returns>
    /// The result of subtracting <paramref name="d2"/> from <paramref name="d1"/>.
    /// </returns>
    /// <param name="d1">The minuend. </param><param name="d2">The subtrahend. </param><exception cref="T:System.OverflowException">The return value is less than <see cref="F:System.Decimal.MinValue"/> or greater than <see cref="F:System.Decimal.MaxValue"/>. </exception><filterpriority>1</filterpriority>
    [SecuritySafeCritical]
    [__DynamicallyInvokable]
    public static Decimal Subtract(Decimal d1, Decimal d2)
    {
      Decimal.FCallAddSub(ref d1, ref d2, (byte) sbyte.MinValue);
      return d1;
    }

    /// <summary>
    /// Converts the value of the specified <see cref="T:System.Decimal"/> to the equivalent 8-bit unsigned integer.
    /// </summary>
    /// 
    /// <returns>
    /// An 8-bit unsigned integer equivalent to <paramref name="value"/>.
    /// </returns>
    /// <param name="value">The decimal number to convert. </param><exception cref="T:System.OverflowException"><paramref name="value"/> is less than <see cref="F:System.Byte.MinValue"/> or greater than <see cref="F:System.Byte.MaxValue"/>. </exception><filterpriority>1</filterpriority>
    [__DynamicallyInvokable]
    public static byte ToByte(Decimal value)
    {
      uint num;
      try
      {
        num = Decimal.ToUInt32(value);
      }
      catch (OverflowException ex)
      {
        throw new OverflowException(Environment.GetResourceString("Overflow_Byte"), (Exception) ex);
      }
      if (num < 0U || num > (uint) byte.MaxValue)
        throw new OverflowException(Environment.GetResourceString("Overflow_Byte"));
      else
        return (byte) num;
    }

    /// <summary>
    /// Converts the value of the specified <see cref="T:System.Decimal"/> to the equivalent 8-bit signed integer.
    /// </summary>
    /// 
    /// <returns>
    /// An 8-bit signed integer equivalent to <paramref name="value"/>.
    /// </returns>
    /// <param name="value">The decimal number to convert. </param><exception cref="T:System.OverflowException"><paramref name="value"/> is less than <see cref="F:System.SByte.MinValue"/> or greater than <see cref="F:System.SByte.MaxValue"/>. </exception><filterpriority>1</filterpriority>
    [CLSCompliant(false)]
    [__DynamicallyInvokable]
    public static sbyte ToSByte(Decimal value)
    {
      int num;
      try
      {
        num = Decimal.ToInt32(value);
      }
      catch (OverflowException ex)
      {
        throw new OverflowException(Environment.GetResourceString("Overflow_SByte"), (Exception) ex);
      }
      if (num < (int) sbyte.MinValue || num > (int) sbyte.MaxValue)
        throw new OverflowException(Environment.GetResourceString("Overflow_SByte"));
      else
        return (sbyte) num;
    }

    /// <summary>
    /// Converts the value of the specified <see cref="T:System.Decimal"/> to the equivalent 16-bit signed integer.
    /// </summary>
    /// 
    /// <returns>
    /// A 16-bit signed integer equivalent to <paramref name="value"/>.
    /// </returns>
    /// <param name="value">The decimal number to convert. </param><exception cref="T:System.OverflowException"><paramref name="value"/> is less than <see cref="F:System.Int16.MinValue"/> or greater than <see cref="F:System.Int16.MaxValue"/>. </exception><filterpriority>1</filterpriority>
    [__DynamicallyInvokable]
    public static short ToInt16(Decimal value)
    {
      int num;
      try
      {
        num = Decimal.ToInt32(value);
      }
      catch (OverflowException ex)
      {
        throw new OverflowException(Environment.GetResourceString("Overflow_Int16"), (Exception) ex);
      }
      if (num < (int) short.MinValue || num > (int) short.MaxValue)
        throw new OverflowException(Environment.GetResourceString("Overflow_Int16"));
      else
        return (short) num;
    }

    [SecuritySafeCritical]
    internal static Currency ToCurrency(Decimal d)
    {
      Currency result = new Currency();
      Decimal.FCallToCurrency(ref result, d);
      return result;
    }

    /// <summary>
    /// Converts the value of the specified <see cref="T:System.Decimal"/> to the equivalent double-precision floating-point number.
    /// </summary>
    /// 
    /// <returns>
    /// A double-precision floating-point number equivalent to <paramref name="d"/>.
    /// </returns>
    /// <param name="d">The decimal number to convert. </param><filterpriority>1</filterpriority>
    [SecuritySafeCritical]
    [__DynamicallyInvokable]
    [MethodImpl(MethodImplOptions.InternalCall)]
    public static double ToDouble(Decimal d);

    [SecurityCritical]
    [MethodImpl(MethodImplOptions.InternalCall)]
    internal static int FCallToInt32(Decimal d);

    /// <summary>
    /// Converts the value of the specified <see cref="T:System.Decimal"/> to the equivalent 32-bit signed integer.
    /// </summary>
    /// 
    /// <returns>
    /// A 32-bit signed integer equivalent to the value of <paramref name="d"/>.
    /// </returns>
    /// <param name="d">The decimal number to convert. </param><exception cref="T:System.OverflowException"><paramref name="d"/> is less than <see cref="F:System.Int32.MinValue"/> or greater than <see cref="F:System.Int32.MaxValue"/>. </exception><filterpriority>1</filterpriority>
    [SecuritySafeCritical]
    [__DynamicallyInvokable]
    public static int ToInt32(Decimal d)
    {
      if ((d.flags & 16711680) != 0)
        Decimal.FCallTruncate(ref d);
      if (d.hi == 0 && d.mid == 0)
      {
        int num1 = d.lo;
        if (d.flags >= 0)
        {
          if (num1 >= 0)
            return num1;
        }
        else
        {
          int num2 = -num1;
          if (num2 <= 0)
            return num2;
        }
      }
      throw new OverflowException(Environment.GetResourceString("Overflow_Int32"));
    }

    /// <summary>
    /// Converts the value of the specified <see cref="T:System.Decimal"/> to the equivalent 64-bit signed integer.
    /// </summary>
    /// 
    /// <returns>
    /// A 64-bit signed integer equivalent to the value of <paramref name="d"/>.
    /// </returns>
    /// <param name="d">The decimal number to convert. </param><exception cref="T:System.OverflowException"><paramref name="d"/> is less than <see cref="F:System.Int64.MinValue"/> or greater than <see cref="F:System.Int64.MaxValue"/>. </exception><filterpriority>1</filterpriority>
    [SecuritySafeCritical]
    [__DynamicallyInvokable]
    public static long ToInt64(Decimal d)
    {
      if ((d.flags & 16711680) != 0)
        Decimal.FCallTruncate(ref d);
      if (d.hi == 0)
      {
        long num1 = (long) d.lo & (long) uint.MaxValue | (long) d.mid << 32;
        if (d.flags >= 0)
        {
          if (num1 >= 0L)
            return num1;
        }
        else
        {
          long num2 = -num1;
          if (num2 <= 0L)
            return num2;
        }
      }
      throw new OverflowException(Environment.GetResourceString("Overflow_Int64"));
    }

    /// <summary>
    /// Converts the value of the specified <see cref="T:System.Decimal"/> to the equivalent 16-bit unsigned integer.
    /// </summary>
    /// 
    /// <returns>
    /// A 16-bit unsigned integer equivalent to the value of <paramref name="value"/>.
    /// </returns>
    /// <param name="value">The decimal number to convert. </param><exception cref="T:System.OverflowException"><paramref name="value"/> is greater than <see cref="F:System.UInt16.MaxValue"/> or less than <see cref="F:System.UInt16.MinValue"/>. </exception><filterpriority>1</filterpriority>
    [CLSCompliant(false)]
    [__DynamicallyInvokable]
    public static ushort ToUInt16(Decimal value)
    {
      uint num;
      try
      {
        num = Decimal.ToUInt32(value);
      }
      catch (OverflowException ex)
      {
        throw new OverflowException(Environment.GetResourceString("Overflow_UInt16"), (Exception) ex);
      }
      if (num < 0U || num > (uint) ushort.MaxValue)
        throw new OverflowException(Environment.GetResourceString("Overflow_UInt16"));
      else
        return (ushort) num;
    }

    /// <summary>
    /// Converts the value of the specified <see cref="T:System.Decimal"/> to the equivalent 32-bit unsigned integer.
    /// </summary>
    /// 
    /// <returns>
    /// A 32-bit unsigned integer equivalent to the value of <paramref name="d"/>.
    /// </returns>
    /// <param name="d">The decimal number to convert. </param><exception cref="T:System.OverflowException"><paramref name="d"/> is negative or greater than <see cref="F:System.UInt32.MaxValue"/>. </exception><filterpriority>1</filterpriority>
    [SecuritySafeCritical]
    [CLSCompliant(false)]
    [__DynamicallyInvokable]
    public static uint ToUInt32(Decimal d)
    {
      if ((d.flags & 16711680) != 0)
        Decimal.FCallTruncate(ref d);
      if (d.hi == 0 && d.mid == 0)
      {
        uint num = (uint) d.lo;
        if (d.flags >= 0 || (int) num == 0)
          return num;
      }
      throw new OverflowException(Environment.GetResourceString("Overflow_UInt32"));
    }

    /// <summary>
    /// Converts the value of the specified <see cref="T:System.Decimal"/> to the equivalent 64-bit unsigned integer.
    /// </summary>
    /// 
    /// <returns>
    /// A 64-bit unsigned integer equivalent to the value of <paramref name="d"/>.
    /// </returns>
    /// <param name="d">The decimal number to convert. </param><exception cref="T:System.OverflowException"><paramref name="d"/> is negative or greater than <see cref="F:System.UInt64.MaxValue"/>. </exception><filterpriority>1</filterpriority>
    [SecuritySafeCritical]
    [CLSCompliant(false)]
    [__DynamicallyInvokable]
    public static ulong ToUInt64(Decimal d)
    {
      if ((d.flags & 16711680) != 0)
        Decimal.FCallTruncate(ref d);
      if (d.hi == 0)
      {
        ulong num = (ulong) (uint) d.lo | (ulong) (uint) d.mid << 32;
        if (d.flags >= 0 || (long) num == 0L)
          return num;
      }
      throw new OverflowException(Environment.GetResourceString("Overflow_UInt64"));
    }

    /// <summary>
    /// Converts the value of the specified <see cref="T:System.Decimal"/> to the equivalent single-precision floating-point number.
    /// </summary>
    /// 
    /// <returns>
    /// A single-precision floating-point number equivalent to the value of <paramref name="d"/>.
    /// </returns>
    /// <param name="d">The decimal number to convert. </param><filterpriority>1</filterpriority>
    [SecuritySafeCritical]
    [__DynamicallyInvokable]
    [MethodImpl(MethodImplOptions.InternalCall)]
    public static float ToSingle(Decimal d);

    /// <summary>
    /// Returns the integral digits of the specified <see cref="T:System.Decimal"/>; any fractional digits are discarded.
    /// </summary>
    /// 
    /// <returns>
    /// The result of <paramref name="d"/> rounded toward zero, to the nearest whole number.
    /// </returns>
    /// <param name="d">The decimal number to truncate. </param><filterpriority>1</filterpriority>
    [SecuritySafeCritical]
    [__DynamicallyInvokable]
    public static Decimal Truncate(Decimal d)
    {
      Decimal.FCallTruncate(ref d);
      return d;
    }

    /// <summary>
    /// Returns the <see cref="T:System.TypeCode"/> for value type <see cref="T:System.Decimal"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The enumerated constant <see cref="F:System.TypeCode.Decimal"/>.
    /// </returns>
    /// <filterpriority>2</filterpriority>
    public TypeCode GetTypeCode()
    {
      return TypeCode.Decimal;
    }

    bool IConvertible.ToBoolean(IFormatProvider provider)
    {
      return Convert.ToBoolean(this);
    }

    char IConvertible.ToChar(IFormatProvider provider)
    {
      throw new InvalidCastException(Environment.GetResourceString("InvalidCast_FromTo", (object) "Decimal", (object) "Char"));
    }

    sbyte IConvertible.ToSByte(IFormatProvider provider)
    {
      return Convert.ToSByte(this);
    }

    byte IConvertible.ToByte(IFormatProvider provider)
    {
      return Convert.ToByte(this);
    }

    short IConvertible.ToInt16(IFormatProvider provider)
    {
      return Convert.ToInt16(this);
    }

    ushort IConvertible.ToUInt16(IFormatProvider provider)
    {
      return Convert.ToUInt16(this);
    }

    int IConvertible.ToInt32(IFormatProvider provider)
    {
      return Convert.ToInt32(this);
    }

    uint IConvertible.ToUInt32(IFormatProvider provider)
    {
      return Convert.ToUInt32(this);
    }

    long IConvertible.ToInt64(IFormatProvider provider)
    {
      return Convert.ToInt64(this);
    }

    ulong IConvertible.ToUInt64(IFormatProvider provider)
    {
      return Convert.ToUInt64(this);
    }

    float IConvertible.ToSingle(IFormatProvider provider)
    {
      return Convert.ToSingle(this);
    }

    double IConvertible.ToDouble(IFormatProvider provider)
    {
      return Convert.ToDouble(this);
    }

    Decimal IConvertible.ToDecimal(IFormatProvider provider)
    {
      return this;
    }

    DateTime IConvertible.ToDateTime(IFormatProvider provider)
    {
      throw new InvalidCastException(Environment.GetResourceString("InvalidCast_FromTo", (object) "Decimal", (object) "DateTime"));
    }

    object IConvertible.ToType(Type type, IFormatProvider provider)
    {
      return Convert.DefaultToType((IConvertible) this, type, provider);
    }

    private void SetBits(int[] bits)
    {
      if (bits == null)
        throw new ArgumentNullException("bits");
      if (bits.Length == 4)
      {
        int num = bits[3];
        if ((num & 2130771967) == 0 && (num & 16711680) <= 1835008)
        {
          this.lo = bits[0];
          this.mid = bits[1];
          this.hi = bits[2];
          this.flags = num;
          return;
        }
      }
      throw new ArgumentException(Environment.GetResourceString("Arg_DecBitCtor"));
    }

    [OnSerializing]
    private void OnSerializing(StreamingContext ctx)
    {
      try
      {
        this.SetBits(Decimal.GetBits(this));
      }
      catch (ArgumentException ex)
      {
        throw new SerializationException(Environment.GetResourceString("Overflow_Decimal"), (Exception) ex);
      }
    }

    [SecurityCritical]
    [MethodImpl(MethodImplOptions.InternalCall)]
    private static void FCallAddSub(ref Decimal d1, ref Decimal d2, byte bSign);

    [SecurityCritical]
    [MethodImpl(MethodImplOptions.InternalCall)]
    private static void FCallAddSubOverflowed(ref Decimal d1, ref Decimal d2, byte bSign, ref bool overflowed);

    [SecurityCritical]
    [MethodImpl(MethodImplOptions.InternalCall)]
    private static void FCallDivide(ref Decimal d1, ref Decimal d2);

    [SecurityCritical]
    [MethodImpl(MethodImplOptions.InternalCall)]
    private static void FCallDivideOverflowed(ref Decimal d1, ref Decimal d2, ref bool overflowed);

    [SecurityCritical]
    [MethodImpl(MethodImplOptions.InternalCall)]
    private static void FCallFloor(ref Decimal d);

    private static void InternalAddUInt32RawUnchecked(ref Decimal value, uint i)
    {
      uint num1 = (uint) value.lo;
      uint num2 = num1 + i;
      value.lo = (int) num2;
      if (num2 >= num1 && num2 >= i)
        return;
      uint num3 = (uint) value.mid;
      uint num4 = num3 + 1U;
      value.mid = (int) num4;
      if (num4 >= num3 && num4 >= 1U)
        return;
      value.hi = value.hi + 1;
    }

    private static uint InternalDivRemUInt32(ref Decimal value, uint divisor)
    {
      uint num1 = 0U;
      if (value.hi != 0)
      {
        ulong num2 = (ulong) (uint) value.hi;
        value.hi = (int) (uint) (num2 / (ulong) divisor);
        num1 = (uint) (num2 % (ulong) divisor);
      }
      if (value.mid != 0 || (int) num1 != 0)
      {
        ulong num2 = (ulong) num1 << 32 | (ulong) (uint) value.mid;
        value.mid = (int) (uint) (num2 / (ulong) divisor);
        num1 = (uint) (num2 % (ulong) divisor);
      }
      if (value.lo != 0 || (int) num1 != 0)
      {
        ulong num2 = (ulong) num1 << 32 | (ulong) (uint) value.lo;
        value.lo = (int) (uint) (num2 / (ulong) divisor);
        num1 = (uint) (num2 % (ulong) divisor);
      }
      return num1;
    }

    private static void InternalRoundFromZero(ref Decimal d, int decimalCount)
    {
      int num1 = ((d.flags & 16711680) >> 16) - decimalCount;
      if (num1 <= 0)
        return;
      uint divisor;
      uint num2;
      do
      {
        int index = num1 > 9 ? 9 : num1;
        divisor = Decimal.Powers10[index];
        num2 = Decimal.InternalDivRemUInt32(ref d, divisor);
        num1 -= index;
      }
      while (num1 > 0);
      if (num2 >= divisor >> 1)
        Decimal.InternalAddUInt32RawUnchecked(ref d, 1U);
      d.flags = decimalCount << 16 & 16711680 | d.flags & int.MinValue;
    }

    [SecurityCritical]
    [MethodImpl(MethodImplOptions.InternalCall)]
    private static void FCallMultiply(ref Decimal d1, ref Decimal d2);

    [SecurityCritical]
    [MethodImpl(MethodImplOptions.InternalCall)]
    private static void FCallMultiplyOverflowed(ref Decimal d1, ref Decimal d2, ref bool overflowed);

    [SecurityCritical]
    [MethodImpl(MethodImplOptions.InternalCall)]
    private static void FCallRound(ref Decimal d, int decimals);

    [SecurityCritical]
    [MethodImpl(MethodImplOptions.InternalCall)]
    private static void FCallToCurrency(ref Currency result, Decimal d);

    [SecurityCritical]
    [MethodImpl(MethodImplOptions.InternalCall)]
    private static void FCallTruncate(ref Decimal d);
  }
}
