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

    public IEnumerable<PluginAdditionalOption> AdditionalOptions => PluginSettings.Options;

    public void UpdateSettings(PowerLauncherPluginSettings settings)
    {
        PluginSettings.UpdateSettings(settings);
        _disabled = false;
    }

    public Control CreateSettingPanel()
    {
        throw new NotImplementedException();
    }

}
