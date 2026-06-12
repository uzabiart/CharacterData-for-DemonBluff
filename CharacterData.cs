// Project: Demon Bluff (Sample Reference)
// File: CharacterData.cs  |  Version: v0.380f 
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
using UnityEngine.Localization.Settings;

public interface ICardData
{
    public string GetCharacterName();
    public string GetDescription();
}

[CreateAssetMenu(menuName = "UMI/new Character")]
public class CharacterData : ScriptableObject, ICharacterLocData, ICardData
{
    public string characterId;
    public string localization_key;

    public string characterName;
    public string iWasName;
    public EGender gender;

    public RoguelikeDataInfo roguelikeInfo;

    public List<CharacterData> bundledCharacters = new List<CharacterData>();
    [TextArea(6, 10)]
    public string description;
    [TextArea(4, 10)]
    public string descriptionPL;
    [TextArea(4, 10)]
    public string descriptionCHN;
    [TextArea(4, 10)]
    public string flavorText;
    [TextArea(2, 5)]
    public string[] additionalFlavorTexts;
    [TextArea(2, 5)]
    public string[] unusedFlavorTexts;
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
    public List<ECharacterStatus> additionalStatuses = new List<ECharacterStatus>();
    public List<ECharacterTag> tags = new List<ECharacterTag>();
    public List<CharacterData> canAppearIf = new List<CharacterData>();
    public ECharacterType type;
    public EAlignment startingAlignment;
    public EAbilityUsage abilityUsage;
    public bool bluffable = true;
    public bool usuallyDisguised = false;
    public bool picking = true;
    public bool doNotCountAsEvilForUi = false;
    public AddedCharacterTypes additionalPossibleCharacters = new AddedCharacterTypes();
    [SerializeReference]
    public Role role;

    [SerializeReference]
    public CharacterLoc translation;

    public string GetCharacterName()
    {
        return characterName;
    }

    public string GetIWas()
    {
        return iWasName;
    }

    public EGender GetGender()
    {
        return gender;
    }

    public string GetFlavorText()
    {

        if (LocalizationSettings.SelectedLocale.LocaleName == "English (en)")
        {
            if (additionalFlavorTexts.Length > 0)
                return additionalFlavorTexts[UnityEngine.Random.Range(0, additionalFlavorTexts.Length)];
        }
        else
        {
            string desc = CharacterLocalization.GetFlavor(this);
            return desc;
        }
        return flavorText;
    }

    public string GetTranslatedName(string localeCode)
    {
        return translation?.GetTranslatedName(localeCode) ?? name;
    }

    public string GetIWasTranslated(string localeCode)
    {
        return translation?.GetIWasTranslated(localeCode) ?? name;
    }

    public CharacterLoc GetTranslation() => translation;

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
        if (LocalizationSettings.SelectedLocale.LocaleName == "English (en)")
            return StringHelper.ConvertTextToTextWithTooltips(description);

        string localized = CharacterLocalization.GetDescription(this);
        return StringHelper.ConvertTextToTextWithTooltips(!string.IsNullOrEmpty(localized) ? localized : description);
    }

    public string GetIfLies()
    {
        if (LocalizationSettings.SelectedLocale.LocaleName == "English (en)")
            return StringHelper.ConvertTextToTextWithTooltips(ifLies);

        string localized = CharacterLocalization.GetIfLies(this);
        return StringHelper.ConvertTextToTextWithTooltips(!string.IsNullOrEmpty(localized) ? localized : ifLies);
    }

    public string GetHints()
    {
        if (LocalizationSettings.SelectedLocale.LocaleName == "English (en)")
            return StringHelper.ConvertTextToTextWithTooltips(hints);

        string localized = CharacterLocalization.GetHints(this);
        return StringHelper.ConvertTextToTextWithTooltips(!string.IsNullOrEmpty(localized) ? localized : hints);
    }

    public string GetArtistName()
    {
        string artist = "normandia";

        if (currentSkin != null)
            artist = currentSkin.artistName;

        return artist;
    }

    [Button]
    public void UpdateCharacterName()
    {
        characterName = this.name;
    }
}

[System.Serializable]
public class AddedCharacterTypes
{
    [ShowInInspector]
    public List<CharacterCount> count = new List<CharacterCount>();
}
[System.Serializable]
public class CharacterCount
{
    public int count;
    public ECharacterType type;
}

[System.Serializable]
public class RoguelikeDataInfo
{
    public int gemsCost = 0;
    public int coinsCost = 0;

    public int points = 10;
    public float pointsMult = 1;
    public int gems = 0;

    public int GetPoints()
    {
        return points;
    }
    public float GetMultiplier()
    {
        return pointsMult;
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
    Infinite = 50,
}

public enum ECharacterTag
{
    None = 0,
    Corrupt = 10,
    Bluff = 20,
}

public class RoleUses
{
    public int uses = 1;
    public ETriggerPhase trigger = ETriggerPhase.Any;
}

[System.Serializable]
public abstract class Role
{
    public virtual List<SpecialRule> GetRules() => new List<SpecialRule>() { };
    public virtual List<ECharacterTag> GetTags() => new List<ECharacterTag>() { };
    public virtual string GetDreamerClue() => "I forgot my dream";
    protected LocalizedRole localization = null;
    public virtual LocalizedRole GetLocalization() => null;
    public abstract string Description { get; }

    Character charRef;
    public CharacterData dataRef;

    public Action<ActedInfo> onActed;
    public Action<ActedInfo> savedOnActed;
    public Dictionary<ETriggerPhase, Action<ActedInfo>> savedTriggerActs;
    public ActedInfo savedActInfo;
    public void Init()
    {
        dataRef = ProjectContext.Instance.gameData.GetCharacterDataOfRole(this);
    }
    public virtual void OnSpawn(Character charRef) { }
    public void ManagePreAct(ETriggerPhase trigger)
    {
        savedTriggerActs[trigger] = onActed;
    }
    public bool OnActed(ETriggerPhase trigger, Character charRef, ActedInfo info)
    {
        if (trigger == ETriggerPhase.OnPicked || charRef.killedByDemon || charRef.killedHidden) return ManageOnPicked();
        else return ActNormal();

        bool ManageOnPicked()
        {
            if (charRef.state == ECharacterState.Hidden)
            {
                savedActInfo = info;
                return false;
            }
            else
                return ActNormal();
        }

        bool ActNormal()
        {
            if (savedActInfo != null)
                onActed?.Invoke(savedActInfo);
            else
                onActed?.Invoke(info);

            return true;
        }
    }
    public virtual void OnInit(Character charRef) { }
    public abstract ActedInfo GetInfo(Character charRef);
    public abstract ActedInfo GetBluffInfo(Character charRef);
    public virtual int GetActCost(ETriggerPhase trigger)
    {
        return 1;
    }
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
        if (!charRef.statuses.Contains(ECharacterStatus.Corrupted))
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
    public string TryLocalize(CharacterData character, System.Action<TranslationContext> setup)
    {
        string result = CharacterLocalizationAPI.GetTranslation(
            character,
            character.translation,
            setup
            //ctx => ctx
            //.SetRaw("n", 4)
            );

        if (result == "ERROR") return "";

        return result;
    }

    //public string TryLocalize(string key, object args)
    //{
    //    //if (ProjectContext.Instance.gameData.language == ELanguage.English)
    //    //return "";
    //    if (ProjectContext.Instance.gameData.language == ELanguage.English)
    //        LocalizationAPI.SetLanguage("en");
    //    if (ProjectContext.Instance.gameData.language == ELanguage.Polish)
    //        LocalizationAPI.SetLanguage("pl");

    //    string text = LocalizationAPI.GetText(key, args);


    //    return text;
    //}
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

        info = ConjourInfo(pickedEvil.GetRegisterAs(), closestEvil, charRef);
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
        info = ConjourInfo(pickedEvil.dataRef, id, charRef);

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
            if (myList[i].GetRegisterAlignment() == EAlignment.Evil)
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
            if (myList[i].GetRegisterAlignment() == EAlignment.Evil)
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

    public string ConjourInfo(CharacterData character, int steps, Character charRef)
    {
        bool onlyOneEvil = false;
        if (steps > 20)
            onlyOneEvil = true;

        string translation = TryLocalize(dataRef,
            ctx => ctx
            .SetRaw("characterName", character)
            .SetBool("onlyOneEvil", onlyOneEvil)
            .SetRaw("n", steps + 1));
        if (!String.IsNullOrEmpty(translation)) return translation;

        if (steps > 20)
            return $"There is only 1 Evil";
        else if (steps == 0)
            return $"{character.GetCharacterName()} is\n{steps + 1} card away\nfrom closest Evil";
        else
            return $"{character.GetCharacterName()} is\n{steps + 1} cards away\nfrom closest Evil";
    }
}

[System.Serializable]
public class Knitter : Role
{
    public override string Description
    => "You start knowing how many pairs of evil players there are";

    public string ConjourInfo(int pairCount, Character charRef)
    {
        string translation = TryLocalize(dataRef,
            ctx => ctx
            .SetRaw("n", pairCount));
        if (!String.IsNullOrEmpty(translation)) return translation;

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
        string info = ConjourInfo(pairCount, charRef);
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

        string info = ConjourInfo(randomPairCount, charRef);
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
            if (ch.GetRegisterAlignment() == EAlignment.Evil)
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
        Role role = infoRoles[UnityEngine.Random.Range(0, infoRoles.Count)];
        role.dataRef = ProjectContext.Instance.gameData.GetCharacterDataOfRole(role);
        ActedInfo newInfo = role.GetInfo(charRef);
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
        role.dataRef = ProjectContext.Instance.gameData.GetCharacterDataOfRole(role);
        ActedInfo newInfo = role.GetBluffInfo(charRef);
        return newInfo;
    }

    public List<Role> infoRoles = new List<Role>()
    {
        new Empath(),
        new Scout(),
        new Investigator(),
        //new BountyHunter(),
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
public class UselessVillager : Role
{
    public override string Description
    => "Vill get bluffed as - if possible";

    public override void OnInit(Character charRef)
    {
        charRef.statuses.AddStatus(ECharacterStatus.Silenced, charRef);
    }

    public override ActedInfo GetInfo(Character charRef)
    {
        return null;
    }

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        //if (trigger == ETriggerPhase.Init) OnInit(charRef);

        if (trigger == ETriggerPhase.Day)
        {
            onActed?.Invoke(new ActedInfo("Mmph mmph!"));
        }
    }
    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        //if (trigger == ETriggerPhase.Init) OnInit(charRef);

        if (trigger == ETriggerPhase.Day)
        {
            onActed?.Invoke(new ActedInfo("Mmph mmph!"));
        }
    }

    public override ActedInfo GetBluffInfo(Character charRef)
    {
        return null;
    }
}

[System.Serializable]
public class Witness : Role
{
    public override string Description
    => "Learn a character that was affected by an Evil ability";

    public string ConjourInfo(Character messedCharacter, Character charRef)
    {
        string translation = TryLocalize(dataRef,
            ctx => ctx
            .SetBool("messedUp", messedCharacter == null ? false : true)
            .SetIds("ids", new List<int>() { messedCharacter == null ? 0 : messedCharacter.id }));
        if (!String.IsNullOrEmpty(translation)) return translation;

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

        string info = ConjourInfo(randomCharacter, charRef);
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

        string info = ConjourInfo(randomCharacter, charRef);
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
                if (cc.GetRegisterAlignment() == EAlignment.Good)
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
        Left = 0,
        Right = 1,
        Both = 2,
    }

    public class ArchitectInfo
    {
        public ECircleSide side;
        public List<Character> characters = new List<Character>();
    }

