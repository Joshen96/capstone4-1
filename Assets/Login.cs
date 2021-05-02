//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
public class Login : MonoBehaviour
{
    Dictionary<string, string> ID_Table = new Dictionary<string, string>();
    public GameObject ID_Input;
    public GameObject PWD_Input;
    private string requestURL = "http://3.35.167.116:8000/v1/users/login/";
    private string headerUsername = "100dongwoo12345";
    private string headerPassword = "123123";
    // Start is called before the first frame update
    void Start()
    {
        //ID_Table.Add("abcd", "1111");
        //ID_Table.Add("abcd3", "1112");
        //ID_Table.Add("abcd4", "1113");
        HttpClientExampleAsync();
    }

    public void Login_test()
    {
        string ID_text = ID_Input.GetComponent<InputField>().text.ToString();
        string PWD_text = PWD_Input.GetComponent<InputField>().text.ToString();
        if (ID_Table.ContainsKey(ID_text))
        {
            if (ID_Table[ID_text] == PWD_text)
            {

                Debug.Log("Success");
            }
            else
            {
                Debug.Log("Wrong PWD");
            }
        }
        else
        {
            Debug.Log("NO ID");
        }
    }
    public class LoginData
    {
        public string login_id;
        public string password;
    }

    public class User
    {
        public string login_id = "";
        public string nickname = "";
        public string damage = "";
        public string hp = "";
        public string exp = "";

        public User(object user)
        {
            
        }
    }
    public void HttpClientExampleAsync()
    {
        Debug.Log("start!!");
        LoginData data = new LoginData();
        
        data.login_id = "100dongwoo12345";
        data.password = "123123";

        string str = JsonUtility.ToJson(data);
        var bytes = System.Text.Encoding.UTF8.GetBytes(str);

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestURL);
        request.Method = "POST";
        request.ContentType = "application/json";
        request.ContentLength = bytes.Length;

        using(var stream = request.GetRequestStream())
        {
            stream.Write(bytes, 0, bytes.Length);
            stream.Flush();
            stream.Close();
        }
        HttpWebResponse res = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(res.GetResponseStream());
        string userData = reader.ReadToEnd(); // user: {}
        //string json = reader.ReadToEnd();
        Debug.Log("user: " + userData);
        //User user = new User(userData);
    }
}
