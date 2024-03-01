// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wox.Plugin;

namespace Community.PowerToys.Run.Plugin._1Password;

public partial class Main : IPluginI18n
{
    public string GetTranslatedPluginTitle()
    {
        return Properties.Resources.plugin_name;
    }

    public string GetTranslatedPluginDescription()
    {
        return Properties.Resources.plugin_description;
    }
}
