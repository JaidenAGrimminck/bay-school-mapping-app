using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;
    public List<TMPro.TMP_InputField> blocks;
    private string[] blockNames = new string[] {
        "A",
        "B",
        "C",
        "D",
        "E",
        "F",
    };

    void Start() {
        Instance = this;

        Load();
    }

    public void Load() {
        if (PlayerPrefs.HasKey("blocks")) {
            Debug.Log("found blocks, loading....");

            string[] blockValues = PlayerPrefs.GetString("blocks").Split(',');

            for (int i = 0; i < blockValues.Length; i++) {
                if (blockValues[i] != blockNames[i] + " Block" && blockValues[i] != "") {
                    blocks[i].text = blockValues[i];
                }
            }
        }
    }

    public string getBlock(int i) {
        return blocks[i].text == "" ? blockNames[i] + " Block" : blocks[i].text;
    }

    public int getBlockIndex(string block) {
        for (int i = 0; i < blockNames.Length; i++) {
            if (blockNames[i] == block) return i;
        }
        return -1;
    }

    public void updateAndSave() {
        string[] blocks = new string[blockNames.Length];

        for (int i = 0; i < blocks.Length; i++) {
            blocks[i] = this.blocks[i].text.Length > 1 ? this.blocks[i].text : this.blockNames[i] + " Block";
        }
        
        PlayerPrefs.SetString("blocks", string.Join(",", blocks));
        PlayerPrefs.Save();

        Debug.Log("saved preferences");
    }
}
