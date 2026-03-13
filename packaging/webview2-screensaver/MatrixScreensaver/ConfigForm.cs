namespace MatrixScreensaver;

internal sealed class ConfigForm : Form
{
    private static readonly string[] KnownFonts =
    {
        "matrixcode",
        "resurrections",
        "gothic",
        "coptic",
        "huberfishA",
        "huberfishD",
        "megacity",
        "neomatrixology"
    };

    private static readonly string[] KnownPresets =
    {
        "classic",
        "3d",
        "resurrections",
        "trinity",
        "operator",
        "megacity",
        "nightmare",
        "paradise",
        "palimpsest",
        "twilight",
        "morpheus",
        "bugs",
        "neomatrixology"
    };

    private static readonly string SupportedVersionsText = string.Join(", ", KnownPresets);
    private static readonly ScreensaverSettings DefaultSettings = ScreensaverSettings.CreateDefault();
    private static readonly Dictionary<string, Dictionary<string, string>> PresetOverrides = new(StringComparer.OrdinalIgnoreCase)
    {
        ["classic"] = new(),
        ["3d"] = new()
        {
            ["volumetric"] = "true",
            ["fallSpeed"] = "0.5",
            ["cycleSpeed"] = "0.03",
            ["raindropLength"] = "0.3"
        },
        ["resurrections"] = new()
        {
            ["font"] = "resurrections",
            ["numColumns"] = "70",
            ["cycleSpeed"] = "0.03",
            ["bloomStrength"] = "0.7",
            ["fallSpeed"] = "0.3",
            ["cursorIntensity"] = "2"
        },
        ["trinity"] = new()
        {
            ["font"] = "resurrections",
            ["numColumns"] = "60",
            ["cycleSpeed"] = "0.01",
            ["bloomStrength"] = "0.7",
            ["fallSpeed"] = "0.3",
            ["cursorIntensity"] = "2",
            ["volumetric"] = "true",
            ["forwardSpeed"] = "0.2",
            ["raindropLength"] = "0.3",
            ["density"] = "0.75"
        },
        ["operator"] = new()
        {
            ["numColumns"] = "108",
            ["cycleSpeed"] = "0.01",
            ["bloomStrength"] = "0.75",
            ["fallSpeed"] = "0.6",
            ["raindropLength"] = "1.5",
            ["cursorIntensity"] = "3"
        },
        ["megacity"] = new()
        {
            ["font"] = "megacity",
            ["animationSpeed"] = "0.5",
            ["numColumns"] = "40"
        },
        ["nightmare"] = new()
        {
            ["font"] = "gothic",
            ["fallSpeed"] = "1.2",
            ["numColumns"] = "60",
            ["cycleSpeed"] = "0.35",
            ["raindropLength"] = "0.5",
            ["slant"] = "22.5"
        },
        ["paradise"] = new()
        {
            ["font"] = "coptic",
            ["bloomStrength"] = "1",
            ["cycleSpeed"] = "0.005",
            ["fallSpeed"] = "0.02",
            ["numColumns"] = "40",
            ["raindropLength"] = "0.4"
        },
        ["palimpsest"] = new()
        {
            ["font"] = "huberfishA",
            ["bloomStrength"] = "0.2",
            ["numColumns"] = "40",
            ["raindropLength"] = "1.2",
            ["fallSpeed"] = "0.5",
            ["slant"] = "-11.25"
        },
        ["twilight"] = new()
        {
            ["font"] = "huberfishD",
            ["cursorIntensity"] = "1.5",
            ["bloomStrength"] = "0.1",
            ["numColumns"] = "50",
            ["raindropLength"] = "0.9",
            ["fallSpeed"] = "0.1"
        },
        ["morpheus"] = new()
        {
            ["font"] = "resurrections",
            ["numColumns"] = "60",
            ["cycleSpeed"] = "0.015",
            ["bloomStrength"] = "0.7",
            ["fallSpeed"] = "0.3",
            ["cursorIntensity"] = "2",
            ["volumetric"] = "true",
            ["forwardSpeed"] = "0.1",
            ["raindropLength"] = "0.4",
            ["density"] = "0.75"
        },
        ["bugs"] = new()
        {
            ["font"] = "resurrections",
            ["numColumns"] = "60",
            ["cycleSpeed"] = "0.01",
            ["bloomStrength"] = "0.7",
            ["fallSpeed"] = "0.3",
            ["cursorIntensity"] = "2",
            ["volumetric"] = "true",
            ["forwardSpeed"] = "0.4",
            ["raindropLength"] = "0.3",
            ["density"] = "0.75"
        },
        ["neomatrixology"] = new()
        {
            ["font"] = "neomatrixology",
            ["animationSpeed"] = "0.8",
            ["numColumns"] = "40",
            ["cursorIntensity"] = "2"
        }
    };

