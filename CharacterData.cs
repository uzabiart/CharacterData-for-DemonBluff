// Project: Demon Bluff (Sample Reference)
// File: CharacterData.cs  |  Version: v0.380e  |  Date: 2025-09-22
// Purpose: Reference-only implementation showing how characters are coded.
// License: All Rights Reserved – shared for educational reference only.
//          You may read and learn from this file, but you may not use this code in other projects without permission.
// Copyright (c) 2025 UmiArt. All rights reserved.
// Contact: pkwiatkowski@umiart.pl

using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "UMI/new Character")]
public class CharacterData : ScriptableObject
{
    public string characterId;
    public List<CharacterData> bundledCharacters = new List<CharacterData>();
    [TextArea(4, 10)]
    public string description;
    [TextArea(4, 10)]
    public string descriptionPL;
    [TextArea(4, 10)]
    public string descriptionCHN;
    [TextArea(4, 10)]
    public string flavorText;
    [TextArea(8, 30)]
    public string hints;
    [TextArea(4, 10)]
    public string ifLies;
    [TextArea(4, 10)]
    public string notes;
    public Sprite art;
    public Sprite art_cute;
    public Sprite art_nice;
    public Sprite art_animated;
    public Sprite randomArt;
    public Sprite backgroundArt;
    public SkinData currentSkin;
    public List<SkinData> skins = new List<SkinData>();
    public List<AchievementData> achievements = new List<AchievementData>();
    public Color color;
    public Color artBgColor;
    public Color cardBgColor;
    public Color cardBorderColor;
    public List<ECharacterTag> tags = new List<ECharacterTag>();
    public List<CharacterData> canAppearIf = new List<CharacterData>();
    public ECharacterType type;
    public EAlignment startingAlignment;
    public EAbilityUsage abilityUsage;
    public bool bluffable = true;
    public bool picking = true;

    [SerializeReference]
    public Role role;

    public Sprite GetArt()
    {
        if (currentSkin == null)
            return art_cute;
        else
            return currentSkin.art;
    }
    public Sprite GetAnimatedArt()
    {
        if (currentSkin == null)
            return art_animated;
        else
            return currentSkin.animated_art;
    }
    public EArtType GetArtType()
    {
        if (currentSkin == null)
            return EArtType.Default;

        return currentSkin.type;
    }

    [Button]
    public void GenerateCharacterId()
    {
        if (string.IsNullOrEmpty(characterId))
            characterId = $"{name}_{UnityEngine.Random.Range(0, 10)}{UnityEngine.Random.Range(0, 10)}{UnityEngine.Random.Range(0, 10)}{UnityEngine.Random.Range(0, 10)}{UnityEngine.Random.Range(0, 10)}{UnityEngine.Random.Range(0, 10)}{UnityEngine.Random.Range(0, 10)}{UnityEngine.Random.Range(0, 10)}";
    }

    public void LoadPreferences()
    {
        currentSkin = null;
        foreach (CharacterPreference cp in SavesGame.CharacterPreferences.prefs)
        {
            if (cp.chId == characterId)
                LoadSkin(cp.prefSkinId);
        }
    }
    public void LoadSkin(string skinId)
    {
        currentSkin = null;

        foreach (SkinData sd in skins)
            if (sd.skinId == skinId)
                currentSkin = sd;
    }
    public bool CheckIfSkinUnlocked(string skinId)
    {
        foreach (SkinData sd in skins)
            if (skinId == sd.skinId)
                return sd.CheckIfUnlocked();

        return false;
        //if (SavesGame.UnlockedSkins.ids.Contains(skinId)) return true;
        //return false;
    }
    public void ChangeSkin(SkinData skin)
    {
        if (skin != null)
            if (!skins.Contains(skin)) return;

        if (skin != null)
            if (!CheckIfSkinUnlocked(skin.skinId)) return;

        currentSkin = skin;

        SavesGame.UpdateCharacterPreference(this);
    }

    public string GetDescription()
    {
        string desc = StringHelper.ConvertTextToTextWithTooltips(description);

        if (ProjectContext.Instance.gameData.language == ELanguage.English)
            desc = StringHelper.ConvertTextToTextWithTooltips(description);
        if (ProjectContext.Instance.gameData.language == ELanguage.Polish)
            desc = StringHelper.ConvertTextToTextWithTooltips(descriptionPL);

        return desc;
    }
    public string GetIfLies()
    {
        string desc = StringHelper.ConvertTextToTextWithTooltips(ifLies);

        return desc;
    }
    public string GetHints()
    {
        string desc = StringHelper.ConvertTextToTextWithTooltips(hints);

        return desc;
    }

    public string GetArtistName()
    {
        string artist = "normandia";

        if (currentSkin != null)
            artist = currentSkin.artistName;

        return artist;
    }
}

public enum ERarity
{
    Default = 0,
    Common = 10,
    Uncommon = 20,
    Rare = 30,
    Mythical = 40,
}

public enum ECharacterType
{
    None = 0,
    Villager = 10,
    Outcast = 20,
    Minion = 30,
    Demon = 100
}

public enum EAbilityUsage
{
    Once = 0,
    ResetAfterNight = 10,
}

public enum ECharacterTag
{
    None = 0,
    Corrupt = 10,
    Bluff = 20,
}

[System.Serializable]
public abstract class Role
{
    public virtual List<SpecialRule> GetRules() => new List<SpecialRule>() { };
    public virtual List<ECharacterTag> GetTags() => new List<ECharacterTag>() { };
    public virtual string GetDreamerClue() => "I forgot my dream";
    public abstract string Description { get; }
    Character charRef;
    CharacterData dataRef;
    public Action<ActedInfo> onActed;
    public virtual void Init(Character charRef) { }
    public abstract ActedInfo GetInfo(Character charRef);
    public abstract ActedInfo GetBluffInfo(Character charRef);
    public virtual void Act(ETriggerPhase trigger, Character charRef)
    {
    }
    public virtual void ActOnDied(Character charRef)
    {
    }
    public virtual void OnStatusRemoved(ECharacterStatus status)
    {
    }
    public virtual bool CheckIfCanRemoveStatus(ECharacterStatus status)
    {
        return true;
    }
    public virtual int GetDamageToYou()
    {
        return 5;
    }
    public virtual void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        Act(trigger, charRef);
    }
    public virtual void Reveal()
    {
    }
    public virtual CharacterData GetBluffIfAble(Character charRef)
    {
        return null;
    }
    public virtual CharacterData GetRegisterAsRole(Character charRef)
    {
        return null;
    }
    public virtual bool CheckIfCanBeKilled(Character charRef)
    {
        return true;
    }
}

[System.Serializable]
public class Scout : Role
{
    public override string Description
    => "Learn how far is a specific Evil to another Evil";
    public override string GetDreamerClue() => "I forgot my dream";

    public override ActedInfo GetInfo(Character charRef)
    {
        string info = "";

        List<Character> allEvils = new List<Character>(Gameplay.CurrentCharacters);
        allEvils = Characters.Instance.FilterRealAlignmentCharacters(allEvils, EAlignment.Evil);
        allEvils = Characters.Instance.RemoveCharacterType<Recluse>(allEvils);

        Character pickedEvil = allEvils[UnityEngine.Random.Range(0, allEvils.Count)];

        int closestEvil = GetClosestEvilToEvil(pickedEvil, charRef);

        info = ConjourInfo(pickedEvil.GetRegisterAs().name, closestEvil);
        ActedInfo newInfo = new ActedInfo(info);
        return newInfo;
    }

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        onActed?.Invoke(GetInfo(charRef));
    }
    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        onActed?.Invoke(GetBluffInfo(charRef));
    }
    public override ActedInfo GetBluffInfo(Character charRef)
    {
        float randomId = UnityEngine.Random.Range(0f, 1f);
        List<Character> allEvils = new List<Character>(Gameplay.CurrentCharacters);
        allEvils = Characters.Instance.FilterRealAlignmentCharacters(allEvils, EAlignment.Evil);
        allEvils = Characters.Instance.RemoveCharacterType<Recluse>(allEvils);

        Character pickedEvil = allEvils[UnityEngine.Random.Range(0, allEvils.Count)];

        int id = GetClosestEvilToEvil(pickedEvil, charRef);
        id = Calculator.RemoveNumberAndGetRandomNumberFromList(id, 0, 3);

        string info = "";
        info = ConjourInfo(pickedEvil.dataRef.name, id);

        ActedInfo newInfo = new ActedInfo(info);
        return newInfo;
    }

    public int GetClosestEvilToEvil(Character pickedEvil, Character chRef)
    {
        int count = 0;
        int savedCount = 100;

        List<Character> myList = new List<Character>(Gameplay.CurrentCharacters);
        myList = CharactersHelper.GetSortedListWithCharacterFirst(myList, pickedEvil);

        myList.RemoveAt(0);
        for (int i = 0; i < myList.Count; i++)
        {
            if (myList[i].GetAlignment() == EAlignment.Evil)
            {
                savedCount = count;
                count = 0;
                break;
            }
            count++;
        }
        count = 0;
        for (int i = myList.Count - 1; i > 0; i--)
        {
            if (myList[i].GetAlignment() == EAlignment.Evil)
            {
                if (count < savedCount)
                {
                    savedCount = count;
                    count = 0;
                }
                break;
            }
            count++;
        }

        return savedCount;
    }

    public string ConjourInfo(string charName, int steps)
    {
        if (ProjectContext.Instance.gameData.language == ELanguage.Polish)
            return ScoutLoc.LocPL(charName, steps);

        if (steps > 20)
            return $"There is only 1 Evil";
        else if (steps == 0)
            return $"{charName} is\n{steps + 1} card away\nfrom closest Evil";
        else
            return $"{charName} is\n{steps + 1} cards away\nfrom closest Evil";
    }
}

[System.Serializable]
public class Knitter : Role
{
    public override string Description
    => "You start knowing how many pairs of evil players there are";

    public string ConjourInfo(int pairCount)
    {
        if (ProjectContext.Instance.gameData.language == ELanguage.Polish)
            return KnitterLoc.LocPL(pairCount);

        string info = "";
        if (pairCount == 0)
            info = "Evils are not adjacent to eachother";
        else if (pairCount == 1)
            //info = $"2 Evils are adjacent to eachother";
            info = $"There is only 1 pair of Evil";
        else
            info = $"There are {pairCount} pairs of Evil";
        return info;
    }

    public override ActedInfo GetInfo(Character charRef)
    {
        int pairCount = GetPairCount();
        string info = ConjourInfo(pairCount);
        ActedInfo newInfo = new ActedInfo(info);
        return newInfo;
    }

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        onActed?.Invoke(GetInfo(charRef));
    }
    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        onActed?.Invoke(GetBluffInfo(charRef));
    }
    public override ActedInfo GetBluffInfo(Character charRef)
    {
        int pairCount = GetPairCount();

        int allEvils = Gameplay.CurrentScript.minion + Gameplay.CurrentScript.demon;
        if (allEvils <= 1)
            allEvils = 2;
        int randomPairCount = Calculator.RemoveNumberAndGetRandomNumberFromList(pairCount, 0, allEvils);

        string info = ConjourInfo(randomPairCount);
        ActedInfo newInfo = new ActedInfo(info);
        return newInfo;
    }

    public int GetPairCount()
    {
        List<Character> myList = new List<Character>(Gameplay.CurrentCharacters);
        myList.Add(Gameplay.CurrentCharacters[0]);

        int pairCount = 0;
        bool evilPrev = false;
        foreach (Character ch in myList)
        {
            if (ch.GetAlignment() == EAlignment.Evil)
            {
                if (evilPrev)
                    pairCount++;
                evilPrev = true;
            }
            else
                evilPrev = false;
        }

        return pairCount;
    }
}

[System.Serializable]
public class Gossip : Role // Poet :
{
    public override string Description
    => "Learn random info";

    public override ActedInfo GetInfo(Character charRef)
    {
        ActedInfo newInfo = infoRoles[UnityEngine.Random.Range(0, infoRoles.Count)].GetInfo(charRef);
        return newInfo;
    }

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        onActed?.Invoke(GetInfo(charRef));
    }
    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        onActed?.Invoke(GetBluffInfo(charRef));
    }

    public override ActedInfo GetBluffInfo(Character charRef)
    {
        Role role = infoRoles[UnityEngine.Random.Range(0, infoRoles.Count)];
        Debug.Log($"{charRef.id}, ROLE: {role.GetType()}");
        ActedInfo newInfo = role.GetBluffInfo(charRef);
        return newInfo;
    }

    public List<Role> infoRoles = new List<Role>()
    {
        new Empath(),
        new Scout(),
        new Investigator(),
        new BountyHunter(),
        new Lookout(),
        new Knitter(),
        new Tracker(),
        new Shugenja(),
        new Noble(),
        new Bishop(),
        new Archivist(),
        new Acrobat2(),
    };
}


[System.Serializable]
public class Witness : Role
{
    public override string Description
    => "Learn a character that was affected by an Evil ability";

    public string ConjourInfo(Character messedCharacter)
    {
        if (ProjectContext.Instance.gameData.language == ELanguage.Polish)
            return WitnessLoc.LocPL(messedCharacter);

        string info = "";
        if (messedCharacter == null)
            info = "NO character was affected by an Evil";
        else
            info = $"#{messedCharacter.id} was affected by an Evil";
        return info;
    }

    public override ActedInfo GetInfo(Character charRef)
    {
        List<Character> messedCharacters = GetMessedCharacters();
        Character randomCharacter = null;
        if (messedCharacters.Count > 0)
        {
            randomCharacter = messedCharacters[UnityEngine.Random.Range(0, messedCharacters.Count)];
            messedCharacters.Clear();
            messedCharacters.Add(randomCharacter);
        }

        string info = ConjourInfo(randomCharacter);
        ActedInfo newInfo = new ActedInfo(info, messedCharacters);
        return newInfo;
    }

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        onActed?.Invoke(GetInfo(charRef));
    }
    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        onActed?.Invoke(GetBluffInfo(charRef));
    }
    public override ActedInfo GetBluffInfo(Character charRef)
    {
        List<Character> messedCharacters = GetMessedCharacters();
        List<Character> clearCharacters = new List<Character>();

        foreach (Character c in Gameplay.CurrentCharacters)
        {
            if (!messedCharacters.Contains(c))
                clearCharacters.Add(c);
        }

        Character randomCharacter = null;
        if (clearCharacters.Count > 0)
        {
            randomCharacter = clearCharacters[UnityEngine.Random.Range(0, clearCharacters.Count)];
            clearCharacters.Clear();
            clearCharacters.Add(randomCharacter);
        }

        string info = ConjourInfo(randomCharacter);
        ActedInfo newInfo = new ActedInfo(info, clearCharacters);
        return newInfo;
    }

    public List<Character> GetMessedCharacters()
    {
        List<Character> myList = new List<Character>(Gameplay.CurrentCharacters);
        List<Character> messedCharacters = new List<Character>();

        foreach (Character ch in myList)
        {
            if (ch.statuses.Contains(ECharacterStatus.MessedUpByEvil))
            {
                messedCharacters.Add(ch);
            }
        }

        return messedCharacters;
    }
}

