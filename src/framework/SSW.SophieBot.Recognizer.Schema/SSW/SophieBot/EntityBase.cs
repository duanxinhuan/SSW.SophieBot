﻿using System.Collections.Generic;
using System.Threading;

namespace SSW.SophieBot
{
    public abstract class RecognizerModelBase : IRecognizerModel
    {
        public virtual async IAsyncEnumerable<bool> SeedAsync(CancellationToken cancellationToken = default)
        {
            yield return true;
        }
    }
}
