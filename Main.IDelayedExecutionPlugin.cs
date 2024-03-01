﻿// Copyright (c) Microsoft Corporation
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

        if (string.IsNullOrEmpty(OnePasswordInstallPath))
            return [new() {
            Title = "1password - Missing Install Path",
            SubTitle = "Set your install path in the plugin's settings",
            Action = context => true,
        }];

        if (string.IsNullOrEmpty(query.Search))
        {
            return [.. results];
        }

        if (_items is null)
            return [new() {
            Title = "DEV ERROR - _items should be init!",
            SubTitle = "Set your install path in the plugin's settings",
            Action = context => true,
        }];

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

        if (_isLoadingVault == true)
        {
            return [.. results];
        }

        if (results.Count() < 3)
        {
            results.Add(new Result()
            {
                Title = "Load More",
                SubTitle = "Load More Vaults",
                Action = _ => {
                    HandleLoadMore();
                    return true;
                }
            });
            results.Add(new Result()
            {
                Title = "Reload All",
                SubTitle = "Warning: Reloading all day may cause search to hang for a moment...",
                Action = _ => {
                    ReloadData();
                    return true;
                }
            });
        }


        return [.. results];
    }


}
