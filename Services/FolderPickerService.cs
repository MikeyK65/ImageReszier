using System.Windows.Forms;

namespace ImageResize.Services;

public class FolderPickerService
{
    public Task<string?> PickFolderAsync(string? initialPath = null)
    {
        var tcs = new TaskCompletionSource<string?>();

        var thread = new Thread(() =>
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = "Select source folder",
                UseDescriptionForTitle = true,
                ShowNewFolderButton = false
            };

            if (!string.IsNullOrWhiteSpace(initialPath) && Directory.Exists(initialPath))
                dialog.InitialDirectory = initialPath;

            var result = dialog.ShowDialog();
            tcs.SetResult(result == DialogResult.OK ? dialog.SelectedPath : null);
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();

        return tcs.Task;
    }
}
