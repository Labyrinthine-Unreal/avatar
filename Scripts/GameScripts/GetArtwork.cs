using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class GetArtwork : MonoBehaviour
{
    public Renderer imageRenderer;
    public string url;
    private ProximityScript proximity;

    private IEnumerator GetAndSetTexture(string imageURL)
    { 
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageURL);
        yield return www.SendWebRequest();

        while(!www.isDone)
        {
            Debug.Log("Fetching Image Under Progress");
        }

        Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
        
        imageRenderer.materials[1].SetTexture("_MainTex", myTexture);
    }

    private IEnumerator GetRequestCoroutine(string uri)
    {
        UnityWebRequest www = UnityWebRequest.Get(uri);
        yield return www.SendWebRequest();

        while(!www.isDone)
        {
            Debug.Log("Fetching Metadata Under process");
        }

        if(!string.IsNullOrEmpty(www.error))
        {
            Debug.Log("Error occured while fetching metadata");
        }

        var result = www.downloadHandler.text;

        var metadata = JsonConvert.DeserializeObject<OpenSeaMetadata>(result);

        proximity.newTitle = "Title: " + metadata.name;
        proximity.newDesc = "Desc: " + metadata.description;

        var imageUrl = metadata.image;
        StartCoroutine(GetAndSetTexture(imageUrl));
    }

    private void SetArtworkDetails(ArtworkDetails details)
    {
        proximity.newOwner = details.owner;
        if(details.isForSale)
        {
            proximity.newIsForSale = "For Sale";
            proximity.newPrice = details.price;
        }
    }

    public IEnumerator GetArtworkDetails(string URI, ArtworkDetails details)
    {
        proximity = gameObject.GetComponent<ProximityScript>();
        StartCoroutine(GetRequestCoroutine(URI));
        SetArtworkDetails(details);
        yield return 0;
    }

}