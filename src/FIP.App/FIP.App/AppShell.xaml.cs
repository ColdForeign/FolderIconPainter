// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using FIP.App.Helpers;
using FIP.App.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.ApplicationSettings;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FIP.App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppShell : Page
    {
        public AppShell()
        {
            this.InitializeComponent();

            Loaded += (sender, args) =>
            {
                NavView.SelectedItem = AllIconsMenuItem;
            };

            Loaded += delegate (object sender, RoutedEventArgs e)
            {
                NavigationOrientationHelper.UpdateTitleBarForElement(NavigationOrientationHelper.IsLeftMode(), this);
                WindowHelper.GetWindowForElement(this).Title = AppTitleText;
                var window = WindowHelper.GetWindowForElement(sender as UIElement);
                window.ExtendsContentIntoTitleBar = true;
                window.SetTitleBar(this.AppTitleBar);
            };
        }

        public static AppShell GetForElement(object obj)
        {
            UIElement element = (UIElement)obj;
            Window window = WindowHelper.GetWindowForElement(element);
            if (window != null)
            {
                return (AppShell)window.Content;
            }
            return null;
        }

        /// <summary>
        /// Gets the navigation frame instance.
        /// </summary>
        public Frame AppFrame => frame;

        public NavigationView NavigationView
        {
            get { return NavView; }
        }

        public string AppTitleText
        {
            get
            {
#if DEBUG
                return "Folder Icon Painter Dev";
#else
                return "Folder Icon Painter";
#endif
            }
        }

        /// <summary>
        /// Default keyboard focus movement for any unhandled keyboarding
        /// </summary>
        private void AppShell_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            FocusNavigationDirection direction = FocusNavigationDirection.None;
            switch (e.Key)
            {
                case VirtualKey.Left:
                case VirtualKey.GamepadDPadLeft:
                case VirtualKey.GamepadLeftThumbstickLeft:
                case VirtualKey.NavigationLeft:
                    direction = FocusNavigationDirection.Left;
                    break;
                case VirtualKey.Right:
                case VirtualKey.GamepadDPadRight:
                case VirtualKey.GamepadLeftThumbstickRight:
                case VirtualKey.NavigationRight:
                    direction = FocusNavigationDirection.Right;
                    break;

                case VirtualKey.Up:
                case VirtualKey.GamepadDPadUp:
                case VirtualKey.GamepadLeftThumbstickUp:
                case VirtualKey.NavigationUp:
                    direction = FocusNavigationDirection.Up;
                    break;

                case VirtualKey.Down:
                case VirtualKey.GamepadDPadDown:
                case VirtualKey.GamepadLeftThumbstickDown:
                case VirtualKey.NavigationDown:
                    direction = FocusNavigationDirection.Down;
                    break;
            }

            
            if (direction != FocusNavigationDirection.None &&
                FocusManager.FindNextFocusableElement(direction) is Control control)
            {
                control.Focus(FocusState.Keyboard);
                e.Handled = true;
            }
        }

        public readonly string AllIconsLabel = "All icons";

        public readonly string CreateCustomIconLabel = "Create custom icon";

        public readonly string AboutLabel = "About";

        /// <summary>
        /// Navigates to the page corresponding to the tapped item.
        /// </summary>
        private void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            var label = args.InvokedItem as string;
            var pageType =
                args.IsSettingsInvoked ? typeof(SettingsPage) :
                label == AllIconsLabel ? typeof(AllIconsPage) :
                label == CreateCustomIconLabel ? typeof(CreateCustomIconPage) :
                label == AboutLabel ? typeof(AboutPage) : null;
            if (pageType != null && pageType != AppFrame.CurrentSourcePageType)
            {
                AppFrame.Navigate(pageType);
            }
        }


        /// <summary>
        /// Ensures the nav menu reflects reality when navigation is triggered outside of
        /// the nav menu buttons.
        /// </summary>
        private void OnNavigatingToPage(object sender, NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                if (e.SourcePageType == typeof(AllIconsPage))
                {
                    NavView.SelectedItem = AllIconsMenuItem;
                }
                else if (e.SourcePageType == typeof(CreateCustomIconPage))
                {
                    NavView.SelectedItem = CreateCustomIconMenuItem;
                }
                else if (e.SourcePageType == typeof(SettingsPage))
                {
                    NavView.SelectedItem = NavView.SettingsItem;
                }
            }
        }

        /// <summary>
        /// Invoked when the View Code button is clicked. Launches the repo on GitHub. 
        /// </summary>
        private async void ViewCodeNavPaneButton_Tapped(object sender, TappedRoutedEventArgs e) =>
            await Launcher.LaunchUriAsync(new Uri(
                "https://github.com/FolderPainter/FolderIconPainter"));

        /// <summary>
        /// Navigates the frame to the previous page.
        /// </summary>
        private void NavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            if (AppFrame.CanGoBack)
            {
                AppFrame.GoBack();
            }
        }
    }
}
