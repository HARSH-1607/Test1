# Nexus AI: Local LLM Bridge for Unity 🤖🎮

**Nexus AI** is a high-performance middleware built with **FastAPI** that bridges Unity 3D environments with local Large Language Models (LLMs) via **Ollama**. It enables 3D agents to process natural language, maintain spatial awareness, and execute complex environmental actions—such as navigation, animations, and singing—in real-time.

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
git clone [https://github.com/HARSH-1607/Nexus-AI-Unity.git](https://github.com/HARSH-1607/Nexus-AI-Unity.git)
cd Nexus-AI-Unity

# Create and activate a virtual environment
python -m venv .venv
source .venv/bin/activate  # macOS/Linux
.venv\Scripts\activate     # Windows

# Install dependencies
pip install fastapi uvicorn pydantic requests