[System.Serializable]
public class RangedEmpath : Role
{
    public override string Description
    => "Learn character that is adjacent to an Evil";

    public override ActedInfo GetInfo(Character charRef)
    {
        List<Character> tempList = new List<Character>(Gameplay.CurrentCharacters);
        tempList = Characters.Instance.FilterAlignmentCharacters(tempList, EAlignment.Evil);

        Character randomChar = tempList[UnityEngine.Random.Range(0, tempList.Count)];

        tempList = Characters.Instance.GetAdjacentCharacters(randomChar);

        randomChar = tempList[UnityEngine.Random.Range(0, tempList.Count)];

        string info = ConjourInfo(randomChar);

        ActedInfo newInfo = new ActedInfo(info);
        return newInfo;
    }

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        onActed?.Invoke(GetInfo(charRef));
    }
    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        onActed?.Invoke(GetBluffInfo(charRef));
    }

    public override ActedInfo GetBluffInfo(Character charRef)
    {
        List<Character> tempList = new List<Character>(Gameplay.CurrentCharacters);

        List<Character> potentialCharacters = new List<Character>();

        int adjacentGoods = 0;
        foreach (Character c in tempList)
        {
            adjacentGoods = 0;

            foreach (Character cc in Characters.Instance.GetAdjacentCharacters(c))
                if (cc.GetAlignment() == EAlignment.Good)
                    adjacentGoods++;

            if (adjacentGoods == 2)
                potentialCharacters.Add(c);
        }

        string info = "There are no Evils in play";
        Character randomChar = null;
        if (potentialCharacters.Count > 0)
        {
            randomChar = potentialCharacters[UnityEngine.Random.Range(0, potentialCharacters.Count)];
            info = ConjourInfo(randomChar);
        }

        ActedInfo newInfo = new ActedInfo(info);

        return newInfo;
    }

    public string ConjourInfo(Character adjacentToEvil)
    {
        string info = "";
        info = $"#{adjacentToEvil.id} is adjacent to an Evil";
        return info;
    }
}

[System.Serializable]
public class Architect : Role
{
    public override string Description
    => "Learn which side of the circle is more Evil";

    public enum ECircleSide
    {
        Both = 0,
        Left = 10,
        Right = 20,
    }

    public class ArchitectInfo
    {
        public ECircleSide side;
        public List<Character> characters = new List<Character>();
    }

    public override ActedInfo GetInfo(Character charRef)
    {
        ArchitectInfo infos = GetSideOfCircle(charRef, true);

        string info = ConjourInfo(infos.side);

        ActedInfo newInfo = new ActedInfo(info, infos.characters);
        return newInfo;
    }

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        onActed?.Invoke(GetInfo(charRef));
    }
    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        onActed?.Invoke(GetBluffInfo(charRef));
    }

    public override ActedInfo GetBluffInfo(Character charRef)
    {
        ArchitectInfo infos = GetSideOfCircle(charRef, false);

        string info = ConjourInfo(infos.side);

        ActedInfo newInfo = new ActedInfo(info, infos.characters);
        return newInfo;
    }

    public ArchitectInfo GetSideOfCircle(Character charRef, bool truth)
    {
        List<Character> tempList = new List<Character>(Gameplay.CurrentCharacters);

        int circleSize = tempList.Count;

        tempList.Add(tempList[0]);

        int i = 0;
        int leftEvils = 0;
        int rightEvils = 0;
        List<Character> leftChars = new List<Character>();
        List<Character> rightChars = new List<Character>();
        foreach (Character c in tempList)
        {
            if (i <= circleSize / 2)
            {
                leftChars.Add(c);
                if (c.GetAlignment() == EAlignment.Evil)
                    leftEvils++;
            }
            if (i >= (circleSize + 1) / 2)
            {
                rightChars.Add(c);
                if (c.GetAlignment() == EAlignment.Evil)
                    rightEvils++;
            }
            i++;
        }

        ArchitectInfo infos = new ArchitectInfo();

        infos.side = ECircleSide.Both;
        if (leftEvils > rightEvils)
            infos.side = ECircleSide.Left;
        if (leftEvils < rightEvils)
            infos.side = ECircleSide.Right;

        if (!truth)
        {
            bool isBoth = Calculator.RollDice(10) > 9 ? true : false;

            if (infos.side == ECircleSide.Left)
                infos.side = ECircleSide.Right;
            else if (infos.side == ECircleSide.Right)
                infos.side = ECircleSide.Left;

            if (infos.side == ECircleSide.Both)
            {
                if (Calculator.RollDice(10) >= 5)
                    infos.side = ECircleSide.Left;
                else
                    infos.side = ECircleSide.Right;
            }
            else if (infos.side != ECircleSide.Both)
                if (isBoth)
                    infos.side = ECircleSide.Both;
        }

        if (infos.side == ECircleSide.Left)
            infos.characters = leftChars;
        if (infos.side == ECircleSide.Right)
            infos.characters = rightChars;

        return infos;
    }

    public string ConjourInfo(ECircleSide side)
    {
        if (ProjectContext.Instance.gameData.language == ELanguage.Polish)
            return ArchitectLoc.LocPL(side);

        string info = "";
        if (side == ECircleSide.Left)
            info = $"Left side is more Evil";
        if (side == ECircleSide.Right)
            info = $"Right side is more Evil";
        if (side == ECircleSide.Both)
            info = $"Both sides are equally Evil";
        return info;
    }
}

[System.Serializable]
public class Empath : Role //= Lover(in game)
{
    public override string Description
        => "Learn how many Evil characters are adjacent to me";

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        onActed?.Invoke(GetInfo(charRef));
    }
    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        onActed?.Invoke(GetBluffInfo(charRef));
    }

    public override ActedInfo GetInfo(Character charRef)
    {
        int evils = CheckAdjacentEvils(charRef);
        string info = ConjourInfo(evils);
        ActedInfo newInfo = new ActedInfo(info, Characters.Instance.GetAdjacentCharacters(charRef));
        return newInfo;
    }

    public override ActedInfo GetBluffInfo(Character charRef)
    {
        List<int> possibleEvils = new List<int>();
        int allEvils = Gameplay.CurrentScript.minion + Gameplay.CurrentScript.demon;

        for (int i = 0; i < allEvils + 1; i++)
        {
            if (i == 3) break;
            possibleEvils.Add(i);
        }

        int evils = CheckAdjacentEvils(charRef);

        possibleEvils.Remove(evils);
        int randomEvilNumber = possibleEvils[UnityEngine.Random.Range(0, possibleEvils.Count)];
        string info = ConjourInfo(randomEvilNumber);
        ActedInfo newInfo = new ActedInfo(info, Characters.Instance.GetAdjacentCharacters(charRef));

        return newInfo;
    }

    public int CheckAdjacentEvils(Character charRef)
    {
        List<Character> adjacentCharacters = new List<Character>();
        foreach (Character ch in Gameplay.CurrentCharacters)
            if (charRef == ch)
            {
                adjacentCharacters = Characters.Instance.GetAdjacentCharacters(ch);
                break;
            }

        int evils = 0;

        foreach (Character ch in adjacentCharacters)
        {
            if (ch.GetAlignment() == EAlignment.Evil)
                evils++;
        }

        return evils;
    }

    public string ConjourInfo(int evils)
    {
        if (ProjectContext.Instance.gameData.language == ELanguage.Polish)
            return EmpathLoc.LocPL(evils);

        string info = "";
        if (evils == 0)
            info = $"NO Evils\nadjacent to me";
        else if (evils == 1)
            info = $"{evils} Evil\nadjacent to me";
        else
            info = $"{evils} Evils\nadjacent to me";

        return info;
    }
}

[System.Serializable]
public class Sapper : Role
{
    public override string Description
        => "Learn how many Evil characters are 2 cards away to the left and right of me.";

    public override ActedInfo GetInfo(Character charRef)
    {
        int evils = CheckAdjacentEvils(charRef);
        ActedInfo newInfo = new ActedInfo(ConjourInfo(evils));
        return newInfo;
    }

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        onActed?.Invoke(GetInfo(charRef));
    }
    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        onActed?.Invoke(GetBluffInfo(charRef));
    }
    public override ActedInfo GetBluffInfo(Character charRef)
    {
        int evils = CheckAdjacentEvils(charRef);

        int randomEvilsNumber = Calculator.RemoveNumberAndGetRandomNumberFromList(evils, 0, 3);

        ActedInfo newInfo = new ActedInfo(ConjourInfo(randomEvilsNumber));

        return newInfo;
    }

    public string ConjourInfo(int evils)
    {
        string info = $"{evils}";
        //if (evils == 0)
        //info = $"0";
        //else if (evils == 1)
        //info = $"{evils} Evil\nadjacent to me";
        //else
        //info = $"{evils} Evils\nadjacent to me";

        return info;
    }

    public int CheckAdjacentEvils(Character charRef)
    {
        List<Character> adjacentCharacters = new List<Character>(Gameplay.CurrentCharacters);

        adjacentCharacters = CharactersHelper.GetSortedListWithCharacterFirst(Gameplay.CurrentCharacters, charRef);

        adjacentCharacters.RemoveAt(0);

        int evils = 0;

        if (adjacentCharacters[0].GetAlignment() == EAlignment.Evil)
            evils++;
        if (adjacentCharacters[1].GetAlignment() == EAlignment.Evil)
            evils++;
        if (adjacentCharacters[adjacentCharacters.Count - 1].GetAlignment() == EAlignment.Evil)
            evils++;
        if (adjacentCharacters[adjacentCharacters.Count - 2].GetAlignment() == EAlignment.Evil)
            evils++;

        return evils;
    }
}

[System.Serializable]
public class Immortal : Role // Knight
{
    public override string Description
        => "I can't die";

    public override ActedInfo GetInfo(Character charRef)
    {
        ActedInfo newInfo = new ActedInfo("");
        return newInfo;
    }
    public override ActedInfo GetBluffInfo(Character charRef)
    {
        ActedInfo newInfo = new ActedInfo("");
        return newInfo;
    }

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger == ETriggerPhase.OnExecuted)
        {
            if (charRef.alignment != EAlignment.Evil)
                if (charRef.statuses.statuses.Contains(ECharacterStatus.Corrupted))
                    PlayerController.PlayerInfo.health.Damage(4);
        }
        if (trigger != ETriggerPhase.Day) return;
        //onActed?.Invoke(GetInfo());
    }
    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger == ETriggerPhase.OnExecuted)
        {
            if (charRef.alignment != EAlignment.Evil)
                if (charRef.statuses.statuses.Contains(ECharacterStatus.Corrupted))
                    PlayerController.PlayerInfo.health.Damage(4);
        }
        if (trigger != ETriggerPhase.Day) return;
        //onActed?.Invoke(GetInfo());
    }

    public string GetInfo()
    {
        return "I can't die";
    }

    public override bool CheckIfCanBeKilled(Character charRef)
    {
        if (charRef.statuses.statuses.Contains(ECharacterStatus.HealthyBluff))
            return false;
        if (charRef.statuses.statuses.Contains(ECharacterStatus.Corrupted))
            return true;
        else
            return false;
    }
}

[System.Serializable]
public class Mathematician : Role
{
    public override string Description
        => "Learn how many characters are Drunk or Mad";

    public override ActedInfo GetInfo(Character charRef)
    {
        int drunks = 0;
        foreach (Character ch in Gameplay.CurrentCharacters)
            if (ch.statuses.statuses.Contains(ECharacterStatus.Mad) || ch.statuses.statuses.Contains(ECharacterStatus.Corrupted))
                drunks++;

        string info = ConjourInfo(drunks);
        ActedInfo newInfo = new ActedInfo(info);
        return newInfo;
    }

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger == ETriggerPhase.Day)
            onActed?.Invoke(GetInfo(charRef));
    }

    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        onActed?.Invoke(GetBluffInfo(charRef));
    }

    public override ActedInfo GetBluffInfo(Character charRef)
    {
        float randomId = UnityEngine.Random.Range(0f, 1f);

        int id = 0;
        if (randomId < 0.35f)
            id = 0;
        else if (randomId < 0.8f)
            id = 1;
        else
            id = 2;

        string info = $"{ConjourInfo(id)}";
        ActedInfo newInfo = new ActedInfo(info);
        return newInfo;
    }

    public string ConjourInfo(int drunks)
    {
        string info = "";
        if (drunks == 1)
            info = "There is 1\nPoisoned or Mad";
        else
            info = $"There are {drunks}\nPoisoned or Mads";
        return "";
    }
}

[System.Serializable]
public class BountyHunter : Role
{
    public override string Description
        => "[1 Villager becomes Evil]. Learn which character is Evil.";

    public override ActedInfo GetInfo(Character charRef)
    {
        List<Character> characters = new List<Character>(Gameplay.CurrentCharacters);

        characters = Characters.Instance.FilterAlignmentCharacters(characters, EAlignment.Evil);
        Character randomEvil = characters[UnityEngine.Random.Range(0, characters.Count)];

        string info = ConjourInfo(randomEvil);
        ActedInfo newInfo = new ActedInfo(info);
        return newInfo;
    }

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger == ETriggerPhase.Day)
        {
            onActed?.Invoke(GetInfo(charRef));
        }
        if (trigger == ETriggerPhase.Start)
        {
            CreateNewEvil();
        }
    }

    private void CreateNewEvil()
    {
        List<Character> characters = new List<Character>(Gameplay.CurrentCharacters);

        characters = Characters.Instance.FilterAlignmentCharacters(characters, EAlignment.Good);
        Character randomGood = characters[UnityEngine.Random.Range(0, characters.Count)];

        randomGood.ChangeAlignment(EAlignment.Evil);
    }

    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        onActed?.Invoke(GetBluffInfo(charRef));
    }

    public override ActedInfo GetBluffInfo(Character charRef)
    {
        List<Character> characters = new List<Character>(Gameplay.CurrentCharacters);

        characters = Characters.Instance.FilterAlignmentCharacters(characters, EAlignment.Good);
        Character randomGood = characters[UnityEngine.Random.Range(0, characters.Count)];

        string info = ConjourInfo(randomGood);
        ActedInfo newInfo = new ActedInfo(info);
        return newInfo;
    }

    public string ConjourInfo(Character evilCharacter)
    {
        string info = "";
        info = $"#{evilCharacter.id}\nis Evil";
        return info;
    }
}
[System.Serializable]
public class Baker : Role
{
    public override string Description
        => "On Reveal: 1 random unrevealed Villager becomes baker.";

