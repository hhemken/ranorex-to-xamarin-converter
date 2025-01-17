# Ranorex to Xamarin.UITest Converter

This tool automates the conversion of Ranorex test suites to Xamarin.UITest format. It processes .rxtst (test suite), .rxrec (recording), and .cs (C# code) files, handling both individual files and entire project directories.

## Features

### File Processing
- Recursive directory scanning
- Multiple file type support:
  - .rxtst (Test Suite files)
  - .rxrec (Recording files)
  - .cs (C# code files)
- Detailed conversion logging
- Error handling with continue-on-error capability

### Test Conversion Capabilities
- Converts Ranorex activities to Xamarin.UITest commands:
  - Click/Touch → Tap
  - SetValue → EnterText
  - Wait → WaitForElement
  - Custom script actions
  - Method invocations
- Handles validation rules:
  - Element existence checks
  - Value comparisons
  - Text content validation
  - Element state (enabled/disabled)
- Element identification conversion
- Test step organization

## Requirements

- .NET Core 3.1 or higher
- Visual Studio 2019 or higher (for using generated tests)
- NuGet packages:
  - NUnit (3.13.2)
  - Xamarin.UITest (3.2.2)

## Installation

1. Clone this repository
2. Build the solution:
```bash
dotnet build
```

## Usage

### Converting an Entire Project Directory

```csharp
// Initialize converter with output and log paths
var converter = new RanorexToXamarinConverter(
    outputPath: "./XamarinTests",
    logPath: "./conversion_log.txt"
);

// Process entire directory
converter.ProcessRanorexDirectory("path/to/ranorex/project");
```

### Converting Individual Files

```csharp
var converter = new RanorexToXamarinConverter("./XamarinTests");

// Convert single test suite file
converter.ConvertSingleFile("path/to/your/file.rxtst");

// Convert single recording file
converter.ConvertSingleFile("path/to/your/file.rxrec");

// Convert single C# file
converter.ConvertSingleFile("path/to/your/file.cs");
```

## Output Structure

```
XamarinTests/
├── Tests/
│   ├── ConvertedTest1.cs
│   ├── ConvertedTest2.cs
│   └── ...
├── Pages/
│   └── ...
├── BaseTestFixture.cs
├── XamarinTests.csproj
└── conversion_log.txt
```

## Logging

The converter generates detailed logs including:
- Timestamp for each operation
- Success/failure status for each file
- Error messages and stack traces
- Summary statistics:
  - Files processed successfully
  - Files skipped
  - Files with errors
  - Total files attempted

Example log output:
```
2025-01-16 10:30:00 - INFO: Starting directory processing: C:\RanorexProject
2025-01-16 10:30:01 - INFO: Processing file: TestSuite.rxtst
2025-01-16 10:30:01 - INFO: Successfully converted: TestSuite.rxtst
2025-01-16 10:30:02 - ERROR: Error processing file Recording1.rxrec: Invalid XML format
...
=== Conversion Summary ===
Total files processed successfully: 45
Files skipped: 2
Files with errors: 3
Total files attempted: 50
========================
```

## Conversion Details

### Test Suite (.rxtst) Conversion
- Converts test suite structure to NUnit test fixtures
- Maintains test case organization
- Converts test case metadata
- Handles test dependencies

### Recording (.rxrec) Conversion
- Converts recorded actions to Xamarin.UITest commands
- Processes element identification paths
- Handles common UI interactions
- Converts validation steps

### C# Code Conversion
- Updates namespace references
- Converts repository element references
- Transforms Ranorex-specific code patterns

## Error Handling

The converter implements robust error handling:
- Validates files before processing
- Skips invalid or empty files
- Continues processing after individual file failures
- Provides detailed error logging
- Maintains conversion statistics

## Limitations

1. Unsupported Ranorex Features:
   - Mouse movement actions
   - Complex gesture recordings
   - Some advanced validation types
   - Platform-specific Ranorex features

2. Element Identification:
   - Some Ranorex RxPath patterns may need manual adjustment
   - Complex element location strategies might require review

3. Custom Code:
   - Custom Ranorex plugins aren't converted
   - Complex custom scripts require manual conversion
   - Method invocations may need adjustment

## Troubleshooting

### Common Issues and Solutions

1. **Missing Element Identifiers**
   - Check original Ranorex element repository
   - Verify element identification strategy
   - Consider using alternative locator types

2. **Conversion Failures**
   - Check the log file for specific error details
   - Verify input file format and content
   - Ensure file permissions are correct

3. **Runtime Errors in Converted Tests**
   - Review element identification queries
   - Check for timing issues in wait statements
   - Verify test dependencies

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add appropriate tests
5. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For issues and feature requests:
1. Check the documentation
2. Review existing issues
3. Create a new issue with:
   - Detailed description
   - Sample files (if possible)
   - Error messages and logs