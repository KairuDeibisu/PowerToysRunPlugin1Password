// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.PowerToys.Settings.UI.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Wox.Plugin;

namespace Community.PowerToys.Run.Plugin._1Password;

public partial class Main : ISettingProvider
{

    public string? OnePasswordInstallPath { get; set; }
    public string? OnePasswordInitVault { get; set; }

    public string? OnePasswordExcludeVault { get; set; }

    public bool OnePasswordPreloadFavorite { get; set; }
    public bool WindowsEnableHistory { get; set; }
    public bool WindowsEnableRoaming { get; set; }




    public IEnumerable<PluginAdditionalOption> AdditionalOptions => new List<PluginAdditionalOption>()
    {
        new PluginAdditionalOption
        {
            Key = nameof(OnePasswordInstallPath),
            DisplayLabel = Properties.Resources.one_password_install_path,
            DisplayDescription = Properties.Resources.one_password_install_path_desc,
            PluginOptionType = PluginAdditionalOption.AdditionalOptionType.Textbox,
        },
        new PluginAdditionalOption
        {
            Key = nameof(OnePasswordInitVault),
            DisplayLabel = Properties.Resources.one_password_init_vault,
            DisplayDescription = Properties.Resources.one_password_init_vault_desc,
            PluginOptionType = PluginAdditionalOption.AdditionalOptionType.Textbox,
        },
        new PluginAdditionalOption
        {
            Key = nameof(OnePasswordExcludeVault),
            DisplayLabel = Properties.Resources.one_password_exlude_vault,
            DisplayDescription = Properties.Resources.one_password_exclude_vault_desc,
            PluginOptionType = PluginAdditionalOption.AdditionalOptionType.Textbox,
        },
        new PluginAdditionalOption
        {
            Key = nameof(OnePasswordPreloadFavorite),
            DisplayLabel = Properties.Resources.one_password_preload_favorite,
            PluginOptionType = PluginAdditionalOption.AdditionalOptionType.Checkbox,
        },
        new PluginAdditionalOption
        {
            Key = nameof(WindowsEnableHistory),
            DisplayLabel = Properties.Resources.windows_enable_history,
            DisplayDescription = Properties.Resources.windows_enable_history_desc,
            PluginOptionType = PluginAdditionalOption.AdditionalOptionType.Checkbox,
        },
        new PluginAdditionalOption
        {
            Key = nameof(WindowsEnableRoaming),
            DisplayLabel = Properties.Resources.windows_enable_roaming,
            DisplayDescription = Properties.Resources.windows_enable_roaming_desc,
            PluginOptionType = PluginAdditionalOption.AdditionalOptionType.Checkbox,
        },
    };

    public void UpdateSettings(PowerLauncherPluginSettings settings)
    {
        OnePasswordInstallPath = (string)GetSettingOrDefault(settings, nameof(OnePasswordInstallPath));
        OnePasswordInitVault = (string)GetSettingOrDefault(settings, nameof(OnePasswordInitVault));
        OnePasswordExcludeVault = (string)GetSettingOrDefault(settings, nameof(OnePasswordExcludeVault));
        OnePasswordPreloadFavorite = (bool)GetSettingOrDefault(settings, nameof(OnePasswordPreloadFavorite));
        WindowsEnableHistory = (bool)GetSettingOrDefault(settings, nameof(WindowsEnableHistory));
        WindowsEnableRoaming = (bool)GetSettingOrDefault(settings, nameof(WindowsEnableRoaming));
    }

    private object GetSettingOrDefault(PowerLauncherPluginSettings settings, string key)
    {
        var defaultOptions = ((ISettingProvider)this).AdditionalOptions;
        var defaultOption = defaultOptions.First(x => x.Key == key);
        var option = settings?.AdditionalOptions?.FirstOrDefault(x => x.Key == key);

        switch (defaultOption.PluginOptionType)
        {
            case PluginAdditionalOption.AdditionalOptionType.Textbox:
                return option?.TextValue ?? defaultOption.TextValue;
            case PluginAdditionalOption.AdditionalOptionType.Checkbox:
                return option?.Value ?? defaultOption.Value;
        }

        throw new NotSupportedException();
    }

    public Control CreateSettingPanel()
    {
        throw new NotImplementedException();
    }

}
