﻿using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using static HsMod.PluginConfig;

namespace HsMod
{
    public class Utils
    {
        public enum CardState
        {
            [Description("默认（不做修改）")]
            Default,
            [Description("仅友方有效")]
            OnlyMy,
            [Description("全部生效")]
            All,
            [Description("禁用特效")]
            Disabled
        }

        public enum SkinType
        {
            [Description("卡背")]
            CARDBACK,
            [Description("卡牌")]
            CARD,
            [Description("硬币")]
            COIN,
            [Description("英雄皮肤")]
            HERO,
            [Description("酒馆鲍勃")]
            BOB,
            [Description("酒馆终结特效")]
            BATTLEGROUNDSFINISHER,
            [Description("酒馆战场")]
            BATTLEGROUNDSBOARD,
            [Description("酒馆英雄皮肤")]
            BATTLEGROUNDSHERO,
            [Description("英雄技能")]
            HEROPOWER,
        }
        public enum AlertPopupResponse
        {
            OK = AlertPopup.Response.OK,
            CONFIRM = AlertPopup.Response.CONFIRM,
            CANCEL = AlertPopup.Response.CANCEL,
            YES,
            DONOTHING
        }

        public enum ConfigTemplate
        {
            [Description("默认")]
            DoNothing,
            [Description("挂机")]
            AwayFromKeyboard,
            [Description("反挂机")]
            AntiAwayFromKeyboard
        }

        public class CardCount
        {
            public int legendary = 0;
            public int epic = 0;
            public int rare = 0;
            public int common = 0;
            public int total = 0;
            public int gLegendary = 0;
            public int gEpic = 0;
            public int gRare = 0;
            public int gCommon = 0;
            public int gTotal = 0;
            public int totalDust = 0;
        };

        public struct CollectionCard
        {
            public string Name;
            public int Count;
            public TAG_RARITY Rarity;
            public TAG_PREMIUM Premium;
        }

        public struct MercenarySkin
        {
            public string Name;
            public List<int> Id;
            public bool hasDiamond;
            public int Diamond;
            public int Default;
        }



        public static string CardsCount(TAG_RARITY Rarity, TAG_PREMIUM premium, int count, ref CardCount cardCount)
        {
            bool golden = (premium == TAG_PREMIUM.GOLDEN);
            switch (Rarity)
            {
                case TAG_RARITY.COMMON:
                    if (golden)
                    {
                        cardCount.gCommon += count;
                        cardCount.gTotal += count;
                        cardCount.total += count;
                        cardCount.totalDust += count * 50;
                        return $"<td>金色普通</td><td>{count}</td>";
                    }
                    else
                    {
                        cardCount.common += count;
                        cardCount.total += count;
                        cardCount.totalDust += count * 5;
                        return $"<td>普通</td><td>{count}</td>";
                    }
                case TAG_RARITY.RARE:
                    if (golden)
                    {
                        cardCount.gRare += count;
                        cardCount.gTotal += count;
                        cardCount.total += count;
                        cardCount.totalDust += count * 100;
                        return $"<td>金色稀有</td><td>{count}</td>";
                    }
                    else
                    {
                        cardCount.rare += count;
                        cardCount.total += count;
                        cardCount.totalDust += count * 20;
                        return $"<td>稀有</td><td>{count}</td>";
                    }
                case TAG_RARITY.EPIC:
                    if (golden)
                    {
                        cardCount.gEpic += count;
                        cardCount.gTotal += count;
                        cardCount.total += count;
                        cardCount.totalDust += count * 400;
                        return $"<td>金色史诗</td><td>{count}</td>";
                    }
                    else
                    {
                        cardCount.epic += count;
                        cardCount.total += count;
                        cardCount.totalDust += count * 100;
                        return $"<td>史诗</td><td>{count}</td>";
                    }
                case TAG_RARITY.LEGENDARY:
                    if (golden)
                    {
                        cardCount.gLegendary += count;
                        cardCount.gTotal += count;
                        cardCount.total += count;
                        cardCount.totalDust += count * 1600;
                        return $"<td>金色传说</td><td>{count}</td>";
                    }
                    else
                    {
                        cardCount.legendary += count;
                        cardCount.total += count;
                        cardCount.totalDust += count * 400;
                        return $"<td>传说</td><td>{count}</td>";
                    }
                default:
                    return "<td>未知</td>";
            }
        }

