using Models;
using Services;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterSelect : MonoBehaviour {

    public GameObject playerFoundPanel;//选择角色界面
    public GameObject characterSelectPanel;//创建角色界面

    CharacterClass charClass;

    public Text descs;//职业描述

    public Text[] names;//职业名字

    public Text characterName;//角色昵称

    public UICharacterView characterView;

    void Start () {
        UserService.Instance.OnCharacterCreate = OnCharacterCreate;

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void InitCharacterSelect(bool init)
    {
        playerFoundPanel.SetActive(true);
        characterSelectPanel.SetActive(false);
        if (init)
        {

        }
    }

    public void OnCharacterCreate()
    {
        if (string.IsNullOrEmpty(characterName.text))
        {
            MessageBox.Show("请输入昵称！");
            return;
        }
        UserService.Instance.SendCharacterCreate(this.characterName.text, charClass);
    }

    void OnCharacterCreate(Result result, string message)
    {
        if (result == Result.Success)
        {
            InitCharacterSelect(true);
        }
        else
        {
            MessageBox.Show(message, "错误 {0}", MessageBoxType.Error);
        }
    }

    public void OnSelectClass(int charClass)
    {
        this.charClass = (CharacterClass)charClass;

        characterView.CurrectCharacter = charClass - 1;

        for (int i = 0; i < 3; i++)
        {
            names[i].text = DataManager.Instance.Characters[i + 1].Name;//将职业名字依次赋值给UI
        }

        descs.text = DataManager.Instance.Characters[charClass].Description;//将职业描述赋值到UI

    }
}
