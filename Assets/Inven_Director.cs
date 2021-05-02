using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Net;
using System.IO;
using SimpleJSON;


public class Item_Inf
{
    // Start is called before the first frame update
    public string itemId;
    public Sprite itemImage;
    public string itemName;
    public int itemCount;
    public string itemType;
    public bool isItemAcquired;

    public Item_Inf(string ItemId, Sprite ItemImage, string ItemName, int ItemCount, string ItemType, bool IsItemAcquired)
    {
        this.itemId = ItemId;
        this.itemImage = ItemImage;
        this.itemName = ItemName;
        this.itemCount = ItemCount;
        this.itemType = ItemType;
        this.isItemAcquired = IsItemAcquired;
    }
}

public class Inven_Director : MonoBehaviour
{


    // Start is called before the first frame update
    int Now_page;
    int Max_page;
    int Count_remainder;
    bool isStart;
    GameObject[] Slot_list = new GameObject[6];
    List<Item_Inf> Item_list = new List<Item_Inf>();
    public Sprite No_Item_mark;
    public Text Left_text;
    public Text Right_text;


    Animator animator;

    void Start()
    {
        isStart = true;
        StartCoroutine(GetRequest("http://3.35.167.116:8000/v1/inventories/"));
        Debug.Log(Item_list);
        for (int i = 0; i < 6; i++) {
            Slot_list[i] = transform.GetChild(i).gameObject;
        }
        animator = gameObject.transform.parent.GetComponent<Animator>();
    }


    // Update is called once per frame
    void Update()
    {
        if (animator.GetBool("Inven_Open") == false)    //인벤 닫혔을 때
        {
            if (Input.GetKeyDown("i"))
            {
                StartCoroutine(GetRequest("http://3.35.167.116:8000/v1/inventories/"));
                animator.SetBool("Inven_Open", true);
            }
        }
        else                                            //인벤 열렸을 때
        {
            if (Input.GetKeyDown("i") || Input.GetKeyDown(KeyCode.Escape))
            {
                animator.SetBool("Inven_Open", false);
            }
        }
        if (Input.GetKeyDown("z"))
        {
            Item_Inf inf = Item_list.Find(item => item.itemName == "광선검");
            Debug.Log(inf.itemId.ToString()+ inf.itemCount.ToString()+ inf.isItemAcquired.ToString());
        }
    }
    public void Click_Right_page()
    {
        if( Now_page < Max_page )
        {
            Now_page++;
            Left_text.text = Now_page.ToString();
            HideItemSlot();
            SetItemSlot();
        }
    }
    public void Click_left_page()
    {
        if (Now_page > 1)
        {
            Now_page--;
            Left_text.text = Now_page.ToString();
            HideItemSlot();
            SetItemSlot();
        }
    }

    void HideItemSlot()
    {
        if (Now_page == Max_page && Count_remainder > 0)
        {
            for(int i = Count_remainder; i < 6; i++)
            {
                Slot_list[i].SetActive(false);
            }
        }
        if (Now_page < Max_page)
        {
            for (int i = 0; i < 6; i++)
            {
                Slot_list[i].SetActive(true);
            }
        }
    }

    void SetItemSlot()
    {
        int count = 6;
        if (Now_page == Max_page && Count_remainder > 0)
            count = Count_remainder;
        for (int i = 0; i < count; i++)
        {
            if (Item_list[(Now_page - 1) * 6 + i].isItemAcquired == false)
            {
                Slot_list[i].transform.GetChild(0).GetComponent<Image>().sprite = No_Item_mark;
                Slot_list[i].transform.GetChild(1).GetComponent<Text>().text = "???";
                Slot_list[i].transform.GetChild(2).GetComponent<Text>().text = "X 0";
            }
            else
            {
                Slot_list[i].transform.GetChild(0).GetComponent<Image>().sprite = Item_list[(Now_page - 1) * 6 + i].itemImage;
                Slot_list[i].transform.GetChild(1).GetComponent<Text>().text = Item_list[(Now_page - 1) * 6 + i].itemName;
                Slot_list[i].transform.GetChild(2).GetComponent<Text>().text = "X " + Item_list[(Now_page - 1) * 6 + i].itemCount.ToString();
            }
        }
    }

    public void Active_Button(int i)
    {
        Debug.Log((Now_page - 1) * 6 + i);
    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            Item_list.Clear();
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    //Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);

                    string requestResult = webRequest.downloadHandler.text;
                    JSONNode result = JSON.Parse(requestResult);
                    Debug.Log(result);
                    Debug.Log(result["results"]); // 배열
                    Debug.Log(result["count"]);   // 아이템 개수
                    Debug.Log("-----------------------------");
                    int count = result["count"];
                    var datas = result["results"];
                    

                    for (int i = 0; i < count; i++)
                    {
                        var data = datas[i];
                        yield return StartCoroutine(SetSprite(data));
                        // https://dorestory.s3.ap-northeast-2.amazonaws.com/static/item_images/resized_zQddhYPMQV_161812242669.png
                        // 이미지 url
                    }
                    if (isStart)
                        yield return StartCoroutine(SetStart());
                    else
                        yield return StartCoroutine(SetInf());
                    //List<Item_Inf> inventories = 
                    //Debug.Log(inventories);
                    break;
            }
        }
    }
    IEnumerator SetSprite(JSONNode Data)
    {
        Sprite urlItemImage;
        using (WWW www = new WWW(Data["image"]))
        {
            yield return www;
            Texture2D tex = new Texture2D(70, 70, TextureFormat.DXT1, false);
            www.LoadImageIntoTexture(tex);
            urlItemImage = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);
        }
        Debug.Log("========================================");
        var itemInf = new Item_Inf(Data["id"], urlItemImage, Data["name"], Data["count"], Data["type"], Data["is_acquired"]);
        Item_list.Add(itemInf);

        Debug.Log(Data["count"]);
    }

    IEnumerator SetStart()
    {
        Now_page = 1;
        Max_page = (int)Mathf.Ceil(Item_list.Count / 6.0f);
        Count_remainder = Item_list.Count % 6;
        Left_text.text = Now_page.ToString();
        Right_text.text = Max_page.ToString();

        HideItemSlot();
        SetItemSlot();
        isStart = false;
        yield return null;
    }
    IEnumerator SetInf()
    {
        HideItemSlot();
        SetItemSlot();
        yield return null;
    }
}