        public static string RankIdxToString(int rank)
        {
            string text;
            switch ((rank - 1) / 10)
            {
                case 0:
                    text = "青铜";
                    break;
                case 1:
                    text = "白银";
                    break;
                case 2:
                    text = "黄金";
                    break;
                case 3:
                    text = "铂金";
                    break;
                case 4:
                    text = "钻石";
                    break;
                case 5:
                    return "传说";
                default:
                    text = "未知";
                    break;
            }
            return text + (11 - (rank - (rank - 1) / 10 * 10)).ToString();
        }


        public struct CardMapping
        {
            public int RealDbID { get; set; }
            public int FakeDbID { get; set; }
            public string RealCardID { get; set; }
            public string FakeCardID { get; set; }
            public SkinType ThisSkinType { get; set; }
        };

        public static void MyLogger(LogLevel level, object message)
        {
            var myLogSource = BepInEx.Logging.Logger.CreateLogSource(PluginInfo.PLUGIN_GUID + ".MyLogger");
            myLogSource.Log(level, message);
            BepInEx.Logging.Logger.Sources.Remove(myLogSource);
        }
        public static void TryRefundCardDisenchantCallback()
        {
            Network.CardSaleResult cardSaleResult = Network.Get().GetCardSaleResult();
            if (cardSaleResult.Action != Network.CardSaleResult.SaleResult.CARD_WAS_SOLD)
            {
                MyLogger(LogLevel.Warning, "分解失败");
                UIStatus.Get().AddInfo("分解失败");
                //AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo
                //{
                //    m_headerText = GameStrings.Get("GLUE_COLLECTION_ERROR_HEADER"),
                //    m_text = GameStrings.Format("GLUE_COLLECTION_CARD_UNKNOWN_ERROR", new object[] { cardSaleResult.Action }),
                //    m_showAlertIcon = true,
                //    m_responseDisplay = AlertPopup.ResponseDisplay.OK
                //};
                //DialogManager.Get().ShowPopup(popupInfo);
            }
            else
            {
                MyLogger(LogLevel.Warning, "分解成功");
                CollectionManager.Get().OnCollectionChanged();
            }
        }

        //毁灭吧
        //public static void TryRuinCardDisenchant()
        //{
        //    int totalSell = 0;
        //    Network network = Network.Get();
        //    network.RegisterNetHandler(PegasusUtil.BoughtSoldCard.PacketID.ID, new Network.NetHandler(TryRefundCardDisenchantCallback), null);
        //    foreach (var record in CollectionManager.Get().GetOwnedCards())
        //    {
        //        if (record != null && record.IsCraftable && (record.OwnedCount > 0))
        //        {
        //            CraftingManager.Get().TryGetCardSellValue(record.CardId, record.PremiumType, out int value);
        //            network.SellCard(record.CardDbId, record.PremiumType, record.OwnedCount, value, record.OwnedCount);
        //            totalSell += record.OwnedCount * value;
        //        }
        //    }
        //    MyLogger(LogLevel.Warning, "尝试分解粉尘：" + totalSell);
        //    UIStatus.Get().AddInfo("尝试分解粉尘：" + totalSell);
        //}

