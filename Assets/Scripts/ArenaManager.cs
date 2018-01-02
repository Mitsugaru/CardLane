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

    private IList<Lane> lanes;

    private System.Random random = new System.Random();

    // Use this for initialization
    void Start()
    {
        lanes = new List<Lane>();
        lanes.Add(LaneA);
        lanes.Add(LaneB);
        lanes.Add(LaneC);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Clear()
    {
        foreach(Lane lane in lanes)
        {
            lane.Clear();   
        }

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

    public IList<Lane> getLanes()
    {
        return lanes;
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

    public void placeCard(GameObject card, Transform parent)
    {
        float scalingFactor = 25f;
        card.transform.parent = parent;
        card.transform.position = parent.position;
        card.transform.localScale = new Vector3(scalingFactor, scalingFactor, scalingFactor);
        card.transform.localRotation = Quaternion.Euler(90f, 0f, 90f);
        playSound();
    }

    public void placeStockpile(GameObject card, Transform parent)
    {
        card.transform.parent = parent;
        card.transform.localPosition = new Vector3(0f, 0.01f * parent.childCount, 0f);
        card.transform.localRotation = Quaternion.Euler(-90f, 0f, 90f + (30f * (parent.childCount - 1)));
        //TODO it'd be cool to do a rotate animation
    }

    private void playSound()
    {
        if (audioSource != null && placeSounds.Length > 0)
        {
            audioSource.PlayOneShot(placeSounds[random.Next(placeSounds.Length)]);
        }
    }
}
