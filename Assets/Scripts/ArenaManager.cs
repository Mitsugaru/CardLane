using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaManager : MonoBehaviour
{

    //TODO inject this later
    public CardManager cardManager;

    public Lane deckArea;

    public Lane LaneA;

    public Lane LaneB;

    public Lane LaneC;

    public AudioSource audioSource;

    public AudioClip[] placeSounds;

    private System.Random random = new System.Random();

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void DemoArena()
    {
        StartCoroutine(DemoArenaRoutine());
    }

    public IEnumerator DemoArenaRoutine()
    {
        IList<Transform> placements = generatePlacements();

        foreach (Transform placement in placements)
        {
            GameObject card = cardManager.SpawnRandom();

            if (card != null)
            {
                placeCard(card, placement);
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    public void Clear()
    {
        IList<Transform> placements = generatePlacements();
        foreach (Transform placement in placements)
        {
            foreach (Transform child in placement)
            {
                GameObject.Destroy(child.gameObject);
            }
            placement.DetachChildren();
        }
    }

    private IList<Transform> generatePlacements()
    {
        IList<Transform> placements = new List<Transform>();
        placements.Add(LaneA.PlayerSlot);
        placements.Add(LaneA.OpponentSlot);
        placements.Add(LaneB.PlayerSlot);
        placements.Add(LaneB.OpponentSlot);
        placements.Add(LaneC.PlayerSlot);
        placements.Add(LaneC.OpponentSlot);
        placements.Add(LaneA.playerStockpile);
        placements.Add(LaneA.opponentStockpile);
        placements.Add(LaneB.playerStockpile);
        placements.Add(LaneB.opponentStockpile);
        placements.Add(LaneC.playerStockpile);
        placements.Add(LaneC.opponentStockpile);

        return placements;
    } 

    private void placeCard(GameObject card, Transform parent)
    {
        float scalingFactor = 25f;
        card.transform.parent = parent;
        card.transform.position = parent.position;
        card.transform.localScale = new Vector3(scalingFactor, scalingFactor, scalingFactor);
        card.transform.localRotation = Quaternion.Euler(90f, 0f, 90f);
        playSound();
    }

    private void playSound()
    {
        if (audioSource != null && placeSounds.Length > 0)
        {
            audioSource.PlayOneShot(placeSounds[random.Next(placeSounds.Length)]);
        }
    }
}
