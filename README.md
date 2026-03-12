# Unity AI Chat Server

A FastAPI server that bridges a Unity 3D application with a local Ollama LLM, enabling an AI character to chat, navigate rooms, dance, and sing within a Unity apartment scene.

## Prerequisites

- **Python 3.8+**
- **Ollama** installed and running locally — [Download Ollama](https://ollama.com)
- The **Llama 3.2** model pulled in Ollama

## Setup

### 1. Clone the Repository

```bash
git clone https://github.com/HARSH-1607/Test1.git
cd Test1
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
  "user_input": "Can you go to the kitchen?",
  "environment_context": "standing in the living room"
}
```

**Response (JSON):**

```json
{
  "response": "[MOVE:Kitchen] Sure, I'll head to the kitchen right away!"
}
```

| Field                | Type   | Description                                                  |
| -------------------- | ------ | ------------------------------------------------------------ |
| `user_input`         | string | The player's message to the AI character                     |
| `environment_context`| string | *(optional)* Description of the character's current context  |

### Character Actions

The AI character supports the following special actions:

| Action   | Trigger                        | Response Format                          |
| -------- | ------------------------------ | ---------------------------------------- |
| **Move** | Ask the character to go somewhere | `[MOVE:RoomName] ...`                 |
| **Dance**| Ask the character to dance     | `[DANCE] ...`                            |
| **Sing** | Ask the character to sing      | `[SING] ...` (includes a short 2-line rhyming song) |

**Available Rooms:** `Kitchen`, `Bedroom`, `Living Room`

## Configuration

| Setting          | Default                                  | Location     |
| ---------------- | ---------------------------------------- | ------------ |
| Server host      | `0.0.0.0`                                | `main.py:50` |
| Server port      | `8000`                                   | `main.py:50` |
| Ollama URL       | `http://localhost:11434/api/generate`     | `main.py:13` |
| LLM model        | `llama3.2`                               | `main.py:34` |
| Available rooms  | `Kitchen, Bedroom, Living Room`          | `main.py:18` |