        public static void TryReadNewCards()
        {
            if ((SceneMgr.Get().GetMode() == SceneMgr.Mode.COLLECTIONMANAGER) || (SceneMgr.Get().GetMode() == SceneMgr.Mode.PACKOPENING))
            {
                foreach (var record in CollectionManager.Get().GetOwnedCards())
                {
                    if (record != null && record.IsNewCard)
                    {
                        CollectionManager.Get().MarkAllInstancesAsSeen(record.CardId, record.PremiumType);
                    }
                }
            }
            if ((SceneMgr.Get()?.GetMode() == SceneMgr.Mode.LETTUCE_COLLECTION) || (SceneMgr.Get().GetMode() == SceneMgr.Mode.LETTUCE_PACK_OPENING)) // 执行后，需要重新进入佣兵收藏
            {
                List<PegasusLettuce.MercenaryAcknowledgeData> m_mercenaryAcknowledgements = new List<PegasusLettuce.MercenaryAcknowledgeData>();
                foreach (var merc in CollectionManager.Get().FindOrderedMercenaries(null, true, null, null, null).m_mercenaries)
                {
                    foreach (var ability in merc.m_abilityList)
                    {
                        if (!ability.IsAcknowledged(merc))
                        {
                            PegasusLettuce.MercenaryAcknowledgeData mercenaryAcknowledgeData = new PegasusLettuce.MercenaryAcknowledgeData
                            {
                                Type = PegasusLettuce.MercenaryAcknowledgeData.AcknowledgeType.ACKNOWLEDGE_MERC_ABILITY_ALL,
                                AssetId = ability.ID,
                                Acknowledged = true,
                                MercenaryId = merc.ID
                            };
                            m_mercenaryAcknowledgements.Append(mercenaryAcknowledgeData);
                            CollectionManager.Get().MarkMercenaryAsAcknowledgedinCollection(mercenaryAcknowledgeData);
                        }
                    }
                    foreach (var equipment in merc.m_equipmentList)
                    {
                        if (!equipment.IsAcknowledged(merc))
                        {
                            PegasusLettuce.MercenaryAcknowledgeData mercenaryAcknowledgeData = new PegasusLettuce.MercenaryAcknowledgeData
                            {
                                Type = PegasusLettuce.MercenaryAcknowledgeData.AcknowledgeType.ACKNOWLEDGE_MERC_EQUIPMENT_ALL,
                                AssetId = equipment.ID,
                                Acknowledged = true,
                                MercenaryId = merc.ID
                            };
                            m_mercenaryAcknowledgements.Append(mercenaryAcknowledgeData);
                            CollectionManager.Get().MarkMercenaryAsAcknowledgedinCollection(mercenaryAcknowledgeData);
                        }
                    }

                    foreach (var artVariation in merc.m_artVariations)
                    {
                        if (!artVariation.m_acknowledged)
                        {
                            PegasusLettuce.MercenaryAcknowledgeData mercenaryAcknowledgeData = new PegasusLettuce.MercenaryAcknowledgeData
                            {
                                Type = PegasusLettuce.MercenaryAcknowledgeData.AcknowledgeType.ACKNOWLEDGE_MERC_ART_VARIATION_ACQUIRED,
                                AssetId = artVariation.m_record.ID,
                                Premium = (uint)artVariation.m_premium,
                                Acknowledged = true,
                                MercenaryId = merc.ID
                            };
                            m_mercenaryAcknowledgements.Append(mercenaryAcknowledgeData);
                            CollectionManager.Get().MarkMercenaryAsAcknowledgedinCollection(mercenaryAcknowledgeData);
                        }
                    }
                }
                Network.Get().RegisterNetHandler(PegasusLettuce.MercenariesCollectionAcknowledgeResponse.PacketID.ID, new Network.NetHandler(OnMercCollectionAcknowledgeResponse), null);
                Network.Get().AcknowledgeMercenaryCollection(m_mercenaryAcknowledgements);
                m_mercenaryAcknowledgements.Clear();
            }

        }

        public static void OnMercCollectionAcknowledgeResponse()
        {
            Network.Get().RemoveNetHandler(PegasusLettuce.MercenariesCollectionAcknowledgeResponse.PacketID.ID, new Network.NetHandler(OnMercCollectionAcknowledgeResponse));
            if (!Network.Get().AcknowledgeMercenaryCollectionResponse().Success)
            {
                MyLogger(LogLevel.Error, "Error acknowledging collection");
            }
        }


