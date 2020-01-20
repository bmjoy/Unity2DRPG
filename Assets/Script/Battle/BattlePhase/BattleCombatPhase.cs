﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCombatPhase : MonoBehaviour
{
    public BattleCommandManager commandManager;
    public CommandExecutor commandExecution;

    public IEnumerator Combat()
    {
        //敵のコマンドを登録
        commandManager.RegisterEnemyCommand();
        var commands = commandManager.battleCommands;

        foreach (var command in commands)
        {
            BattleCharacter owner = command.owner;
            if (owner.IsDead()) continue;
            yield return StartCoroutine(commandExecution.Execution(command));
        }
        commandManager.Clea();
        yield break;
    }
}