    private static readonly Dictionary<string, string> BasePresetValues = new(StringComparer.OrdinalIgnoreCase)
    {
        ["numColumns"] = "80",
        ["animationSpeed"] = "1",
        ["fps"] = "60",
        ["fallSpeed"] = "0.3",
        ["bloomStrength"] = "0.7",
        ["renderer"] = "regl",
        ["font"] = "matrixcode",
        ["camera"] = "false",
        ["glyphRotation"] = "0",
        ["cursorIntensity"] = "2",
        ["ditherMagnitude"] = "0.05",
        ["forwardSpeed"] = "0.25",
        ["density"] = "1",
        ["resolution"] = "0.75",
        ["raindropLength"] = "0.75",
        ["cycleSpeed"] = "0.03",
        ["slant"] = "0",
        ["volumetric"] = "false",
        ["skipIntro"] = "true",
        ["palette"] = "",
        ["backgroundColor"] = "",
        ["effect"] = "",
        ["useHalfFloat"] = "true",
        ["imageUrl"] = ""
    };

    private readonly SettingsManager _settingsManager;
    private readonly ScreensaverSettings _settings;

    private readonly ComboBox _presetCombo = new();
    private readonly NumericUpDown _numColumns = new();
    private readonly NumericUpDown _animationSpeed = new();
    private readonly NumericUpDown _fps = new();
    private readonly NumericUpDown _fallSpeed = new();
    private readonly NumericUpDown _bloomStrength = new();
    private readonly TextBox _palette = new();
    private readonly TextBox _renderer = new();
    private readonly CheckBox _useHalfFloat = new();
    private readonly TextBox _backgroundColor = new();
    private readonly TextBox _effect = new();
    private readonly ComboBox _font = new();
    private readonly TextBox _camera = new();
    private readonly TextBox _glyphRotation = new();
    private readonly TextBox _cursorIntensity = new();
    private readonly TextBox _ditherMagnitude = new();
    private readonly TextBox _forwardSpeed = new();
    private readonly TextBox _density = new();
    private readonly TextBox _resolution = new();
    private readonly TextBox _raindropLength = new();
    private readonly TextBox _cycleSpeed = new();
    private readonly TextBox _slant = new();
    private readonly TextBox _volumetric = new();
    private readonly TextBox _skipIntro = new();
    private readonly TextBox _customQuery = new();
    private readonly TextBox _imageUrl = new();
    private readonly Button _browseImage = new() { Text = "Browse...", AutoSize = true };
    private readonly TextBox _developerQuery = new();
    private readonly Button _applyPreset = new() { Text = "Load preset values", AutoSize = true };

