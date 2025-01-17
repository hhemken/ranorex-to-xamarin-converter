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
                        string targetPath = GetTargetPath(filePath);
                        ConvertSingleFile(filePath);
                        _filesProcessed++;
                        LogMessage($"Successfully converted: {filePath} -> {targetPath}");
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

    private string GetTargetPath(string sourcePath)
    {
        string extension = Path.GetExtension(sourcePath).ToLower();
        string fileName = Path.GetFileNameWithoutExtension(sourcePath);
        
        switch (extension)
        {
            case ".rxtst":
                return Path.Combine(_outputPath, "Tests", $"{fileName}Tests.cs");
            case ".rxrec":
                return Path.Combine(_outputPath, "Tests", $"{fileName}Tests.cs");
            case ".cs":
                return Path.Combine(_outputPath, $"{fileName}.cs");
            default:
                return Path.Combine(_outputPath, Path.GetFileName(sourcePath));
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

    
    private string? ConvertActivity(string activityType, Dictionary<string, string> variables)
    {
        // Create original step comment
        string originalStep = $"// Original Ranorex step - Type: {activityType}";
        foreach (var kvp in variables)
        {
            originalStep += $", {kvp.Key}: {kvp.Value}";
        }
    
        string? convertedStep = activityType.ToLower() switch
        {
            "click" or "touch" => $"app.Tap(x => x.Marked(\"{variables.GetValueOrDefault("target")}\"));",
            
            "setvalue" => $"app.EnterText(x => x.Marked(\"{variables.GetValueOrDefault("target")}\"), \"{variables.GetValueOrDefault("value")}\");",
            
            "wait" => int.TryParse(variables.GetValueOrDefault("timeout", "5000"), out int timeout) 
                ? $"app.WaitForElement(x => x.Marked(\"{variables.GetValueOrDefault("target")}\"), timeout: TimeSpan.FromMilliseconds({timeout}));"
                : $"app.WaitForElement(x => x.Marked(\"{variables.GetValueOrDefault("target")}\"));",
            
            "executescript" => null, // Will be handled as comment
            
            "invoke" => null, // Will be handled as comment
            
            _ => null
        };
    
        // If we couldn't convert the step, return it as a TODO comment
        if (convertedStep == null)
        {
            return $"{originalStep}\n        // TODO: No direct Xamarin.UITest equivalent - Needs manual conversion";
        }
    
        // Return both the original comment and the converted step
        return $"{originalStep}\n        {convertedStep}";
    }
    
    // Updated ConvertValidation method
    private string? ConvertValidation(string validationType, string compareValue, XElement? element)
    {
        string elementId = element?.Attribute("id")?.Value ?? string.Empty;
        
        // Create original validation comment
        string originalValidation = $"// Original Ranorex validation - Type: {validationType}, Compare: {compareValue}, ElementId: {elementId}";
    
        string? convertedValidation = validationType.ToLower() switch
        {
            "exists" => $"Assert.That(app.Query(x => x.Marked(\"{elementId}\")).Any(), Is.True, \"Element {elementId} should exist\");",
            
            "notexists" => $"Assert.That(app.Query(x => x.Marked(\"{elementId}\")).Any(), Is.False, \"Element {elementId} should not exist\");",
            
            "equals" => $@"Assert.That(app.Query(x => x.Marked(""{elementId}"")).First().Text, Is.EqualTo(""{compareValue}""));",
            
            "contains" => $@"Assert.That(app.Query(x => x.Marked(""{elementId}"")).First().Text, Does.Contain(""{compareValue}""));",
            
            "enabled" => $"Assert.That(app.Query(x => x.Marked(\"{elementId}\")).First().Enabled, Is.True, \"Element {elementId} should be enabled\");",
            
            "disabled" => $"Assert.That(app.Query(x => x.Marked(\"{elementId}\")).First().Enabled, Is.False, \"Element {elementId} should be disabled\");",
            
            _ => null
        };
    
        // If we couldn't convert the validation, return it as a TODO comment
        if (convertedValidation == null)
        {
            return $"{originalValidation}\n        // TODO: No direct Xamarin.UITest equivalent - Needs manual conversion";
        }
    
        // Return both the original comment and the converted validation
        return $"{originalValidation}\n        {convertedValidation}";
    }
    
    private string? ConvertCustomScript(string script)
    {
        if (string.IsNullOrEmpty(script))
            return null;
    
        return $@"// Original Ranorex custom script:
            /*
            {script}
            */
            // TODO: Custom script needs manual conversion to Xamarin.UITest";
    }

    private string? ConvertMethodInvocation(string method, Dictionary<string, string> variables)
    {
        if (string.IsNullOrEmpty(method))
            return null;
    
        StringBuilder comment = new StringBuilder();
        comment.AppendLine($"// Original Ranorex method invocation: {method}");
        comment.AppendLine("        // Parameters:");
        foreach (var kvp in variables)
        {
            comment.AppendLine($"        //   {kvp.Key}: {kvp.Value}");
        }
        comment.AppendLine("        // TODO: Method invocation needs manual conversion to Xamarin.UITest");
    
        return comment.ToString();
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

    private string? ConvertRecordedAction(XElement action)
    {
        string actionType = action.Attribute("type")?.Value ?? string.Empty;
        string varName = action.Attribute("varname")?.Value ?? string.Empty;
        var path = action.Elements("path").FirstOrDefault();
        var mouseButton = action.Attribute("mousebutton")?.Value;
        var value = action.Attribute("value")?.Value;
    
        // Create original action comment
        string originalAction = $"// Original Ranorex recording - Type: {actionType}";
        if (!string.IsNullOrEmpty(varName)) originalAction += $", VarName: {varName}";
        if (!string.IsNullOrEmpty(mouseButton)) originalAction += $", MouseButton: {mouseButton}";
        if (!string.IsNullOrEmpty(value)) originalAction += $", Value: {value}";
    
        // Extract element identification information
        string elementQuery = BuildElementQuery(path);
    
        string? convertedAction = actionType.ToLower() switch
        {
            "click" or "touch" => $"app.Tap({elementQuery});",
            
            "setvalue" => $"app.EnterText({elementQuery}, \"{value}\");",
            
            "movemouse" => null, // No direct equivalent
            
            "keyboard" => value?.Contains("Return") ?? false 
                ? "app.PressEnter();"
                : value?.Contains("Tab") ?? false 
                    ? "app.DismissKeyboard();" 
                    : $"app.EnterText(\"{value}\");",
            
            "wait" => $"app.WaitForElement({elementQuery});",
            
            "validate" => $"Assert.That(app.Query({elementQuery}).Any(), Is.True);",
            
            _ => null
        };
    
        // If we couldn't convert the action, return it as a TODO comment
        if (convertedAction == null)
        {
            return $"{originalAction}\n        // TODO: No direct Xamarin.UITest equivalent - Needs manual conversion";
        }
    
        // Return both the original comment and the converted action
        return $"{originalAction}\n        {convertedAction}";
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
    