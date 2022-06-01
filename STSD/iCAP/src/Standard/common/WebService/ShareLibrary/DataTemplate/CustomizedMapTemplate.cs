using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLibrary.DataTemplate
{
    public class CustomizedMapTemplate
    {
        public string Name { get; set; }
        public IFormFile File { get; set; }
    }
}
