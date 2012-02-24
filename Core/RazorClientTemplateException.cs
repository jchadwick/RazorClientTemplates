using System;

namespace RazorClientTemplates
{
    public class RazorClientTemplateException : Exception
    {
        public RazorClientTemplateException(string message)
            : base(message)
        {
        }

        public RazorClientTemplateException(Exception exception)
            : base("Error rendering Razor Client Template", exception)
        {
        }
    }
}