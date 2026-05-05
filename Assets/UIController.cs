using UnityEngine;
using UnityEngine.UI;       
using TMPro;               

public class UIController : MonoBehaviour
{
    // These come from the assignment 
    // GIVEN CODE
    public LLMUnity.LLMCharacter llmCharacter;

    // GIVEN CODE
    public TMPro.TMP_InputField playerText;

    // GIVEN CODE
    public TMPro.TMP_Text aiText;

    // GIVEN CODE
    public UnityEngine.UI.Button submit;

    void Start(){
        // GIVEN CODE
        submit.onClick.AddListener(OnSubmitButtonClick);
    }

    // Submit button.
    void OnSubmitButtonClick() {

        if (llmCharacter == null)
        {
            Debug.LogError("LLM Character is not assigned in the Inspector.");
            return;
        }

        // Check that the player input field is assigned and not empty.
        if (playerText == null || string.IsNullOrWhiteSpace(playerText.text)) {
            Debug.LogWarning("Player input is empty.");
            return;
        }

        // Show a temporary message 
        if (aiText != null) {
            aiText.text = "Thinking/Generating...";
        }

        
        llmCharacter.Chat(playerText.text, HandleReply);
    }


    
    private void HandleReply(string reply)
    {
        // Debug Log
        Debug.Log(reply);
        aiText.text = reply;
    }
}
