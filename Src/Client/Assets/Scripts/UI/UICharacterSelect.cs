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

    public List<GameObject> uiChars = new List<GameObject>();

    public Transform uiCharactersList;

    public GameObject uiCharInfo;

    public UICharacterView characterView;

    private int selectCharacterIdx = -1;

    void Start () {
        InitCharacterSelect(true);
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
            foreach (var old in uiChars)
            {
                Destroy(old);
            }
            uiChars.Clear();

            for (int i = 0; i < User.Instance.Info.Player.Characters.Count; i++)
            {
                GameObject go = Instantiate(uiCharInfo, uiCharactersList);
                UICharInfo charInfo = go.GetComponent<UICharInfo>();
                charInfo.info = User.Instance.Info.Player.Characters[i];

                Button button = go.GetComponent<Button>();
                int idx = i;
                button.onClick.AddListener(() =>
                {
                    OnSelectCharacter(idx);
                });

                uiChars.Add(go);
                go.SetActive(true);
            }
        }
    }

    public void OnSelectCharacter(int idx)
    {
        this.selectCharacterIdx = idx;
        var cha = User.Instance.Info.Player.Characters[idx];
        Debug.LogFormat("Select Char:[{0}]{1}[{2}]", cha.Id, cha.Name, cha.Class);
        User.Instance.CurrentCharacter = cha;
        characterView.CurrectCharacter = idx;
        
        for (int i = 0; i < User.Instance.Info.Player.Characters.Count; i++)
        {
            UICharInfo ci = this.uiChars[i].GetComponent<UICharInfo>();
            ci.Selected = idx == i;
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
