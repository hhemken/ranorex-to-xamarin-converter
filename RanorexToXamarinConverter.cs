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
                // Add other relevant properties
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
        // Example conversion of common Ranorex actions to Xamarin.UITest
        var conversionMap = new Dictionary<string, string>
        {
            {"Click", "Tap"},
            {"SetValue", "EnterText"},
            {"Touch", "Tap"},
            {"ValidateAttributeEqual", "WaitForElement"}
        };
        
        // Add logic to convert specific Ranorex actions to Xamarin.UITest commands
        // This would need to be customized based on your specific test cases
    }

    private void ConvertSingleCSharpFile(string csharpPath)
    {
        // Create output directory if it doesn't exist
        Directory.CreateDirectory(_outputPath);
        
        var code = File.ReadAllText(csharpPath);
        var convertedCode = ConvertRanorexCode(code);
        
        // Save converted file
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
                // Log unsupported action type
                Console.WriteLine($"Unsupported action type: {actionType}");
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