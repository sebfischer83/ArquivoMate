using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArquivoMate.Domain.Entities
{
    public class PathConfiguration
    {
        public string DocumentFilePathTemplate { get; set; } = null!;

        public string OriginalFilePathTemplate { get; set; } = null!;


    }
}
