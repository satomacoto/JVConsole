# JVParser

## Prerequirements

### Windows

Install Visual Studio 2022.

### Mac

```
brew install homebrew/cask/dotnet-sdk
```

## How to use

```
dotnet run [input file path] [output dir]
```

e.g.

```
time dotnet run ./txt/JV-RACE-20211122155500.txt ./data/jsonl
```

Read ./txt/JV-RACE-20211122155500.txt and outputs ./data/jsonl

```
./data/jsonl/JV-RACE-20211122155500-RA.jsonl
./data/jsonl/JV-RACE-20211122155500-SE.jsonl
...
```
