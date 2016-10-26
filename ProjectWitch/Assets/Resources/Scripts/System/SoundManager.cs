using UnityEngine;
using System.Collections.Generic;

//サウンドの種類
public enum SoundType { BGM, SE, Voice }

//サウンド管理クラス
public class SoundManager : MonoBehaviour {

    //BGMのソースオブジェクト
    [SerializeField]
    private GameObject mBGMSource = null;
    private CriAtomSource mcBGM = null;

    //SEのソースオブジェクト
    [SerializeField]
    private GameObject mSESource = null;
    private CriAtomSource mcSE = null;

    //ボイスのソースオブジェクト
    [SerializeField]
    private GameObject mVoiceSource = null;
    private CriAtomSource mcVoice = null;
    
    //サウンドの種類とキューシート名の対応表
    private readonly Dictionary<SoundType, string> mCueSheetList
        = new Dictionary<SoundType, string>()
        {
            { SoundType.BGM,    "BGM" },
            { SoundType.SE,     "SE" },
            { SoundType.Voice,  "Voice" }
        };
    
    public void Start()
    {
        //エラーチェック
        Debug.Assert(mBGMSource, "BGMソースをセットしてください");
        Debug.Assert(mSESource, "SEソースをセットしてください");
        Debug.Assert(mVoiceSource, "Voiceソースをセットしてください");

        //コンポーネントの取得
        mcBGM = mBGMSource.GetComponent<CriAtomSource>();
        mcSE = mSESource.GetComponent<CriAtomSource>();
        mcVoice = mVoiceSource.GetComponent<CriAtomSource>();

        //エラーチェック
        Debug.Assert(mcBGM, "BGMソースにCriAtomSourceコンポーネントをセットしてください");
        Debug.Assert(mcSE, "SEソースにCriAtomSourceコンポーネントをセットしてください");
        Debug.Assert(mcVoice, "VoiceソースにCriAtomSourceコンポーネントをセットしてください");
    }

    public void Update()
    {
        var game = Game.GetInstance();

        //ボリュームの同期
        var master = game.Config.MasterVolume;
        mcBGM.volume = game.Config.BGMVolume * master;
        mcSE.volume = game.Config.SEVolume * master;
        mcVoice.volume = game.Config.VoiceVolume * master;
    }

    public void Play(string name, SoundType type)
    {
        if(!IsExist(name, type))
        {
            Debug.LogWarning("指定したサウンドファイルは存在しません　" +
                "　種別：" + mCueSheetList[type] + 
                "　名前：" + name);
            return;
        }

        var source = GetSource(type);

        source.cueSheet = mCueSheetList[type];
        source.cueName = name;

        var inst = source.Play();
        
    }

    public void Stop(SoundType type)
    {
        var source = GetSource(type);

        source.Stop();
    }

    public bool IsPlaying(SoundType type)
    {
        var source = GetSource(type);

        return source.status == CriAtomSource.Status.Playing;
    }

    public string GetCueName(SoundType type)
    {
        var source = GetSource(type);

        return source.cueName;
    }

    private CriAtomSource GetSource(SoundType type)
    {

        CriAtomSource source = null;
        switch (type)
        {
            case SoundType.BGM: source = mcBGM; break;
            case SoundType.SE: source = mcSE; break;
            case SoundType.Voice: source = mcVoice; break;
            default: break;
        }

        Debug.Assert(source, "サウンドタイプが不正です");

        return source;
    }

    private bool IsExist(string cueName, SoundType type)
    {
        CriAtomExAcb acb = null;
        switch (type)
        {
            case SoundType.BGM: acb = CriAtom.GetAcb("BGM"); break;
            case SoundType.SE: acb = CriAtom.GetAcb("SE"); break;
            case SoundType.Voice: acb = CriAtom.GetAcb("Voice"); break;
            default: break;
        }

        return acb.Exists(cueName);
    }

}
