// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using OnePassword.Vaults;
using OnePassword;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wox.Plugin;
using OnePassword.Accounts;
using OnePassword.Items;
using ManagedCommon;

namespace Community.PowerToys.Run.Plugin._1Password;

public partial class Main : IReloadable
{

    public void ReloadData()
    {
        Logger.LogDebug("Reloading 1Password plugin data");

        if (_context is not null)
        {
            UpdateIconPath(_context.API.GetCurrentTheme());
        }


        _items.Clear();

        if (!_disabled)
        {
            InitializeItems();
        }


    }

    public void LoadVault(Vault vault)
    {
        if (_passwordManager is null || _vaults is null || _vaults.ContainsKey(vault.Id))
        {
            return;
        }

        AddItemsFromVault(_passwordManager.GetItems(vault));
        _vaults.TryAdd(vault.Id, vault);
    }




}
