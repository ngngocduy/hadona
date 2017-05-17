// Type: System.Web.Mvc.WebViewPage`1
// Assembly: System.Web.Mvc, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// Assembly location: D:\Data\Projects\TheKyMoi_Project\TuHao\packages\Microsoft.AspNet.Mvc.4.0.20710.0\lib\net40\System.Web.Mvc.dll

namespace System.Web.Mvc
{
  /// <summary>
  /// Represents the properties and methods that are needed in order to render a view that uses ASP.NET Razor syntax.
  /// </summary>
  /// <typeparam name="TModel">The type of the view data model.</typeparam>
  public abstract class WebViewPage<TModel> : WebViewPage
  {
    private ViewDataDictionary<TModel> _viewData;

    /// <summary>
    /// Gets or sets the <see cref="T:System.Web.Mvc.AjaxHelper"/> object that is used to render HTML markup using Ajax.
    /// </summary>
    /// 
    /// <returns>
    /// The <see cref="T:System.Web.Mvc.AjaxHelper"/> object that is used to render HTML markup using Ajax.
    /// </returns>
    public AjaxHelper<TModel> Ajax { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="T:System.Web.Mvc.HtmlHelper"/> object that is used to render HTML elements.
    /// </summary>
    /// 
    /// <returns>
    /// The <see cref="T:System.Web.Mvc.HtmlHelper"/> object that is used to render HTML elements.
    /// </returns>
    public HtmlHelper<TModel> Html { get; set; }

    /// <summary>
    /// Gets the Model property of the associated <see cref="T:System.Web.Mvc.ViewDataDictionary"/> object.
    /// </summary>
    /// 
    /// <returns>
    /// The Model property of the associated <see cref="T:System.Web.Mvc.ViewDataDictionary"/> object.
    /// </returns>
    public TModel Model
    {
      get
      {
        return this.ViewData.Model;
      }
    }

    /// <summary>
    /// Gets or sets a dictionary that contains data to pass between the controller and the view.
    /// </summary>
    /// 
    /// <returns>
    /// A dictionary that contains data to pass between the controller and the view.
    /// </returns>
    public ViewDataDictionary<TModel> ViewData
    {
      get
      {
        if (this._viewData == null)
          this.SetViewData((ViewDataDictionary) new ViewDataDictionary<TModel>());
        return this._viewData;
      }
      set
      {
        this.SetViewData((ViewDataDictionary) value);
      }
    }

    /// <summary>
    /// Initializes the <see cref="T:System.Web.Mvc.AjaxHelper"/>, <see cref="T:System.Web.Mvc.HtmlHelper"/>, and <see cref="T:System.Web.Mvc.UrlHelper"/> classes.
    /// </summary>
    public override void InitHelpers()
    {
      base.InitHelpers();
      this.Ajax = new AjaxHelper<TModel>(this.ViewContext, (IViewDataContainer) this);
      this.Html = new HtmlHelper<TModel>(this.ViewContext, (IViewDataContainer) this);
    }

    /// <summary>
    /// Sets the view data.
    /// </summary>
    /// <param name="viewData">The view data.</param>
    protected override void SetViewData(ViewDataDictionary viewData)
    {
      this._viewData = new ViewDataDictionary<TModel>(viewData);
      base.SetViewData((ViewDataDictionary) this._viewData);
    }
  }
}
