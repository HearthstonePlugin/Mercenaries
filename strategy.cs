using System.Collections.Generic;
// Token: 0x02000002 RID: 2
public class Strategy
{
	// Token: 0x06000001 RID: 1 RVA: 0x00002064 File Offset: 0x00000264
	public void Nomination()
	{
		ZoneHand zoneHand = ZoneMgr.Get().FindZoneOfType<ZoneHand>(global::Player.Side.FRIENDLY);
		if (zoneHand != null)
		{
			foreach (string a in new List<string>(new string[]
			{
				"瓦尔登·晨拥",
				"冰雪之王洛克霍拉",
				"吉安娜·普罗德摩尔",
				"凯恩·血蹄",
				"迪亚波罗",
				"巴琳达·斯通赫尔斯",
				"瓦莉拉·萨古纳尔",
				"希奈丝特拉"
			}))
			{
				foreach (Card card in zoneHand.GetCards())
				{
					if (a == card.GetEntity().GetName())
					{
						MercenariesHelper.MercenariesHelper.EntranceQueue.Enqueue(card.GetEntity());
						break;
					}
					if (MercenariesHelper.MercenariesHelper.EntranceQueue.Count >= 3)
					{
						break;
					}
				}
			}
		}
	}

	// Token: 0x06000002 RID: 2 RVA: 0x00002180 File Offset: 0x00000380
	public void Combat()
	{
		ZonePlay zone = ZoneMgr.Get().FindZoneOfType<ZonePlay>(global::Player.Side.FRIENDLY);
		ZonePlay zonePlay = ZoneMgr.Get().FindZoneOfType<ZonePlay>(global::Player.Side.OPPOSING);
		string text = "";
		bool flag = false;
		int num = 0;
		MercenariesHelper.MercenariesHelper.Battles battles = new MercenariesHelper.MercenariesHelper.Battles();
		foreach (Card card in zone.GetCards())
		{
			string text2 = card.GetEntity().GetName();
			text2 = text2.Substring(0, text2.Length);
			this.GetLettuceAbilityEntitys(card.GetEntity());
			List<Entity> lettuceAbilityEntitys = this.GetLettuceAbilityEntitys(card.GetEntity());
			MercenariesHelper.MercenariesHelper.Battles battles2 = new MercenariesHelper.MercenariesHelper.Battles();
			if (text2 == "巴琳达·斯通赫尔斯")
			{
				battles.Ability = this.selAbility(lettuceAbilityEntitys, "烈焰之刺");
			}
			if (text2 == "瓦莉拉·萨古纳")
			{
				battles.Ability = this.selAbility(lettuceAbilityEntitys, "影袭");
			}
			if (text2 == "瓦尔登·晨拥")
			{
				battles.Ability = this.selAbility(lettuceAbilityEntitys, "急速冰冻");
				if (battles.Ability.GetName().IndexOf("急速冰冻") == -1)
				{
					battles.Ability = this.selAbility(lettuceAbilityEntitys, "冰风暴");
				}
			}
			if (text2 == "吉安娜·普罗德摩尔")
			{
				battles.Ability = this.selAbility(lettuceAbilityEntitys, "浮冰术");
				battles.target = this.HandleCards(zonePlay.GetCards(), true, false, false, TAG_ROLE.TANK);
			}
			if (text2 == "冰雪之王洛克霍拉")
			{
				battles2.Ability = this.selAbility(lettuceAbilityEntitys, "冰雹");
			}
			if (text2 == "凯恩·血蹄")
			{
				battles.Ability = this.selAbility(lettuceAbilityEntitys, "大地践踏");
				if (battles.Ability.GetName().IndexOf("大地践踏") == -1)
				{
					battles.Ability = this.selAbility(lettuceAbilityEntitys, "坚韧光环");
				}
			}
			if (text2 == "迪亚波罗")
			{
				battles.Ability = this.selAbility(lettuceAbilityEntitys, "火焰践踏");
				if (battles.Ability.GetName().IndexOf("火焰践踏") == -1)
				{
					battles.Ability = this.selAbility(lettuceAbilityEntitys, "末日");
				}
				if (battles.Ability.GetName().IndexOf("末日") > 0)
				{
					battles.target = this.HandleCards(zonePlay.GetCards(), false, true, false, 0);
				}
			}
			if (text2 == "希奈丝特拉")
			{
				card.GetEntity().GetName();
				Entity entity = card.GetEntity();
				battles.Ability = this.selAbility(lettuceAbilityEntitys, "暮光灭绝");
				if (battles.Ability.GetName().IndexOf("暮光灭绝") == -1)
				{
					battles.Ability = this.selAbility(lettuceAbilityEntitys, "法力壁垒");
					battles.target = entity;
				}
			}
			if (text2 == "次级水元素")
			{
				foreach (Card card2 in zonePlay.GetCards())
				{
					string name = card.GetEntity().GetName();
					if (name == "玛法里奥·怒风" || name == "冰雪之王洛克霍拉" || name == "拉格纳罗斯" || name == "萨尔" || name == "巴琳达·斯通赫尔斯" || name == "瓦莉拉·萨古纳尔" || name == "迪亚波罗" || name == "瓦尔登·晨拥" || name == "吉安娜·普罗德摩尔" || name == "安度因·乌瑞恩" || name == "沃金" || name == "迦顿男爵")
					{
						if (!flag)
						{
							text = name;
						}
						else
						{
							num++;
						}
					}
				}
				battles.Ability = this.selAbility(lettuceAbilityEntitys, "攻击");
				if (text != null && battles.Ability.GetName().IndexOf("攻击") > 0)
				{
					battles.Ability = this.HandleSpeCard(zonePlay.GetCards(), text, false);
				}
				flag = false;
			}
			if (battles2.Ability != null)
			{
				MercenariesHelper.MercenariesHelper.BattleQueue.Enqueue(battles2);
			}
			MercenariesHelper.MercenariesHelper.BattleQueue.Enqueue(battles);
		}
		if (battles.Ability != null)
		{
			MercenariesHelper.MercenariesHelper.BattleQueue.Enqueue(battles);
			for (int i = 0; i < MercenariesHelper.MercenariesHelper.BattleQueue.Count - 1; i++)
			{
				battles = MercenariesHelper.MercenariesHelper.BattleQueue.Dequeue();
				MercenariesHelper.MercenariesHelper.BattleQueue.Enqueue(battles);
			}
		}
	}

