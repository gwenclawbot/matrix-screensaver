using System.Text.Encodings.Web;
using System.Text.Json;
using System.Web;

namespace MatrixScreensaver;

internal sealed class SettingsManager
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public string SettingsDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "matrix-screensaver");
    public string SettingsFilePath => Path.Combine(SettingsDirectory, "settings.json");
    public static string LocalImagesDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "matrix-screensaver", "images");

    public ScreensaverSettings Load()
    {
        try
        {
            if (!File.Exists(SettingsFilePath))
            {
                return ScreensaverSettings.CreateDefault();
            }

            var json = File.ReadAllText(SettingsFilePath);
            var loaded = JsonSerializer.Deserialize<ScreensaverSettings>(json, JsonOptions);
            if (loaded is null)
            {
                return ScreensaverSettings.CreateDefault();
            }

            return MigrateIfNeeded(loaded);
        }
        catch
        {
            return ScreensaverSettings.CreateDefault();
        }
    }

    public void Save(ScreensaverSettings settings)
    {
        Directory.CreateDirectory(SettingsDirectory);
        var serialized = JsonSerializer.Serialize(settings, JsonOptions);
        File.WriteAllText(SettingsFilePath, serialized);
    }

    private static ScreensaverSettings MigrateIfNeeded(ScreensaverSettings input)
    {
        if (input.SchemaVersion < ScreensaverSettings.CurrentSchemaVersion)
        {
            input.SchemaVersion = ScreensaverSettings.CurrentSchemaVersion;
        }

        if (string.IsNullOrWhiteSpace(input.Preset))
        {
            input.Preset = "classic";
        }

        return input;
    }
}

internal sealed class ScreensaverSettings
{
    public const int CurrentSchemaVersion = 1;

    public int SchemaVersion { get; set; } = CurrentSchemaVersion;
    public string Preset { get; set; } = "classic";
    public int NumColumns { get; set; } = 80;
    public int AnimationSpeed { get; set; } = 100;
    public int Fps { get; set; } = 60;
    public int FallSpeed { get; set; } = 30;
    public decimal BloomStrength { get; set; } = 0.7m;
    public string Palette { get; set; } = "matrix";
    public string Renderer { get; set; } = "regl";
    public bool UseHalfFloat { get; set; } = true;
    public string BackgroundColor { get; set; } = "#000000";
    public string Effect { get; set; } = string.Empty;
    public string Font { get; set; } = "matrixcode";
    public string Camera { get; set; } = "false";
    public string GlyphRotation { get; set; } = "0";
    public string CursorIntensity { get; set; } = "2.0";
    public string DitherMagnitude { get; set; } = "0.05";
    public string ForwardSpeed { get; set; } = "0.25";
    public string Density { get; set; } = "1.0";
    public string Resolution { get; set; } = "0.75";
    public string RaindropLength { get; set; } = "0.75";
    public string CycleSpeed { get; set; } = "0.03";
    public string Slant { get; set; } = "0";
    public string Volumetric { get; set; } = "false";
    public string SkipIntro { get; set; } = "true";
    public string ImageUrl { get; set; } = string.Empty;
    public string CustomQuery { get; set; } = string.Empty;

    public static ScreensaverSettings CreateDefault() => new();

    public string ToQueryString()
    {
        var ic = System.Globalization.CultureInfo.InvariantCulture;
        var query = HttpUtility.ParseQueryString(string.Empty);

        // 'version' maps to upstream preset names (classic, resurrections, megacity, etc.)
        query["version"] = Preset;

        // Integer params — upstream uses them directly as integers.
        query["numColumns"] = NumColumns.ToString();
        query["fps"] = Fps.ToString();

        // Decimal multipliers — UI stores 1-500 as a percentage of the upstream default (1.0).
        // Divide by 100 to get the upstream-expected decimal (e.g. 100 → 1.0, 50 → 0.5).
        query["animationSpeed"] = (AnimationSpeed / 100m).ToString(ic);
        query["fallSpeed"] = (FallSpeed / 100m).ToString(ic);

        // bloomStrength is already stored as a decimal in the upstream's expected range.
        query["bloomStrength"] = BloomStrength.ToString(ic);

        // renderer is a plain string ("regl" or "webgpu").
        query["renderer"] = Renderer;

        if (!string.IsNullOrWhiteSpace(Effect))
        {
            query["effect"] = Effect;
        }

        if (!string.IsNullOrWhiteSpace(Font))
        {
            query["font"] = Font;
        }

        if (!string.IsNullOrWhiteSpace(Camera))
        {
            query["camera"] = Camera;
        }

        if (!string.IsNullOrWhiteSpace(GlyphRotation))
        {
            query["glyphRotation"] = GlyphRotation;
        }

        if (!string.IsNullOrWhiteSpace(CursorIntensity))
        {
            query["cursorIntensity"] = CursorIntensity;
        }

        if (!string.IsNullOrWhiteSpace(DitherMagnitude))
        {
            query["ditherMagnitude"] = DitherMagnitude;
        }

        if (!string.IsNullOrWhiteSpace(ForwardSpeed))
        {
            query["forwardSpeed"] = ForwardSpeed;
        }

        if (!string.IsNullOrWhiteSpace(Density))
        {
            query["density"] = Density;
        }

        if (!string.IsNullOrWhiteSpace(Resolution))
        {
            query["resolution"] = Resolution;
        }

        if (!string.IsNullOrWhiteSpace(RaindropLength))
        {
            query["raindropLength"] = RaindropLength;
        }

        if (!string.IsNullOrWhiteSpace(CycleSpeed))
        {
            query["cycleSpeed"] = CycleSpeed;
        }

        if (!string.IsNullOrWhiteSpace(Slant))
        {
            query["slant"] = Slant;
        }

        if (!string.IsNullOrWhiteSpace(Volumetric))
        {
            query["volumetric"] = Volumetric;
        }

        if (!string.IsNullOrWhiteSpace(SkipIntro))
        {
            query["skipIntro"] = SkipIntro;
        }

        if (string.Equals(Effect, "image", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(ImageUrl))
        {
            query["url"] = ImageUrl;
        }

        if (LooksLikeNumberTuple(Palette))
        {
            query["palette"] = Palette;
        }

        if (LooksLikeNumberTuple(BackgroundColor))
        {
            query["backgroundColor"] = BackgroundColor;
        }

        // Skip SwiftShader (software WebGL) warning — the screensaver will run regardless.
        query["suppressWarnings"] = "true";

        var composed = query.ToString() ?? string.Empty;

        if (!string.IsNullOrWhiteSpace(CustomQuery))
        {
            var custom = CustomQuery.Trim().TrimStart('?');
            composed = string.IsNullOrWhiteSpace(composed) ? custom : $"{composed}&{custom}";
        }

        return composed;
    }

    private static bool LooksLikeNumberTuple(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var trimmed = value.Trim();
        return trimmed.Contains(',');
    }
}