    public override ActedInfo GetInfo(Character charRef)
    {
        ArchitectInfo infos = GetSideOfCircle(charRef, true);

        string info = ConjourInfo(infos.side, charRef);

        ActedInfo newInfo = new ActedInfo(info, infos.characters);
        return newInfo;
    }

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger == ETriggerPhase.Day)
            onActed?.Invoke(GetInfo(charRef));
    }
    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger == ETriggerPhase.Day)
            onActed?.Invoke(GetBluffInfo(charRef));

        if (trigger == ETriggerPhase.OnExecuted)
            if (!charRef.statuses.Contains(ECharacterStatus.HealthyBluff))
                CheckAchievementsAndUnlockIfAble();
    }

    public override ActedInfo GetBluffInfo(Character charRef)
    {
        ArchitectInfo infos = GetSideOfCircle(charRef, false);

        string info = ConjourInfo(infos.side, charRef);

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
                if (c.GetRegisterAlignment() == EAlignment.Evil)
                    leftEvils++;
            }
            if (i >= (circleSize + 1) / 2)
            {
                rightChars.Add(c);
                if (c.GetRegisterAlignment() == EAlignment.Evil)
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

    public string ConjourInfo(ECircleSide side, Character charRef)
    {
        string translation = TryLocalize(dataRef,
            ctx => ctx
            .SetCustom("side", (int)side));
        if (!String.IsNullOrEmpty(translation)) return translation;

        string info = "";
        if (side == ECircleSide.Left)
            info = $"Left side is more Evil";
        if (side == ECircleSide.Right)
            info = $"Right side is more Evil";
        if (side == ECircleSide.Both)
            info = $"Both sides are equally Evil";
        return info;
    }

    //ACHIEVEMENTS
    private void CheckAchievementsAndUnlockIfAble()
    {
        ProjectContext.UnlockAchievement("Architect_Halloween_ACHIV_3290");
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
        string info = ConjourInfo(evils, charRef);
        ActedInfo newInfo = new ActedInfo(info, Characters.Instance.GetAdjacentCharacters(charRef));
        CheckAchievementsAndUnlockIfAble(newInfo);
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
        string info = ConjourInfo(randomEvilNumber, charRef);
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
            if (ch.GetRegisterAlignment() == EAlignment.Evil)
                evils++;
        }

        return evils;
    }

    public string ConjourInfo(int evils, Character charRef)
    {
        string translation = TryLocalize(dataRef,
            ctx => ctx
            .SetRaw("n", evils));
        if (!String.IsNullOrEmpty(translation)) return translation;

        string info = "";
        if (evils == 0)
            info = $"NO Evils\nadjacent to me";
        else if (evils == 1)
            info = $"{evils} Evil\nadjacent to me";
        else
            info = $"{evils} Evils\nadjacent to me";

        return info;
    }

    //ACHIEVEMENTS
    private void CheckAchievementsAndUnlockIfAble(ActedInfo info)
    {
        foreach (Character c in info.characters)
            AchievementsHelper.LoversHelper.AddCharacter(c);
        //if (charRef.GetAlignment() == EAlignment.Evil)
    }
}

[System.Serializable]
public class Sapper : Role // Villager
{
    public override string Description
        => "Learn if there is an Evil near me [Range 2]";

    public override ActedInfo GetInfo(Character charRef)
    {
        int evils = CheckAdjacentEvils(GetRange2Characters(charRef));
        ActedInfo newInfo = new ActedInfo(ConjourInfo(evils, charRef), GetRange2Characters(charRef));
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
        int evils = CheckAdjacentEvils(GetRange2Characters(charRef));

        if (evils > 0)
            evils = 0;
        else
            evils = UnityEngine.Random.Range(1, 3);

        ActedInfo newInfo = new ActedInfo(ConjourInfo(evils, charRef), GetRange2Characters(charRef));

        return newInfo;
    }

    public string ConjourInfo(int evils, Character charRef)
    {
        string translation = TryLocalize(dataRef,
            ctx => ctx
            .SetRaw("n", evils));
        if (!String.IsNullOrEmpty(translation)) return translation;

        string info = $"There is at least 1 Evil near me!";
        if (evils == 0)
            info = $"NO Evils near me";

        return info;
    }

    public List<Character> GetRange2Characters(Character charRef)
    {
        List<Character> range2Characters = new List<Character>();
        List<Character> adjacentCharacters = new List<Character>(Gameplay.CurrentCharacters);

        adjacentCharacters = CharactersHelper.GetSortedListWithCharacterFirst(Gameplay.CurrentCharacters, charRef);

        adjacentCharacters.RemoveAt(0);

        range2Characters.Add(adjacentCharacters[0]);
        range2Characters.Add(adjacentCharacters[1]);
        range2Characters.Add(adjacentCharacters[adjacentCharacters.Count - 1]);
        range2Characters.Add(adjacentCharacters[adjacentCharacters.Count - 2]);

        return range2Characters;
    }

    public int CheckAdjacentEvils(List<Character> adjacentCharacters)
    {
        int evils = 0;

        foreach (Character c in adjacentCharacters)
            if (c.GetRegisterAlignment() == EAlignment.Evil)
                evils++;

        return evils;
    }
}
[System.Serializable]
public class Sapper2 : Role
{
    public override string Description
        => "Learn if there is an Evil near me [Range 2]";

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

        if (adjacentCharacters[0].GetRegisterAlignment() == EAlignment.Evil)
            evils++;
        if (adjacentCharacters[1].GetRegisterAlignment() == EAlignment.Evil)
            evils++;
        if (adjacentCharacters[adjacentCharacters.Count - 1].GetRegisterAlignment() == EAlignment.Evil)
            evils++;
        if (adjacentCharacters[adjacentCharacters.Count - 2].GetRegisterAlignment() == EAlignment.Evil)
            evils++;

        return evils;
    }
}

[System.Serializable]
public class Rambler : Role // NL
{
    public override string Description
        => "";

    int currentTimesPicked = 0;
    int maxTimesPicked = 1;

    string savedQote;

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger == ETriggerPhase.Day)
        {
            string info = GetARandomQuote();
            ActedInfo newInfo = new ActedInfo(info);
            OnActed(trigger, charRef, newInfo);
        }
        if (trigger == ETriggerPhase.OnPicked)
        {
            if (currentTimesPicked < maxTimesPicked)
            {
                currentTimesPicked++;
                ActedInfo newInfo = GetInfo(charRef);
                if (newInfo == null)
                {
                    charRef.actedInfos[charRef.actedInfos.Count - 1].characters = new List<Character>() { CharacterPicker.CurrentPicker };
                    return;
                }
                InterefereCharacter(charRef);
                OnActed(trigger, charRef, newInfo);
                //if (OnActed(trigger, charRef, newInfo))
                //{
                //    ActedInfo info = new ActedInfo($"...");
                //    charRef.InterfereActed(info, 0.02f, isDelay: true, howLong: 3f);
                //}
            }
        }
    }

    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger == ETriggerPhase.Day)
        {
            string info = GetARandomQuote();
            ActedInfo newInfo = new ActedInfo(info);
            OnActed(trigger, charRef, newInfo);
        }
        if (trigger == ETriggerPhase.OnPicked)
        {
            if (currentTimesPicked < maxTimesPicked)
            {
                ActedInfo newInfo = GetBluffInfo(charRef);
                if (newInfo == null)
                {
                    charRef.actedInfos[charRef.actedInfos.Count - 1].characters = new List<Character>() { CharacterPicker.CurrentPicker };
                    return;
                }
                InterefereCharacter(charRef);
                OnActed(trigger, charRef, newInfo);
                currentTimesPicked++;
            }
        }
    }

    private void InterefereCharacter(Character charRef)
    {
        if (charRef.state == ECharacterState.Hidden) return;

        Character ch = CharacterPicker.CurrentPicker;
        ActedInfo info = new ActedInfo($"#{charRef.id}\nshut up!", new List<Character>() { charRef });

        ch.InterfereActed(info, 0.02f, isDelay: false, howLong: 3f);
    }

    List<string> quotes = new List<string>()
    {
        "Get it twisted! Your actions impact nothing",
        "When you duct tape your mouth, nothing can stop you",
        "I got the sofa without making any payments on it",
        "Statistically, you're like 12.5% grandma",
        "I usually spend an hour a day just making myself sneeze",
        "I just had a brain… genius moment",
        "I keep waking up my cat with my exaltations",
        "I got a new think",
        "One time, the Poet called me an egg",
        "As long as you don't die, you win",
        "I'm probably the most humble person I know",
        "I am an idiot sandwich",
        "Mostly what keeps my house clean is the fear of God",
        "Successful people are better people",
        "Its easier to order a sandwich than to develop personality",
        "Look, I've got being annoying down to a science",
        "To confuse your enemies, you must first confuse yourself",
        "To stay unpredictable, never know what you are doing",
        "Oh, how the turns have tabled",
        "Hey Village, Rambler here",
        "If you are lost, simply remember where you are",
        "If you fail, simply succeed next time",
        "If you close your eyes, things become harder to see",
        //"Don't be wrong, be correct instead",
        "Don't love your job, job your love",
        "Don’t feed the hand that bites",
        "The more questions you ask the more confused you’ll be",
        //"Darkness is usually harder to see in",
        //"A closed door is harder to walk through",
        "If you are mad, just calm down",
        "An apple is like… it’s made out of edible wood",
        "If it almost rhymes. It must almost be true",
        "You don’t ever learn information permanently, you rent it",
        "Roughly half of all outcomes are worse than the other half",
        "I have been alive for almost my whole life",
        "The longer something takes, the longer it feels",
    };

    public override ActedInfo GetInfo(Character charRef)
    {
        Character ch = CharacterPicker.CurrentPicker;

        List<Character> characters = new List<Character>();
        characters.Add(ch);

        if (!IsCharacterLying(ch, charRef)) return null;

        string info = ConjourInfo(IsCharacterLying(ch, charRef), ch);

        ActedInfo newInfo = new ActedInfo(info, characters);
        return newInfo;
    }

    public override ActedInfo GetBluffInfo(Character charRef)
    {
        Character ch = CharacterPicker.CurrentPicker;

        List<Character> characters = new List<Character>();
        characters.Add(ch);

        if (IsCharacterLying(ch, charRef)) return null;

        string info = ConjourInfo(!IsCharacterLying(ch, charRef), ch);

        ActedInfo newInfo = new ActedInfo(info, characters);
        return newInfo;
    }

    public bool IsCharacterLying(Character c, Character charRef)
    {
        //if (CharacterHelper.CheckLyingAppearance(c))
        if (CharacterHelper.CheckLyingAppearance(c))
            return true;

        return false;
    }

    public string GetARandomQuote()
    {
        string info = quotes[UnityEngine.Random.Range(0, quotes.Count)];
        savedQote = info;
        return info;
    }

    public string ConjourInfo(bool isPickerDisguised, Character ch)
    {
        string info = $"#{ch.id}\nis NOT Lying";
        if (isPickerDisguised)
            info = $"...";

        CheckAchievementsAndUnlockIfAble(info);
        //return $"#{ch.id}\nis Disguised";

        return info;
    }

    //ACHIEVEMENTS
    private void CheckAchievementsAndUnlockIfAble(string info)
    {
        if (info == "...")
            ProjectContext.UnlockAchievement("Rambler_1_ACHIV_3167");
    }
}
[System.Serializable]
public class Rambler2 : Role // NL
{
    public override string Description
       => "";

    int currentTimesPicked = 0;
    int maxTimesPicked = 1;

