using System;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RazorClientTemplates
{
    [TestClass]
    public class RazorClientTemplateEngineTests
    {
        private readonly string _templateSource;

        public RazorClientTemplateEngineTests()
        {
            var path = Path.Combine(Environment.CurrentDirectory, @"Templates\Movie.cshtml");
            _templateSource = File.ReadAllText(path);
        }

        [TestMethod]
        public void ShouldRenderClientTemplate()
        {
            Debug.Write(new RazorClientTemplateEngine().RenderClientTemplate(_templateSource));
        }
    }
}
