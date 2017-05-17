// Type: OfficeOpenXml.Style.ExcelColor
// Assembly: EPPlus, Version=3.1.3.0, Culture=neutral, PublicKeyToken=ea159fdaa78159a1
// Assembly location: D:\Data\Projects\TheKyMoi_Shop\packages\EPPlus.3.1.3.3\lib\net35\EPPlus.dll

using OfficeOpenXml;
using OfficeOpenXml.Style.XmlAccess;
using System;
using System.Drawing;

namespace OfficeOpenXml.Style
{
  /// <summary>
  /// Color for cellstyling
  /// 
  /// </summary>
  public sealed class ExcelColor : StyleBase
  {
    private eStyleClass _cls;
    private StyleBase _parent;

    /// <summary>
    /// The theme color
    /// 
    /// </summary>
    public string Theme
    {
      get
      {
        return this.GetSource().Theme;
      }
    }

    /// <summary>
    /// The tint value
    /// 
    /// </summary>
    public Decimal Tint
    {
      get
      {
        return this.GetSource().Tint;
      }
      set
      {
        if (value > new Decimal(1) || value < new Decimal(-1))
          throw new ArgumentOutOfRangeException("Value must be between -1 and 1");
        int num = this._ChangedEvent((StyleBase) this, new StyleChangeEventArgs(this._cls, eStyleProperty.Tint, (object) value, this._positionID, this._address));
      }
    }

    /// <summary>
    /// The RGB value
    /// 
    /// </summary>
    public string Rgb
    {
      get
      {
        return this.GetSource().Rgb;
      }
      internal set
      {
        int num = this._ChangedEvent((StyleBase) this, new StyleChangeEventArgs(this._cls, eStyleProperty.Color, (object) value, this._positionID, this._address));
      }
    }

    /// <summary>
    /// The indexed color number.
    /// 
    /// </summary>
    public int Indexed
    {
      get
      {
        return this.GetSource().Indexed;
      }
      set
      {
        int num = this._ChangedEvent((StyleBase) this, new StyleChangeEventArgs(this._cls, eStyleProperty.IndexedColor, (object) value, this._positionID, this._address));
      }
    }

    internal override string Id
    {
      get
      {
        return string.Concat(new object[4]
        {
          (object) this.Theme,
          (object) this.Tint,
          (object) this.Rgb,
          (object) this.Indexed
        });
      }
    }

    internal ExcelColor(ExcelStyles styles, XmlHelper.ChangedEventHandler ChangedEvent, int worksheetID, string address, eStyleClass cls, StyleBase parent)
      : base(styles, ChangedEvent, worksheetID, address)
    {
      this._parent = parent;
      this._cls = cls;
    }

    /// <summary>
    /// Set the color of the object
    /// 
    /// </summary>
    /// <param name="color">The color</param>
    public void SetColor(Color color)
    {
      this.Rgb = color.ToArgb().ToString("X");
    }

    private ExcelColorXml GetSource()
    {
      this.Index = this._parent.Index < 0 ? 0 : this._parent.Index;
      switch (this._cls)
      {
        case eStyleClass.Font:
          return this._styles.Fonts[this.Index].Color;
        case eStyleClass.BorderTop:
          return this._styles.Borders[this.Index].Top.Color;
        case eStyleClass.BorderLeft:
          return this._styles.Borders[this.Index].Left.Color;
        case eStyleClass.BorderBottom:
          return this._styles.Borders[this.Index].Bottom.Color;
        case eStyleClass.BorderRight:
          return this._styles.Borders[this.Index].Right.Color;
        case eStyleClass.BorderDiagonal:
          return this._styles.Borders[this.Index].Diagonal.Color;
        case eStyleClass.FillBackgroundColor:
          return this._styles.Fills[this.Index].BackgroundColor;
        case eStyleClass.FillPatternColor:
          return this._styles.Fills[this.Index].PatternColor;
        default:
          throw new Exception("Invalid style-class for Color");
      }
    }

    internal override void SetIndex(int index)
    {
      this._parent.Index = index;
    }
  }
}
