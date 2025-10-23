// Project: Demon Bluff (Sample Reference)
// File: Characters.cs  |  Version: v0.380f 
// Purpose: Reference-only implementation showing how characters are coded.
// License: All Rights Reserved – shared for reference only.
//          You may read and learn from this file, but you may not use this code directly in other projects without permission.
// Copyright (c) 2025 UmiArt. All rights reserved.
// Contact: pkwiatkowski@umiart.pl

using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Characters : MonoBehaviour
{
    public List<Character> characters = new List<Character>();
    public static Characters Instance;
    public CharacterData[] startGameActOrder;
    public CharactersPool[] characterPool;
    public CharactersPool currentPool;

    public List<CharacterData> UniquePool = new List<CharacterData>();
    public List<CharacterData> DuplicatesPool = new List<CharacterData>();

    public Action onSetup;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        HideAll();
    }
    private void OnDisable()
    {
        HideAll();
    }

    public void HideAll()
    {
        foreach (CharactersPool cp in characterPool)
            cp.gameObject.SetActive(false);
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (GameData.DebugMode)
            if (Input.GetKeyDown(KeyCode.R))
                RevealAllDebug();
#endif
    }


    public void Init(List<CharacterData> charactersList)
    {
        //characters = GetComponentsInChildren<Character>();
        characters.Clear();

        HideAll();

        foreach (CharactersPool cp in characterPool)
        {
            if (cp.characters.Length == charactersList.Count)
            {
                currentPool = cp;
                cp.gameObject.SetActive(true);
                characters = new List<Character>(cp.characters);
            }
        }

        ManageCharacters(charactersList);
    }

    private void ManageCharacters(List<CharacterData> charactersList)
    {
        int i = 0;
        UpdateCharacterPositions();
        PickRoundBluffs();
        PickRoundDuplicates();
        foreach (Character ch in characters)
        {
            int id = Math.Abs(characters.Count - i);
            ch.Init(charactersList[i], id);
            i++;
        }

        Gameplay.UpdateCharacters(characters);

        foreach (CharacterData cd in startGameActOrder)
            foreach (Character c in characters)
            {
                if (cd == c.GetCharacterBluffIfAble())
                {
                    c.Act(ETriggerPhase.Start);
                    if (!(cd.role is Alchemist) &&
                        !(cd.role is Poisoner) &&
                        !(cd.role is Puzzlemaster))
                        break;
                }
            }

        onSetup?.Invoke();

        StartCoroutine(ShuffleDeck());
    }

    private IEnumerator ShuffleDeck()
    {
        yield return new WaitForSeconds(0.5f);
        Gameplay.Instance.ShuffleDeck();
        GameplayEvents.OnDeckShuffled?.Invoke();
        UIEvents.OnUpdateDeckView?.Invoke();
    }

    public CharacterData GetRandomUniqueVillagerBluff()
    {
        List<CharacterData> uniqueVil = new List<CharacterData>(UniquePool);
        uniqueVil = Characters.Instance.FilterRealCharacterType(uniqueVil, ECharacterType.Villager);
        return uniqueVil[UnityEngine.Random.Range(0, uniqueVil.Count)];
    }
    public CharacterData GetRandomUniqueBluff()
    {
        return UniquePool[UnityEngine.Random.Range(0, UniquePool.Count)];
    }
    public CharacterData GetRandomDuplicateBluff()
    {
        return DuplicatesPool[UnityEngine.Random.Range(0, DuplicatesPool.Count)];
    }
    private void PickRoundBluffs()
    {
        List<CharacterData> notInPlayCharacters = Gameplay.Instance.GetAscensionAllStartingCharacters();
        List<CharacterData> currentCharacters = Gameplay.Instance.GetScriptCharacters();

        UniquePool.Clear();
        notInPlayCharacters.RemoveAll(cd => currentCharacters.Contains(cd));
        notInPlayCharacters = FilterBluffableCharacters(notInPlayCharacters);

        CharacterData newC = null;
        List<CharacterData> villagers = FilterRealCharacterType(notInPlayCharacters, ECharacterType.Villager);
        List<CharacterData> outcasts = FilterRealCharacterType(notInPlayCharacters, ECharacterType.Outcast);

        for (int i = 0; i < 4; i++)
        {
            if (villagers.Count == 0) break;

            newC = villagers[UnityEngine.Random.Range(0, villagers.Count)];
            UniquePool.Add(newC);
            villagers.Remove(newC);
        }
        if (outcasts.Count > 0)
        {
            newC = outcasts[UnityEngine.Random.Range(0, outcasts.Count)];
            UniquePool.Add(newC);
            outcasts.Remove(newC);
        }

        // Safeguard
        if (UniquePool.Count == 0)
        {
            notInPlayCharacters = Gameplay.Instance.GetAscensionAllStartingCharacters();
            notInPlayCharacters = FilterBluffableCharacters(notInPlayCharacters);
            notInPlayCharacters = FilterAlignmentCharacters(notInPlayCharacters, EAlignment.Good);

            UniquePool.Add(notInPlayCharacters[UnityEngine.Random.Range(0, notInPlayCharacters.Count)]);
        }
    }
    private void PickRoundDuplicates()
    {
        List<CharacterData> notInPlayCharacters = Gameplay.Instance.GetScriptCharacters();
        DuplicatesPool.Clear();
        notInPlayCharacters = FilterBluffableCharacters(notInPlayCharacters);

        CharacterData newC = null;
        List<CharacterData> villagers = FilterRealCharacterType(notInPlayCharacters, ECharacterType.Villager);
        List<CharacterData> outcasts = FilterRealCharacterType(notInPlayCharacters, ECharacterType.Outcast);
        FilterAlignmentCharacters(notInPlayCharacters, EAlignment.Good);
        for (int i = 0; i < 4; i++)
        {
            newC = villagers[UnityEngine.Random.Range(0, villagers.Count)];
            DuplicatesPool.Add(newC);
            villagers.Remove(newC);

            if (villagers.Count == 0) break;
        }
        if (outcasts.Count > 0)
        {
            newC = outcasts[UnityEngine.Random.Range(0, outcasts.Count)];
            DuplicatesPool.Add(newC);
            outcasts.Remove(newC);
        }

        // Safeguard

        if (DuplicatesPool.Count == 0)
        {
            notInPlayCharacters = Gameplay.Instance.GetAscensionAllStartingCharacters();
            notInPlayCharacters = FilterBluffableCharacters(notInPlayCharacters);
            notInPlayCharacters = FilterAlignmentCharacters(notInPlayCharacters, EAlignment.Good);

            DuplicatesPool.Add(notInPlayCharacters[UnityEngine.Random.Range(0, notInPlayCharacters.Count)]);
        }
    }

    [Button]
    public void UpdateCharacterPositions()
    {
        int i = 0;
        float rotateVal = 360f / (float)characters.Count;

        foreach (Character ch in characters)
        {
            ch.transform.localEulerAngles = new Vector3(0, 0, rotateVal * i);
            i++;
            ch.ResetRotation();
        }
    }

    public void HighlightCharacters(List<Character> chList)
    {
        foreach (Character c in chList)
        {
            if (c != null)
                c.ShowHighlight();
        }
    }
    public void DisableHighlightAll()
    {
        foreach (Character c in characters)
        {
            if (c != null)
                c.DisableHighlight();
        }
    }

    public void RevealAllDebug()
    {
        string description = "";

        foreach (Character c in Gameplay.CurrentCharacters)
        {
            description += $"{c.id}: {c.GetCharacterBluffIfAble().name}";
            if (c.state == ECharacterState.Dead)
                description += $", D";
            if (c.state == ECharacterState.Hidden)
                description += $", H";
            if (c.bluff != null)
                description += $", realRole: {c.dataRef}; ";
            else
                description += $"; ";

            if (c.acteds.GetActed() != "")
                description += $"act: '{c.acteds.GetActed()}'";

            description += ";; \n";
        }
        Debug.Log(description);

        foreach (Character ch in characters)
        {
            ch.RevealAllReal();
        }
    }
    public void RevealAll()
    {
        string description = "";

        foreach (Character c in Gameplay.CurrentCharacters)
        {
            description += $"{c.id}: {c.GetCharacterBluffIfAble().name}";
            if (c.state == ECharacterState.Dead)
                description += $", D";
            if (c.state == ECharacterState.Hidden)
                description += $", H";
            if (c.bluff != null)
                description += $", realRole: {c.dataRef}; ";
            else
                description += $"; ";

            if (c.acteds.GetActed() != "")
                description += $"act: '{c.acteds.GetActed()}'";

            description += ";; \n";
        }
        Debug.Log(description);

        foreach (Character ch in characters)
        {
            ch.ChangeState(ECharacterState.Revealed);
            ch.RevealAllReal();
            ch.ChangeState(ch.prevState);
        }
    }

    public List<Character> GetAdjacentAliveCharacters(Character ch)
    {
        List<Character> aliveCharacters = FilterAliveCharacters(Gameplay.CurrentCharacters);
        List<Character> adjacentCharacters = new List<Character>();
        int id = 0;
        foreach (Character c in aliveCharacters)
        {
            if (c == ch)
            {
                adjacentCharacters.Add(aliveCharacters[id - 1 < 0 ? aliveCharacters.Count - 1 : id - 1]);
                adjacentCharacters.Add(aliveCharacters[id + 1 > aliveCharacters.Count - 1 ? 0 : id + 1]);
                break;
            }
            id++;
        }
        return adjacentCharacters;
    }
    public List<Character> GetAdjacentCharacters(Character ch)
    {
        List<Character> allCharacters = new List<Character>(Gameplay.CurrentCharacters);
        List<Character> adjacentCharacters = new List<Character>();
        int id = 0;
        foreach (Character c in allCharacters)
        {
            if (c == ch)
            {
                adjacentCharacters.Add(allCharacters[id - 1 < 0 ? allCharacters.Count - 1 : id - 1]);
                adjacentCharacters.Add(allCharacters[id + 1 > allCharacters.Count - 1 ? 0 : id + 1]);
                break;
            }
            id++;
        }
        return adjacentCharacters;
    }
    public Character GetRandomAliveCharacter(List<Character> chars)
    {
        return chars[UnityEngine.Random.Range(0, chars.Count)];
    }
    public List<CharacterData> FilterCharacterType(List<CharacterData> inpuCharacters, ECharacterType type)
    {
        List<CharacterData> filteredCharacters = new List<CharacterData>();
        foreach (CharacterData c in inpuCharacters)
            if (c.type == type)
                filteredCharacters.Add(c);
        return filteredCharacters;
    }
    public List<Character> FilterCharacterType(List<Character> inpuCharacters, ECharacterType type)
    {
        List<Character> filteredCharacters = new List<Character>();
        foreach (Character c in inpuCharacters)
            if (c.GetCharacterType() == type)
                filteredCharacters.Add(c);
        return filteredCharacters;
    }
    public List<Character> FilterRealCharacterType(List<Character> inpuCharacters, ECharacterType type)
    {
        List<Character> filteredCharacters = new List<Character>();
        foreach (Character c in inpuCharacters)
            if (c.dataRef.type == type)
                filteredCharacters.Add(c);
        return filteredCharacters;
    }
    public List<Character> FilterRealCharacterRole<T>(List<Character> inpuCharacters) where T : Role
    {
        List<Character> filteredCharacters = new List<Character>();
        foreach (Character c in inpuCharacters)
            if (c.dataRef.role is T)
                filteredCharacters.Add(c);
        return filteredCharacters;
    }
    public List<Character> FilterCharacterContainsStatus(List<Character> inpuCharacters, ECharacterStatus status)
    {
        List<Character> filteredCharacters = new List<Character>();
        foreach (Character c in inpuCharacters)
            if (c.statuses.Contains(status))
                filteredCharacters.Add(c);
        return filteredCharacters;
    }
    public List<Character> FilterCharacterMissingStatus(List<Character> inpuCharacters, ECharacterStatus status)
    {
        List<Character> filteredCharacters = new List<Character>();
        foreach (Character c in inpuCharacters)
            if (!c.statuses.Contains(status))
                filteredCharacters.Add(c);
        return filteredCharacters;
    }
    public List<Character> GetCharactersAtRange(int range, Character charRef)
    {
        List<Character> filteredChrs = new List<Character>();
        List<Character> chrs = new List<Character>(Gameplay.CurrentCharacters);
        chrs = CharactersHelper.GetSortedListWithCharacterFirst(chrs, charRef);

        chrs.RemoveAt(0);
        if (range != 0)
            if (chrs.Count > range - 1)
                filteredChrs.Add(chrs[range - 1]);
        if (range != 0)
            if (chrs.Count > range - 1)
                filteredChrs.Add(chrs[chrs.Count - range]);

        return filteredChrs;
    }

    public List<CharacterData> FilterRealCharacterType(List<CharacterData> inpuCharacters, ECharacterType type)
    {
        List<CharacterData> filteredCharacters = new List<CharacterData>();
        foreach (CharacterData c in inpuCharacters)
            if (c.type == type)
                filteredCharacters.Add(c);
        return filteredCharacters;
    }
    public List<Character> FilterAlignmentCharacters(List<Character> inpuCharacters, EAlignment alignment)
    {
        List<Character> filteredCharacters = new List<Character>();
        foreach (Character c in inpuCharacters)
            if (c.GetAlignment() == alignment)
                filteredCharacters.Add(c);
        return filteredCharacters;
    }
    public List<Character> FilterRealAlignmentCharacters(List<Character> inpuCharacters, EAlignment alignment)
    {
        List<Character> filteredCharacters = new List<Character>();
        foreach (Character c in inpuCharacters)
            if (c.alignment == alignment)
                filteredCharacters.Add(c);
        return filteredCharacters;
    }
    public List<Character> RemoveCharacterType<T>(List<Character> inputCharacters)
    {
        List<Character> filteredCharacters = new List<Character>(inputCharacters);
        foreach (Character c in inputCharacters)
            if (c.dataRef is T)
                filteredCharacters.Remove(c);
        return filteredCharacters;
    }
    public List<Character> FilterRevealedCharacters(List<Character> inputCharacters)
    {
        List<Character> filteredCharacters = new List<Character>();
        foreach (Character c in inputCharacters)
            if (c.state != ECharacterState.Hidden)
                filteredCharacters.Add(c);
        return filteredCharacters;
    }
    public List<Character> FilterHiddenCharacters(List<Character> inputCharacters)
    {
        List<Character> filteredCharacters = new List<Character>();
        foreach (Character c in inputCharacters)
            if (c.state == ECharacterState.Hidden)
                filteredCharacters.Add(c);
        return filteredCharacters;
    }
    public List<CharacterData> FilterAlignmentCharacters(List<CharacterData> inpuCharacters, EAlignment alignment)
    {
        List<CharacterData> filteredCharacters = new List<CharacterData>();
        foreach (CharacterData c in inpuCharacters)
            if (c.startingAlignment == alignment)
                filteredCharacters.Add(c);
        return filteredCharacters;
    }
    public List<CharacterData> FilterBluffableCharacters(List<CharacterData> inpuCharacters)
    {
        List<CharacterData> filteredCharacters = new List<CharacterData>();
        foreach (CharacterData c in inpuCharacters)
            if (c.bluffable)
                filteredCharacters.Add(c);
        return filteredCharacters;
    }
    public List<Character> FilterBluffableCharacters(List<Character> inpuCharacters)
    {
        List<Character> filteredCharacters = new List<Character>();
        foreach (Character c in inpuCharacters)
            if (c.dataRef.bluffable)
                filteredCharacters.Add(c);
        return filteredCharacters;
    }
    public List<CharacterData> FilterNotInPlayCharacters(List<CharacterData> inpuCharacters)
    {
        List<CharacterData> filteredCharacters = new List<CharacterData>(inpuCharacters);
        foreach (Character c in Gameplay.CurrentCharacters)
            if (filteredCharacters.Contains(c.dataRef))
                filteredCharacters.Remove(c.dataRef);

        return filteredCharacters;
    }
    public List<CharacterData> FilterNotInPlayCharactersUnique(List<CharacterData> inpuCharacters)
    {
        List<CharacterData> filteredCharacters = new List<CharacterData>(inpuCharacters);
        foreach (Character c in Gameplay.CurrentCharacters)
            if (filteredCharacters.Contains(c.dataRef))
            {
                CharacterData dataToRemove = c.dataRef;
                filteredCharacters.RemoveAll(cd => cd == dataToRemove);
            }

        return filteredCharacters;
    }
    public List<CharacterData> FilterNotInDeckCharactersUnique(List<CharacterData> allCharacters)
    {
        List<CharacterData> filteredCharacters = new List<CharacterData>(allCharacters);
        foreach (CharacterData cd in Gameplay.Instance.GetScriptCharacters())
        {
            CharacterData dataToRemove = cd;
            filteredCharacters.RemoveAll(cd => cd == dataToRemove);
        }
        return filteredCharacters;
    }
    public List<Character> FilterAliveCharacters(List<Character> inpuCharacters)
    {
        List<Character> aliveCharacters = new List<Character>();
        foreach (Character c in inpuCharacters)
            if (c.state != ECharacterState.Dead)
                aliveCharacters.Add(c);
        return aliveCharacters;
    }
}