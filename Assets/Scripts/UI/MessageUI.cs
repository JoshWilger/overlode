using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageUI : MonoBehaviour
{
    [SerializeField] private UIController uiController;
    [SerializeField] private MessageClass[] messages;
    [SerializeField] private Sprite[] faces;
    [SerializeField] private Button nextButton;
    [SerializeField] private Image characterImage;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private GameObject focus;

    public int currentMessageIndex;

    // Start is called before the first frame update
    private void Start()
    {
        currentMessageIndex = 0;
    }

    private void OnEnable()
    {
        focus.SetActive(true);
        NextText();
        nextButton.onClick.AddListener(NextText);
    }

    private void OnDisable()
    {
        nextButton.onClick.RemoveAllListeners();
        currentMessageIndex++;
        uiController.Paused(false);
    }

    private void NextText()
    {
        var currentMessage = messages[currentMessageIndex];

        var index = Array.IndexOf(currentMessage.paragraphs, messageText.text);
        var trueIndex = index < 0 || index + 1 >= currentMessage.paragraphs.Length ? 0 : index + 1;

        messageText.text = currentMessage.paragraphs[trueIndex];
        characterImage.sprite = faces[(int)currentMessage.face[trueIndex]];
    }
}
