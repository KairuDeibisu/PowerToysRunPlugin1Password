// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.PowerToys.Settings.UI.Library;
using OnePassword.Vaults;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wox.Plugin;

namespace Community.PowerToys.Run.Plugin._1Password
{
    public static class PluginSettings
    {
        public static string? OnePasswordInstallPath { get; set; }
        public static string? OnePasswordInitVault { get; set; }
        public static string? OnePasswordExcludeVault { get; set; }
        public static string? OnePasswordEmail { get; set; }
        public static bool OnePasswordPreloadFavorite { get; set; }
        public static bool WindowsEnableHistory { get; set; }
        public static bool WindowsEnableRoaming { get; set; }

        public static IEnumerable<PluginAdditionalOption> Options => new List<PluginAdditionalOption>
        {
            CreateOption(nameof(OnePasswordInstallPath), Properties.Resources.one_password_install_path, Properties.Resources.one_password_install_path_desc),
            CreateOption(nameof(OnePasswordInitVault), Properties.Resources.one_password_init_vault, Properties.Resources.one_password_init_vault_desc),
            CreateOption(nameof(OnePasswordExcludeVault), Properties.Resources.one_password_exlude_vault, Properties.Resources.one_password_exclude_vault_desc),
            CreateOption(nameof(OnePasswordEmail), Properties.Resources.one_password_email, Properties.Resources.one_password_email_desc),
            CreateOption(nameof(OnePasswordPreloadFavorite), Properties.Resources.one_password_preload_favorite, optionType: PluginAdditionalOption.AdditionalOptionType.Checkbox),
            CreateOption(nameof(WindowsEnableHistory), Properties.Resources.windows_enable_history,
                Properties.Resources.windows_enable_history_desc, PluginAdditionalOption.AdditionalOptionType.Checkbox),
            CreateOption(nameof(WindowsEnableRoaming), Properties.Resources.windows_enable_roaming,
                Properties.Resources.windows_enable_roaming_desc, PluginAdditionalOption.AdditionalOptionType.Checkbox)
        };

        public static void UpdateSettings(PowerLauncherPluginSettings settings)
        {
            OnePasswordInstallPath = GetSettingOrDefault<string>(settings, nameof(OnePasswordInstallPath));
            OnePasswordEmail = GetSettingOrDefault<string>(settings, nameof(OnePasswordEmail));
            OnePasswordInitVault = GetSettingOrDefault<string>(settings, nameof(OnePasswordInitVault));
            OnePasswordExcludeVault = GetSettingOrDefault<string>(settings, nameof(OnePasswordExcludeVault));
            OnePasswordPreloadFavorite = GetSettingOrDefault<bool>(settings, nameof(OnePasswordPreloadFavorite));
            WindowsEnableHistory = GetSettingOrDefault<bool>(settings, nameof(WindowsEnableHistory));
            WindowsEnableRoaming = GetSettingOrDefault<bool>(settings, nameof(WindowsEnableRoaming));
        }


        private static PluginAdditionalOption CreateOption(string key, string displayLabel, string displayDescription = "", PluginAdditionalOption.AdditionalOptionType optionType = PluginAdditionalOption.AdditionalOptionType.Textbox)
        {
            return new PluginAdditionalOption
            {
                Key = key,
                DisplayLabel = displayLabel,
                DisplayDescription = displayDescription,
                PluginOptionType = optionType
            };
        }

        private static T GetSettingOrDefault<T>(PowerLauncherPluginSettings settings, string key)
        {
            var defaultOption = Options.First(x => x.Key == key);
            var option = settings?.AdditionalOptions?.FirstOrDefault(x => x.Key == key);

            object value;

            switch (defaultOption.PluginOptionType)
            {
                case PluginAdditionalOption.AdditionalOptionType.Textbox:
                    value = option?.TextValue ?? defaultOption.TextValue;
                    break;
                case PluginAdditionalOption.AdditionalOptionType.Checkbox:
                    value = option?.Value ?? defaultOption.Value;
                    break;
                default:
                    throw new NotSupportedException();
            }

            return (T)Convert.ChangeType(value, typeof(T));
        }

    }
}
