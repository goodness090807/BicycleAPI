using System.Text;
using Serilog;

namespace BicycleAPI.Api.Extensions;

public static class CommandLineExtension
{
    public static void LogStartupInfo(WebApplication app, WebApplicationBuilder builder)
    {
        var urls = app.Urls.Any() ? string.Join(", ", app.Urls) : builder.Configuration["ASPNETCORE_URLS"]!;
        var baseUrl = urls.Split(',')[0].Trim();
        var useColor = SupportsAnsiColors();

        var lines = new List<(string Label, string Value, bool IsUrl)>
        {
            ("Application", builder.Environment.ApplicationName, false),
            ("Environment", builder.Environment.EnvironmentName, false),
            ("Listening", urls, false),
            ("", "", false),
            ("Scalar UI", $"{baseUrl}/scalar", true),
            ("Health", $"{baseUrl}/health", true)
        };

        if (app.Environment.IsDevelopment())
        {
            lines.Add(("OpenAPI", $"{baseUrl}/openapi/v1.json", true));
        }

        Log.Information(BuildStartupMessage(lines, useColor));
    }

    static string BuildStartupMessage(List<(string Label, string Value, bool IsUrl)> lines, bool useColor)
    {
        const int boxWidth = 60;
        var border = new string('-', boxWidth);

        var sb = new StringBuilder();
        sb.AppendLine();
        sb.AppendLine($"+{border}+");

        var title = useColor
            ? $"{Ansi.Green}{Ansi.Bold}APPLICATION STARTED{Ansi.Reset}"
            : "APPLICATION STARTED";
        const int titlePadding = boxWidth - 19;
        sb.AppendLine($"|  {title}{new string(' ', titlePadding - 2)}|");
        sb.AppendLine($"+{border}+");

        foreach (var (label, value, isUrl) in lines)
        {
            sb.AppendLine(FormatLine(label, value, isUrl, useColor, boxWidth, border));
        }

        sb.AppendLine($"+{border}+");
        return sb.ToString();
    }

    static string FormatLine(string label, string value, bool isUrl, bool useColor, int boxWidth, string border)
    {
        if (string.IsNullOrEmpty(label))
            return $"+{border}+";

        var displayLabel = $"  {label,-12}: ";
        var padding = boxWidth - displayLabel.Length - value.Length;

        if (!useColor)
            return $"|  {label,-12}: {value}{new string(' ', padding)}|";

        var coloredLabel = $"{Ansi.Cyan}{label}{Ansi.Reset}";
        var coloredValue = isUrl ? $"{Ansi.Yellow}{value}{Ansi.Reset}" : value;
        var content = $"  {coloredLabel.PadRight(12 + Ansi.Cyan.Length + Ansi.Reset.Length)}: {coloredValue}";
        return $"|{content}{new string(' ', padding)}|";
    }

    static bool SupportsAnsiColors()
    {
        // 如果輸出被重定向，不使用顏色
        if (Console.IsOutputRedirected)
            return false;

        // 遵守 NO_COLOR 標準 (https://no-color.org/)
        if (Environment.GetEnvironmentVariable("NO_COLOR") != null)
            return false;

        // Windows Terminal、VS Code 終端機、現代 PowerShell 都支援
        if (Environment.GetEnvironmentVariable("WT_SESSION") != null)
            return true;

        if (Environment.GetEnvironmentVariable("TERM_PROGRAM") == "vscode")
            return true;

        // 檢查 TERM 環境變數
        var term = Environment.GetEnvironmentVariable("TERM");
        if (!string.IsNullOrEmpty(term) &&
            (term.Contains("color") || term.Contains("xterm") || term.Contains("256")))
            return true;

        // Windows 10+ 的 conhost 也支援 ANSI
        if (OperatingSystem.IsWindows() && Environment.OSVersion.Version.Major >= 10)
            return true;

        return false;
    }

    static class Ansi
    {
        public const string Reset = "\x1b[0m";
        public const string Bold = "\x1b[1m";
        public const string Green = "\x1b[32m";
        public const string Yellow = "\x1b[33m";
        public const string Cyan = "\x1b[36m";
    }
}
