// Type: System.Data.LoadOption
// Assembly: System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.Data.dll

namespace System.Data
{
  /// <summary>
  /// Controls how the values from the data source will be applied to existing rows when using the <see cref="Overload:System.Data.DataTable.Load"/> or <see cref="Overload:System.Data.DataSet.Load"/> method.
  /// </summary>
  /// <filterpriority>2</filterpriority>
  public enum LoadOption
  {
    OverwriteChanges = 1,
    PreserveChanges = 2,
    Upsert = 3,
  }
}
