# RanorexToXamarinConverter

A C# tool for converting Ranorex test automation scripts to Xamarin.UITest format. This converter handles test suites (.rxtst), recordings (.rxrec), and C# test files (.cs) from Ranorex and converts them into equivalent Xamarin.UITest implementations.

## Features

- Converts Ranorex test suites to NUnit-based Xamarin.UITest projects
- Handles multiple file types:
  - Test Suites (.rxtst)
  - Recordings (.rxrec)
  - C# Test Files (.cs)
- Creates complete project structure with necessary configurations
- Maintains detailed conversion logs
- Supports batch processing of entire directories
- Preserves test case structure and validation rules
- Handles common UI interactions:
  - Clicks and touches
  - Text input
  - Wait conditions
  - Element validation
  - Keyboard actions

## Prerequisites

- .NET Core 3.1 or later
- NUnit (3.13.2 or later)
- Xamarin.UITest (3.2.2 or later)

## Installation

1. Clone the repository:
```bash
git clone https://github.com/yourusername/RanorexToXamarinConverter.git
```

2. Build the solution:
```bash
dotnet build
```

## Usage

### Basic Usage

```csharp
// Initialize the converter
var converter = new RanorexToXamarinConverter(
    outputPath: "./XamarinTests",
    logPath: "./conversion_log.txt"
);

// Convert a single file
converter.ConvertSingleFile("path/to/your/test.rxtst");

// Or convert an entire directory
converter.ProcessRanorexDirectory("path/to/ranorex/project");
```

### Output Structure

The converter creates the following directory structure:
```
XamarinTests/
├── Pages/
├── Tests/
│   └── ConvertedTests.cs
├── BaseTestFixture.cs
└── XamarinTests.csproj
```

## Conversion Details

### Supported Conversions

| Ranorex Action | Xamarin.UITest Equivalent |
|----------------|--------------------------|
| Click/Touch | `app.Tap()` |
| SetValue | `app.EnterText()` |
| Wait | `app.WaitForElement()` |
| Keyboard | `app.PressEnter()`, `app.EnterText()` |
| Validation | `Assert.That()` |

### Validation Rules

The converter supports various validation types:
- Element existence checking
- Text comparison
- Contains validation
- Element state validation (enabled/disabled)

## Logging

The converter maintains detailed logs of the conversion process:
- Processing start and end times
- Files processed
- Conversion successes and failures
- Error details and stack traces
- Summary statistics

Example log output:
```
2025-01-16 10:30:15 - INFO: Starting directory processing: C:\RanorexProject
2025-01-16 10:30:15 - INFO: Processing file: C:\RanorexProject\MyTest.rxtst
2025-01-16 10:30:16 - INFO: Successfully converted: C:\RanorexProject\MyTest.rxtst -> C:\XamarinTests\Tests\MyTestTests.cs
2025-01-16 10:30:16 - INFO: === Conversion Summary ===
Total files processed successfully: 1
Files skipped: 0
Files with errors: 0
Total files attempted: 1
```

## Known Limitations

1. Mouse movement actions (`movemouse`) are not supported in Xamarin.UITest
2. Complex custom scripts may require manual review
3. Platform-specific Ranorex features might not have direct equivalents
4. Some advanced Ranorex selectors may need manual adjustment

## Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

## Support

For issues, questions, or contributions, please:
1. Check the existing issues
2. Create a new issue if needed
3. Contact the maintainers

## Acknowledgments

- Thanks to the Ranorex and Xamarin.UITest communities
- Contributors and maintainers
- Users who provide feedback and suggestions