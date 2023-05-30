# AutoLLaMo: Autonomous LLM AI Assistants, written in C#
An experimental open-source application that creates assistants, powered by LLMs such as GPT-4, to autonomously achieve your goals. By providing LLMs with a toolset with which to interact with the world, this project explores the full potential of autonimous AI.

## 🙌 Acknowledgements
We would like to thank the [Auto-GPT](https://github.com/Significant-Gravitas/Auto-GPT) contributors for their work and inspiration. AutoLLaMo builds from their efforts and enables contributions from C# developers in this space. Please visit [their website](https://news.agpt.co/) to learn more and support their project.

## 🌟 Features

- 🔄 Expands its capabilities by generating its own Plugins
- 🔌 Extensibility with Plugins

## 🛠️ Prerequisites
- [.NET 7.0 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

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
    - Search for the line containing `OpenAIApiKey=`.
    - After the "=", input your unique OpenAI API Key (excluding any quotes or spaces).
    - Search for the line containing `OutputDirectory=`.
    - After the "=", input the directory where you would like AutoLLaMo to write files. For example `${USERPROFILE}\Documents\AutoLLaMoOutput`.
    - Save the `.env` file and close it. (Do not add this file to source control since it contains your secret keys.)

5. Open a terminal in the root `/AutoLLaMo` directory and build the image using Docker Compose

``` shell
docker-compose build autollamo
```

6. Run AutoLLaMo
``` shell
docker-compose run --rm autollamo
```

## 🚧 Limitations

1. Only a pre-alpha version, committed to GitHub recently. For example, there is no long or short term memory management yet and there are very few plugins.
2. Like Auto-GPT it may not perform well in complex, real-world business scenarios.
3. Quite expensive to run, so set and monitor your API key limits with OpenAI!

## ⚖️ Disclaimer
AutoLLaMo is an experimental project provided "as is" without any warranties, express or implied. By using this software, you agree to assume all risks related to its use, including but not limited to data loss, system failure, costs, or other potential issues. The authors of AutoLLaMo do not accept responsibility or liability for any losses, damages, or other consequences resulting from using this software. You are solely responsible for decisions and actions performed by AutoLLaMo.

**Please note that using OpenAI APIs can be costly due to token usage.** By using this project, you acknowledge responsibility for monitoring and managing your token usage and associated costs. Therefore, it is strongly recommended to regularly check your OpenAI API and other external service usage and set up necessary limits or alerts to prevent unexpected charges.

As an autonomously-driven software application, AutoLLaMo may generate content or take actions that do not align with real-world business practices or legal requirements. You are responsible for ensuring that actions or decisions based on this software's output comply with all applicable laws, regulations, and ethical standards. Therefore, please exercise caution and use your best judgment when implementing or relying on AutoLLaMo's output or actions.

By using AutoLLaMo, you agree to indemnify, defend, and hold harmless the developers, contributors, and any affiliated parties from and against any and all claims, damages, losses, liabilities, costs, and expenses (including reasonable attorneys' fees), whether in an action of contract, tort or otherwise, arising from your use of this software.
