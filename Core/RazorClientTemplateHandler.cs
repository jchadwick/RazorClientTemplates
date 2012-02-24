using System.IO;
using System.Net;
using System.Web;

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
            var razorTemplatePath = context.Request["template"];

            if(string.IsNullOrWhiteSpace(razorTemplatePath))
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.StatusDescription = "Invalid template path";
                return;
            }

            context.Response.Clear();
            context.Response.ContentType = "text/javascript";

            using(var stream = GetRazorTemplateStream(context, razorTemplatePath))
            {
                context.Response.Output.Write(context.Request["name"] + " = ");
                _templateEngine.RenderClientTemplate(stream, context.Response.Output);
            }
        }

        private static StreamReader GetRazorTemplateStream(HttpContextBase context, string razorTemplatePath)
        {
            var virtualPath = VirtualPathUtility.ToAppRelative(razorTemplatePath);
            var serverPath = context.Server.MapPath(virtualPath);
            return new StreamReader(serverPath);
        }
    }
}
