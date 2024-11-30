﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxHealth;
    [SerializeField] private BaseBullet bullet;

    private float health;
    private List<Skill> skillList = new List<Skill>();
    private bool isShooting = false;
    private bool isSideBullets = true;
    private float coolDownTime = 1f;

    private int numberOfShot = 1; // Số lượng đạn được bắn ra khi click

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(x: horizontal, y: vertical, z: 0) * Time.deltaTime * moveSpeed;
        transform.position += move;

        if (Input.GetKeyDown(KeyCode.Mouse0) && !isShooting)
        {
            StartCoroutine(HandleShooting());
        }
    }

    private IEnumerator HandleShooting()
    {
        isShooting = true;

        for (int i = 0; i < numberOfShot; i++)
        {
            Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            Shoot(direction);

            // Tránh các viên đạn bị chồng lên nhau
            yield return new WaitForSeconds(.1f);
        }

        // Bắn thêm đạn sang 2 bên nếu có skill SideBullets
        if (isSideBullets)
        {
            Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;

            // Tính hai hướng vuông góc sang hai bên
            Vector2 leftDirection = new Vector2(-direction.y, direction.x);
            Vector2 rightDirection = new Vector2(direction.y, -direction.x);

            Shoot(leftDirection);
            Shoot(rightDirection);
        } 

        // Chờ thời gian cool down
        yield return new WaitForSeconds(coolDownTime);

        isShooting = false;
    }    

    private void Shoot(Vector2 direction)
    {
        BaseBullet returnBullet = BulletManager.Instance.Shoot(bullet);
        Debug.Log(returnBullet);
        returnBullet.BulletInit(transform.position, direction);
    }    

    public void AddSkill(Skill skill)
    {
        if (!skillList.Contains(skill))
        {
            skillList.Add(skill);
            skill.ApplySkill(this.gameObject);
        }
    }

    public void IncreaseMaxHealth(float amount)
    {
        maxHealth += amount;
    }    

    public void Heal(float amount)
    {
        health += amount;
    }    

    public void AddShot(int number)
    {
        numberOfShot += number;
    }      

    public void ActiveSideBullet()
    {
        isSideBullets = true;
    }    
}