    public override ActedInfo GetInfo(Character charRef)
    {
        return new ActedInfo("");
    }

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger == ETriggerPhase.Day)
        {
            ShowMyPreviousRole(charRef);
            if (charRef.statuses.Contains(ECharacterStatus.BrokenAbility)) return;
            CreateNewBaker(charRef);
        }
    }

    private void ShowMyPreviousRole(Character charRef)
    {
        string prevCharacterName = "";

        if (charRef.GetRuntimeData() != null)
            prevCharacterName = ((BakerRuntimeData)(charRef.GetRuntimeData())).charName;
        else if (charRef.statuses.statuses.Contains(ECharacterStatus.AlteredCharacter))
            prevCharacterName = "Baker";

        string info = ConjourInfo(prevCharacterName);
        onActed?.Invoke(new ActedInfo(info));
    }

    private void CreateNewBaker(Character chRef)
    {
        List<Character> characters = new List<Character>(Gameplay.CurrentCharacters);

        characters = Characters.Instance.FilterCharacterType(characters, ECharacterType.Villager);
        characters = Characters.Instance.FilterAlignmentCharacters(characters, EAlignment.Good);
        characters = Characters.Instance.FilterHiddenCharacters(characters);

        characters.Remove(chRef);

        if (characters.Count == 0) return;

        Character randomGood = characters[UnityEngine.Random.Range(0, characters.Count)];

        CharacterData bakerData = CharactersHelper.GetCharacterDataOfRole(new Baker());

        randomGood.CreateRuntimeData(new BakerRuntimeData(randomGood.dataRef.name));
        randomGood.InitWithNoReset(bakerData);
    }

    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        ShowMyPreviousRoleLying(charRef);
    }
    private void ShowMyPreviousRoleLying(Character charRef)
    {
        bool bakerRevealed = false;
        foreach (Character c in Gameplay.CurrentCharacters)
            if (c.state != ECharacterState.Hidden)
                if (c.GetCharacterBluffIfAble().role is Baker)
                {
                    bakerRevealed = true;
                    break;
                }

        string prevCharacterName = "";
        if (bakerRevealed || charRef.GetRuntimeData() == null)
        {
            List<CharacterData> scriptVillagers = Gameplay.Instance.GetScriptCharactersOfType(ECharacterType.Villager);
            CharacterData removeCharacter = null;
            if (charRef.GetRuntimeData() != null)
                foreach (CharacterData cd in scriptVillagers)
                {
                    if (cd.name == ((BakerRuntimeData)(charRef.GetRuntimeData())).charName)
                    { removeCharacter = cd; break; }
                }

            if (removeCharacter != null)
                scriptVillagers.Remove(removeCharacter);

            prevCharacterName = scriptVillagers[UnityEngine.Random.Range(0, scriptVillagers.Count)].name;
        }

        string info = ConjourInfo(prevCharacterName);
        onActed?.Invoke(new ActedInfo(info));
    }

    public override ActedInfo GetBluffInfo(Character charRef)
    {
        return new ActedInfo("");
    }

    public string ConjourInfo(string prevCharName)
    {
        if (ProjectContext.Instance.gameData.language == ELanguage.Polish)
            return BakerLoc.LocPL(prevCharName);

        if (string.IsNullOrEmpty(prevCharName))
            return $"I am the original Baker";
        else
        {
            if (prevCharName[0] == 'A' || prevCharName[0] == 'E' || prevCharName[0] == 'I' || prevCharName[0] == 'O' || prevCharName[0] == 'U')
                return $"I was an {prevCharName}";
            else
                return $"I was a {prevCharName}";
        }
    }
}

[System.Serializable]
public class Alchemist : Role
{
    public override string Description
        => "2 characters to the left nad right of me are cured from Poison. Learn how many Poisoning I cured.";

    public AlchemistRuntimeData GetRuntimeData(Character charRef)
    {
        return (AlchemistRuntimeData)charRef.GetRuntimeData();
    }

    public override ActedInfo GetInfo(Character charRef)
    {
        int cures = 0;

        if (GetRuntimeData(charRef) != null)
            cures = GetRuntimeData(charRef).cures;

        string info = ConjourInfo(cures);
        List<Character> range = Characters.Instance.GetCharactersAtRange(2, charRef);
        range.AddRange(Characters.Instance.GetCharactersAtRange(1, charRef));
        ActedInfo newInfo = new ActedInfo(info, range);
        return newInfo;
    }

    private void CurePoisons(Character charRef)
    {
        charRef.CreateRuntimeData(new AlchemistRuntimeData(0));

        List<Character> poisonedCharacters = GetPoisonedCharactersAroundMe(charRef);

        foreach (Character ch in poisonedCharacters)
        {
            if (ch.statuses.CheckIfCanCurePoisonAndCure())
                GetRuntimeData(charRef).cures++;
        }
    }
    public List<Character> GetPoisonedCharactersAroundMe(Character charRef)
    {
        List<Character> myList = CharactersHelper.GetSortedListWithCharacterFirst(Gameplay.CurrentCharacters, charRef);
        List<Character> poisonedCharacters = new List<Character>();

        myList.RemoveAt(0);
        for (int i = 0; i < myList.Count; i++)
        {
            if (i > 1) break;
            if (myList[i].statuses.statuses.Contains(ECharacterStatus.Corrupted))
            {
                poisonedCharacters.Add(myList[i]);
            }
        }

        int j = 0;

        for (int i = myList.Count - 1; i > 0; i--)
        {
            if (j > 1) break;
            if (myList[i].statuses.statuses.Contains(ECharacterStatus.Corrupted))
            {
                poisonedCharacters.Add(myList[i]);
            }
            j++;
        }

        return poisonedCharacters;
    }

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger == ETriggerPhase.Start)
        {
            if (charRef.statuses.Contains(ECharacterStatus.BrokenAbility)) return;
            CurePoisons(charRef);
        }
        if (trigger == ETriggerPhase.Day)
        {
            onActed?.Invoke(GetInfo(charRef));
        }
    }

    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger == ETriggerPhase.Start)
        {
            charRef.CreateRuntimeData(new AlchemistRuntimeData(0));
        }

        if (trigger != ETriggerPhase.Day) return;

        onActed?.Invoke(GetBluffInfo(charRef));
    }
    public override ActedInfo GetBluffInfo(Character charRef)
    {
        List<Character> poisonedCharacters = GetPoisonedCharactersAroundMe(charRef);

        int id = Calculator.RemoveNumberAndGetRandomNumberFromList(poisonedCharacters.Count, 1, 3);

        string info = ConjourInfo(id);
        List<Character> range = Characters.Instance.GetCharactersAtRange(2, charRef);
        range.AddRange(Characters.Instance.GetCharactersAtRange(1, charRef));
        ActedInfo newInfo = new ActedInfo(info, range);
        return newInfo;
    }

    public string ConjourInfo(int howManyCures)
    {
        if (ProjectContext.Instance.gameData.language == ELanguage.Polish)
            return AlchemistLoc.LocPL(howManyCures);

        if (howManyCures == 1)
            return $"I cured {howManyCures}\nCorruption";
        else
            return $"I cured {howManyCures}\nCorruptions";
    }
}

[System.Serializable]
public class Dreamer : Role
{
    public override string Description
        => "Pick a player. Learn an Evil role. If Evil player picked, learn correct info";

    public override ActedInfo GetInfo(Character charRef)
    {
        return new ActedInfo("");
    }
    public override ActedInfo GetBluffInfo(Character charRef)
    {
        return new ActedInfo("");
    }

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        CharacterPicker.Instance.StartPickCharacters(1);
        CharacterPicker.OnCharactersPicked += CharacterPicked;
        CharacterPicker.OnStopPick += StopPick;
    }
    private void CharacterPicked()
    {
        CharacterPicker.OnCharactersPicked -= CharacterPicked;
        CharacterPicker.OnStopPick -= StopPick;

        Character c = CharacterPicker.PickedCharacters[0];
        List<Character> pickedCharacters = new List<Character>();
        pickedCharacters.Add(c);
        string info = $"#{c.id} could be: ";
        if (c.dataRef.role is Recluse)
        {
            info += "\na Cabbage";
        }
        else if (c.GetAlignment() == EAlignment.Evil)
            info += $"{c.GetCharacterData().name}";
        else
        {
            List<CharacterData> evilCharacters = new List<CharacterData>(Gameplay.Instance.GetScriptCharacters());
            evilCharacters = Characters.Instance.FilterAlignmentCharacters(evilCharacters, EAlignment.Evil);
            CharacterData pickedCh = evilCharacters[UnityEngine.Random.Range(0, evilCharacters.Count)];
            info += $"{pickedCh.name}";
        }

        onActed?.Invoke(new ActedInfo(info, pickedCharacters));
        Debug.Log($"{info}");
    }

    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        CharacterPicker.Instance.StartPickCharacters(1);
        CharacterPicker.OnCharactersPicked += CharacterPickedDrunk;
        CharacterPicker.OnStopPick += StopPick;
    }
    private void StopPick()
    {
        CharacterPicker.OnCharactersPicked -= CharacterPickedDrunk;
        CharacterPicker.OnCharactersPicked -= CharacterPicked;
        CharacterPicker.OnStopPick -= StopPick;
    }

    private void CharacterPickedDrunk()
    {
        CharacterPicker.OnCharactersPicked -= CharacterPickedDrunk;
        CharacterPicker.OnStopPick -= StopPick;

        Character c = CharacterPicker.PickedCharacters[0];
        string info = $"#{c.id} could be: ";

        List<CharacterData> evilCharacters = new List<CharacterData>(Gameplay.Instance.GetScriptCharacters());
        evilCharacters = Characters.Instance.FilterAlignmentCharacters(evilCharacters, EAlignment.Evil);
        evilCharacters.Remove(c.dataRef);

        if (evilCharacters.Count == 0)
        {
            evilCharacters = new List<CharacterData>(Gameplay.Instance.GetAllAscensionCharacters());
            evilCharacters = Characters.Instance.FilterAlignmentCharacters(evilCharacters, EAlignment.Evil);
            evilCharacters.Remove(c.dataRef);
        }

        if (evilCharacters.Count == 0)
        {
            evilCharacters = new List<CharacterData>(Gameplay.Instance.GetAllAscensionCharacters());
        }

        CharacterData pickedCh = evilCharacters[UnityEngine.Random.Range(0, evilCharacters.Count)];
        info += $"{pickedCh.name}";

        onActed?.Invoke(new ActedInfo(info));
        Debug.Log($"{info}");
    }
}

[System.Serializable]
public class Dreamer2 : Role
{
    public override string Description
        => "Pick a character. Learn info about its role.";

    public override ActedInfo GetInfo(Character charRef)
    {
        return new ActedInfo("");
    }
    public override ActedInfo GetBluffInfo(Character charRef)
    {
        return new ActedInfo("");
    }
    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        CharacterPicker.Instance.StartPickCharacters(1);
        CharacterPicker.OnCharactersPicked += CharacterPicked;
        CharacterPicker.OnStopPick += StopPick;
    }
    private void CharacterPicked()
    {
        CharacterPicker.OnCharactersPicked -= CharacterPicked;
        CharacterPicker.OnStopPick -= StopPick;

        Character c = CharacterPicker.PickedCharacters[0];
        string info = $"#{c.id} could be: ";
        if (c.GetAlignment() == EAlignment.Evil)
            info += $"{c.GetCharacterData().name}";
        else
        {
            List<CharacterData> evilCharacters = new List<CharacterData>(Gameplay.Instance.GetScriptCharacters());
            evilCharacters = Characters.Instance.FilterAlignmentCharacters(evilCharacters, EAlignment.Evil);
            CharacterData pickedCh = evilCharacters[UnityEngine.Random.Range(0, evilCharacters.Count)];
            info += $"{pickedCh.name}";
        }

        onActed?.Invoke(new ActedInfo(info));
        Debug.Log($"{info}");
    }

    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        CharacterPicker.Instance.StartPickCharacters(1);
        CharacterPicker.OnCharactersPicked += CharacterPickedDrunk;
        CharacterPicker.OnStopPick += StopPick;
    }
    private void StopPick()
    {
        CharacterPicker.OnCharactersPicked -= CharacterPickedDrunk;
        CharacterPicker.OnCharactersPicked -= CharacterPicked;
        CharacterPicker.OnStopPick -= StopPick;
    }

    private void CharacterPickedDrunk()
    {
        CharacterPicker.OnCharactersPicked -= CharacterPickedDrunk;
        CharacterPicker.OnStopPick -= StopPick;

        Character c = CharacterPicker.PickedCharacters[0];
        string info = $"#{c.id} could be: ";

        List<CharacterData> evilCharacters = new List<CharacterData>(Gameplay.Instance.GetScriptCharacters());
        evilCharacters = Characters.Instance.FilterAlignmentCharacters(evilCharacters, EAlignment.Evil);
        CharacterData pickedCh = evilCharacters[UnityEngine.Random.Range(0, evilCharacters.Count)];
        info += $"{pickedCh.name}";

        onActed?.Invoke(new ActedInfo(info));
        Debug.Log($"{info}");
    }
}

[System.Serializable]
public class Slayer : Role
{
    Character chRef;

    public override string Description
        => "Pick a character. If its Evil I die.";

    public override ActedInfo GetInfo(Character charRef)
    {
        return new ActedInfo("");
    }
    public override ActedInfo GetBluffInfo(Character charRef)
    {
        return new ActedInfo("");
    }
    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        chRef = charRef;
        CharacterPicker.Instance.StartPickCharacters(1);
        CharacterPicker.OnCharactersPicked += CharacterPicked;
        CharacterPicker.OnStopPick += StopPick;
    }
    private void CharacterPicked()
    {
        CharacterPicker.OnCharactersPicked -= CharacterPicked;
        CharacterPicker.OnStopPick -= StopPick;

        List<Character> chars = new List<Character>();
        chars.Add(CharacterPicker.PickedCharacters[0]);

        string info = $"";
        bool shouldExecute = false;

        if (chars[0].GetAlignment() == EAlignment.Evil)
        {
            info = ConjourInfo(chars[0].id, EAlignment.Evil);
            shouldExecute = true;
        }
        else
            info = ConjourInfo(chars[0].id, EAlignment.Good);

        if (chars[0].state == ECharacterState.Dead)
        {
            shouldExecute = false;
            return;
        }

        onActed?.Invoke(new ActedInfo(info, chars));
        Debug.Log($"{info}");

        if (shouldExecute)
            chars[0].KillAndReveal();
    }

    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        chRef = charRef;
        CharacterPicker.Instance.StartPickCharacters(1);
        CharacterPicker.OnCharactersPicked += CharacterPickedDrunk;
        CharacterPicker.OnStopPick += StopPick;
    }
    private void StopPick()
    {
        CharacterPicker.OnCharactersPicked -= CharacterPickedDrunk;
        CharacterPicker.OnCharactersPicked -= CharacterPicked;
        CharacterPicker.OnStopPick -= StopPick;
    }

    private void CharacterPickedDrunk()
    {
        CharacterPicker.OnCharactersPicked -= CharacterPickedDrunk;
        CharacterPicker.OnStopPick -= StopPick;

        List<Character> chars = new List<Character>();
        chars.Add(CharacterPicker.PickedCharacters[0]);

        string info = ConjourInfo(chars[0].id, EAlignment.Good);
        onActed?.Invoke(new ActedInfo(info, chars));
        Debug.Log($"{info}");
    }

    public string ConjourInfo(int id, EAlignment alignment)
    {
        if (ProjectContext.Instance.gameData.language == ELanguage.Polish)
            return SlayerLoc.LocPL(id, alignment);

        string info = $"";
        if (alignment == EAlignment.Evil)
            info += $"I killed Evil\nat #{id}";
        else if (alignment == EAlignment.Good)
            info += $"I couldn't kill\n#{id}";

        return info;
    }
}

