using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Xml.Linq;

namespace RazorClientTemplates
{
    public class RazorClientTemplateHandler : IHttpHandler
    {
        private readonly RazorClientTemplateEngine _templateEngine;

        public bool IsReusable
        {
            get { return false; }
        }


        public RazorClientTemplateHandler()
            : this(RazorClientTemplateEngine.Current)
        {
        }

        public RazorClientTemplateHandler(RazorClientTemplateEngine templateEngine)
        {
            _templateEngine = templateEngine;
        }


        public void ProcessRequest(HttpContext context)
        {
            ProcessRequest(new HttpContextWrapper(context));
        }

        public void ProcessRequest(HttpContextBase context)
        {
            var template = context.Request["template"];
            var category = context.Request["category"];
            var function = context.Request["function"];

            if(string.IsNullOrWhiteSpace(template))
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.StatusDescription = "Invalid template path";
                return;
            }

            if (string.IsNullOrWhiteSpace(function))
                function = template;

            context.Response.Clear();
            context.Response.ContentType = "text/javascript";

            using (var stream = GetRazorTemplateStream(context, template, category))
            {
                context.Response.Output.Write(function + " = ");
                _templateEngine.RenderClientTemplate(stream, context.Response.Output);
            }
        }

        public static string GenerateUrl(string templateName, string controllerName = null, string functionName = null)
        {
            var urlBuilder = new StringBuilder(HandlerPath);

            urlBuilder.AppendFormat("?template={0}", templateName);

            if (!string.IsNullOrWhiteSpace(controllerName))
                urlBuilder.AppendFormat("&category={0}", controllerName);

            if (!string.IsNullOrWhiteSpace(functionName))
                urlBuilder.AppendFormat("&function={0}", functionName);

            return urlBuilder.ToString();
        }

        private static StreamReader GetRazorTemplateStream(HttpContextBase context, string template, string category)
        {
            var controller = string.IsNullOrWhiteSpace(category) ? "Shared" : category;

            var razorTemplatePath = string.Format("~/Views/{0}/{1}.cshtml", controller, template);
            var virtualPath = VirtualPathUtility.ToAppRelative(razorTemplatePath);
            var serverPath = context.Server.MapPath(virtualPath);
            return new StreamReader(serverPath);
        }


        public static string HandlerPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_handlerPath) == false)
                    return _handlerPath;


                var webConfig = WebConfigurationManager.OpenWebConfiguration("~/");

                var handlers = new List<XElement>();

                ConfigurationSection webServerSection = webConfig.GetSection("system.webServer");
                if (webServerSection != null)
                {
                    var webServerXml = webServerSection.SectionInformation.GetRawXml();
                    var webServerHandlers = XDocument.Parse(webServerXml).Descendants("handlers").Descendants("add");
                    handlers.AddRange(webServerHandlers);
                }

                ConfigurationSection webSection = webConfig.GetSection("system.web");
                if (webSection != null)
                {
                    var webXml = webSection.SectionInformation.GetRawXml();
                    var webHandlers = XDocument.Parse(webXml).Descendants("httpHandlers").Descendants("add");
                    handlers.AddRange(webHandlers);
                }

                var thisHandler =
                    handlers
                        .FirstOrDefault(x => x.Attributes("type")
                                                .Any(attr => attr.Value.StartsWith(typeof(RazorClientTemplateHandler).FullName)));

                if (thisHandler == null)
                    throw new RazorClientTemplateException("Couldn't locate RazorClientTemplateHandler - is it registered?");

                _handlerPath = VirtualPathUtility.ToAbsolute("~/" + thisHandler.Attribute("path").Value);

                return _handlerPath;
            }
        }
        private static string _handlerPath;
    }
}