    string savedQote;

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger == ETriggerPhase.AfterRoundStart)
        {
            ShutUpAdjacentTruthfulls(charRef);
        }
        if (trigger == ETriggerPhase.Day)
        {
            string info = "";
            info = ConjourInfo();
            ActedInfo newInfo = new ActedInfo(info, Characters.Instance.GetAdjacentCharacters(charRef));

            //ShutUpAdjacentTruthfulls(charRef);

            OnActed(trigger, charRef, newInfo);
        }
    }

    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger == ETriggerPhase.AfterRoundStart)
        {
            ShutUpAdjacentLyings(charRef);
        }
        if (trigger == ETriggerPhase.Day)
        {
            string info = "";
            info = ConjourInfo();
            ActedInfo newInfo = new ActedInfo(info, Characters.Instance.GetAdjacentCharacters(charRef));


            OnActed(trigger, charRef, newInfo);
        }
    }

    private void ShutUpAdjacentTruthfulls(Character charRef)
    {
        List<Character> adjacents = Characters.Instance.GetAdjacentCharacters(charRef);

        foreach (Character c in adjacents)
            InterefereCharacter(charRef, c, false);

        //foreach (Character c in adjacents)
        //if (!CharacterHelper.CheckLyingAppearance(c))
        //InterefereCharacter(charRef, c);
    }
    private void ShutUpAdjacentLyings(Character charRef)
    {
        List<Character> adjacents = Characters.Instance.GetAdjacentCharacters(charRef);

        foreach (Character c in adjacents)
            InterefereCharacter(charRef, c, true);

        //foreach (Character c in adjacents)
        //if (CharacterHelper.CheckLyingAppearance(c))
        //InterefereCharacter(charRef, c);
    }

    private void InterefereCharacter(Character charRef, Character ch, bool shouldLie)
    {
        if (ch.state == ECharacterState.Hidden)
        {
            Action<ActedInfo, ETriggerPhase> handler = null;
            handler = (info, trigger) =>
            {
                InterfereCharacterOnReveal(shouldLie, info, trigger, charRef, ch);
            };
            ch.onAboutToAct += handler;
            return;
        }

        if (CharacterHelper.CheckLyingAppearance(ch) != shouldLie) return;

        ActedInfo info = new ActedInfo($"#{charRef.id}\nshut up!", new List<Character>() { charRef });

        ch.InterfereActed(info, 0.02f, isDelay: false, howLong: 3f);
    }

    int amountShutups = 0;
    public void InterfereCharacterOnReveal(bool shouldLie, ActedInfo info, ETriggerPhase trigger, Character charRef, Character targetCh)
    {
        if (CharacterHelper.CheckLyingAppearance(targetCh) == shouldLie)
            if (targetCh.state != ECharacterState.Hidden)
            {
                string translation = TryLocalize(dataRef,
                    ctx => ctx
                    .SetBool("shutup", true)
                    .SetIds("ids", new List<int>() { charRef.id })
                    );
                if (!String.IsNullOrEmpty(translation))
                    info.desc = translation;
                else
                {
                    amountShutups++;
                    info.desc = $"#{charRef.id}\nshut up!";
                }

                Debug.Log($"NANI: " + amountShutups);
                CheckAchievementsAndUnlockIfAble();

                info.characters = new List<Character>() { charRef };
            }
    }

    List<string> quotes = new List<string>()
    {
        "Get it twisted! Your actions impact nothing",
        "When you duct tape your mouth, nothing can stop you",
        "I got the sofa without making any payments on it",
        "Statistically, you're like 12.5% grandma",
        "I usually spend an hour a day just making myself sneeze",
        "I just had a brain… genius moment",
        "I keep waking up my cat with my exaltations",
        "I got a new think",
        "One time, the Poet called me an egg",
        "As long as you don't die, you win",
        "I'm probably the most humble person I know",
        "I am an idiot sandwich",
        "Mostly what keeps my house clean is the fear of God",
        "Successful people are better people",
        "Its easier to order a sandwich than to develop personality",
        "Look, I've got being annoying down to a science",
        "To confuse your enemies, you must first confuse yourself",
        "To stay unpredictable, never know what you are doing",
        "Oh, how the turns have tabled",
        "Hey Village, Rambler here",
        "If you are lost, simply remember where you are",
        "If you fail, simply succeed next time",
        "If you close your eyes, things become harder to see",
        "Don't love your job, job your love",
        "Don’t feed the hand that bites",
        "The more questions you ask the more confused you’ll be",
        "If you are mad, just calm down",
        "An apple is like… it’s made out of edible wood",
        "If it almost rhymes. It must almost be true",
        "You don’t ever learn information permanently, you rent it",
        "Roughly half of all outcomes are worse than the other half",
        "I have been alive for almost my whole life",
        "The longer something takes, the longer it feels",
    };

    public override ActedInfo GetInfo(Character charRef)
    {
        return null;
    }

    public override ActedInfo GetBluffInfo(Character charRef)
    {
        return null;
    }

    public bool IsCharacterLying(Character c, Character charRef)
    {
        //if (CharacterHelper.CheckLyingAppearance(c))
        if (CharacterHelper.CheckLyingAppearance(c))
            return true;

        return false;
    }

    public string GetARandomQuote()
    {
        string info = quotes[UnityEngine.Random.Range(0, quotes.Count)];
        savedQote = info;
        return info;
    }

    public string ConjourInfo()
    {
        int randomQuote = UnityEngine.Random.Range(1, 21);
        string translation = TryLocalize(dataRef,
            ctx => ctx
            .SetBool("shutup", false)
            .SetRaw("n", randomQuote)
            );
        if (!String.IsNullOrEmpty(translation)) return translation;

        string info = GetARandomQuote();

        return info;
    }

    //ACHIEVEMENTS
    private void CheckAchievementsAndUnlockIfAble()
    {
        if (amountShutups >= 2)
            ProjectContext.UnlockAchievement("Rambler_1_ACHIV_3167");
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
                {
                    CorruptedKnightKilled();
                    PlayerController.PlayerInfo.health.Damage(4);
                }
        }

        if (trigger == ETriggerPhase.OnProtected)
            ProtectedKnight(charRef);

        //if (trigger != ETriggerPhase.Day) return;
        //onActed?.Invoke(GetInfo());
    }

    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger == ETriggerPhase.OnExecuted)
        {
            if (charRef.alignment != EAlignment.Evil)
                if (charRef.statuses.statuses.Contains(ECharacterStatus.Corrupted))
                {
                    CorruptedKnightKilled();
                    PlayerController.PlayerInfo.health.Damage(4);
                }
        }

        if (trigger == ETriggerPhase.OnProtected)
            ProtectedKnight(charRef);

        //if (trigger != ETriggerPhase.Day) return;
        //onActed?.Invoke(GetInfo());
    }

    public string GetInfo()
    {
        return "I can't die";
    }

    public override bool CheckIfCanBeKilled(Character charRef)
    {
        //if (charRef.statuses.statuses.Contains(ECharacterStatus.BrokenAbility))
        //return true;
        if (charRef.statuses.statuses.Contains(ECharacterStatus.HealthyBluff))
            return false;
        if (charRef.statuses.statuses.Contains(ECharacterStatus.Corrupted))
            return true;
        if (charRef.alignment == EAlignment.Evil)
            return true;
        else
            return false;
    }

    private void ProtectedKnight(Character charRef)
    {
        if (AchievementsHelper.KnightsHelper.StabbedKnights.Count <= 3)
            AchievementsHelper.KnightsHelper.ProtectedKnight(charRef);
        if (AchievementsHelper.KnightsHelper.StabbedKnights.Count >= 3)
            ProjectContext.UnlockAchievement("Knight_Halloween_ACHIV_6588");
    }
    void CorruptedKnightKilled()
    {
        AchievementsHelper.KnightsHelper.KnightKilled();
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

    CharacterData prevChar;

    public override ActedInfo GetInfo(Character charRef)
    {
        return new ActedInfo("");
    }

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger == ETriggerPhase.Day)
        {
            CheckAchievementsAndUnlockIfAble(charRef);

            ShowMyPreviousRole(charRef);
            if (charRef.statuses.Contains(ECharacterStatus.BrokenAbility)) return;
            CreateNewBaker(charRef);
        }
    }

    private void ShowMyPreviousRole(Character charRef)
    {
        CharacterData prevCharacter = null;
        prevCharacter = prevChar;

        if (charRef.statuses.statuses.Contains(ECharacterStatus.AlteredCharacter))
            prevCharacter = ProjectContext.Instance.gameData.GetCharacterDataOfRole(this);

        string info = ConjourInfo(prevCharacter, charRef);
        onActed?.Invoke(new ActedInfo(info));
    }

    private void CreateNewBaker(Character chRef)
    {
        List<Character> characters = new List<Character>(Gameplay.CurrentCharacters);

        characters = Characters.Instance.FilterCharacterType(characters, ECharacterType.Villager);
        characters = Characters.Instance.FilterRealAlignmentCharacters(characters, EAlignment.Good);
        characters = Characters.Instance.FilterHiddenCharacters(characters);

        characters.Remove(chRef);

        if (characters.Count == 0) return;

        Character randomGood = characters[UnityEngine.Random.Range(0, characters.Count)];

        CharacterData bakerData = ProjectContext.Instance.gameData.GetCharacterDataOfRole(this);

        CharacterData savedData = randomGood.dataRef;
        randomGood.InitWithNoReset(bakerData);
        ((Baker)randomGood.role).InjectPreviousRole(savedData);
    }

    public void InjectPreviousRole(CharacterData prevData)
    {
        prevChar = prevData;
    }

    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        ShowMyPreviousRoleLying(charRef);
        CheckAchievementsAndUnlockIfAble(charRef);

        if (charRef.statuses.Contains(ECharacterStatus.WorkingAbility))
            CreateNewBaker(charRef);
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

        CharacterData prevCharacter = null;
        if (bakerRevealed || charRef.GetRuntimeData() == null)
        {
            List<CharacterData> scriptVillagers = Gameplay.Instance.GetScriptCharactersOfType(ECharacterType.Villager);
            CharacterData removeCharacter = null;
            if (charRef.GetRuntimeData() != null)
                foreach (CharacterData cd in scriptVillagers)
                {
                    if (cd.GetCharacterName() == ((BakerRuntimeData)(charRef.GetRuntimeData())).charName)
                    { removeCharacter = cd; break; }
                }

            if (removeCharacter != null)
                scriptVillagers.Remove(removeCharacter);

            prevCharacter = scriptVillagers[UnityEngine.Random.Range(0, scriptVillagers.Count)];
        }

        string info = ConjourInfo(prevCharacter, charRef);
        onActed?.Invoke(new ActedInfo(info));
    }

    public override ActedInfo GetBluffInfo(Character charRef)
    {
        return new ActedInfo("");
    }

    public string ConjourInfo(CharacterData prevCharacter, Character charRef)
    {
        string translation = TryLocalize(dataRef,
            ctx => ctx
            .SetRaw("characterName", prevCharacter)
            .SetBool("original", prevCharacter == null)
        );
        if (!String.IsNullOrEmpty(translation)) return translation;

        if (prevCharacter == null)
            return $"I am the original Baker";
        else
        {
            if (prevCharacter.GetCharacterName()[0] == 'A'
                || prevCharacter.GetCharacterName()[0] == 'E'
                || prevCharacter.GetCharacterName()[0] == 'I'
                || prevCharacter.GetCharacterName()[0] == 'O'
                || prevCharacter.GetCharacterName()[0] == 'U')
                return $"I was an {prevCharacter.GetCharacterName()}";
            else
                return $"I was a {prevCharacter.GetCharacterName()}";
        }
    }

    //ACHIEVEMENTS
    private void CheckAchievementsAndUnlockIfAble(Character charRef)
    {
        AchievementsHelper.BekrsHelper.AddCharacter(charRef);
    }
}

[System.Serializable]
public class Alchemist : Role
{
    public override string Description
        => "2 characters to the left nad right of me are cured from Poison. Learn how many Poisoning I cured.";

    public int corruptions = 0;

    public override void OnSpawn(Character charRef)
    {
        charRef.statuses.AddResistance(ECharacterStatus.Corrupted, charRef);
    }

    public override ActedInfo GetInfo(Character charRef)
    {
        int cures = 0;

        cures = corruptions;

        string info = ConjourInfo(cures, charRef);
        List<Character> range = Characters.Instance.GetCharactersAtRange(2, charRef);
        range.AddRange(Characters.Instance.GetCharactersAtRange(1, charRef));
        ActedInfo newInfo = new ActedInfo(info, range);
        return newInfo;
    }

    private void CurePoisons(Character charRef)
    {
        List<Character> poisonedCharacters = GetPoisonedCharactersAroundMe(charRef);

        string cures = "";
        int i = 0;
        List<Character> curedCharacters = new List<Character>();
        foreach (Character ch in poisonedCharacters)
        {
            corruptions++;
            if (ch.statuses.CheckIfCanCurePoisonAndCure())
            {
                cures += $" {GameLogStrings.TargetCharacterData(i)},";
                curedCharacters.Add(ch);
                i++;
            }
        }

        GameplayEvents.OnLogRoundAction?.Invoke(
            new GameLog(
                $"{GameLogStrings.thisData} cured{cures}",
                charRef,
                targets: curedCharacters
                ));
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
        if (trigger == ETriggerPhase.Init) OnInit(charRef);

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
            if (charRef.statuses.Contains(ECharacterStatus.WorkingAbility))
                CurePoisons(charRef);

            charRef.CreateRuntimeData(new AlchemistRuntimeData(0));
        }

        if (trigger != ETriggerPhase.Day) return;

