# Unity-LLM-Observer 🤖🎮

**Unity-LLM-Observer** is a high-performance middleware built with **FastAPI** that bridges Unity 3D environments with local Large Language Models (LLMs) via **Ollama**. It enables 3D agents to process natural language, maintain spatial awareness, and execute complex environmental actions—such as navigation, animations, and singing—in real-time.

---

## 🌟 Key Features
* **Local Inference:** Optimized for `Llama 3.2` via Ollama for zero-latency, privacy-focused, and cost-free AI interaction.
* **Spatial Awareness:** The AI understands its environment (Kitchen, Bedroom, etc.) and can trigger movement via specialized action tokens.
* **Action-Oriented API:** Beyond simple chat, the server parses intent to return executable command tags like `[MOVE]`, `[DANCE]`, and `[SING]`.
* **Unity-Ready:** Designed for seamless integration with C# `UnityWebRequest` and JSON serialization.

---

## 🛠️ Tech Stack
* **Backend:** Python 3.10+, FastAPI, Uvicorn
* **AI Engine:** Ollama (Llama 3.2)
* **Communication:** JSON over REST API
* **Frontend:** Unity Engine (C#)

---

## 🚀 Quick Start

### 1. Prerequisites
* [Ollama](https://ollama.com) installed and running.
* Pull the required model:
    ```bash
    ollama pull llama3.2
    ```

### 2. Installation
```bash
# Clone the repository
git clone [https://github.com/HARSH-1607/Unity-LLM-Observer.git](https://github.com/HARSH-1607/Unity-LLM-Observer.git)
cd Unity-LLM-Observer

# Create and activate a virtual environment
python -m venv .venv
source .venv/bin/activate  # macOS/Linux
.venv\Scripts\activate     # Windows

# Install dependencies
pip install fastapi uvicorn pydantic requests
3. Run the ServerBashpython main.py
The server will start on http://localhost:8000.📡 API DocumentationPOST /chatThe primary endpoint for player-to-agent communication.Request Body:JSON{
  "user_input": "I'm hungry, let's go to the kitchen.",
  "environment_context": "The character is currently in the Living Room."
}
System Response:JSON{
  "response": "[MOVE:Kitchen] That sounds like a plan! I'll head to the kitchen now. What are we making?"
}
Supported Action TokensThe LLM is programmed to wrap its intent in specific tokens that the Unity client parses to trigger animations and logic:ActionToken FormatDescriptionNavigation[MOVE:RoomName]Triggers pathfinding to Kitchen, Bedroom, or Living Room.Animation[DANCE]Triggers the character's dancing state machine.Performance[SING]Generates a short rhyme and triggers singing audio/animation.⚙️ ConfigurationYou can modify the environment and model settings in main.py:Ollama API: http://localhost:11434/api/generateDefault Model: llama3.2Host/Port: 0.0.0.0:8000
# Install dependencies
pip install fastapi uvicorn pydantic requests
