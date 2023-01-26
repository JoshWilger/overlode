using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Message", menuName = "Message Class")]
public class MessageClass : ScriptableObject
{
    public Face[] face;
    public string[] paragraphs;

    public enum Face
    {
        chicken,
        chickenTalk,
        dwarf,
        dwarfTalk,
        miner,
        minerTalk,
        lady,
        ladyTalk
    }
}