        onActed?.Invoke(GetBluffInfo(charRef));
    }
    public override ActedInfo GetBluffInfo(Character charRef)
    {
        List<Character> poisonedCharacters = GetPoisonedCharactersAroundMe(charRef);

        int id = Calculator.RemoveNumberAndGetRandomNumberFromList(poisonedCharacters.Count, 0, 2);

        string info = ConjourInfo(id, charRef);
        List<Character> range = Characters.Instance.GetCharactersAtRange(2, charRef);
        range.AddRange(Characters.Instance.GetCharactersAtRange(1, charRef));
        ActedInfo newInfo = new ActedInfo(info, range);
        return newInfo;
    }

    public string ConjourInfo(int howManyCorruptions, Character charRef)
    {
        string translation = TryLocalize(dataRef,
            ctx => ctx
            .SetRaw("n", howManyCorruptions));
        if (!String.IsNullOrEmpty(translation)) return translation;

        if (howManyCorruptions == 0)
            return $"NO one was Corrupted around me";
        else if (howManyCorruptions == 1)
            return $"There was\n{howManyCorruptions} Corruption\naround me";
        else
            return $"There were\n{howManyCorruptions} Corruptions\naround me";
    }
}
[System.Serializable]
public class Alchemist2 : Role
{
    public override string Description
        => "Villagers within [Range 2] from me are Cured from Corruption.Learn how many I Cured. If I didn't Cure anybody, I become Corrupted and I annouce it.";

    public int corruptions = 0;

    public override void OnSpawn(Character charRef)
    {
        charRef.statuses.AddResistance(ECharacterStatus.Corrupted, charRef);
    }

    public override ActedInfo GetInfo(Character charRef)
    {
        int cures = 0;

        cures = corruptions;

        string info = ConjourInfo(cures, charRef);
        List<Character> range = Characters.Instance.GetCharactersAtRange(2, charRef);
        range.AddRange(Characters.Instance.GetCharactersAtRange(1, charRef));
        ActedInfo newInfo = new ActedInfo(info, range);
        return newInfo;
    }

    private void CurePoisons(Character charRef)
    {
        List<Character> poisonedCharacters = GetPoisonedCharactersAroundMe(charRef);

        foreach (Character ch in poisonedCharacters)
        {
            corruptions++;
            ch.statuses.CheckIfCanCurePoisonAndCure();
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
        if (trigger == ETriggerPhase.Init) OnInit(charRef);

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
            if (charRef.statuses.Contains(ECharacterStatus.WorkingAbility))
                CurePoisons(charRef);

            charRef.CreateRuntimeData(new AlchemistRuntimeData(0));
        }

        if (trigger != ETriggerPhase.Day) return;

        onActed?.Invoke(GetBluffInfo(charRef));
    }
    public override ActedInfo GetBluffInfo(Character charRef)
    {
        List<Character> poisonedCharacters = GetPoisonedCharactersAroundMe(charRef);

        int id = Calculator.RemoveNumberAndGetRandomNumberFromList(poisonedCharacters.Count, 0, 2);

        string info = ConjourInfo(id, charRef);
        List<Character> range = Characters.Instance.GetCharactersAtRange(2, charRef);
        range.AddRange(Characters.Instance.GetCharactersAtRange(1, charRef));
        ActedInfo newInfo = new ActedInfo(info, range);
        return newInfo;
    }

    public string ConjourInfo(int howManyCorruptions, Character charRef)
    {
        string translation = TryLocalize(dataRef,
            ctx => ctx
            .SetRaw("n", howManyCorruptions));
        if (!String.IsNullOrEmpty(translation)) return translation;

        if (howManyCorruptions == 0)
            return $"I am Corrupted!";
        else if (howManyCorruptions == 1)
            return $"There was\n{howManyCorruptions} Corruption\naround me";
        else
            return $"There were\n{howManyCorruptions} Corruptions\naround me";
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
        CharacterPicker.Instance.StartPickCharacters(2, charRef);
        CharacterPicker.OnCharactersPicked += CharacterPicked;
        CharacterPicker.OnStopPick += StopPick;
    }
    private void CharacterPicked()
    {
        CharacterPicker.OnCharactersPicked -= CharacterPicked;
        CharacterPicker.OnStopPick -= StopPick;

        Character c1 = CharacterPicker.PickedCharacters[0];
        Character c2 = CharacterPicker.PickedCharacters[1];
        List<Character> pickedCharacters = new List<Character>();
        pickedCharacters.Add(c1);
        pickedCharacters.Add(c2);
        List<Character> chars = new List<Character>(pickedCharacters);

        foreach (Character c in pickedCharacters)
            if (c.dataRef.role is Recluse)
            {
                string infos = "";

                infos = ConjourInfo(new List<int>() { c1.id, c2.id }, null, true);

                onActed?.Invoke(new ActedInfo(infos, pickedCharacters));
                return;
            }

        CharacterData realRole = null;

        Character pickedRealCharacter = pickedCharacters[UnityEngine.Random.Range(0, 2)];
        pickedCharacters.Remove(pickedRealCharacter);

        realRole = pickedRealCharacter.dataRef;

        CharacterData fakeRole = null;

        if (fakeRole == null)
            if (pickedCharacters[0].bluff != null)
                if (pickedCharacters[0].bluff.GetCharacterName() != realRole.GetCharacterName())
                    fakeRole = pickedCharacters[0].bluff;

        if (fakeRole == null)
        {
            List<CharacterData> eligibleFakeCharacters = new List<CharacterData>();
            foreach (CharacterData cd in Gameplay.Instance.GetScriptCharacters())
                if (cd.usuallyDisguised)
                    eligibleFakeCharacters.Add(cd);

            if (eligibleFakeCharacters.Count >= 1)
                eligibleFakeCharacters.Remove(realRole);

            if (eligibleFakeCharacters.Count >= 1)
                eligibleFakeCharacters.Remove(pickedCharacters[0].dataRef);

            if (eligibleFakeCharacters.Count >= 1)
                fakeRole = eligibleFakeCharacters[UnityEngine.Random.Range(0, eligibleFakeCharacters.Count)];
        }


        string info = "";
        List<Character> allCharacters = new List<Character>(Gameplay.CurrentCharacters);
        List<Character> availableCharacters = new List<Character>(Gameplay.CurrentCharacters);

        CharacterData realRoleName = null;
        CharacterData fakeRoleName = null;


        foreach (Character c in allCharacters)
        {
            if (fakeRole != null)
                if (fakeRole == c.dataRef)
                    availableCharacters.Remove(c);
            if (fakeRole != null)
                if (fakeRole == c.bluff)
                    availableCharacters.Remove(c);

            if (realRole != null)
                if (realRole == c.dataRef)
                    availableCharacters.Remove(c);
            if (realRole != null)
                if (realRole == c.bluff)
                    availableCharacters.Remove(c);
        }

        if (fakeRole != null)
            fakeRoleName = fakeRole;
        else
        {
            Character c = availableCharacters[UnityEngine.Random.Range(0, availableCharacters.Count)];
            fakeRoleName = c.dataRef;
            availableCharacters.Remove(c);
        }
        if (realRole != null)
            realRoleName = realRole;
        else
        {
            Character c = availableCharacters[UnityEngine.Random.Range(0, availableCharacters.Count)];
            realRoleName = c.dataRef;
            availableCharacters.Remove(c);
        }

        info = ConjourInfo(new List<int>() { c1.id, c2.id }, new List<CharacterData>() { realRoleName, fakeRoleName }, false);
        onActed?.Invoke(new ActedInfo(info, chars));
        Debug.Log($"{info}");
    }

    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        CharacterPicker.Instance.StartPickCharacters(2, charRef);
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

        Character c1 = CharacterPicker.PickedCharacters[0];
        Character c2 = CharacterPicker.PickedCharacters[1];
        List<Character> pickedCharacters = new List<Character>();
        pickedCharacters.Add(c1);
        pickedCharacters.Add(c2);

        List<CharacterData> fakeRoles = new List<CharacterData>();

        foreach (Character c in pickedCharacters)
            if (c.bluff != null)
                if (!fakeRoles.Contains(c.bluff))
                    if (pickedCharacters.All(other => other.dataRef != c.bluff))
                        fakeRoles.Add(c.bluff);

        List<CharacterData> eligibleFakeCharacters = new List<CharacterData>();
        foreach (CharacterData cd in Gameplay.Instance.GetScriptCharacters())
            if (cd.usuallyDisguised)
                if (c1.dataRef != cd)
                    if (c2.dataRef != cd)
                        eligibleFakeCharacters.Add(cd);

        foreach (CharacterData cd in fakeRoles)
            eligibleFakeCharacters.Remove(cd);

        if (fakeRoles.Count < 2)
            if (eligibleFakeCharacters.Count > 0)
            {
                CharacterData picked = eligibleFakeCharacters[UnityEngine.Random.Range(0, eligibleFakeCharacters.Count)];
                fakeRoles.Add(picked);
                eligibleFakeCharacters.Remove(picked);
            }

        if (fakeRoles.Count < 2)
            if (eligibleFakeCharacters.Count > 0)
            {
                CharacterData picked = eligibleFakeCharacters[UnityEngine.Random.Range(0, eligibleFakeCharacters.Count)];
                fakeRoles.Add(picked);
                eligibleFakeCharacters.Remove(picked);
            }

        string info = "";
        CharacterData fakeRoleData1 = null;
        CharacterData fakeRoleData2 = null;

        if (fakeRoles.Count > 0)
            fakeRoleData1 = fakeRoles[0];
        if (fakeRoles.Count > 1)
            fakeRoleData2 = fakeRoles[1];

        if (fakeRoleData1 == null)
            fakeRoleData1 = GetRandomNonRepeatedFakeCharacter(new List<CharacterData>() { c1.dataRef, c1.bluff, c2.dataRef, c2.bluff });

        if (fakeRoleData2 == null)
            fakeRoleData2 = GetRandomNonRepeatedFakeCharacter(new List<CharacterData>() { c1.dataRef, c1.bluff, c2.dataRef, c2.bluff, fakeRoleData1 });

