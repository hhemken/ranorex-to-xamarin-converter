using System;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class RanorexToXamarinConverter
{
    private readonly string _outputPath;
    private readonly string _logPath;
    private int _filesProcessed = 0;
    private int _filesSkipped = 0;
    private int _filesError = 0;
    
    private readonly List<string> _supportedExtensions = new List<string> 
    { 
        ".rxtst", 
        ".cs", 
        ".rxrec" 
    };

    public RanorexToXamarinConverter(string outputPath, string logPath = null)
    {
        _outputPath = outputPath;
        _logPath = logPath ?? Path.Combine(outputPath, "conversion_log.txt");
        InitializeLog();
    }

    public void ProcessRanorexDirectory(string directoryPath)
    {
        LogMessage($"Starting directory processing: {directoryPath}");
        
        try
        {
            // Process all files in directory and subdirectories
            foreach (string filePath in Directory.EnumerateFiles(directoryPath, "*.*", SearchOption.AllDirectories))
            {
                string extension = Path.GetExtension(filePath).ToLower();
                
                if (_supportedExtensions.Contains(extension))
                {
                    try
                    {
                        LogMessage($"Processing file: {filePath}");
                        ConvertSingleFile(filePath);
                        _filesProcessed++;
                        LogMessage($"Successfully converted: {filePath}");
                    }
                    catch (Exception ex)
                    {
                        _filesError++;
                        LogError($"Error processing file {filePath}: {ex.Message}");
                        LogError($"Stack trace: {ex.StackTrace}");
                        // Continue with next file
                    }
                }
            }
            
            // Log summary
            LogSummary();
        }
        catch (Exception ex)
        {
            LogError($"Fatal error processing directory: {ex.Message}");
            throw;
        }
    }

    public void ConvertSingleFile(string inputPath)
    {
        if (!ValidateFile(inputPath))
        {
            return;
        }

        string extension = Path.GetExtension(inputPath).ToLower();
        LogMessage($"Converting file with extension: {extension}");
        
        switch (extension)
        {
            case ".rxtst":
                ConvertRxtstFile(inputPath);
                break;
            case ".cs":
                ConvertSingleCSharpFile(inputPath);
                break;
            case ".rxrec":
                ConvertRecordingFile(inputPath);
                break;
            default:
                throw new ArgumentException($"Unsupported file type: {extension}");
        }
    }

    private void ConvertRxtstFile(string rxtstPath)
    {
        // Parse the .rxtst file
        var testSuite = XDocument.Load(rxtstPath);
        var testCases = ParseTestCases(testSuite);
        
        // Create the test project structure if it doesn't exist
        CreateTestProjectStructure();
        
        // Convert each test case
        foreach (var testCase in testCases)
        {
            ConvertTestCase(testCase);
        }
    }

    private List<TestCase> ParseTestCases(XDocument testSuite)
    {
        var testCases = new List<TestCase>();
        
        // Extract test cases from Ranorex XML structure
        var testCaseElements = testSuite.Descendants("test")
            .Where(x => x.Attribute("type")?.Value == "testcase");
            
        foreach (var element in testCaseElements)
        {
            testCases.Add(new TestCase
            {
                Name = element.Attribute("name")?.Value,
                Path = element.Attribute("path")?.Value,
            });
        }
        
        return testCases;
    }
    private void ConvertTestCase(TestCase testCase)
    {
        var xamarinTest = new StringBuilder();
        
        // Add required using statements
        xamarinTest.AppendLine("using System;");
        xamarinTest.AppendLine("using NUnit.Framework;");
        xamarinTest.AppendLine("using Xamarin.UITest;");
        
        // Convert test steps
        ConvertTestSteps(testCase, xamarinTest);
        
        // Save the converted test
        File.WriteAllText(
            Path.Combine(_outputPath, $"{testCase.Name}Tests.cs"),
            xamarinTest.ToString());
    }

    private void ConvertTestSteps(TestCase testCase, StringBuilder xamarinTest)
    {
        // Get the test case module from the path
        var testModule = XDocument.Load(testCase.Path);
        
        // Start building the test class
        xamarinTest.AppendLine($@"[TestFixture]
public class {testCase.Name}Tests : BaseTestFixture
{{
    [Test]
    public void {testCase.Name}Test()
    {{");

        // Process each activity in the test case
        foreach (var activity in testModule.Descendants("activity"))
        {
            string activityType = activity.Attribute("type")?.Value ?? "";
            var variables = activity.Elements("var").ToDictionary(
                v => v.Attribute("name")?.Value ?? "",
                v => v.Attribute("value")?.Value ?? ""
            );

            string convertedStep = ConvertActivity(activityType, variables);
            if (!string.IsNullOrEmpty(convertedStep))
            {
                xamarinTest.AppendLine($"        {convertedStep}");
            }
        }

        // Process any validation rules
        foreach (var validation in testModule.Descendants("validationrule"))
        {
            string validationType = validation.Attribute("type")?.Value ?? "";
            string compareValue = validation.Attribute("compare")?.Value ?? "";
            var element = validation.Element("element");

            string convertedValidation = ConvertValidation(validationType, compareValue, element);
            if (!string.IsNullOrEmpty(convertedValidation))
            {
                xamarinTest.AppendLine($"        {convertedValidation}");
            }
        }

        // Close the test method and class
        xamarinTest.AppendLine("    }");
        xamarinTest.AppendLine("}");
    }

    private string ConvertActivity(string activityType, Dictionary<string, string> variables)
    {
        switch (activityType.ToLower())
        {
            case "click":
            case "touch":
                return $"app.Tap(x => x.Marked(\"{variables.GetValueOrDefault("target")}\"));";

            case "setvalue":
                return $"app.EnterText(x => x.Marked(\"{variables.GetValueOrDefault("target")}\"), \"{variables.GetValueOrDefault("value")}\");";

            case "wait":
                int timeout;
                if (int.TryParse(variables.GetValueOrDefault("timeout", "5000"), out timeout))
                {
                    return $"app.WaitForElement(x => x.Marked(\"{variables.GetValueOrDefault("target")}\"), timeout: TimeSpan.FromMilliseconds({timeout}));";
                }
                return $"app.WaitForElement(x => x.Marked(\"{variables.GetValueOrDefault("target")}\"));";

            case "executescript":
                string script = variables.GetValueOrDefault("script", "");
                return ConvertCustomScript(script);

            case "invoke":
                string method = variables.GetValueOrDefault("method", "");
                return ConvertMethodInvocation(method, variables);

            default:
                LogMessage($"Unsupported activity type: {activityType}");
                return null;
        }
    }

    private string ConvertValidation(string validationType, string compareValue, XElement element)
    {
        string elementId = element?.Attribute("id")?.Value ?? "";
        
        switch (validationType.ToLower())
        {
            case "exists":
                return $"Assert.That(app.Query(x => x.Marked(\"{elementId}\")).Any(), Is.True, \"Element {elementId} should exist\");";

            case "notexists":
                return $"Assert.That(app.Query(x => x.Marked(\"{elementId}\")).Any(), Is.False, \"Element {elementId} should not exist\");";

            case "equals":
                return $@"Assert.That(app.Query(x => x.Marked(""{elementId}"")).First().Text, Is.EqualTo(""{compareValue}""));";

            case "contains":
                return $@"Assert.That(app.Query(x => x.Marked(""{elementId}"")).First().Text, Does.Contain(""{compareValue}""));";

            case "enabled":
                return $"Assert.That(app.Query(x => x.Marked(\"{elementId}\")).First().Enabled, Is.True, \"Element {elementId} should be enabled\");";

            case "disabled":
                return $"Assert.That(app.Query(x => x.Marked(\"{elementId}\")).First().Enabled, Is.False, \"Element {elementId} should be disabled\");";

            default:
                LogMessage($"Unsupported validation type: {validationType}");
                return null;
        }
    }

    private string ConvertCustomScript(string script)
    {
        return $"// Custom script conversion needed: {script}";
    }

    private string ConvertMethodInvocation(string method, Dictionary<string, string> variables)
    {
        return $"// Method invocation conversion needed: {method}";
    }
    
    private void ConvertRecordingFile(string rxrecPath)
    {
        var recording = XDocument.Load(rxrecPath);
        var testClassName = Path.GetFileNameWithoutExtension(rxrecPath) + "Tests";
        var xamarinTest = new StringBuilder();

        // Add standard using statements
        xamarinTest.AppendLine("using System;");
        xamarinTest.AppendLine("using NUnit.Framework;");
        xamarinTest.AppendLine("using Xamarin.UITest;");
        xamarinTest.AppendLine();

        // Create test class
        xamarinTest.AppendLine($"[TestFixture]");
        xamarinTest.AppendLine($"public class {testClassName} : BaseTestFixture");
        xamarinTest.AppendLine("{");

        // Convert recorded actions
        xamarinTest.AppendLine("    [Test]");
        xamarinTest.AppendLine($"    public void {testClassName}Main()");
        xamarinTest.AppendLine("    {");

        // Process each recorded action
        foreach (var action in recording.Descendants("action"))
        {
            string convertedAction = ConvertRecordedAction(action);
            if (!string.IsNullOrEmpty(convertedAction))
            {
                xamarinTest.AppendLine($"        {convertedAction}");
            }
        }

        xamarinTest.AppendLine("    }");
        xamarinTest.AppendLine("}");

        // Save converted test
        string outputPath = Path.Combine(_outputPath, $"{testClassName}.cs");
        File.WriteAllText(outputPath, xamarinTest.ToString());
    }

    private string ConvertRecordedAction(XElement action)
    {
        string actionType = action.Attribute("type")?.Value ?? "";
        string varName = action.Attribute("varname")?.Value ?? "";
        var path = action.Elements("path").FirstOrDefault();
        var mouseButton = action.Attribute("mousebutton")?.Value;
        var value = action.Attribute("value")?.Value;

        // Extract element identification information
        string elementQuery = BuildElementQuery(path);

        switch (actionType.ToLower())
        {
            case "click":
            case "touch":
                return $"app.Tap({elementQuery});";

            case "setvalue":
                return $"app.EnterText({elementQuery}, \"{value}\");";

            case "movemouse":
                // Xamarin.UITest doesn't have direct mouse movement equivalent
                return null;

            case "keyboard":
                if (value?.Contains("Return") ?? false)
                    return "app.PressEnter();";
                else if (value?.Contains("Tab") ?? false)
                    return "app.DismissKeyboard();";
                return $"app.EnterText(\"{value}\");";

            case "wait":
                return $"app.WaitForElement({elementQuery});";

            case "validate":
                return $"Assert.That(app.Query({elementQuery}).Any(), Is.True);";

            default:
                LogMessage($"Unsupported action type: {actionType}");
                return null;
        }
    }

    private string BuildElementQuery(XElement pathElement)
    {
        if (pathElement == null)
            return "x => x.All()";

        var conditions = new List<string>();

        // Process each adapter in the path
        foreach (var adapter in pathElement.Elements("adapter"))
        {
            string id = adapter.Attribute("id")?.Value;
            string role = adapter.Attribute("role")?.Value;
            string title = adapter.Attribute("title")?.Value;

            if (!string.IsNullOrEmpty(id))
                conditions.Add($"Marked(\"{id}\")");
            else if (!string.IsNullOrEmpty(title))
                conditions.Add($"Text(\"{title}\")");
            else if (!string.IsNullOrEmpty(role))
                conditions.Add($"Class(\"{role}\")");
        }

        if (!conditions.Any())
            return "x => x.All()";

        return $"x => x.{string.Join(".", conditions)}";
    }

    private void ConvertSingleCSharpFile(string csharpPath)
    {
        Directory.CreateDirectory(_outputPath);
        var code = File.ReadAllText(csharpPath);
        var convertedCode = ConvertRanorexCode(code);
        
        string outputFileName = Path.GetFileName(csharpPath);
        File.WriteAllText(
            Path.Combine(_outputPath, outputFileName),
            convertedCode);
    }

    private string ConvertRanorexCode(string code)
    {
        // Replace Ranorex namespaces
        code = code.Replace("using Ranorex;", "using Xamarin.UITest;");
        
        // Convert Ranorex repository references
        code = Regex.Replace(
            code,
            @"repo\.([A-Za-z0-9_]+)\.Click\(\)",
            "app.Tap(x => x.Marked(\"$1\"))");
            
        // Add more conversion patterns as needed
        
        return code;
    }
    
    private void CreateTestProjectStructure()
    {
        // Create necessary directories
        Directory.CreateDirectory(_outputPath);
        Directory.CreateDirectory(Path.Combine(_outputPath, "Pages"));
        Directory.CreateDirectory(Path.Combine(_outputPath, "Tests"));
        
        // Create base test class
        CreateBaseTestClass();
        
        // Create solution and project files
        CreateProjectFiles();
    }

    private void CreateBaseTestClass()
    {
        var baseTest = @"
using NUnit.Framework;
using Xamarin.UITest;

namespace XamarinTests
{
    public class BaseTestFixture
    {
        protected IApp app;
        protected bool OnAndroid;

        [SetUp]
        public virtual void BeforeEachTest()
        {
            app = ConfigureApp
                .Android
                .StartApp();
        }
    }
}";
        
        File.WriteAllText(
            Path.Combine(_outputPath, "BaseTestFixture.cs"),
            baseTest);
    }

    private void CreateProjectFiles()
    {
        // Create .csproj file
        var csproj = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>netcoreapp3.1</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include=""NUnit"" Version=""3.13.2"" />
    <PackageReference Include=""Xamarin.UITest"" Version=""3.2.2"" />
  </ItemGroup>
</Project>";
        
        File.WriteAllText(
            Path.Combine(_outputPath, "XamarinTests.csproj"),
            csproj);
    }

    private void InitializeLog()
    {
        // Create or clear the log file
        Directory.CreateDirectory(Path.GetDirectoryName(_logPath));
        File.WriteAllText(_logPath, $"Conversion started at: {DateTime.Now}\n");
    }

    private void LogMessage(string message)
    {
        string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - INFO: {message}";
        File.AppendAllText(_logPath, logEntry + "\n");
        Console.WriteLine(logEntry);
    }

    private void LogError(string message)
    {
        string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - ERROR: {message}";
        File.AppendAllText(_logPath, logEntry + "\n");
        Console.WriteLine(logEntry);
    }

    private void LogSummary()
    {
        string summary = $@"
=== Conversion Summary ===
Total files processed successfully: {_filesProcessed}
Files skipped: {_filesSkipped}
Files with errors: {_filesError}
Total files attempted: {_filesProcessed + _filesSkipped + _filesError}
========================";
        
        LogMessage(summary);
    }

    private bool ValidateFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            LogError($"File not found: {filePath}");
            _filesSkipped++;
            return false;
        }
        
        if (new FileInfo(filePath).Length == 0)
        {
            LogError($"Empty file: {filePath}");
            _filesSkipped++;
            return false;
        }
        
        return true;
    }
}

public class TestCase
{
    public string Name { get; set; }
    public string Path { get; set; }
    // Add other properties as needed
}

// Example usage:
/*
var converter = new RanorexToXamarinConverter(
    outputPath: "./XamarinTests",
    logPath: "./conversion_log.txt"
);

// Convert entire directory
converter.ProcessRanorexDirectory("path/to/ranorex/project");

// Or convert single file
converter.ConvertSingleFile("path/to/your/file.rxtst");
*/