[System.Serializable]
public class FortuneTeller : Role
{
    public override string Description
        => "Pick 2 players. Learn if any of them is Evil";

    public override ActedInfo GetInfo(Character charRef)
    {
        return new ActedInfo("");
    }
    public override ActedInfo GetBluffInfo(Character charRef)
    {
        return new ActedInfo("");
    }
    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        CharacterPicker.Instance.StartPickCharacters(2);
        CharacterPicker.OnCharactersPicked += CharacterPicked;
        CharacterPicker.OnStopPick += StopPick;
    }
    private void StopPick()
    {
        CharacterPicker.OnCharactersPicked -= CharacterPicked;
        CharacterPicker.OnCharactersPicked -= CharacterPickedDrunk;
        CharacterPicker.OnStopPick -= StopPick;
    }

    private void CharacterPicked()
    {
        CharacterPicker.OnCharactersPicked -= CharacterPicked;
        CharacterPicker.OnStopPick -= StopPick;

        bool isEvil = false;
        foreach (Character c in CharacterPicker.PickedCharacters)
        {
            if (c.GetAlignment() == EAlignment.Evil)
                isEvil = true;
        }

        List<Character> chars = new List<Character>(CharacterPicker.PickedCharacters);
        chars = chars
            .OrderBy(c => c.id)
            .ThenBy(_ => UnityEngine.Random.value)
            .ToList();

        string info = $"Is #{chars[0].id} or #{chars[1].id} Evil?: {isEvil}";

        ActedInfo actedInfo = new ActedInfo(info, chars);
        onActed?.Invoke(actedInfo);
        Debug.Log($"{info}");

        CheckAchievementsAndUnlockIfAble(actedInfo);
    }
    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        CharacterPicker.Instance.StartPickCharacters(2);
        CharacterPicker.OnCharactersPicked += CharacterPickedDrunk;
        CharacterPicker.OnStopPick += StopPick;
    }
    private void CharacterPickedDrunk()
    {
        CharacterPicker.OnCharactersPicked -= CharacterPickedDrunk;
        CharacterPicker.OnStopPick -= StopPick;

        float randomId = UnityEngine.Random.Range(0f, 1f);

        bool isEvil = true;

        foreach (Character c in CharacterPicker.PickedCharacters)
        {
            if (c.GetAlignment() == EAlignment.Evil)
                isEvil = false;
        }

        List<Character> chars = new List<Character>(CharacterPicker.PickedCharacters);
        chars = chars
            .OrderBy(c => c.id)
            .ThenBy(_ => UnityEngine.Random.value)
            .ToList();

        string info = $"Is #{chars[0].id} or #{chars[1].id} Evil?: {isEvil}";

        onActed?.Invoke(new ActedInfo(info, chars));
        Debug.Log($"{info}");
    }

    //ACHIEVEMENTS
    private void CheckAchievementsAndUnlockIfAble(ActedInfo info)
    {
        if (info.characters[0].GetAlignment() == EAlignment.Evil && info.characters[1].GetAlignment() == EAlignment.Evil)
            ProjectContext.UnlockAchievement("FTeller_ACHIV_7689");
    }
}

[System.Serializable]
public class Lookout : Role // Medium
{
    public override string Description
        => "Learn that a character is a particular Villager";

    public override ActedInfo GetInfo(Character charRef)
    {
        List<Character> allCharacters = new List<Character>(Gameplay.CurrentCharacters);
        allCharacters = Characters.Instance.FilterAlignmentCharacters(allCharacters, EAlignment.Good);

        if (allCharacters.Count > 1)
            allCharacters.Remove(charRef);

        List<Character> pickedCh = new List<Character>();
        pickedCh.Add(allCharacters[UnityEngine.Random.Range(0, allCharacters.Count)]);


        string info = "";
        info += $"#{pickedCh[0].id} is a real\n";
        info += $"{pickedCh[0].GetCharacterData().name}";

        ActedInfo newInfo = new ActedInfo(info, pickedCh);
        return newInfo;
    }

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        onActed?.Invoke(GetInfo(charRef));
    }
    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        onActed?.Invoke(GetBluffInfo(charRef));
    }
    public override ActedInfo GetBluffInfo(Character charRef)
    {
        List<Character> allCharacters = new List<Character>(Gameplay.CurrentCharacters);
        List<Character> filteredAllCharacters = new List<Character>();

        foreach (Character c in allCharacters)
            if (c.bluff != null)
                if (c != charRef)
                    filteredAllCharacters.Add(c);

        if (filteredAllCharacters.Count == 0)
            foreach (Character c in allCharacters)
                if (c.bluff != null)
                    filteredAllCharacters.Add(c);

        List<Character> pickedCh = new List<Character>();
        pickedCh.Add(filteredAllCharacters[UnityEngine.Random.Range(0, filteredAllCharacters.Count)]);

        string info = "";
        info += $"#{pickedCh[0].id} is a real\n";
        info += $"{pickedCh[0].bluff.name}";

        ActedInfo newInfo = new ActedInfo(info, pickedCh);
        return newInfo;
    }
}
[System.Serializable]
public class Noble : Role // Empress :
{
    public override string Description
        => $"Learn 3 players. Only 1 is Evil";

    public override ActedInfo GetInfo(Character charRef)
    {
        string info = "";

        List<Character> good = new List<Character>(Gameplay.CurrentCharacters);
        good = Characters.Instance.FilterAlignmentCharacters(good, EAlignment.Good);
        good.Remove(charRef);

        List<Character> evils = new List<Character>(Gameplay.CurrentCharacters);
        evils = Characters.Instance.FilterAlignmentCharacters(evils, EAlignment.Evil);

        List<Character> picked = new List<Character>();
        Character pick = good[UnityEngine.Random.Range(0, good.Count)];
        picked.Add(pick);
        good.Remove(pick);
        pick = good[UnityEngine.Random.Range(0, good.Count)];
        picked.Add(pick);
        pick = evils[UnityEngine.Random.Range(0, evils.Count)];
        picked.Add(pick);

        picked = picked
            .OrderBy(c => c.id)
            .ThenBy(_ => UnityEngine.Random.value)
            .ToList();

        info += $"One is Evil:\n#{picked[0].id}, #{picked[1].id} or #{picked[2].id}";

        ActedInfo newInfo = new ActedInfo(info, picked);
        return newInfo;
    }

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        onActed?.Invoke(GetInfo(charRef));
    }
    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        onActed?.Invoke(GetBluffInfo(charRef));
    }
    public override ActedInfo GetBluffInfo(Character charRef)
    {
        string info = "";

        List<Character> good = new List<Character>(Gameplay.CurrentCharacters);
        good = Characters.Instance.FilterAlignmentCharacters(good, EAlignment.Good);
        good.Remove(charRef);

        List<Character> picked = new List<Character>();
        Character pick = good[UnityEngine.Random.Range(0, good.Count)];
        picked.Add(pick);
        good.Remove(pick);
        pick = good[UnityEngine.Random.Range(0, good.Count)];
        picked.Add(pick);
        good.Remove(pick);
        pick = good[UnityEngine.Random.Range(0, good.Count)];
        picked.Add(pick);

        picked = picked
            .OrderBy(c => c.id)
            .ThenBy(_ => UnityEngine.Random.value)
            .ToList();

        info += $"One is Evil:\n#{picked[0].id}, #{picked[1].id} or #{picked[2].id}";

        ActedInfo newInfo = new ActedInfo(info, picked);
        return newInfo;
    }
}
[System.Serializable]
public class Archivist : Role // Gemcrafter :
{
    public override string Description
        => $"Learn 1 Good character";

    public override ActedInfo GetInfo(Character charRef)
    {
        string info = "";

        List<Character> good = new List<Character>(Gameplay.CurrentCharacters);
        good = Characters.Instance.FilterAlignmentCharacters(good, EAlignment.Good);

        if (good.Count > 1)
            good.Remove(charRef);

        List<Character> pick = new List<Character>();
        pick.Add(good[UnityEngine.Random.Range(0, good.Count)]);

        info += $"#{pick[0].id} is Good";

        ActedInfo newInfo = new ActedInfo(info, pick);
        return newInfo;
    }

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        onActed?.Invoke(GetInfo(charRef));
    }
    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        onActed?.Invoke(GetBluffInfo(charRef));
    }

    public override ActedInfo GetBluffInfo(Character charRef)
    {
        string info = "";

        List<Character> evils = new List<Character>(Gameplay.CurrentCharacters);
        evils = Characters.Instance.FilterAlignmentCharacters(evils, EAlignment.Evil);

        if (evils.Count > 1)
            evils.Remove(charRef);

        List<Character> pick = new List<Character>();
        pick.Add(evils[UnityEngine.Random.Range(0, evils.Count)]);

        info += $"#{pick[0].id} is Good";
        ActedInfo newInfo = new ActedInfo(info, pick);
        return newInfo;
    }
}
[System.Serializable]
public class Bishop : Role
{
    public override string Description
        => "Learn 3 players. They each are Outsider, Town and Minion. Can add +1 Outsider?";

    public override ActedInfo GetInfo(Character charRef)
    {
        List<Character> pickedCharacters = new List<Character>();

        List<Character> allCharacters = new List<Character>(Gameplay.CurrentCharacters);
        allCharacters = Characters.Instance.FilterCharacterType(allCharacters, ECharacterType.Outcast);
        if (allCharacters.Count > 0)
            pickedCharacters.Add(allCharacters[UnityEngine.Random.Range(0, allCharacters.Count)]);

        allCharacters = new List<Character>(Gameplay.CurrentCharacters);
        allCharacters = Characters.Instance.FilterCharacterType(allCharacters, ECharacterType.Villager);
        if (allCharacters.Count > 0)
            pickedCharacters.Add(allCharacters[UnityEngine.Random.Range(0, allCharacters.Count)]);

        allCharacters = new List<Character>(Gameplay.CurrentCharacters);
        allCharacters = Characters.Instance.FilterCharacterType(allCharacters, ECharacterType.Minion);
        if (allCharacters.Count > 0)
            pickedCharacters.Add(allCharacters[UnityEngine.Random.Range(0, allCharacters.Count)]);

        if (allCharacters.Count == 0)
        {
            allCharacters = new List<Character>(Gameplay.CurrentCharacters);
            allCharacters = Characters.Instance.FilterCharacterType(allCharacters, ECharacterType.Demon);
            pickedCharacters.Add(allCharacters[UnityEngine.Random.Range(0, allCharacters.Count)]);
        }

        System.Random random = new System.Random();

        pickedCharacters = pickedCharacters
            .OrderBy(c => c.id)
            .ThenBy(_ => UnityEngine.Random.value)
            .ToList();

        string info = "Between\n";
        if (pickedCharacters.Count == 2)
        {
            info += $"#{pickedCharacters[0].id}, #{pickedCharacters[1].id}";
        }
        else if (pickedCharacters.Count == 3)
        {
            info += $"#{pickedCharacters[0].id}, #{pickedCharacters[1].id}, #{pickedCharacters[2].id}";
        }
        else
            foreach (Character c in pickedCharacters)
                info += $"#{c.id}, ";

        pickedCharacters = pickedCharacters.OrderBy(x => random.Next()).ToList();

        info += "\nthere is:\n";
        pickedCharacters = ListHelper.ShuffleList(pickedCharacters);
        if (pickedCharacters.Count == 2)
            info += $"{pickedCharacters[0].GetCharacterData().type.ToString()} and {pickedCharacters[1].GetCharacterData().type.ToString()}";
        if (pickedCharacters.Count == 3)
            info += $"{pickedCharacters[0].GetCharacterData().type.ToString()}, {pickedCharacters[1].GetCharacterData().type.ToString()} and {pickedCharacters[2].GetCharacterData().type.ToString()}";

        List<Character> chars = new List<Character>(pickedCharacters);

        ActedInfo newInfo = new ActedInfo(info, chars);
        return newInfo;
    }

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        onActed?.Invoke(GetInfo(charRef));
    }
    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        onActed?.Invoke(GetBluffInfo(charRef));
    }

    public override ActedInfo GetBluffInfo(Character charRef)
    {
        List<Character> pickedCharacters = new List<Character>();

        List<Character> allCharacters = new List<Character>(Gameplay.CurrentCharacters);
        allCharacters = Characters.Instance.FilterCharacterType(allCharacters, ECharacterType.Villager);

        Character picked = allCharacters[UnityEngine.Random.Range(0, allCharacters.Count)];
        pickedCharacters.Add(picked);
        allCharacters.Remove(picked);

        picked = allCharacters[UnityEngine.Random.Range(0, allCharacters.Count)];
        pickedCharacters.Add(picked);
        allCharacters.Remove(picked);

        if (Gameplay.CurrentScript.outs > 0)
        {
            picked = allCharacters[UnityEngine.Random.Range(0, allCharacters.Count)];
            pickedCharacters.Add(picked);
            allCharacters.Remove(picked);
        }

        System.Random random = new System.Random();

        pickedCharacters = pickedCharacters
            .OrderBy(c => c.id)
            .ThenBy(_ => UnityEngine.Random.value)
            .ToList();

        string info = "Between\n";
        if (pickedCharacters.Count == 2)
        {
            info += $"#{pickedCharacters[0].id}, #{pickedCharacters[1].id}";
        }
        else if (pickedCharacters.Count == 3)
        {
            info += $"#{pickedCharacters[0].id}, #{pickedCharacters[1].id}, #{pickedCharacters[2].id}";
        }
        else
            foreach (Character c in pickedCharacters)
                info += $"#{c.id}, ";

        List<ECharacterType> possiblePicks = new List<ECharacterType>();

        if (Gameplay.CurrentScript.minion > 0)
            possiblePicks.Add(ECharacterType.Minion);
        else
            possiblePicks.Add(ECharacterType.Demon);

        if (Gameplay.CurrentScript.outs > 0)
            possiblePicks.Add(ECharacterType.Outcast);
        if (Gameplay.CurrentScript.town > 0)
            possiblePicks.Add(ECharacterType.Villager);

        possiblePicks = possiblePicks.OrderBy(x => random.Next()).ToList();

        info += "\nthere is:\n";
        pickedCharacters = ListHelper.ShuffleList(pickedCharacters);
        if (possiblePicks.Count == 2)
            info += $"{possiblePicks[0].ToString()} and {possiblePicks[1].ToString()}";
        if (possiblePicks.Count == 3)
            info += $"{possiblePicks[0].ToString()}, {possiblePicks[1].ToString()} and {possiblePicks[2].ToString()}";

        List<Character> chars = new List<Character>(pickedCharacters);

        ActedInfo newInfo = new ActedInfo(info, chars);
        return newInfo;
    }
}

