// Type: System.Web.Optimization.CssRewriteUrlTransform
// Assembly: System.Web.Optimization, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// Assembly location: D:\Data\Projects\update_tkm_shop\PixelCMS.Web\bin\System.Web.Optimization.dll

using System;
using System.Text.RegularExpressions;
using System.Web;

namespace System.Web.Optimization
{
  public class CssRewriteUrlTransform : IItemTransform
  {
    internal static string RebaseUrlToAbsolute(string baseUrl, string url)
    {
      if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(baseUrl) || url.StartsWith("/", StringComparison.OrdinalIgnoreCase))
        return url;
      if (!baseUrl.EndsWith("/", StringComparison.OrdinalIgnoreCase))
        baseUrl = baseUrl + "/";
      return VirtualPathUtility.ToAbsolute(baseUrl + url);
    }

    internal static string ConvertUrlsToAbsolute(string baseUrl, string content)
    {
      if (string.IsNullOrWhiteSpace(content))
        return content;
      else
        return new Regex("url\\(['\"]?(?<url>[^)]+?)['\"]?\\)").Replace(content, (MatchEvaluator) (match => "url(" + CssRewriteUrlTransform.RebaseUrlToAbsolute(baseUrl, match.Groups["url"].Value) + ")"));
    }

    public string Process(string includedVirtualPath, string input)
    {
      if (includedVirtualPath == null)
        throw new ArgumentNullException("includedVirtualPath");
      else
        return CssRewriteUrlTransform.ConvertUrlsToAbsolute(VirtualPathUtility.GetDirectory(includedVirtualPath.Substring(1)), input);
    }
  }
}