        public static void TryRefundCardDisenchant()
        {
            int totalSell = 0;
            Network network = Network.Get();
            network.RegisterNetHandler(PegasusUtil.BoughtSoldCard.PacketID.ID, new Network.NetHandler(TryRefundCardDisenchantCallback), null);
            foreach (var record in CollectionManager.Get().GetOwnedCards())
            {
                if (record != null && record.IsCraftable && record.IsRefundable && (record.OwnedCount > 0))
                {
                    CraftingManager.Get().TryGetCardSellValue(record.CardId, record.PremiumType, out int value);
                    network.SellCard(record.CardDbId, record.PremiumType, record.OwnedCount, value, record.OwnedCount);
                    totalSell += record.OwnedCount * value;
                    //    _ = NetCache.Get().GetNetObject<NetCache.NetCacheCollection>();
                    //    CollectionManager.Get().OnCollectionChanged();
                    //    CollectionManager.Get().GetOwnedCount(record.CardId, record.PremiumType))
                }
            }
            //network.RemoveNetHandler(PegasusUtil.BoughtSoldCard.PacketID.ID, new Network.NetHandler(TryRefundCardDisenchantCallback));
            MyLogger(LogLevel.Warning, "尝试分解粉尘：" + totalSell);
            UIStatus.Get().AddInfo("尝试分解粉尘：" + totalSell);
        }

        public static List<int> CacheCoin = new List<int>();
        public static List<int> CacheCoinCard = new List<int>();
        public static List<int> CacheGameBoard = new List<int>();
        public static List<int> CacheBgsBoard = new List<int>();
        public static List<int> CacheCardBack = new List<int>();
        public static List<int> CacheBgsFinisher = new List<int>();
        public static Dictionary<int, Assets.CardHero.HeroType> CacheHeroes = new Dictionary<int, Assets.CardHero.HeroType>();
        public static string CacheFullName;
        public static List<MercenarySkin> CacheMercenarySkin = new List<MercenarySkin>();

        public static class CacheInfo
        {

            public static void UpdateCoin()  // 不能传入参数 List<T> List
            {
                CacheCoin.Clear();
                foreach (var record in GameDbf.Coin.GetRecords())
                {
                    if (record != null)
                    {
                        CacheCoin.Add(record.CardId);
                    }
                }
            }
            public static void UpdateCoinCard()
            {
                CacheCoinCard.Clear();
                foreach (var record in GameDbf.Coin.GetRecords())
                {
                    if (record != null)
                    {
                        CacheCoinCard.Add(record.CardId);
                    }
                }
            }

            public static void UpdateGameBoard()
            {
                CacheGameBoard.Clear();
                foreach (var record in GameDbf.Board.GetRecords())
                {
                    if (record != null)
                    {
                        CacheGameBoard.Add(record.ID);
                    }
                }
            }
            public static void UpdateBgsBoard()
            {
                CacheBgsBoard.Clear();
                foreach (var record in GameDbf.BattlegroundsBoardSkin.GetRecords())
                {
                    if (record != null)
                    {
                        CacheBgsBoard.Add(record.ID);
                    }
                }
            }
            public static void UpdateHeroes()
            {
                CacheHeroes.Clear();
                foreach (var record in GameDbf.CardHero.GetRecords())
                {
                    if (record != null)
                    {
                        CacheHeroes.Add(record.CardId, record.HeroType);
                    }
                }
            }
            public static void UpdateCardBack()
            {
                CacheCardBack.Clear();
                foreach (var record in GameDbf.CardBack.GetRecords())
                {
                    if (record != null)
                    {
                        CacheCardBack.Add(record.ID);
                    }
                }
            }
            public static void UpdateBgsFinisher()
            {
                CacheBgsFinisher.Clear();
                foreach (var record in GameDbf.BattlegroundsFinisher.GetRecords())
                {
                    if (record != null)
                    {
                        CacheBgsFinisher.Add(record.ID);
                    }
                }
            }

            public static void UpdateMercenarySkin()
            {
                CacheMercenarySkin.Clear();
                foreach (var merc in GameDbf.LettuceMercenary.GetRecords())
                {

                    if (merc != null && merc.MercenaryArtVariations.Count > 0)
                    {
                        MercenarySkin mercSkin = new MercenarySkin
                        {
                            Name = merc.MercenaryArtVariations.First().CardRecord.Name.GetString(),
                            Id = new List<int>(),
                            hasDiamond = false
                        };
                        foreach (var art in merc.MercenaryArtVariations.OrderBy(x => x.ID).ToList())
                        {
                            if (art != null)
                            {
                                mercSkin.Id.Add(art.CardId);
                                if (art.DefaultVariation)
                                {
                                    mercSkin.Default = art.CardId;
                                }
                                foreach (var premiums in art.MercenaryArtVariationPremiums)
                                {
                                    if (premiums != null && premiums.Premium == Assets.MercenaryArtVariationPremium.MercenariesPremium.PREMIUM_DIAMOND)
                                    {
                                        mercSkin.hasDiamond = true;
                                        mercSkin.Diamond = art.CardId;
                                    }
                                }
                            }
                        }
                        CacheMercenarySkin.Add(mercSkin);
                    }
                }
            }

        }

