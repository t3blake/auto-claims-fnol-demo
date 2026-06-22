using Microsoft.Data.Sqlite;
using Microsoft.Win32;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace AutoClaimsFnolApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private const string AdjusterUser = "adjuster1";
    private const string AdjusterPassword = "pass123";
    private const string AdminUser = "admin";
    private const string AdminPassword = "admin";

    private class ClaimImage
    {
        public string FilePath { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public override string ToString() => $"{Description} ({Path.GetFileName(FilePath)})";
    }

    private readonly string _dbPath;
    private string _currentUser = string.Empty;
    private string _currentRole = string.Empty;
    private int _loginAttempts;
    private int _currentStep = 1;
    private readonly List<ClaimImage> _currentImages = new();

    public MainWindow()
    {
        InitializeComponent();

        _dbPath = Path.Combine(AppContext.BaseDirectory, "claims-fnol.db");
        InitializeDatabase();
        ResetWizard();
        ShowLogin();
    }

    private void InitializeDatabase()
    {
        using var connection = new SqliteConnection($"Data Source={_dbPath}");
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
CREATE TABLE IF NOT EXISTS Claims (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    ClaimNumber TEXT NOT NULL,
    ClaimantName TEXT NOT NULL,
    ClaimantPhone TEXT NOT NULL,
    ClaimantEmail TEXT,
    PolicyNumber TEXT,
    IncidentDate TEXT NOT NULL,
    IncidentTime TEXT,
    IncidentLocation TEXT NOT NULL,
    IncidentType TEXT NOT NULL,
    Weather TEXT NOT NULL,
    Road TEXT NOT NULL,
    PoliceReportFiled TEXT NOT NULL,
    PoliceReportNumber TEXT,
    ImageFileName TEXT,
    ImageDescriptions TEXT,
    ImageIncidentType TEXT NOT NULL,
    ImageVehicleCount INTEGER NOT NULL,
    ImpactType TEXT NOT NULL,
    Vehicle1Position TEXT NOT NULL,
    Vehicle1Direction TEXT NOT NULL,
    Vehicle2Position TEXT,
    Vehicle2Direction TEXT,
    DamageZones TEXT NOT NULL,
    RoadFactors TEXT NOT NULL,
    Confidence TEXT NOT NULL,
    Assumptions TEXT,
    PrimaryMake TEXT,
    PrimaryModel TEXT,
    PrimaryYear TEXT,
    PrimaryDamage TEXT,
    MultiVehicle TEXT NOT NULL,
    OtherMake TEXT,
    OtherModel TEXT,
    OtherDamage TEXT,
    InjuriesReported TEXT NOT NULL,
    WitnessPresent TEXT NOT NULL,
    WitnessName TEXT,
    Status TEXT NOT NULL,
    CreatedAt TEXT NOT NULL
);
";
        command.ExecuteNonQuery();

        // Migration: Add ImageDescriptions column if it doesn't exist
        var checkCmd = connection.CreateCommand();
        checkCmd.CommandText = "PRAGMA table_info(Claims);";
        var hasImageDescriptions = false;
        using (var reader = checkCmd.ExecuteReader())
        {
            while (reader.Read())
            {
                var columnName = reader.GetString(1);
                if (columnName == "ImageDescriptions")
                {
                    hasImageDescriptions = true;
                    break;
                }
            }
        }

        if (!hasImageDescriptions)
        {
            var alterCmd = connection.CreateCommand();
            alterCmd.CommandText = "ALTER TABLE Claims ADD COLUMN ImageDescriptions TEXT;";
            try
            {
                alterCmd.ExecuteNonQuery();
            }
            catch
            {
                // Column might already exist or other issue, continue gracefully
            }
        }
    }

    private void ShowOnlyPanel(UIElement panel)
    {
        LoginPanel.Visibility = Visibility.Collapsed;
        MainMenuPanel.Visibility = Visibility.Collapsed;
        NewClaimPanel.Visibility = Visibility.Collapsed;
        ConfirmationPanel.Visibility = Visibility.Collapsed;
        SearchPanel.Visibility = Visibility.Collapsed;
        AdminPanel.Visibility = Visibility.Collapsed;

        panel.Visibility = Visibility.Visible;
    }

    private void ShowLogin()
    {
        ShowOnlyPanel(LoginPanel);
        LoginStatusText.Text = string.Empty;
        UsernameTextBox.Focus();
    }

    private void ShowMainMenu()
    {
        ShowOnlyPanel(MainMenuPanel);
        LoggedInAsText.Text = $"Logged in as: {_currentUser} ({_currentRole})";
        AdminButton.IsEnabled = _currentRole == "Admin";
    }

    private void ShowWizard()
    {
        ShowOnlyPanel(NewClaimPanel);
        RenderWizardStep();
    }

    private void RenderWizardStep()
    {
        Step1Panel.Visibility = _currentStep == 1 ? Visibility.Visible : Visibility.Collapsed;
        Step2Panel.Visibility = _currentStep == 2 ? Visibility.Visible : Visibility.Collapsed;
        Step3Panel.Visibility = _currentStep == 3 ? Visibility.Visible : Visibility.Collapsed;
        Step4Panel.Visibility = _currentStep == 4 ? Visibility.Visible : Visibility.Collapsed;
        Step5Panel.Visibility = _currentStep == 5 ? Visibility.Visible : Visibility.Collapsed;
        Step6Panel.Visibility = _currentStep == 6 ? Visibility.Visible : Visibility.Collapsed;

        WizardValidationText.Text = string.Empty;
        WizardTitleText.Text = _currentStep switch
        {
            1 => "NEW CLAIM - Page 1 of 6: Claimant Information",
            2 => "NEW CLAIM - Page 2 of 6: Incident Details",
            3 => "NEW CLAIM - Page 3 of 6: Upload Accident Image",
            4 => "NEW CLAIM - Page 4 of 6: Image Analysis & Interpretation",
            5 => "NEW CLAIM - Page 5 of 6: Vehicle & Injury Information",
            _ => "NEW CLAIM - Page 6 of 6: Review & Submit"
        };

        BackStepButton.IsEnabled = _currentStep > 1;
        NextStepButton.Visibility = _currentStep < 6 ? Visibility.Visible : Visibility.Collapsed;
        SubmitClaimButton.Visibility = _currentStep == 6 ? Visibility.Visible : Visibility.Collapsed;

        if (_currentStep == 6)
        {
            ReviewSummaryTextBox.Text = BuildReviewSummary();
        }
    }

    private void ResetWizard()
    {
        _currentStep = 1;
        _currentImages.Clear();

        ClaimantNameTextBox.Text = string.Empty;
        ClaimantPhoneTextBox.Text = string.Empty;
        ClaimantEmailTextBox.Text = string.Empty;
        PolicyNumberTextBox.Text = string.Empty;

        IncidentDatePicker.SelectedDate = DateTime.Today;
        IncidentTimeTextBox.Text = string.Empty;
        IncidentLocationTextBox.Text = string.Empty;
        IncidentTypeComboBox.SelectedIndex = -1;
        WeatherComboBox.SelectedIndex = -1;
        RoadComboBox.SelectedIndex = -1;
        PoliceYesRadio.IsChecked = false;
        PoliceNoRadio.IsChecked = false;
        PoliceReportTextBox.Text = string.Empty;

        UploadedImagePreview.Source = null;
        ImageDescriptionTextBox.Text = string.Empty;
        RefreshImagesList();

        ImageIncidentTypeComboBox.SelectedIndex = -1;
        ImageVehicleCountTextBox.Text = string.Empty;
        ImpactTypeComboBox.SelectedIndex = -1;
        Vehicle1PositionComboBox.SelectedIndex = -1;
        Vehicle1DirectionComboBox.SelectedIndex = -1;
        Vehicle2PositionComboBox.SelectedIndex = -1;
        Vehicle2DirectionComboBox.SelectedIndex = -1;
        ConfidenceComboBox.SelectedIndex = -1;
        AssumptionsTextBox.Text = string.Empty;

        DamageFrontCheckBox.IsChecked = false;
        DamageRearCheckBox.IsChecked = false;
        DamageDriverCheckBox.IsChecked = false;
        DamagePassengerCheckBox.IsChecked = false;
        DamageRoofCheckBox.IsChecked = false;
        DamageUnderCheckBox.IsChecked = false;

        FactorIntersectionCheckBox.IsChecked = false;
        FactorLaneMergeCheckBox.IsChecked = false;
        FactorParkedCheckBox.IsChecked = false;
        FactorMedianCheckBox.IsChecked = false;
        FactorGravelCheckBox.IsChecked = false;
        FactorWetCheckBox.IsChecked = false;

        PrimaryMakeTextBox.Text = string.Empty;
        PrimaryModelTextBox.Text = string.Empty;
        PrimaryYearTextBox.Text = string.Empty;
        PrimaryDamageComboBox.SelectedIndex = -1;
        MultiVehicleCheckBox.IsChecked = false;
        OtherMakeTextBox.Text = string.Empty;
        OtherModelTextBox.Text = string.Empty;
        OtherDamageComboBox.SelectedIndex = -1;
        InjuriesYesRadio.IsChecked = false;
        InjuriesNoRadio.IsChecked = false;
        WitnessYesRadio.IsChecked = false;
        WitnessNoRadio.IsChecked = false;
        WitnessNameTextBox.Text = string.Empty;
    }

    private static string ComboValue(ComboBox box)
    {
        return (box.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? string.Empty;
    }

    private static bool HasDigits(string value)
    {
        foreach (var ch in value)
        {
            if (char.IsDigit(ch))
            {
                return true;
            }
        }

        return false;
    }

    private bool ValidateCurrentStep(out string message)
    {
        message = string.Empty;

        if (_currentStep == 1)
        {
            if (string.IsNullOrWhiteSpace(ClaimantNameTextBox.Text))
            {
                message = "Claimant full name is required.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(ClaimantPhoneTextBox.Text) || !HasDigits(ClaimantPhoneTextBox.Text))
            {
                message = "Valid claimant phone number is required.";
                return false;
            }
        }

        if (_currentStep == 2)
        {
            if (!IncidentDatePicker.SelectedDate.HasValue)
            {
                message = "Incident date is required.";
                return false;
            }

            if (IncidentDatePicker.SelectedDate.Value.Date > DateTime.Today)
            {
                message = "Incident date cannot be in the future.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(IncidentLocationTextBox.Text))
            {
                message = "Incident location is required.";
                return false;
            }

            if (IncidentTypeComboBox.SelectedIndex < 0 || WeatherComboBox.SelectedIndex < 0 || RoadComboBox.SelectedIndex < 0)
            {
                message = "Incident type, weather, and road conditions are required.";
                return false;
            }

            if (PoliceYesRadio.IsChecked != true && PoliceNoRadio.IsChecked != true)
            {
                message = "Police report filed (Yes/No) is required.";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(IncidentTimeTextBox.Text) && !TimeSpan.TryParseExact(IncidentTimeTextBox.Text, "hh\\:mm", CultureInfo.InvariantCulture, out _))
            {
                message = "Incident time must be in HH:MM format (24-hour).";
                return false;
            }
        }

        if (_currentStep == 3)
        {
            if (_currentImages.Count == 0)
            {
                message = "At least one image is required before proceeding to analysis.";
                return false;
            }
        }

        if (_currentStep == 4)
        {
            if (_currentImages.Count == 0)
            {
                message = "Image analysis requires uploaded images. Go back to Page 3 and upload file(s).";
                return false;
            }

            if (ImageIncidentTypeComboBox.SelectedIndex < 0 || ImpactTypeComboBox.SelectedIndex < 0 ||
                Vehicle1PositionComboBox.SelectedIndex < 0 || Vehicle1DirectionComboBox.SelectedIndex < 0 ||
                ConfidenceComboBox.SelectedIndex < 0)
            {
                message = "Complete all required image analysis dropdown fields.";
                return false;
            }

            if (!int.TryParse(ImageVehicleCountTextBox.Text, out var vehicleCount) || vehicleCount < 1)
            {
                message = "Image vehicle count must be a number >= 1.";
                return false;
            }

            if (ComboValue(ImageIncidentTypeComboBox) == "Multi-Vehicle" &&
                (Vehicle2PositionComboBox.SelectedIndex < 0 || Vehicle2DirectionComboBox.SelectedIndex < 0))
            {
                message = "Vehicle 2 position and direction are required for Multi-Vehicle incidents.";
                return false;
            }

            var damageChecked = DamageFrontCheckBox.IsChecked == true || DamageRearCheckBox.IsChecked == true ||
                                DamageDriverCheckBox.IsChecked == true || DamagePassengerCheckBox.IsChecked == true ||
                                DamageRoofCheckBox.IsChecked == true || DamageUnderCheckBox.IsChecked == true;
            if (!damageChecked)
            {
                message = "At least one damage zone must be checked.";
                return false;
            }

            var factorsChecked = FactorIntersectionCheckBox.IsChecked == true || FactorLaneMergeCheckBox.IsChecked == true ||
                                 FactorParkedCheckBox.IsChecked == true || FactorMedianCheckBox.IsChecked == true ||
                                 FactorGravelCheckBox.IsChecked == true || FactorWetCheckBox.IsChecked == true;
            if (!factorsChecked)
            {
                message = "At least one road/scene factor must be checked.";
                return false;
            }
        }

        if (_currentStep == 5)
        {
            if (InjuriesYesRadio.IsChecked != true && InjuriesNoRadio.IsChecked != true)
            {
                message = "Injuries reported (Yes/No) is required.";
                return false;
            }

            if (WitnessYesRadio.IsChecked != true && WitnessNoRadio.IsChecked != true)
            {
                message = "Witness present (Yes/No) is required.";
                return false;
            }
        }

        return true;
    }

    private static string BuildCommaList(params (string Name, bool Selected)[] checks)
    {
        var values = new List<string>();
        foreach (var check in checks)
        {
            if (check.Selected)
            {
                values.Add(check.Name);
            }
        }

        return string.Join(", ", values);
    }

    private string BuildReviewSummary()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Claimant: {ClaimantNameTextBox.Text}");
        sb.AppendLine($"Phone: {ClaimantPhoneTextBox.Text}");
        sb.AppendLine($"Email: {ClaimantEmailTextBox.Text}");
        sb.AppendLine($"Policy: {PolicyNumberTextBox.Text}");
        sb.AppendLine();
        sb.AppendLine($"Incident Date: {IncidentDatePicker.SelectedDate:yyyy-MM-dd}");
        sb.AppendLine($"Incident Time: {IncidentTimeTextBox.Text}");
        sb.AppendLine($"Location: {IncidentLocationTextBox.Text}");
        sb.AppendLine($"Incident Type: {ComboValue(IncidentTypeComboBox)}");
        sb.AppendLine($"Weather: {ComboValue(WeatherComboBox)}");
        sb.AppendLine($"Road: {ComboValue(RoadComboBox)}");
        sb.AppendLine($"Police Report Filed: {(PoliceYesRadio.IsChecked == true ? "Yes" : "No")}");
        sb.AppendLine($"Police Report Number: {PoliceReportTextBox.Text}");
        sb.AppendLine();
        var imageStr = _currentImages.Count > 0
            ? string.Join(", ", _currentImages.Select(i => $"{i.Description} ({Path.GetFileName(i.FilePath)})"))
            : "(No images)";
        sb.AppendLine($"Images: {imageStr}");
        sb.AppendLine($"Image Incident Type: {ComboValue(ImageIncidentTypeComboBox)}");
        sb.AppendLine($"Vehicle Count: {ImageVehicleCountTextBox.Text}");
        sb.AppendLine($"Impact Type: {ComboValue(ImpactTypeComboBox)}");
        sb.AppendLine($"Vehicle 1 Position/Direction: {ComboValue(Vehicle1PositionComboBox)} / {ComboValue(Vehicle1DirectionComboBox)}");
        sb.AppendLine($"Vehicle 2 Position/Direction: {ComboValue(Vehicle2PositionComboBox)} / {ComboValue(Vehicle2DirectionComboBox)}");
        sb.AppendLine($"Damage Zones: {BuildCommaList(("Front", DamageFrontCheckBox.IsChecked == true), ("Rear", DamageRearCheckBox.IsChecked == true), ("Driver Side", DamageDriverCheckBox.IsChecked == true), ("Passenger Side", DamagePassengerCheckBox.IsChecked == true), ("Roof", DamageRoofCheckBox.IsChecked == true), ("Undercarriage", DamageUnderCheckBox.IsChecked == true))}");
        sb.AppendLine($"Road Factors: {BuildCommaList(("Intersection", FactorIntersectionCheckBox.IsChecked == true), ("Lane Merge", FactorLaneMergeCheckBox.IsChecked == true), ("Parked Vehicle", FactorParkedCheckBox.IsChecked == true), ("Median/Divider", FactorMedianCheckBox.IsChecked == true), ("Gravel/Dirt", FactorGravelCheckBox.IsChecked == true), ("Wet Surface", FactorWetCheckBox.IsChecked == true))}");
        sb.AppendLine($"Confidence: {ComboValue(ConfidenceComboBox)}");
        sb.AppendLine($"Assumptions: {AssumptionsTextBox.Text}");
        sb.AppendLine();
        sb.AppendLine($"Primary Vehicle: {PrimaryMakeTextBox.Text} {PrimaryModelTextBox.Text} {PrimaryYearTextBox.Text} ({ComboValue(PrimaryDamageComboBox)})");
        sb.AppendLine($"Multi-Vehicle: {(MultiVehicleCheckBox.IsChecked == true ? "Yes" : "No")}");
        sb.AppendLine($"Other Vehicle: {OtherMakeTextBox.Text} {OtherModelTextBox.Text} ({ComboValue(OtherDamageComboBox)})");
        sb.AppendLine($"Injuries Reported: {(InjuriesYesRadio.IsChecked == true ? "Yes" : "No")}");
        sb.AppendLine($"Witness Present: {(WitnessYesRadio.IsChecked == true ? "Yes" : "No")}");
        sb.AppendLine($"Witness Name: {WitnessNameTextBox.Text}");
        return sb.ToString();
    }

    private string GenerateClaimNumber()
    {
        var now = DateTime.Now;
        return $"CLM-{now:yyyy}-{now:MMddHHmmss}";
    }

    private void SaveClaim()
    {
        try
        {
            var claimNumber = GenerateClaimNumber();
            var damageZones = BuildCommaList(("Front", DamageFrontCheckBox.IsChecked == true), ("Rear", DamageRearCheckBox.IsChecked == true), ("Driver Side", DamageDriverCheckBox.IsChecked == true), ("Passenger Side", DamagePassengerCheckBox.IsChecked == true), ("Roof", DamageRoofCheckBox.IsChecked == true), ("Undercarriage", DamageUnderCheckBox.IsChecked == true));
            var roadFactors = BuildCommaList(("Intersection", FactorIntersectionCheckBox.IsChecked == true), ("Lane Merge", FactorLaneMergeCheckBox.IsChecked == true), ("Parked Vehicle", FactorParkedCheckBox.IsChecked == true), ("Median/Divider", FactorMedianCheckBox.IsChecked == true), ("Gravel/Dirt", FactorGravelCheckBox.IsChecked == true), ("Wet Surface", FactorWetCheckBox.IsChecked == true));

            var imageDescriptions = _currentImages.Count > 0
                ? string.Join("|", _currentImages.Select(i => $"{i.Description}:{Path.GetFileName(i.FilePath)}"))
                : string.Empty;

            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"
INSERT INTO Claims (
    ClaimNumber, ClaimantName, ClaimantPhone, ClaimantEmail, PolicyNumber,
    IncidentDate, IncidentTime, IncidentLocation, IncidentType, Weather, Road,
    PoliceReportFiled, PoliceReportNumber, ImageFileName, ImageDescriptions, ImageIncidentType, ImageVehicleCount,
    ImpactType, Vehicle1Position, Vehicle1Direction, Vehicle2Position, Vehicle2Direction,
    DamageZones, RoadFactors, Confidence, Assumptions,
    PrimaryMake, PrimaryModel, PrimaryYear, PrimaryDamage,
    MultiVehicle, OtherMake, OtherModel, OtherDamage,
    InjuriesReported, WitnessPresent, WitnessName, Status, CreatedAt)
VALUES (
    $ClaimNumber, $ClaimantName, $ClaimantPhone, $ClaimantEmail, $PolicyNumber,
    $IncidentDate, $IncidentTime, $IncidentLocation, $IncidentType, $Weather, $Road,
    $PoliceReportFiled, $PoliceReportNumber, $ImageFileName, $ImageDescriptions, $ImageIncidentType, $ImageVehicleCount,
    $ImpactType, $Vehicle1Position, $Vehicle1Direction, $Vehicle2Position, $Vehicle2Direction,
    $DamageZones, $RoadFactors, $Confidence, $Assumptions,
    $PrimaryMake, $PrimaryModel, $PrimaryYear, $PrimaryDamage,
    $MultiVehicle, $OtherMake, $OtherModel, $OtherDamage,
    $InjuriesReported, $WitnessPresent, $WitnessName, 'Submitted for Review', $CreatedAt);
";

            command.Parameters.AddWithValue("$ClaimNumber", claimNumber);
            command.Parameters.AddWithValue("$ClaimantName", ClaimantNameTextBox.Text.Trim());
            command.Parameters.AddWithValue("$ClaimantPhone", ClaimantPhoneTextBox.Text.Trim());
            command.Parameters.AddWithValue("$ClaimantEmail", ClaimantEmailTextBox.Text.Trim());
            command.Parameters.AddWithValue("$PolicyNumber", PolicyNumberTextBox.Text.Trim());
            command.Parameters.AddWithValue("$IncidentDate", IncidentDatePicker.SelectedDate?.ToString("yyyy-MM-dd") ?? string.Empty);
            command.Parameters.AddWithValue("$IncidentTime", IncidentTimeTextBox.Text.Trim());
            command.Parameters.AddWithValue("$IncidentLocation", IncidentLocationTextBox.Text.Trim());
            command.Parameters.AddWithValue("$IncidentType", ComboValue(IncidentTypeComboBox));
            command.Parameters.AddWithValue("$Weather", ComboValue(WeatherComboBox));
            command.Parameters.AddWithValue("$Road", ComboValue(RoadComboBox));
            command.Parameters.AddWithValue("$PoliceReportFiled", PoliceYesRadio.IsChecked == true ? "Yes" : "No");
            command.Parameters.AddWithValue("$PoliceReportNumber", PoliceReportTextBox.Text.Trim());
            command.Parameters.AddWithValue("$ImageFileName", _currentImages.Count > 0 ? Path.GetFileName(_currentImages[0].FilePath) : string.Empty);
            command.Parameters.AddWithValue("$ImageDescriptions", imageDescriptions);
            command.Parameters.AddWithValue("$ImageIncidentType", ComboValue(ImageIncidentTypeComboBox));
            command.Parameters.AddWithValue("$ImageVehicleCount", int.Parse(ImageVehicleCountTextBox.Text));
            command.Parameters.AddWithValue("$ImpactType", ComboValue(ImpactTypeComboBox));
            command.Parameters.AddWithValue("$Vehicle1Position", ComboValue(Vehicle1PositionComboBox));
            command.Parameters.AddWithValue("$Vehicle1Direction", ComboValue(Vehicle1DirectionComboBox));
            command.Parameters.AddWithValue("$Vehicle2Position", ComboValue(Vehicle2PositionComboBox));
            command.Parameters.AddWithValue("$Vehicle2Direction", ComboValue(Vehicle2DirectionComboBox));
            command.Parameters.AddWithValue("$DamageZones", damageZones);
            command.Parameters.AddWithValue("$RoadFactors", roadFactors);
            command.Parameters.AddWithValue("$Confidence", ComboValue(ConfidenceComboBox));
            command.Parameters.AddWithValue("$Assumptions", AssumptionsTextBox.Text.Trim());
            command.Parameters.AddWithValue("$PrimaryMake", PrimaryMakeTextBox.Text.Trim());
            command.Parameters.AddWithValue("$PrimaryModel", PrimaryModelTextBox.Text.Trim());
            command.Parameters.AddWithValue("$PrimaryYear", PrimaryYearTextBox.Text.Trim());
            command.Parameters.AddWithValue("$PrimaryDamage", ComboValue(PrimaryDamageComboBox));
            command.Parameters.AddWithValue("$MultiVehicle", MultiVehicleCheckBox.IsChecked == true ? "Yes" : "No");
            command.Parameters.AddWithValue("$OtherMake", OtherMakeTextBox.Text.Trim());
            command.Parameters.AddWithValue("$OtherModel", OtherModelTextBox.Text.Trim());
            command.Parameters.AddWithValue("$OtherDamage", ComboValue(OtherDamageComboBox));
            command.Parameters.AddWithValue("$InjuriesReported", InjuriesYesRadio.IsChecked == true ? "Yes" : "No");
            command.Parameters.AddWithValue("$WitnessPresent", WitnessYesRadio.IsChecked == true ? "Yes" : "No");
            command.Parameters.AddWithValue("$WitnessName", WitnessNameTextBox.Text.Trim());
            command.Parameters.AddWithValue("$CreatedAt", DateTime.UtcNow.ToString("o"));
            command.ExecuteNonQuery();

            ConfirmationText.Text = $"Claim Number: {claimNumber}\nClaimant: {ClaimantNameTextBox.Text.Trim()}\nStatus: Submitted for Review\n\nNext steps:\n- An adjuster will contact the claimant within 24 hours.\n- Reference this claim number in all follow-up.";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving claim:\n\n{ex.Message}\n\n{ex.InnerException?.Message}", "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
            WizardValidationText.Text = $"Failed to save claim: {ex.Message}";
        }
    }

    private void AutoPopulateImageAnalysisDefaults()
    {
        if (_currentImages.Count == 0)
        {
            return;
        }

        var fileName = Path.GetFileName(_currentImages[0].FilePath).ToLowerInvariant();
        var multi = fileName.Contains("2") || fileName.Contains("3") || fileName.Contains("multi") || fileName.Contains("tbone");
        var ambiguous = fileName.Contains("ambig") || fileName.Contains("unclear") || fileName.Contains("blur");

        ImageIncidentTypeComboBox.SelectedIndex = multi ? 1 : 0;
        ImageVehicleCountTextBox.Text = multi ? "2" : "1";
        ImpactTypeComboBox.SelectedIndex = fileName.Contains("rear") ? 2 : (fileName.Contains("head") ? 0 : 1);
        Vehicle1PositionComboBox.SelectedIndex = 0;
        Vehicle1DirectionComboBox.SelectedIndex = 4;
        Vehicle2PositionComboBox.SelectedIndex = multi ? 2 : -1;
        Vehicle2DirectionComboBox.SelectedIndex = multi ? 6 : -1;
        DamageFrontCheckBox.IsChecked = true;
        DamageRearCheckBox.IsChecked = multi;
        FactorIntersectionCheckBox.IsChecked = true;
        ConfidenceComboBox.SelectedIndex = ambiguous ? 1 : 0;
        AssumptionsTextBox.Text = ambiguous
            ? "Image has partial ambiguity; confirm impact type if needed."
            : "Clear accident geometry detected from uploaded image.";
    }

    private void RefreshSearchResults(string query)
    {
        SearchResultsListBox.Items.Clear();

        using var connection = new SqliteConnection($"Data Source={_dbPath}");
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = @"
SELECT ClaimNumber, ClaimantName, IncidentDate, Status, CreatedAt
FROM Claims
WHERE ClaimNumber LIKE $q OR ClaimantName LIKE $q
ORDER BY Id DESC
LIMIT 100;";
        command.Parameters.AddWithValue("$q", $"%{query}%");

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var line = $"{reader.GetString(0)} | {reader.GetString(1)} | {reader.GetString(2)} | {reader.GetString(3)} | {reader.GetString(4)}";
            SearchResultsListBox.Items.Add(line);
        }
    }

    private void RefreshAdminStats()
    {
        using var connection = new SqliteConnection($"Data Source={_dbPath}");
        connection.Open();

        using var countCommand = connection.CreateCommand();
        countCommand.CommandText = "SELECT COUNT(*) FROM Claims;";
        var total = Convert.ToInt32(countCommand.ExecuteScalar());

        using var lastCommand = connection.CreateCommand();
        lastCommand.CommandText = "SELECT ClaimNumber, CreatedAt FROM Claims ORDER BY Id DESC LIMIT 1;";
        using var reader = lastCommand.ExecuteReader();

        var sb = new StringBuilder();
        sb.AppendLine("Auto Claims FNOL - Admin Stats");
        sb.AppendLine($"Database Path: {_dbPath}");
        sb.AppendLine($"Total Claims: {total}");
        if (reader.Read())
        {
            sb.AppendLine($"Latest Claim: {reader.GetString(0)}");
            sb.AppendLine($"Latest CreatedAt (UTC): {reader.GetString(1)}");
        }
        else
        {
            sb.AppendLine("Latest Claim: (none)");
        }

        sb.AppendLine($"Current User: {_currentUser} ({_currentRole})");
        AdminStatsTextBox.Text = sb.ToString();
    }

    private void ResetDatabase()
    {
        using var connection = new SqliteConnection($"Data Source={_dbPath}");
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Claims;";
        command.ExecuteNonQuery();
    }

    private void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        var user = UsernameTextBox.Text.Trim();
        var pass = PasswordTextBox.Password;

        if ((user == AdjusterUser && pass == AdjusterPassword) || (user == AdminUser && pass == AdminPassword))
        {
            _currentUser = user;
            _currentRole = user == AdminUser ? "Admin" : "Adjuster";
            _loginAttempts = 0;
            UsernameTextBox.Text = string.Empty;
            PasswordTextBox.Password = string.Empty;
            ShowMainMenu();
            return;
        }

        _loginAttempts++;
        LoginStatusText.Text = $"Invalid username or password. Attempt {_loginAttempts} of 3.";
        PasswordTextBox.Password = string.Empty;

        if (_loginAttempts >= 3)
        {
            MessageBox.Show("Maximum login attempts exceeded. Terminal locked.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            Close();
        }
    }

    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void NewClaimButton_Click(object sender, RoutedEventArgs e)
    {
        ResetWizard();
        ShowWizard();
    }

    private void SearchClaimsButton_Click(object sender, RoutedEventArgs e)
    {
        ShowOnlyPanel(SearchPanel);
        SearchTextBox.Text = string.Empty;
        SearchResultsListBox.Items.Clear();
    }

    private void AdminButton_Click(object sender, RoutedEventArgs e)
    {
        if (_currentRole != "Admin")
        {
            MessageBox.Show("System Administration is restricted to admin users.", "Access Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        ShowOnlyPanel(AdminPanel);
        RefreshAdminStats();
    }

    private void LogOffButton_Click(object sender, RoutedEventArgs e)
    {
        _currentUser = string.Empty;
        _currentRole = string.Empty;
        ShowLogin();
    }

    private void CancelClaimButton_Click(object sender, RoutedEventArgs e)
    {
        if (MessageBox.Show("Cancel claim intake and return to Main Menu?", "Cancel Claim", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
        {
            ShowMainMenu();
        }
    }

    private void BackStepButton_Click(object sender, RoutedEventArgs e)
    {
        if (_currentStep > 1)
        {
            _currentStep--;
            RenderWizardStep();
        }
    }

    private void NextStepButton_Click(object sender, RoutedEventArgs e)
    {
        if (!ValidateCurrentStep(out var message))
        {
            WizardValidationText.Text = message;
            return;
        }

        if (_currentStep == 3)
        {
            AutoPopulateImageAnalysisDefaults();
        }

        if (_currentStep < 6)
        {
            _currentStep++;
            RenderWizardStep();
        }
    }

    private void SubmitClaimButton_Click(object sender, RoutedEventArgs e)
    {
        _currentStep = 1;
        if (!ValidateCurrentStep(out var s1))
        {
            WizardValidationText.Text = s1;
            _currentStep = 6;
            return;
        }

        _currentStep = 2;
        if (!ValidateCurrentStep(out var s2))
        {
            WizardValidationText.Text = s2;
            _currentStep = 6;
            return;
        }

        _currentStep = 4;
        if (!ValidateCurrentStep(out var s4))
        {
            WizardValidationText.Text = s4;
            _currentStep = 6;
            return;
        }

        _currentStep = 5;
        if (!ValidateCurrentStep(out var s5))
        {
            WizardValidationText.Text = s5;
            _currentStep = 6;
            return;
        }

        _currentStep = 6;
        SaveClaim();
        ShowOnlyPanel(ConfirmationPanel);
    }

    private void ReturnToMainMenuButton_Click(object sender, RoutedEventArgs e)
    {
        ShowMainMenu();
    }

    private void AddImageButton_Click(object sender, RoutedEventArgs e)
    {
        var description = ImageDescriptionTextBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(description))
        {
            MessageBox.Show("Please enter a description for the image (e.g., 'Front-end damage', 'Traffic flow diagram').", "Missing Description", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var dialog = new OpenFileDialog
        {
            Filter = "Image Files (*.jpg;*.jpeg;*.png;*.bmp;*.gif)|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
            Title = "Select Accident Image"
        };

        if (dialog.ShowDialog() != true)
        {
            return;
        }

        _currentImages.Add(new ClaimImage { FilePath = dialog.FileName, Description = description });
        RefreshImagesList();
        ImageDescriptionTextBox.Text = string.Empty;
    }

    private void RemoveImageButton_Click(object sender, RoutedEventArgs e)
    {
        if (ImagesListBox.SelectedIndex < 0)
        {
            MessageBox.Show("Please select an image to remove.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        _currentImages.RemoveAt(ImagesListBox.SelectedIndex);
        RefreshImagesList();
    }

    private void ImagesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ImagesListBox.SelectedIndex < 0 || ImagesListBox.SelectedIndex >= _currentImages.Count)
        {
            UploadedImagePreview.Source = null;
            return;
        }

        var image = _currentImages[ImagesListBox.SelectedIndex];
        try
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.UriSource = new Uri(image.FilePath);
            bitmap.EndInit();
            UploadedImagePreview.Source = bitmap;
        }
        catch
        {
            MessageBox.Show($"Could not load image: {image.FilePath}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void RefreshImagesList()
    {
        ImagesListBox.Items.Clear();
        foreach (var img in _currentImages)
        {
            ImagesListBox.Items.Add(img);
        }

        ImageStatusText.Text = _currentImages.Count == 0
            ? "Status: No images uploaded"
            : $"Status: {_currentImages.Count} image(s) attached";
    }

    private void SearchButton_Click(object sender, RoutedEventArgs e)
    {
        RefreshSearchResults(SearchTextBox.Text.Trim());
    }

    private void BackToMenuFromSearchButton_Click(object sender, RoutedEventArgs e)
    {
        ShowMainMenu();
    }

    private void ResetDatabaseButton_Click(object sender, RoutedEventArgs e)
    {
        var confirm = MessageBox.Show(
            "Are you sure? This will erase all claims and reset to defaults.",
            "Reset Database",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (confirm != MessageBoxResult.Yes)
        {
            return;
        }

        ResetDatabase();
        RefreshAdminStats();
        MessageBox.Show("Database reset successfully. System ready for demo.", "Reset Complete", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void RefreshStatsButton_Click(object sender, RoutedEventArgs e)
    {
        RefreshAdminStats();
    }

    private void BackToMenuFromAdminButton_Click(object sender, RoutedEventArgs e)
    {
        ShowMainMenu();
    }
}