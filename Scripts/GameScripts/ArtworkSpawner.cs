using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class ArtworkSpawner : MonoBehaviour
{
    public GameObject artwork;
    public List<Transform> spawnPoints;

    // Start is called before the first frame update
    void Start()
    {
        spawnArtworks();
    }

    async void spawnArtworks()
    {
        QueryMarketplace queryMarketplace = new QueryMarketplace();
        QueryNFT queryNFT = new QueryNFT();

        // Get a list of all the listed Artworks
        var itemCount = await queryMarketplace.GetItemCounter();
        var allListedArtworks = await queryMarketplace.GetAllMarketItems();

        // Calculate the number of artworks to spawn
        var count = (itemCount <= spawnPoints.Count) ? itemCount : spawnPoints.Count;

        // Spawn the artworks
        for(int i=0; i<count; i++)
        {
            GameObject spawnedObj = Instantiate(artwork, spawnPoints[i].position, spawnPoints[i].rotation);
            var scriptRef = spawnedObj.GetComponent<GetArtwork>();

            string contractAddress = allListedArtworks[i].nftContractAddress;
            string tokenURI = await queryNFT.GetTokenURI(allListedArtworks[i].tokenId, contractAddress);

            ArtworkDetails details = new ArtworkDetails(
                "Owner: " + allListedArtworks[i].owner,
                !allListedArtworks[i].isSold,
                "Price: " + allListedArtworks[i].price + "Wei"
            );

            StartCoroutine(scriptRef.GetArtworkDetails(tokenURI, details));
        }
    }
}