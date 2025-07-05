# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build Commands

### JVDownloader (.NET Framework 4.8)
```bash
# Debug build
msbuild JVDownloader/JVDownloader.csproj /p:Configuration=Debug

# Release build
msbuild JVDownloader/JVDownloader.csproj /p:Configuration=Release
```

### JVParser (.NET 6.0)
```bash
# Build
dotnet build JVParser/JVParser.csproj

# Run with arguments
dotnet run --project JVParser/JVParser.csproj [input_file] [output_dir]

# Build release version
dotnet build JVParser/JVParser.csproj -c Release
```

## Architecture Overview

This solution consists of two console applications for working with JRA-VAN horse racing data:

### JVDownloader
- **Purpose**: Downloads horse racing data from JRA-VAN Data Lab service via COM interop
- **Framework**: .NET Framework 4.8 (required for COM compatibility with JVDTLabLib)
- **Key Components**:
  - `Program.cs`: Entry point with three main commands:
    - `setup`: Initial JRA-VAN setup
    - `jv`: Download data with date range (e.g., `jv -s 20241201 -e 20241231 -spec RACE`)
    - `jvrt`: Real-time data download
  - `Logger.cs`: Comprehensive error code mapping for JV-Link API responses
  - Progress form for download status visualization
  - Outputs text files in format: `JV-{spec}-{timestamp}.txt`

### JVParser
- **Purpose**: Parses JV data files and converts to JSONL format
- **Framework**: .NET 6.0
- **Key Components**:
  - `JVData_Struct.cs`: Data structures matching JV binary format (contains over 30 record types)
  - `RecordSpecStreamWriterManager.cs`: Manages output streams, creating separate JSONL files per record type
  - `OutputFileAlreadyExistsException.cs`: Custom exception for file handling
  - Outputs JSONL files organized by record specification (e.g., `RA.jsonl`, `SE.jsonl`)

### Data Flow
1. JVDownloader connects to JRA-VAN servers using COM interop
2. Downloads binary race data and saves as text files
3. JVParser reads these text files and parses the binary structures
4. Converts each record to JSON and outputs to specification-specific JSONL files

## Important Notes

- **No test framework**: This project lacks unit tests. When adding new functionality, consider implementing tests manually
- **COM dependency**: JVDownloader requires Windows and JRA-VAN Data Lab software installed
- **Multi-framework**: The solution uses both .NET Framework 4.8 and .NET 6.0 for compatibility reasons
- **Data structures**: JV data format is complex with many record types defined in `JVData_Struct.cs`
- **Error handling**: Logger.cs contains comprehensive JV-Link error codes that should be referenced when debugging download issues