﻿using Entities;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputContorller : MonoBehaviour {

	public Rigidbody rb;
	SkillBridge.Message.CharacterState state;

	public Character character;

	public float rotateSpeed = 2.0f;

	public float turnAngle = 10;

	public int speed;

	public EntityContorller entityContorller;

	public bool onAir = false;
	void Start () 
	{
		state = SkillBridge.Message.CharacterState.Idle;
		if (this.character == null)
		{
			DataManager.Instance.Load();
			NCharacterInfo cinfo = new NCharacterInfo();
			cinfo.Id = 1;
			cinfo.Name = "Test";
			cinfo.Tid = 1;
			cinfo.Entity = new NEntity();
			cinfo.Entity.Position = new NVector3();
			cinfo.Entity.Direction = new NVector3();
			cinfo.Entity.Direction.X = 0;
			cinfo.Entity.Direction.Y = 100;
			cinfo.Entity.Direction.Z = 0;
			this.character = new Character(cinfo);

			if (entityContorller != null) entityContorller.entity = this.character;
		}

    }

	void FixedUpdate()
	{
		if (character == null)
			return;
        #region 前后移动
        float vertical = Input.GetAxis("Vertical");
        if (vertical > 0.01)//向前移动
        {
            if (state != CharacterState.Move)
            {
                state = CharacterState.Move;
                this.character.MoveForward();
                this.SendEntityEvent(EntityEvent.MoveFwd);
            }
            this.rb.velocity = this.rb.velocity.y * Vector3.up + GameObjectTool.LogicToWorld(character.direction) * (character.speed + 9.81f) / 100f;
        }
        else if (vertical < -0.01)//向后移动
        {
            if (state != CharacterState.Move)
            {
                state = CharacterState.Move;
                this.character.MoveBack();
                this.SendEntityEvent(EntityEvent.MoveBack);
            }
            this.rb.velocity = this.rb.velocity.y * Vector3.up + GameObjectTool.LogicToWorld(character.direction) * (character.speed + 9.81f) / 100f;
        }
        else
        {
            if (state != CharacterState.Idle)
            {
                state = CharacterState.Idle;//状态机设置为Idle状态
                this.rb.velocity = Vector3.zero;//将矢量设置为0
                this.character.Stop();//通知角色停止
                this.SendEntityEvent(EntityEvent.Idle);
            }
        }
        #endregion

        if (Input.GetButtonDown("Jump"))
		{
            this.SendEntityEvent(EntityEvent.Jump);
        }

        #region 左右移动
        float horizontal = Input.GetAxis("Horizontal");
        if (horizontal < -0.1 || horizontal > 0.1)
        {
            this.transform.Rotate(0, horizontal * rotateSpeed, 0);
            Vector3 dir = GameObjectTool.LogicToWorld(character.direction);
            Quaternion rot = new Quaternion();
            rot.SetFromToRotation(dir, this.transform.forward);

            if (rot.eulerAngles.y > this.turnAngle && rot.eulerAngles.y < (360 - this.turnAngle))
            {
                character.SetDirection(GameObjectTool.WorldToLogic(this.transform.forward));
                rb.transform.forward = this.transform.forward;
                this.SendEntityEvent(EntityEvent.None);
            }
        }
        #endregion
    }

    Vector3 lastPos;
    float lastSync = 0;
    private void LateUpdate()
    {
        Vector3 offset = this.rb.transform.position = lastPos;
        this.speed = (int)(offset.magnitude * 100f / Time.deltaTime);

        this.lastPos = this.rb.transform.position;

        if ((GameObjectTool.WorldToLogic(this.rb.transform.position) - this.character.position).magnitude > 50)
        {
            this.character.SetPosition(GameObjectTool.WorldToLogic(this.rb.transform.position));
            this.SendEntityEvent(EntityEvent.None);
        }
        this.transform.position = this.rb.transform.position;
    }

	void SendEntityEvent(EntityEvent entityEvent)
	{
		if (entityContorller != null)
			entityContorller.OnEntityEvent(entityEvent);
	}
}
