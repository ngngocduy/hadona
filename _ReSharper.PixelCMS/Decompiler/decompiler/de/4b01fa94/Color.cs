// Type: System.Drawing.Color
// Assembly: System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// Assembly location: C:\Windows\Microsoft.NET\Framework\v2.0.50727\System.Drawing.dll

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.Globalization;
using System.Text;

namespace System.Drawing
{
  /// <summary>
  /// Represents an ARGB (alpha, red, green, blue) color.
  /// 
  /// </summary>
  /// <filterpriority>1</filterpriority><completionlist cref="T:System.Drawing.Color"/>
  [TypeConverter(typeof (ColorConverter))]
  [DebuggerDisplay("{NameAndARGBValue}")]
  [Editor("System.Drawing.Design.ColorEditor, System.Drawing.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof (UITypeEditor))]
  [Serializable]
  public struct Color
  {
    /// <summary>
    /// Represents a color that is null.
    /// 
    /// </summary>
    /// <filterpriority>1</filterpriority>
    public static readonly Color Empty = new Color();
    private static short StateKnownColorValid = (short) 1;
    private static short StateARGBValueValid = (short) 2;
    private static short StateValueMask = Color.StateARGBValueValid;
    private static short StateNameValid = (short) 8;
    private static long NotDefinedValue = 0L;
    private const int ARGBAlphaShift = 24;
    private const int ARGBRedShift = 16;
    private const int ARGBGreenShift = 8;
    private const int ARGBBlueShift = 0;
    private readonly string name;
    private readonly long value;
    private readonly short knownColor;
    private readonly short state;

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Transparent
    {
      get
      {
        return new Color(KnownColor.Transparent);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color AliceBlue
    {
      get
      {
        return new Color(KnownColor.AliceBlue);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color AntiqueWhite
    {
      get
      {
        return new Color(KnownColor.AntiqueWhite);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Aqua
    {
      get
      {
        return new Color(KnownColor.Aqua);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Aquamarine
    {
      get
      {
        return new Color(KnownColor.Aquamarine);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Azure
    {
      get
      {
        return new Color(KnownColor.Azure);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Beige
    {
      get
      {
        return new Color(KnownColor.Beige);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Bisque
    {
      get
      {
        return new Color(KnownColor.Bisque);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Black
    {
      get
      {
        return new Color(KnownColor.Black);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color BlanchedAlmond
    {
      get
      {
        return new Color(KnownColor.BlanchedAlmond);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Blue
    {
      get
      {
        return new Color(KnownColor.Blue);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color BlueViolet
    {
      get
      {
        return new Color(KnownColor.BlueViolet);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Brown
    {
      get
      {
        return new Color(KnownColor.Brown);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color BurlyWood
    {
      get
      {
        return new Color(KnownColor.BurlyWood);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color CadetBlue
    {
      get
      {
        return new Color(KnownColor.CadetBlue);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Chartreuse
    {
      get
      {
        return new Color(KnownColor.Chartreuse);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Chocolate
    {
      get
      {
        return new Color(KnownColor.Chocolate);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Coral
    {
      get
      {
        return new Color(KnownColor.Coral);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color CornflowerBlue
    {
      get
      {
        return new Color(KnownColor.CornflowerBlue);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Cornsilk
    {
      get
      {
        return new Color(KnownColor.Cornsilk);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Crimson
    {
      get
      {
        return new Color(KnownColor.Crimson);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Cyan
    {
      get
      {
        return new Color(KnownColor.Cyan);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color DarkBlue
    {
      get
      {
        return new Color(KnownColor.DarkBlue);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color DarkCyan
    {
      get
      {
        return new Color(KnownColor.DarkCyan);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color DarkGoldenrod
    {
      get
      {
        return new Color(KnownColor.DarkGoldenrod);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color DarkGray
    {
      get
      {
        return new Color(KnownColor.DarkGray);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color DarkGreen
    {
      get
      {
        return new Color(KnownColor.DarkGreen);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color DarkKhaki
    {
      get
      {
        return new Color(KnownColor.DarkKhaki);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color DarkMagenta
    {
      get
      {
        return new Color(KnownColor.DarkMagenta);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color DarkOliveGreen
    {
      get
      {
        return new Color(KnownColor.DarkOliveGreen);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color DarkOrange
    {
      get
      {
        return new Color(KnownColor.DarkOrange);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color DarkOrchid
    {
      get
      {
        return new Color(KnownColor.DarkOrchid);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color DarkRed
    {
      get
      {
        return new Color(KnownColor.DarkRed);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color DarkSalmon
    {
      get
      {
        return new Color(KnownColor.DarkSalmon);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color DarkSeaGreen
    {
      get
      {
        return new Color(KnownColor.DarkSeaGreen);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color DarkSlateBlue
    {
      get
      {
        return new Color(KnownColor.DarkSlateBlue);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color DarkSlateGray
    {
      get
      {
        return new Color(KnownColor.DarkSlateGray);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color DarkTurquoise
    {
      get
      {
        return new Color(KnownColor.DarkTurquoise);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color DarkViolet
    {
      get
      {
        return new Color(KnownColor.DarkViolet);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color DeepPink
    {
      get
      {
        return new Color(KnownColor.DeepPink);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color DeepSkyBlue
    {
      get
      {
        return new Color(KnownColor.DeepSkyBlue);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color DimGray
    {
      get
      {
        return new Color(KnownColor.DimGray);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color DodgerBlue
    {
      get
      {
        return new Color(KnownColor.DodgerBlue);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Firebrick
    {
      get
      {
        return new Color(KnownColor.Firebrick);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color FloralWhite
    {
      get
      {
        return new Color(KnownColor.FloralWhite);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color ForestGreen
    {
      get
      {
        return new Color(KnownColor.ForestGreen);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Fuchsia
    {
      get
      {
        return new Color(KnownColor.Fuchsia);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Gainsboro
    {
      get
      {
        return new Color(KnownColor.Gainsboro);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color GhostWhite
    {
      get
      {
        return new Color(KnownColor.GhostWhite);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Gold
    {
      get
      {
        return new Color(KnownColor.Gold);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Goldenrod
    {
      get
      {
        return new Color(KnownColor.Goldenrod);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> strcture representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Gray
    {
      get
      {
        return new Color(KnownColor.Gray);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Green
    {
      get
      {
        return new Color(KnownColor.Green);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color GreenYellow
    {
      get
      {
        return new Color(KnownColor.GreenYellow);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Honeydew
    {
      get
      {
        return new Color(KnownColor.Honeydew);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color HotPink
    {
      get
      {
        return new Color(KnownColor.HotPink);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color IndianRed
    {
      get
      {
        return new Color(KnownColor.IndianRed);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Indigo
    {
      get
      {
        return new Color(KnownColor.Indigo);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Ivory
    {
      get
      {
        return new Color(KnownColor.Ivory);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Khaki
    {
      get
      {
        return new Color(KnownColor.Khaki);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Lavender
    {
      get
      {
        return new Color(KnownColor.Lavender);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color LavenderBlush
    {
      get
      {
        return new Color(KnownColor.LavenderBlush);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color LawnGreen
    {
      get
      {
        return new Color(KnownColor.LawnGreen);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color LemonChiffon
    {
      get
      {
        return new Color(KnownColor.LemonChiffon);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color LightBlue
    {
      get
      {
        return new Color(KnownColor.LightBlue);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color LightCoral
    {
      get
      {
        return new Color(KnownColor.LightCoral);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color LightCyan
    {
      get
      {
        return new Color(KnownColor.LightCyan);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color LightGoldenrodYellow
    {
      get
      {
        return new Color(KnownColor.LightGoldenrodYellow);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color LightGreen
    {
      get
      {
        return new Color(KnownColor.LightGreen);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color LightGray
    {
      get
      {
        return new Color(KnownColor.LightGray);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color LightPink
    {
      get
      {
        return new Color(KnownColor.LightPink);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color LightSalmon
    {
      get
      {
        return new Color(KnownColor.LightSalmon);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color LightSeaGreen
    {
      get
      {
        return new Color(KnownColor.LightSeaGreen);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color LightSkyBlue
    {
      get
      {
        return new Color(KnownColor.LightSkyBlue);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color LightSlateGray
    {
      get
      {
        return new Color(KnownColor.LightSlateGray);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color LightSteelBlue
    {
      get
      {
        return new Color(KnownColor.LightSteelBlue);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color LightYellow
    {
      get
      {
        return new Color(KnownColor.LightYellow);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Lime
    {
      get
      {
        return new Color(KnownColor.Lime);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color LimeGreen
    {
      get
      {
        return new Color(KnownColor.LimeGreen);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Linen
    {
      get
      {
        return new Color(KnownColor.Linen);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Magenta
    {
      get
      {
        return new Color(KnownColor.Magenta);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Maroon
    {
      get
      {
        return new Color(KnownColor.Maroon);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color MediumAquamarine
    {
      get
      {
        return new Color(KnownColor.MediumAquamarine);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color MediumBlue
    {
      get
      {
        return new Color(KnownColor.MediumBlue);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color MediumOrchid
    {
      get
      {
        return new Color(KnownColor.MediumOrchid);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color MediumPurple
    {
      get
      {
        return new Color(KnownColor.MediumPurple);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color MediumSeaGreen
    {
      get
      {
        return new Color(KnownColor.MediumSeaGreen);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color MediumSlateBlue
    {
      get
      {
        return new Color(KnownColor.MediumSlateBlue);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color MediumSpringGreen
    {
      get
      {
        return new Color(KnownColor.MediumSpringGreen);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color MediumTurquoise
    {
      get
      {
        return new Color(KnownColor.MediumTurquoise);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color MediumVioletRed
    {
      get
      {
        return new Color(KnownColor.MediumVioletRed);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color MidnightBlue
    {
      get
      {
        return new Color(KnownColor.MidnightBlue);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color MintCream
    {
      get
      {
        return new Color(KnownColor.MintCream);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color MistyRose
    {
      get
      {
        return new Color(KnownColor.MistyRose);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Moccasin
    {
      get
      {
        return new Color(KnownColor.Moccasin);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color NavajoWhite
    {
      get
      {
        return new Color(KnownColor.NavajoWhite);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Navy
    {
      get
      {
        return new Color(KnownColor.Navy);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color OldLace
    {
      get
      {
        return new Color(KnownColor.OldLace);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Olive
    {
      get
      {
        return new Color(KnownColor.Olive);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color OliveDrab
    {
      get
      {
        return new Color(KnownColor.OliveDrab);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Orange
    {
      get
      {
        return new Color(KnownColor.Orange);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color OrangeRed
    {
      get
      {
        return new Color(KnownColor.OrangeRed);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Orchid
    {
      get
      {
        return new Color(KnownColor.Orchid);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color PaleGoldenrod
    {
      get
      {
        return new Color(KnownColor.PaleGoldenrod);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color PaleGreen
    {
      get
      {
        return new Color(KnownColor.PaleGreen);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color PaleTurquoise
    {
      get
      {
        return new Color(KnownColor.PaleTurquoise);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color PaleVioletRed
    {
      get
      {
        return new Color(KnownColor.PaleVioletRed);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color PapayaWhip
    {
      get
      {
        return new Color(KnownColor.PapayaWhip);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color PeachPuff
    {
      get
      {
        return new Color(KnownColor.PeachPuff);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Peru
    {
      get
      {
        return new Color(KnownColor.Peru);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Pink
    {
      get
      {
        return new Color(KnownColor.Pink);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Plum
    {
      get
      {
        return new Color(KnownColor.Plum);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color PowderBlue
    {
      get
      {
        return new Color(KnownColor.PowderBlue);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Purple
    {
      get
      {
        return new Color(KnownColor.Purple);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Red
    {
      get
      {
        return new Color(KnownColor.Red);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color RosyBrown
    {
      get
      {
        return new Color(KnownColor.RosyBrown);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color RoyalBlue
    {
      get
      {
        return new Color(KnownColor.RoyalBlue);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color SaddleBrown
    {
      get
      {
        return new Color(KnownColor.SaddleBrown);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Salmon
    {
      get
      {
        return new Color(KnownColor.Salmon);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color SandyBrown
    {
      get
      {
        return new Color(KnownColor.SandyBrown);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color SeaGreen
    {
      get
      {
        return new Color(KnownColor.SeaGreen);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color SeaShell
    {
      get
      {
        return new Color(KnownColor.SeaShell);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Sienna
    {
      get
      {
        return new Color(KnownColor.Sienna);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Silver
    {
      get
      {
        return new Color(KnownColor.Silver);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color SkyBlue
    {
      get
      {
        return new Color(KnownColor.SkyBlue);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color SlateBlue
    {
      get
      {
        return new Color(KnownColor.SlateBlue);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color SlateGray
    {
      get
      {
        return new Color(KnownColor.SlateGray);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Snow
    {
      get
      {
        return new Color(KnownColor.Snow);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color SpringGreen
    {
      get
      {
        return new Color(KnownColor.SpringGreen);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color SteelBlue
    {
      get
      {
        return new Color(KnownColor.SteelBlue);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Tan
    {
      get
      {
        return new Color(KnownColor.Tan);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Teal
    {
      get
      {
        return new Color(KnownColor.Teal);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Thistle
    {
      get
      {
        return new Color(KnownColor.Thistle);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Tomato
    {
      get
      {
        return new Color(KnownColor.Tomato);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Turquoise
    {
      get
      {
        return new Color(KnownColor.Turquoise);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Violet
    {
      get
      {
        return new Color(KnownColor.Violet);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Wheat
    {
      get
      {
        return new Color(KnownColor.Wheat);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color White
    {
      get
      {
        return new Color(KnownColor.White);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color WhiteSmoke
    {
      get
      {
        return new Color(KnownColor.WhiteSmoke);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color Yellow
    {
      get
      {
        return new Color(KnownColor.Yellow);
      }
    }

    /// <summary>
    /// Gets a system-defined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Drawing.Color"/> representing a system-defined color.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public static Color YellowGreen
    {
      get
      {
        return new Color(KnownColor.YellowGreen);
      }
    }

    /// <summary>
    /// Gets the red component value of this <see cref="T:System.Drawing.Color"/> structure.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// The red component value of this <see cref="T:System.Drawing.Color"/>.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
    public byte R
    {
      get
      {
        return (byte) ((ulong) (this.Value >> 16) & (ulong) byte.MaxValue);
      }
    }

    /// <summary>
    /// Gets the green component value of this <see cref="T:System.Drawing.Color"/> structure.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// The green component value of this <see cref="T:System.Drawing.Color"/>.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
    public byte G
    {
      get
      {
        return (byte) ((ulong) (this.Value >> 8) & (ulong) byte.MaxValue);
      }
    }

    /// <summary>
    /// Gets the blue component value of this <see cref="T:System.Drawing.Color"/> structure.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// The blue component value of this <see cref="T:System.Drawing.Color"/>.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
    public byte B
    {
      get
      {
        return (byte) ((ulong) this.Value & (ulong) byte.MaxValue);
      }
    }

    /// <summary>
    /// Gets the alpha component value of this <see cref="T:System.Drawing.Color"/> structure.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// The alpha component value of this <see cref="T:System.Drawing.Color"/>.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
    public byte A
    {
      get
      {
        return (byte) ((ulong) (this.Value >> 24) & (ulong) byte.MaxValue);
      }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:System.Drawing.Color"/> structure is a predefined color. Predefined colors are represented by the elements of the <see cref="T:System.Drawing.KnownColor"/> enumeration.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// true if this <see cref="T:System.Drawing.Color"/> was created from a predefined color by using either the <see cref="M:System.Drawing.Color.FromName(System.String)"/> method or the <see cref="M:System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor)"/> method; otherwise, false.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public bool IsKnownColor
    {
      get
      {
        return ((int) this.state & (int) Color.StateKnownColorValid) != 0;
      }
    }

    /// <summary>
    /// Specifies whether this <see cref="T:System.Drawing.Color"/> structure is uninitialized.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// This property returns true if this color is uninitialized; otherwise, false.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public bool IsEmpty
    {
      get
      {
        return (int) this.state == 0;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:System.Drawing.Color"/> structure is a named color or a member of the <see cref="T:System.Drawing.KnownColor"/> enumeration.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// true if this <see cref="T:System.Drawing.Color"/> was created by using either the <see cref="M:System.Drawing.Color.FromName(System.String)"/> method or the <see cref="M:System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor)"/> method; otherwise, false.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public bool IsNamedColor
    {
      get
      {
        if (((int) this.state & (int) Color.StateNameValid) == 0)
          return this.IsKnownColor;
        else
          return true;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:System.Drawing.Color"/> structure is a system color. A system color is a color that is used in a Windows display element. System colors are represented by elements of the <see cref="T:System.Drawing.KnownColor"/> enumeration.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// true if this <see cref="T:System.Drawing.Color"/> was created from a system color by using either the <see cref="M:System.Drawing.Color.FromName(System.String)"/> method or the <see cref="M:System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor)"/> method; otherwise, false.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public bool IsSystemColor
    {
      get
      {
        if (!this.IsKnownColor)
          return false;
        if ((int) this.knownColor > 26)
          return (int) this.knownColor > 167;
        else
          return true;
      }
    }

    string NameAndARGBValue
    {
      private get
      {
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{{Name={0}, ARGB=({1}, {2}, {3}, {4})}}", (object) this.Name, (object) this.A, (object) this.R, (object) this.G, (object) this.B);
      }
    }

    /// <summary>
    /// Gets the name of this <see cref="T:System.Drawing.Color"/>.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// The name of this <see cref="T:System.Drawing.Color"/>.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode"/></PermissionSet>
    public string Name
    {
      get
      {
        if (((int) this.state & (int) Color.StateNameValid) != 0)
          return this.name;
        if (this.IsKnownColor)
          return KnownColorTable.KnownColorToName((KnownColor) this.knownColor) ?? ((object) (KnownColor) this.knownColor).ToString();
        else
          return Convert.ToString(this.value, 16);
      }
    }

    long Value
    {
      private get
      {
        if (((int) this.state & (int) Color.StateValueMask) != 0)
          return this.value;
        if (this.IsKnownColor)
          return (long) KnownColorTable.KnownColorToArgb((KnownColor) this.knownColor);
        else
          return Color.NotDefinedValue;
      }
    }

    static Color()
    {
    }

    internal Color(KnownColor knownColor)
    {
      this.value = 0L;
      this.state = Color.StateKnownColorValid;
      this.name = (string) null;
      this.knownColor = (short) knownColor;
    }

    private Color(long value, short state, string name, KnownColor knownColor)
    {
      this.value = value;
      this.state = state;
      this.name = name;
      this.knownColor = (short) knownColor;
    }

    /// <summary>
    /// Tests whether two specified <see cref="T:System.Drawing.Color"/> structures are equivalent.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// true if the two <see cref="T:System.Drawing.Color"/> structures are equal; otherwise, false.
    /// 
    /// </returns>
    /// <param name="left">The <see cref="T:System.Drawing.Color"/> that is to the left of the equality operator.
    ///                 </param><param name="right">The <see cref="T:System.Drawing.Color"/> that is to the right of the equality operator.
    ///                 </param><filterpriority>3</filterpriority>
    public static bool operator ==(Color left, Color right)
    {
      if (left.value != right.value || (int) left.state != (int) right.state || (int) left.knownColor != (int) right.knownColor)
        return false;
      if (left.name == right.name)
        return true;
      if (left.name == null || right.name == null)
        return false;
      else
        return left.name.Equals(right.name);
    }

    /// <summary>
    /// Tests whether two specified <see cref="T:System.Drawing.Color"/> structures are different.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// true if the two <see cref="T:System.Drawing.Color"/> structures are different; otherwise, false.
    /// 
    /// </returns>
    /// <param name="left">The <see cref="T:System.Drawing.Color"/> that is to the left of the inequality operator.
    ///                 </param><param name="right">The <see cref="T:System.Drawing.Color"/> that is to the right of the inequality operator.
    ///                 </param><filterpriority>3</filterpriority>
    public static bool operator !=(Color left, Color right)
    {
      return !(left == right);
    }

    private static void CheckByte(int value, string name)
    {
      if (value >= 0 && value <= (int) byte.MaxValue)
        return;
      throw new ArgumentException(System.Drawing.SR.GetString("InvalidEx2BoundArgument", (object) name, (object) value, (object) 0, (object) (int) byte.MaxValue));
    }

    private static long MakeArgb(byte alpha, byte red, byte green, byte blue)
    {
      return (long) (uint) ((int) red << 16 | (int) green << 8 | (int) blue | (int) alpha << 24) & (long) uint.MaxValue;
    }

    /// <summary>
    /// Creates a <see cref="T:System.Drawing.Color"/> structure from a 32-bit ARGB value.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// The <see cref="T:System.Drawing.Color"/> structure that this method creates.
    /// 
    /// </returns>
    /// <param name="argb">A value specifying the 32-bit ARGB value.
    ///                 </param><filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
    public static Color FromArgb(int argb)
    {
      return new Color((long) argb & (long) uint.MaxValue, Color.StateARGBValueValid, (string) null, (KnownColor) 0);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Drawing.Color"/> structure from the four ARGB component (alpha, red, green, and blue) values. Although this method allows a 32-bit value to be passed for each component, the value of each component is limited to 8 bits.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// The <see cref="T:System.Drawing.Color"/> that this method creates.
    /// 
    /// </returns>
    /// <param name="alpha">The alpha component. Valid values are 0 through 255.
    ///                 </param><param name="red">The red component. Valid values are 0 through 255.
    ///                 </param><param name="green">The green component. Valid values are 0 through 255.
    ///                 </param><param name="blue">The blue component. Valid values are 0 through 255.
    ///                 </param><exception cref="T:System.ArgumentException"><paramref name="alpha"/>, <paramref name="red"/>, <paramref name="green"/>, or <paramref name="blue"/> is less than 0 or greater than 255.
    ///                 </exception><filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
    public static Color FromArgb(int alpha, int red, int green, int blue)
    {
      Color.CheckByte(alpha, "alpha");
      Color.CheckByte(red, "red");
      Color.CheckByte(green, "green");
      Color.CheckByte(blue, "blue");
      return new Color(Color.MakeArgb((byte) alpha, (byte) red, (byte) green, (byte) blue), Color.StateARGBValueValid, (string) null, (KnownColor) 0);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Drawing.Color"/> structure from the specified <see cref="T:System.Drawing.Color"/> structure, but with the new specified alpha value. Although this method allows a 32-bit value to be passed for the alpha value, the value is limited to 8 bits.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// The <see cref="T:System.Drawing.Color"/> that this method creates.
    /// 
    /// </returns>
    /// <param name="alpha">The alpha value for the new <see cref="T:System.Drawing.Color"/>. Valid values are 0 through 255.
    ///                 </param><param name="baseColor">The <see cref="T:System.Drawing.Color"/> from which to create the new <see cref="T:System.Drawing.Color"/>.
    ///                 </param><exception cref="T:System.ArgumentException"><paramref name="alpha"/> is less than 0 or greater than 255.
    ///                 </exception><filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
    public static Color FromArgb(int alpha, Color baseColor)
    {
      Color.CheckByte(alpha, "alpha");
      return new Color(Color.MakeArgb((byte) alpha, baseColor.R, baseColor.G, baseColor.B), Color.StateARGBValueValid, (string) null, (KnownColor) 0);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Drawing.Color"/> structure from the specified 8-bit color values (red, green, and blue). The alpha value is implicitly 255 (fully opaque). Although this method allows a 32-bit value to be passed for each color component, the value of each component is limited to 8 bits.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// The <see cref="T:System.Drawing.Color"/> that this method creates.
    /// 
    /// </returns>
    /// <param name="red">The red component value for the new <see cref="T:System.Drawing.Color"/>. Valid values are 0 through 255.
    ///                 </param><param name="green">The green component value for the new <see cref="T:System.Drawing.Color"/>. Valid values are 0 through 255.
    ///                 </param><param name="blue">The blue component value for the new <see cref="T:System.Drawing.Color"/>. Valid values are 0 through 255.
    ///                 </param><exception cref="T:System.ArgumentException"><paramref name="red"/>, <paramref name="green"/>, or <paramref name="blue"/> is less than 0 or greater than 255.
    ///                 </exception><filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
    public static Color FromArgb(int red, int green, int blue)
    {
      return Color.FromArgb((int) byte.MaxValue, red, green, blue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Drawing.Color"/> structure from the specified predefined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// The <see cref="T:System.Drawing.Color"/> that this method creates.
    /// 
    /// </returns>
    /// <param name="color">An element of the <see cref="T:System.Drawing.KnownColor"/> enumeration.
    ///                 </param><filterpriority>1</filterpriority>
    public static Color FromKnownColor(KnownColor color)
    {
      if (!System.Drawing.ClientUtils.IsEnumValid((Enum) color, (int) color, 1, 174))
        return Color.FromName(((object) color).ToString());
      else
        return new Color(color);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Drawing.Color"/> structure from the specified name of a predefined color.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// The <see cref="T:System.Drawing.Color"/> that this method creates.
    /// 
    /// </returns>
    /// <param name="name">A string that is the name of a predefined color. Valid names are the same as the names of the elements of the <see cref="T:System.Drawing.KnownColor"/> enumeration.
    ///                 </param><filterpriority>1</filterpriority>
    public static Color FromName(string name)
    {
      object namedColor = ColorConverter.GetNamedColor(name);
      if (namedColor != null)
        return (Color) namedColor;
      else
        return new Color(Color.NotDefinedValue, Color.StateNameValid, name, (KnownColor) 0);
    }

    /// <summary>
    /// Gets the hue-saturation-brightness (HSB) brightness value for this <see cref="T:System.Drawing.Color"/> structure.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// The brightness of this <see cref="T:System.Drawing.Color"/>. The brightness ranges from 0.0 through 1.0, where 0.0 represents black and 1.0 represents white.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
    public float GetBrightness()
    {
      float num1 = (float) this.R / (float) byte.MaxValue;
      float num2 = (float) this.G / (float) byte.MaxValue;
      float num3 = (float) this.B / (float) byte.MaxValue;
      float num4 = num1;
      float num5 = num1;
      if ((double) num2 > (double) num4)
        num4 = num2;
      if ((double) num3 > (double) num4)
        num4 = num3;
      if ((double) num2 < (double) num5)
        num5 = num2;
      if ((double) num3 < (double) num5)
        num5 = num3;
      return (float) (((double) num4 + (double) num5) / 2.0);
    }

    /// <summary>
    /// Gets the hue-saturation-brightness (HSB) hue value, in degrees, for this <see cref="T:System.Drawing.Color"/> structure.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// The hue, in degrees, of this <see cref="T:System.Drawing.Color"/>. The hue is measured in degrees, ranging from 0.0 through 360.0, in HSB color space.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
    public float GetHue()
    {
      if ((int) this.R == (int) this.G && (int) this.G == (int) this.B)
        return 0.0f;
      float num1 = (float) this.R / (float) byte.MaxValue;
      float num2 = (float) this.G / (float) byte.MaxValue;
      float num3 = (float) this.B / (float) byte.MaxValue;
      float num4 = 0.0f;
      float num5 = num1;
      float num6 = num1;
      if ((double) num2 > (double) num5)
        num5 = num2;
      if ((double) num3 > (double) num5)
        num5 = num3;
      if ((double) num2 < (double) num6)
        num6 = num2;
      if ((double) num3 < (double) num6)
        num6 = num3;
      float num7 = num5 - num6;
      if ((double) num1 == (double) num5)
        num4 = (num2 - num3) / num7;
      else if ((double) num2 == (double) num5)
        num4 = (float) (2.0 + ((double) num3 - (double) num1) / (double) num7);
      else if ((double) num3 == (double) num5)
        num4 = (float) (4.0 + ((double) num1 - (double) num2) / (double) num7);
      float num8 = num4 * 60f;
      if ((double) num8 < 0.0)
        num8 += 360f;
      return num8;
    }

    /// <summary>
    /// Gets the hue-saturation-brightness (HSB) saturation value for this <see cref="T:System.Drawing.Color"/> structure.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// The saturation of this <see cref="T:System.Drawing.Color"/>. The saturation ranges from 0.0 through 1.0, where 0.0 is grayscale and 1.0 is the most saturated.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
    public float GetSaturation()
    {
      float num1 = (float) this.R / (float) byte.MaxValue;
      float num2 = (float) this.G / (float) byte.MaxValue;
      float num3 = (float) this.B / (float) byte.MaxValue;
      float num4 = 0.0f;
      float num5 = num1;
      float num6 = num1;
      if ((double) num2 > (double) num5)
        num5 = num2;
      if ((double) num3 > (double) num5)
        num5 = num3;
      if ((double) num2 < (double) num6)
        num6 = num2;
      if ((double) num3 < (double) num6)
        num6 = num3;
      if ((double) num5 != (double) num6)
        num4 = ((double) num5 + (double) num6) / 2.0 > 0.5 ? (float) (((double) num5 - (double) num6) / (2.0 - (double) num5 - (double) num6)) : (float) (((double) num5 - (double) num6) / ((double) num5 + (double) num6));
      return num4;
    }

    /// <summary>
    /// Gets the 32-bit ARGB value of this <see cref="T:System.Drawing.Color"/> structure.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// The 32-bit ARGB value of this <see cref="T:System.Drawing.Color"/>.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
    public int ToArgb()
    {
      return (int) this.Value;
    }

    /// <summary>
    /// Gets the <see cref="T:System.Drawing.KnownColor"/> value of this <see cref="T:System.Drawing.Color"/> structure.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// An element of the <see cref="T:System.Drawing.KnownColor"/> enumeration, if the <see cref="T:System.Drawing.Color"/> is created from a predefined color by using either the <see cref="M:System.Drawing.Color.FromName(System.String)"/> method or the <see cref="M:System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor)"/> method; otherwise, 0.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public KnownColor ToKnownColor()
    {
      return (KnownColor) this.knownColor;
    }

    /// <summary>
    /// Converts this <see cref="T:System.Drawing.Color"/> structure to a human-readable string.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A string that is the name of this <see cref="T:System.Drawing.Color"/>, if the <see cref="T:System.Drawing.Color"/> is created from a predefined color by using either the <see cref="M:System.Drawing.Color.FromName(System.String)"/> method or the <see cref="M:System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor)"/> method; otherwise, a string that consists of the ARGB component names and their values.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder(32);
      stringBuilder.Append(this.GetType().Name);
      stringBuilder.Append(" [");
      if (((int) this.state & (int) Color.StateNameValid) != 0)
        stringBuilder.Append(this.Name);
      else if (((int) this.state & (int) Color.StateKnownColorValid) != 0)
        stringBuilder.Append(this.Name);
      else if (((int) this.state & (int) Color.StateValueMask) != 0)
      {
        stringBuilder.Append("A=");
        stringBuilder.Append(this.A);
        stringBuilder.Append(", R=");
        stringBuilder.Append(this.R);
        stringBuilder.Append(", G=");
        stringBuilder.Append(this.G);
        stringBuilder.Append(", B=");
        stringBuilder.Append(this.B);
      }
      else
        stringBuilder.Append("Empty");
      stringBuilder.Append("]");
      return ((object) stringBuilder).ToString();
    }

    /// <summary>
    /// Tests whether the specified object is a <see cref="T:System.Drawing.Color"/> structure and is equivalent to this <see cref="T:System.Drawing.Color"/> structure.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// true if <paramref name="obj"/> is a <see cref="T:System.Drawing.Color"/> structure equivalent to this <see cref="T:System.Drawing.Color"/> structure; otherwise, false.
    /// 
    /// </returns>
    /// <param name="obj">The object to test.
    ///                 </param><filterpriority>1</filterpriority>
    public override bool Equals(object obj)
    {
      if (obj is Color)
      {
        Color color = (Color) obj;
        if (this.value == color.value && (int) this.state == (int) color.state && (int) this.knownColor == (int) color.knownColor)
        {
          if (this.name == color.name)
            return true;
          if (this.name == null || color.name == null)
            return false;
          else
            return this.name.Equals(this.name);
        }
      }
      return false;
    }

    /// <summary>
    /// Returns a hash code for this <see cref="T:System.Drawing.Color"/> structure.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// An integer value that specifies the hash code for this <see cref="T:System.Drawing.Color"/>.
    /// 
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public override int GetHashCode()
    {
      return this.value.GetHashCode() ^ this.state.GetHashCode() ^ this.knownColor.GetHashCode();
    }
  }
}