        info = ConjourInfo(new List<int>() { c1.id, c2.id }, new List<CharacterData>() { fakeRoleData1, fakeRoleData2 }, false);
        onActed?.Invoke(new ActedInfo(info, pickedCharacters));
        Debug.Log($"{info}");
    }

    public CharacterData GetRandomNonRepeatedFakeCharacter(List<CharacterData> nonRepeatList)
    {
        List<CharacterData> availableCharacters = new List<CharacterData>();

        foreach (Character c in Gameplay.CurrentCharacters)
        {
            if (!nonRepeatList.Contains(c.dataRef) && !availableCharacters.Contains(c.dataRef))
                availableCharacters.Add(c.dataRef);

            if (c.bluff != null && !nonRepeatList.Contains(c.bluff) && !availableCharacters.Contains(c.bluff))
                availableCharacters.Add(c.bluff);
        }

        return availableCharacters[UnityEngine.Random.Range(0, availableCharacters.Count)];
    }

    public string ConjourInfo(List<int> ids, List<CharacterData> names, bool isCabbage)
    {
        ids = ids
            .OrderBy(i => i)
            .ThenBy(_ => UnityEngine.Random.value)
            .ToList();

        if (names != null)
            names = names
                .OrderBy(i => UnityEngine.Random.value)
                .ToList();

        string translation = TryLocalize(dataRef,
            ctx => ctx
            .SetRaw("characterName", names == null ? null : names[0])
            .SetRaw("characterName2", names == null ? null : names[1])
            .SetIds("ids", ids)
            .SetBool("iscabbage", isCabbage)
            );
        if (!String.IsNullOrEmpty(translation)) return translation;

        string info = $"Among\n#{ids[0]}, #{ids[1]}\nthere is:\n";
        if (isCabbage)
            info += $"a Cabbage";
        else
            info += $"{names[0].GetCharacterName()} or {names[1].GetCharacterName()}";

        return info;
    }
}
[System.Serializable]
public class DreamerOld : Role
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
        CharacterPicker.Instance.StartPickCharacters(1, charRef);
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

        string role = "";
        if (c.dataRef.role is Recluse)
        {
            role = "\na Cabbage";
        }
        else if (c.GetRegisterAlignment() == EAlignment.Evil)
            role = $"{c.GetCharacterData().GetCharacterName()}";
        else
        {
            List<CharacterData> evilCharacters = new List<CharacterData>(Gameplay.Instance.GetScriptCharacters());
            evilCharacters = Characters.Instance.FilterAlignmentCharacters(evilCharacters, EAlignment.Evil);
            CharacterData pickedCh = evilCharacters[UnityEngine.Random.Range(0, evilCharacters.Count)];
            role = $"{pickedCh.GetCharacterName()}";
        }

        string info = "";
        info = ConjourInfo(c.id, role);
        onActed?.Invoke(new ActedInfo(info, pickedCharacters));
        Debug.Log($"{info}");
    }

    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        CharacterPicker.Instance.StartPickCharacters(1, charRef);
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

        string info = "";
        info = ConjourInfo(c.id, pickedCh.GetCharacterName());
        onActed?.Invoke(new ActedInfo(info));
        Debug.Log($"{info}");
    }

    public string ConjourInfo(int id, string roleName)
    {
        string info = $"#{id} could be: ";
        info += $"{roleName}";

        return info;
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
        CharacterPicker.Instance.StartPickCharacters(2, charRef);
        CharacterPicker.OnCharactersPicked += CharacterPicked;
        CharacterPicker.OnStopPick += StopPick;
    }
    private void CharacterPicked()
    {
        CharacterPicker.OnCharactersPicked -= CharacterPicked;
        CharacterPicker.OnStopPick -= StopPick;

        Character c1 = CharacterPicker.PickedCharacters[0];
        Character c2 = CharacterPicker.PickedCharacters[1];

        List<ECharacterType> charTypes = new List<ECharacterType>()
        {
            ECharacterType.Demon,
            ECharacterType.Minion,
            ECharacterType.Outcast,
            ECharacterType.Villager
        };

        charTypes.Remove(c1.GetCharacterType());
        charTypes.Remove(c2.GetCharacterType());

        ECharacterType pickedRole = charTypes[UnityEngine.Random.Range(0, charTypes.Count)];

        string info = ConjourInfo(c1, c2, StringHelper.GetCharacterTypeName(pickedRole));

        onActed?.Invoke(new ActedInfo(info, new List<Character>() { c1, c2 }));
        Debug.Log($"{info}");
    }

    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        CharacterPicker.Instance.StartPickCharacters(2, charRef);
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

        List<Character> chs = new List<Character>() { CharacterPicker.PickedCharacters[0], CharacterPicker.PickedCharacters[1] };

        List<ECharacterType> charTypes = new List<ECharacterType>();

        foreach (Character ch in chs)
            if (ch.dataRef.usuallyDisguised)
                charTypes.Add(ch.GetCharacterType());

        if (charTypes.Count == 0)
            foreach (Character ch in chs)
                charTypes.Add(ch.GetCharacterType());

        ECharacterType pickedRole = charTypes[UnityEngine.Random.Range(0, charTypes.Count)];

        string info = ConjourInfo(chs[0], chs[1], StringHelper.GetCharacterTypeName(pickedRole));

        onActed?.Invoke(new ActedInfo(info, new List<Character>() { chs[0], chs[1] }));
        Debug.Log($"{info}");
    }

    public string ConjourInfo(Character c1, Character c2, string roleName)
    {
        string info = $"#{c1.id} and #{c2.id}:\nNone of them is {roleName}";
        return info;
    }
}

[System.Serializable]
public class InvestigatorNew : Role // Investigator
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
        CharacterPicker.Instance.StartPickCharacters(2, charRef);
        CharacterPicker.OnCharactersPicked += CharacterPicked;
        CharacterPicker.OnStopPick += StopPick;
    }
    private void CharacterPicked()
    {
        CharacterPicker.OnCharactersPicked -= CharacterPicked;
        CharacterPicker.OnStopPick -= StopPick;

        Character c1 = CharacterPicker.PickedCharacters[0];
        Character c2 = CharacterPicker.PickedCharacters[1];

        List<ECharacterType> charTypes = new List<ECharacterType>()
        {
            ECharacterType.Demon,
            ECharacterType.Minion,
            ECharacterType.Outcast,
            ECharacterType.Villager
        };

        ECharacterType type = ECharacterType.Villager;
        foreach (ECharacterType ct in charTypes)
        {
            if (c1.GetCharacterType() == ECharacterType.Demon || c2.GetCharacterType() == ECharacterType.Demon)
            { type = ECharacterType.Demon; break; }
            if (c1.GetCharacterType() == ECharacterType.Minion || c2.GetCharacterType() == ECharacterType.Minion)
            { type = ECharacterType.Minion; break; }
            if (c1.GetCharacterType() == ECharacterType.Outcast || c2.GetCharacterType() == ECharacterType.Outcast)
            { type = ECharacterType.Outcast; break; }
        }

        //string info = ConjourInfo(c1, c2, type.ToString());
        string info = ConjourInfo(c1, c2, type);

        onActed?.Invoke(new ActedInfo(info, new List<Character>() { c1, c2 }));
        Debug.Log($"{info}");
    }

    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        CharacterPicker.Instance.StartPickCharacters(2, charRef);
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

        Character c1 = CharacterPicker.PickedCharacters[0];
        Character c2 = CharacterPicker.PickedCharacters[1];

        List<ECharacterType> charTypes = new List<ECharacterType>()
        {
            ECharacterType.Demon,
            ECharacterType.Minion,
            ECharacterType.Outcast,
            ECharacterType.Villager
        };

        ECharacterType type = ECharacterType.Villager;
        foreach (ECharacterType ct in charTypes)
        {
            if (c1.GetCharacterType() == ECharacterType.Demon || c2.GetCharacterType() == ECharacterType.Demon)
            { type = ECharacterType.Demon; break; }
            if (c1.GetCharacterType() == ECharacterType.Minion || c2.GetCharacterType() == ECharacterType.Minion)
            { type = ECharacterType.Minion; break; }
            if (c1.GetCharacterType() == ECharacterType.Outcast || c2.GetCharacterType() == ECharacterType.Outcast)
            { type = ECharacterType.Outcast; break; }
        }

        charTypes.Remove(type);
        charTypes.Remove(c1.GetCharacterType());
        charTypes.Remove(c2.GetCharacterType());

        //if (c1.GetRegisterAlignment() == EAlignment.Good && c2.GetRegisterAlignment() == EAlignment.Good)
        //{ charTypes.Remove(ECharacterType.Outcast); charTypes.Remove(ECharacterType.Villager); }

        if (type == ECharacterType.Outcast || type == ECharacterType.Villager)
        { charTypes.Remove(ECharacterType.Outcast); charTypes.Remove(ECharacterType.Villager); }
        else
        {
            List<CharacterData> chars = Gameplay.Instance.GetAllCurrentCharacters();
            chars = Characters.Instance.FilterCharacterType(chars, ECharacterType.Outcast);
            chars = Characters.Instance.FilterDisguisedCharacters(chars);
            if (chars.Count == 0)
                charTypes.Remove(ECharacterType.Outcast);
        }

        charTypes.Reverse();

        foreach (ECharacterType ct in charTypes)
        {
            if (ct == ECharacterType.Villager)
            { type = ECharacterType.Villager; break; }
            if (ct == ECharacterType.Outcast)
            { type = ECharacterType.Outcast; break; }
            type = charTypes[UnityEngine.Random.Range(0, charTypes.Count)];
            break;
        }

        string info = ConjourInfo(c1, c2, type);
        //string info = ConjourInfo(c1, c2, type.ToString());

        onActed?.Invoke(new ActedInfo(info, new List<Character>() { c1, c2 }));
        Debug.Log($"{info}");
    }

    public string ConjourInfo(Character c1, Character c2, ECharacterType charType)
    {
        string translation = TryLocalize(dataRef,
            ctx => ctx
            .SetIds("ids", new List<int>() { c1.id, c2.id })
            .SetKeywords("keywords", new List<string>() { charType.ToString() })
            );
        if (!String.IsNullOrEmpty(translation)) return translation;

        string info = $"#{c1.id} or #{c2.id}:\nis {StringHelper.GetCharacterTypeName(charType)}";
        //string info = $"{roleName} is more Evil between\n#{c1.id} and #{c2.id}";
        return info;
    }
}
[System.Serializable]
public class InvestigatorNew2 : Role // Investigator
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
        CharacterPicker.Instance.StartPickCharacters(2, charRef);
        CharacterPicker.OnCharactersPicked += CharacterPicked;
        CharacterPicker.OnStopPick += StopPick;
    }
    private void CharacterPicked()
    {
        CharacterPicker.OnCharactersPicked -= CharacterPicked;
        CharacterPicker.OnStopPick -= StopPick;

        List<Character> characters = new List<Character>() { CharacterPicker.PickedCharacters[0], CharacterPicker.PickedCharacters[1] };

        characters = ListHelper.ShuffleList<Character>(characters);

        List<ECharacterType> suspicions = new List<ECharacterType>()
        {
            ECharacterType.Demon,
            ECharacterType.Minion,
            ECharacterType.Outcast,
            ECharacterType.Villager
        };

        Character pickedCharacter = null;

        foreach (Character c in characters)
            if (c.GetCharacterType() == ECharacterType.Demon)
                pickedCharacter = c;
        if (pickedCharacter == null)
            foreach (Character c in characters)
                if (c.GetCharacterType() == ECharacterType.Minion)
                    pickedCharacter = c;
        if (pickedCharacter == null)
            foreach (Character c in characters)
                if (c.GetCharacterType() == ECharacterType.Outcast)
                    pickedCharacter = c;
        if (pickedCharacter == null)
            pickedCharacter = characters[0];

        characters = characters
            .OrderBy(c => c.id)
            .ThenBy(_ => UnityEngine.Random.value)
            .ToList();

        string info = ConjourInfo(characters[0], characters[1], pickedCharacter);

        Debug.Log(characters[0]);
        Debug.Log(characters[1]);

        onActed?.Invoke(new ActedInfo(info, new List<Character>() { characters[0], characters[1] }));
        Debug.Log($"{info}");
    }

    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        CharacterPicker.Instance.StartPickCharacters(2, charRef);
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

        List<Character> characters = new List<Character>() { CharacterPicker.PickedCharacters[0], CharacterPicker.PickedCharacters[1] };

        characters = ListHelper.ShuffleList<Character>(characters);

        List<ECharacterType> suspicions = new List<ECharacterType>()
        {
            ECharacterType.Demon,
            ECharacterType.Minion,
            ECharacterType.Outcast,
            ECharacterType.Villager
        };

        Character pickedCharacter = null;

        foreach (Character c in characters)
            if (c.GetCharacterType() == ECharacterType.Demon)
                pickedCharacter = c;
        if (pickedCharacter == null)
            foreach (Character c in characters)
                if (c.GetCharacterType() == ECharacterType.Minion)
                    pickedCharacter = c;
        if (pickedCharacter == null)
            foreach (Character c in characters)
                if (c.GetCharacterType() == ECharacterType.Outcast)
                    pickedCharacter = c;
        if (pickedCharacter == null)
            pickedCharacter = characters[0];

        characters = characters
            .OrderBy(c => c.id)
            .ThenBy(_ => UnityEngine.Random.value)
            .ToList();

        string info = ConjourInfo(characters[0], characters[1], pickedCharacter);

        onActed?.Invoke(new ActedInfo(info, new List<Character>() { characters[0], characters[1] }));
        Debug.Log($"{info}");
    }

    public string ConjourInfo(Character c1, Character c2, Character character)
    {
        string info = $"Between\n#{c1.id} and #{c2.id}:\n#{character.id} is more Suspicious";
        return info;
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
        CharacterPicker.Instance.StartPickCharacters(1, charRef);
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

        if (chars[0].GetRegisterAlignment() == EAlignment.Evil)
        {
            if (chRef.statuses.Contains(ECharacterStatus.Lying))
                info = ConjourInfo(chars[0].id, EAlignment.Good, chRef);
            else
                info = ConjourInfo(chars[0].id, EAlignment.Evil, chRef);
            shouldExecute = true;
        }
        else
            info = ConjourInfo(chars[0].id, EAlignment.Good, chRef);

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
        CharacterPicker.Instance.StartPickCharacters(1, charRef);
        if (charRef.statuses.Contains(ECharacterStatus.WorkingAbility))
            CharacterPicker.OnCharactersPicked += CharacterPicked;
        else
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

        string info = ConjourInfo(chars[0].id, EAlignment.Good, chRef);
        onActed?.Invoke(new ActedInfo(info, chars));
        Debug.Log($"{info}");
    }

    public string ConjourInfo(int id, EAlignment alignment, Character charRef)
    {
        string translation = TryLocalize(dataRef,
            ctx => ctx
            .SetIds("ids", new List<int>() { id })
            .SetBool("killed", alignment == EAlignment.Evil ? true : false));
        if (!String.IsNullOrEmpty(translation)) return translation;

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
    Character chRef;
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
        chRef = charRef;
        CharacterPicker.Instance.StartPickCharacters(2, charRef);
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
            if (c.GetRegisterAlignment() == EAlignment.Evil)
                isEvil = true;
        }

        List<Character> chars = new List<Character>(CharacterPicker.PickedCharacters);
        chars = chars
            .OrderBy(c => c.id)
            .ThenBy(_ => UnityEngine.Random.value)
            .ToList();

        string info = ConjourInfo(chars[0].id, chars[1].id, isEvil);
        ActedInfo actedInfo = new ActedInfo(info, chars);
        onActed?.Invoke(actedInfo);
        Debug.Log($"{info}");

        CheckAchievementsAndUnlockIfAble(actedInfo);
    }
    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        chRef = charRef;
        CharacterPicker.Instance.StartPickCharacters(2, charRef);
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
            if (c.GetRegisterAlignment() == EAlignment.Evil)
                isEvil = false;
        }

        List<Character> chars = new List<Character>(CharacterPicker.PickedCharacters);
        chars = chars
            .OrderBy(c => c.id)
            .ThenBy(_ => UnityEngine.Random.value)
            .ToList();

        string info = ConjourInfo(chars[0].id, chars[1].id, isEvil);
        onActed?.Invoke(new ActedInfo(info, chars));
        Debug.Log($"{info}");
    }

    public string ConjourInfo(int id, int id2, bool isEvil)
    {
        string translation = TryLocalize(dataRef,
            ctx => ctx
            .SetIds("ids", new List<int>() { id, id2 })
            .SetBool("isEvil", isEvil));
        if (!String.IsNullOrEmpty(translation)) return translation;

        string info = $"";

        if (!isEvil)
            info = $"Is #{id} or #{id2} Evil?: False";
        else
            info = $"Is #{id} or #{id2} Evil?: True";

        return info;
    }

    //ACHIEVEMENTS
    private void CheckAchievementsAndUnlockIfAble(ActedInfo info)
    {
        if (info.characters[0].GetRegisterAlignment() == EAlignment.Evil && info.characters[1].GetRegisterAlignment() == EAlignment.Evil)
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


        string info = ConjourInfo(pickedCh[0].id, pickedCh[0].GetCharacterData(), charRef);
        ActedInfo newInfo = new ActedInfo(info, pickedCh);
        return newInfo;
    }

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger == ETriggerPhase.Day)
            onActed?.Invoke(GetInfo(charRef));

    }
    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger == ETriggerPhase.Day)
            onActed?.Invoke(GetBluffInfo(charRef));

        if (trigger == ETriggerPhase.OnExecuted)
            if (!charRef.statuses.Contains(ECharacterStatus.HealthyBluff))
                CheckAchievementsAndUnlockIfAble();
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

        string info = ConjourInfo(pickedCh[0].id, pickedCh[0].bluff, charRef);
        ActedInfo newInfo = new ActedInfo(info, pickedCh);
        return newInfo;
    }

    public string ConjourInfo(int id, CharacterData ch, Character charef)
    {
        bool surprised = false;
        if (ch.role is Drunk || ch.role is Doppleganger || ch.role is Lycanthrope)
            surprised = true;

        string translation = TryLocalize(dataRef,
            ctx => ctx
            .SetRaw("characterName", ch)
            .SetIds("ids", new List<int>() { id })
            .SetBool("surprised", surprised)
            );
        if (!String.IsNullOrEmpty(translation)) return translation;

        string info = "";
        if (surprised)
            info += $"#{id} is actually a\n";
        else
            info += $"#{id} is a real\n";
        info += $"{ch.GetCharacterName()}";

        return info;
    }

    //ACHIEVEMENTS
    private void CheckAchievementsAndUnlockIfAble()
    {
        ProjectContext.UnlockAchievement("Medium_Halloween_ACHIV_6997");
    }
}
[System.Serializable]
public class Noble : Role // Empress :
{
    public override string Description
        => $"Learn 3 players. Only 1 is Evil";

