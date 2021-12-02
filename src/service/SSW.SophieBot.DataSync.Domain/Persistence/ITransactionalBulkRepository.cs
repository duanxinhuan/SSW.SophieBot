﻿using System.Threading;
using System.Threading.Tasks;

namespace SSW.SophieBot.DataSync.Domain.Persistence
{
    public interface ITransactionalBulkRepository<T, TOperation> : IBulkRepository<T>
    {
        Task<ITransactionBatch<TOperation>> BeginTransactionAsync(CancellationToken cancellationToken = default);
    }
}
