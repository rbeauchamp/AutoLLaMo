# AutoLLaMo: Autonomous LLM AI Assistants, written in C#

AutoLLaMo is an experimental open-source application that uses large language models (LLMs) like GPT-4 to create autonomous assistants. By providing LLMs with a toolset to interact with the world, AutoLLaMo aims to unlock the full potential of autonomous AI.

## Table of Contents

- [Acknowledgements](#-acknowledgements)
- [Features](#-features)
- [Prerequisites](#%EF%B8%8F-prerequisites)
- [Quick Start](#-quick-start)
- [Limitations](#-limitations)
- [Contributing](#-contributing)
- [Code of Conduct](#-code-of-conduct)
- [Disclaimer](#-disclaimer)

## 🙌 Acknowledgements

We would like to thank the [Auto-GPT](https://github.com/Significant-Gravitas/Auto-GPT) contributors for their work and inspiration. AutoLLaMo builds from their efforts and enables contributions from C# developers in this space. Please visit [their website](https://news.agpt.co/) to learn more and support their project.

## 🌟 Features

- 🔄 Expands its capabilities by generating its own Plugins
- 🔌 Extensibility with Plugins

## 🛠️ Prerequisites

- [.NET 7.0 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [GPT-4 API access](https://openai.com/waitlist/gpt-4-api)

## 🚀 Quick Start

1. Set up your OpenAI [API Key](https://platform.openai.com/account/api-keys)
2. Clone this repository
``` shell
git clone https://github.com/rbeauchamp/AutoLLaMo.git
```
3. Configure your environment variables:
    - Locate the file named `.env.template` within the root `/AutoLLaMo` directory.
    - Duplicate this file and rename it as `.env` by removing the template extension.
    - Open the newly created `.env` file using a text editor.
    - Search for the line containing `OpenAIApiKey=`
    - After the "=", input your unique OpenAI API Key (excluding any quotes or spaces).
    - Search for the line containing `OutputDirectory=`
    - After the "=", input the directory where you would like AutoLLaMo to write files. For example `${USERPROFILE}\Documents\AutoLLaMoOutput`
    - Save the `.env` file and close it. (Do not add this file to source control since it contains your secret keys.)
4. Open a terminal in the root `/AutoLLaMo` directory and build the image using Docker Compose
``` shell
docker-compose build autollamo
```
5. Run AutoLLaMo
``` shell
docker-compose run --rm autollamo
```

## 🚧 Limitations

1. Only a pre-alpha version, committed to GitHub recently. For example, there is no long or short term memory management yet and there are very few plugins.
2. Like Auto-GPT it may not perform well in complex, real-world business scenarios.
3. Quite expensive to run, so set and monitor your API key limits with OpenAI!

## 📝 Contributing

We value all the people who are interested in contributing to AutoLLaMo. If you want to contribute to this project, be sure to review [Contributing to AutoLLaMo](CONTRIBUTING.md). It contains the necessary information about how you can start contributing and the guidelines we follow.

## 👥 Code of Conduct

We expect all our contributors to abide by the [Code of Conduct](CODE_OF_CONDUCT.md). Please read the full text so that you can understand what actions will not be tolerated.

## 🛡 Disclaimer
AutoLLaMo is provided "as is", without guarantees. It runs autonomously and users are responsible for its supervision and all associated risks, including potential data loss or system failure. The creators of AutoLLaMo are not liable for any issues arising from its use.

This software does not guarantee data privacy or security. Users are advised to not input sensitive or personal data.

**OpenAI APIs can incur high costs due to token usage.** Users should monitor their token usage and costs, set up limits or alerts, and manage external services to avoid unexpected charges.

AutoLLaMo may produce content or actions inconsistent with legal or business standards due to its autonomous nature. Users must ensure compliance with laws, regulations, and ethical norms when using this software's output.

By using AutoLLaMo, users agree to protect the developers, contributors, and affiliated parties against any claims or damages arising from their use of the software.

This software is distributed under the MIT License. By using this software, you agree to the terms of this license.
