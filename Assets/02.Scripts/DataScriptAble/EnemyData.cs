using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EWeakType
{
    None,
    Fire,
    Cold,
    Physic,
    Electric,
}

[CreateAssetMenu(fileName = "GameData", menuName = "Data/EnemyData", order = 1)]
public class EnemyData : ScriptableObject
{
    public EObjectName objectName;
    public string fileName; // 파일 이름
    public string enemyFullName; // 이름
    public string description; // 설명
    public ERatingType ratingType;
    public EWeakType weakType; // 취약 속성
    public int hp; // 체력
    public int mp; // 마나
    public int atk; // 공격력
    public int def; // 방어력
    public float checkTime; // 몇초마다 공격 확인할 지
    public float atkSpd; // 공격 속도
    public float atkRange; // 공격 범위
    public float sightRange; // 공격 체크 범위
    [Range(0,100)]
    public float atkRate; // 공격 확률
    public float atkTime; // 공격 시간
    [Range(0, 5)]
    public float movSpd; // 이동 속도
    [Range(0, 100)]
    public float mineralGetRate; // 자원 획득 확률
    public int minMineral; // 최소 얻는 자원
    public int maxMineral; // 최대 얻는 자원
}