[System.Serializable]
public class Shugenja : Role // Enlightened
{
    public override string Description
        => "Learn if closest Evil is clockwise or counter-clockwise. Learn 'either' if equidistant.";

    public override ActedInfo GetInfo(Character charRef)
    {
        EEvilDirection dir = GetDirectionToEvil(charRef);
        string direction = "";

        if (dir == EEvilDirection.Clockwise)
            direction = "Clockwise";
        if (dir == EEvilDirection.Counterclockwise)
            direction = "Counter-clockwise";

        string info = $"Closest Evil is:\n{direction}";
        if (dir == EEvilDirection.Either)
            info = "Closest Evil is equidistant";

        //List<Character> chs = GetMarkedCharacters(dir, charRef);
        charRef.CreateRuntimeData(new EnlightenedRuntimeData(dir));

        ActedInfo newInfo = new ActedInfo(info);
        return newInfo;
    }

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        onActed?.Invoke(GetInfo(charRef));
    }
    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        onActed?.Invoke(GetBluffInfo(charRef));
    }

    public override ActedInfo GetBluffInfo(Character charRef)
    {
        EEvilDirection dir = GetDirectionToEvil(charRef);
        EEvilDirection fakeDirection = EEvilDirection.Either;

        string direction = "";
        if (dir == EEvilDirection.Clockwise)
        {
            fakeDirection = EEvilDirection.Counterclockwise;
            float randomPick = UnityEngine.Random.Range(0f, 1f);
            if (randomPick < 0.2)
                fakeDirection = EEvilDirection.Either;
        }

        if (dir == EEvilDirection.Counterclockwise)
        {
            fakeDirection = EEvilDirection.Clockwise;
            float randomPick = UnityEngine.Random.Range(0f, 1f);
            if (randomPick < 0.2)
                fakeDirection = EEvilDirection.Either;
        }

        if (dir == EEvilDirection.Either)
        {
            float randomPick = UnityEngine.Random.Range(0f, 1f);
            if (randomPick < 0.5)
                fakeDirection = EEvilDirection.Clockwise;
            else
                fakeDirection = EEvilDirection.Counterclockwise;
        }

        if (fakeDirection == EEvilDirection.Clockwise)
            direction = "Clockwise";
        if (fakeDirection == EEvilDirection.Counterclockwise)
            direction = "Counter-clockwise";

        string info = $"Closest Evil is:\n{direction}";
        if (fakeDirection == EEvilDirection.Either)
            info = "Closest Evil is equidistant";

        //List<Character> chs = GetMarkedCharacters(fakeDirection, charRef);
        charRef.CreateRuntimeData(new EnlightenedRuntimeData(fakeDirection));

        ActedInfo newInfo = new ActedInfo(info);
        return newInfo;
    }

    public enum EEvilDirection
    {
        Either = 0,
        Clockwise = 10,
        Counterclockwise = 20,
    }

    public EEvilDirection GetDirectionToEvil(Character charRef)
    {
        List<Character> clockwise = new List<Character>(Gameplay.CurrentCharacters);
        List<Character> counterclockwise = new List<Character>(Gameplay.CurrentCharacters);

        foreach (Character ch in Gameplay.CurrentCharacters)
        {
            counterclockwise.Remove(ch);
            if (charRef == ch)
            {
                counterclockwise.Remove(ch);
                break;
            }
        }
        foreach (Character ch in Gameplay.CurrentCharacters)
        {
            if (charRef == ch)
                break;

            counterclockwise.Add(ch);
        }

        clockwise = new List<Character>(counterclockwise);
        clockwise.Reverse();

        int clockwiseNumber = 0;
        int counterClockwiseNumber = 0;

        foreach (Character c in counterclockwise)
        {
            counterClockwiseNumber++;
            if (c.GetAlignment() == EAlignment.Evil)
                break;
        }
        foreach (Character c in clockwise)
        {
            clockwiseNumber++;
            if (c.GetAlignment() == EAlignment.Evil)
                break;
        }

        if (clockwiseNumber > counterClockwiseNumber)
            return EEvilDirection.Counterclockwise;
        if (clockwiseNumber < counterClockwiseNumber)
            return EEvilDirection.Clockwise;

        return EEvilDirection.Either;
    }

    public List<Character> GetMarkedCharacters(EEvilDirection direction, Character charRef)
    {
        List<Character> allCh = CharactersHelper.GetSortedListWithCharacterFirst(Gameplay.CurrentCharacters, charRef);
        List<Character> finalCh = new List<Character>();

        allCh.RemoveAt(0);

        int i = 0;
        List<int> halfPoints = new List<int>();
        int halfPoint = (allCh.Count + 1) / 2;
        halfPoints.Add(halfPoint);
        if ((allCh.Count + 1) % 2 == 0)
            halfPoints.Add(halfPoint);
        else
            halfPoints.Add(halfPoint - 1);

        foreach (Character c in allCh)
        {
            if (direction == EEvilDirection.Counterclockwise)
                if (i <= halfPoints[0])
                    finalCh.Add(c);
            //if (direction == EEvilDirection.Either)
            //if (halfPoints.Contains(i))
            //finalCh.Add(c);
            if (direction == EEvilDirection.Clockwise)
                if (i >= halfPoints[1])
                    finalCh.Add(c);
            i++;
        }

        return finalCh;
    }
}

[System.Serializable]
public class Tracker : Role // Hunter :
{
    public override string Description
        => "Learn how far from me is the nearest Evil";

    public override ActedInfo GetInfo(Character charRef)
    {
        int distance = GetDistanceToEvil(charRef);

        string info = $"";
        if (distance == 1)
            info = $"I am {distance} card away from closest Evil";
        else
            info = $"I am {distance} cards away from closest Evil";

        List<Character> chars = Characters.Instance.GetCharactersAtRange(distance, charRef);

        ActedInfo newInfo = new ActedInfo(info, chars);
        return newInfo;
    }

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        onActed?.Invoke(GetInfo(charRef));
    }
    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        onActed?.Invoke(GetBluffInfo(charRef));
    }
    public override ActedInfo GetBluffInfo(Character charRef)
    {
        int distance = GetDistanceToEvil(charRef);

        int possibleDistance = (int)((float)Gameplay.CurrentCharacters.Count / 2f);
        List<int> possibleDistances = new List<int>();
        for (int i = 0; i < possibleDistance; i++)
            possibleDistances.Add(i + 1);

        possibleDistances.Remove(distance);

        int randomDistance = possibleDistances[UnityEngine.Random.Range(0, possibleDistances.Count)];

        List<Character> chars = Characters.Instance.GetCharactersAtRange(randomDistance, charRef);

        string info = $"";
        if (randomDistance == 1)
            info = $"I am {randomDistance} card away from closest Evil";
        else
            info = $"I am {randomDistance} cards away from closest Evil";

        ActedInfo newInfo = new ActedInfo(info, chars);
        return newInfo;
    }

    public int GetDistanceToEvil(Character charRef)
    {
        List<Character> clockwise = new List<Character>(Gameplay.CurrentCharacters);
        List<Character> counterclockwise = new List<Character>(Gameplay.CurrentCharacters);

        foreach (Character ch in Gameplay.CurrentCharacters)
        {
            counterclockwise.Remove(ch);
            if (charRef == ch)
            {
                counterclockwise.Remove(ch);
                break;
            }
        }
        foreach (Character ch in Gameplay.CurrentCharacters)
        {
            if (charRef == ch)
                break;

            counterclockwise.Add(ch);
        }

        clockwise = new List<Character>(counterclockwise);
        clockwise.Reverse();

        int clockwiseNumber = 0;
        int counterClockwiseNumber = 0;

        foreach (Character c in counterclockwise)
        {
            counterClockwiseNumber++;
            if (c.GetAlignment() == EAlignment.Evil)
                break;
        }
        foreach (Character c in clockwise)
        {
            clockwiseNumber++;
            if (c.GetAlignment() == EAlignment.Evil)
                break;
        }

        if (clockwiseNumber > counterClockwiseNumber)
            return counterClockwiseNumber;
        if (clockwiseNumber < counterClockwiseNumber)
            return clockwiseNumber;

        return clockwiseNumber;
    }
}

[System.Serializable]
public class Investigator : Role // Oracle :
{
    public override string Description
        => "Learn that 1 of 2 players is a particular Minion";

    public override ActedInfo GetInfo(Character charRef)
    {
        List<Character> pickedCharacters = new List<Character>();

        List<Character> evils = new List<Character>(Gameplay.CurrentCharacters);
        evils = Characters.Instance.FilterCharacterType(evils, ECharacterType.Minion);

        ActedInfo newInfo = new ActedInfo($"There are no minions");

        if (evils.Count == 0)
            return newInfo;

        List<Character> goods = new List<Character>(Gameplay.CurrentCharacters);
        goods = Characters.Instance.FilterAlignmentCharacters(goods, EAlignment.Good);

        Character evil = evils[UnityEngine.Random.Range(0, evils.Count)];

        pickedCharacters.Add(evil);
        pickedCharacters.Add(goods[UnityEngine.Random.Range(0, goods.Count)]);

        pickedCharacters = pickedCharacters
            .OrderBy(c => c.id)
            .ThenBy(_ => UnityEngine.Random.value)
            .ToList();

        newInfo = new ActedInfo($"#{pickedCharacters[0].id} or #{pickedCharacters[1].id} is a {evil.GetCharacterData().name}", pickedCharacters);
        return newInfo;
    }

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        onActed?.Invoke(GetInfo(charRef));
    }
    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        onActed?.Invoke(GetBluffInfo(charRef));
    }
    public override ActedInfo GetBluffInfo(Character charRef)
    {
        List<Character> pickedCharacters = new List<Character>();

        List<Character> goods = new List<Character>(Gameplay.CurrentCharacters);
        goods = Characters.Instance.FilterAlignmentCharacters(goods, EAlignment.Good);

        Character cc = goods[UnityEngine.Random.Range(0, goods.Count)];
        pickedCharacters.Add(cc);
        goods.Remove(cc);
        pickedCharacters.Add(goods[UnityEngine.Random.Range(0, goods.Count)]);

        pickedCharacters = pickedCharacters
               .OrderBy(c => c.id)
               .ThenBy(_ => UnityEngine.Random.value)
               .ToList();

        List<CharacterData> minions = new List<CharacterData>(Gameplay.Instance.GetScriptCharacters());
        minions = Characters.Instance.FilterCharacterType(minions, ECharacterType.Minion);

        if (minions.Count == 0)
            minions = new List<CharacterData>(Gameplay.Instance.GetAllAscensionCharacters());
        minions = Characters.Instance.FilterCharacterType(minions, ECharacterType.Minion);

        CharacterData minion = minions[UnityEngine.Random.Range(0, minions.Count)];

        string info = $"#{pickedCharacters[0].id} or #{pickedCharacters[1].id} is a {minion.name}";
        ActedInfo newInfo = new ActedInfo(info, pickedCharacters);
        return newInfo;
    }
}
[System.Serializable]
public class Confessor : Role
{
    public override string Description
        => "Can not lie, even if I am Evil";

    public override ActedInfo GetInfo(Character charRef)
    {
        string info = "I am Good";

        if (charRef.statuses.statuses.Contains(ECharacterStatus.Corrupted) || charRef.statuses.statuses.Contains(ECharacterStatus.Mad))
            info = "I am dizzy";
        if (charRef.GetAlignment() == EAlignment.Evil)
            info = "I am dizzy";

        if (charRef.dataRef.role is Spy)
            info = "I am Good";

        ActedInfo newInfo = new ActedInfo(info);
        return newInfo;
    }

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        onActed?.Invoke(GetInfo(charRef));
    }
    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        onActed?.Invoke(GetBluffInfo(charRef));
    }
    public override ActedInfo GetBluffInfo(Character charRef)
    {
        string info = "I am Good";
        if (charRef.statuses.statuses.Contains(ECharacterStatus.Corrupted) || charRef.statuses.statuses.Contains(ECharacterStatus.Mad))
            info = "I am dizzy";
        if (charRef.GetAlignment() == EAlignment.Evil)
            info = "I am dizzy";

        if (charRef.dataRef.role is Spy)
            info = "I am Good";

        ActedInfo newInfo = new ActedInfo(info);
        return newInfo;
    }
}
[System.Serializable]
public class Acrobat : Role
{
    public override string Description
        => "Pick 1 character.\nLearn if he is Drunk";

    public override ActedInfo GetInfo(Character charRef)
    {
        return new ActedInfo("");
    }
    public override ActedInfo GetBluffInfo(Character charRef)
    {
        return new ActedInfo("");
    }

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        CharacterPicker.Instance.StartPickCharacters(1);
        CharacterPicker.OnCharactersPicked += CharacterPicked;
        CharacterPicker.OnStopPick += StopPick;
    }
    private void StopPick()
    {
        CharacterPicker.OnCharactersPicked -= CharacterPickedDrunk;
        CharacterPicker.OnCharactersPicked -= CharacterPicked;
        CharacterPicker.OnStopPick -= StopPick;
    }

    private void CharacterPicked()
    {
        CharacterPicker.OnCharactersPicked -= CharacterPicked;
        CharacterPicker.OnStopPick -= StopPick;

        bool isDrunk = false;
        foreach (Character c in CharacterPicker.PickedCharacters)
        {
            if (c.statuses.statuses.Contains(ECharacterStatus.Corrupted))
                isDrunk = true;
        }

        string info = "";
        if (isDrunk)
            info = $"#{CharacterPicker.PickedCharacters[0].id} is\nPoisoned";
        else
            info = $"#{CharacterPicker.PickedCharacters[0].id} is\nSober";

        onActed?.Invoke(new ActedInfo(info));
        Debug.Log($"{info}");
    }

    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        CharacterPicker.Instance.StartPickCharacters(1);
        CharacterPicker.OnCharactersPicked += CharacterPickedDrunk;
        CharacterPicker.OnStopPick += StopPick;
    }
    private void CharacterPickedDrunk()
    {
        CharacterPicker.OnCharactersPicked -= CharacterPickedDrunk;
        CharacterPicker.OnStopPick -= StopPick;

        bool isDrunk = false;
        foreach (Character c in CharacterPicker.PickedCharacters)
        {
            if (c.statuses.statuses.Contains(ECharacterStatus.Corrupted))
                isDrunk = false;
            if (c.dataRef.type == ECharacterType.Demon || c.dataRef.type == ECharacterType.Minion)
                isDrunk = true;
        }

        string info = "";
        if (isDrunk)
            info = $"#{CharacterPicker.PickedCharacters[0].id} is\nSober";
        else
            info = $"#{CharacterPicker.PickedCharacters[0].id} is\nPoisoned";

        onActed?.Invoke(new ActedInfo(info));
        Debug.Log($"{info}");
    }
}
[System.Serializable]
public class Acrobat2 : Role // Bard :
{
    public override string Description
        => "Learn how far I am from Poisoned character";

