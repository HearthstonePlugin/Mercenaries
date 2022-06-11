using System;
using System.Collections.Generic;

public class Strategy
{
    private List<string> AttackAbility = new List<string>();    // 不考虑抉择
    private List<string> AbilityTargetType = new List<string>();
    private List<bool> AttackTaunt = new List<bool>();
    private List<string> AttackStrategy = new List<string>();
    private List<string> FirstAttack = new List<string>();
    private List<string> heroNames = new List<string>();
    private void InitTeamConifg()
    {
        string teamConfigPath = @"BepInEx\config\MercenaryTeam.cfg";

        if (!System.IO.File.Exists(@teamConfigPath))
        {
            string TeamConifg = "# 佣兵名称-技能顺序-技能指向（默认=>0敌方，1自己，2无指向（尝试指向敌方0）,与设定技能顺序对应）-策略（暴击/攻击最大生命值/攻击最小生命值，默认101）-强制嘲讽优先（缺省0=>false）\n";
            TeamConifg += "瓦尔登·晨拥-213-222\n";    // 技能index需要-1
            TeamConifg += "冰雪之王洛克霍拉-123-222\n";
            TeamConifg += "吉安娜·普罗德摩尔-213-000-101\n";
            TeamConifg += "凯恩·血蹄-321-220\n";
            TeamConifg += "迪亚波罗-231-200-001\n";
            TeamConifg += "希奈丝特拉-231-110\n";
            TeamConifg += "# 优先目标，如果有则覆盖默认策略，当技能指向为2时，忽略执行。以>开头填写，无空格\n";
            TeamConifg += ">玛法里奥·怒风\n";
            TeamConifg += ">冰雪之王洛克霍拉\n";
            TeamConifg += ">拉格纳罗斯\n";
            TeamConifg += ">萨尔\n";
            TeamConifg += ">巴琳达·斯通赫尔斯\n";
            TeamConifg += ">瓦莉拉·萨古纳尔\n";
            TeamConifg += ">迪亚波罗\n";
            TeamConifg += ">瓦尔登·晨拥\n";
            TeamConifg += ">吉安娜·普罗德摩尔\n";
            TeamConifg += ">安度因·乌瑞恩\n";
            TeamConifg += ">沃金\n";
            TeamConifg += ">迦顿男爵\n";
            System.IO.File.WriteAllText(@teamConfigPath, TeamConifg);
            System.Diagnostics.Process.Start("explorer.exe", @"BepInEx\config\");
        }


        foreach (string line in System.IO.File.ReadLines(@teamConfigPath))
        {
            if (line[0] == '#') continue; // 跳过注释
            if (line[0] == '>')
            {
                FirstAttack.Add(line.Substring(1));
                continue;
            }
            string[] lineSplit = line.Split('-'); // 分割配置
            if (lineSplit.Length > 0)
            {
                heroNames.Add(lineSplit[0]);
                AttackAbility.Add(lineSplit[1]);
                if (lineSplit.Length == 3) AbilityTargetType.Add(lineSplit[2]);
                else AbilityTargetType.Add("000");
                if (lineSplit.Length == 4) AttackStrategy.Add(lineSplit[3]);
                else AttackStrategy.Add("101");
                if (lineSplit.Length == 5) AttackTaunt.Add(lineSplit[4] == "1");
                else AttackTaunt.Add(false);
            }
        }
    }


    public void Nomination()
    {
        this.InitTeamConifg();
        //heroNames.ForEach(i => Console.Write("{0}\t", i));
        //AttackAbility.ForEach(i => Console.Write("{0}\t", i));
        //AbilityTargetType.ForEach(i => Console.Write("{0}\t", i));
        //AttackStrategy.ForEach(i => Console.Write("{0}\t", i));
        //AttackTaunt.ForEach(i => Console.Write("{0}\t", i));
        MercenariesHelper.MercenariesHelper.BattleQueue.Clear();
        ZoneHand zoneHand = ZoneMgr.Get().FindZoneOfType<ZoneHand>(global::Player.Side.FRIENDLY);
        if (zoneHand != null)
        {

            foreach (string hero in heroNames)
            {
                foreach (Card card in zoneHand.GetCards())
                {
                    if (hero == card.GetEntity().GetName())
                    {
                        MercenariesHelper.MercenariesHelper.EntranceQueue.Enqueue(card.GetEntity());
                        break;
                    }
                    if (MercenariesHelper.MercenariesHelper.EntranceQueue.Count >= 3)
                    {
                        return;
                    }
                }
            }
        }
    }

    public void Combat()
    {
        this.InitTeamConifg();
        MercenariesHelper.MercenariesHelper.BattleQueue.Clear();
        ZonePlay friendlyZone = ZoneMgr.Get().FindZoneOfType<ZonePlay>(global::Player.Side.FRIENDLY);
        MercenariesHelper.MercenariesHelper.Battles battles = new MercenariesHelper.MercenariesHelper.Battles();

        for (int i = 0; i < heroNames.Count; i++)
        {
            foreach (Card card in friendlyZone.GetCards())
            {
                string firendlyCardName = card.GetEntity().GetName();
                
                if (firendlyCardName == heroNames[i])
                {
                    List<int> lettuceAbilityEntityIDs = card.GetEntity().GetLettuceAbilityEntityIDs();
                    int AttackAbility1 = int.Parse(AttackAbility[i]) / 100 - 1;
                    int AttackAbility2 = int.Parse(AttackAbility[i]) / 10 % 10 - 1;
                    int AttackAbility3 = int.Parse(AttackAbility[i]) % 10 - 1;
                    int CurrentAbilityCount;
                    AttackAbility1 = AttackAbility1 < 3 ? AttackAbility1 : 0;
                    AttackAbility2 = AttackAbility2 < 3 ? AttackAbility2 : 0;
                    AttackAbility3 = AttackAbility3 < 3 ? AttackAbility3 : 0;
                    AttackAbility1 = lettuceAbilityEntityIDs[AttackAbility1];
                    AttackAbility2 = lettuceAbilityEntityIDs[AttackAbility2];
                    AttackAbility3 = lettuceAbilityEntityIDs[AttackAbility3];
                    //Console.Write(heroNames[i]);
                    //Console.Write("\t挑选技能");

                    Entity CurrentAbility = GameState.Get().GetEntity(AttackAbility1);
                    CurrentAbilityCount = 1;
                    if (CurrentAbility.GetTag(GAME_TAG.LETTUCE_CURRENT_COOLDOWN) != 0)
                    {
                        //Console.WriteLine("第一优先技能存在CD，尝试更换!");
                        CurrentAbility = GameState.Get().GetEntity(AttackAbility2);
                        CurrentAbilityCount = 2;
                    }
                    if (CurrentAbility.GetTag(GAME_TAG.LETTUCE_CURRENT_COOLDOWN) != 0)
                    {
                        //Console.WriteLine("第二优先技能存在CD，尝试更换！");
                        CurrentAbility = GameState.Get().GetEntity(AttackAbility3);
                        CurrentAbilityCount = 3;
                    }
                    battles.Ability = CurrentAbility;

                    //GameState.Get().HasResponse(CurrentAbility, new bool?(false));
                    //Console.WriteLine("技能选择："+ CurrentAbility.GetName());
                    int CurrentAbilityTargetType = 0;
                    switch (CurrentAbilityCount)
                    {
                        case 1:
                            CurrentAbilityTargetType = int.Parse(AbilityTargetType[i]) / 100;
                            break;
                        case 2:
                            CurrentAbilityTargetType = int.Parse(AbilityTargetType[i]) / 10 % 10;
                            break;
                        case 3:
                            CurrentAbilityTargetType = int.Parse(AbilityTargetType[i]) % 10;
                            break;
                    }

                    switch (CurrentAbilityTargetType)
                    {
                        case 0:
                            battles.target = this.HandleCard(i, card.GetEntity().GetMercenaryRole());
                            break;
                        case 1:
                            battles.target = card.GetEntity();
                            break;
                        case 2:
                            break;
                    }
                    battles.source = card.GetEntity();
                    MercenariesHelper.MercenariesHelper.BattleQueue.Enqueue(battles);
                    break; // 退出循环执行下一个技能设定
                }
            }

        }
    }

    private Entity HandleCard(int idx, TAG_ROLE myRole)
    {
        ZonePlay opposingZone = ZoneMgr.Get().FindZoneOfType<ZonePlay>(global::Player.Side.OPPOSING);
        List<Card> opposingCards = opposingZone.GetCards();
        Entity target;
        if (AttackTaunt[idx])    // FIXME：从描述中找攻击
        {
            foreach (Card card in opposingCards)
            {
                if (card.GetEntity().HasTaunt() && !card.GetEntity().IsStealthed())
                {
                    return card.GetEntity();
                }
            }
        }

        for (int i = 0; i < FirstAttack.Count; i++)    // 优先攻击
        {
            foreach (Card card in opposingCards)
            {
                string opposingCardName = card.GetEntity().GetName();
                if (opposingCardName == FirstAttack[i])
                {
                    //Console.WriteLine("尝试优先攻击目标："+ opposingCardName);
                    if (!card.GetEntity().IsStealthed())    // 非潜行
                        return card.GetEntity();
                }
            }
        }

        bool crit = (int.Parse(AttackStrategy[idx]) / 100) == 1;
        bool healthMax = (int.Parse(AttackStrategy[idx]) / 10 % 10) == 1;
        bool healthMin = (int.Parse(AttackStrategy[idx]) % 10) == 1;
        int health = -1;
        TAG_ROLE enemyRole = TAG_ROLE.INVALID;
        if (crit)
        {
            target = null;
            if (myRole == TAG_ROLE.CASTER) enemyRole = TAG_ROLE.TANK;
            if (myRole == TAG_ROLE.TANK) enemyRole = TAG_ROLE.FIGHTER;
            if (myRole == TAG_ROLE.FIGHTER) enemyRole = TAG_ROLE.CASTER;

            foreach (Card card in opposingCards)
            {
                if ((card.GetEntity().GetMercenaryRole() == enemyRole) && (enemyRole != TAG_ROLE.INVALID) && (!card.GetEntity().IsStealthed()))
                {
                    if (health == -1)
                    {
                        target = card.GetEntity();
                        health = target.GetCurrentHealth();
                    }
                    if (healthMax && (health != -1))
                    {
                        if (card.GetEntity().GetCurrentHealth() > health)
                        {
                            target = card.GetEntity();
                            health = target.GetCurrentHealth();

                        }
                    }
                    if (healthMin && (health != -1))
                    {
                        if (card.GetEntity().GetCurrentHealth() < health)
                        {
                            target = card.GetEntity();
                            health = target.GetCurrentHealth();
                        }
                    }
                }
            }
            if (target != null)
            {
                return target;
            }
        }
        target = null;
        foreach (Card card in opposingCards)
        {
            if (!card.GetEntity().IsStealthed())
            {
                if (health == -1)
                {
                    target = card.GetEntity();
                    health = target.GetCurrentHealth();
                }
                if (healthMax && (health != -1))
                {
                    if (card.GetEntity().GetCurrentHealth() > health)
                    {
                        target = card.GetEntity();
                        health = target.GetCurrentHealth();

                    }
                }
                if (healthMin && (health != -1))
                {
                    if (card.GetEntity().GetCurrentHealth() < health)
                    {
                        target = card.GetEntity();
                        health = target.GetCurrentHealth();
                    }
                }
            }
        }
        if (target != null)
        {
            return target;
        }
        return opposingCards[0].GetEntity();
    }
}
