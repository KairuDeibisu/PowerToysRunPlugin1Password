// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using OnePassword.Vaults;
using OtpNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Input;
using Wox.Plugin;
using static System.Net.Mime.MediaTypeNames;



namespace Community.PowerToys.Run.Plugin._1Password;

public partial class Main : IContextMenu
{
    public List<ContextMenuResult> LoadContextMenus(Result selectedResult)
    {

        return selectedResult.ContextData is null || _disabled
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



                            EnhancedClipboard.CopyHelper(fieldValue ?? "Missing", PluginSettings.WindowsEnableHistory, PluginSettings.WindowsEnableRoaming);

                            return true;
                        },
                    },
                    new()
                    {
                        PluginName = Name,
                        Title = "Copy password to clipboard",
                        FontFamily = "Segoe Fluent Icons,Segoe MDL2 Assets",
                        Glyph = "\xF78D", // eye
                        AcceleratorKey = Key.X,
                        AcceleratorModifiers = ModifierKeys.Control,
                        Action = _ =>
                        {
                            if (selectedResult.ContextData is not string itemid || _passwordManager is null) {
                                return false;
                            }

                            var item = _passwordManager.SearchForItem(itemid);
                            var fieldvalue = item?.Fields.FirstOrDefault(field => field.Label == "password")?.Value;

                            EnhancedClipboard.CopyHelper(fieldvalue ?? "missing", PluginSettings.WindowsEnableHistory, PluginSettings.WindowsEnableRoaming);

                            return true;
                        },
                    },
                    new()
                    {
                        PluginName = Name,
                        Title = "Copy OTP to clipboard",
                        FontFamily = "Segoe Fluent Icons,Segoe MDL2 Assets",
                        Glyph = "\xEC92", // Clock
                        AcceleratorKey = Key.O,
                        AcceleratorModifiers = ModifierKeys.Control,
                        Action = _ =>
                        {
                            if (selectedResult.ContextData is not string itemId || _passwordManager is null) {
                                return false;
                            }

                            var item = _passwordManager.SearchForItem(itemId);
                            var otpUrl  = item?.Fields.FirstOrDefault(field => field.Label == "one-time password")?.Value;

                            if (otpUrl is null) {
                                System.Windows.Clipboard.SetText("Missing");
                                return true;
                            }

                                // Decode the URL
                               var uri = new Uri(otpUrl);
                               var label = Uri.UnescapeDataString(uri.AbsolutePath.Substring(1)); // Removing the leading '/'
                               var queryString = HttpUtility.ParseQueryString(uri.Query);
                               var secret = queryString["secret"];
                               var issuer = queryString["issuer"];

                               // Decode the base32 encoded secret
                               var secretBytes = Base32Encoding.ToBytes(secret);

                               // Create a TOTP object with the secret, default parameters are used here
                               var totp = new Totp(secretBytes);

                               // Generate a TOTP code
                               var code = totp.ComputeTotp();

                            EnhancedClipboard.CopyHelper(code  ?? "Missing", PluginSettings.WindowsEnableHistory, PluginSettings.WindowsEnableRoaming);

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


