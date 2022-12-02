// Licensed under the MIT License.

using FIP.App.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;

namespace FIP.App.UserControls
{
    public sealed partial class DropZone : UserControl
    {
        public DropZone()
        {
            this.InitializeComponent();
        }

        public Brush Color
        {
            get { return (Brush)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Color.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Brush), typeof(DropZone), new PropertyMetadata(0));

        private async void ZoneButton_Click(object sender, RoutedEventArgs e)
        {

            // Clear previous returned file name, if it exists, between iterations of this scenario
            ZoneButton.Content = "";

            // Create a folder picker
            FolderPicker openPicker = new FolderPicker();

            // Retrieve the window handle (HWND) of the current WinUI 3 window.
            var window = WindowHelper.GetWindowForElement(this);
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

            // Initialize the folder picker with the window handle (HWND).
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

            // Set options for your folder picker
            openPicker.SuggestedStartLocation = PickerLocationId.Desktop;
            openPicker.FileTypeFilter.Add("*");

            // Open the picker for the user to pick a folder
            StorageFolder folder = await openPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                ZoneButton.Content = "Picked folder: " + folder.Name;
            }
            else
            {
                ZoneButton.Content = "Operation cancelled.";
            }

        }

        private async void ZoneButton_Drop(object sender, DragEventArgs e)
        {

            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();
                if (items.Count > 0)
                {
                    foreach (var appFile in items)
                    {
                        ZoneTextBlock.Text += appFile.Path;
                        ZoneTextBlock.Text += '\n';
                    }
                }
            }
        }

        private void ZoneButton_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Move;

            if (e.DragUIOverride != null)
            {
                e.DragUIOverride.Caption = "Paint Folder!";
                e.DragUIOverride.IsContentVisible = true;
            }
        }
    }
}
