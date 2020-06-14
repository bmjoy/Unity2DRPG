﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCommand : BattleCommand
{
    public SpellData spellData;

    public SpellCommand(SpellData spellData)
    {
        this.spellData = spellData;
    }

    public SpellCommand(SpellData spellData, BattleCharacter owner, List<BattleCharacter> target) : base(owner, target)
    {
        this.spellData = spellData;
    }

    public override IEnumerator Execution()
    {
        int spellMp = spellData.mp;
        string spellMessage = $"{owner.CharacterName}は　{spellData.skillName}をとなえた"; ;
        _BattleLogic.Instance.Message(spellMessage);
        Debug.Log("呪文");

        //yield return StartCoroutine(message.ShowAuto(spellMessage));

        if (owner.status.mp <= spellMp)
        {
            //yield return StartCoroutine(message.ShowAuto("しかし ＭＰが たりない！"));
            _BattleLogic.Instance.Message("しかし ＭＰが たりない！");
            yield break;
        }
        //封印
        //しかし呪文は封じられている
        if(owner.battleStaus.isSpellLimit)
        {
            _BattleLogic.Instance.Message("しかし 呪文は 封印されいる");
            yield break;
        }

        owner.GainMp(spellMp);

        //効果を実行
        //yield return StartCoroutine(effectExecutor.Execution(spellCommand));
        yield return CommandEffectExecutor.Instance.Execution(this);
        yield break;
    }

    public override List<CommandEffect> GetEffect()
    {
        return spellData.effects;
    }

    public override Command GetCommand()
    {
        return spellData;
    }

    public override TargetType GetTargetType()
    {
        TargetUnit targetUnit = spellData.targetUnit;
        TargetRange targetRange = spellData.targetRange;
        return new TargetType(targetUnit, targetRange);
    }
}
