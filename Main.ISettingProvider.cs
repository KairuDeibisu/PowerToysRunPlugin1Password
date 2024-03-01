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



    public IEnumerable<PluginAdditionalOption> AdditionalOptions => new List<PluginAdditionalOption>()
    {
        new PluginAdditionalOption
        {
            Key = nameof(OnePasswordInstallPath),
            DisplayLabel = Properties.Resources.one_password_install_path,
            PluginOptionType = PluginAdditionalOption.AdditionalOptionType.Textbox,
        },
        new PluginAdditionalOption
        {
            Key = nameof(OnePasswordInitVault),
            DisplayLabel = Properties.Resources.one_password_init_vault,
            PluginOptionType = PluginAdditionalOption.AdditionalOptionType.Textbox,
        },
        new PluginAdditionalOption
        {
            Key = nameof(OnePasswordExcludeVault),
            DisplayLabel = Properties.Resources.one_password_exlude_vault,
            PluginOptionType = PluginAdditionalOption.AdditionalOptionType.Textbox,
        },
    };

    public void UpdateSettings(PowerLauncherPluginSettings settings)
    {
        OnePasswordInstallPath = (string)GetSettingOrDefault(settings, nameof(OnePasswordInstallPath));
        OnePasswordInitVault = (string)GetSettingOrDefault(settings, nameof(OnePasswordInitVault));
        OnePasswordExcludeVault = (string)GetSettingOrDefault(settings, nameof(OnePasswordExcludeVault));
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
        }

        throw new NotSupportedException();
    }

    public Control CreateSettingPanel()
    {
        throw new NotImplementedException();
    }

}
