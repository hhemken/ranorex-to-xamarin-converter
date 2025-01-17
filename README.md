# Ranorex to Xamarin.UITest Converter

This tool automates the conversion of Ranorex test suites to Xamarin.UITest format. It handles .rxtst (test suite), .rxrec (recording), and .cs (C# code) files.

## Features

- Converts Ranorex test files to Xamarin.UITest format
- Processes entire project directories recursively
- Handles multiple file types:
  - .rxtst (Test Suite files)
  - .rxrec (Recording files)
  - .cs (C# code files)
- Generates detailed conversion logs
- Continues processing if individual files fail
- Creates complete Xamarin.UITest project structure

## Requirements

- .NET Core 3.1 or higher
- Visual Studio 2019 or higher (for using generated tests)
- Xamarin.UITest NuGet package (for running converted tests)

## Installation

1. Clone this repository
2. Build the solution:
```bash
dotnet build
```

## Usage

### Converting a Single File

```csharp
var converter = new RanorexToXamarinConverter(
    outputPath: "./XamarinTests",
    logPath: "./conversion_log.txt"
);

// Convert single file
converter.ConvertSingleFile("path/to/your/file.rxtst");
```

### Converting an Entire Project

```csharp
var converter = new RanorexToXamarinConverter(
    outputPath: "./XamarinTests",
    logPath: "./conversion_log.txt"
);

// Process entire directory
converter.ProcessRanorexDirectory("path/to/ranorex/project");
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
└── XamarinTests.csproj
```

## Logging

The converter creates a detailed log file containing:
- Timestamp for each action
- Success/failure status for each file
- Error messages and stack traces
- Summary of processed files

Example log output:
```
2025-01-16 10:30:00 - INFO: Starting directory processing: C:\RanorexProject
2025-01-16 10:30:01 - INFO: Processing file: TestSuite.rxtst
2025-01-16 10:30:01 - INFO: Successfully converted: TestSuite.rxtst
```

## Conversion Details

### Test Suite (.rxtst) Conversion
- Converts test suite structure to NUnit test fixtures
- Maintains test case organization
- Converts test case metadata

### Recording (.rxrec) Conversion
- Converts recorded actions to Xamarin.UITest commands
- Handles common actions:
  - Click/Touch → Tap
  - SetValue → EnterText
  - Keyboard actions → EnterText/PressEnter
  - Wait → WaitForElement
  - Validate → Assert with Query

### C# Code Conversion
- Converts Ranorex namespaces to Xamarin.UITest
- Updates element identification syntax
- Converts common Ranorex methods to Xamarin.UITest equivalents

## Limitations

1. Some Ranorex features don't have direct equivalents in Xamarin.UITest:
   - Mouse movement actions
   - Complex gesture recordings
   - Some validation types

2. Element identification strategies may need manual adjustment

3. Custom Ranorex plugins or extensions aren't converted

## Troubleshooting

### Common Issues

1. **File Conversion Fails**
   - Check the log file for specific error details
   - Verify file is not empty or corrupted
   - Ensure file uses standard Ranorex patterns

2. **Element Identification Issues**
   - Review converted element queries
   - Adjust identification strategies if needed
   - Consider using different locator types

3. **Missing Dependencies**
   - Ensure all required NuGet packages are installed
   - Verify .NET version compatibility

## Contributing

Contributions are welcome! Please:
1. Fork the repository
2. Create a feature branch
3. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For issues and feature requests, please:
1. Check the documentation
2. Review existing issues
3. Create a new issue if needed
First document:

Understanding AI Agents
AI agents are self-governing creatures that employ sensors to keep an eye on their environment, process information, and accomplish predefined goals. They can be anything from basic bots to sophisticated systems that can adjust and learn over time. Typical instances include recommendation engines like Netflix and Amazon’s, chatbots like Siri and Alexa, and self-driving cars from Tesla and Waymo.

Also essential in a number of sectors are these agents: UiPath and Blue Prism are examples of robotic process automation (RPA) programs that automate repetitive processes. DeepMind and IBM Watson Health are examples of healthcare diagnostics systems that help diagnose diseases and recommend treatments. In their domains, AI agents greatly improve productivity, precision, and customisation.

Why AI Agents are Important?
These agents play a critical role in improving our daily lives and accomplishing particular objectives.

AI agents are significant because they can:

lowering the amount of human labor required to complete routine operations, resulting in increased production and efficiency.
analyzing enormous volumes of data to offer conclusions and suggestions that support decision-making.
utilizing chatbots and virtual assistants to provide individualized interactions and assistance.
enabling complex applications in industries like as banking, transportation, and healthcare.
In essence, AI agents are pivotal in driving the next wave of technological advancements, making systems smarter and more responsive to user needs.

Applications and Use Cases of AI Agents
AI agents have a wide range of applications across various industries. Here are some notable use cases:

Customer Service: AI agents in the form of chatbots and virtual assistants handle customer inquiries, resolve issues, and provide personalized support. They can operate 24/7, offering consistent and efficient service.
Finance: Financial forecasting, algorithmic trading, and fraud detection are applications of AI agents. They perform trades based on market trends, examine transaction data, and spot questionable patterns.
Healthcare: AI agents assist in diagnosing diseases, recommending treatments, and monitoring patient health. They analyze medical data, provide insights, and support clinical decision-making.
Marketing: AI agents personalize marketing campaigns, segment audiences, and optimize ad spend. They analyze customer data, predict behavior, and tailor content to individual preferences.
Supply Chain Management: AI systems estimate demand, improve inventory levels, and simplify logistics. They examine information from manufacturers, suppliers, and retailers to guarantee smooth operations.
Brief Introduction of ReAct Pattern
The ReAct pattern operates in a loop of Thought, Action, Pause, Observation, Answer.

This loop allows the AI agent to reason about the input, act on it by leveraging external resources, and then integrate the results back into its reasoning process. By doing so, the AI agent can provide more accurate and contextually relevant responses, significantly expanding its utility.

The ReAct pattern is a potent design pattern that combines reasoning and action-taking skills to improve the capabilities of AI agents. LLMs such as GPT-3 or GPT-4 benefit greatly from this technique because it allows them to interface with other tools and APIs to carry out activities beyond their original programming.

The ReAct pattern operates in a cyclic loop consisting of the following steps:

Thought: The AI agent processes the input and reasons about what needs to be done. This involves understanding the question or command and determining the appropriate action to take.
Action: Based on the reasoning, the agent performs a predefined action. This could involve searching for information, performing calculations, or interacting with external APIs.
Pause: The agent waits for the action to be completed. This is a crucial step where the agent pauses to receive the results of the action performed.
Observation: The agent observes the results of the action. It analyzes the output received from the action to understand the information or results obtained.
Answer: The agent uses the observed results to generate a response. This response is then provided to the user, completing the loop.
Importance and Benefits of Using ReAct
The ReAct pattern is important for several reasons:

Enhanced Capabilities: By integrating external actions, the AI agent can perform tasks that require specific information or computations, thus enhancing its overall capabilities.
Improved Accuracy: The pattern allows the AI agent to fetch real-time information and perform accurate calculations, leading to more precise and relevant responses.
Flexibility: The ReAct pattern makes AI agents more flexible and adaptable to various tasks. They can interact with different APIs and tools to perform a wide range of actions.
Scalability: This pattern allows for the addition of new actions and capabilities over time, making the AI agent scalable and future-proof.
Real-World Applications: The ReAct pattern enables AI agents to be deployed in real-world scenarios where they can interact with dynamic environments and provide valuable insights and assistance.
Tools and Libraries Needed
Python is a versatile and powerful programming language that is widely used in AI and machine learning due to its simplicity and extensive library support. For building AI agents, several Python libraries are essential:

OpenAI API: This library allows you to interact with OpenAI’s language models, such as GPT-3 and GPT-4. It provides the necessary functions to generate text, answer questions, and perform various language-related tasks.
httpx: This is a powerful HTTP client for Python that supports asynchronous requests. It is used to interact with external APIs, fetch data, and perform web searches.
re (Regular Expressions): This module provides support for regular expressions in Python. It is used to parse and match patterns in strings, which is useful for processing the AI agent’s responses.
Introduction to OpenAI API and httpx Library
The OpenAI API is a robust platform that provides access to advanced language models developed by OpenAI. These models can understand and generate human-like text, making them ideal for building AI agents. With the OpenAI API, you can:

Generate text based on prompts
Answer questions
Perform language translations
Summarize text
And much more
The httpx library is an HTTP client for Python that supports both synchronous and asynchronous requests. It is designed to be easy to use while providing powerful features for making web requests. With httpx, you can:

Send GET and POST requests
Handle JSON responses
Manage sessions and cookies
Perform asynchronous requests for better performance
Together, the OpenAI API and httpx library provide the foundational tools needed to build and enhance AI agents, enabling them to interact with external resources and perform a wide range of actions.

Setting Up the Environment
Let us now set up the environment by following certain steps:

Step1: Installation of Required Libraries
To get started with building your AI agent, you need to install the necessary libraries. Here are the steps to set up your environment:

Install Python: Ensure you have Python installed on your system. You can download it from the official Python website:
Set Up a Virtual Environment: It’s good practice to create a virtual environment for your project to manage dependencies. Run the following commands to set up a virtual environment:
python -m venv ai_agent_env
source ai_agent_env/bin/activate  # On Windows, use `ai_agent_env\Scripts\activate`
Install OpenAI API and httpx: Use pip to install the required libraries:
pip install openai httpx
Install Additional Libraries: You may also need other libraries like re for regular expressions, which is included in the Python Standard Library, so no separate installation is required.
Step2: Setting Up API Keys and Environment Variables
To use the OpenAI API, you need an API key. Follow these steps to set up your API key:

Obtain an API Key: Sign up for an account on the OpenAI website and obtain your API key from the API section.
Set Up Environment Variables: Store your API key in an environment variable to keep it secure. Add the following line to your .bashrc or .zshrc file (or use the appropriate method for your operating system):
export OPENAI_API_KEY='your_openai_api_key_here'
Access the API Key in Your Code: In your Python code, you can access the API key using the os module:
import os
openai.api_key = os.getenv('OPENAI_API_KEY')
With the environment set up, you are now ready to start building your AI agent.

Building the AI Agent
Let us now build the AI agent.

Creating the Basic Structure of the AI Agent
To build the AI agent, we will create a class that handles interactions with the OpenAI API and manages the reasoning and actions. Here’s a basic structure to get started:

import openai
import re
import httpx

class ChatBot:
    def __init__(self, system=""):
        self.system = system
        self.messages = []
        if self.system:
            self.messages.append({"role": "system", "content": system})
    
    def __call__(self, message):
        self.messages.append({"role": "user", "content": message})
        result = self.execute()
        self.messages.append({"role": "assistant", "content": result})
        return result
    
    def execute(self):
        completion = openai.ChatCompletion.create(model="gpt-3.5-turbo", messages=self.messages)
        return completion.choices[0].message.content
This class initializes the AI agent with an optional system message and handles user interactions. The __call__ method takes user messages and generates responses using the OpenAI API.

Implementing the ReAct Pattern
To implement the ReAct pattern, we need to define the loop of Thought, Action, Pause, Observation, and Answer. Here’s how we can incorporate this into our AI agent:

Define the Prompt
prompt = """
You run in a loop of Thought, Action, PAUSE, Observation.
At the end of the loop you output an Answer.
Use Thought to describe your thoughts about the question you have been asked.
Use Action to run one of the actions available to you - then return PAUSE.
Observation will be the result of running those actions.

Your available actions are:
calculate:
e.g. calculate: 4 * 7 / 3
Runs a calculation and returns the number - uses Python so be sure to use floating point
syntax if necessary

wikipedia:
e.g. wikipedia: Django
Returns a summary from searching Wikipedia

simon_blog_search:
e.g. simon_blog_search: Django
Search Simon's blog for that term

Example session:
Question: What is the capital of France?
Thought: I should look up France on Wikipedia
Action: wikipedia: France
PAUSE

You will be called again with this:
Observation: France is a country. The capital is Paris.

You then output:
Answer: The capital of France is Paris
""".strip()
Define the query Function
action_re = re.compile('^Action: (\w+): (.*)
The query function runs the ReAct loop by sending the question to the AI agent, parsing the actions, executing them, and feeding the observations back into the loop.

Implementing Actions
Let us now look into the implementing actions.

Action: Wikipedia Search
The Wikipedia search action allows the AI agent to search for information on Wikipedia. Here’s how to implement it:

def wikipedia(q):
    response = httpx.get("https://en.wikipedia.org/w/api.php", params={
        "action": "query",
        "list": "search",
        "srsearch": q,
        "format": "json"
    })
    return response.json()["query"]["search"][0]["snippet"]
Action: Blog Search
The blog search action allows the AI agent to search for information on a specific blog. Here’s how to implement it:

def simon_blog_search(q):
    response = httpx.get("https://datasette.simonwillison.net/simonwillisonblog.json", params={
        "sql": """
        select
          blog_entry.title || ': ' || substr(html_strip_tags(blog_entry.body), 0, 1000) as text,
          blog_entry.created
        from
          blog_entry join blog_entry_fts on blog_entry.rowid = blog_entry_fts.rowid
        where
          blog_entry_fts match escape_fts(:q)
        order by
          blog_entry_fts.rank
        limit
          1
        """.strip(),
        "_shape": "array",
        "q": q,
    })
    return response.json()[0]["text"]
Action: Calculation
The calculation action allows the AI agent to perform mathematical calculations. Here’s how to implement it:

def calculate(what):
    return eval(what)
Adding Actions to the AI Agent
Next, we need to register these actions in a dictionary so the AI agent can use them:

known_actions = {
    "wikipedia": wikipedia,
    "calculate": calculate,
    "simon_blog_search": simon_blog_search
}
Integrating Actions with the AI Agent
To integrate the actions with the AI agent, we need to ensure that the query function can handle the different actions and feed the observations back into the reasoning loop. Here’s how to complete the integration:

def query(question, max_turns=5):
    i = 0
    bot = ChatBot(prompt)
    next_prompt = question
    while i < max_turns:
        i += 1
        result = bot(next_prompt)
        print(result)
        actions = [action_re.match(a) for a in result.split('\n') if action_re.match(a)]
        if actions:
            action, action_input = actions[0].groups()
            if action not in known_actions:
                raise Exception(f"Unknown action: {action}: {action_input}")
            print(" -- running {} {}".format(action, action_input))
            observation = known_actions[action](action_input)
            print("Observation:", observation)
            next_prompt = f"Observation: {observation}"
        else:
            return result
With this setup, the AI agent can reason about the input, perform actions, observe the results, and generate responses.

Testing and Debugging
Let us now follow the steps for testing and debugging.

Running Sample Queries
To test the AI agent, you can run sample queries and observe the results. Here are a few examples:

print(query("What does England share borders with?"))

print(query("Has Simon been to Madagascar?"))

print(query("Fifteen * twenty five"))

Debugging Common Issues
While testing, you might encounter some common issues. Here are a few tips to debug them:

API Errors: Ensure your API keys are correctly set and have the necessary permissions.
Network Issues: Check your internet connection and ensure the endpoints you are calling are reachable.
Incorrect Outputs: Verify the logic in your action functions and ensure they return the correct results.
Unhandled Actions: Make sure all possible actions are defined in the known_actions dictionary.
Improving the AI Agent
Let us now improve AI agents.

Enhancing Robustness and Security
To make the AI agent more robust and secure:

Validate Inputs: Ensure all inputs are properly validated to prevent injection attacks, especially in the calculate function.
Error Handling: Implement error handling in the action functions to gracefully manage exceptions.
Logging: Add logging to track the agent’s actions and observations for easier debugging.
Adding More Actions and Capabilities
To enhance the AI agent’s capabilities, you can add more actions such as:

Weather Information: Integrate with a weather API to fetch real-time weather data.
News Search: Implement a news search action to fetch the latest news articles.
Translation: Add a translation action using a translation API to support multilingual queries.
Real-World Applications
Customer Support: AI agents can handle customer inquiries, resolve issues, and provide personalized recommendations.
Healthcare: AI agents assist in diagnosing diseases, recommending treatments, and monitoring patient health.
Finance: AI agents detect fraud, execute trades, and provide financial advice.
Marketing: AI agents personalize marketing campaigns, segment audiences, and optimize ad spend.
Future Prospects and Advancements
The future of AI agents is promising, with advancements in machine learning, natural language processing, and AI ethics. Emerging trends include:

Autonomous Systems: More sophisticated autonomous systems capable of handling complex tasks.
Human-AI Collaboration: Enhanced collaboration between humans and AI agents for improved decision-making.
Ethical AI: Focus on developing ethical AI agents that prioritize privacy, fairness, and transparency.
Conclusion
In this comprehensive guide, we explored the concept of AI agents, their significance, and the ReAct pattern that enhances their capabilities. We covered the necessary tools and libraries, set up the environment, and walked through building an AI agent from scratch. We also discussed implementing actions, integrating them with the AI agent, and testing and debugging the system. Finally, we looked at real-world applications and future prospects of AI agents.

By following this guide, you now have the knowledge to create your own build AI agents from scratch. Experiment with different actions, enhance the agent’s capabilities, and explore new possibilities in the exciting field of artificial intelligence.

Key Takeaways
Understanding the core concepts and significance of AI agents.
Implementation of the ReAct pattern to allow AI agents to perform actions and reason about their observations.
Knowledge of the essential tools and libraries like OpenAI API, httpx, and Python regular expressions.
A detailed guide on building an AI agent from scratch, including defining actions and integrating them.
Techniques for effectively testing and debugging AI agents.
Strategies to enhance the AI agent’s capabilities and ensure its robustness and security.
Practical examples of how AI agents are used in various industries and their future advancements.



Second document:

The recent shift from LLM-powered chatbots to what the field now defines as agentic systems or agentic AI can be summarized with a good old saying: “Less talk, more action.”

Keeping up with advancement can be daunting, especially if you already have an existing business to run. Not to mention that the speed and complexity of advancement can make you feel like it’s the first day of school.

This piece provides an overview of AI agents based on their components and characteristics. The introductory section covers the components that form the term “AI agent” to create an intuitive definition. After establishing a definition, the following sections include an exploration of the evolution of the form factor of LLM applications, notably from traditional chatbots to agentic systems.

Overall, the key aim is to understand why AI agents are becoming increasingly important in the field of AI application development and how they differ from LLM-powered chatbots. By the end of this guide, you'll have a more thorough understanding of AI agents, their potential applications, and how they might impact workflows in your organization.

Table of contents

What is an AI agent?
From LLMs to AI agents
The key components and characteristics of an AI agent
Conclusion
What is an AI agent?
Illustration explaining the concept of AI Agents.
The two components of the term "AI agent" can give us a deeper understanding of its meaning. Let’s start with the easy one, artificial intelligence, also known as AI.

Artificial intelligence (AI)
 refers to non-biological forms of intelligence that are loosely based on the computational mimicry of human intelligence and aim to execute tasks that traditionally require human intellect. The primary method of providing intelligence to computational systems is through machine learning and deep learning techniques, where computer algorithms—specifically, layers of neural networks—learn patterns and features from provided datasets. AI systems are developed to tackle detection, classification, and predictive tasks, with content generation becoming a prominent problem domain due to the effectiveness of transformer-based foundation models. In some cases, these AI systems match human performance, and in particular scenarios, they exceed it.

The second component, "agent," is a familiar term used in both technological and human contexts, and understanding both perspectives can help clarify the concept of AI agents.

In computer science and technology: The term "agent" in computer science-based topics refers to an entity (software agent) with environmental awareness and perception enabled via sensors and an ability to act within its environment through action mechanisms. In this context, an agent is a computational system that:
Has autonomy to make decisions and take actions.
Can interact with its environment.
Can pursue goals or carry out tasks.
May learn or use knowledge to achieve its objectives.
In human context: The term "agent" typically refers to a person who acts on behalf of another person, group, or organization, usually playing the role of a proxy for decision-making, information gathering, and sharing. An agent's role and responsibilities could include:
Making decisions or taking actions for someone else with permitted authorization from the party being represented.
Formally representing a person in transactions and contractual scenarios, again, with authorization from the primary party.
An intermediary between multiple parties.
To understand AI agents, we must combine the characteristics of both technological and human contexts where the term "agent" is used while applying the guiding principles of artificial intelligence. This combination allows us to understand how and why AI agents are uniquely suited to perform tasks that typically require human intelligence and agency.

Based on this foundational context of the term AI agent, we can form the definition of AI agents.

An AI agent is a computational entity with an awareness of its environment that’s equipped with faculties that enable perception through input, action through tool use, and cognitive abilities through foundation models backed by long-term and short-term memory.

AI Agent diagram.
From LLMs to AI agents
Alright, you are an AI engineer now.

But before you head off and start building the next one billion revenue-generating AI product, let's take a couple of steps back and understand how we even got to AI agents in the first place. We will be looking at the changes we've seen in a short period of time when it comes to LLM applications.

The evolution of the form factor of LLM applications has been one of the fastest developments we've seen in modern applications.

The evolution of the form factor of LLM applications.
Traditional chatbots to LLM-powered chatbots
Chatbots are not new; you've probably interacted with a chatbot on a website before generative AI (gen AI) was coined. Traditional chatbots in the pre-gen AI era were fundamentally different from today's AI-powered conversational agents. Here's how they typically operated:

Heuritstic-based responses: “If this, then this” logic, or more formally rule-based logic, was the basis of the operating model of traditional chatbots. They were programmed with a set of predefined rules and decision trees to determine how to respond to user inputs.
Canned responses: Behind traditional chatbots was a set of pre-written responses that were shown to the user based on detecting certain keywords or phrases. This worked to an extent.
Human handoff: There was always a “Speak to a human” button in traditional chatbots, and to be honest, this hasn’t changed drastically. “Human in the loop” is still a much-needed mechanism for even agentic systems.
LLM-powered chatbot example.
LLM-powered chatbots were the first mainstream introduction to LLM applications. On 30th November 2022, OpenAI released ChatGPT, a web interface that provided a simple but familiar interface of traditional chatbots (area for input and output visualization), but behind this web interface was GPT-3.5, an LLM created by OpenAI and trained on a large corpus of internet.

GPT (Generative Pre-trained Transformer) is based on the Transformer architecture, which was introduced by Google in 2017. This architecture uses self-attention mechanisms to process input sequences, allowing the model to consider each word's context in relation to all other words in the input.

Unlike traditional chatbots, LLMs, such as GPT-3.5, can generate human-like text based on the input provided. One key differentiating factor of GPT-3.5 and other transformer-based LLMs is that the mechanism of content generation is not simply based on pattern recognition and feature extraction from the training dataset, but these foundation models can create what seems to be novel and contextually relevant content when prompted.

The introduction of GPT-powered chatbots like ChatGPT opened up a world of new possibilities, both for enterprise and commercial use cases. Notable use cases include code generation, content creation, improved customer service, etc. The capabilities of LLM-powered chatbots marked a significant shift from traditional rule-based chatbots to more flexible, intelligent, and capable AI assistants.

Despite their advanced capabilities, LLM-powered chatbots still faced certain limitations. One significant challenge was personalization. These systems struggled to maintain consistent, personalized interactions over extended conversations or multiple sessions. Even more concerning was the capacity for LLMs to synthesize responses that were human-like and coherent yet inaccurate. This phenomenon became a cause for concern, mainly because these systems began providing incorrect information with high confidence, a phenomenon now known as “hallucination.”

It's important to understand that when an LLM "hallucinates," it's not malfunctioning but rather doing exactly what it has been trained to do: generate the next output token based on a set of probabilities informed by the input tokens and its training data. This process can sometimes lead to plausible-sounding but factually incorrect outputs.

Addressing these limitations became a key focus in the development of more advanced AI systems, leading to the exploration of techniques that can “ground” the output of LLMs. One prominent technique is 
retrieval-augmented generation or RAG
.

LLM-powered chatbots to RAG chatbots
RAG is a technique that utilizes information retrieval methods to locate and provide relevant data that is then combined with user prompts and fed as input to LLMs. This process ensures that the output generated by the LLM is based on both:

Non-parametric knowledge: Information retrieved from external data sources in response to the specific query or context; this is usually real-time data spruced from the internet or proprietary data
Parametric knowledge: The inherent knowledge embedded in the LLM's parameters during its training
By leveraging both these sources of information, RAG aims to produce more accurate, up-to-date, and contextually relevant responses. This approach mitigates some limitations of pure LLM-based systems, such as hallucinations or outdated information, by grounding the model's responses in retrievable, verifiable data.

LLM-powered chatbots to RAG chatbots example.
The effort to improve the output of LLMs had multiple fronts, and one of these was prompt engineering. Prompt engineering refers to the practice of composing input queries to LLMs that steer the output toward desired characteristics, such as improved accuracy, relevance, and specificity. This technique involves carefully crafting the initial prompt given to an LLM to ensure the output is more precise, contextually appropriate, and task-specific responses.

A few prompt engineering techniques have emerged, such as in-context learning, chain of thought (CoT), and ReAct (Reason and Act).

In-context learning: Leveraging the generalization capabilities of LLMs, in-context learning involves the provision of input-output pairs that demonstrate the task to be solved and the desired output. This technique can be implemented in two main ways:

One-shot learning: Providing a single input-output pair as an example
Few-shot learning: Providing multiple input-output pairs as examples
The process typically ends with an input that doesn't have a corresponding output. Based on the provided examples, the LLM generates an output that is conditioned and guided by the input-output pairs given in the prompt.

This approach allows the LLM to adapt to specific tasks or styles without fine-tuning the model's parameters. Instead, it relies on the model's ability to recognize patterns and apply them to new, similar situations within the same context.

While in-context learning prompting techniques enabled LLMs to generalize to new tasks, subsequent developments like chain-of-thought and ReAct prompting leveraged the emergent reasoning and planning capabilities of LLMs. CoT enabled LLMs to decompose complex tasks into smaller, simpler sub-parts through a step-by-step reasoning process. ReAct combines a LLM's ability to reason with action planning.

RAG chatbot to AI agents
As LLMs scaled to hundreds of billions of parameters, they exhibited increasingly sophisticated emergent abilities. These include advanced reasoning, multi-step planning, and tool use or function calling.

Tool use, sometimes called “function calling,” refers to an LLM's ability to generate a structured output or schema that specifies the selection of one or more functions from a predefined set and the assignment of appropriate parameter values for these functions. The tool use’s capability within an LLM is dependent on an input prompt describing an objective or task and a suite of function definitions provided to the LLM, typically in JSON format.

The LLM analyzes the input and function definitions to determine which function(s) to invoke and how to populate their parameters. This structured output can then be used by an external system to execute the actual function calls.

What is a tool?

Generally, anything that can be programmatically defined and invoked can be defined as a tool with an accompanying JSON definition provided to an LLM. Therefore, RAG capabilities can be a tool, and API calls to external systems can also be tools.

LLMs that have access to tools and function calling capabilities are sometimes referred to as “tool-augmented LLMs,” but notably, the combination of advanced reasoning, multi-step planning, and tool-use capabilities facilitated the emergence of AI agents. The last piece of the puzzle is the environment an AI agent resides within. AI agents operate within an iterative execution environment that enables an objective-driven system that can iterate on a previous execution output that informs the current execution, and this can be different from the conversational-based system interface.

LLMs with the combination of advanced reasoning, multi-step planning, and tool-use capabilities facilitated the emergence of AI agents.
Agentic systems or compound AI systems are currently emerging as a paradigm of implementation for modern AI applications that are complex compared to LLM-based chatbots and multifaceted in their integration with system components. Agentic systems can be defined as computational architecture encompassing one or more AI agents with autonomous decision-making capabilities, able to access and utilize various system components and resources to achieve defined objectives while adapting to environmental feedback. More resources on understanding agentic systems will be provided in the near future.

Another key term to be aware of is “agentic RAG,” which refers to a paradigm that leverages LLMs' routing, tool use, reasoning, and planning capabilities alongside information retrieval based on comparing query and stored data semantics. This system paradigm enables the development of dynamic LLM applications that can access various tools to execute queries, decompose tasks, and solve complex problems.

To truly understand AI agents, it’s important to consider their components, characteristics, and capabilities.

The key components and characteristics of an AI agent
The key components and characteristics of an AI agent diagram.
The components of an AI agent are the crucial parts that form its architecture and enable its functionality. These components work together to process information, make decisions, and interact with the environment. The primary components include the brain, action, and perception modules, each playing a vital role in the agent's operation.

An agent is a computational entity composed of several integrated components, including the brain, perception and action components. These components work cohesively to enable the agent to achieve its defined objectives and goals.

Brain: This component of an agent's architecture is responsible for the cognitive abilities of an agent, including its ability to reason, plan, and make decisions. The brain of an agent is essentially the LLM. The emergent abilities of LLMs provide the agent abilities such as reasoning, comprehension, planning, etc. At the same time, and similar to humans, the brain component encapsulates different modules such as memory, profiler, and knowledge.

The memory module stores agent interactions with other external entities or systems. This stored information can be recalled to inform future execution steps and act accordingly based on historical interactions. The profiler module enables the agent to take on certain roles based on descriptions of the characteristics of roles that are intended to condition the agent into a set of behaviors.

The knowledge module within the brain component of an agent enables the storage and retrieval of domain-specific, relevant, and useful information to leverage in planning and taking action toward an objective.

Action: The capability of an agent to react to its environment and new information is facilitated by the action component, which includes modules that enable the agent to generate responses and invoke other systems. An LLM-based agent is equipped to decompose tasks into steps using processes within the brain component. Each step can be associated with a tool from the agent's arsenal. With LLMs' reasoning and planning capabilities, the agent can effectively decide when to utilize a tool at each step.

Perception: This component is solely responsible for capturing and processing inputs from the agent’s environment. Within the scope of agentic systems and interactions, inputs come in various forms, but the primary inputs provided to agents are auditory, textual, and visual.

The characteristics of an AI agent are the distinctive features and behaviors that define its capabilities and operational mode. These characteristics determine how an AI agent interacts with its environment, processes information, and achieves its objectives. Key characteristics include autonomy, proactivity, reactivity, and interactivity.

Below is a summary of the key characteristics of agents:

AI agents are reflective and proactive: AI agents utilize advanced reasoning patterns to tackle complex problems. They employ techniques like ReAct and chain-of-thought to decompose tasks, plan actions, and reflect on outcomes. Leveraging LLMs' emergent properties of reasoning and planning, these agents continuously adapt their strategies based on feedback, previous execution outputs, and environmental inputs. This iterative process of planning, execution, and reflection enables AI agents to execute input objectives efficiently.
AI agents are interactive: In some cases, AI agents may be required to interact with other agents within the same system or external systems, and often, they are expected to engage with humans for feedback or review of outputs from execution steps. AI agents can also comprehend the context of outputs from other agents and humans and change their course of action and next steps. The interactivity of AI agents extends into the undertaking of personas or roles to drive and condition the actions of the AI agents toward predictability based on the adopted role. In a multi-agent environment, this enables the mimicry of societal roles and collaboration based on role definitions.
AI agents are autonomous and reactive: Their autonomous characteristics enable them to perform actions based on both internal processing results and external observations, often without requiring explicit external commands. This reactivity is facilitated by two key capabilities: tool use and input processing. These capabilities allow AI agents to dynamically respond to changes in their environment or task conditions, adjusting their behavior and actions accordingly.
Conclusion
In our exploration, we've developed an understanding of AI agents and their characteristics and even provided a working definition. However, it's crucial to note an important caveat: There is currently no consolidated industry standard for what precisely constitutes an AI agent in today's rapidly evolving AI landscape.

Instead, the industry has generally agreed that the classification of a system as an AI agent lies on a spectrum or continuum. This nuanced perspective acknowledges that AI systems can exhibit varying degrees of agency, autonomy, and capability.

This is where the term "agentic" enters the discussion. "Agentic" refers to the degree to which an AI system demonstrates agent-like qualities. These qualities might include the:

Level of autonomy in decision-making.
Ability to interact with and manipulate its environment.
Capacity for goal-oriented behavior.
Adaptability to new situations.
Degree of proactive behavior.
This continuum-based understanding allows for a more flexible and inclusive approach to categorizing AI systems. It recognizes that as AI technology advances, the line between "simple" AI systems and fully-fledged AI agents may become increasingly blurred.

Where are the value and impact of the new form factor of LLM application realized?

When it comes to software and application development, we tend to focus on value and impact, as well as the return on investment made in early exploration and experimentation efforts on AI agents and agentic systems. Primarily, we see value being realized around productivity gains through the automation of manual processes. Manual approvals, documentation, and reviews are embedded within most workflows in enterprise organizations. Agentic systems are showing early potential in automating—or, in other words, "agentifying"—aspects of an existing workflow that are repetitive.

Another value of agentic systems is the alleviation of decision-making within enterprise workflows. AI agents can be prompted with rules and guidelines that steer their decision-making capabilities when they are embedded within agentic systems and compound AI systems. But even more value and impact can be observed in what can be described as bringing the everyday individual closer to the system without the need for technical knowledge gain, and this is because agentic systems allow for an interface that is text- and image-driven to be a primary driver for calling and execution of the system functionality. The versatility of inputs from foundation models enables agentic systems to be steered by natural language, reducing the technical complexity of system interaction.

Where are the efforts of players in the 
AI stack
 currently placed?

Reliability, scalability, and performance of AI agents are areas of focus for the key players in the AI industry who are attempting to provide solutions. Approaches to solving these focus areas include increasing the parameters in foundation models that enable reasoning capabilities in AI agents or developing tools to orchestrate workflows within a system in which an AI agent resides.




In light of thsese two documents, please write an ai agent in python that search through the GitHub site to find useful code relevant to a prompt.




