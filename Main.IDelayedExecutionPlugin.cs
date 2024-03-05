// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wox.Plugin;

namespace Community.PowerToys.Run.Plugin._1Password;

public partial class Main : IDelayedExecutionPlugin
{
    public List<Result> Query(Query query, bool delayedExecution)
    {
        ArgumentNullException.ThrowIfNull(query);

        var results = new ConcurrentBag<Result>();


        Parallel.ForEach(_items, item =>
        {
            if (string.IsNullOrEmpty(item?.Vault?.Name)
                || string.IsNullOrEmpty(item?.Title)
                || !item.Title.Contains(query.Search, StringComparison.CurrentCultureIgnoreCase)
            ) return;

            results.Add(new Result()
            {
                Title = item.Vault.Name,
                SubTitle = item.Title,
                ContextData = item.Id,
            });

        });

  
        return [.. results];
    }


}