    public override ActedInfo GetInfo(Character charRef)
    {
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

        string info = ConjourInfo(picked[0].id, picked[1].id, picked[2].id, charRef);
        ActedInfo newInfo = new ActedInfo(info, picked);

        return newInfo;
    }

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger == ETriggerPhase.Day)
            onActed?.Invoke(GetInfo(charRef));

        if (trigger == ETriggerPhase.OnExecuted)
            if (charRef.GetRegisterAlignment() == EAlignment.Good)
                CheckAchievementsAndUnlockIfAble(charRef);
    }
    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger == ETriggerPhase.Day)
            onActed?.Invoke(GetBluffInfo(charRef));

        if (trigger == ETriggerPhase.OnExecuted)
            if (charRef.GetRegisterAlignment() == EAlignment.Good)
                CheckAchievementsAndUnlockIfAble(charRef);
    }
    public override ActedInfo GetBluffInfo(Character charRef)
    {
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

        string info = ConjourInfo(picked[0].id, picked[1].id, picked[2].id, charRef);
        ActedInfo newInfo = new ActedInfo(info, picked);

        return newInfo;
    }

    public string ConjourInfo(int id, int id2, int id3, Character charRef)
    {
        //string localization = TryLocalize<AlchemistLoc>(new List<object>() { howManyCures });
        //if (!string.IsNullOrEmpty(localization))
        //return localization;

        string translation = TryLocalize(dataRef,
            ctx => ctx
            .SetIds("ids", new List<int>() { id, id2, id3 })
            );
        if (!String.IsNullOrEmpty(translation)) return translation;

        string info = $"One is Evil:\n#{id}, #{id2} or #{id3}";

        return info;
    }

    //ACHIEVEMENTS
    private void CheckAchievementsAndUnlockIfAble(Character charRef)
    {
        //if (charRef.GetAlignment() == EAlignment.Evil)
        ProjectContext.UnlockAchievement("Empress_Halloween_ACHIV_8451");
    }
}
[System.Serializable]
public class Archivist : Role // Gemcrafter :
{
    public override string Description
        => $"Learn 1 Good character";

    public override ActedInfo GetInfo(Character charRef)
    {
        List<Character> good = new List<Character>(Gameplay.CurrentCharacters);
        good = Characters.Instance.FilterAlignmentCharacters(good, EAlignment.Good);

        if (good.Count > 1)
            good.Remove(charRef);

        List<Character> pick = new List<Character>();
        pick.Add(good[UnityEngine.Random.Range(0, good.Count)]);

        string info = ConjourInfo(pick[0].id, charRef);
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
        List<Character> evils = new List<Character>(Gameplay.CurrentCharacters);
        evils = Characters.Instance.FilterAlignmentCharacters(evils, EAlignment.Evil);

        if (evils.Count > 1)
            evils.Remove(charRef);

        List<Character> pick = new List<Character>();
        pick.Add(evils[UnityEngine.Random.Range(0, evils.Count)]);

        string info = ConjourInfo(pick[0].id, charRef);
        ActedInfo newInfo = new ActedInfo(info, pick);
        return newInfo;
    }
    public string ConjourInfo(int id, Character charRef)
    {
        string translation = TryLocalize(dataRef,
            ctx => ctx
            .SetIds("ids", new List<int>() { id })
            );

        if (!String.IsNullOrEmpty(translation)) return translation;

        string info = $"#{id} is Good";

        return info;
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

        List<int> ids = new List<int>();
        foreach (Character c in pickedCharacters)
            ids.Add(c.id);

        pickedCharacters = pickedCharacters.OrderBy(x => random.Next()).ToList();
        pickedCharacters = ListHelper.ShuffleList(pickedCharacters);

        List<ECharacterType> types = new List<ECharacterType>();
        foreach (Character c in pickedCharacters)
            types.Add(c.GetCharacterData().type);

        string info = ConjourInfo(ids, types, charRef);
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

        //if (allCharacters.Count > 0)
        //{
        if (allCharacters.Count > 0)
        {
            picked = allCharacters[UnityEngine.Random.Range(0, allCharacters.Count)];
            pickedCharacters.Add(picked);
            allCharacters.Remove(picked);
        }
        //}

        if (allCharacters.Count > 0)
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

        List<int> ids = new List<int>();
        foreach (Character c in pickedCharacters)
            ids.Add(c.id);

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

        pickedCharacters = ListHelper.ShuffleList(pickedCharacters);

        List<ECharacterType> types = new List<ECharacterType>();
        foreach (ECharacterType ct in possiblePicks)
            types.Add(ct);

        string info = ConjourInfo(ids, types, charRef);
        List<Character> chars = new List<Character>(pickedCharacters);
        ActedInfo newInfo = new ActedInfo(info, chars);
        return newInfo;
    }

    public string ConjourInfo(List<int> ids, List<ECharacterType> characters, Character charRef)
    {
        List<string> keywords = new List<string>();
        foreach (ECharacterType ct in characters)
            keywords.Add(ct.ToString());

        string translation = TryLocalize(dataRef,
            ctx => ctx
            .SetIds("ids", ids)
            .SetKeywords("keywords", keywords)
            );
        if (!String.IsNullOrEmpty(translation)) return translation;

        string info = "Between\n";

        if (ids.Count == 2)
            info += $"#{ids[0]}, #{ids[1]}";
        if (ids.Count == 3)
            info += $"#{ids[0]}, #{ids[1]}, #{ids[2]}";
        if (ids.Count == 1)
        {
            info = $"#{ids[0]} is a {characters[0].ToString()}";
            return info;
        }

        info += "\nthere is:\n";

        if (characters.Count == 2)
            info += $"{characters[0].ToString()} and {characters[1].ToString()}";
        if (characters.Count == 3)
            info += $"{characters[0].ToString()}, {characters[1].ToString()} and {characters[2].ToString()}";

        return info;
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

        string info = ConjourInfo(dir, charRef);
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

        string info = ConjourInfo(fakeDirection, charRef);
        charRef.CreateRuntimeData(new EnlightenedRuntimeData(fakeDirection));

        ActedInfo newInfo = new ActedInfo(info);
        return newInfo;
    }

    public enum EEvilDirection
    {
        Clockwise = 0,
        Counterclockwise = 1,
        Either = 2,
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
            if (c.GetRegisterAlignment() == EAlignment.Evil)
                break;
        }
        foreach (Character c in clockwise)
        {
            clockwiseNumber++;
            if (c.GetRegisterAlignment() == EAlignment.Evil)
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

    public string ConjourInfo(EEvilDirection direction, Character charRef)
    {
        string translation = TryLocalize(dataRef,
            ctx => ctx
            .SetCustom("clock", (int)direction)
            );
        if (!String.IsNullOrEmpty(translation)) return translation;

        string dir = "";
        if (direction == EEvilDirection.Clockwise)
            dir = "Clockwise";
        if (direction == EEvilDirection.Counterclockwise)
            dir = "Counter-clockwise";

        string info = $"Closest Evil is:\n{dir}";
        if (direction == EEvilDirection.Either)
            info = "Closest Evil is equidistant";

        return info;
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

        string info = ConjourInfo(distance, charRef);

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

        string info = ConjourInfo(randomDistance, charRef);

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
            if (c.GetRegisterAlignment() == EAlignment.Evil)
                break;
        }
        foreach (Character c in clockwise)
        {
            clockwiseNumber++;
            if (c.GetRegisterAlignment() == EAlignment.Evil)
                break;
        }

        if (clockwiseNumber > counterClockwiseNumber)
            return counterClockwiseNumber;
        if (clockwiseNumber < counterClockwiseNumber)
            return clockwiseNumber;

        return clockwiseNumber;
    }