	// Token: 0x06000003 RID: 3 RVA: 0x00002650 File Offset: 0x00000850
	private Entity HandleSpeCard(List<Card> cards, string TargetCard, bool ifowner)
	{
		ZoneMgr.Get().FindZoneOfType<ZonePlay>(global::Player.Side.FRIENDLY);
		ZoneMgr.Get().FindZoneOfType<ZonePlay>(global::Player.Side.OPPOSING);
		Entity entity = cards[0].GetEntity();
		foreach (Card card in cards)
		{
			string name = card.GetEntity().GetName();
			if (card.GetZone().m_Side == global::Player.Side.FRIENDLY && name == TargetCard)
			{
				return card.GetEntity();
			}
		}
		return entity;
	}

	// Token: 0x06000004 RID: 4 RVA: 0x000026F0 File Offset: 0x000008F0
	private Entity HandleCards(List<Card> cards, bool healthMin = false, bool healthMax = false, bool isTaunt = false, TAG_ROLE tAG_ROLE = 0)
	{
		foreach (Card card in cards)
		{
			if (card.GetEntity().GetMercenaryRole() == tAG_ROLE && !card.GetEntity().IsStealthed())
			{
				return card.GetEntity();
			}
		}
		if (isTaunt)
		{
			foreach (Card card2 in cards)
			{
				if (card2.GetEntity().HasTaunt() && !card2.GetEntity().IsStealthed())
				{
					return card2.GetEntity();
				}
			}
		}
		Entity entity = cards[0].GetEntity();
		if (healthMin)
		{
			foreach (Card card3 in cards)
			{
				if (card3.GetEntity().GetCurrentHealth() < entity.GetCurrentHealth() && !card3.GetEntity().IsStealthed())
				{
					entity = card3.GetEntity();
				}
			}
		}
		if (healthMax)
		{
			foreach (Card card4 in cards)
			{
				if (card4.GetEntity().GetCurrentHealth() > entity.GetCurrentHealth() && !card4.GetEntity().IsStealthed())
				{
					entity = card4.GetEntity();
				}
			}
		}
		return entity;
	}

	// Token: 0x06000005 RID: 5 RVA: 0x0000289C File Offset: 0x00000A9C
	private List<Entity> GetLettuceAbilityEntitys(Entity entity)
	{
		List<Entity> list = new List<Entity>();
		foreach (int num in entity.GetLettuceAbilityEntityIDs())
		{
			Entity entity2 = GameState.Get().GetEntity(num);
			if (entity2 != null && !entity2.IsLettuceEquipment())
			{
				list.Add(entity2);
			}
		}
		return list;
	}

	// Token: 0x06000006 RID: 6 RVA: 0x00002910 File Offset: 0x00000B10
	private Entity selAbility(List<Entity> AbilityEntitys, string AbilityName)
	{
		foreach (Entity entity in AbilityEntitys)
		{
			string text = entity.GetName();
			text = text.Substring(0, text.Length - 1);
			if (AbilityName == text && GameState.Get().HasResponse(entity, new bool?(false)))
			{
				return entity;
			}
		}
		Entity result = new Entity();
		foreach (Entity entity2 in AbilityEntitys)
		{
			if (GameState.Get().HasResponse(entity2, new bool?(false)))
			{
				result = entity2;
				break;
			}
		}
		return result;
	}
}
