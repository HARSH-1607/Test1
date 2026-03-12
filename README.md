# Unity AI Chat Server

A FastAPI server that bridges a Unity 3D application with a local Ollama LLM, enabling an AI character to chat and navigate within a Unity scene.

## Prerequisites

- **Python 3.8+**
- **Ollama** installed and running locally — [Download Ollama](https://ollama.com)
- The **Llama 3.2** model pulled in Ollama

## Setup

### 1. Clone the Repository

```bash
git clone <your-repo-url>
cd Test
```

### 2. Create a Virtual Environment

```bash
python -m venv .venv
```

Activate it:

- **Windows:** `.venv\Scripts\activate`
- **macOS / Linux:** `source .venv/bin/activate`

### 3. Install Dependencies

```bash
pip install fastapi uvicorn pydantic requests
```

### 4. Pull the Ollama Model

Make sure Ollama is running, then pull the model:

```bash
ollama pull llama3.2
```

## Starting the Server

```bash
python main.py
```

The server will start on **http://localhost:8000**.

## API Usage

### `POST /chat`

Send a chat message to the AI character.

**Request Body (JSON):**

```json
{
  "user_input": "Hey, can you check the computer?",
  "environment_context": "standing near the bookshelf"
}
```

**Response (JSON):**

```json
{
  "response": "[MOVE:Computer] Sure, I'll head over to the computer now."
}
```

| Field                | Type   | Description                                      |
| -------------------- | ------ | ------------------------------------------------ |
| `user_input`         | string | The player's message to the AI character          |
| `environment_context`| string | *(optional)* Description of the character's current context |

## Configuration

| Setting          | Default                                  | Location     |
| ---------------- | ---------------------------------------- | ------------ |
| Server host      | `0.0.0.0`                                | `main.py:47` |
| Server port      | `8000`                                   | `main.py:47` |
| Ollama URL       | `http://localhost:11434/api/generate`     | `main.py:13` |
| LLM model        | `llama3.2`                               | `main.py:31` |
| Available objects | `Computer, Bookshelf, Window, Bed`       | `main.py:18` |
