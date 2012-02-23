using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Core;

namespace RazorClientTemplates
{
    public static class RazorClientTemplateHtmlExtensions
    {
        private static readonly RazorClientTemplateEngine TemplateEngine = new RazorClientTemplateEngine();

        public static IHtmlString ClientTemplate(this HtmlHelper html, string templateName, object model = null)
        {
            var partialOutput = html.Partial(templateName, model);

            if(html.HasClientTemplateBeenRendered(templateName))
            {
                return partialOutput;
            }

            var buffer = new StringWriter();
            buffer.WriteLine("<script type='text/javascript'>");
            buffer.WriteLine(templateName + "_ClientTemplate = ");

            using (var reader = html.GetPartialViewStream(templateName))
                TemplateEngine.RenderClientTemplate(reader, buffer);
            
            buffer.WriteLine("</script>");
            buffer.WriteLine(partialOutput.ToString());

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

        private static bool HasClientTemplateBeenRendered(this HtmlHelper html, string templateName)
        {
            var requestCache = html.ViewContext.HttpContext.Items;

            var renderedTemplates = requestCache["RazorClientTemplates"] as IList<string>;

            if(renderedTemplates == null)
            {
                requestCache["RazorClientTemplates"] = renderedTemplates = new List<string>();
            }

            if (renderedTemplates.Contains(templateName))
                return true;

            renderedTemplates.Add(templateName);

            return false;
        }
    }
}
