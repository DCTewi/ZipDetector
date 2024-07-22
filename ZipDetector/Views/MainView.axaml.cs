using Avalonia.Controls;
using Avalonia.Input;

using System.IO;
using System.Linq;

using ZipDetector.Utils;

namespace ZipDetector.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();

        buttonChooseInput.Click += OnButtonChooseInputClicked;
        buttonChooseOutput.Click += OnButtonChooseOutputClicked;
        buttonStart.Click += OnButtonStartClicked;

        textInputPath.AddHandler(DragDrop.DropEvent, (sender, e) =>
        {
            if (e.Data.Contains(DataFormats.Files))
            {
                var files = e.Data.GetFiles() ?? [];
                if (files.Any())
                {
                    var result = files.First().Path.LocalPath;
                    textInputPath.Text = result;
                    textOutputPath.Text = GenerateDefaultFilename(result);
                }
            }
        });

        textOutputPath.AddHandler(DragDrop.DropEvent, (sender, e) =>
        {
            if (e.Data.Contains(DataFormats.Files))
            {
                var files = e.Data.GetFiles() ?? [];
                if (files.Any())
                {
                    textOutputPath.Text = files.First().Path.AbsolutePath;
                }
            }
        });
    }

    private async void OnButtonStartClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        labelMessage.Content = "transforming...";
        var message = await ZipDetectorUtil.DetectAsync(
            textInputPath.Text ?? string.Empty, textOutputPath.Text ?? string.Empty);
        labelMessage.Content = message;
    }

    private async void OnButtonChooseOutputClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var file = await TopLevel.GetTopLevel(this)!.StorageProvider.SaveFilePickerAsync(new()
        {
            Title = "select output file",
            DefaultExtension = "zip",
        });

        if (file is not null)
        {
            textOutputPath.Text = file.Path.LocalPath;
        }
    }

    private async void OnButtonChooseInputClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var file = await TopLevel.GetTopLevel(this)!.StorageProvider.OpenFilePickerAsync(new()
        {
            Title = "select input file",
            AllowMultiple = false,
        });

        if (file.Count > 0)
        {
            var result = file[0].Path.LocalPath;
            textInputPath.Text = result;
            textOutputPath.Text = GenerateDefaultFilename(result);
        }
    }

    private static string GenerateDefaultFilename(string path)
    {
        var result = $"{Directory.GetParent(path)}{Path.DirectorySeparatorChar}{Path.GetFileNameWithoutExtension(path)}_zipped.zip";
        return result;
    }
}
