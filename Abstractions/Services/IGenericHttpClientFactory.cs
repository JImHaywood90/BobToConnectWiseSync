using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstractions.Services
{
    public interface IGenericHttpClientFactory
    {
        IGenericHttpClient CreateClient(string name);
    }
}
