// Name: Paul Lewis Marcos
// Date: February 20, 2026
// Assignment: CS 152 Project - Programming Paradigms
// Description: Manages the chat UI, sending user input to a local LLM character and
//              displaying the AI response. Also controls the hide/show toggle for the panel.

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    // Reference to the LLM character that handles AI chat
    public LLMUnity.LLMCharacter llmCharacter;

    // Input field where the user types their question
    public TMPro.TMP_InputField playerText;

    // Text component that displays the AI's response
    public TMPro.TMP_Text aiText;

    // ScrollRect wrapping the AI response text so the user can scroll through it
    public ScrollRect scrollView;

    // The submit button the user clicks to send their message
    public UnityEngine.UI.Button submit;

    // The panel containing the chat UI (input field, AI text, submit button)
    public GameObject chatPanel;

    // The hide/show toggle button that stays visible at all times
    public UnityEngine.UI.Button hideButton;

    // Stops generation and clears both text fields
    public UnityEngine.UI.Button clearButton;

    private bool isUIVisible = true;
    private Coroutine scrollCoroutine;

    void Start()
    {
        // Listen for when the submit button is clicked
        submit.onClick.AddListener(OnSubmitButtonClick);

        // Listen for when the hide button is clicked
        hideButton.onClick.AddListener(OnHideButtonClick);

        // Listen for when the clear button is clicked
        clearButton.onClick.AddListener(OnClearButtonClick);
    }

    // Called when the user clicks the submit button
    void OnSubmitButtonClick()
    {
        // Make sure the LLM character is assigned in the Inspector
        if (llmCharacter == null)
        {
            Debug.LogError("LLM Character is not assigned in the Inspector.");
            return;
        }

        // Don't send anything if the input field is empty
        if (playerText == null || string.IsNullOrWhiteSpace(playerText.text))
        {
            Debug.LogWarning("Player input is empty.");
            return;
        }

        // Show a placeholder message while waiting for the AI to respond
        if (aiText != null)
        {
            aiText.text = "Thinking/Generating...";
        }

        // Send the player's text to the LLM and handle the reply when it comes back
        llmCharacter.Chat(playerText.text, HandleReply);
    }

    // Toggles the chat panel on/off
    void OnHideButtonClick()
    {
        isUIVisible = !isUIVisible;
        chatPanel.SetActive(isUIVisible);
    }

    // Stops any active generation and clears both text fields
    void OnClearButtonClick()
    {
        llmCharacter.CancelRequests();
        aiText.text = "";
        playerText.text = "";
    }

    // Called by the LLM on each streamed token and when generation finishes
    private void HandleReply(string reply)
    {
        aiText.text = reply;

        // Only start one scroll coroutine per frame to avoid freezing from
        // running expensive layout operations on every streamed token
        if (scrollCoroutine != null)
            StopCoroutine(scrollCoroutine);
        scrollCoroutine = StartCoroutine(ScrollToBottomNextFrame());
    }

    // Waits one frame so the layout has time to update, then scrolls to the bottom
    private IEnumerator ScrollToBottomNextFrame()
    {
        yield return new WaitForEndOfFrame();
        LayoutRebuilder.ForceRebuildLayoutImmediate(scrollView.content);
        scrollView.verticalNormalizedPosition = 0f;
    }
}