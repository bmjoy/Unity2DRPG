﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//戦闘用プレイヤーメンバー
public class PlayerParty : MonoBehaviour
{
    public GameObject playerEntity;
    public List<PlayerCharacter> partyMember;

    protected static PlayerParty instance;

    public static PlayerParty Instance
    {
        get
        {
            if (instance != null)
                return instance;

            instance = FindObjectOfType<PlayerParty>();

            if (instance != null)
                return instance;

            CreateInstance();

            return instance;
        }
    }

    private static void CreateInstance()
    {
        instance = new GameObject("PlayerParty").AddComponent<PlayerParty>();
        instance.playerEntity = Resources.Load<GameObject>("PlayerEntity");
    }

    //シングルトンの必要はなくなった
    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    public List<PlayerCharacter> AliveMember()
    {
        return partyMember.Where(x => !x.IsDead()).ToList();
    }

    public PlayerCharacter GetMember(int id)
    {
        return partyMember.FirstOrDefault(x => x.playerData.CharacterID == id);
    }

    public void SetUp()
    {
        var member = GameController.GetParty().GetMember();

        foreach (var item in member)
        {
            PlayerCharacter playerCharacter = Create(item);
            Join(playerCharacter);
        }
    }

    public PlayerCharacter Create(CharacterData playerData)
    {
        GameObject player = Instantiate(playerEntity);
        player.name = playerData.playerData.CharacterName;

        PlayerCharacter playerCharacter = player.GetComponent<PlayerCharacter>();

        playerCharacter.CharacterName = playerData.playerData.CharacterName;
        playerCharacter.playerData = playerData.playerData;
        playerCharacter.basicStatus = playerData.status.Copy();
        //
        playerCharacter.characterData = playerData;
        playerCharacter.battleStaus = new BattleStaus(playerData.status.Copy());
        Debug.Log(playerData.equip);
        playerCharacter.battleStaus.equip = playerData.equip;
        Debug.Log(playerCharacter.battleStaus.equip);
        //
        playerCharacter.SetUp();
        return playerCharacter;
    }

    public PlayerCharacter Create(PlayerData playerData)
    {
        GameObject player = Instantiate(playerEntity);
        player.name = playerData.CharacterName;

        PlayerCharacter playerCharacter = player.GetComponent<PlayerCharacter>();

        playerCharacter.CharacterName = playerData.CharacterName;
        playerCharacter.playerData = playerData;
        playerCharacter.basicStatus = playerData.Status.Copy();
        playerCharacter.SetUp();
        return playerCharacter;
    }

    //新規加入
    public void Join(PlayerData playerData)
    {
        PlayerCharacter playerCharacter = Create(playerData);
        Join(playerCharacter);
    }

    public void Join(int id)
    {
        PlayerData playerData = GameController.Instance.characterMaster.characterData.FirstOrDefault(x => x.CharacterID == id);
        Join(playerData);
    }

    //戦闘中仲間の追加
    public void Join(PlayerCharacter playerCharacter)
    {
        playerCharacter.transform.parent = gameObject.transform;
        partyMember.Add(playerCharacter);
        Debug.Log(playerCharacter.CharacterName + "がパーティーに加入した");
    }

    public void Load()
    {
        if (!GameController.GetSaveSystem().ExistsSaveData())
        {
            Debug.LogWarning("セーブデータが存在しません");
            return;
        }

        SaveData saveData = GameController.Instance.saveData;
        var characterDatas = saveData.partyData.characterDatas;

        foreach (var data in characterDatas)
        {
            PlayerCharacter playerCharacter = Create(data.characterData);
            playerCharacter.basicStatus = data.characterStatus.Copy();
            Join(playerCharacter);
        }
    }

    public void FullRecovery()
    {
        partyMember.ForEach(x => x.Recover(9999));
    }
}