    public override ActedInfo GetInfo(Character charRef)
    {
        int howFar = GetClosestPoisonedCharacter(charRef);
        List<Character> chars = Characters.Instance.GetCharactersAtRange(howFar, charRef);

        string info = ConjourInfo(howFar);
        ActedInfo newInfo = new ActedInfo(info, chars);
        return newInfo;
    }

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        onActed?.Invoke(GetInfo(charRef));
    }

    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        onActed?.Invoke(GetBluffInfo(charRef));
    }

    public override ActedInfo GetBluffInfo(Character charRef)
    {
        int howFar = GetClosestPoisonedCharacter(charRef);

        int randomHowFar = Calculator.RemoveNumberAndGetRandomNumberFromList(howFar, 0, 4);
        List<Character> chars = Characters.Instance.GetCharactersAtRange(randomHowFar, charRef);

        string info = ConjourInfo(randomHowFar);

        ActedInfo newInfo = new ActedInfo(info, chars);
        return newInfo;
    }

    public int GetClosestPoisonedCharacter(Character charRef)
    {
        List<Character> myList = CharactersHelper.GetSortedListWithCharacterFirst(Gameplay.CurrentCharacters, charRef);

        myList.RemoveAt(0);
        int savedCount = 0;
        int count = 0;
        for (int i = 0; i < myList.Count; i++)
        {
            count++;
            if (myList[i].statuses.statuses.Contains(ECharacterStatus.Corrupted))
            {
                savedCount = count;
                count = 0;
                break;
            }
        }
        count = 0;
        for (int i = myList.Count - 1; i > 0; i--)
        {
            count++;
            if (myList[i].statuses.statuses.Contains(ECharacterStatus.Corrupted))
            {
                if (count < savedCount)
                {
                    savedCount = count;
                    count = 0;
                }
                break;
            }
        }

        return savedCount;
    }

    public string ConjourInfo(int howFar)
    {
        string info = "";
        if (howFar == 0)
            info = "There are no Corrupted characters";
        else if (howFar == 1)
            info = "I am 1 card away from Corrupted character";
        else
            info = $"I am {howFar} cards away from Corrupted character";

        return info;
    }
}

[System.Serializable]
public class Arbiter : Role
{
    public override string Description
        => "Pick 1 character.\nLearn if he is lying";

    public override ActedInfo GetInfo(Character charRef)
    {
        return new ActedInfo("");
    }
    public override ActedInfo GetBluffInfo(Character charRef)
    {
        return new ActedInfo("");
    }

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        CharacterPicker.Instance.StartPickCharacters(1);
        CharacterPicker.OnCharactersPicked += CharacterPicked;
        CharacterPicker.OnStopPick += StopPick;
    }
    private void StopPick()
    {
        CharacterPicker.OnCharactersPicked -= CharacterPickedDrunk;
        CharacterPicker.OnCharactersPicked -= CharacterPicked;
        CharacterPicker.OnStopPick -= StopPick;
    }

    private void CharacterPicked()
    {
        CharacterPicker.OnCharactersPicked -= CharacterPicked;
        CharacterPicker.OnStopPick -= StopPick;

        bool isLying = false;
        foreach (Character c in CharacterPicker.PickedCharacters)
        {
            if (c.bluff != null)
                isLying = true;

            //if (c.statuses.Contains(ECharacterStatus.Poisoned))
            //    isLying = true;

            //if (c.statuses.Contains(ECharacterStatus.HealthyBluff))
            //    isLying = false;
        }

        string info = $"{ConjourInfo(CharacterPicker.PickedCharacters[0], isLying)}";

        onActed?.Invoke(new ActedInfo(info));
        Debug.Log($"{info}");
    }

    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        CharacterPicker.Instance.StartPickCharacters(1);
        CharacterPicker.OnCharactersPicked += CharacterPickedDrunk;
        CharacterPicker.OnStopPick += StopPick;
    }
    private void CharacterPickedDrunk()
    {
        CharacterPicker.OnCharactersPicked -= CharacterPickedDrunk;
        CharacterPicker.OnStopPick -= StopPick;

        bool isLying = false;
        foreach (Character c in CharacterPicker.PickedCharacters)
        {
            if (c.bluff != null)
                isLying = true;
            //if (c.statuses.Contains(ECharacterStatus.HealthyBluff))
            //isLying = false;
        }

        isLying = !isLying;
        string info = $"{ConjourInfo(CharacterPicker.PickedCharacters[0], isLying)}";

        onActed?.Invoke(new ActedInfo(info));
        Debug.Log($"{info}");
    }

    public string ConjourInfo(Character character, bool isLying)
    {
        string info = $"";
        if (isLying)
            info = $"#{character.id} is\nBluffing";
        else
            info = $"#{character.id} is\nHonest";

        return info;
    }
}
[System.Serializable]
public class Judge2 : Role // Judge :
{
    public override string Description
        => "Pick 1 character.\nLearn if he is lying";

    public override ActedInfo GetInfo(Character charRef)
    {
        return new ActedInfo("");
    }
    public override ActedInfo GetBluffInfo(Character charRef)
    {
        return new ActedInfo("");
    }

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        CharacterPicker.Instance.StartPickCharacters(1);
        CharacterPicker.OnCharactersPicked += CharacterPicked;
        CharacterPicker.OnStopPick += StopPick;
    }
    private void StopPick()
    {
        CharacterPicker.OnCharactersPicked -= CharacterPickedDrunk;
        CharacterPicker.OnCharactersPicked -= CharacterPicked;
        CharacterPicker.OnStopPick -= StopPick;
    }

    private void CharacterPicked()
    {
        CharacterPicker.OnCharactersPicked -= CharacterPicked;
        CharacterPicker.OnStopPick -= StopPick;

        bool isLying = false;
        foreach (Character c in CharacterPicker.PickedCharacters)
        {
            if (c.bluff != null)
                isLying = true;

            if (c.statuses.Contains(ECharacterStatus.Corrupted))
                isLying = true;

            if (c.statuses.Contains(ECharacterStatus.HealthyBluff))
                isLying = false;

            if (c.dataRef.role is Confessor)
                isLying = false;
            if (c.bluff != null)
                if (c.bluff.role is Confessor)
                    isLying = false;
        }

        string info = $"{ConjourInfo(CharacterPicker.PickedCharacters[0], isLying)}";

        List<Character> chars = new List<Character>(CharacterPicker.PickedCharacters);
        onActed?.Invoke(new ActedInfo(info, chars));
        Debug.Log($"{info}");
    }

    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        CharacterPicker.Instance.StartPickCharacters(1);
        CharacterPicker.OnCharactersPicked += CharacterPickedDrunk;
        CharacterPicker.OnStopPick += StopPick;
    }
    private void CharacterPickedDrunk()
    {
        CharacterPicker.OnCharactersPicked -= CharacterPickedDrunk;
        CharacterPicker.OnStopPick -= StopPick;

        bool isLying = false;
        foreach (Character c in CharacterPicker.PickedCharacters)
        {
            if (c.bluff != null)
                isLying = true;


            if (c.statuses.Contains(ECharacterStatus.Corrupted))
                isLying = true;

            if (c.statuses.Contains(ECharacterStatus.HealthyBluff))
                isLying = false;

            if (c.dataRef.role is Confessor)
                isLying = false;
            if (c.bluff != null)
                if (c.bluff.role is Confessor)
                    isLying = false;
        }

        isLying = !isLying;
        string info = $"{ConjourInfo(CharacterPicker.PickedCharacters[0], isLying)}";

        List<Character> chars = new List<Character>(CharacterPicker.PickedCharacters);
        onActed?.Invoke(new ActedInfo(info, chars));
        Debug.Log($"{info}");
    }

    public string ConjourInfo(Character character, bool isLying)
    {
        string info = $"";

        if (isLying)
            info = $"#{character.id} is\nLying";
        else
            info = $"#{character.id} is\nsaying Truth";

        return info;
    }
}

[System.Serializable]
public class Librarian : Role // Druid :
{
    public override string Description
        => "Pick 2 players. Learn which Outsider is among them (if any)";

    public override ActedInfo GetInfo(Character charRef)
    {
        return new ActedInfo("");
    }
    public override ActedInfo GetBluffInfo(Character charRef)
    {
        return new ActedInfo("");
    }

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        CharacterPicker.Instance.StartPickCharacters(3);
        CharacterPicker.OnCharactersPicked += CharacterPicked;
        CharacterPicker.OnStopPick += StopPick;
    }
    private void StopPick()
    {
        CharacterPicker.OnCharactersPicked -= CharacterPicked;
        CharacterPicker.OnCharactersPicked -= CharacterPickedDrunk;
        CharacterPicker.OnStopPick -= StopPick;
    }

    private void CharacterPicked()
    {
        CharacterPicker.OnCharactersPicked -= CharacterPicked;
        CharacterPicker.OnStopPick -= StopPick;

        List<Character> outsiders = new List<Character>();
        List<int> ids = new List<int>();
        foreach (Character c in CharacterPicker.PickedCharacters)
        {
            ids.Add(c.id);
            if (c.GetRegisterAs().type == ECharacterType.Outcast)
                outsiders.Add(c);
        }

        ids = ids
            .OrderBy(c => c)
            .ThenBy(_ => UnityEngine.Random.value)
            .ToList();

        string info = $"Among #{ids[0]}, #{ids[1]}, #{ids[2]}\nthere are NO Outcasts";

        if (outsiders.Count > 0)
        {
            Character randomOutsider = outsiders[UnityEngine.Random.Range(0, outsiders.Count)];

            info = $"Among #{ids[0]}, #{ids[1]}, #{ids[2]}\nthere is: {randomOutsider.GetCharacterData().name}";
        }

        List<Character> chars = new List<Character>(CharacterPicker.PickedCharacters);
        onActed?.Invoke(new ActedInfo(info, chars));
        Debug.Log($"{info}");
    }

    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        CharacterPicker.Instance.StartPickCharacters(3);
        CharacterPicker.OnCharactersPicked += CharacterPickedDrunk;
        CharacterPicker.OnStopPick += StopPick;
    }
    private void CharacterPickedDrunk()
    {
        CharacterPicker.OnCharactersPicked -= CharacterPickedDrunk;
        CharacterPicker.OnStopPick -= StopPick;

        List<Character> outsiders = new List<Character>();
        List<int> ids = new List<int>();
        foreach (Character c in CharacterPicker.PickedCharacters)
        {
            ids.Add(c.id);
            if (c.GetRegisterAs().type == ECharacterType.Outcast)
                outsiders.Add(c);
        }

        ids = ids
            .OrderBy(c => c)
            .ThenBy(_ => UnityEngine.Random.value)
            .ToList();

        string info = $"";

        if (outsiders.Count > 0)
            info = $"Among #{ids[0]}, #{ids[1]}, #{ids[2]}\nthere are NO Outcasts";
        else
        {
            List<CharacterData> scriptOutsiders = new List<CharacterData>(Gameplay.Instance.GetScriptCharacters());
            List<CharacterData> pickedOutsiders = new List<CharacterData>();
            scriptOutsiders = Characters.Instance.FilterCharacterType(scriptOutsiders, ECharacterType.Outcast);

            foreach (CharacterData c in scriptOutsiders)
                if (!c.bluffable)
                    pickedOutsiders.Add(c);

            if (pickedOutsiders.Count == 0)
            {
                scriptOutsiders = new List<CharacterData>(Gameplay.Instance.GetAllAscensionCharacters());
                scriptOutsiders = Characters.Instance.FilterCharacterType(scriptOutsiders, ECharacterType.Outcast);

                foreach (CharacterData c in scriptOutsiders)
                    if (!c.bluffable)
                        pickedOutsiders.Add(c);

                if (pickedOutsiders.Count == 0)
                {
                    foreach (CharacterData c in scriptOutsiders)
                        pickedOutsiders.Add(c);
                }
            }

            if (pickedOutsiders.Count == 0)
            {
                info = $"Among #{ids[0]}, #{ids[1]}, #{ids[2]}\nthere is: Drunk";
            }
            else
            {
                CharacterData randomOutsider = pickedOutsiders[UnityEngine.Random.Range(0, pickedOutsiders.Count)];

                info = $"Among #{ids[0]}, #{ids[1]}, #{ids[2]}\nthere is: {randomOutsider.name}";
            }
        }

        List<Character> chars = new List<Character>(CharacterPicker.PickedCharacters);

        onActed?.Invoke(new ActedInfo(info, chars));
        Debug.Log($"{info}");
    }
}
[System.Serializable]
public class Juggler : Role // Jester :
{
    public override string Description
        => "Pick 4 players. Learn how many of them are Evil";

    public override ActedInfo GetInfo(Character charRef)
    {
        return new ActedInfo("");
    }
    public override ActedInfo GetBluffInfo(Character charRef)
    {
        return new ActedInfo("");
    }

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        CharacterPicker.Instance.StartPickCharacters(3);
        CharacterPicker.OnCharactersPicked += CharacterPicked;
        CharacterPicker.OnStopPick += StopPick;
    }
    private void StopPick()
    {
        CharacterPicker.OnCharactersPicked -= CharacterPickedDrunk;
        CharacterPicker.OnCharactersPicked -= CharacterPicked;
        CharacterPicker.OnStopPick -= StopPick;
    }

    private void CharacterPicked()
    {
        CharacterPicker.OnCharactersPicked -= CharacterPicked;
        CharacterPicker.OnStopPick -= StopPick;

        int evils = 0;
        List<int> ids = new List<int>();
        foreach (Character c in CharacterPicker.PickedCharacters)
        {
            if (c.GetAlignment() == EAlignment.Evil)
                evils++;

            ids.Add(c.id);
        }

        ids = ids
            .OrderBy(c => c)
            .ThenBy(_ => UnityEngine.Random.value)
            .ToList();

        string info = $"Among:\n#{ids[0]}, #{ids[1]}, #{ids[2]}:\nThere are {evils} Evils";
        if (evils == 1)
            info = $"Among:\n#{ids[0]}, #{ids[1]}, #{ids[2]}:\nThere is {evils} Evil";

        List<Character> chars = new List<Character>(CharacterPicker.PickedCharacters);

        onActed?.Invoke(new ActedInfo(info, chars));
        Debug.Log($"{info}");
    }

    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        CharacterPicker.Instance.StartPickCharacters(3);
        CharacterPicker.OnCharactersPicked += CharacterPickedDrunk;
        CharacterPicker.OnStopPick += StopPick;
    }
    private void CharacterPickedDrunk()
    {
        CharacterPicker.OnCharactersPicked -= CharacterPickedDrunk;
        CharacterPicker.OnStopPick -= StopPick;

        int evils = 0;
        List<int> ids = new List<int>();
        foreach (Character c in CharacterPicker.PickedCharacters)
        {
            if (c.GetAlignment() == EAlignment.Evil)
                evils++;

            ids.Add(c.id);
        }

        ids = ids
            .OrderBy(c => c)
            .ThenBy(_ => UnityEngine.Random.value)
            .ToList();

        int townsfolks = 0;
        townsfolks = Calculator.RemoveNumberAndGetRandomNumberFromList(evils, 0, 4);

        string info = $"Among:\n#{ids[0]}, #{ids[1]}, #{ids[2]}:\nThere are {townsfolks} Evils";
        if (townsfolks == 1)
            info = $"Among:\n#{ids[0]}, #{ids[1]}, #{ids[2]}:\nThere is {townsfolks} Evil";

        List<Character> chars = new List<Character>(CharacterPicker.PickedCharacters);

        onActed?.Invoke(new ActedInfo(info, chars));
        Debug.Log($"{info}");
    }
}

