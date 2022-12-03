using System.Collections;
using UnityEngine;

public class LoadAssetBundle : MonoBehaviour
{
    // Google Driveにアセットバンドル化させたオブジェクトを配置(テスト)
    string url = "https://drive.google.com/uc?export=download&id=1uZkzuA5U_lZJGDZStvxXhMahvlVuYdwk";
    //string url = "https://drive.google.com/uc?export=download&id=12HC6UsuZaM3F78VcxnVSXsfVu0Z9VDYE";
    //string url = "https://drive.google.com/uc?export=download&id=1jLeMqmxpSq4dvTMgYxsOVRQ3sNLQl_O1";

    // Start is called before the first frame update
    void Start()
    {
        //WWW www = new WWW(url);

        WWW www = WWW.LoadFromCacheOrDownload(url,1);

        StartCoroutine(DownloadAssetModel(www));
    }


    IEnumerator DownloadAssetModel(WWW www)
    {
        yield return www;

        while (www.isDone == false)
        {
            yield return null;
        }

        AssetBundle bundle = www.assetBundle;

        if (www.error == null)
        {
            // ★Cubeとかはロードできるが、ゴブリンとかのキャラクターはロードできるが何故かピンク色になる…

          //  GameObject obj2 = Instantiate((GameObject)bundle.LoadAsset("Cube"), new Vector3(1, 4, 0), Quaternion.identity);
          //  GameObject obj = Instantiate((GameObject)bundle.LoadAsset("Goblin_rouge_r"), new Vector3(0,3,0), Quaternion.identity);

          //  GameObject obj = Instantiate((GameObject)bundle.LoadAsset("GoblinHigh"), new Vector3(0, 3, 0), Quaternion.identity);
        }
        else
        {
            Debug.Log(www.error);
        }
    }
}
