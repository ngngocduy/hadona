// Type: System.Web.Mvc.WebViewPage
// Assembly: System.Web.Mvc, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// Assembly location: D:\Data\Projects\TheKyMoi_Project\BaoBiHuyPhat\packages\Microsoft.AspNet.Mvc.4.0.20710.0\lib\net40\System.Web.Mvc.dll

using System;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.Mvc.Properties;
using System.Web.WebPages;

namespace System.Web.Mvc
{
  /// <summary>
  /// Represents the properties and methods that are needed in order to render a view that uses ASP.NET Razor syntax.
  /// </summary>
  public abstract class WebViewPage : WebPageBase, IViewDataContainer, IViewStartPageChild
  {
    private ViewDataDictionary _viewData;
    private DynamicViewDataDictionary _dynamicViewData;
    private HttpContextBase _context;

    /// <summary>
    /// Gets or sets the <see cref="T:System.Web.Mvc.AjaxHelper"/> object that is used to render HTML using Ajax.
    /// </summary>
    /// 
    /// <returns>
    /// The <see cref="T:System.Web.Mvc.AjaxHelper"/> object that is used to render HTML using Ajax.
    /// </returns>
    public AjaxHelper<object> Ajax { get; set; }

    /// <summary>
    /// Gets the <see cref="T:System.Web.HttpContext"/> object that is associated with the page.
    /// </summary>
    /// 
    /// <returns>
    /// The <see cref="T:System.Web.HttpContext"/> object that is associated with the page.
    /// </returns>
    public override HttpContextBase Context
    {
      get
      {
        return this._context ?? this.ViewContext.HttpContext;
      }
      set
      {
        this._context = value;
      }
    }

    /// <summary>
    /// Gets or sets the <see cref="T:System.Web.Mvc.HtmlHelper"/> object that is used to render HTML elements.
    /// </summary>
    /// 
    /// <returns>
    /// The <see cref="T:System.Web.Mvc.HtmlHelper"/> object that is used to render HTML elements.
    /// </returns>
    public HtmlHelper<object> Html { get; set; }

    /// <summary>
    /// Gets the Model property of the associated <see cref="T:System.Web.Mvc.ViewDataDictionary"/> object.
    /// </summary>
    /// 
    /// <returns>
    /// The Model property of the associated <see cref="T:System.Web.Mvc.ViewDataDictionary"/> object.
    /// </returns>
    public object Model
    {
      get
      {
        return this.ViewData.Model;
      }
    }

    internal string OverridenLayoutPath { get; set; }

    /// <summary>
    /// Gets the temporary data to pass to the view.
    /// </summary>
    /// 
    /// <returns>
    /// The temporary data to pass to the view.
    /// </returns>
    public TempDataDictionary TempData
    {
      get
      {
        return this.ViewContext.TempData;
      }
    }

    /// <summary>
    /// Gets or sets the URL of the rendered page.
    /// </summary>
    /// 
    /// <returns>
    /// The URL of the rendered page.
    /// </returns>
    public UrlHelper Url { get; set; }

    /// <summary>
    /// Gets the view bag.
    /// </summary>
    /// 
    /// <returns>
    /// The view bag.
    /// </returns>
    public object ViewBag
    {
      get
      {
        if (this._dynamicViewData == null)
          this._dynamicViewData = new DynamicViewDataDictionary((Func<ViewDataDictionary>) (() => this.ViewData));
        return (object) this._dynamicViewData;
      }
    }

    /// <summary>
    /// Gets or sets the information that is used to render the view.
    /// </summary>
    /// 
    /// <returns>
    /// The information that is used to render the view, which includes the form context, the temporary data, and the view data of the associated view.
    /// </returns>
    public ViewContext ViewContext { get; set; }

    /// <summary>
    /// Gets or sets a dictionary that contains data to pass between the controller and the view.
    /// </summary>
    /// 
    /// <returns>
    /// A dictionary that contains data to pass between the controller and the view.
    /// </returns>
    public ViewDataDictionary ViewData
    {
      get
      {
        if (this._viewData == null)
          this.SetViewData(new ViewDataDictionary());
        return this._viewData;
      }
      set
      {
        this.SetViewData(value);
      }
    }

    /// <summary>
    /// Sets the view context and view data for the page.
    /// </summary>
    /// <param name="parentPage">The parent page.</param>
    protected override void ConfigurePage(WebPageBase parentPage)
    {
      WebViewPage webViewPage = parentPage as WebViewPage;
      if (webViewPage == null)
      {
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, MvcResources.CshtmlView_WrongViewBase, new object[1]
        {
          (object) parentPage.VirtualPath
        }));
      }
      else
      {
        this.ViewContext = webViewPage.ViewContext;
        this.ViewData = webViewPage.ViewData;
        this.InitHelpers();
      }
    }

    /// <summary>
    /// Runs the page hierarchy for the ASP.NET Razor execution pipeline.
    /// </summary>
    public override void ExecutePageHierarchy()
    {
      TextWriter writer = this.ViewContext.Writer;
      this.ViewContext.Writer = this.Output;
      base.ExecutePageHierarchy();
      if (!string.IsNullOrEmpty(this.OverridenLayoutPath))
        this.Layout = this.OverridenLayoutPath;
      this.ViewContext.Writer = writer;
    }

    /// <summary>
    /// Initializes the <see cref="T:System.Web.Mvc.AjaxHelper"/>, <see cref="T:System.Web.Mvc.HtmlHelper"/>, and <see cref="T:System.Web.Mvc.UrlHelper"/> classes.
    /// </summary>
    public virtual void InitHelpers()
    {
      this.Ajax = new AjaxHelper<object>(this.ViewContext, (IViewDataContainer) this);
      this.Html = new HtmlHelper<object>(this.ViewContext, (IViewDataContainer) this);
      this.Url = new UrlHelper(this.ViewContext.RequestContext);
    }

    /// <summary>
    /// Sets the view data.
    /// </summary>
    /// <param name="viewData">The view data.</param>
    protected virtual void SetViewData(ViewDataDictionary viewData)
    {
      this._viewData = viewData;
    }
  }
}
