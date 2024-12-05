using ArquivoMate.Infrastructure.Settings.FileProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArquivoMate.Infrastructure.Settings.DeliveryProvider
{
    public enum DeliveryProviderType
    {
        Local,
        BunnySimple,
        BunnyToken,
        BunnyS3
    }
}
