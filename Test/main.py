from fastapi import FastAPI
from pydantic import BaseModel
import requests

app = FastAPI()

# Data structure matching the JSON Unity sends
class ChatRequest(BaseModel):
    user_input: str
    environment_context: str = ""

# The default local address for Ollama
OLLAMA_URL = "http://localhost:11434/api/generate"

@app.post("/chat")
async def chat_with_character(data: ChatRequest):
    # The rooms must exactly match the names of your RoomWaypoints in Unity
    available_rooms = "Kitchen, Bedroom, Living Room"

    # The "Brain" of your character
    system_prompt = (
        f"You are a helpful AI character in a 3D apartment. "
        f"Context: {data.environment_context}. "
        f"You can move to these rooms: {available_rooms}. "
        f"RULES: "
        f"1. If you want to move to a room, or the user asks you to, you MUST start your sentence with [MOVE:RoomName]. "
        f"2. If the user asks you to dance, you MUST start your sentence with [DANCE]. "
        f"3. If the user asks you to sing, you MUST start your sentence with [SING] and write a short 2-line rhyming song. "
        f"4. Otherwise, just reply normally. "
        f"5. Keep your responses conversational and strictly limited to 1 or 2 short sentences."
    )

    payload = {
        "model": "llama3.2",
        "prompt": f"{system_prompt}\nPlayer: {data.user_input}\nCharacter:",
        "stream": False
    }

    try:
        response = requests.post(OLLAMA_URL, json=payload)
        response_data = response.json()
        return {"response": response_data.get("response", "Error generating response")}
    except Exception as e:
        # If Ollama is closed or crashes, this sends the error back to Unity gracefully
        return {"error": str(e)}

if __name__ == "__main__":
    import uvicorn
    # Runs the server locally on port 8000
    uvicorn.run(app, host="0.0.0.0", port=8000)