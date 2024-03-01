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
    private HashSet<Item>? _items;
    private Dictionary<string, Vault>? _loadedVaults;

    // The vault that should be loaded when the plugin is initialized.
    private volatile bool _initialVaultsLoaded = false;


    // A queue of vaults that are waiting to be loaded.
    private ConcurrentQueue<Vault> _vaultsQueue = new ConcurrentQueue<Vault>();


    // Initialization, settings update, and disposal methods
    public void Init(PluginInitContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        //_context.CurrentPluginMetadata.Author = Properties.Resources.plugin_author;
        _context.API.ThemeChanged += OnThemeChanged;
        
        UpdateIconPath(_context.API.GetCurrentTheme());

        _items = new HashSet<Item>();
        _loadedVaults = new Dictionary<string, Vault>();

        ReloadData();

    }

    private void UpdateIconPath(Theme theme)
    {
        IconPath = theme is Theme.Light or Theme.HighContrastWhite ? "Images/_1PasswordPlugin.light.png" : "Images/_1PasswordPlugin.dark.png";
    }

    private void OnThemeChanged(Theme oldtheme, Theme newTheme)
    {
        UpdateIconPath(newTheme);
    }


    /// <summary>
    /// DO NOT USE
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public List<Result> Query(Query query)
    {
        ArgumentNullException.ThrowIfNull(query);

        var results = new List<Result>();
        return results;
    }


 }
