# fa.libraries.dotnet
## Description
This is a collection of FrostAura libraries written for dotnet core 7+. Documentation pending.
## Status
| Project | Status | Platform
| --- | --- | --- |
| FrostAura.Libraries.* | [![NuGet Workflow](https://github.com/faGH/fa.libraries.dotnet/actions/workflows/nuget_workflow.yml/badge.svg)](https://github.com/faGH/fa.libraries.dotnet/actions/workflows/nuget_workflow.yml) | GitHub Actions

## NuGet Packages
| Project | Nuget |
| --- | --- |
| FrostAura.Libraries.Core | [![NuGet](https://img.shields.io/nuget/v/FrostAura.Libraries.Core.svg?style=for-the-badge)](https://www.nuget.org/packages/FrostAura.Libraries.Core/)[![NuGet](https://img.shields.io/nuget/dt/FrostAura.Libraries.Core.svg?style=for-the-badge)](https://www.nuget.org/packages/FrostAura.Libraries.Core/) |
| FrostAura.Libraries.Data | [![NuGet](https://img.shields.io/nuget/v/FrostAura.Libraries.Data.svg?style=for-the-badge)](https://www.nuget.org/packages/FrostAura.Libraries.Data/)[![NuGet](https://img.shields.io/nuget/dt/FrostAura.Libraries.Data.svg?style=for-the-badge)](https://www.nuget.org/packages/FrostAura.Libraries.Data/) |
| FrostAura.Libraries.Finance | [![NuGet](https://img.shields.io/nuget/v/FrostAura.Libraries.Finance.svg?style=for-the-badge)](https://www.nuget.org/packages/FrostAura.Libraries.Finance/)[![NuGet](https://img.shields.io/nuget/dt/FrostAura.Libraries.Finance.svg?style=for-the-badge)](https://www.nuget.org/packages/FrostAura.Libraries.Finance/) |
| FrostAura.Libraries.Http | [![NuGet](https://img.shields.io/nuget/v/FrostAura.Libraries.Http.svg?style=for-the-badge)](https://www.nuget.org/packages/FrostAura.Libraries.Http/)[![NuGet](https://img.shields.io/nuget/dt/FrostAura.Libraries.Http.svg?style=for-the-badge)](https://www.nuget.org/packages/FrostAura.Libraries.Http/) |
| FrostAura.Libraries.Security.OAuth | [![NuGet](https://img.shields.io/nuget/v/FrostAura.Libraries.Security.OAuth.svg?style=for-the-badge)](https://www.nuget.org/packages/FrostAura.Libraries.Security.OAuth/)[![NuGet](https://img.shields.io/nuget/dt/FrostAura.Libraries.Security.OAuth.svg?style=for-the-badge)](https://www.nuget.org/packages/FrostAura.Libraries.Security.OAuth/) |
| FrostAura.Libraries.Semantic.Core | [![NuGet](https://img.shields.io/nuget/v/FrostAura.Libraries.Intelligence.Semantic.Core.svg?style=for-the-badge)](https://www.nuget.org/packages/FrostAura.Libraries.Intelligence.Semantic.Core/)[![NuGet](https://img.shields.io/nuget/dt/FrostAura.Libraries.Intelligence.Semantic.Core.svg?style=for-the-badge)](https://www.nuget.org/packages/FrostAura.Libraries.Intelligence.Semantic.Core/) |

## Documentation
| Content | Description
| -- | -- |
| [Repo Structure](.docs/repo_structure.md) | The structuring of the repo.
| [Design](.docs/design.md) | The software architecture diagram(s) and design(s).
| [Workflow](.docs/workflow.md) | The software automated software pipeline(s).
| [Support & Contribute](.docs/support_contribute.md) | Basic queries, constributing to the repo and supporting the team(s) working on this open-source repo.

## Mermaid Diagrams
### AudioTranscriptionChain
```mermaid
classDiagram
    class AudioTranscriptionChain {
        +AudioTranscriptionChain(IServiceProvider, ISemanticKernelLanguageModelsDataAccess, ILogger<AudioTranscriptionChain>)
        +Task~string~ TranscribeAudioFileAsync(string, CancellationToken)
        +Task~string~ TranscribeAudioFilesAsync(string, CancellationToken)
    }
    AudioTranscriptionChain --> BaseChain
    BaseChain <|-- AudioTranscriptionChain
    AudioTranscriptionChain : +string QueryExample
    AudioTranscriptionChain : +string QueryInputExample
    AudioTranscriptionChain : +string Reasoning
    AudioTranscriptionChain : +List~Thought~ ChainOfThoughts
```

### TextToImageChain
```mermaid
classDiagram
    class TextToImageChain {
        +TextToImageChain(IServiceProvider, ISemanticKernelLanguageModelsDataAccess, ILogger<TextToImageChain>)
        +Task~string~ GenerateImageAndGetFilePathAsync(string, CancellationToken)
    }
    TextToImageChain --> BaseChain
    BaseChain <|-- TextToImageChain
    TextToImageChain : +string QueryExample
    TextToImageChain : +string QueryInputExample
    TextToImageChain : +string Reasoning
    TextToImageChain : +List~Thought~ ChainOfThoughts
```

### TextToSpeechChain
```mermaid
classDiagram
    class TextToSpeechChain {
        +TextToSpeechChain(IServiceProvider, ISemanticKernelLanguageModelsDataAccess, ILogger<TextToSpeechChain>)
        +Task~string~ SpeakTextAndGetFilePathAsync(string, CancellationToken)
    }
    TextToSpeechChain --> BaseChain
    BaseChain <|-- TextToSpeechChain
    TextToSpeechChain : +string QueryExample
    TextToSpeechChain : +string QueryInputExample
    TextToSpeechChain : +string Reasoning
    TextToSpeechChain : +List~Thought~ ChainOfThoughts
```

### GetFNBAccountBalancesChain
```mermaid
classDiagram
    class GetFNBAccountBalancesChain {
        +GetFNBAccountBalancesChain(IServiceProvider, ISemanticKernelLanguageModelsDataAccess, ILogger<GetFNBAccountBalancesChain>)
        +Task~string~ GetFNBAccountBalancesTableAsync(CancellationToken)
    }
    GetFNBAccountBalancesChain --> BaseChain
    BaseChain <|-- GetFNBAccountBalancesChain
    GetFNBAccountBalancesChain : +string QueryExample
    GetFNBAccountBalancesChain : +string QueryInputExample
    GetFNBAccountBalancesChain : +string Reasoning
    GetFNBAccountBalancesChain : +List~Thought~ ChainOfThoughts
```

### YouTubeShortFactualVideoGenerationChain
```mermaid
classDiagram
    class YouTubeShortFactualVideoGenerationChain {
        +YouTubeShortFactualVideoGenerationChain(IServiceProvider, ISemanticKernelLanguageModelsDataAccess, ILogger<YouTubeShortFactualVideoGenerationChain>)
        +Task~string~ GenerateDocumentaryVideoAsync(string, CancellationToken)
        +Task~string~ GenerateDocumentaryVideoWithStateAsync(string, Dictionary~string, string~, CancellationToken)
    }
    YouTubeShortFactualVideoGenerationChain --> BaseChain
    BaseChain <|-- YouTubeShortFactualVideoGenerationChain
    YouTubeShortFactualVideoGenerationChain : +string QueryExample
    YouTubeShortFactualVideoGenerationChain : +string QueryInputExample
    YouTubeShortFactualVideoGenerationChain : +string Reasoning
    YouTubeShortFactualVideoGenerationChain : +List~Thought~ ChainOfThoughts
```
