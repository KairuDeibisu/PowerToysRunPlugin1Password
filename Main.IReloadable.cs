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

namespace Community.PowerToys.Run.Plugin._1Password;

public partial class Main : IReloadable
{

    public void ReloadData()
    {
        if (_context is not null)
        {
            UpdateIconPath(_context.API.GetCurrentTheme());
        }

        _items.Clear();
        InitializeItems();

    }

    private bool InitializePasswordManager() {
        if (string.IsNullOrEmpty(OnePasswordInstallPath))
        {
            DisablePlugin(Properties.Resources.error_missing_required_one_password_cli_path);
            return false;
        }

        if (_items is null || _vaults is null)
        {
            DisablePlugin(Properties.Resources.error_internal_error_vaults_not_initialized);
            return false;
        }

        var onePasswordManagerOptions = new OnePasswordManagerOptions { Path = OnePasswordInstallPath, AppIntegrated = true };
        _passwordManager = new OnePasswordManager(onePasswordManagerOptions);
        return true;
    }

    private bool InitializeAccountHandling()
    {
        var accounts = _passwordManager.GetAccounts();
        if (accounts.IsEmpty)
        {
            DisablePlugin(Properties.Resources.error_one_password_no_accounts_found);
            return false;
        }

        Account? account = null;
        if (accounts.Count > 1)
        {
            if (string.IsNullOrEmpty(OnePasswordEmail))
            {
                DisablePlugin(Properties.Resources.error_email_not_specified);
                return false;

            }

            account = accounts.FirstOrDefault(acc => acc.Email == OnePasswordEmail);
            if (account is null)
            {
                DisablePlugin(Properties.Resources.error_email_found_no_match);
                return false;

            }
        }

        if (account is null)
        {
            return true;
        }

        _passwordManager.UseAccount(account?.Email);
        return true;
    }

    public void LoadVault(Vault vault)
    {
        if (_passwordManager is null || _vaults is null  || _vaults.ContainsKey(vault.Id))
        {
            return;
        }

        AddItemsFromVault(_passwordManager.GetItems(vault));
        _vaults.TryAdd(vault.Id, vault);
    }




}