        public static void UpdateHeroPowerMapping()
        {
            HeroesPowerMapping.Clear();
            //HeroesPowerMapping.Add("CS2_083b_H1", "CS2_102_H1"); 求解存在问题，如，玛维的技能无法正确列出。
            foreach (var hero in HeroesMapping)
            {
                string rawHeroPower = GameUtils.GetHeroPowerCardIdFromHero(hero.Key);
                string aimHeroPower = GameUtils.GetHeroPowerCardIdFromHero(hero.Value);

                if (rawHeroPower != string.Empty && aimHeroPower != string.Empty && (!HeroesPowerMapping.ContainsKey(rawHeroPower)))
                {
                    HeroesPowerMapping.Add(rawHeroPower, aimHeroPower);
                    //MyLogger(LogLevel.Debug, "HeroPowerMapping: " + rawHeroPower + " -> " + aimHeroPower);
                }

            }
        }


        public static long CalcMercenaryCoinNeed(LettuceMercenary merc)
        {
            long coinNeed = 0;
            if (!merc.m_owned)
            {
                coinNeed = merc.GetCraftingCost() - merc.m_currencyAmount;
                return (coinNeed > 0) ? coinNeed : 4096;
            }

            foreach (var ability in merc.m_abilityList)
            {
                if (ability.GetNextUpgradeCost() <= 0)
                {
                    continue;
                }
                int tier = ability.GetNextTier() - 1;
                switch (tier)
                {
                    case 1:
                        coinNeed += 50 + 125 + 150 + 150;
                        break;
                    case 2:
                        coinNeed += 125 + 150 + 150;
                        break;
                    case 3:
                        coinNeed += 150 + 150; ;
                        break;
                    case 4:
                        coinNeed += 150;
                        break;
                    case 5:
                        coinNeed += 0;
                        break;
                }
            }
            foreach (var equipment in merc.m_equipmentList)
            {
                if (equipment.GetNextUpgradeCost() <= 0)
                {
                    continue;
                }
                int tier = equipment.GetNextTier() - 1;
                switch (tier)
                {
                    case 1:
                        coinNeed += 100 + 150 + 175;
                        break;
                    case 2:
                        coinNeed += 150 + 175;
                        break;
                    case 3:
                        coinNeed += 175; ;
                        break;
                    case 4:
                        coinNeed += 0;
                        break;
                }
            }
            coinNeed -= merc.m_currencyAmount;
            return (coinNeed > 0) ? coinNeed : 8192;
        }

        public static class CheckInfo
        {
            public static bool IsCoin()
            {
                if (CacheCoin.Count == 0) CacheInfo.UpdateCoin();
                if (CacheCoin.Contains(skinCoin.Value)) return true;
                else return false;
            }
            public static bool IsCoin(string cardId)
            {
                int dbId = GameUtils.TranslateCardIdToDbId(cardId);
                if (CacheCoinCard.Count == 0) CacheInfo.UpdateCoinCard();
                if (CacheCoinCard.Contains(dbId)) return true;
                else return false;
            }
            public static bool IsBoard()
            {
                if (CacheGameBoard.Count == 0) CacheInfo.UpdateGameBoard();
                if (CacheGameBoard.Contains(skinBoard.Value)) return true;
                else return false;
            }
            public static bool IsBgsBoard()
            {
                if (CacheBgsBoard.Count == 0) CacheInfo.UpdateBgsBoard();
                if (CacheBgsBoard.Contains(skinBgsBoard.Value)) return true;
                else return false;
            }
            public static bool IsHero(int DbID, out Assets.CardHero.HeroType heroType)
            {
                if (CacheHeroes.Count == 0) CacheInfo.UpdateHeroes();
                if (CacheHeroes.ContainsKey(DbID))
                {
                    heroType = CacheHeroes[DbID];
                    return true;
                }
                else
                {
                    heroType = Assets.CardHero.HeroType.UNKNOWN;
                    return false;
                }
            }
            public static bool IsCardBack()
            {
                if (CacheCardBack.Count == 0) CacheInfo.UpdateCardBack();
                if (CacheCardBack.Contains(skinCardBack.Value)) return true;
                else return false;
            }
            public static bool IsBgsFinisher()
            {
                if (CacheBgsFinisher.Count == 0) CacheInfo.UpdateBgsFinisher();
                if (CacheBgsFinisher.Contains(skinBgsFinisher.Value)) return true;
                else return false;
            }

