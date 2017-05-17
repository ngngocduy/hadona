using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PixelCMS.Core.Models
{
    public static class ExtensionMethods
    {
       

        #region Controller Extend

        /// <summary>
        /// PMTs the json ajax.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="data">The data.</param>
        /// <param name="error">if set to <c>true</c> [error].</param>
        /// <param name="messages">The messages.</param>
        /// <param name="behavior">The behavior.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="contentEncoding">The content encoding.</param>
        /// <returns></returns>
        /// <author>CanhChan</author>
        /// <created>11/27/2012 8:09 PM</created>
        internal static JsonResult PMTJsonAjax(this Controller controller, object data, bool error = false, object messages = null,
            JsonRequestBehavior behavior = JsonRequestBehavior.AllowGet, string contentType = null, Encoding contentEncoding = null)
        {
            // Tạo dữ liệu trả về
            var jsonData = new { error = error, messages = messages, data = data };

            return new JsonResult
            {
                Data = jsonData,
                JsonRequestBehavior = behavior
            };
        }

        /// <summary>
        /// Renders the partial view.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="viewName">Name of the view.</param>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        /// <author>Canh Chan</author>
        /// <created>12/25/2012 4:28 PM</created>
        /// <url>http://stackoverflow.com/questions/1181849/asp-net-mvc-combine-json-result-with-viewresult</url>
        internal static string RenderPartialView(this Controller controller, string viewName, object model = null)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = controller.ControllerContext.RouteData.GetRequiredString("action");

            controller.ViewData.Model = model;               

            using (var sw = new System.IO.StringWriter())
            {
                if (controller.ControllerContext == null)
                {
                    controller.ControllerContext = new ControllerContext(System.Web.HttpContext.Current.Request.RequestContext, controller);
                }

                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);

                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);                

                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        #endregion

        #region Others

        /// <summary>
        /// Adds the strings.
        /// </summary>
        /// <param name="val">The val.</param>
        /// <param name="values">The values.</param>
        public static void AddStrings(this List<string> val, params string[] values)
        {
            foreach (string e in values)
            {
                val.Add(e);
            }
        }

        /// <summary>
        /// Nulls the is zero.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val">The val.</param>
        /// <returns></returns>
        /// <author>CanhChan</author>
        /// <date>7/1/2011</date>
        public static T NullIsZero<T>(this Nullable<T> val) where T : struct
        {
            return val.HasValue ? val.Value : (T)Convert.ChangeType(0, typeof(T));
        }

        /// <summary>
        /// Nulls the is empty.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val">The val.</param>
        /// <returns></returns>
        /// <author>CanhChan</author>
        /// <date>7/21/2011</date>
        public static string NullIsEmpty<T>(this Nullable<T> val) where T : struct
        {
            return val.HasValue ? val.Value.ToString() : string.Empty;
        }

        /// <summary>
        /// Nulls the or zero is empty.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val">The val.</param>
        /// <returns></returns>
        /// <author>CanhChan</author>
        /// <date>7/21/2011</date>
        public static string NullOrZeroIsEmpty<T>(this Nullable<T> val) where T : struct
        {
            if (val.HasValue)
            {
                decimal dec = 0;

                if (decimal.TryParse(val.Value.ToString(), out dec))
                {
                    return dec != 0 ? dec.ToString() : string.Empty;
                }
            }

            return val.HasValue ? val.Value.ToString() : string.Empty;
        }

        /// <summary>
        /// Nulls the or zero is empty formated.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val">The val.</param>
        /// <returns></returns>
        /// <author>CanhChan</author>
        /// <date>7/21/2011</date>
        public static string NullOrZeroIsEmptyFormated<T>(this Nullable<T> val) where T : struct
        {
            if (val.HasValue)
            {
                decimal dec = 0;

                if (decimal.TryParse(val.Value.ToString(), out dec))
                {
                    return dec != 0 ? dec.ToString("N0") : string.Empty;
                }
            }

            return val.HasValue ? val.Value.ToString() : string.Empty;
        }

        /// <summary>
        /// Nulls the is false.
        /// </summary>
        /// <param name="val">The val.</param>
        /// <returns></returns>
        /// <author>Canh Chan</author>
        /// <date>04/04/2012 8:22 SA</date>
        public static bool NullIsFalse(this Nullable<bool> val)
        {
            return val.HasValue ? val.Value : false;
        }

        /// <summary>
        /// Parses the specified val.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val">The val.</param>
        /// <returns></returns>
        /// <author>CanhChan</author>
        /// <date>7/18/2011</date>
        public static T Parse<T>(this string val) where T : struct
        {
            return (T)Convert.ChangeType(val, typeof(T));
        }

        /// <summary>
        /// Tries the parse null when fail.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val">The val.</param>
        /// <returns></returns>
        /// <author>CanhChan</author>
        /// <date>7/27/2011</date>
        public static Nullable<T> TryParseNullWhenFail<T>(this string val) where T : struct
        {
            try
            {
                T value = (T)Convert.ChangeType(val, typeof(T));

                return value;
            }
            catch { }

            return null;
        }

        /// <summary>
        /// Replaces the specified val.
        /// </summary>
        /// <param name="val">The val.</param>
        /// <param name="dicValue">The dic value.</param>
        /// <returns></returns>
        /// <author>Hong Tranh</author>
        /// <date>20/08/2011</date>
        public static string Replace(this string val, Dictionary<string, string> dicValue)
        {
            dicValue.ToList().ForEach(m => val = val.Replace(m.Key, m.Value));
            return val;
        }

        /// <summary>
        /// Zeroes the is null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val">The val.</param>
        /// <returns></returns>
        /// <author>CanhChan</author>
        /// <date>23/08/2011</date>
        public static Nullable<T> ZeroIsNull<T>(this Nullable<T> val) where T : struct
        {
            try
            {
                decimal decVal = (decimal)Convert.ChangeType(val, typeof(decimal));

                if (decVal == 0)
                {
                    return null;
                }
            }
            catch { }

            return val;
        }

        /// <summary>
        /// Zeroes the is emty format.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val">The val.</param>
        /// <returns></returns>
        public static string ZeroIsEmtyFormat<T>(this T val)
        {
            try
            {
                decimal decVal = (decimal)Convert.ChangeType(val, typeof(decimal));

                if (decVal == 0)
                {
                    return string.Empty;
                }
                //else
                //{
                //    return decVal.ToString("N0");
                //}
            }
            catch { }

            return val.ToString();
        }

        /// <summary>
        /// Clones the specified val.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val">The val.</param>
        /// <returns></returns>
        /// <author>Canh Chan</author>
        /// <date>16/12/2011 5:25 CH</date>
        public static List<T> Clone<T>(this List<T> val)
        {
            List<T> listClone = new List<T>();

            if (val != null)
            {
                foreach (T v in val)
                {
                    listClone.Add(v);
                }
            }

            return listClone;
        }

        /// <summary>
        /// Parses to int.
        /// </summary>
        /// <param name="val">The val.</param>
        /// <returns></returns>
        public static int ParseToInt(this Enum val)
        {
            return (int)Convert.ChangeType(val, typeof(int));
        }

        /// <summary>
        /// Gets the enum desciption.
        /// </summary>
        /// <param name="val">The val.</param>
        /// <returns></returns>
        /// <author>Canh Chan</author>
        /// <date>17/12/2011 3:38 CH</date>
        public static string GetEnumDesciption(this Enum val)
        {
            string name = Enum.GetName(val.GetType(), val);

            System.Reflection.FieldInfo obj = val.GetType().GetField(name);

            if (obj != null)
            {
                object[] attributes = obj.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);

                return attributes.Length > 0 ? ((System.ComponentModel.DescriptionAttribute)attributes[0]).Description : null;
            }

            return null;
        }

        /// <summary>
        /// Nulls the is empty.
        /// </summary>
        /// <param name="val">The val.</param>
        /// <returns></returns>
        public static string NullIsEmpty(this string val)
        {
            if (val == null)
            {
                return string.Empty;
            }

            return val;
        }

        /// <summary>
        /// Determines whether [is null or empty] [the specified val].
        /// </summary>
        /// <param name="val">The val.</param>
        /// <returns></returns>
        /// <author>Canh Chan</author>
        /// <created>12/26/2012 11:15 AM</created>
        public static bool IsNullOrEmpty(this string val)
        {
            return string.IsNullOrEmpty(val);
        }

        /// <summary>
        /// Parses the specified vals.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vals">The vals.</param>
        /// <returns></returns>
        public static IList<T> Parse<T>(this IList<object> vals)
        {
            IList<T> data = new List<T>();

            if (vals != null)
            {
                foreach (var d in vals)
                {
                    T value = (T)Convert.ChangeType(d, typeof(T));

                    data.Add(value);
                }
            }

            return data;
        }        

        #endregion
    }
}
