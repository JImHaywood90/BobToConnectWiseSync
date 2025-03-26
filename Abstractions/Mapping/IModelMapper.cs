using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstractions.Mapping
{
    public interface IModelMapper<TSource, TTarget>
    {
        TTarget Map(TSource source, params object[] args);

    }
}
