// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Wox.Plugin;

namespace Community.PowerToys.Run.Plugin._1Password;

public partial class Main : IContextMenu
{
    public List<ContextMenuResult> LoadContextMenus(Result selectedResult)
    {
        return selectedResult.ContextData is null
            ? ([])
            : ([
                    new()
                    {
                        PluginName = Name,
                        Title = "Copy username to clipboard",
                        FontFamily = "Segoe Fluent Icons,Segoe MDL2 Assets",
                        Glyph = "\xE8C8", // Copy
                        AcceleratorKey = Key.C,
                        AcceleratorModifiers = ModifierKeys.Control,
                        Action = _ =>
                        {
                            if (selectedResult.ContextData is not string itemId || _passwordManager is null) {
                                return false;
                            }

                            var item = _passwordManager.SearchForItem(itemId);
                            var fieldValue = item?.Fields.FirstOrDefault(field => field.Label == "email" || field.Label == "username")?.Value;

                            System.Windows.Clipboard.SetText(fieldValue ?? "Missing");

                            return true;
                        },
                    },
                    new()
                    {
                        PluginName = Name,
                        Title = "Copy password to clipboard",
                        FontFamily = "Segoe Fluent Icons,Segoe MDL2 Assets",
                        Glyph = "\xE8C8", // Copy
                        AcceleratorKey = Key.X,
                        AcceleratorModifiers = ModifierKeys.Control,
                        Action = _ =>
                        {
                            if (selectedResult.ContextData is not string itemId || _passwordManager is null) {
                                return false;
                            }

                            var item = _passwordManager.SearchForItem(itemId);
                            var fieldValue = item?.Fields.FirstOrDefault(field => field.Label == "password")?.Value;

                            System.Windows.Clipboard.SetText(fieldValue ?? "Missing");

                            return true;
                        },
                    },
                    new()
                    {
                        PluginName = Name,
                        Title = "Reload Item",
                        FontFamily = "Segoe Fluent Icons,Segoe MDL2 Assets",
                        Glyph = "\xE72C", // Refresh
                        AcceleratorKey = Key.R,
                        AcceleratorModifiers = ModifierKeys.Control,
                        Action = _ =>
                        {
                            if (selectedResult.ContextData is not string itemId || _passwordManager is null) {
                                return false;
                            }

                            var item = _passwordManager.SearchForItem(itemId);

                            var old = _items?.FirstOrDefault(item => item.Id == itemId);
                            
                            if (old is null) {
                                return false;
                            }

                            if (item is null) {
                                _items?.Remove(old);
                                return true;
                            }

                            _items?.Remove(old);

                            _items?.Add(item);
                            
                            return true;
                        },
                    }
        ]);
    }

}