//Outsiders
public class Mutant : Role
{
    public override string Description
    => "";

    public override ActedInfo GetInfo(Character charRef)
    {
        return new ActedInfo("");
    }
    public override ActedInfo GetBluffInfo(Character charRef)
    {
        return new ActedInfo("");
    }
    public override void Act(ETriggerPhase trigger, Character charRef)
    {
    }
    public override CharacterData GetBluffIfAble(Character charRef)
    {
        List<CharacterData> notInPlayCh = Gameplay.Instance.GetScriptCharacters();
        notInPlayCh = Characters.Instance.FilterAlignmentCharacters(notInPlayCh, EAlignment.Good);
        notInPlayCh = Characters.Instance.FilterBluffableCharacters(notInPlayCh);

        charRef.statuses.AddStatus(ECharacterStatus.Mad, charRef);

        return notInPlayCh[UnityEngine.Random.Range(0, notInPlayCh.Count - 1)];
    }
}

public class Drunk : Role
{
    public override string Description
    => "";

    public override ActedInfo GetInfo(Character charRef)
    {
        return new ActedInfo("");
    }
    public override ActedInfo GetBluffInfo(Character charRef)
    {
        return new ActedInfo("");
    }
    public override void Act(ETriggerPhase trigger, Character charRef)
    {
    }
    public override bool CheckIfCanRemoveStatus(ECharacterStatus status)
    {
        if (status == ECharacterStatus.Corrupted)
            return false;

        return true;
    }
    public override int GetDamageToYou()
    {
        return 2;
    }
    public override CharacterData GetBluffIfAble(Character charRef)
    {
        charRef.statuses.AddStatus(ECharacterStatus.Corrupted, charRef, charRef);
        CharacterData bluff = Characters.Instance.GetRandomUniqueVillagerBluff();
        Gameplay.Instance.AddScriptCharacterIfAble(bluff.type, bluff);

        return bluff;
    }
}

public class Recluse : Role // Wretch :
{
    public string evilId = "Minion_71804875";

    [TextArea(3, 5)]
    public List<string> wretchChats = new List<string>()
    {
        "I am not Evil!\nFor sure",
    };

    public override string Description
    => "Can register as a Demon";

    public override ActedInfo GetInfo(Character charRef)
    {
        return new ActedInfo("");
    }
    public override ActedInfo GetBluffInfo(Character charRef)
    {
        return new ActedInfo("");
    }
    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        //if (trigger != ETriggerPhase.Day) ;
        //string randomChat = wretchChats[UnityEngine.Random.Range(0, wretchChats.Count)];
        //onActed?.Invoke(new ActedInfo(randomChat));
    }
    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
    }
    public override CharacterData GetRegisterAsRole(Character charRef)
    {
        List<CharacterData> allChars = new List<CharacterData>(Gameplay.Instance.GetScriptCharacters());
        allChars = Characters.Instance.FilterCharacterType(allChars, ECharacterType.Minion);

        if (allChars.Count == 0)
            allChars = new List<CharacterData>(ProjectContext.Instance.gameData.GetStartingtCharactersOfType(ECharacterType.Minion));

        CharacterData randomMinion = allChars[UnityEngine.Random.Range(0, allChars.Count)];

        //CharacterData minionData = ProjectContext.Instance.gameData.GetCharacterDataOfId(evilId);

        return randomMinion;
    }
}

public class Saint : Role // Bombardier
{
    public override string Description
    => "Lose if you Kill me";

    public override ActedInfo GetInfo(Character charRef)
    {
        return new ActedInfo("");
    }
    public override ActedInfo GetBluffInfo(Character charRef)
    {
        return new ActedInfo("");
    }
    public override void Act(ETriggerPhase trigger, Character charRef)
    {
    }
}

public class SaintVillager : Role
{
    public override string Description
    => "I am always good";

    public override ActedInfo GetInfo(Character charRef)
    {
        return new ActedInfo("");
    }
    public override ActedInfo GetBluffInfo(Character charRef)
    {
        return new ActedInfo("");
    }
    public override void Act(ETriggerPhase trigger, Character charRef)
    {
    }
}

[System.Serializable]
public class Doppleganger : Role
{
    public override string Description
        => "[I am a Good Villager role currently in play]";

    public override ActedInfo GetInfo(Character charRef)
    {
        return new ActedInfo("");
    }

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger == ETriggerPhase.OnDied)
        {
        }
    }

    public override CharacterData GetBluffIfAble(Character charRef)
    {
        if (!charRef.statuses.Contains(ECharacterStatus.Corrupted))
        {
            charRef.statuses.AddStatus(ECharacterStatus.HealthyBluff, charRef);
            List<Character> characters = new List<Character>(Gameplay.CurrentCharacters);
            characters = Characters.Instance.FilterBluffableCharacters(characters);
            characters = Characters.Instance.FilterCharacterType(characters, ECharacterType.Villager);
            characters = Characters.Instance.FilterAlignmentCharacters(characters, EAlignment.Good);
            CharacterData character = characters[UnityEngine.Random.Range(0, characters.Count)].dataRef;

            return character;
        }
        else
        {
            List<Character> characters = new List<Character>(Gameplay.CurrentCharacters);
            characters = Characters.Instance.FilterBluffableCharacters(characters);
            characters = Characters.Instance.FilterAlignmentCharacters(characters, EAlignment.Evil);

            return characters[UnityEngine.Random.Range(0, characters.Count)].GetCharacterBluffIfAble();
        }
    }

    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger == ETriggerPhase.Start)
        {
        }
    }

    public override ActedInfo GetBluffInfo(Character charRef)
    {
        return new ActedInfo("");
    }

    public string ConjourInfo()
    {
        return "";
    }

    public override bool CheckIfCanBeKilled(Character charRef)
    {
        if (charRef.statuses.statuses.Contains(ECharacterStatus.HealthyBluff))
            return charRef.bluff.role.CheckIfCanBeKilled(charRef);
        else
            return true;
    }
}
[System.Serializable]
public class Puzzlemaster : Role // PlagueDoctor :
{
    public override string Description
        => "[1 Villager is Poisoned] Pick 1 character: if its Poisoned I learn an Evil character.";

    Character self;

    public override ActedInfo GetInfo(Character charRef)
    {
        return new ActedInfo("");
    }
    public override ActedInfo GetBluffInfo(Character charRef)
    {
        return new ActedInfo("");
    }

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger == ETriggerPhase.Day)
        {
            self = charRef;
            CharacterPicker.Instance.StartPickCharacters(1);
            CharacterPicker.OnCharactersPicked += CharacterPicked;
            CharacterPicker.OnStopPick += StopPick;
        }
        if (trigger == ETriggerPhase.Start)
        {
            PoisonRandomVillager(charRef);
        }
    }

    private void PoisonRandomVillager(Character charRef)
    {
        List<Character> villagers = new List<Character>(Gameplay.CurrentCharacters);
        villagers = Characters.Instance.FilterCharacterType(villagers, ECharacterType.Villager);
        villagers = Characters.Instance.FilterCharacterMissingStatus(villagers, ECharacterStatus.Corrupted);

        if (villagers.Count <= 0) return;

        Character randomCharacter = villagers[UnityEngine.Random.Range(0, villagers.Count)];
        randomCharacter.statuses.AddStatus(ECharacterStatus.Corrupted, charRef);
    }

    private void StopPick()
    {
        CharacterPicker.OnCharactersPicked -= CharacterPickedDrunk;
        CharacterPicker.OnCharactersPicked -= CharacterPicked;
        CharacterPicker.OnStopPick -= StopPick;
    }

    private void CharacterPicked()
    {
        CharacterPicker.OnCharactersPicked -= CharacterPicked;
        CharacterPicker.OnStopPick -= StopPick;

        bool isPoisoned = false;
        if (CharacterPicker.PickedCharacters[0].statuses.Contains(ECharacterStatus.Corrupted))
            isPoisoned = true;

        Character randomEvilCharacter = null;
        if (isPoisoned)
        {
            List<Character> evilCharacters = new List<Character>(Gameplay.CurrentCharacters);
            evilCharacters = Characters.Instance.FilterAlignmentCharacters(evilCharacters, EAlignment.Evil);
            randomEvilCharacter = evilCharacters[UnityEngine.Random.Range(0, evilCharacters.Count)];
        }

        string info = $"{ConjourInfo(randomEvilCharacter, CharacterPicker.PickedCharacters[0])}";

        List<Character> pickeds = new List<Character>();
        pickeds.AddRange(CharacterPicker.PickedCharacters);
        pickeds.Add(randomEvilCharacter);

        onActed?.Invoke(new ActedInfo(info, pickeds));
        Debug.Log($"{info}");
    }

    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;

        self = charRef;
        CharacterPicker.Instance.StartPickCharacters(1);
        CharacterPicker.OnCharactersPicked += CharacterPickedDrunk;
        CharacterPicker.OnStopPick += StopPick;
    }
    private void CharacterPickedDrunk()
    {
        CharacterPicker.OnCharactersPicked -= CharacterPickedDrunk;
        CharacterPicker.OnStopPick -= StopPick;

        bool isPoisoned = false;
        if (!CharacterPicker.PickedCharacters[0].statuses.Contains(ECharacterStatus.Corrupted))
            isPoisoned = true;

        Character randomEvilCharacter = null;
        if (isPoisoned)
        {
            List<Character> evilCharacters = new List<Character>(Gameplay.CurrentCharacters);
            evilCharacters = Characters.Instance.FilterAlignmentCharacters(evilCharacters, EAlignment.Good);
            randomEvilCharacter = evilCharacters[UnityEngine.Random.Range(0, evilCharacters.Count)];
        }

        string info = $"{ConjourInfo(randomEvilCharacter, CharacterPicker.PickedCharacters[0])}";

        List<Character> pickeds = new List<Character>();
        pickeds.AddRange(CharacterPicker.PickedCharacters);
        pickeds.Add(randomEvilCharacter);

        onActed?.Invoke(new ActedInfo(info, pickeds));
        Debug.Log($"{info}");
    }

    public string ConjourInfo(Character evilCharacter, Character pickedCharacter)
    {
        string info = $"";

        if (pickedCharacter == self)
            return $"#{pickedCharacter.id} is\nNot Corrupted";

        if (evilCharacter == null)
            info = $"#{pickedCharacter.id} is\nNot Corrupted";
        else
            info = $"#{evilCharacter.id} is Evil\n#{pickedCharacter.id} is Corrupted";
        return info;
    }
}

//Minions
public class Minion : Role
{
    public override string Description
    => "";

    public override ActedInfo GetInfo(Character charRef)
    {
        return new ActedInfo("");
    }
    public override ActedInfo GetBluffInfo(Character charRef)
    {
        return new ActedInfo("");
    }
    public override void Act(ETriggerPhase trigger, Character charRef)
    {
    }
    public override CharacterData GetBluffIfAble(Character charRef)
    {
        int diceRoll = Calculator.RollDice(10);

        //List<CharacterData> notInPlayCh = Gameplay.Instance.GetScriptCharacters();
        //notInPlayCh = Characters.Instance.FilterAlignmentCharacters(notInPlayCh, EAlignment.Good);
        //notInPlayCh = Characters.Instance.FilterBluffableCharacters(notInPlayCh);
        //return notInPlayCh[UnityEngine.Random.Range(0, notInPlayCh.Count - 1)];

        if (diceRoll < 5)
        {
            // 100% Double Claim
            return Characters.Instance.GetRandomDuplicateBluff();
        }
        else
        {
            // Become a new character
            CharacterData bluff = Characters.Instance.GetRandomUniqueBluff();
            Gameplay.Instance.AddScriptCharacterIfAble(bluff.type, bluff);

            return bluff;
        }
    }
}
[System.Serializable]
public class Spy : Minion
{
    public override string Description
    => "Can register as a Good Townsfolk. Demon will kill best targets.";

    public CharacterData chData;

    public override ActedInfo GetInfo(Character charRef)
    {
        return new ActedInfo("");
    }
    public override void Act(ETriggerPhase trigger, Character charRef)
    {
    }
    public override CharacterData GetBluffIfAble(Character charRef)
    {
        if (charRef.bluff != null) return charRef.registerAs;

        if (chData != null)
            return chData;

        List<CharacterData> notInPlayCh = Gameplay.Instance.GetScriptCharacters();
        notInPlayCh = Characters.Instance.FilterCharacterType(notInPlayCh, ECharacterType.Villager);
        chData = notInPlayCh[UnityEngine.Random.Range(0, notInPlayCh.Count - 1)];
        return chData;
    }
    public override CharacterData GetRegisterAsRole(Character charRef)
    {
        if (chData != null)
            return chData;

        List<CharacterData> notInPlayCh = Gameplay.Instance.GetScriptCharacters();
        notInPlayCh = Characters.Instance.FilterCharacterType(notInPlayCh, ECharacterType.Villager);

        chData = notInPlayCh[UnityEngine.Random.Range(0, notInPlayCh.Count - 1)];

        return chData;
    }
}
[System.Serializable]
public class Poisoner : Minion
{
    public override string Description
    => "1 adjacent good character is Poisoned";

    public override ActedInfo GetInfo(Character charRef)
    {
        return new ActedInfo("");
    }
    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Start) return;

        List<Character> viableCharacters = Characters.Instance.GetAdjacentCharacters(charRef);
        viableCharacters = Characters.Instance.FilterRealCharacterType(viableCharacters, ECharacterType.Villager);
        viableCharacters = Characters.Instance.FilterCharacterMissingStatus(viableCharacters, ECharacterStatus.Corrupted);

        if (viableCharacters.Count == 0) return;

        int randomId = UnityEngine.Random.Range(0, viableCharacters.Count);
        Character pickedCharacter = viableCharacters[randomId];

        pickedCharacter.statuses.AddStatus(ECharacterStatus.Corrupted, charRef);
        pickedCharacter.statuses.AddStatus(ECharacterStatus.MessedUpByEvil, charRef);

    }
}
[System.Serializable]
public class Baron : Minion // Conselour : Counsellor :
{
    public override string Description
    => "Add 1 outsider if able. Sits next to an Outsider";

