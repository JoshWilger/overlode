using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageUI : MonoBehaviour
{
    [SerializeField] private UIController uiController;
    [SerializeField] private GolemMovement golemScript;
    [SerializeField] private MessageClass[] messages;
    [SerializeField] private Sprite[] faces;
    [SerializeField] private Button nextButton;
    [SerializeField] private Image characterImage;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private GameObject focus;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private GameObject boss;
    [SerializeField] private AudioSource musicAud;
    [SerializeField] private AudioClip[] music;

    public int currentMessageIndex;

    // Start is called before the first frame update
    private void Awake()
    {
        currentMessageIndex = 0;
    }

    private void OnEnable()
    {
        uiController.pauseToggle.enabled = false;
        uiController.Paused(true);
        focus.SetActive(true);
        NextText();
        nextButton.onClick.AddListener(NextText);
        characterName.text = messages[currentMessageIndex].characterName;
        nextButton.interactable = messages[currentMessageIndex].paragraphs.Length > 1;
        if (currentMessageIndex + 4 < messages.Length)
        {
            musicAud.clip = music[currentMessageIndex];
        }
        else if (currentMessageIndex + 4 == messages.Length)
        {
            return;
        }
        else
        {
            musicAud.clip = music[currentMessageIndex - 1];
        }
        musicAud.time = 0;
        musicAud.Play();
        Debug.Log(musicAud.clip.name);
    }

    private void OnDisable()
    {
        nextButton.onClick.RemoveAllListeners();
        moneyText.text = "$" + (long.Parse(moneyText.text.Substring(1)) + messages[currentMessageIndex].reward);
        if (currentMessageIndex + 1 != messages.Length)
        {
            currentMessageIndex++;
        }
        boss.SetActive(currentMessageIndex + 1 >= messages.Length);
        if (currentMessageIndex + 1 == messages.Length)
        {
            golemScript.EndSummoningSequence();
        }
        uiController.Paused(false);
        uiController.pauseToggle.enabled = true;
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