    public string ConjourInfo(int distance, Character charRef)
    {
        string translation = TryLocalize(dataRef,
            ctx => ctx
            .SetRaw("n", distance)
            );
        if (!String.IsNullOrEmpty(translation)) return translation;

        string info = "";
        if (distance == 1)
            info = $"I am {distance} card away from closest Evil";
        else
            info = $"I am {distance} cards away from closest Evil";

        return info;
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

        evils.RemoveAll(c => c.role is Recluse);

        string info = ConjourInfo(0, 0, null, charRef, true);
        ActedInfo newInfo = new ActedInfo(info);

        if (evils.Count == 0)
            return newInfo;

        List<Character> goods = new List<Character>(Gameplay.CurrentCharacters);
        goods = Characters.Instance.FilterAlignmentCharacters(goods, EAlignment.Good);
        goods.Remove(charRef);

        Character evil = evils[UnityEngine.Random.Range(0, evils.Count)];

        pickedCharacters.Add(evil);
        pickedCharacters.Add(goods[UnityEngine.Random.Range(0, goods.Count)]);

        pickedCharacters = pickedCharacters
            .OrderBy(c => c.id)
            .ThenBy(_ => UnityEngine.Random.value)
            .ToList();

        info = ConjourInfo(pickedCharacters[0].id, pickedCharacters[1].id, evil.GetCharacterData(), charRef);
        newInfo = new ActedInfo(info, pickedCharacters);
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

        string info = ConjourInfo(pickedCharacters[0].id, pickedCharacters[1].id, minion, charRef);
        ActedInfo newInfo = new ActedInfo(info, pickedCharacters);
        return newInfo;
    }

    public string ConjourInfo(int id1, int id2, CharacterData minionData, Character charRef, bool noMinions = false)
    {
        string translation = TryLocalize(dataRef,
            ctx => ctx
            .SetRaw("characterName", minionData)
            .SetIds("ids", new List<int>() { id1, id2 })
            .SetBool("nominions", noMinions)
            );
        if (!String.IsNullOrEmpty(translation)) return translation;

        if (noMinions)
            return $"There are no minions";

        string info = $"#{id1} or #{id2} is a {minionData.GetCharacterName()}";
        return info;
    }
}
[System.Serializable]
public class Confessor : Role
{
    public override string Description
        => "Can not lie, even if I am Evil";

    public override ActedInfo GetInfo(Character charRef)
    {
        bool dizzy = false;

        if (charRef.statuses.statuses.Contains(ECharacterStatus.Corrupted))
            dizzy = true;
        if (charRef.GetRegisterAlignment() == EAlignment.Evil)
            dizzy = true;

        if (charRef.dataRef.role is Spy)
            dizzy = false;

        string info = ConjourInfo(dizzy, charRef);
        ManageArt(charRef, dizzy);
        ActedInfo newInfo = new ActedInfo(info);
        return newInfo;
    }

    public override void OnInit(Character charRef)
    {
        charRef.statuses.AddStatus(ECharacterStatus.AppearTruthfull, charRef);
    }

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger == ETriggerPhase.Init) OnInit(charRef);

        if (trigger != ETriggerPhase.Day) return;
        onActed?.Invoke(GetInfo(charRef));
    }
    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger == ETriggerPhase.Init) OnInit(charRef);

        if (trigger != ETriggerPhase.Day) return;
        onActed?.Invoke(GetBluffInfo(charRef));
    }
    public override ActedInfo GetBluffInfo(Character charRef)
    {
        bool dizzy = false;

        if (charRef.statuses.statuses.Contains(ECharacterStatus.Corrupted))
            dizzy = true;
        if (charRef.GetRegisterAlignment() == EAlignment.Evil)
            dizzy = true;

        if (charRef.dataRef.role is Spy)
            dizzy = false;

        string info = ConjourInfo(dizzy, charRef);
        ManageArt(charRef, dizzy);
        ActedInfo newInfo = new ActedInfo(info);
        return newInfo;
    }

    private void ManageArt(Character charRef, bool dizzy)
    {
        if (dizzy)
        {
            charRef.ShowAnimatedArt();
        }
    }

    public string ConjourInfo(bool dizzy, Character charRef)
    {
        string translation = TryLocalize(dataRef,
            ctx => ctx
            .SetBool("dizzy", dizzy)
            );
        if (!String.IsNullOrEmpty(translation)) return translation;

        string info = "I am Good";
        if (dizzy) info = "I am dizzy";
        return info;
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
        CharacterPicker.Instance.StartPickCharacters(1, charRef);
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
        CharacterPicker.Instance.StartPickCharacters(1, charRef);
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

        string info = ConjourInfo(howFar, charRef);
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
        if (trigger == ETriggerPhase.Day)
            onActed?.Invoke(GetBluffInfo(charRef));

        if (trigger == ETriggerPhase.OnExecuted)
            if (!charRef.statuses.Contains(ECharacterStatus.HealthyBluff))
                CheckAchievementsAndUnlockIfAble();
    }

    public override ActedInfo GetBluffInfo(Character charRef)
    {
        int howFar = GetClosestPoisonedCharacter(charRef);

        int randomHowFar = Calculator.RemoveNumberAndGetRandomNumberFromList(howFar, 0, 4);
        List<Character> chars = Characters.Instance.GetCharactersAtRange(randomHowFar, charRef);

        string info = ConjourInfo(randomHowFar, charRef);

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

    public string ConjourInfo(int howFar, Character charRef)
    {
        string translation = TryLocalize(dataRef,
            ctx => ctx
            .SetRaw("n", howFar)
            );
        if (!String.IsNullOrEmpty(translation)) return translation;

        string info = "";
        if (howFar == 0)
            info = "There are no Corrupted characters";
        else if (howFar == 1)
            info = "I am 1 card away from Corrupted character";
        else
            info = $"I am {howFar} cards away from Corrupted character";

        return info;
    }

    //ACHIEVEMENTS
    private void CheckAchievementsAndUnlockIfAble()
    {
        ProjectContext.UnlockAchievement("Bard_Halloween_ACHIV_6761");
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
        CharacterPicker.Instance.StartPickCharacters(1, charRef);
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
        CharacterPicker.Instance.StartPickCharacters(1, charRef);
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
    Character charRef;
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
        this.charRef = charRef;
        CharacterPicker.Instance.StartPickCharacters(1, charRef);
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
            isLying = CharacterHelper.CheckLyingAppearance(c);
        }

        string info = $"{ConjourInfo(CharacterPicker.PickedCharacters[0], isLying)}";

        List<Character> chars = new List<Character>(CharacterPicker.PickedCharacters);
        onActed?.Invoke(new ActedInfo(info, chars));
        Debug.Log($"{info}");
    }

    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        this.charRef = charRef;
        CharacterPicker.Instance.StartPickCharacters(1, charRef);
        CharacterPicker.OnCharactersPicked += CharacterPickedDrunk;
        CharacterPicker.OnStopPick += StopPick;
    }
    private void CharacterPickedDrunk()
    {
        CharacterPicker.OnCharactersPicked -= CharacterPickedDrunk;
        CharacterPicker.OnStopPick -= StopPick;

        bool isLying = false;
        foreach (Character c in CharacterPicker.PickedCharacters)
            isLying = CharacterHelper.CheckLyingAppearance(c);

        isLying = !isLying;
        string info = $"{ConjourInfo(CharacterPicker.PickedCharacters[0], isLying)}";

        List<Character> chars = new List<Character>(CharacterPicker.PickedCharacters);
        onActed?.Invoke(new ActedInfo(info, chars));
        Debug.Log($"{info}");
    }

    public string ConjourInfo(Character character, bool isLying)
    {
        string translation = TryLocalize(dataRef,
            ctx => ctx
            .SetBool("lying", isLying)
            .SetIds("ids", new List<int>() { character.id })
            );
        if (!String.IsNullOrEmpty(translation)) return translation;

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
    Character chRef;

    public override string Description
        => "Pick 2 players. Learn which Outsider is among them (if any)";

    string drunkId = "Drunk_15369527";

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
        CharacterPicker.Instance.StartPickCharacters(3, charRef);
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

        string info = ConjourInfo(ids[0], ids[1], ids[2], null);

        if (outsiders.Count > 0)
        {
            Character randomOutsider = outsiders[UnityEngine.Random.Range(0, outsiders.Count)];

            info = ConjourInfo(ids[0], ids[1], ids[2], randomOutsider.GetCharacterData());
        }

        List<Character> chars = new List<Character>(CharacterPicker.PickedCharacters);
        onActed?.Invoke(new ActedInfo(info, chars));
        Debug.Log($"{info}");
    }

    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        chRef = charRef;
        CharacterPicker.Instance.StartPickCharacters(3, charRef);
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
            info = ConjourInfo(ids[0], ids[1], ids[2], null);
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
                CharacterData drunkData = ProjectContext.Instance.gameData.GetCharacterDataOfId(drunkId);
                info = ConjourInfo(ids[0], ids[1], ids[2], drunkData);
            }
            else
            {
                CharacterData randomOutsider = pickedOutsiders[UnityEngine.Random.Range(0, pickedOutsiders.Count)];
                info = ConjourInfo(ids[0], ids[1], ids[2], randomOutsider);
            }
        }

        List<Character> chars = new List<Character>(CharacterPicker.PickedCharacters);

        onActed?.Invoke(new ActedInfo(info, chars));
        Debug.Log($"{info}");
    }

    public string ConjourInfo(int id1, int id2, int id3, CharacterData cd)
    {
        string translation = TryLocalize(dataRef,
            ctx => ctx
            .SetBool("outcast", cd == null ? true : false)
            .SetIds("ids", new List<int>() { id1, id2, id3 })
            .SetRaw("characterName", cd)
            );
        if (!String.IsNullOrEmpty(translation)) return translation;

        string info = $"";

        if (cd == null)
            info = $"Among #{id1}, #{id2}, #{id3}\nthere are NO Outcasts";
        else
            info = $"Among #{id1}, #{id2}, #{id3}\nthere is: {cd.GetCharacterName()}";

        return info;
    }
}
[System.Serializable]
public class Juggler : Role // Jester :
{
    Character chRef;
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
        chRef = charRef;
        CharacterPicker.Instance.StartPickCharacters(3, charRef);
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
            if (c.GetRegisterAlignment() == EAlignment.Evil)
                evils++;