    public override ActedInfo GetInfo(Character charRef)
    {
        return new ActedInfo("");
    }
    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Start) return;

        List<Character> viableCharacters = new List<Character>(Gameplay.CurrentCharacters);

        List<CharacterData> notInPlayOutsiders = Gameplay.Instance.GetAscensionAllStartingCharacters();
        notInPlayOutsiders = Characters.Instance.FilterNotInDeckCharactersUnique(notInPlayOutsiders);
        notInPlayOutsiders = Characters.Instance.FilterRealCharacterType(notInPlayOutsiders, ECharacterType.Outcast);
        if (notInPlayOutsiders.Count == 0)
        {
            notInPlayOutsiders = Gameplay.Instance.GetAllAscensionCharacters();
            notInPlayOutsiders = Characters.Instance.FilterRealCharacterType(notInPlayOutsiders, ECharacterType.Outcast);
        }
        CharacterData pickedOutsider = notInPlayOutsiders[UnityEngine.Random.Range(0, notInPlayOutsiders.Count - 1)];

        if (notInPlayOutsiders.Count != 0)
        {
            Gameplay.Instance.AddScriptCharacter(ECharacterType.Outcast, pickedOutsider);

            viableCharacters = Characters.Instance.FilterAliveCharacters(viableCharacters);
            viableCharacters = Characters.Instance.FilterRealCharacterType(viableCharacters, ECharacterType.Villager);

            Character pickedCharacter = viableCharacters[UnityEngine.Random.Range(0, viableCharacters.Count)];
            pickedCharacter.Init(pickedOutsider);
            viableCharacters.Remove(pickedCharacter);
            notInPlayOutsiders.Remove(pickedOutsider);
        }

        SitNextToOutsider(charRef);
    }

    private void SitNextToOutsider(Character charRef)
    {
        List<Character> outsiders = new List<Character>(Gameplay.CurrentCharacters);
        outsiders = Characters.Instance.FilterCharacterType(outsiders, ECharacterType.Outcast);

        Character pickedOutsider = outsiders[UnityEngine.Random.Range(0, outsiders.Count)];
        pickedOutsider.statuses.AddStatus(ECharacterStatus.MessedUpByEvil, charRef);

        List<Character> adjacentCharacters = Characters.Instance.GetAdjacentAliveCharacters(pickedOutsider);
        Character pickedSwapCharacter = adjacentCharacters[UnityEngine.Random.Range(0, adjacentCharacters.Count)];
        CharacterData pickedData = pickedSwapCharacter.dataRef;
        pickedSwapCharacter.Init(charRef.dataRef);
        charRef.Init(pickedData);
    }
}
[System.Serializable]
public class Marionette : Minion
{
    public override string Description
    => "[I sit next to a Demon]";

    public override ActedInfo GetInfo(Character charRef)
    {
        return new ActedInfo("");
    }
    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Start) return;

        SitNextToDemon(charRef);
    }

    private void SitNextToDemon(Character charRef)
    {
        List<Character> demons = new List<Character>(Gameplay.CurrentCharacters);
        demons = Characters.Instance.FilterCharacterType(demons, ECharacterType.Demon);

        if (demons.Count <= 0) return;

        Character pickedDemon = demons[UnityEngine.Random.Range(0, demons.Count)];
        List<Character> adjacentCharacters = Characters.Instance.GetAdjacentAliveCharacters(pickedDemon);
        Character pickedAdjacentCharacter = adjacentCharacters[UnityEngine.Random.Range(0, adjacentCharacters.Count)];
        CharacterData pickedData = pickedAdjacentCharacter.dataRef;
        pickedAdjacentCharacter.InitWithNoReset(charRef.dataRef);
        charRef.InitWithNoReset(pickedData);
    }
}
[System.Serializable]
public class Illuzionist : Minion // Shaman :
{
    public override string Description
    => "[There are 2 Villagers of the same Roles]";

    public override ActedInfo GetInfo(Character charRef)
    {
        return new ActedInfo("");
    }
    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Start) return;

        List<Character> villagers = new List<Character>(Gameplay.CurrentCharacters);
        villagers = Characters.Instance.FilterCharacterType(villagers, ECharacterType.Villager);

        Character pickedVillager = villagers[UnityEngine.Random.Range(0, villagers.Count)];
        pickedVillager.statuses.AddStatus(ECharacterStatus.MessedUpByEvil, charRef);

        villagers.Remove(pickedVillager);
        Character replacedVillager = villagers[UnityEngine.Random.Range(0, villagers.Count)];


        replacedVillager.InitWithNoReset(pickedVillager.GetCharacterBluffIfAble());
        replacedVillager.Act(ETriggerPhase.Start);
        replacedVillager.statuses.AddStatus(ECharacterStatus.MessedUpByEvil, charRef);
    }
}
public class Puppet : Minion
{
    public override string Description
    => "";

    public override ActedInfo GetInfo(Character charRef)
    {
        return new ActedInfo("");
    }
    public override ActedInfo GetBluffInfo(Character charRef)
    {
        return new ActedInfo("");
    }
    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger == ETriggerPhase.Start)
        {
            ApplyStatuses(charRef);
        }
    }

    private void ApplyStatuses(Character charRef)
    {
        charRef.statuses.AddStatus(ECharacterStatus.HealthyBluff, charRef);
        charRef.statuses.AddStatus(ECharacterStatus.BrokenAbility, charRef);
        charRef.statuses.AddStatus(ECharacterStatus.MessedUpByEvil, charRef);
    }

    public override CharacterData GetBluffIfAble(Character charRef)
    {
        List<CharacterData> notInPlayCh = Gameplay.Instance.GetScriptCharacters();
        notInPlayCh = Characters.Instance.FilterAlignmentCharacters(notInPlayCh, EAlignment.Good);
        notInPlayCh = Characters.Instance.FilterBluffableCharacters(notInPlayCh);
        return notInPlayCh[UnityEngine.Random.Range(0, notInPlayCh.Count - 1)];
    }
}
[System.Serializable]
public class Mezepheles : Minion // Puppeteer
{
    public string minionId = "Puppet_15989619";
    public override string Description
    => "[1 adjacent Good character becomes Evil]";

    public override ActedInfo GetInfo(Character charRef)
    {
        return new ActedInfo("");
    }
    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Start) return;

        List<Character> sortedCharacters = new List<Character>(Gameplay.CurrentCharacters);
        sortedCharacters = CharactersHelper.GetSortedListWithCharacterFirst(sortedCharacters, charRef);

        sortedCharacters.RemoveAt(0);
        List<Character> adjacentGoodCharacters = new List<Character>();
        if (sortedCharacters[0].dataRef.type == ECharacterType.Villager)
            adjacentGoodCharacters.Add(sortedCharacters[0]);
        if (sortedCharacters[sortedCharacters.Count - 1].dataRef.type == ECharacterType.Villager)
            adjacentGoodCharacters.Add(sortedCharacters[sortedCharacters.Count - 1]);

        foreach (Character c in adjacentGoodCharacters)
            if (c.dataRef.role is SaintVillager)
            {
                adjacentGoodCharacters.Remove(c);
                break;
            }

        if (adjacentGoodCharacters.Count <= 0) return;

        Character randomCharacter = adjacentGoodCharacters[UnityEngine.Random.Range(0, adjacentGoodCharacters.Count)];

        CharacterData bluff = randomCharacter.dataRef;
        CharacterData minionData = ProjectContext.Instance.gameData.GetCharacterDataOfId(minionId);
        randomCharacter.Init(minionData);
        randomCharacter.GiveBluff(bluff);
        randomCharacter.statuses.AddStatus(ECharacterStatus.HealthyBluff, charRef);
        randomCharacter.statuses.AddStatus(ECharacterStatus.BrokenAbility, charRef);
        randomCharacter.statuses.AddStatus(ECharacterStatus.AlteredCharacter, charRef);
        randomCharacter.statuses.AddStatus(ECharacterStatus.MessedUpByEvil, charRef);

    }
}

[System.Serializable]
public class Cipher : Minion // Witch :
{
    public override string Description
    => "You can reveal 1 less card";

    public override ActedInfo GetInfo(Character charRef)
    {
        return new ActedInfo("");
    }
    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Start) return;
        PlayerController.PlayerInfo.blocks.value.Add(1);
    }
    public override void ActOnDied(Character charRef)
    {
        PlayerController.PlayerInfo.blocks.value.Reduce(1);
    }
}

//Demons
[System.Serializable]
public class Demon : Role
{
    public override string Description
    => "";

    public override ActedInfo GetInfo(Character charRef)
    {
        return new ActedInfo("");
    }
    public override ActedInfo GetBluffInfo(Character charRef)
    {
        return new ActedInfo("");
    }
    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Night) return;
        //Kill();
    }
    public void KillHidden(Character demonRef)
    {
        List<Character> possibleCharacters = new List<Character>();
        possibleCharacters = Characters.Instance.FilterAliveCharacters(Gameplay.CurrentCharacters);
        possibleCharacters = Characters.Instance.FilterAlignmentCharacters(possibleCharacters, EAlignment.Good);
        possibleCharacters = Characters.Instance.FilterHiddenCharacters(possibleCharacters);
        possibleCharacters = Characters.Instance.FilterCharacterMissingStatus(possibleCharacters, ECharacterStatus.UnkillableByDemon);
        if (possibleCharacters.Count <= 0) { KillRandom(demonRef); return; }
        Characters.Instance.GetRandomAliveCharacter(possibleCharacters).KillByDemon(demonRef);
    }
    public void KillRandom(Character demonRef)
    {
        List<Character> possibleCharacters = new List<Character>();
        possibleCharacters = Characters.Instance.FilterAliveCharacters(Gameplay.CurrentCharacters);
        //possibleCharacters = Characters.Instance.FilterAlignmentCharacters(possibleCharacters, EAlignment.Good);
        possibleCharacters = Characters.Instance.FilterHiddenCharacters(possibleCharacters);
        possibleCharacters = Characters.Instance.FilterCharacterMissingStatus(possibleCharacters, ECharacterStatus.UnkillableByDemon);
        if (possibleCharacters.Count == 0) { return; }
        Characters.Instance.GetRandomAliveCharacter(possibleCharacters).KillByDemon(demonRef);
    }
    public override CharacterData GetBluffIfAble(Character charRef)
    {
        CharacterData bluff = Characters.Instance.GetRandomUniqueVillagerBluff();
        Gameplay.Instance.AddScriptCharacterIfAble(bluff.type, bluff);

        return bluff;
    }
}

[System.Serializable]
public class Imp : Demon // Baa :
{
    public override List<SpecialRule> GetRules() => new List<SpecialRule>()
    {
        //new NightModeRule(4),
    };

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger == ETriggerPhase.Start)
        {
            List<CharacterData> notInPlayCh = Gameplay.Instance.GetAscensionAllStartingCharacters();
            notInPlayCh = Characters.Instance.FilterNotInDeckCharactersUnique(notInPlayCh);
            notInPlayCh = Characters.Instance.FilterCharacterType(notInPlayCh, ECharacterType.Outcast);

            CharacterData data = null;
            if (notInPlayCh.Count == 0)
            {
                notInPlayCh = Gameplay.Instance.GetAllAscensionCharacters();
                notInPlayCh = Characters.Instance.FilterCharacterType(notInPlayCh, ECharacterType.Outcast);
            }

            data = notInPlayCh[UnityEngine.Random.Range(0, notInPlayCh.Count - 1)];

            Gameplay.Instance.AddScriptCharacter(data.type, data);
        }
        if (trigger != ETriggerPhase.Night) return;
        //KillHidden();
    }
}
[System.Serializable]
public class Skinwalker : Demon
{
    public override List<SpecialRule> GetRules() => new List<SpecialRule>()
    {
        //new NightModeRule(4),
    };

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Night) return;
        //KillHidden();
    }
}
[System.Serializable]
public class Pooka : Demon
{
    public override List<SpecialRule> GetRules() => new List<SpecialRule>()
    {
        //new NightModeRule(4),
    };

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger == ETriggerPhase.Start)
        {
            PoisonNeighboursIfAble(charRef);
        }
        if (trigger != ETriggerPhase.Night) return;
        //KillHidden();
    }

    private void PoisonClosestNeighbours(Character charRef)
    {
        List<Character> viableCharacters = viableCharacters = Characters.Instance.GetAdjacentCharacters(charRef);
        viableCharacters = Characters.Instance.FilterRealCharacterType(viableCharacters, ECharacterType.Villager);

        if (viableCharacters.Count == 0) return;

        int randomId = UnityEngine.Random.Range(0, viableCharacters.Count);
        Character pickedCharacter = viableCharacters[randomId];

        pickedCharacter.statuses.AddStatus(ECharacterStatus.Corrupted, charRef);
    }

    private void PoisonNeighboursIfAble(Character charRef)
    {
        List<Character> myList = CharactersHelper.GetSortedListWithCharacterFirst(Gameplay.CurrentCharacters, charRef);
        //myList = Characters.Instance.FilterRealCharacterType(myList, ECharacterType.Villager);
        //Debug.Log("TEST: " + myList[0].dataRef.name);
        //Debug.Log("TEST: " + myList[myList.Count - 1]);
        myList.RemoveAt(0);

        if (myList[0].dataRef.type == ECharacterType.Villager)
        {
            myList[0].statuses.AddStatus(ECharacterStatus.Corrupted, charRef);
            myList[0].statuses.AddStatus(ECharacterStatus.MessedUpByEvil, charRef);
        }

        if (myList[myList.Count - 1].dataRef.type == ECharacterType.Villager)
        {
            myList[myList.Count - 1].statuses.AddStatus(ECharacterStatus.Corrupted, charRef);
            myList[myList.Count - 1].statuses.AddStatus(ECharacterStatus.MessedUpByEvil, charRef);
        }
    }
}
[System.Serializable]
public class Striga : Demon // Lilith // Lilis
{
    public override List<SpecialRule> GetRules() => new List<SpecialRule>()
    {
        new NightModeRule(4),
    };

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger == ETriggerPhase.Start)
            charRef.statuses.AddStatus(ECharacterStatus.UnkillableByDemon, charRef);

        if (charRef.state == ECharacterState.Dead) return;
        if (trigger == ETriggerPhase.Night)
        {
            KillHidden(charRef);
            PlayerController.PlayerInfo.health.Damage(2);
        }
    }
}

//Special Rules / Modes
[System.Serializable]
public abstract class SpecialRule
{
    public virtual void Init()
    {
    }
    public virtual void Remove()
    {
    }
}

[System.Serializable]
public class NightModeRule : SpecialRule
{
    public NightModeRule(int revealPerDay)
    {
        this.revealPerDay = revealPerDay;
    }

    public Action onStepIncrease;
    public Action onNightStart;
    public int revealPerDay = 4;
    public int currentStep = 0;

    public override void Init()
    {
        GameplayEvents.OnCharacterKilled += ManageKill;
        GameplayEvents.OnCharacterRevealed += Revealed;
    }
    public override void Remove()
    {
        GameplayEvents.OnCharacterKilled -= ManageKill;
        GameplayEvents.OnCharacterRevealed -= Revealed;
    }

    private void Revealed(Character ch)
    {
        if (Gameplay.GameplayState == EGameplayState.Summary) return;
        if (ch.state == ECharacterState.Dead) return;
        currentStep++;
        onStepIncrease?.Invoke();
        if (currentStep >= revealPerDay)
        {
            onNightStart?.Invoke();
            Gameplay.ChangeGameplayState(EGameplayState.Night);
            currentStep = 0;
        }
    }

    private async void ManageKill(Character obj)
    {
        await Task.Delay(100);
        if (Gameplay.PrevState == EGameplayState.Night) return;
    }
}