// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ManagedCommon;
using Microsoft.PowerToys.Settings.UI.Library;
using OnePassword;
using OnePassword.Items;
using OnePassword.Vaults;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using Wox.Plugin;

namespace Community.PowerToys.Run.Plugin._1Password;

public partial class Main : IPlugin
{
    // IPlugin
    public static string PluginID => "9576F2CB78674305838C65FD2BE224AC";
    public string? IconPath { get; set; }
    private PluginInitContext? _context;
    public string Name => Properties.Resources.plugin_name;
    public string Description => Properties.Resources.plugin_description;

    
    // A wrapper around the 1password CLI client.
    private OnePasswordManager? _passwordManager;

    // A set of items loaded from the 1password CLI client.
    // This set is used to store the items that are loaded from the 1password CLI client.
    // The set is used to ensure that the items are unique and to provide fast lookups.
    // Also, the items only contain the data that is needed for the plugin to function, which, in this case, is the item's ID, name.
    private List<Item>? _items;
    private Dictionary<string, Vault>? _vaults;

    // A queue of vaults that are waiting to be loaded.
    private Queue<Vault> _vaultsQueue = new Queue<Vault>();


    // A flag that indicates an error occurred
    private bool _disabled = false;
    private string _disabledReason = "";


    // Initialization, settings update, and disposal methods
    public void Init(PluginInitContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _context.API.ThemeChanged += OnThemeChanged;
        
        UpdateIconPath(_context.API.GetCurrentTheme());

        _items = new List<Item>();
        _vaults = new Dictionary<string, Vault>();
        _vaultsQueue = new Queue<Vault>();

        InitializeConfiguration();

    }

    private void InitializeConfiguration()
    {
        if (!InitializePasswordManager()) return;
        if (!InitializeAccountHandling()) return;
        InitializeLazyVaults();
        InitializeItems();
    }

    private void InitializeLazyVaults()
    {
        var allVaults = _passwordManager.GetVaults();

        // Remove excluded vaults and the initial vault
        if (!string.IsNullOrEmpty(OnePasswordExcludeVault))
        {
            allVaults.RemoveAll(vault => vault.Name == OnePasswordExcludeVault || vault.Name == OnePasswordInitVault);
        }

        var initalVault = allVaults.FirstOrDefault(vault => vault.Name == OnePasswordInitVault);
        if (initalVault is not null)
        {
            _vaults.Add(initalVault.Id, initalVault);
        }

        // Queue remaining vaults for lazy loading
        foreach (var vault in allVaults)
        {
            _vaultsQueue.Enqueue(vault);
        }
    }

    private void InitializeItems()
    {
        if (OnePasswordPreloadFavorite) AddItemsFromVault(_passwordManager.SearchForItems(favorite: true));

        foreach(var vault in _vaults.Values)
        {
            AddItemsFromVault(_passwordManager.GetItems(vault));
        }

    }


    private void AddItemsFromVault(IEnumerable<Item> items)
    {
        foreach (var item in items)
        {
            if (!_items.Any(i => i.Id == item.Id || i?.Vault?.Name == OnePasswordExcludeVault))
            {
                _items.Add(item);
            }
        }
    }


    private void UpdateIconPath(Theme theme)
    {
        IconPath = theme is Theme.Light or Theme.HighContrastWhite ? "Images/_1PasswordPlugin.light.png" : "Images/_1PasswordPlugin.dark.png";
    }

    private void OnThemeChanged(Theme oldtheme, Theme newTheme)
    {
        UpdateIconPath(newTheme);
    }


    public List<Result> Query(Query query)
    {
        ArgumentNullException.ThrowIfNull(query);

        var results = new List<Result>();

        if (_disabled)
            return [new() {
            Title = "1password - has been disabled",
            SubTitle = _disabledReason,
        }];

        return results;
    }

    private void DisablePlugin(string reason)
    {
        _disabled = true;
        _disabledReason = reason;
    }



}