            public static bool IsHero(string cardID, out Assets.CardHero.HeroType heroType)
            {
                if (CacheHeroes.Count == 0) CacheInfo.UpdateHeroes();
                int dbid = GameUtils.TranslateCardIdToDbId(cardID);
                if (CacheHeroes.ContainsKey(dbid))
                {
                    heroType = CacheHeroes[dbid];
                    return true;
                }
                else
                {
                    heroType = Assets.CardHero.HeroType.UNKNOWN;
                    return false;
                }
            }

            public static bool IsMercenarySkin(string cardID, out MercenarySkin skin)
            {
                if (CacheMercenarySkin.Count == 0) CacheInfo.UpdateMercenarySkin();
                int dbid = GameUtils.TranslateCardIdToDbId(cardID);

                foreach (var mercSkin in CacheMercenarySkin)
                {
                    if (mercSkin.Id.Contains(dbid))
                    {
                        skin = mercSkin;
                        return true;
                    }
                }
                skin = new MercenarySkin();
                return false;
            }

        }


        public static class LeakInfo
        {
            public static void Mercenaries(string savePath = @"BepInEx\HsMercenaries.log")
            {
                //List<LettuceTeam> teams = CollectionManager.Get().GetTeams();
                //System.IO.File.WriteAllText(savePath, DateTime.Now.ToLocalTime().ToString() + "\t获取到您的队伍如下：\n");
                //foreach (LettuceTeam team in teams)
                //{
                //    System.IO.File.AppendAllText(savePath, team.Name + "\n");
                //}
                System.IO.File.WriteAllText(savePath, DateTime.Now.ToLocalTime().ToString() + "\t获取到关卡信息如下：\n");
                System.IO.File.AppendAllText(savePath, "[ID]\t[Heroic?]\t[Bounty]\t[BossName]\n");
                foreach (var record in GameDbf.LettuceBounty.GetRecords())     // 生成关卡名称
                {
                    string saveString;
                    if (record != null)
                    {
                        saveString = record.ID.ToString() + (record.Heroic ? " Heroic " : " ") + record.BountySetRecord.Name.GetString() + " " + LettuceVillageDataUtil.GetBountyBossName(record);
                        System.IO.File.AppendAllText(savePath, saveString + "\n");
                    }
                }
            }
            public static void MyCards(string savePath = @"BepInEx\HsRefundCards.log")
            {
                System.IO.File.AppendAllText(savePath, DateTime.Now.ToLocalTime().ToString() + "\t获取到全额分解卡牌情况如下：\n");
                System.IO.File.AppendAllText(savePath, "[Name]\t[PremiumType]\t[Rarity]\t[CardId]\t[CardDbId]\t[OwnedCount]\n");
                //Filter<CollectibleCard> filter3 = new Filter<CollectibleCard>((CollectibleCard card) => card.IsRefundable);
                foreach (var record in CollectionManager.Get().GetOwnedCards())
                {
                    string saveString;
                    if (record != null && record.IsCraftable && record.IsRefundable && (record.OwnedCount > 0))
                    {
                        // GameUtils.TranslateDbIdToCardId(record.CardId, false);
                        saveString = $"{record.Name}\t{record.PremiumType}\t{record.Rarity}\t{record.CardId}\t{record.CardDbId}\t{record.OwnedCount}\t";
                        System.IO.File.AppendAllText(savePath, saveString + "\n");
                    }
                }
            }
            public static void Skins(string savePath = @"BepInEx\HsSkins.log")
            {
                System.IO.File.WriteAllText(savePath, DateTime.Now.ToLocalTime().ToString() + "\t获取到硬币皮肤如下：\n");
                System.IO.File.AppendAllText(savePath, "[CARD_ID]\t[Name]\n");
                foreach (var record in GameDbf.Coin.GetRecords())
                {
                    string saveString;
                    if (record != null)
                    {
                        saveString = $"{record.CardId}\t{record.Name.GetString()}";
                        System.IO.File.AppendAllText(savePath, saveString + "\n");
                    }
                }
                System.IO.File.AppendAllText(savePath, DateTime.Now.ToLocalTime().ToString() + "\t获取到卡背信息如下：\n");
                System.IO.File.AppendAllText(savePath, "[ID]\t[Name]\n");
                foreach (var record in GameDbf.CardBack.GetRecords())
                {
                    string saveString;
                    if (record != null)
                    {
                        // GameUtils.TranslateDbIdToCardId(record.CardId, false);
                        saveString = $"{record.ID}\t{record.Name.GetString()}";
                        System.IO.File.AppendAllText(savePath, saveString + "\n");
                    }
                }

                System.IO.File.AppendAllText(savePath, DateTime.Now.ToLocalTime().ToString() + "\t获取到游戏面板信息如下：\n");
                System.IO.File.AppendAllText(savePath, "[ID]\t[NOTE_DESC]\n");
                foreach (var record in GameDbf.Board.GetRecords())
                {
                    string saveString;
                    if (record != null)
                    {
                        // GameUtils.TranslateDbIdToCardId(record.CardId, false);
                        saveString = $"{record.ID}\t{record.GetVar("NOTE_DESC")}";
                        System.IO.File.AppendAllText(savePath, saveString + "\n");
                    }
                }

                System.IO.File.AppendAllText(savePath, DateTime.Now.ToLocalTime().ToString() + "\t获取到酒馆战斗面板如下：\n");
                System.IO.File.AppendAllText(savePath, "[ID]\t[CollectionShortName]\t[CollectionName]\n");
                foreach (var record in GameDbf.BattlegroundsBoardSkin.GetRecords())
                {
                    string saveString;
                    if (record != null)
                    {
                        // GameUtils.TranslateDbIdToCardId(record.CardId, false);
                        saveString = $"{record.ID}\t{record.CollectionShortName.GetString()}\t{record.CollectionName.GetString()}";
                        System.IO.File.AppendAllText(savePath, saveString + "\n");
                    }
                }

                System.IO.File.AppendAllText(savePath, DateTime.Now.ToLocalTime().ToString() + "\t获取到酒馆终结特效如下：\n");
                System.IO.File.AppendAllText(savePath, "[ID]\t[CollectionShortName]\t[CollectionName]\n");
                foreach (var record in GameDbf.BattlegroundsFinisher.GetRecords())
                {
                    string saveString;
                    if (record != null)
                    {
                        // GameUtils.TranslateDbIdToCardId(record.CardId, false);
                        saveString = $"{record.ID}\t{record.CollectionShortName.GetString()}\t{record.CollectionName.GetString()}";
                        System.IO.File.AppendAllText(savePath, saveString + "\n");
                    }
                }

                System.IO.File.AppendAllText(savePath, DateTime.Now.ToLocalTime().ToString() + "\t获取到英雄皮肤（包括酒馆）如下：\n");
                System.IO.File.AppendAllText(savePath, "[CARD_ID]\t[Name]\t[HeroType]\n");
                foreach (var record in GameDbf.CardHero.GetRecords())
                {
                    string saveString;
                    if (record != null)
                    {
                        string name = GameDbf.Card.GetRecord(record.CardId).Name.GetString();
                        // GameUtils.TranslateDbIdToCardId(record.CardId, false);
                        saveString = $"{record.CardId}\t{name}\t{record.HeroType}\t";
                        System.IO.File.AppendAllText(savePath, saveString + "\n");
                    }
                }
            }
        }
    }
}