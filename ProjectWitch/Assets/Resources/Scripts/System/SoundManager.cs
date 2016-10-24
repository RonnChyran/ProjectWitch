using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using GameData;

public class SoundManager : MonoBehaviour {

    //各オーディオのチャンネル数
    public int BGMChannelSize { get; set; }
    public int SEChannelSize { get; set; }
    public int VoiceChannelSize { get; set; }

    //各オーディオソース
    private List<AudioSource> mcBGMs;
    private List<AudioSource> mcSEs;
    private List<AudioSource> mcVoices;

    //サウンドリスト
    private List<int> mSoundList;

    //定数
    private const string filePath = "Data/sound.csv";
	
	// Update is called once per frame
	void Update () {
	
	}

    //初期化
    public void Init()
    {
        //コンポーネントの作成
        for (int i = 0; i < BGMChannelSize; i++) mcBGMs.Add(gameObject.AddComponent<AudioSource>());
        for (int i = 0; i < SEChannelSize; i++) mcSEs.Add(gameObject.AddComponent<AudioSource>());
        for (int i = 0; i < VoiceChannelSize; i++) mcVoices.Add(gameObject.AddComponent<AudioSource>());
    }

    //再初期化
    public void Reset()
    {
        //コンポーネントをいったん削除
        foreach (var c in mcBGMs) Destroy(c);
        foreach (var c in mcSEs) Destroy(c);
        foreach (var c in mcVoices) Destroy(c);

        //再初期化
        Init();
    }

    public void PlayLoop(int id)
    {

    }

    public void PlayOneShot(int id)
    {

    }
}

namespace GameData
{
    public class SoundDataFormat
    {
        //事前ロードを行うかどうか
        bool PreLoad { get; set; }

        //ファイル名
        string Name { get; set; }

        //種別
        public enum SoundType : int { BGM = 0, SE = 1, Voice = 2 }
        SoundType Type { get; set; }

        //AudioClipのインスタンス
        private AudioClip mResource = null;
        public AudioClip AudioClip
        {
            get
            {
                if (mResource) LoadResource();
                return mResource;
            }
            private set { }
        }



        private void LoadResource()
        {
            var game = Game.GetInstance();

            var filepath = "";
            switch(Type)
            {
                case SoundType.BGM:
                    filepath = GamePath.BGM;
                    break;

                case SoundType.SE:
                    filepath = GamePath.SE;
                    break;

                case SoundType.Voice:
                    filepath = GamePath.Voice;
                    break;

                default:
                    break;
            }

            mResource = Resources.Load(filepath + Name) as AudioClip;
        }
    }
}