    public ConfigForm(SettingsManager settingsManager, ScreensaverSettings settings)
    {
        _settingsManager = settingsManager;
        _settings = settings;

        Text = "Matrix Screensaver Settings";
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        AutoSize = false;
        ClientSize = new Size(660, 760);
        MinimumSize = new Size(640, 680);

        var grid = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            Padding = new Padding(12),
            ColumnCount = 2
        };

        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 250));
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 360));

        var scrollPanel = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true
        };
        scrollPanel.Controls.Add(grid);

        _presetCombo.DropDownStyle = ComboBoxStyle.DropDown;
        _presetCombo.MaxDropDownItems = 20;
        _presetCombo.IntegralHeight = false;
        _presetCombo.Items.AddRange(KnownPresets.Cast<object>().ToArray());

        _font.DropDownStyle = ComboBoxStyle.DropDown;
        _font.MaxDropDownItems = 10;
        _font.Items.AddRange(KnownFonts.Cast<object>().ToArray());

        ConfigureNumeric(_numColumns, 10, 500, _settings.NumColumns);
        ConfigureNumeric(_animationSpeed, 1, 500, _settings.AnimationSpeed);
        ConfigureNumeric(_fps, 0, 60, _settings.Fps);
        ConfigureNumeric(_fallSpeed, 1, 500, _settings.FallSpeed);

        _bloomStrength.DecimalPlaces = 2;
        _bloomStrength.Increment = 0.05m;
        _bloomStrength.Minimum = 0;
        _bloomStrength.Maximum = 5;
        _bloomStrength.Value = _settings.BloomStrength;

        _palette.Text = _settings.Palette;
        _renderer.Text = _settings.Renderer;
        _useHalfFloat.Text = "Use half-float rendering";
        _useHalfFloat.Checked = _settings.UseHalfFloat;
        _backgroundColor.Text = _settings.BackgroundColor;
        _effect.Text = _settings.Effect;
        _font.Text = _settings.Font;
        _camera.Text = _settings.Camera;
        _glyphRotation.Text = _settings.GlyphRotation;
        _cursorIntensity.Text = _settings.CursorIntensity;
        _ditherMagnitude.Text = _settings.DitherMagnitude;
        _forwardSpeed.Text = _settings.ForwardSpeed;
        _density.Text = _settings.Density;
        _resolution.Text = _settings.Resolution;
        _raindropLength.Text = _settings.RaindropLength;
        _cycleSpeed.Text = _settings.CycleSpeed;
        _slant.Text = _settings.Slant;
        _volumetric.Text = _settings.Volumetric;
        _skipIntro.Text = _settings.SkipIntro;
        _customQuery.Text = _settings.CustomQuery;

        _palette.PlaceholderText = "R,G,B,% tuples (or leave as matrix default)";
        _renderer.PlaceholderText = "regl or webgpu";
        _backgroundColor.PlaceholderText = "R,G,B (0-1 each)";
        _effect.PlaceholderText = "plain|pride|stripes|none|image|mirror";
        // _font is a ComboBox — placeholder text not needed
        _camera.PlaceholderText = "true or false";
        _glyphRotation.PlaceholderText = "degrees (default 0)";
        _cursorIntensity.PlaceholderText = "> 0 (default 2.0)";
        _ditherMagnitude.PlaceholderText = "0-1 (default 0.05)";
        _forwardSpeed.PlaceholderText = "any number (default 1.0)";
        _density.PlaceholderText = "any number (default 1.0)";
        _resolution.PlaceholderText = "> 0 (default 1.0)";
        _raindropLength.PlaceholderText = "any number (default 1.0)";
        _cycleSpeed.PlaceholderText = "any number (default 1.0)";
        _slant.PlaceholderText = "degrees (default 0)";
        _volumetric.PlaceholderText = "true or false (default false)";
        _skipIntro.PlaceholderText = "true or false (default true)";
        _customQuery.PlaceholderText = "key=value&key2=value2";
        _imageUrl.PlaceholderText = "URL or local file path (used when effect=image)";

        _browseImage.Click += (_, _) =>
        {
            using var ofd = new OpenFileDialog
            {
                Title = "Select image for screensaver background",
                Filter = "Images|*.jpg;*.jpeg;*.png;*.gif;*.bmp;*.webp|All files|*.*"
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var imagesDir = SettingsManager.LocalImagesDirectory;
                    Directory.CreateDirectory(imagesDir);
                    var destName = Path.GetFileName(ofd.FileName);
                    var destPath = Path.Combine(imagesDir, destName);
                    File.Copy(ofd.FileName, destPath, overwrite: true);
                    _imageUrl.Text = WebViewHost.BuildImageUrl(destName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Could not copy image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        };

        _developerQuery.ReadOnly = true;
        _developerQuery.Multiline = true;
        _developerQuery.ScrollBars = ScrollBars.Vertical;
        _developerQuery.Height = 72;

        var imageUrlRow = new TableLayoutPanel
        {
            ColumnCount = 2,
            Dock = DockStyle.Fill,
            AutoSize = true
        };
        imageUrlRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        imageUrlRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        imageUrlRow.Controls.Add(_imageUrl, 0, 0);
        imageUrlRow.Controls.Add(_browseImage, 1, 0);

        var presetRow = new TableLayoutPanel
        {
            ColumnCount = 2,
            Dock = DockStyle.Fill,
            AutoSize = true
        };
        presetRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        presetRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        presetRow.Controls.Add(_presetCombo, 0, 0);
        presetRow.Controls.Add(_applyPreset, 1, 0);

        AddRow(grid, "Version (preset)", presetRow);
        AddRow(grid, "Supported", new Label
        {
            AutoSize = true,
            MaximumSize = new Size(360, 0),
            Text = SupportedVersionsText
        });
        AddRow(grid, "Columns (10-500, default 80)", _numColumns);
        AddRow(grid, "Animation speed (% of base, 1-500, default 100)", _animationSpeed);
        AddRow(grid, "FPS (0-60, default 60)", _fps);
        AddRow(grid, "Fall speed (% of base, 1-500, default 30)", _fallSpeed);
        AddRow(grid, "Bloom strength (0.00-5.00, default 0.70)", _bloomStrength);
        AddRow(grid, "Palette (R,G,B,% tuples)", _palette);
        AddRow(grid, "Renderer (regl or webgpu)", _renderer);
        AddRow(grid, "Background color (R,G,B tuple)", _backgroundColor);
        AddRow(grid, "Effect (plain|pride|stripes|none|image|mirror)", _effect);
        AddRow(grid, "Image URL (when effect=image)", imageUrlRow);
        AddRow(grid, "Font", _font);
        AddRow(grid, "Camera (true/false, default false)", _camera);
        AddRow(grid, "Glyph rotation (degrees, default 0)", _glyphRotation);
        AddRow(grid, "Cursor intensity (> 0, default 2.0)", _cursorIntensity);
        AddRow(grid, "Dither magnitude (default 0.05)", _ditherMagnitude);
        AddRow(grid, "Forward speed (default 0.25)", _forwardSpeed);
        AddRow(grid, "Density (default 1.0)", _density);
        AddRow(grid, "Resolution (> 0, default 0.75)", _resolution);
        AddRow(grid, "Raindrop length (default 0.75)", _raindropLength);
        AddRow(grid, "Cycle speed (default 0.03)", _cycleSpeed);
        AddRow(grid, "Slant (degrees, default 0)", _slant);
        AddRow(grid, "Volumetric (true/false, default false)", _volumetric);
        AddRow(grid, "Skip intro (true/false, default true)", _skipIntro);
        AddRow(grid, "Custom query (any key=value&...)", _customQuery);
        AddRow(grid, string.Empty, _useHalfFloat);
        AddRow(grid, "Developer query", _developerQuery);

        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            FlowDirection = FlowDirection.RightToLeft,
            AutoSize = true,
            Padding = new Padding(12)
        };

        var ok = new Button { Text = "Save", DialogResult = DialogResult.OK, AutoSize = true };
        var cancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, AutoSize = true };
        var defaults = new Button { Text = "Restore defaults", AutoSize = true };
        var about = new Button { Text = "About", AutoSize = true };

        defaults.Click += (_, _) => LoadSettings(ScreensaverSettings.CreateDefault());
        _applyPreset.Click += (_, _) => ApplyPresetValuesForSelection();
        about.Click += (_, _) => MessageBox.Show(
            "Matrix Screensaver\n\nBuilt on Rezmason/matrix (MIT).\nCtrl+O opens this dialog in /t mode.",
            "About",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
        ok.Click += (_, _) => SaveAndClose();

        buttons.Controls.Add(ok);
        buttons.Controls.Add(cancel);
        buttons.Controls.Add(defaults);
        buttons.Controls.Add(about);

        Controls.Add(scrollPanel);
        Controls.Add(buttons);

        AcceptButton = ok;
        CancelButton = cancel;

        LoadSettings(_settings);
        WireChangeEvents();
        RefreshDeveloperQuery();
    }

    private static void ConfigureNumeric(NumericUpDown control, int min, int max, int value)
    {
        control.Minimum = min;
        control.Maximum = max;
        control.Value = Math.Min(max, Math.Max(min, value));
    }

    private static void AddRow(TableLayoutPanel table, string label, Control control)
    {
        var row = table.RowCount++;
        table.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        table.Controls.Add(new Label
        {
            Text = label,
            AutoSize = true,
            MaximumSize = new Size(240, 0),
            TextAlign = ContentAlignment.MiddleLeft,
            Margin = new Padding(3, 8, 3, 3)
        }, 0, row);

        control.Dock = DockStyle.Fill;
        control.Margin = new Padding(3, 3, 3, 3);
        table.Controls.Add(control, 1, row);
    }

    private void WireChangeEvents()
    {
        foreach (var control in new Control[] { _presetCombo, _font, _numColumns, _animationSpeed, _fps, _fallSpeed, _bloomStrength, _palette, _renderer, _useHalfFloat, _backgroundColor, _effect, _imageUrl, _camera, _glyphRotation, _cursorIntensity, _ditherMagnitude, _forwardSpeed, _density, _resolution, _raindropLength, _cycleSpeed, _slant, _volumetric, _skipIntro, _customQuery })
        {
            switch (control)
            {
                case TextBox text:
                    text.TextChanged += (_, _) => RefreshDeveloperQuery();
                    break;
                case NumericUpDown numeric:
                    numeric.ValueChanged += (_, _) => RefreshDeveloperQuery();
                    break;
                case ComboBox combo:
                    combo.SelectedIndexChanged += (_, _) => RefreshDeveloperQuery();
                    break;
                case CheckBox check:
                    check.CheckedChanged += (_, _) => RefreshDeveloperQuery();
                    break;
            }
        }
    }

    private void LoadSettings(ScreensaverSettings settings)
    {
        _presetCombo.Text = string.IsNullOrWhiteSpace(settings.Preset) ? "classic" : settings.Preset;
        if (string.IsNullOrWhiteSpace(_presetCombo.Text))
        {
            _presetCombo.Text = "classic";
        }

        _numColumns.Value = Clamp(_numColumns, settings.NumColumns);
        _animationSpeed.Value = Clamp(_animationSpeed, settings.AnimationSpeed);
        _fps.Value = Clamp(_fps, settings.Fps);
        _fallSpeed.Value = Clamp(_fallSpeed, settings.FallSpeed);
        _bloomStrength.Value = Math.Max(_bloomStrength.Minimum, Math.Min(_bloomStrength.Maximum, settings.BloomStrength));
        SetTextOrDefault(_palette, settings.Palette, DefaultSettings.Palette);
        SetTextOrDefault(_renderer, settings.Renderer, DefaultSettings.Renderer);
        _useHalfFloat.Checked = settings.UseHalfFloat;
        SetTextOrDefault(_backgroundColor, settings.BackgroundColor, DefaultSettings.BackgroundColor);
        SetTextOrDefault(_effect, settings.Effect, DefaultSettings.Effect);
        _font.Text = string.IsNullOrWhiteSpace(settings.Font) ? DefaultSettings.Font : settings.Font;
        SetTextOrDefault(_camera, settings.Camera, DefaultSettings.Camera);
        SetTextOrDefault(_glyphRotation, settings.GlyphRotation, DefaultSettings.GlyphRotation);
        SetTextOrDefault(_cursorIntensity, settings.CursorIntensity, DefaultSettings.CursorIntensity);
        SetTextOrDefault(_ditherMagnitude, settings.DitherMagnitude, DefaultSettings.DitherMagnitude);
        SetTextOrDefault(_forwardSpeed, settings.ForwardSpeed, DefaultSettings.ForwardSpeed);
        SetTextOrDefault(_density, settings.Density, DefaultSettings.Density);
        SetTextOrDefault(_resolution, settings.Resolution, DefaultSettings.Resolution);
        SetTextOrDefault(_raindropLength, settings.RaindropLength, DefaultSettings.RaindropLength);
        SetTextOrDefault(_cycleSpeed, settings.CycleSpeed, DefaultSettings.CycleSpeed);
        SetTextOrDefault(_slant, settings.Slant, DefaultSettings.Slant);
        SetTextOrDefault(_volumetric, settings.Volumetric, DefaultSettings.Volumetric);
        SetTextOrDefault(_skipIntro, settings.SkipIntro, DefaultSettings.SkipIntro);
        _imageUrl.Text = settings.ImageUrl ?? string.Empty;
        _customQuery.Text = settings.CustomQuery;
    }

    private static void SetTextOrDefault(TextBox control, string? value, string? fallback)
    {
        control.Text = string.IsNullOrWhiteSpace(value) ? fallback ?? string.Empty : value;
    }

    private static decimal Clamp(NumericUpDown control, int value)
    {
        return Math.Max(control.Minimum, Math.Min(control.Maximum, value));
    }

    private void ApplyPresetValuesForSelection()
    {
        var presetName = _presetCombo.Text.Trim();
        if (string.IsNullOrWhiteSpace(presetName))
        {
            return;
        }

        var values = new Dictionary<string, string>(BasePresetValues, StringComparer.OrdinalIgnoreCase)
        {
            ["version"] = presetName
        };

        if (PresetOverrides.TryGetValue(presetName, out var overrides))
        {
            foreach (var (key, value) in overrides)
            {
                values[key] = value;
            }
        }

        _numColumns.Value = Clamp(_numColumns, ParseInt(values, "numColumns", (int)_numColumns.Value));
        _animationSpeed.Value = Clamp(_animationSpeed, (int)Math.Round(ParseDecimal(values, "animationSpeed", (decimal)_animationSpeed.Value / 100m) * 100m));
        _fps.Value = Clamp(_fps, ParseInt(values, "fps", (int)_fps.Value));
        _fallSpeed.Value = Clamp(_fallSpeed, (int)Math.Round(ParseDecimal(values, "fallSpeed", (decimal)_fallSpeed.Value / 100m) * 100m));
        _bloomStrength.Value = Math.Max(_bloomStrength.Minimum, Math.Min(_bloomStrength.Maximum, ParseDecimal(values, "bloomStrength", _bloomStrength.Value)));

        _renderer.Text = GetValue(values, "renderer", _renderer.Text);
        _font.Text = GetValue(values, "font", string.IsNullOrWhiteSpace(_font.Text) ? DefaultSettings.Font : _font.Text);
        _camera.Text = GetValue(values, "camera", _camera.Text);
        _glyphRotation.Text = GetValue(values, "glyphRotation", _glyphRotation.Text);
        _cursorIntensity.Text = GetValue(values, "cursorIntensity", _cursorIntensity.Text);
        _ditherMagnitude.Text = GetValue(values, "ditherMagnitude", _ditherMagnitude.Text);
        _forwardSpeed.Text = GetValue(values, "forwardSpeed", _forwardSpeed.Text);
        _density.Text = GetValue(values, "density", _density.Text);
        _resolution.Text = GetValue(values, "resolution", _resolution.Text);
        _raindropLength.Text = GetValue(values, "raindropLength", _raindropLength.Text);
        _cycleSpeed.Text = GetValue(values, "cycleSpeed", _cycleSpeed.Text);
        _slant.Text = GetValue(values, "slant", _slant.Text);
        _volumetric.Text = GetValue(values, "volumetric", _volumetric.Text);
        _skipIntro.Text = GetValue(values, "skipIntro", _skipIntro.Text);
        _palette.Text = GetValue(values, "palette", string.Empty);
        _backgroundColor.Text = GetValue(values, "backgroundColor", string.Empty);
        _effect.Text = GetValue(values, "effect", string.Empty);
        _imageUrl.Text = GetValue(values, "imageUrl", string.Empty);
        _useHalfFloat.Checked = string.Equals(GetValue(values, "useHalfFloat", "true"), "true", StringComparison.OrdinalIgnoreCase);
        _customQuery.Text = string.Empty;

        RefreshDeveloperQuery();
    }

    private static int ParseInt(IReadOnlyDictionary<string, string> values, string key, int fallback)
    {
        if (!values.TryGetValue(key, out var raw))
        {
            return fallback;
        }

        return int.TryParse(raw, out var parsed) ? parsed : fallback;
    }

    private static decimal ParseDecimal(IReadOnlyDictionary<string, string> values, string key, decimal fallback)
    {
        if (!values.TryGetValue(key, out var raw))
        {
            return fallback;
        }

        return decimal.TryParse(raw, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : fallback;
    }

    private static string GetValue(IReadOnlyDictionary<string, string> values, string key, string fallback)
    {
        return values.TryGetValue(key, out var value) ? value : fallback;
    }

    private ScreensaverSettings GatherSettings()
    {
        return new ScreensaverSettings
        {
            SchemaVersion = ScreensaverSettings.CurrentSchemaVersion,
            Preset = string.IsNullOrWhiteSpace(_presetCombo.Text) ? "classic" : _presetCombo.Text.Trim(),
            NumColumns = (int)_numColumns.Value,
            AnimationSpeed = (int)_animationSpeed.Value,
            Fps = (int)_fps.Value,
            FallSpeed = (int)_fallSpeed.Value,
            BloomStrength = _bloomStrength.Value,
            Palette = _palette.Text.Trim(),
            Renderer = _renderer.Text.Trim(),
            UseHalfFloat = _useHalfFloat.Checked,
            BackgroundColor = _backgroundColor.Text.Trim(),
            Effect = _effect.Text.Trim(),
            Font = string.IsNullOrWhiteSpace(_font.Text) ? DefaultSettings.Font : _font.Text.Trim(),
            Camera = _camera.Text.Trim(),
            GlyphRotation = _glyphRotation.Text.Trim(),
            CursorIntensity = _cursorIntensity.Text.Trim(),
            DitherMagnitude = _ditherMagnitude.Text.Trim(),
            ForwardSpeed = _forwardSpeed.Text.Trim(),
            Density = _density.Text.Trim(),
            Resolution = _resolution.Text.Trim(),
            RaindropLength = _raindropLength.Text.Trim(),
            CycleSpeed = _cycleSpeed.Text.Trim(),
            Slant = _slant.Text.Trim(),
            Volumetric = _volumetric.Text.Trim(),
            SkipIntro = _skipIntro.Text.Trim(),
            ImageUrl = _imageUrl.Text.Trim(),
            CustomQuery = _customQuery.Text.Trim()
        };
    }

    private void RefreshDeveloperQuery()
    {
        _developerQuery.Text = GatherSettings().ToQueryString();
    }

    private void SaveAndClose()
    {
        var updated = GatherSettings();
        _settingsManager.Save(updated);
        DialogResult = DialogResult.OK;
        Close();
    }
}
