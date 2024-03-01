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

namespace Community.PowerToys.Run.Plugin._1Password;

public partial class Main : IReloadable
{
    public void ReloadData()
    {
        if (_context is not null)
        {
            UpdateIconPath(_context.API.GetCurrentTheme());
        }

        if (string.IsNullOrEmpty(OnePasswordInstallPath) || string.IsNullOrEmpty(OnePasswordInitVault) || _items is null || _loadedVaults is null)
        {
            return;
        }

        _items.Clear();

        var onePasswordManagerOptions = new OnePasswordManagerOptions
        {
            Path = OnePasswordInstallPath,
            AppIntegrated = true
        };

        _passwordManager = new OnePasswordManager(onePasswordManagerOptions);

        if (_initialVaultsLoaded) {
            foreach (var vault in _loadedVaults.Values) {
                foreach (var item in _passwordManager.GetItems(vault))
                {
                    _items.Add(item);
                }
            }
            return;
        }

        var allVaults = _passwordManager.GetVaults();

        if (string.IsNullOrEmpty(OnePasswordExcludeVault))
        {
            _ = allVaults.RemoveAll(vault => vault.Name == OnePasswordExcludeVault);
        }

        Vault? initVault = null;

        foreach (var vault in allVaults)
        {
            if (vault.Name == OnePasswordInitVault)
            {
                initVault = vault;
                continue;
            }

            _vaultsQueue.Enqueue(vault);
        }

        if (initVault is not null)
        {
            LoadVault(initVault);
            _initialVaultsLoaded = true;
        }
    }

    // Handle Lazy Loading

    private volatile bool _isLoadingVault = false;
    public void LoadVault(Vault? vault)
    {
        if (_passwordManager == null || vault is null || _loadedVaults is null ||  _loadedVaults.ContainsKey(vault.Id) )
        {
            return;
        }

        _isLoadingVault = true;

        var itemsToAdd = _passwordManager.GetItems(vault);

        foreach (var item in itemsToAdd)
        {
            _items?.Add(item);
        }

        _loadedVaults.TryAdd(vault.Id, vault);

        _isLoadingVault = false;
    }
    private void HandleLoadMore()
    {
        if (_passwordManager == null || _isLoadingVault || (_vaultsQueue.Count == 0))
        {
            return;
        }

        _ = _vaultsQueue.TryDequeue(out Vault? result);
        LoadVault(result);
    }
}