            ids.Add(c.id);
        }

        ids = ids
            .OrderBy(c => c)
            .ThenBy(_ => UnityEngine.Random.value)
            .ToList();

        string info = ConjourInfo(ids[0], ids[1], ids[2], evils);

        List<Character> chars = new List<Character>(CharacterPicker.PickedCharacters);

        onActed?.Invoke(new ActedInfo(info, chars));
        Debug.Log($"{info}");
    }

    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        chRef = charRef;
        CharacterPicker.Instance.StartPickCharacters(3, charRef);
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
            if (c.GetRegisterAlignment() == EAlignment.Evil)
                evils++;

            ids.Add(c.id);
        }

        ids = ids
            .OrderBy(c => c)
            .ThenBy(_ => UnityEngine.Random.value)
            .ToList();

        int townsfolks = 0;
        townsfolks = Calculator.RemoveNumberAndGetRandomNumberFromList(evils, 0, 4);

        string info = ConjourInfo(ids[0], ids[1], ids[2], townsfolks);

        List<Character> chars = new List<Character>(CharacterPicker.PickedCharacters);

        onActed?.Invoke(new ActedInfo(info, chars));
        Debug.Log($"{info}");
    }

    public string ConjourInfo(int id1, int id2, int id3, int evilsAmount)
    {
        string translation = TryLocalize(dataRef,
            ctx => ctx
            .SetIds("ids", new List<int>() { id1, id2, id3 })
            .SetRaw("n", evilsAmount)
            );
        if (!String.IsNullOrEmpty(translation)) return translation;

        string info = $"Among:\n#{id1}, #{id2}, #{id3}:\nThere are {evilsAmount} Evils";
        if (evilsAmount == 1)
            info = $"Among:\n#{id1}, #{id2}, #{id3}:\nThere is {evilsAmount} Evil";

        return info;
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

        return notInPlayCh[UnityEngine.Random.Range(0, notInPlayCh.Count)];
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
        if (trigger == ETriggerPhase.Start)
            charRef.statuses.AddStatus(ECharacterStatus.Corrupted, charRef, charRef);
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
        //CharacterData bluff = Characters.Instance.GetRandomDuplicateBluff();
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
public class Doppleganger : Role // Doppelganger
{
    public override string Description
        => "[I am a Good Villager role currently in play]";

    public override ActedInfo GetInfo(Character charRef)
    {
        return new ActedInfo("");
    }

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        //if (trigger == ETriggerPhase.Init)
        //charRef.statuses.AddStatus(ECharacterStatus.HealthyBluff, charRef);

        if (trigger == ETriggerPhase.OnDied)
        {
        }
    }

    public override CharacterData GetBluffIfAble(Character charRef)
    {
        if (!charRef.statuses.Contains(ECharacterStatus.Corrupted))
        {
            OnSpawn(charRef);
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

    public override void OnSpawn(Character charRef)
    {
        charRef.statuses.AddStatus(ECharacterStatus.HealthyBluff, charRef);
    }

    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        //if (trigger == ETriggerPhase.Init)
        //charRef.statuses.AddStatus(ECharacterStatus.HealthyBluff, charRef);
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
            CharacterPicker.Instance.StartPickCharacters(1, charRef);
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
        villagers = Characters.Instance.FilterCharactersWithoutResistance(villagers, ECharacterStatus.Corrupted);

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
        CharacterPicker.Instance.StartPickCharacters(1, charRef);
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
        string translation = TryLocalize(dataRef,
            ctx => ctx
            .SetIds("ids", new List<int>() { evilCharacter != null ? evilCharacter.id : pickedCharacter.id })
            .SetIds("ids2", new List<int>() { evilCharacter != null ? pickedCharacter.id : 0 })
            .SetBool("iscorrupted", evilCharacter == null ? false : true)
            );
        if (!String.IsNullOrEmpty(translation)) return translation;

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

[System.Serializable]
public class Lycanthrope : Role
{
    public string werewolfId = "Werewolf_78350415";
    public override string Description => throw new NotImplementedException();

    public override List<SpecialRule> GetRules() => new List<SpecialRule>()
    {
        new NightModeRule(4),
    };

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (charRef.state == ECharacterState.Dead) return;
        if (trigger == ETriggerPhase.Night)
        {
            if (charRef.state != ECharacterState.Hidden) return;
            if (charRef.state == ECharacterState.Dead) return;

            CharacterData werewolfData = ProjectContext.Instance.gameData.GetCharacterDataOfId(werewolfId);
            CharacterData bluff = charRef.bluff;
            charRef.Init(werewolfData);
            charRef.GiveBluff(bluff);

            //charRef.statuses.RemoveStatusIfAble(ECharacterStatus.HealthyBluff);
            //charRef.statuses.RemoveStatusIfAble(ECharacterStatus.BrokenAbility);
            charRef.statuses.AddStatus(ECharacterStatus.MessedUpByEvil, charRef);
        }
    }

    public override void OnSpawn(Character charRef)
    {
        charRef.statuses.AddStatus(ECharacterStatus.HealthyBluff, charRef);
        //charRef.statuses.AddStatus(ECharacterStatus.BrokenAbility, charRef);
    }

    public override ActedInfo GetInfo(Character charRef)
    {
        return new ActedInfo("");
    }

    public override ActedInfo GetBluffInfo(Character charRef)
    {
        return new ActedInfo("");
    }

    public override CharacterData GetBluffIfAble(Character charRef)
    {
        OnSpawn(charRef);
        //charRef.statuses.AddStatus(ECharacterStatus.BrokenAbility, charRef);
        CharacterData bluff = Characters.Instance.GetRandomUniqueVillagerBluff();
        Gameplay.Instance.AddScriptCharacterIfAble(bluff.type, bluff);

        return bluff;
    }

    public override bool CheckIfCanBeKilled(Character charRef)
    {
        if (charRef.statuses.statuses.Contains(ECharacterStatus.HealthyBluff))
            return charRef.bluff.role.CheckIfCanBeKilled(charRef);
        else
            return true;
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
public class Werewolf : Role
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
        if (trigger == ETriggerPhase.Night)
            if (charRef.state != ECharacterState.Dead)
                PlayerController.PlayerInfo.health.Damage(1);
    }
    public override CharacterData GetBluffIfAble(Character charRef)
    {
        //List<CharacterData> notInPlayCh = Gameplay.Instance.GetScriptCharacters();
        //notInPlayCh = Characters.Instance.FilterAlignmentCharacters(notInPlayCh, EAlignment.Good);
        //notInPlayCh = Characters.Instance.FilterBluffableCharacters(notInPlayCh);
        //return notInPlayCh[UnityEngine.Random.Range(0, notInPlayCh.Count - 1)];

        // Become a new character
        //CharacterData bluff = Characters.Instance.GetRandomUniqueBluff();
        //Gameplay.Instance.AddScriptCharacterIfAble(bluff.type, bluff);

        return null;
        //return charRef.bluff;
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
        chData = notInPlayCh[UnityEngine.Random.Range(0, notInPlayCh.Count)];
        return chData;
    }
    public override CharacterData GetRegisterAsRole(Character charRef)
    {
        if (chData != null)
            return chData;

        List<CharacterData> notInPlayCh = Gameplay.Instance.GetScriptCharacters();
        notInPlayCh = Characters.Instance.FilterCharacterType(notInPlayCh, ECharacterType.Villager);

        chData = notInPlayCh[UnityEngine.Random.Range(0, notInPlayCh.Count)];

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
        viableCharacters = Characters.Instance.FilterCharactersWithoutResistance(viableCharacters, ECharacterStatus.Corrupted);

        if (viableCharacters.Count == 0) return;

        int randomId = UnityEngine.Random.Range(0, viableCharacters.Count);
        Character pickedCharacter = viableCharacters[randomId];

        pickedCharacter.statuses.AddStatus(ECharacterStatus.Corrupted, charRef);
        pickedCharacter.statuses.AddStatus(ECharacterStatus.MessedUpByEvil, charRef);

    }
}
[System.Serializable]
public class Baron : Minion // Chancellor : Counsellor :
{
    public override string Description
    => "Add 1 outsider if able. Sits next to an Outsider";

    public override ActedInfo GetInfo(Character charRef)
    {
        return new ActedInfo("");
    }
    CharacterData pickedCharacterPrevData;
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
        CharacterData pickedOutsider = notInPlayOutsiders[UnityEngine.Random.Range(0, notInPlayOutsiders.Count)];

        if (notInPlayOutsiders.Count != 0)
        {
            Gameplay.Instance.AddScriptCharacter(ECharacterType.Outcast, pickedOutsider);

            viableCharacters = Characters.Instance.FilterAliveCharacters(viableCharacters);
            viableCharacters = Characters.Instance.FilterRealCharacterType(viableCharacters, ECharacterType.Villager);

            Character pickedCharacter = viableCharacters[UnityEngine.Random.Range(0, viableCharacters.Count)];
            pickedCharacterPrevData = pickedCharacter.dataRef;
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
        pickedSwapCharacter.DisableStartAbility();

        GameplayEvents.OnLogRoundAction?.Invoke(
            new GameLog(
                $"{GameLogStrings.thisData} transformed {GameLogStrings.TargetData(0)} into {GameLogStrings.TargetCharacterData(0)}",
                pickedSwapCharacter,
                new List<Character>() { pickedOutsider },
                new List<CharacterData>() { pickedCharacterPrevData }
                ));
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

        //replacedVillager.InitWithNoReset(pickedVillager.GetCharacterBluffIfAble());
        replacedVillager.Init(pickedVillager.GetCharacterBluffIfAble());

        if (Characters.Instance.CheckIfCharacterShouldStartAct(pickedVillager.GetCharacterBluffIfAble()))
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
        return notInPlayCh[UnityEngine.Random.Range(0, notInPlayCh.Count)];
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
    public CharacterData blockedOutcast;

    public override List<SpecialRule> GetRules() => new List<SpecialRule>()
    {
        //new NightModeRule(4),
    };

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger == ETriggerPhase.Start)
        {
            List<CharacterData> outcasts = new List<CharacterData>(Gameplay.Instance.GetScriptCharacters());
            outcasts = Characters.Instance.FilterCharacterType(outcasts, ECharacterType.Outcast);

            if (outcasts.Count <= 0) return;

            CharacterData pickedOutcast = outcasts[UnityEngine.Random.Range(0, outcasts.Count)];

            List<CharacterData> disguisedOutcasts = new List<CharacterData>();
            foreach (CharacterData cd in outcasts)
                if (cd.usuallyDisguised)
                    disguisedOutcasts.Add(cd);

            if (disguisedOutcasts.Count > 0)
            {
                pickedOutcast = disguisedOutcasts[UnityEngine.Random.Range(0, disguisedOutcasts.Count)];
            }
            blockedOutcast = pickedOutcast;

            if (pickedOutcast != null)
            {
                SetMessedByEvil(pickedOutcast, charRef);
                DeckView.AddToObscuredDeckView(pickedOutcast);
            }
        }
        if (trigger == ETriggerPhase.OnDied)
        {
            if (blockedOutcast != null)
                DeckView.RemoveObscuredDeckView(blockedOutcast);
        }
    }

    private void SetMessedByEvil(CharacterData pickedOutcast, Character charRef)
    {
        List<Character> chars = new List<Character>(Gameplay.CurrentCharacters);

        foreach (Character ch in chars)
            if (ch.dataRef == pickedOutcast)
                ch.statuses.AddStatus(ECharacterStatus.MessedUpByEvil, charRef);
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
            if (myList[0].statuses.CheckIfCanAddStatus(ECharacterStatus.Corrupted))
                myList[0].statuses.AddStatus(ECharacterStatus.MessedUpByEvil, charRef);
            myList[0].statuses.AddStatus(ECharacterStatus.Corrupted, charRef);
        }

        if (myList[myList.Count - 1].dataRef.type == ECharacterType.Villager)
        {
            if (myList[0].statuses.CheckIfCanAddStatus(ECharacterStatus.Corrupted))
                myList[myList.Count - 1].statuses.AddStatus(ECharacterStatus.MessedUpByEvil, charRef);
            myList[myList.Count - 1].statuses.AddStatus(ECharacterStatus.Corrupted, charRef);
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

[System.Serializable]
public class Delusion : Demon
{
    //public override List<SpecialRule> GetRules() => new List<SpecialRule>()
    //{
    //    new NightModeRule(4),
    //};

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger == ETriggerPhase.Start)
        {
            Character pickedMinion = GetRandomMinion(charRef);

            CharacterData bluff = Characters.Instance.GetRandomUniqueVillagerBluff();
            //CharacterData bluff = Characters.Instance.GetRandomDuplicateBluff();

            // We disable Bakers for now - its too broken, but will revert it later
            if (bluff.role is Baker)
            {
                Characters.Instance.RemoveCharacterDataFromList(bluff, Characters.Instance.UniquePool);
                bluff = Characters.Instance.GetRandomUniqueVillagerBluff();
            }

            charRef.GiveBluff(bluff);
            charRef.statuses.AddStatus(ECharacterStatus.HealthyBluff, charRef);
            charRef.statuses.AddStatus(ECharacterStatus.BrokenAbility, charRef);
            charRef.statuses.AddStatus(ECharacterStatus.AlteredCharacter, charRef);

            bluff = Characters.Instance.GetRandomUniqueVillagerBluff();

            if (pickedMinion == null) return;
            pickedMinion.GiveBluff(bluff);
            pickedMinion.statuses.AddStatus(ECharacterStatus.HealthyBluff, charRef);
            pickedMinion.statuses.AddStatus(ECharacterStatus.BrokenAbility, charRef);
            pickedMinion.statuses.AddStatus(ECharacterStatus.AlteredCharacter, charRef);
            pickedMinion.statuses.AddStatus(ECharacterStatus.MessedUpByEvil, charRef);
        }
    }

    public Character GetRandomMinion(Character charRef)
    {
        List<Character> minions = new List<Character>(Gameplay.CurrentCharacters);
        minions = Characters.Instance.FilterCharacterType(minions, ECharacterType.Minion);

        if (minions.Count == 0) return null;

        Character pickedMinion = minions[UnityEngine.Random.Range(0, minions.Count)];
        return pickedMinion;
    }
}



//Character Data HELPERS
public class TriggerHelper : Role
{
    public ERelicTrigger trigger;

    public override string Description => "";

    public override ActedInfo GetBluffInfo(Character charRef)
    { return null; }

    public override ActedInfo GetInfo(Character charRef)
    { return null; }

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger == ETriggerPhase.Start)
            Gameplay.TriggerRelics(this.trigger);
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