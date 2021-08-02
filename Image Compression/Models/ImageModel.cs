using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Image_Compression.Models
{
    public class ImageModel
    {
        public List<string> images { get; set; }
        public string imageByteArray { get; set; }
        public string watermarkpath { get; set; }
    }
}

