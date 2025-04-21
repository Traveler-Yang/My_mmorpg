using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILogin : MonoBehaviour {

	public InputField username;//账号
	public InputField password;//密码
	public Button loginButton;//登录按钮
	void Start () {
		UserService.Instance.OnLogin = OnLogin;
	}
	
	
	void Update () {
		
	}
    public void OnLogin(SkillBridge.Message.Result result, string msg)
	{
        MessageBox.Show(string.Format("结果:{0} msg:{1}", result, msg));
    }

	public void OnLoginClickButton()
	{
		if (string.IsNullOrEmpty(username.text))
		{
            MessageBox.Show("请输入账号");
            return;
        }
		if (string.IsNullOrEmpty(password.text))
		{
            MessageBox.Show("请输入密码");
            return;
        }
		UserService.Instance.SendLogin(username.text, password.text);
	}

}
