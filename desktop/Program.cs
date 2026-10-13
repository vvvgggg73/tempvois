using System.Diagnostics;
using Microsoft.Web.WebView2.WinForms;

namespace Nocturne;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new PlayerWindow());
    }
}

internal sealed class PlayerWindow : Form
{
    private readonly WebView2 browser = new() { Dock = DockStyle.Fill };

    public PlayerWindow()
    {
        Text = "NOCTURNE";
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(1180, 800);
        MinimumSize = new Size(720, 560);
        BackColor = Color.FromArgb(7, 19, 33);
        Controls.Add(browser);
        Shown += LoadPlayer;
    }

    private async void LoadPlayer(object? sender, EventArgs args)
    {
        try
        {
            await browser.EnsureCoreWebView2Async();
            var core = browser.CoreWebView2;
            core.Settings.AreDevToolsEnabled = false;
            core.Settings.IsStatusBarEnabled = false;
            core.SetVirtualHostNameToFolderMapping(
                "nocturne.local",
                AppContext.BaseDirectory,
                Microsoft.Web.WebView2.Core.CoreWebView2HostResourceAccessKind.Allow);
            browser.Source = new Uri("https://nocturne.local/index.html");
        }
        catch (Exception exception)
        {
            var result = MessageBox.Show(
                this,
                "Для запуска NOCTURNE нужен Microsoft Edge WebView2 Runtime. Открыть страницу установки?\n\n" + exception.Message,
                "Не найден WebView2 Runtime",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                Process.Start(new ProcessStartInfo("https://developer.microsoft.com/microsoft-edge/webview2/")
                {
                    UseShellExecute = true
                });
            }
            Close();
        }
    }
}