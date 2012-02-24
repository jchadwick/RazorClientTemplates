using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace RazorClientTemplates
{
    public static class RazorClientTemplateHtmlExtensions
    {
        private static readonly RazorClientTemplateEngine TemplateEngine = new RazorClientTemplateEngine();

        public static IHtmlString ClientTemplate(this HtmlHelper html, string templateName)
        {
            var buffer = new StringWriter();

            using (var reader = html.GetPartialViewStream(templateName))
                TemplateEngine.RenderClientTemplate(reader, buffer);

            return MvcHtmlString.Create(buffer.GetStringBuilder().ToString());
        }

        private static TextReader GetPartialViewStream(this HtmlHelper html, string templateName)
        {
            try
            {
                var view = ViewEngines.Engines.FindPartialView(html.ViewContext.Controller.ControllerContext, templateName);
                
                if(view == null || view.View == null)
                    throw new RazorClientTemplateException(string.Format("Partial View {0} not found", templateName));

                var razorView = view.View as RazorView;

                if (razorView == null)
                {
                    throw new RazorClientTemplateException(
                        string.Format("View Type {0} is not supported", view.View.GetType()));
                }

                var location = html.ViewContext.HttpContext.Server.MapPath(razorView.ViewPath);
                
                return new StreamReader(location);
            }
            catch (Exception e)
            {
                throw new RazorClientTemplateException(e);
            }
        }
    }
}
